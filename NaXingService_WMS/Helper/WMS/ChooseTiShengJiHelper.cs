using NanXingData_WMS.Dao;
using NanXingService_WMS.Entity.StockEntity;
using NanXingService_WMS.Services.WMS.AGV;
using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Helper.WMS
{
    /// <summary>
    /// 跨楼层任务提升机选择帮助类
    /// </summary>
    public class ChooseTiShengJiHelper
    {

        RedisHelper redisHelper=new RedisHelper();

        string redisKey = "ChooseTSJ";
        //选择与解除都需要加锁

        TiShengJiInfoService _tiShengJiInfoService;

        public ChooseTiShengJiHelper(TiShengJiInfoService tiShengJiInfoService)
        {
            _tiShengJiInfoService = tiShengJiInfoService;
        }

        public TiShengJiInfo ChooseTSJByCount(string missionNo)
        {
            using (redisHelper.CreateLock(redisKey, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1),TimeSpan.FromMilliseconds(200)))
            {
                //第一步：获取Redis中每个提升机任务的数量
                TiShengJiInfo tiShengJiInfo = null;
                List<TiShengJiInfo> tsjList = _tiShengJiInfoService.GetList(u=>u.IsOpen==1);
               
                List<TiShengJiMission> tsjMissions = new List<TiShengJiMission>(tsjList.Count);
                TiShengJiMission tiShengJiMission = null;
                foreach (var item in tsjList)
                {
                    TiShengJiState tiShengJiState = redisHelper.StringGet<TiShengJiState>($"TSJ-State:{item.TsjIp}_{item.TsjPort}");
                    if (tiShengJiState == null)
                        continue;

                    TiShengJiMission tiShengJiMissionTemp = new TiShengJiMission();
                    tiShengJiMissionTemp.TsjName = item.TsjName;

                    var missions = redisHelper.GetSets<string>(item.TsjName);
                    tiShengJiMissionTemp.LieCount = missions.Count();
                    if (tiShengJiMissionTemp.LieCount > 0)
                    {
                        tiShengJiMissionTemp.Lie = new List<string>(tiShengJiMissionTemp.LieCount);
                        foreach (var temp in missions)
                            tiShengJiMissionTemp.Lie.Add(temp.ToString());
                    }
                    tsjMissions.Add(tiShengJiMissionTemp);
                    
                }
                if (tsjMissions.Count == 0)
                    return null;
                //直接排序
                tsjMissions = tsjMissions.OrderBy(u => u.LieCount).ToList();

                tiShengJiMission = tsjMissions.FirstOrDefault();
                tiShengJiInfo = tsjList.FirstOrDefault(u => u.TsjName == tiShengJiMission.TsjName);
                //记录在Redis中
                redisHelper.SetAdd(tiShengJiMission.TsjName, missionNo, TimeSpan.FromMinutes(60));


                return tiShengJiInfo;
            }
        }


        public TiShengJiInfo ChooseTSJByHash(string wareLocaNo,string lieName)
        {
            //第一步：上锁

            using (redisHelper.CreateLock(redisKey, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1),
                     TimeSpan.FromMilliseconds(200)))
            {
                TiShengJiInfo tiShengJiInfo = null;

                List<TiShengJiInfo> tsjList = _tiShengJiInfoService.GetList(u => u.IsOpen == 1);
                List<TiShengJiMission> tsjMissions = new List<TiShengJiMission>(tsjList.Count);
                TiShengJiMission tiShengJiMission = null;
                foreach(var item in tsjList)
                {
                    tiShengJiMission = new TiShengJiMission();
                    tiShengJiMission.TsjName = item.TsjName;
                    
                    var keys = redisHelper.HashKeys(item.TsjName);
                    tiShengJiMission.LieCount = keys.Count();
                    if (tiShengJiMission.LieCount > 0)
                    {
                        tiShengJiMission.Lie = new List<string>(tiShengJiMission.LieCount);
                        foreach (var temp in keys)
                            tiShengJiMission.Lie.Add(temp.ToString());
                    }
                    tsjMissions.Add(tiShengJiMission);
                    tiShengJiMission = null;
                }
                
                //先看有没有同列数据
                tiShengJiMission =tsjMissions.Where(u=>u.LieCount>0 &&u.Lie.Contains(lieName)).FirstOrDefault();
                if (tiShengJiMission == null)
                {
                    //排序，得出
                    tsjMissions = tsjMissions.OrderBy(u => u.LieCount).ToList();
                    tiShengJiMission = tsjMissions.FirstOrDefault();
                }
                
                if (tiShengJiMission != null)
                {
                    tiShengJiInfo = tsjList.FirstOrDefault(u => u.TsjName == tiShengJiMission.TsjName);
                    var redisValue1 = redisHelper.HashGet<List<int>>(tiShengJiMission.TsjName, lieName);
                    string[] arr = wareLocaNo.Split('-');
                    int index = Convert.ToInt32(arr[2]);
                    if (redisValue1 == null)
                        redisValue1 = new List<int>(1);
                    if (!redisValue1.Contains(index))
                    {
                        redisValue1.Add(index);

                        redisHelper.HashSet(tiShengJiMission.TsjName, lieName, 
                            redisValue1,TimeSpan.FromMinutes(60));
                    }
                    return tiShengJiInfo;
                }
                return tiShengJiInfo;

            }
        }

        public bool RemoveMissionInTSJ(string tsjName, string missionNo)
        {
            using (redisHelper.CreateLock(redisKey, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1),
                     TimeSpan.FromMilliseconds(200)))
            {
                try
                {
                    redisHelper.SetDelete(tsjName, missionNo);
                    redisHelper.KeyExpire(tsjName,TimeSpan.FromMinutes(60));
                    return true;
                }
                catch
                {
                    return false;
                }

            }
        }


        public bool RemoveLieInTSJ(string tsjName, string wareLocaNo, string lieName)
        {
            using (redisHelper.CreateLock(redisKey, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1),
                     TimeSpan.FromMilliseconds(200)))
            {
                try
                {
                    string[] arr = wareLocaNo.Split('-');
                    int index = Convert.ToInt32(arr[2]);
                    var redisValue1 = redisHelper.HashGet<List<int>>(tsjName, lieName);
                    if (redisValue1.Contains(index))
                    {
                        redisValue1.Remove(index);
                        if (redisValue1.Count == 0)
                            redisHelper.HashDelete(tsjName, lieName);
                        else
                            redisHelper.HashSet(tsjName, lieName, redisValue1, TimeSpan.FromMinutes(60));
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
                
            }
        }
    }
}
