using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanXingService_WMS.Entity
{
    public class PrintRecordListsDto
    {
        public int ID { get; set; }
        /// <summary>
        /// 标签条码号
        /// </summary>
        
        public string BarcodeNumber { get; set; }
        /// <summary>
        /// 标签打印日期
        /// </summary>
        public DateTime? PrintDate { get; set; }

        /// <summary>
        /// 打印操作人
        /// </summary>
        
        public string PrintOperator { get; set; }

        /// <summary>
        /// 打印操作人编码
        /// </summary>
        
        public string PrintOperatorCode { get; set; }

        /// <summary>
        /// 关联的报工主键id
        /// </summary>
        public int? ProductUploadHistory_ID { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        

        public string InWarehouseCode { get; set; }

        /// <summary>5
        /// 标签记录状态:已打印，已作废，已入库等
        /// </summary>

        public string PrintRecordState { get; set; }

        /// <summary>
        /// 打印单打印数量
        /// </summary>
        public decimal? ProCount { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        
        public string ItemName { get; set; }

        
        public string Reserve1 { get; set; }
        
        public string Reserve2 { get; set; }
        
        public string Reserve3 { get; set; }
        
        public string Reserve4 { get; set; }
        
        public string Reserve5 { get; set; }
        
        public string Reserve6 { get; set; }
        /// <summary>
        /// 控制台号
        /// </summary>

        
        public string ConsoleNo { get; set; }

        /// <summary>
        /// 打印备注
        /// </summary>

        
        public string Remark { get; set; }

        /// <summary>
        /// 收货人工号
        /// </summary>
        
        public string ReceiverNo { get; set; }
        /// <summary>
        /// 收货人名称
        /// </summary>
        
        public string ReceiverName { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        
        public string CRMID { get; set; }


        /// <summary>
        /// 产地名称
        /// </summary>
        
        public string OriginName { get; set; }
        /// <summary>
        /// 产地编号
        /// </summary>
        

        public string OriginNo { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        
        public string Channel { get; set; }

        /// <summary>
        /// 保管者类型ID
        /// </summary>
        
        public string CustodianTypeID { get; set; }
        /// <summary>
        /// 生产标准
        /// </summary>
        
        public string ProStandard { get; set; }

        /// <summary>
        /// 生产标准编号
        /// </summary>
        
        public string ProStandardNo { get; set; }
        /// <summary>
        /// 生产批号(加车间号)
        /// </summary>
        

        public string UploadBatch { get; set; }

        /// <summary>
        /// 排产单明细号
        /// </summary>
        

        public string PlanOrder_XuHao { get; set; }

        /// <summary>
        /// 收成年份
        /// </summary>
        
        public string HarvestYear { get; set; }




        /// <summary>
        /// 入库数量kg
        /// </summary>
        public decimal? ProCountKg { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? ValidityDate { get; set; }

        /// <summary>
        /// 材料类型
        /// </summary>
        
        public string MaterialType { get; set; }

        /// <summary>
        /// 海关编码名称
        /// </summary>
        
        public string CustomsName { get; set; }

        /// <summary>
        /// 海关编码编号
        /// </summary>
        
        public string CustomsNo { get; set; }



        /// <summary>
        /// 产品编码
        /// </summary>
        
        public string ItemNo { get; set; }

        /// <summary>
        /// 包装日期
        /// </summary>

        public DateTime? PackDate { get; set; }

        /// <summary>
        /// 货主ID
        /// </summary>
        
        public string ConsignorID { get; set; }

        /// <summary>
        /// 货主类型
        /// </summary>
        
        public string ConsignorType { get; set; }

        /// <summary>
        /// 生产车间
        /// </summary>
        

        public string CheJianClass { get; set; }

        /// <summary>
        /// 交货人
        /// </summary>
        

        public string DeliveryMan { get; set; }

        /// <summary>
        /// 交货人编码
        /// </summary>
        

        public string DeliveryNo { get; set; }

        /// <summary>
        /// 配方名称
        /// </summary>
        

        public string ProductRecipe { get; set; }

        /// <summary>
        /// 配方编号
        /// </summary>
        

        public string ProductRecipeNo { get; set; }

        /// <summary>
        /// 生产单明细号
        /// </summary>
        

        public string ProductOrder_XuHao { get; set; }

        /// <summary>
        /// 生产单位
        /// </summary>
        

        public string Unit { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        

        public string Spec { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        

        public string BoxNo { get; set; }

        public string AddDate { get; set; }
        /// <summary>
        /// 入库人
        /// </summary>
        public string InWareHousePeople { get; set; }

        /// <summary>
        /// 入库人编号
        /// </summary>
        public string InwarehouseNo { get; set; }

        public string PrintRecordListsIDS { get; set; }
    }
}
