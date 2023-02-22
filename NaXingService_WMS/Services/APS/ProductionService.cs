using NanXingData_WMS.Dao;
using NanXingData_WMS.DaoUtils;
using NanXingService_WMS.Entity.ProductEntity;
using NanXingService_WMS.Entity;
using NanXingService_WMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Rest.Azure.OData;
using HslCommunication.BasicFramework;

namespace NanXingService_WMS.Services.APS
{
    public class ProductionService:DbBase<Production>
    {
        public RunResult<IQueryable<ProKanBanChart>> GetJiTaiBigBoxHourReport(DateTime sPlanTime, DateTime ePlanTime)
        {
            var dateQuery = DateUtils.GetAllHours(sPlanTime, ePlanTime).AsQueryable();
            string[] arr = { "2L大包装车间", "3L大包装车间"};
            var cheJianArr = from a in dateQuery
                             from b in arr.AsQueryable()
                             select new
                             {
                                 Hour = a.Hour,
                                 CheJianName = b
                             };
           
            var proCountQuery = GetList(u => u.prodate>sPlanTime && u.prodate<ePlanTime)
                .GroupBy(u=> new { u.position, u.prodate.Hour}).Select(u=>new
                {
                    u.Key.position,
                    u.Key.Hour,
                    proCount=u.Count()
                }).AsQueryable();
            var query = (from a in dateQuery
                         join b in proCountQuery
                         on new { a.Hour, position="2L大包装车间" } equals new { b.Hour,b.position }
                         into stationRealTimeInfo1   //划重点划重点
                         from str1 in stationRealTimeInfo1.DefaultIfEmpty()
                         join c in proCountQuery
                         on new { a.Hour, position = "3L大包装车间" } equals new { c.Hour, c.position }
                         into stationRealTimeInfo2   //划重点划重点
                         from str2 in stationRealTimeInfo2.DefaultIfEmpty()
                         select new 
                     {
                         a.Hour,
                         ProCount2 = str1 == null ? 0 : str1.proCount,
                          ProCount3 = str2 == null ? 0 : str2.proCount,
                     }).ToList();
            ProKanBanChart proKanBanChart = new ProKanBanChart("大包装车间产量");
            for (int i = 0; i< query.Count; i++)
            {
                proKanBanChart.XData.Add(query[i].Hour.ToString());
                proKanBanChart.YData1.Add(
                    query[i].Hour<DateTime.Now.Hour ? query[i].ProCount2.ToString() : string.Empty);
                proKanBanChart.YData2.Add(
                    query[i].Hour<DateTime.Now.Hour ? query[i].ProCount3.ToString() : string.Empty);
            }
            return RunResult<IQueryable<ProKanBanChart>>.True(new List<ProKanBanChart> (1){ proKanBanChart }.AsQueryable());
        }

        public RunResult<IQueryable<ProKanBanChart>> GetJiTaiBigBoxDateReport(DateTime sPlanTime, DateTime ePlanTime)
        {
            var dateQuery = DateUtils.GetAllDates(sPlanTime, ePlanTime).AsQueryable();
            
           
            var proCountQuery = GetList(u => u.prodate>sPlanTime && u.prodate<=ePlanTime)
                .GroupBy(u =>  u.prodate.Date ).Select(u => new
                {
                    Date=u.Key,
                    proCount = u.Count()
                }).AsQueryable();
            var query = (from a in dateQuery
                         join b in proCountQuery
                         on a.Date equals b.Date
                         into stationRealTimeInfo1   //划重点划重点
                         from str1 in stationRealTimeInfo1.DefaultIfEmpty()
                        
                         select new
                         {
                             a.Date,
                             ProCount = str1 == null ? 0 : str1.proCount,
                         }).ToList();
            ProKanBanChart proKanBanChart = new ProKanBanChart("大包装车间产量");
            for (int i = 0; i< query.Count; i++)
            {
                proKanBanChart.XData.Add(query[i].Date.ToString("MM-dd"));
                proKanBanChart.YData1.Add(query[i].ProCount.ToString());
                
            }
            return RunResult<IQueryable<ProKanBanChart>>.True(new List<ProKanBanChart>(1) { proKanBanChart }.AsQueryable());
        }

    }
}
