using GeLiData_WMS;
using GeLiData_WMSUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeLiService_WMS.Services.WMS.AGV
{
    public class TiShengJiStateService: DbBase<TiShengJiState>
    {
        //DbBase<TiShengJiState> tishengjiDao = new DbBase<TiShengJiState>();
        DataTable dt = null;
        string tableName = "TiShengJiState";
        #region 查询
        public List<TiShengJiState> GetToday()
        {
            DateTime dt = DateTime.Now.AddMinutes(-30);
            return GetIQueryable(u => u.InputTime <= dt).OrderByDescending(u => u.InputTime).ToList() ;
        }

        #endregion

        #region 添加
        public void AddState(TiShengJiState state)
        {
            if (dt == null)
                dt = ClassToDataTable(typeof(TiShengJiState));
            dt.Clear();
            dt = ParseInDataTable(dt,state);
            SetDataTableToTable(dt, tableName);
        }
        #endregion

    }
}
