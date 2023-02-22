using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Utils;
using NanXingService_WMS.Utils.RedisUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Services
{
    public class UserService 
    {
        RedisHelper redisHelper = new RedisHelper();
        DbBase<Users> userDao = new DbBase<Users>();
        /// <summary>
        /// 登陆并获取权限
        /// </summary>
        /// <param name="name">登录名</param>
        /// <param name="pass">登录密码</param>
        /// <param name="roles">权限集合</param>
        /// <returns>是否成功</returns>
        public bool DoLogin(string name, string pass, ref List<string> roles)
        {
            string redis_Key = $"pass:{name}";
            string passStr = string.Empty;
            if (redisHelper.KeyExists(redis_Key))
            {
               passStr= redisHelper.StringGet(redis_Key);
            }
            else
            {
                using (var locker=redisHelper.CreateLock(redis_Key))
                {
                    if (redisHelper.KeyExists(redis_Key))
                    {
                        passStr = redisHelper.StringGet(redis_Key);
                    }
                    else
                    {
                        Users user = userDao.GetIQueryable(u => u.Name == name).FirstOrDefault();
                        if (user != null)
                        {
                            passStr = user.Password;
                            redisHelper.StringSet(redis_Key, passStr, TimeSpan.FromMinutes(3));
                        }
                    }
                }
            }

            if (PasswordUtil.ComparePasswords(passStr, pass))
            {
                //roles = GetRolePowerNames(user);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前登录用户拥有的全部权限列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<string> GetRolePowerNames(Users user)
        {
            List<string> rolePowerNames = new List<string>();
            foreach (Roles temp in user.Roles)
            {
                foreach (Powers temp2 in temp.Powers)
                {
                    if (!rolePowerNames.Contains(temp2.Name))
                    {
                        rolePowerNames.Add(temp2.Name);
                    }
                }
            }
            return rolePowerNames;
        }

        /// <summary>
        /// 根据登录名获取用户数据
        /// </summary>
        /// <param name="name">登录名</param>
        /// <returns>用户数据</returns>
        public Users GetByName(string name)
        {
            return userDao.GetList(u=>u.Name==name,false,DbMainSlave.Slave,u=>u.Name).FirstOrDefault();

        }

    }
}
