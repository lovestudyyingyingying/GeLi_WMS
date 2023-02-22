using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity;
using NanXingService_WMS.Entity.CRMEntity;
using NanXingService_WMS.Entity.CRMEntity.CRMAppleNoEntity;
using NanXingService_WMS.Helper.APS;
using NanXingService_WMS.Services.APS;
using NanXingService_WMS.Threads.CRMThreads;
using NanXingService_WMS.Utils.Extensions;
using NanXingService_WMS.Utils.RabbitMQ;
using NanXingService_WMS.Utils.RedisUtils;
//using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using UtilsSharp.Standard;

namespace NanXingService_WMS.Managers
{
    public class CRMPlanManager
    {
        CRMPlanHeadService _crmpHeadService;
        CRMPlanListService _crmpListService;
        ProPlanOrderlistsService _proListService;
        CRMFilesService _filesService;
        CRMApiHelper crmApiHelper;
        RedisHelper redisHelper = new RedisHelper();
        public CRMPlanManager(CRMPlanHeadService crmpHeadService, CRMPlanListService crmpListService,
            ProPlanOrderlistsService proListService, CRMFilesService filesService)
        {
            _crmpHeadService = crmpHeadService;
            _crmpListService = crmpListService;
            _proListService = proListService;
            _filesService = filesService;
            crmApiHelper = new CRMApiHelper();
        }



        #region 查询

        /// <summary>
        /// 显示CRM下推的申请单页面
        /// </summary>
        /// <param name="crmwhereLambda"></param>
        /// <param name="prowhereLambda"></param>
        /// <returns></returns>
        public IQueryable<CRMApplyIndexData> QueryIndex(
            Expression<Func<CRMPlanList, bool>> crmwhereLambda = null,
            Expression<Func<ProPlanOrderlists, bool>> prowhereLambda = null,
            Expression<Func<CRMApplyIndexData, bool>> cIndexWhereLambda = null)
        {
            var CRMListQ = crmwhereLambda == null ? _crmpListService.GetAllQueryable(null)
                : _crmpListService.GetIQueryable(crmwhereLambda);
            var PROListQ = prowhereLambda == null ? _proListService.GetAllQueryable(null)
                : _proListService.GetIQueryable(prowhereLambda);

            var q = from a in CRMListQ
                    join b in PROListQ
                    on a.ID equals (b.CRMPlanList_ID)
                    into ab
                    from abi in ab.DefaultIfEmpty()
                    select new CRMApplyIndexData
                    {
                        ID = a.ID,
                        CRMApplyNo = a.CRMPlanHead.CRMApplyNo,
                        CRMApplyNo_Xuhao = a.CRMApplyNo_Xuhao,
                        ClientName = a.CRMPlanHead.ClientName,
                        ApplicantName = a.CRMPlanHead.ApplicantName,
                        ApplyTime = a.CRMPlanHead.ApplyTime,
                        OrderDate = a.CRMPlanHead.OrderDate,
                        ApplyNoState = a.ApplyNoState,
                        EmergencyDegree = a.EmergencyDegree,
                        ItemNo = a.ItemNo,
                        ItemName = a.ItemName,
                        ApplicantDept = a.CRMPlanHead.ApplicantDept,
                        CRMApplyList_InCode = a.CRMApplyList_InCode,
                        Spec = a.Spec,
                        OrderCount = a.OrderCount,
                        Unit = a.Unit,
                        InventoryCount = a.InventoryCount,
                        Reserve1 = a.CRMPlanHead.Reserve1,
                        Biaozhun = a.Biaozhun,
                        ProductRecipe = a.ProductRecipe,
                        BoxNo = a.BoxNo,
                        BoxName = a.BoxName,
                        BoxRemark = a.BoxRemark,
                        Reserve2 = a.Reserve2,
                        Reserve3 = a.Reserve3,
                        Remark = a.Remark,
                        DeliveryDate = a.DeliveryDate,
                        ProPlanOrderlists = a.ProPlanOrderlists,
                        Reserve9 = a.Reserve9,
                        Reserve10 = a.Reserve10,
                        Reserve11 = a.Reserve11,


                        GiveDept = abi.GiveDept,
                        Ingredients = abi.Ingredients,
                        HalfProState = abi.HalfProState,
                        PlanOrder_XuHao = abi.PlanOrder_XuHao,
                        //产品别名
                        InName = abi.InName,
                        //原料名
                        MaterialItem = abi.MaterialItem,
                        //排产量
                        PcCount = ((int?)abi.PcCount),
                        //排产单位
                        PcUnit = abi.Unit,

                        //批号
                        BatchNo = abi.BatchNo,
                        //排产状态
                        crmListStatus = a.crmListStatus,
                        //排产日期
                        PlanDate = abi.PlanDate,
                        //排产人员
                        Jingbanren = abi.Jingbanren,
                        
                        PlanNewdate = abi.Newdate
                    };
            return q;

        }
        

        /// <summary>
        /// 显示申请单合拼排产单页面
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<CRMApplyIndexData> GetMissionControlIndex
            (DateTime sDate,DateTime eDate)
        {
            return _crmpListService.SelectToQuery<CRMApplyIndexData>(u => u.DeliveryDate>=sDate && u.DeliveryDate<=eDate, u => new CRMApplyIndexData
            {
                ID = u.ID,
                CRMApplyNo = u.CRMPlanHead.CRMApplyNo,
                CRMApplyNo_Xuhao = u.CRMApplyNo_Xuhao,
                ClientName = u.CRMPlanHead.ClientName,
                ApplicantName = u.CRMPlanHead.ApplicantName,
                ApplyTime = u.CRMPlanHead.ApplyTime,
                OrderDate = u.CRMPlanHead.OrderDate,
                ApplyNoState = u.ApplyNoState,
                EmergencyDegree = u.EmergencyDegree,
                ItemNo = u.ItemNo,

                ItemName = u.ItemName,
                //产品别名
                //原料名
                Spec = u.Spec,
                OrderCount = u.OrderCount,
                Unit = u.Unit,
                InventoryCount = u.InventoryCount,
                //排产量
                //排产单位
                //批号
                Biaozhun = u.Biaozhun,
                ProductRecipe = u.ProductRecipe,
                BoxNo = u.BoxNo,
                BoxName = u.BoxName,
                BoxRemark = u.BoxRemark,
                Remark = u.Remark,
                DeliveryDate = u.DeliveryDate,
                
                //排产单号
                //排产状态
                //排产日期
            });
        }

        /// <summary>
        /// 显示申请单合拼排产单页面
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<CRMApplyIndexData> GetMissionControlIndex
            (Expression<Func<CRMPlanList, bool>> whereLambda = null)
        {
            return _crmpListService.SelectToQuery<CRMApplyIndexData>(whereLambda, u => new CRMApplyIndexData
            {
                ID = u.ID,
                CRMApplyNo = u.CRMPlanHead.CRMApplyNo,
                CRMApplyNo_Xuhao = u.CRMApplyNo_Xuhao,
                ClientName = u.CRMPlanHead.ClientName,
                ApplicantName = u.CRMPlanHead.ApplicantName,
                ApplyTime = u.CRMPlanHead.ApplyTime,
                OrderDate = u.CRMPlanHead.OrderDate,
                ApplyNoState = u.ApplyNoState,
                EmergencyDegree = u.EmergencyDegree,
                ItemNo = u.ItemNo,
                Reserve2 = u.Reserve2,
                Reserve3 = u.Reserve3,
                ItemName = u.ItemName,
                //产品别名
                //原料名
                Spec = u.Spec,
                OrderCount = u.OrderCount,
                Unit = u.Unit,
                InventoryCount = u.InventoryCount,
                //排产量
                //排产单位
                //批号
                Biaozhun = u.Biaozhun,
                ProductRecipe = u.ProductRecipe,
                BoxNo = u.BoxNo,
                BoxName = u.BoxName,
                BoxRemark = u.BoxRemark,
                Remark = u.Remark,
                DeliveryDate = u.DeliveryDate,
                ProPlanOrderlists = u.ProPlanOrderlists,
                CRMApplyList_InCode = u.CRMApplyList_InCode,
                //排产单号
                //排产状态
                //排产日期

                YanBanCount = u.Reserve9,
                YanBanUnit = u.Reserve10
            }) ;
        }


        /// <summary>
        /// 根据ID找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CRMPlanList GetList(int id)
        {
            return _crmpListService.FindById(id);
        }
        #endregion
        Random rd = new Random();
        string key = "AddOrUpdateCRM";
        #region 添加
        /// <summary>
        /// 插入或修改CRM数据
        /// </summary>
        /// <param name="crmpPlanList"></param>
        /// <returns></returns>
        public async Task<RunResult<string>> AddOrUpdateCRMApply(CRMPlanWriter temp)
        {
            //先查询与转换，查询转换不加事务，减少事务锁表时间，并添加锁
           

            if (temp.CRMPlanLists != null && temp.CRMPlanLists.Count == 0)
                throw (new Exception($"订单明细不能为空"));

            DateTime dtt2 = DateTime.Now;

            //第一步：判断是否含有相同的申请单号，如果无则新增
            //var task=_crmpHeadService.GetListAsync(u => u.CRMApplyNo == temp.CRMApplyNo, false,
            //    DbMainSlave.Master);
           
            //int i = new Random(Guid.NewGuid().GetHashCode()).Next(0, 35);
            //Debug.WriteLine("随机数：" + i);
            //var qList = await task;
           
                //= qList[i];
            var task=_crmpHeadService.FirstOrDefaultAsync(
                u => u.CRMApplyNo == temp.CRMApplyNo,false,DbMainSlave.Master);
            CRMPlanHead crmpHeader = await task;
            
            bool isNewOrder = true;
            if (crmpHeader != null)
            {
                CRMPlanHead crmpHeaderNew = (CRMPlanHead)_crmpHeadService.ParseValue(temp, typeof(CRMPlanHead));
                PropertyDescriptorCollection writerPdc = TypeDescriptor.GetProperties(typeof(CRMPlanWriter));

                PropertyDescriptorCollection headerPdc = TypeDescriptor.GetProperties(typeof(CRMPlanHead));
                PropertyDescriptor pd2 = null;
                foreach (PropertyDescriptor pd in writerPdc)
                {
                    Type proType = pd.PropertyType.Name == "Nullable`1" ? pd.PropertyType.GenericTypeArguments[0] : pd.PropertyType;
                    //如果isICollection为false则判断是否包含
                    bool ret = (!proType.FullName.Contains("Collection") && proType.FullName.StartsWith("System"));
                    if (headerPdc.Contains(pd) && ret)
                    {
                        object value = pd.GetValue(temp);
                        pd2 = headerPdc.Find(pd.Name, true);
                        if (pd2 != null)
                            pd2.SetValue(crmpHeader, value);
                        pd2 = null;
                    }
                }
                isNewOrder = false;
            }
            else
            {
                crmpHeader = (CRMPlanHead)_crmpHeadService.ParseValue(temp, typeof(CRMPlanHead));
            }

            //第二步：如果有相同的申请单号，则判断明细单是否新增，然后调用AddOrUpdate-

            //bool isNewOrder = crmpHeader.ID == 0;

            List<CRMPlanList> cmplLists = new List<CRMPlanList>(temp.CRMPlanLists.Count);
            foreach (CRMPlanListWriter cpwTemp in temp.CRMPlanLists)
            {
                if (cpwTemp.DeliveryDate == null)
                    throw (new Exception($"订单 {cpwTemp.CRMApplyNo_Xuhao} 交付日期不能为空"));
                cpwTemp.ParseKgCount();
                CRMPlanList crmpList = (CRMPlanList)_crmpListService.ParseValue(cpwTemp, typeof(CRMPlanList));

                crmpList.ConvertRate = cpwTemp.Reserve1;
                //crmpList.CRMPlanHead_Id = crmpHeader.ID;
                //dt = _crmpListService.ParseInDataTable(dt, crmpList);
                cmplLists.Add(crmpList);
            }


            //Logger.Default.Process(new Log(LevelType.Info, 
            //    "转换用时：" + (DateTime.Now - dtt2).TotalSeconds));
            using (redisHelper.CreateLock(key, TimeSpan.FromSeconds(300), TimeSpan.FromSeconds(20), TimeSpan.FromMilliseconds(500)))
            {
                Logger.Default.Process(new Log(LevelType.Info, 
                    "线程ID:" + Thread.CurrentThread.ManagedThreadId.ToString()));
                dtt2 = DateTime.Now;
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        _crmpHeadService.InsertOrUpdate(crmpHeader);
                        _crmpHeadService.SaveChanges();
                        Logger.Default.Process(new Log(LevelType.Info, 
                            "header_ID:" + crmpHeader.ID+";CRMApplyNo:"+ crmpHeader.CRMApplyNo));

                        string state = "未排产";

                        if (isNewOrder)
                        {
                            foreach (CRMPlanList list in cmplLists)
                            {
                                list.crmListStatus = state;
                                list.CRMPlanHead_Id = crmpHeader.ID;
                            }
                            var listQ = cmplLists.AsQueryable();
                            _crmpListService.InsertBulk(listQ);
                            _crmpListService.SaveChanges();
                        }
                        //如果不是新增申请单
                        else
                        {
                            var listQ = crmpHeader.CRMPlanList;
                            //= oldCmplLists.AsQueryable();
                            List<CRMPlanList> addCmplLists = new List<CRMPlanList>();
                            List<CRMPlanList> editCmplLists = new List<CRMPlanList>();

                            foreach (var newlist in cmplLists)
                            {
                                if (listQ.Any(u => 
                                u.CRMApplyList_InCode == newlist.CRMApplyList_InCode))
                                {
                                    var old = listQ.FirstOrDefault(u => 
                                    u.CRMApplyList_InCode == newlist.CRMApplyList_InCode);
                                    newlist.ID = old.ID;
                                    newlist.crmListStatus = old.crmListStatus;
                                    newlist.CRMPlanHead_Id = crmpHeader.ID;
                                    editCmplLists.Add(newlist);
                                }
                                else
                                {
                                    newlist.CRMPlanHead_Id = crmpHeader.ID;
                                    newlist.crmListStatus = state;
                                    addCmplLists.Add(newlist);
                                }
                            }

                            DataTable addDt = _crmpListService.ConvertToDataTable(addCmplLists);
                            DataTable editDt = _crmpListService.ConvertToDataTable(editCmplLists);
                            string exp = string.Empty;
                            var ret = _crmpListService.BatchInsertOrUpdate(addDt, editDt, "CRMPlanList", out exp);

                            if (!ret)
                                throw (new Exception(exp));
                        }

                        //Debug.WriteLine("预插入子数据：" + (DateTime.Now - dtt1  ).TotalMilliseconds);
                        Logger.Default.Process(new Log(LevelType.Info, 
                           $"接收CRM订单数据：{crmpHeader.CRMApplyNo};事务用时：" + (DateTime.Now - dtt2).TotalSeconds));
                        Logger.Default.Process(new Log(LevelType.Info, $"成功：{crmpHeader.CRMApplyNo}"));
                        tran.Complete();
                        return RunResult<string>.True();
                    }
                    catch (Exception ex)
                    {
                        Logger.Default.Process(new Log(LevelType.Error, ex.ToString()));
                        //Debug.WriteLine(ex.ToString());
                        Logger.Default.Process(new Log(LevelType.Info,
                            $"接收CRM订单数据：{crmpHeader.CRMApplyNo};失败用时：" + (DateTime.Now - dtt2).TotalSeconds));
                        
                        return RunResult<string>.False(ex.ToString());
                    }
                    //finally
                    //{
                    //    //tran.Dispose();
                    //}
                }
            }

        }

        #endregion

        #region 修改

        /// <summary>
        /// 缓存中修改数据，但还没保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public void ChangeCRMStatue(int id, string status)
        {
            CRMPlanList cpl = _crmpListService.FindById(id, DbMainSlave.Master);
            cpl.crmListStatus = status;
            _crmpListService.Update(cpl, new List<string>(1) { "crmListStatus" });
        }

        public void ChangeCRMStatueOverApi(long[] ids, string status, string reason)
        {
            //long[] idsArr = Array.ConvertAll(ids, s => (long)s);

            List<CRMPlanList> planLists = _crmpListService.GetList(u => ids.Contains(u.ID)
                ,false,DbMainSlave.Master);
            List<long> list = new List<long>();
            foreach(CRMPlanList temp in planLists)
            {
                if (string.IsNullOrEmpty(temp.CRMApplyList_InCode))
                    continue;

                
                //CRMApplyState applyState = new CRMApplyState();

                //applyState.crm_ID = temp.CRMApplyList_InCode;
                //applyState.pcState = status;
                //applyState.pcReason = reason;

                //CRMWriteEntity writeEntity = new CRMWriteEntity(WriteTypeEnum.CRMChangeState.GetEnumDescription(), applyState);
               
                list.Add(temp.ID);
            }
            
            _crmpListService.UpdateByPlus(u => ids.Contains(u.ID), u => new CRMPlanList { crmListStatus = status,Reserve7=reason });
            RabbitMQUtils.Send(CRMReturnWriteThread.queueName_CRMReturnWrite, list);
        }

       
        #endregion

        #region 其他
        /// <summary>
        /// IQueryable转换成DataTable
        /// </summary>
        /// <param name="q">IQueryable集合</param>
        /// <returns>DataTable</returns>
        public DataTable ConvertToDataTable(IQueryable<CRMApplyIndexData> q)
        {
            DataTable dt = _crmpListService.ClassToDataTable(typeof(CRMApplyIndexData));
            dt = _crmpListService.ParseInDataTable(dt, q);
            return dt;
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 异步保存CRM文件
        /// </summary>
        /// <param name="request">上传文件请求</param>
        /// <returns>结果</returns>
        /// <summary>
        /// 异步保存CRM文件
        /// </summary>
        /// <param name="request">上传文件请求</param>
        /// <returns>结果</returns>
        public async Task<bool> SaveCRMFile(string CRMID, string fileName, string request)
        {
            //保存到文件夹，并保存在数据库中
            try
            {
                if (request.Length > 0)
                {
                    var savePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + $"uploadImgs/{CRMID}/";
                    if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
                    CRMFiles files = null;

                    string path = savePath + DateTime.Now.Ticks.ToString() + "-" + fileName;
                    path = path.Replace("\\", "/");
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(request));
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                    byte[] b = stream.ToArray();
                    await fs.WriteAsync(b, 0, b.Length);
                    fs.Close();
                    if (!_filesService.Any(u => u.CRMFilePath == path && u.CRMID == CRMID))
                    {
                        files = new CRMFiles();
                        files.CRMID = CRMID;
                        files.CRMFilePath = path;
                        files.UpdateTime = DateTime.Now;
                        _filesService.Insert(files);
                        _filesService.SaveChanges();
                        Logger.Default.Process(new Log(LevelType.Info,$"保存文件成功"));
                    }
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logger.Default.Process(new Log(LevelType.Error,
                    $"保存文件失败：{ex}"));

                Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
        public static bool Write(string s, BinaryWriter bw)
        {
            try
            {
                char a;
                byte[] bits;
                sbyte[] bitsb;
                for (int i = 0; i < s.Length; i++)
                {
                    a = s[i];
                    bits = BitConverter.GetBytes(a);
                    byte[] temsb = { bits[0] };
                    bitsb = ToJavaBytes(temsb);
                    WriteSBytes(bitsb, bw);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public static sbyte[] ToJavaBytes(byte[] bytes)
        {
            int len = bytes.Length;
            sbyte[] sbs = new sbyte[len];
            for (int i = 0; i < len; i++)
            {
                var b = bytes[i];
                if (b > 127)
                    sbs[len - 1 - i] = (sbyte)(b - 256);
                else
                    sbs[len - 1 - i] = (sbyte)b;

            }
            return sbs;
        }
        public string ChangeRemark(string remark, string reserve2, string reserve3)
        {
            if (string.IsNullOrEmpty(reserve2) && !string.IsNullOrEmpty(reserve3))
            {
                remark = "产期限制" + reserve3 + "天;\n" + remark;
            }
            else if (string.IsNullOrEmpty(reserve3) && !string.IsNullOrEmpty(reserve2))
            {
                remark = "最早日期" + reserve2 + ";\n" + remark;
            }
            else if (!string.IsNullOrEmpty(reserve3) && !string.IsNullOrEmpty(reserve2))
            {
                remark = "最早日期" + reserve2 + ";\n" + "产期限制" + reserve3 + "天;\n" + remark;
            }
            return remark;
        }
        public static bool WriteSBytes(sbyte[] sbytes, BinaryWriter bw)
        {
            try
            {
                int len = sbytes.Length;
                for (int i = 0; i < len; i++)
                {
                    sbyte b = sbytes[i];
                    bw.Write(b);
                }
                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 转换Byte[]到String 默认采用UTF-8
        /// </summary>
        /// <param name="bytes">数据流的BYTE数组</param>
        /// <param name="str">最终转换结果字符串</param>
        /// <param name="indexStart">String的在数据流中的起始位置</param>
        /// <returns>下一个数据类型在数据流中的起始位置</returns>
        private int CoverJavaByteToString(byte[] bytes, ref string str, int indexStart)
        {
            byte[] uidLen = new byte[2];
            Array.Copy(bytes, indexStart, uidLen, 0, 2);
            ushort len = CoverJavaByteToUnshort(uidLen);
            str = Encoding.UTF8.GetString(bytes, 2 + indexStart, len);
            return str.Length + 2 + indexStart;
        }
        private ushort CoverJavaByteToUnshort(byte[] bytes)
        {
            ushort r = 0;
            r |= (ushort)((bytes[0] & 0xff) << 8);
            r |= (ushort)(bytes[1] & 0xff);
            return r;
        }
        /// <summary>
        /// 二进制样式的字符串转byte数组
        /// </summary>
        /// <param name="binaryStr">二进制样式的字符串</param>
        /// <returns></returns>
        private byte[] BinaryStr2ByteArray(string binaryStr)
        {
            if (string.IsNullOrEmpty(binaryStr)) binaryStr = string.Empty;

            List<byte> byte_List = new List<byte>();
            var strL = binaryStr.Length;
            if (strL == 0)
                byte_List.Add(0);
            else if (strL > 0 && strL <= 4)
                byte_List.Add(Convert.ToByte(binaryStr, 2));
            else
            {
                var tempStr = string.Empty;
                for (var i = strL; i > 0; i = i - 4)
                {
                    if (i - 4 > 0)
                        tempStr = binaryStr.Substring(i - 4, 4);
                    else
                        tempStr = binaryStr.Substring(0, i);
                    byte_List.Add(Convert.ToByte(tempStr, 2));
                }
            }

            byte_List.Reverse();
            return byte_List.ToArray();
        }

        public static byte[] HexStringToBytes(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr))
            {
                return new byte[0];
            }

            if (hexStr.StartsWith("0x"))
            {
                hexStr = hexStr.Remove(0, 2);
            }

            var count = hexStr.Length;

            if (count % 2 == 1)
            {
                throw new ArgumentException("Invalid length of bytes:" + count);
            }

            var byteCount = count / 2;
            var result = new byte[byteCount];
            for (int ii = 0; ii < byteCount; ++ii)
            {
                var tempBytes = Byte.Parse(hexStr.Substring(2 * ii, 2), System.Globalization.NumberStyles.HexNumber);
                result[ii] = tempBytes;
            }

            return result;
        }

        /// <summary>
        /// 将二进制转成字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] ToBytes(string orgStr)
        {
            //undefined
            byte[] result = null;

            {
                //undefined
                #region 网上的错误算法
                //var binaryBits = orgStr.ToCharArray().Select(i => (byte)(i - 48)).ToArray();
                //var binarySize = orgStr.Length;
                //result = new byte[binarySize];

                //Array.Copy(binaryBits, result, binarySize);
                #endregion

                if (orgStr.Length > 8)
                {
                    //undefined
                    ///get the lenght of byte array
                    int len = orgStr.Length % 8 == 0 ? orgStr.Length / 8 : (orgStr.Length / 8) + 1;
                    ///initial the result with the length calculated previously
                    result = new byte[len];
                    /// define a varibale which will be used to split the string
                    ///complement the length of the orgianl string, which can be dividened by 8

                    /// Assign the original string to another temp variable in case of confused with the original one
                    /// This temporary string will be renewed every time after getting the result of a subString
                    string tempStr = orgStr.PadLeft((8 - orgStr.Length % 8) + orgStr.Length, '0');
                    for (int i = 0; i < len; i++)
                    {
                        //undefined
                        string binStr;

                        binStr = tempStr.Substring(i * 0, 8);
                        tempStr = tempStr.Substring(8, tempStr.Length - 8);

                        result[i] = Convert.ToByte(binStr, 2);
                    }
                }
                else
                {
                    //undefined
                    result = new byte[1];
                    result[0] = Convert.ToByte(orgStr, 2);
                }
            }

            return result;
        }

        /// <summary>
        /// 将字符串转成二进制
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string bianma(string s)
        {
            byte[] data = Encoding.Unicode.GetBytes(s);
            StringBuilder result = new StringBuilder(data.Length * 8);

            foreach (byte b in data)
            {
                result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return result.ToString();
        }

        #endregion
        
       
    }
}
