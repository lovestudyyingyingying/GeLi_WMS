namespace NanXingService_WMS.Entity.StockEntity
{
    public class WareLocationIndexData
    {
        public int ID { get; set; }

        public string AGVPosition { get; set; }

        public string WareLocaNo { get; set; }

        public int? WareArea_ID { get; set; }

        public string WareLocaState { get; set; }

        public string TrayStateNo { get; set; }

        public string TrayStateBatchNo { get; set; }

        public int IsOpen { get; set; }
    }
}
