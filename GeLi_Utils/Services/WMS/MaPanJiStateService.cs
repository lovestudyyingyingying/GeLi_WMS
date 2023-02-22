using GeLiData_WMS.Dao;
using GeLiData_WMSUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Services.WMS
{
    public  class MaPanJiStateService:DbBase<MaPanJiState>
    {
        //public MaPanJiState SaveState(List<bool> bools,int banNum,string stateStr)
        //{
        //    MaPanJiState state = new MaPanJiState();
        //    state.IsDieBan = bools[0];
        //    state.IsDieBanReadyAndAllowIn = bools[1];
        //    state.IsDieBaning = bools[2];
        //    state.IsChaiBan = bools[3];
        //    state.IsChaiBanReadyAndAllowIn=bools[4];
        //    state.IsChaiBaning=bools[5];
        //    state.IsChaiBanEnd=bools[6];
        //    state.IsDieBanEnd=bools[7];
        //    state.InputTime=DateTime.Now;
        //    state.BanNum = banNum;
        //    state.Reserve1 = stateStr;
        //    Insert(state);
        //    SaveChanges();
        //    return state;
        //}


        



        /// <summary>
        /// 创建码盘机状态
        /// </summary>
        /// <param name="bools"></param>
        /// <param name="banNum"></param>
        /// <param name="stateStr"></param>
        /// <returns></returns>
        public int CreatMaPanJiState(List<bool> bools, int banNum, string stateStr)
        {
            MaPanJiState state = new MaPanJiState();
            state.IsDieBan = bools[0];
            state.IsDieBanReadyAndAllowIn = bools[1];
            state.IsDieBaning = bools[2];
            state.IsChaiBan = bools[3];
            state.IsChaiBanReadyAndAllowIn = bools[4];
            state.IsChaiBaning = bools[5];
            state.IsChaiBanEnd = bools[6];
            state.IsDieBanEnd = bools[7];
            state.InputTime = DateTime.Now;
            state.Reserve1 = stateStr;
            state.BanNum = banNum;
            Insert(state);
            SaveChanges();
            return state.ID;
        }



    }


}
