using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity;
using NanXingService_WMS.Utils.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Managers
{
    public class MesConsolHelper : DbBase
    {

        DbBase<ProductOrderlists> dbBaseorderList = new DbBase<ProductOrderlists>();
        DbBase<ProductUploadHistory> dbbaseUploadHistory = new DbBase<ProductUploadHistory>();
        DbBase<ProductOrderheaders> dbBaseHeard = new DbBase<ProductOrderheaders>();
        DbBase<PrintRecordLists> dbBasePrintrecord = new DbBase<PrintRecordLists>();
        List<SqlParameter> sqlParms = new List<SqlParameter>(1);

        public IQueryable<PrintRecordListsDto> GetBiaoQianListResult(Expression<Func<PrintRecordLists, bool>> expression)
        {
            var printRecordLists = dbBasePrintrecord.GetList(expression, false, DbMainSlave.Master);
            var printRecordListsDto = printRecordLists.GroupBy(p => new { p.BarcodeNumber, p.PrintOperatorCode, p.PrintRecordState, p.ItemName, p.ReceiverNo, p.InWarehouseCode }).Select(u => new PrintRecordListsDto
            {
                ID = u.Select(a => a.ID).FirstOrDefault(),
                BarcodeNumber = u.Key.BarcodeNumber,
                PrintOperator = u.Select(a => a.PrintOperator).FirstOrDefault(),
                PrintOperatorCode = u.Key.PrintOperatorCode,
                PrintDate = u.Select(a => a.PrintDate).FirstOrDefault(),
                ItemName = u.Key.ItemName,
                ReceiverNo = u.Key.ReceiverNo,
                PrintRecordState = u.Key.PrintRecordState??"",
                ProCount = u.Sum(a => a.ProCount),
                PrintRecordListsIDS = string.Join(",", u.Select(a => a.ID)),
                InWarehouseCode = u.Key.InWarehouseCode,
                Remark = u.Select(a => a.Remark).FirstOrDefault(),
                ReceiverName = u.Select(a => a.ReceiverName).FirstOrDefault(),
                ProductOrder_XuHao = u.Select(a => a.ProductOrder_XuHao).FirstOrDefault(),
                UploadBatch = u.Select(a => a.UploadBatch).FirstOrDefault(),
                PackDate = u.Select(a => a.PackDate).FirstOrDefault(),
                InwarehouseNo = u.Select(a => a.InwarehouseRecordLists).FirstOrDefault() == null ? null : u.Select(a => a.InwarehouseRecordLists).FirstOrDefault().Select(b => b.DeliveryMan).FirstOrDefault()??"",
                InWareHousePeople = u.Select(a => a.InwarehouseRecordLists).FirstOrDefault() == null ? null : u.Select(a => a.InwarehouseRecordLists).FirstOrDefault().Select(b => b.DeliveryNo).FirstOrDefault() ?? "",
                Reserve2 = u.Select(a => a.Reserve2).FirstOrDefault() ?? "",
            }).OrderByDescending(u => u.PrintDate).AsQueryable();
            //printRecordListsDto = printRecordListsDto.OrderBy(u => u.ProductOrder_XuHao).ThenBy(u => u.BarcodeNumber).AsQueryable();
            return printRecordListsDto;
        }


        public bool DeleteBiaoQian(List<int> ids)
        {
            try
            {
                //对id进行查找   //并将报工记录修改回写CRM
                var printrecordList = dbBasePrintrecord.GetList(u => ids.Contains(u.ID), false, DbMainSlave.Master).Where(u => u.PrintRecordState != "申请入库").ToList();
                printrecordList.ForEach(item =>
                {

                    //同一批上报的所有单据
                    var productUploadHistorys = dbbaseUploadHistory.GetList(u => u.Reserve3 == item.Reserve1, false, DbMainSlave.Master);//根据默认应该id排序，得到所有的报工记录

                    //拿到标签数量
                    var biaoQianNum = item.ProCount ?? 0;
                    //循环前的剩余数量
                    decimal sulpsNum = item.ProCount ?? 0;
                    //逆序删除
                    for (int i = 0; i < productUploadHistorys.Count; i++)
                    {

                        int ii = productUploadHistorys.Count - 1 - i;
                        //拿到上报记录的数量
                        var Uploadnum = productUploadHistorys[ii].ProCount;
                        //标签数量减剩余数量
                        sulpsNum = sulpsNum - Uploadnum;
                        if (sulpsNum > 0)
                        {
                            productUploadHistorys[ii].ProCount = 0;

                        }
                        else
                        {
                            productUploadHistorys[ii].ProCount = -sulpsNum;
                        }

                        productUploadHistorys[ii].Newdate = DateTime.Now;
                        if (productUploadHistorys[ii].ProCount == 0)
                        {

                            productUploadHistorys[ii].Reserve2 = "已删除";
                        }
                        string xuhao = productUploadHistorys[ii].ProductOrder_XuHao;
                        //操作明细记录并回写CRM
                        var productOrderlists = dbBaseorderList.GetList(u => u.ProductOrder_XuHao == xuhao, false, DbMainSlave.Master).FirstOrDefault();
                        if (item.Reserve2 == null)
                        {
                            productOrderlists.ProCount -= (Uploadnum - productUploadHistorys[ii].ProCount);
                        }
                        else
                        {
                            productOrderlists.TemplateReportedNum -= (Uploadnum - productUploadHistorys[ii].ProCount);

                        }
                        productOrderlists.Newdate = DateTime.Now;
                        dbBaseorderList.Update(productOrderlists);
                        dbBaseorderList.SaveChanges();
                        //回写CRM
                        try
                        {
                            if (productOrderlists.ProPlanOrderlists.crmPlanList != null)
                                WriteBackCRM(productOrderlists.ProPlanOrderlists.crmPlanList.ID);
                        }
                        catch (Exception e)
                        {
                            Logger.Default.Process(new Log(LevelType.Error, e.ToString()));

                        }

                        if (sulpsNum <= 0)
                        {
                            break;
                        }
                    }

                    //item.PrintRecordState = "已删除";
                    ////item.ProductUploadHistory.IsPrint = false;
                    //productOrder.ProCount -= item.ProCount;
                    //productOrder.Moddate = DateTime.Now;
                    //item.ProductUploadHistory.ProCount = 0;
                    //item.ProductUploadHistory.Reserve2 = "已删除";
                    //item.ProductUploadHistory_ID = null;
                    //dbBaseproductOrder.Update(productOrder);
                    //dbBaseproductOrder.SaveChanges();
                    //FormOperateHelper.WriteBackCRM(productOrder.ProPlanOrderlists.crmPlanList.ID);
                    dbbaseUploadHistory.UpdateAll(productUploadHistorys);
                    dbbaseUploadHistory.SaveChanges();
                    item.PrintRecordState = "已删除";
                }
                );
                dbBasePrintrecord.UpdateAll(printrecordList);
                dbBasePrintrecord.SaveChanges();
                return true;
            }
            catch
            {
                Logger.Default.Process(new Log(LevelType.Error, "删除标签失败"));
                return false;
            }


        }

        /// <summary>
        /// 上传记录的流水
        /// </summary>
        /// <returns></returns>
        public string GetReportLiuShui()
        {
            sqlParms.Clear();
            sqlParms.Add(new SqlParameter("@MaintainCate", "UploadBatc"));
            return QueryOne<string>("exec GetSeq_3 @MaintainCate", sqlParms, DbMainSlave.Master);
        }

        /// <summary>
        /// 报工并回写CRM
        /// </summary>
        /// <param name="productOrderNo">生产头单号</param>
        /// <param name="reportNum">上报数量</param>
        /// <param name="BaoGongType">报工类型填写:控制台/小程序/控制台样板</param>
        /// <param name="shengChanPiHao">生产批号（带车间）</param>
        /// <param name="reportNo">上报人编号</param>
        /// <param name="reprotName">上报人名字</param>
        /// <param name="consolNo">控制台号，小程序不填</param>
        public void ReprotAndWriteBackCRM(string productOrderNo, decimal reportNum, string BaoGongType, string shengChanPiHao, string reportNo, string reprotName, string consolNo = null)
        {

            List<ProductUploadHistory> productUploadsList = new List<ProductUploadHistory>();

            var productHead = dbBaseHeard.GetList(u => u.ProductOrderNo == productOrderNo, false, DbMainSlave.Master).FirstOrDefault();
            if (productHead == null)
            {

                return;

                //}
            }
            //productHead.ProductOrder_State = "生产中";
            //拿到它下面的所有明细
            var productorderListAll = productHead.ProductOrderlists;
            var timePiCi = DateTime.Now.ToString("yyyyMMddHHmmss");
            //得到所有的报工数量
            decimal shangBaoNum = reportNum;

            for (int i = 0; i < productorderListAll.Count; i++)
            {
                if (shangBaoNum <= 0)
                {
                    break;
                }
                var supNum = (productorderListAll[i].PcCount ?? 0) - (productorderListAll[i].ProCount ?? 0);
                if (supNum <= 0 && i != productorderListAll.Count - 1)
                {
                    continue;
                }
                decimal proCount = 0;
                //总报工数量大于等于这条的剩余数量,则取这条记录的数量或者已经是随后一次循环了
                if (shangBaoNum >= supNum && i != productorderListAll.Count - 1)
                {
                    proCount = supNum;

                }
                else//总报工只够这条明细
                {
                    proCount = shangBaoNum;
                }
                shangBaoNum -= supNum;

                ProductUploadHistory productUploadHistory = new ProductUploadHistory();
                productUploadHistory.ModUser = reprotName;
                DateTime dateTime = DateTime.Now;
                productUploadHistory.Newdate = dateTime;
                productUploadHistory.Moddate = DateTime.Now;
                productUploadHistory.ProductOrder_XuHao = productorderListAll[i].ProductOrder_XuHao;
                productUploadHistory.ItemName = productorderListAll[0].ItemName;
                productUploadHistory.Spec = productorderListAll[0].Spec;
                productUploadHistory.Unit = productorderListAll[0].Unit;
                productUploadHistory.ProCount = proCount;
                productUploadHistory.Chejianclass = productorderListAll[0].Chejianclass;
                productUploadHistory.BaoGongType = BaoGongType;
                productUploadHistory.UploadUser = reportNo;
                productUploadHistory.Reserve1 = shengChanPiHao;
                productUploadHistory.ConsoleNo = consolNo;
                productUploadHistory.IsPrint = true;
                productUploadHistory.Reserve3 = timePiCi;

                productUploadHistory.LiuShuiHao = GetReportLiuShui();
                productUploadHistory.UploadBatch = shengChanPiHao.Substring(0, 8) + "-" + productUploadHistory.LiuShuiHao;
                decimal oldnum = productorderListAll[i].ProCount ?? 0;
                //更新上报记录的数量
                productorderListAll[i].ProCount = oldnum + proCount;
                productorderListAll[i].Newdate = dateTime;
                productorderListAll[i].BatchNo = shengChanPiHao.Substring(0, 8) + "-" + productUploadHistory.LiuShuiHao;
                //更新上报记录
                //productUploadHistory = UpdateUploadHistoryAndGetPrintRecord(productUploadHistory, productOrderlist);
                productUploadsList.Add(productUploadHistory);

            }
            //保存订单信息
            //productHead.ProductOrderlists = productorderListAll;
            dbBaseorderList.UpdateAll(productorderListAll);
            dbBaseorderList.SaveChanges();
            dbbaseUploadHistory.InsertRange(productUploadsList);
            dbbaseUploadHistory.SaveChanges();


            foreach (var item in productorderListAll)
            {
                if (item.ProPlanOrderlists.crmPlanList != null)
                    WriteBackCRM(item.ProPlanOrderlists.crmPlanList.ID);
            }


        }

        //public void GetPrintRecordLists()
        //{
        //    #region 生成打印标签信息
        //    List<PrintRecordLists> printList = new List<PrintRecordLists>();
        //    //标签数量（张）
        //    int biaoQianNum = (int.Parse(numProcount.Num.ToString()) % int.Parse(numSingleNum.Num.ToString()) == 0 ? (int.Parse(numProcount.Num.ToString()) / int.Parse(numSingleNum.Num.ToString())) : (int.Parse(numProcount.Num.ToString()) / int.Parse(numSingleNum.Num.ToString()) + 1));
        //    List<PrintItem> printItems = new List<PrintItem>();
        //    //var printRecord = productUploadsList[0].PrintRecordLists.FirstOrDefault();
        //    for (int i = 0; i < biaoQianNum; i++)
        //    {

        //        decimal num;
        //        if (i == biaoQianNum - 1 && (numProcount.Num % numSingleNum.Num != 0))
        //        {
        //            num = numProcount.Num % numSingleNum.Num;
        //        }
        //        else
        //        {
        //            num = numSingleNum.Num;
        //        }
        //        var printRecord = GetPrintRecordBindWithHead(productHead, productUploadsList[0], num);
        //        var printItem = new PrintItem
        //        {
        //            Barcode = printRecord.BarcodeNumber,
        //            BiaoQianNo = FormOperateHelper.GetBarCodeUrl(printRecord.BarcodeNumber),
        //            Itemno = printRecord.ItemNo + "\r\n" + printRecord.CRMID,
        //            ItemName = printRecord.ItemName,
        //            ProCount = num.ToString(),
        //            UpLoadBatch = printRecord.UploadBatch,
        //            Spec = printRecord.Spec,
        //            Unit = printRecord.Unit,
        //            OrderSource = printRecord.Channel,
        //            BoxNo = printRecord.BoxNo,
        //            CheJianClass = printRecord.CheJianClass,
        //            remark = tbxRemark.Text,
        //            BiaoZhun = printRecord.ProStandard,
        //            DeliveryMan = Program.username + GetChineseName(),
        //            ReceiverName = tbxUserName.Text + tbxChineseName.Text,
        //            ProductRecipe = printRecord.ProductRecipe,
        //            Moddate = printRecord.UploadBatch.Substring(0, 8),
        //        };

        //        printItems.Add(printItem);
        //        printList.Add(printRecord);
        //    }

        //    #endregion
        //}

        public void WriteBackCRM(long CRMId)
        {
            try
            {
                RabbitMQUtils.Send("CRMReturnWrite", CRMId);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());


            }
        }


    }
}
