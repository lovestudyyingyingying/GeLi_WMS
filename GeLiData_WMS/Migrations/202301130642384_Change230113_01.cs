namespace GeLiData_WMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change230113_01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AGVAlarmLog",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        deviceNum = c.String(maxLength: 50),
                        alarmDesc = c.String(maxLength: 1000),
                        alarmType = c.Int(),
                        areaId = c.Int(),
                        alarmReadFlag = c.Int(),
                        channelDeviceId = c.String(maxLength: 50),
                        alarmSource = c.String(maxLength: 50),
                        channelName = c.String(maxLength: 256),
                        alarmDate = c.DateTime(),
                        recTime = c.DateTime(nullable: false),
                        deviceName = c.String(maxLength: 50),
                        alarmGrade = c.Int(),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AGVMissionInfo",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WHName = c.String(maxLength: 20),
                        MissionNo = c.String(maxLength: 20),
                        OrderTime = c.DateTime(),
                        TrayNo = c.String(maxLength: 50),
                        Mark = c.String(maxLength: 50),
                        StartLocation = c.String(maxLength: 50),
                        StartPosition = c.String(maxLength: 50),
                        StartMiddleLocation = c.String(maxLength: 50),
                        StartMiddlePosition = c.String(maxLength: 50),
                        EndLocation = c.String(maxLength: 50),
                        EndPosition = c.String(maxLength: 50),
                        EndMiddleLocation = c.String(maxLength: 50),
                        EndMiddlePosition = c.String(maxLength: 50),
                        SendState = c.String(maxLength: 50, unicode: false),
                        SendMsg = c.String(maxLength: 100),
                        StateMsg = c.String(maxLength: 100, unicode: false),
                        RunState = c.String(maxLength: 50, unicode: false),
                        StateTime = c.DateTime(),
                        StockPlan_ID = c.Int(),
                        OrderGroupId = c.String(maxLength: 255),
                        ModelProcessCode = c.String(maxLength: 20),
                        AGVCarId = c.String(maxLength: 20),
                        userId = c.String(maxLength: 10),
                        MissionFloor_ID = c.Int(),
                        Remark = c.String(maxLength: 50),
                        IsFloor = c.Int(),
                        SendAGVPoStr = c.String(maxLength: 50),
                        ApiReturnPoStr = c.String(maxLength: 50),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AGVMissionInfo_Floor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsContinued = c.Int(),
                        TSJ_Name = c.String(maxLength: 20),
                        TiShengJiRecord_ID = c.Int(),
                        MissionNo = c.String(maxLength: 20),
                        OrderTime = c.DateTime(),
                        TrayNo = c.String(maxLength: 50),
                        Mark = c.String(maxLength: 50),
                        StartLocation = c.String(maxLength: 50),
                        StartPosition = c.String(maxLength: 50),
                        StartMiddleLocation = c.String(maxLength: 50),
                        StartMiddlePosition = c.String(maxLength: 50),
                        EndLocation = c.String(maxLength: 50),
                        EndPosition = c.String(maxLength: 50),
                        EndMiddleLocation = c.String(maxLength: 50),
                        EndMiddlePosition = c.String(maxLength: 50),
                        SendState = c.String(maxLength: 50, unicode: false),
                        SendMsg = c.String(maxLength: 100),
                        StateMsg = c.String(maxLength: 100, unicode: false),
                        RunState = c.String(maxLength: 50, unicode: false),
                        StateTime = c.DateTime(),
                        StockPlan_ID = c.Int(),
                        OrderGroupId = c.String(maxLength: 255),
                        ModelProcessCode = c.String(maxLength: 20),
                        AGVCarId = c.String(maxLength: 20),
                        userId = c.String(maxLength: 10),
                        MissionFloor_ID = c.Int(),
                        Remark = c.String(maxLength: 50),
                        IsFloor = c.Int(),
                        SendAGVPoStr = c.String(maxLength: 50),
                        ApiReturnPoStr = c.String(maxLength: 50),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AGVMissionInfo", t => t.MissionFloor_ID)
                .ForeignKey("dbo.TiShengJiRunRecord", t => t.TiShengJiRecord_ID)
                .Index(t => t.TiShengJiRecord_ID)
                .Index(t => t.MissionFloor_ID);
            
            CreateTable(
                "dbo.TiShengJiRunRecord",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TsjName = c.String(maxLength: 20),
                        TsjIp = c.String(maxLength: 25),
                        TsjPort = c.Int(nullable: false),
                        OrderTime = c.DateTime(nullable: false),
                        TrayCount = c.Int(nullable: false),
                        StartFloor = c.String(maxLength: 20),
                        EndFloor = c.String(maxLength: 20),
                        InsideTrayNo = c.String(maxLength: 20),
                        OutsideTrayNo = c.String(maxLength: 20),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Depts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        SortIndex = c.Int(nullable: false),
                        Remark = c.String(maxLength: 500),
                        ParentID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Depts", t => t.ParentID)
                .Index(t => t.ParentID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 50),
                        Enabled = c.Boolean(nullable: false),
                        Gender = c.String(maxLength: 10),
                        ChineseName = c.String(maxLength: 100),
                        EnglishName = c.String(maxLength: 100),
                        Photo = c.String(maxLength: 200),
                        QQ = c.String(maxLength: 50),
                        CompanyEmail = c.String(maxLength: 100),
                        OfficePhone = c.String(maxLength: 50),
                        OfficePhoneExt = c.String(maxLength: 50),
                        HomePhone = c.String(maxLength: 50),
                        CellPhone = c.String(maxLength: 50),
                        Address = c.String(maxLength: 500),
                        Remark = c.String(maxLength: 500),
                        IdentityCard = c.String(maxLength: 50),
                        Birthday = c.DateTime(),
                        TakeOfficeTime = c.DateTime(),
                        LastLoginTime = c.DateTime(),
                        CreateTime = c.DateTime(),
                        DeptID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Depts", t => t.DeptID)
                .Index(t => t.DeptID);
            
            CreateTable(
                "dbo.Onlines",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IPAdddress = c.String(maxLength: 50),
                        LoginTime = c.DateTime(nullable: false),
                        UpdateTime = c.DateTime(),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Remark = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Powers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        GroupName = c.String(maxLength: 50),
                        Title = c.String(maxLength: 200),
                        Remark = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ImageUrl = c.String(maxLength: 200),
                        NavigateUrl = c.String(maxLength: 200),
                        Remark = c.String(maxLength: 500),
                        SortIndex = c.Int(nullable: false),
                        ParentID = c.Int(),
                        ViewPowerID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Menus", t => t.ParentID)
                .ForeignKey("dbo.Powers", t => t.ViewPowerID)
                .Index(t => t.ParentID)
                .Index(t => t.ViewPowerID);
            
            CreateTable(
                "dbo.Titles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Remark = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WareLocation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WareLocaNo = c.String(maxLength: 50),
                        WareLoca_Lie = c.String(maxLength: 50),
                        WareLoca_Index = c.String(maxLength: 50),
                        WareArea_ID = c.Int(),
                        header_ID = c.Int(),
                        AGVPosition = c.String(maxLength: 50),
                        WareLocaState = c.String(maxLength: 10),
                        LockHis_ID = c.Int(),
                        BatchNo = c.String(maxLength: 50),
                        TrayState_ID = c.Int(),
                        IsOpen = c.Int(),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TrayState", t => t.TrayState_ID)
                .ForeignKey("dbo.WareArea", t => t.WareArea_ID)
                .ForeignKey("dbo.WareLoactionLockHis", t => t.LockHis_ID)
                .ForeignKey("dbo.Users", t => t.header_ID)
                .Index(t => t.WareLoca_Lie)
                .Index(t => t.WareArea_ID)
                .Index(t => t.header_ID)
                .Index(t => t.LockHis_ID)
                .Index(t => t.TrayState_ID);
            
            CreateTable(
                "dbo.TrayState",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TrayNO = c.String(nullable: false, maxLength: 20),
                        optdate = c.DateTime(nullable: false),
                        OnlineCount = c.Int(nullable: false),
                        WareLocation_ID = c.Int(),
                        proname = c.String(maxLength: 50),
                        itemno = c.String(maxLength: 50),
                        spec = c.String(maxLength: 20),
                        batchNo = c.String(maxLength: 100),
                        boxName = c.String(maxLength: 10, fixedLength: true),
                        color = c.String(maxLength: 10),
                        probiaozhun = c.String(maxLength: 10),
                        position = c.String(maxLength: 20),
                        remark = c.String(),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.WareLocation", t => t.WareLocation_ID)
                .Index(t => t.WareLocation_ID);
            
            CreateTable(
                "dbo.TrayPro",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        optdate = c.DateTime(nullable: false),
                        TrayStateID = c.Int(nullable: false),
                        prosn = c.String(maxLength: 60),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TrayState", t => t.TrayStateID)
                .Index(t => t.TrayStateID);
            
            CreateTable(
                "dbo.WareArea",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WareNo = c.String(maxLength: 50),
                        War_ID = c.Int(),
                        WareHouse_ID = c.Int(),
                        WareAreaState = c.Boolean(),
                        InstockRule = c.String(maxLength: 10),
                        protype = c.String(maxLength: 10),
                        InstockWay = c.String(maxLength: 10),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.WareAreaClass", t => t.War_ID)
                .ForeignKey("dbo.WareHouse", t => t.WareHouse_ID)
                .Index(t => t.War_ID)
                .Index(t => t.WareHouse_ID);
            
            CreateTable(
                "dbo.WareAreaClass",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AreaClass = c.String(maxLength: 50),
                        SortIndex = c.Int(),
                        Remark = c.String(maxLength: 50),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WareHouse",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WHName = c.String(maxLength: 50),
                        WHPosition = c.String(maxLength: 50),
                        WHState = c.Boolean(),
                        Remark = c.String(maxLength: 50),
                        AGVModelCode = c.String(maxLength: 20),
                        AGVServerIP = c.String(maxLength: 20),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AGVRunModel",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WareHouse_ID = c.Int(nullable: false),
                        TiShengJi_ID = c.Int(),
                        AGVModelCode = c.String(maxLength: 20),
                        AGVModelName = c.String(maxLength: 50),
                        ModelDesc = c.String(maxLength: 100),
                        SendOrderPath = c.String(maxLength: 50),
                        ApiRuturnPath = c.String(maxLength: 50),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TiShengJiInfo", t => t.TiShengJi_ID)
                .ForeignKey("dbo.WareHouse", t => t.WareHouse_ID, cascadeDelete: true)
                .Index(t => t.WareHouse_ID)
                .Index(t => t.TiShengJi_ID);
            
            CreateTable(
                "dbo.TiShengJiInfo",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TsjName = c.String(maxLength: 20),
                        TsjIp = c.String(maxLength: 25),
                        TsjPort = c.Int(nullable: false),
                        InputTime = c.DateTime(nullable: false),
                        Floors = c.String(maxLength: 200),
                        TsjPosition_1F = c.String(maxLength: 20),
                        TsjPosition_2F = c.String(maxLength: 20),
                        TsjPosition_3F = c.String(maxLength: 20),
                        TsjInModel_1F = c.String(maxLength: 20),
                        TsjInModel_2F = c.String(maxLength: 20),
                        TsjInModel_3F = c.String(maxLength: 20),
                        TsjOutModel_1F = c.String(maxLength: 20),
                        TsjOutModel_2F = c.String(maxLength: 20),
                        TsjOutModel_3F = c.String(maxLength: 20),
                        AGVServerIP = c.String(maxLength: 20),
                        IsOpen = c.Int(nullable: false),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WareLoactionLockHis",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WareLocaNo = c.String(maxLength: 50),
                        PreState = c.String(maxLength: 20),
                        Locker = c.String(maxLength: 20),
                        LockTime = c.DateTime(),
                        UnLockTime = c.DateTime(),
                        MissionID = c.Int(),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AGVMissionInfo", t => t.MissionID)
                .Index(t => t.MissionID);
            
            CreateTable(
                "dbo.DeviceStatesInfo",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        deviceCode = c.String(maxLength: 50),
                        payLoad = c.String(maxLength: 50),
                        devicePostionRec = c.String(maxLength: 50),
                        devicePosition = c.String(maxLength: 50),
                        battery = c.String(maxLength: 50),
                        deviceName = c.String(maxLength: 50),
                        deviceStatusInt = c.Int(),
                        deviceStatus = c.String(maxLength: 50),
                        recTime = c.DateTime(nullable: false),
                        devicePostionX = c.String(maxLength: 10),
                        devicePostionY = c.String(maxLength: 10),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.FaHuoPlan",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        danjutype = c.String(maxLength: 100),
                        danjuno = c.String(maxLength: 100),
                        itemno = c.String(maxLength: 100),
                        itemname = c.String(maxLength: 100),
                        spec = c.String(maxLength: 100),
                        saleunit = c.String(maxLength: 100),
                        salequt = c.Decimal(precision: 18, scale: 2),
                        outqut = c.Decimal(precision: 18, scale: 2),
                        boxnum = c.Decimal(precision: 18, scale: 2),
                        fhdate = c.DateTime(),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StockPlan",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PlanNo = c.String(maxLength: 50),
                        proname = c.String(maxLength: 50),
                        batchNo = c.String(maxLength: 50),
                        probiaozhun = c.String(maxLength: 50),
                        spec = c.String(maxLength: 50),
                        count = c.Decimal(precision: 18, scale: 2),
                        plantime = c.DateTime(nullable: false),
                        planUser = c.String(maxLength: 50),
                        states = c.String(maxLength: 50),
                        color = c.String(maxLength: 50),
                        mark = c.String(maxLength: 10),
                        position = c.String(maxLength: 20),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StockRecord",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MissionNo = c.String(maxLength: 20),
                        TrayNo = c.String(maxLength: 50),
                        ProName = c.String(maxLength: 100),
                        BatchNo = c.String(maxLength: 50),
                        ProCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockType = c.String(maxLength: 20),
                        StockTypeDesc = c.String(maxLength: 30),
                        StartLocation = c.String(maxLength: 20),
                        EndLocation = c.String(maxLength: 20),
                        OrderTime = c.DateTime(nullable: false),
                        FinishTime = c.DateTime(nullable: false),
                        RecordTime = c.DateTime(nullable: false),
                        OrderUser = c.String(maxLength: 10),
                        OrderAGV = c.String(maxLength: 20),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TiShengJiState",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TsjIp = c.String(maxLength: 25),
                        InputTime = c.DateTime(),
                        deviceState = c.String(maxLength: 20),
                        carState = c.String(maxLength: 20),
                        carTarget = c.String(maxLength: 20),
                        CarCount = c.Int(),
                        F1Count = c.Int(),
                        F2Count = c.Int(),
                        F3Count = c.Int(),
                        CarState2 = c.String(maxLength: 20),
                        F1State = c.String(maxLength: 20),
                        F2State = c.String(maxLength: 20),
                        F3State = c.String(maxLength: 20),
                        F1DuiJieWei = c.String(maxLength: 20),
                        F2DuiJieWei = c.String(maxLength: 20),
                        F3DuiJieWei = c.String(maxLength: 20),
                        OrderReceive = c.String(maxLength: 20),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TouLiaoRecord",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RecTime = c.DateTime(),
                        prosn = c.String(maxLength: 20),
                        userID = c.String(maxLength: 10),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TrayWeightRecord",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Prosn = c.String(maxLength: 50),
                        BatchNo = c.String(maxLength: 50),
                        Position = c.String(maxLength: 50),
                        Proname = c.String(maxLength: 200),
                        Spec = c.String(maxLength: 50),
                        Biaozhun = c.String(maxLength: 50),
                        Result = c.String(maxLength: 10),
                        TrayCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrayWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BoxName = c.String(maxLength: 100),
                        Color = c.String(maxLength: 20),
                        Itemno = c.String(maxLength: 50),
                        RecTime = c.DateTime(nullable: false),
                        Rec_UserID = c.Int(),
                        Reserve1 = c.String(maxLength: 50),
                        Reserve2 = c.String(maxLength: 50),
                        Reserve3 = c.String(maxLength: 50),
                        Reserve4 = c.String(maxLength: 50),
                        Reserve5 = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.RolePowers",
                c => new
                    {
                        PowerID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PowerID, t.RoleID })
                .ForeignKey("dbo.Powers", t => t.PowerID, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.PowerID)
                .Index(t => t.RoleID);
            
            CreateTable(
                "dbo.RoleUsers",
                c => new
                    {
                        RoleID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleID, t.UserID })
                .ForeignKey("dbo.Roles", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.RoleID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.TitleUsers",
                c => new
                    {
                        TitleID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TitleID, t.UserID })
                .ForeignKey("dbo.Titles", t => t.TitleID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.TitleID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "DeptID", "dbo.Depts");
            DropForeignKey("dbo.WareLocation", "header_ID", "dbo.Users");
            DropForeignKey("dbo.WareLocation", "LockHis_ID", "dbo.WareLoactionLockHis");
            DropForeignKey("dbo.WareLoactionLockHis", "MissionID", "dbo.AGVMissionInfo");
            DropForeignKey("dbo.WareLocation", "WareArea_ID", "dbo.WareArea");
            DropForeignKey("dbo.WareArea", "WareHouse_ID", "dbo.WareHouse");
            DropForeignKey("dbo.AGVRunModel", "WareHouse_ID", "dbo.WareHouse");
            DropForeignKey("dbo.AGVRunModel", "TiShengJi_ID", "dbo.TiShengJiInfo");
            DropForeignKey("dbo.WareArea", "War_ID", "dbo.WareAreaClass");
            DropForeignKey("dbo.WareLocation", "TrayState_ID", "dbo.TrayState");
            DropForeignKey("dbo.TrayState", "WareLocation_ID", "dbo.WareLocation");
            DropForeignKey("dbo.TrayPro", "TrayStateID", "dbo.TrayState");
            DropForeignKey("dbo.TitleUsers", "UserID", "dbo.Users");
            DropForeignKey("dbo.TitleUsers", "TitleID", "dbo.Titles");
            DropForeignKey("dbo.RoleUsers", "UserID", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.RolePowers", "RoleID", "dbo.Roles");
            DropForeignKey("dbo.RolePowers", "PowerID", "dbo.Powers");
            DropForeignKey("dbo.Menus", "ViewPowerID", "dbo.Powers");
            DropForeignKey("dbo.Menus", "ParentID", "dbo.Menus");
            DropForeignKey("dbo.Onlines", "UserID", "dbo.Users");
            DropForeignKey("dbo.Depts", "ParentID", "dbo.Depts");
            DropForeignKey("dbo.AGVMissionInfo_Floor", "TiShengJiRecord_ID", "dbo.TiShengJiRunRecord");
            DropForeignKey("dbo.AGVMissionInfo_Floor", "MissionFloor_ID", "dbo.AGVMissionInfo");
            DropIndex("dbo.TitleUsers", new[] { "UserID" });
            DropIndex("dbo.TitleUsers", new[] { "TitleID" });
            DropIndex("dbo.RoleUsers", new[] { "UserID" });
            DropIndex("dbo.RoleUsers", new[] { "RoleID" });
            DropIndex("dbo.RolePowers", new[] { "RoleID" });
            DropIndex("dbo.RolePowers", new[] { "PowerID" });
            DropIndex("dbo.WareLoactionLockHis", new[] { "MissionID" });
            DropIndex("dbo.AGVRunModel", new[] { "TiShengJi_ID" });
            DropIndex("dbo.AGVRunModel", new[] { "WareHouse_ID" });
            DropIndex("dbo.WareArea", new[] { "WareHouse_ID" });
            DropIndex("dbo.WareArea", new[] { "War_ID" });
            DropIndex("dbo.TrayPro", new[] { "TrayStateID" });
            DropIndex("dbo.TrayState", new[] { "WareLocation_ID" });
            DropIndex("dbo.WareLocation", new[] { "TrayState_ID" });
            DropIndex("dbo.WareLocation", new[] { "LockHis_ID" });
            DropIndex("dbo.WareLocation", new[] { "header_ID" });
            DropIndex("dbo.WareLocation", new[] { "WareArea_ID" });
            DropIndex("dbo.WareLocation", new[] { "WareLoca_Lie" });
            DropIndex("dbo.Menus", new[] { "ViewPowerID" });
            DropIndex("dbo.Menus", new[] { "ParentID" });
            DropIndex("dbo.Onlines", new[] { "UserID" });
            DropIndex("dbo.Users", new[] { "DeptID" });
            DropIndex("dbo.Depts", new[] { "ParentID" });
            DropIndex("dbo.AGVMissionInfo_Floor", new[] { "MissionFloor_ID" });
            DropIndex("dbo.AGVMissionInfo_Floor", new[] { "TiShengJiRecord_ID" });
            DropTable("dbo.TitleUsers");
            DropTable("dbo.RoleUsers");
            DropTable("dbo.RolePowers");
            DropTable("dbo.TrayWeightRecord");
            DropTable("dbo.TouLiaoRecord");
            DropTable("dbo.TiShengJiState");
            DropTable("dbo.StockRecord");
            DropTable("dbo.StockPlan");
            DropTable("dbo.FaHuoPlan");
            DropTable("dbo.DeviceStatesInfo");
            DropTable("dbo.WareLoactionLockHis");
            DropTable("dbo.TiShengJiInfo");
            DropTable("dbo.AGVRunModel");
            DropTable("dbo.WareHouse");
            DropTable("dbo.WareAreaClass");
            DropTable("dbo.WareArea");
            DropTable("dbo.TrayPro");
            DropTable("dbo.TrayState");
            DropTable("dbo.WareLocation");
            DropTable("dbo.Titles");
            DropTable("dbo.Menus");
            DropTable("dbo.Powers");
            DropTable("dbo.Roles");
            DropTable("dbo.Onlines");
            DropTable("dbo.Users");
            DropTable("dbo.Depts");
            DropTable("dbo.TiShengJiRunRecord");
            DropTable("dbo.AGVMissionInfo_Floor");
            DropTable("dbo.AGVMissionInfo");
            DropTable("dbo.AGVAlarmLog");
        }
    }
}
/*
 CREATE TABLE [dbo].[AGVAlarmLog] (
    [ID] [int] NOT NULL IDENTITY,
    [deviceNum] [nvarchar](50),
    [alarmDesc] [nvarchar](1000),
    [alarmType] [int],
    [areaId] [int],
    [alarmReadFlag] [int],
    [channelDeviceId] [nvarchar](50),
    [alarmSource] [nvarchar](50),
    [channelName] [nvarchar](256),
    [alarmDate] [datetime],
    [recTime] [datetime] NOT NULL,
    [deviceName] [nvarchar](50),
    [alarmGrade] [int],
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.AGVAlarmLog] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[AGVMissionInfo] (
    [ID] [int] NOT NULL IDENTITY,
    [WHName] [nvarchar](20),
    [MissionNo] [nvarchar](20),
    [OrderTime] [datetime],
    [TrayNo] [nvarchar](50),
    [Mark] [nvarchar](50),
    [StartLocation] [nvarchar](50),
    [StartPosition] [nvarchar](50),
    [StartMiddleLocation] [nvarchar](50),
    [StartMiddlePosition] [nvarchar](50),
    [EndLocation] [nvarchar](50),
    [EndPosition] [nvarchar](50),
    [EndMiddleLocation] [nvarchar](50),
    [EndMiddlePosition] [nvarchar](50),
    [SendState] [varchar](50),
    [SendMsg] [nvarchar](100),
    [StateMsg] [varchar](100),
    [RunState] [varchar](50),
    [StateTime] [datetime],
    [StockPlan_ID] [int],
    [OrderGroupId] [nvarchar](255),
    [ModelProcessCode] [nvarchar](20),
    [AGVCarId] [nvarchar](20),
    [userId] [nvarchar](10),
    [MissionFloor_ID] [int],
    [Remark] [nvarchar](50),
    [IsFloor] [int],
    [SendAGVPoStr] [nvarchar](50),
    [ApiReturnPoStr] [nvarchar](50),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.AGVMissionInfo] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[AGVMissionInfo_Floor] (
    [ID] [int] NOT NULL IDENTITY,
    [IsContinued] [int],
    [TSJ_Name] [nvarchar](20),
    [TiShengJiRecord_ID] [int],
    [MissionNo] [nvarchar](20),
    [OrderTime] [datetime],
    [TrayNo] [nvarchar](50),
    [Mark] [nvarchar](50),
    [StartLocation] [nvarchar](50),
    [StartPosition] [nvarchar](50),
    [StartMiddleLocation] [nvarchar](50),
    [StartMiddlePosition] [nvarchar](50),
    [EndLocation] [nvarchar](50),
    [EndPosition] [nvarchar](50),
    [EndMiddleLocation] [nvarchar](50),
    [EndMiddlePosition] [nvarchar](50),
    [SendState] [varchar](50),
    [SendMsg] [nvarchar](100),
    [StateMsg] [varchar](100),
    [RunState] [varchar](50),
    [StateTime] [datetime],
    [StockPlan_ID] [int],
    [OrderGroupId] [nvarchar](255),
    [ModelProcessCode] [nvarchar](20),
    [AGVCarId] [nvarchar](20),
    [userId] [nvarchar](10),
    [MissionFloor_ID] [int],
    [Remark] [nvarchar](50),
    [IsFloor] [int],
    [SendAGVPoStr] [nvarchar](50),
    [ApiReturnPoStr] [nvarchar](50),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.AGVMissionInfo_Floor] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_TiShengJiRecord_ID] ON [dbo].[AGVMissionInfo_Floor]([TiShengJiRecord_ID])
CREATE INDEX [IX_MissionFloor_ID] ON [dbo].[AGVMissionInfo_Floor]([MissionFloor_ID])
CREATE TABLE [dbo].[TiShengJiRunRecord] (
    [ID] [int] NOT NULL IDENTITY,
    [TsjName] [nvarchar](20),
    [TsjIp] [nvarchar](25),
    [TsjPort] [int] NOT NULL,
    [OrderTime] [datetime] NOT NULL,
    [TrayCount] [int] NOT NULL,
    [StartFloor] [nvarchar](20),
    [EndFloor] [nvarchar](20),
    [InsideTrayNo] [nvarchar](20),
    [OutsideTrayNo] [nvarchar](20),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.TiShengJiRunRecord] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Depts] (
    [ID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](60) NOT NULL,
    [SortIndex] [int] NOT NULL,
    [Remark] [nvarchar](500),
    [ParentID] [int],
    CONSTRAINT [PK_dbo.Depts] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_ParentID] ON [dbo].[Depts]([ParentID])
CREATE TABLE [dbo].[Users] (
    [ID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](50) NOT NULL,
    [Email] [nvarchar](100) NOT NULL,
    [Password] [nvarchar](50) NOT NULL,
    [Enabled] [bit] NOT NULL,
    [Gender] [nvarchar](10),
    [ChineseName] [nvarchar](100),
    [EnglishName] [nvarchar](100),
    [Photo] [nvarchar](200),
    [QQ] [nvarchar](50),
    [CompanyEmail] [nvarchar](100),
    [OfficePhone] [nvarchar](50),
    [OfficePhoneExt] [nvarchar](50),
    [HomePhone] [nvarchar](50),
    [CellPhone] [nvarchar](50),
    [Address] [nvarchar](500),
    [Remark] [nvarchar](500),
    [IdentityCard] [nvarchar](50),
    [Birthday] [datetime],
    [TakeOfficeTime] [datetime],
    [LastLoginTime] [datetime],
    [CreateTime] [datetime],
    [DeptID] [int],
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_DeptID] ON [dbo].[Users]([DeptID])
CREATE TABLE [dbo].[Onlines] (
    [ID] [int] NOT NULL IDENTITY,
    [IPAdddress] [nvarchar](50),
    [LoginTime] [datetime] NOT NULL,
    [UpdateTime] [datetime],
    [UserID] [int] NOT NULL,
    CONSTRAINT [PK_dbo.Onlines] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_UserID] ON [dbo].[Onlines]([UserID])
CREATE TABLE [dbo].[Roles] (
    [ID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](50) NOT NULL,
    [Remark] [nvarchar](500),
    CONSTRAINT [PK_dbo.Roles] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Powers] (
    [ID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](50) NOT NULL,
    [GroupName] [nvarchar](50),
    [Title] [nvarchar](200),
    [Remark] [nvarchar](500),
    CONSTRAINT [PK_dbo.Powers] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Menus] (
    [ID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](50) NOT NULL,
    [ImageUrl] [nvarchar](200),
    [NavigateUrl] [nvarchar](200),
    [Remark] [nvarchar](500),
    [SortIndex] [int] NOT NULL,
    [ParentID] [int],
    [ViewPowerID] [int],
    CONSTRAINT [PK_dbo.Menus] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_ParentID] ON [dbo].[Menus]([ParentID])
CREATE INDEX [IX_ViewPowerID] ON [dbo].[Menus]([ViewPowerID])
CREATE TABLE [dbo].[Titles] (
    [ID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](50) NOT NULL,
    [Remark] [nvarchar](500),
    CONSTRAINT [PK_dbo.Titles] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[WareLocation] (
    [ID] [int] NOT NULL IDENTITY,
    [WareLocaNo] [nvarchar](50),
    [WareLoca_Lie] [nvarchar](50),
    [WareLoca_Index] [nvarchar](50),
    [WareArea_ID] [int],
    [header_ID] [int],
    [AGVPosition] [nvarchar](50),
    [WareLocaState] [nvarchar](10),
    [LockHis_ID] [int],
    [BatchNo] [nvarchar](50),
    [TrayState_ID] [int],
    [IsOpen] [int],
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.WareLocation] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_WareLoca_Lie] ON [dbo].[WareLocation]([WareLoca_Lie])
CREATE INDEX [IX_WareArea_ID] ON [dbo].[WareLocation]([WareArea_ID])
CREATE INDEX [IX_header_ID] ON [dbo].[WareLocation]([header_ID])
CREATE INDEX [IX_LockHis_ID] ON [dbo].[WareLocation]([LockHis_ID])
CREATE INDEX [IX_TrayState_ID] ON [dbo].[WareLocation]([TrayState_ID])
CREATE TABLE [dbo].[TrayState] (
    [ID] [int] NOT NULL IDENTITY,
    [TrayNO] [nvarchar](20) NOT NULL,
    [optdate] [datetime] NOT NULL,
    [OnlineCount] [int] NOT NULL,
    [WareLocation_ID] [int],
    [proname] [nvarchar](50),
    [itemno] [nvarchar](50),
    [spec] [nvarchar](20),
    [batchNo] [nvarchar](100),
    [boxName] [nchar](10),
    [color] [nvarchar](10),
    [probiaozhun] [nvarchar](10),
    [position] [nvarchar](20),
    [remark] [nvarchar](max),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.TrayState] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_WareLocation_ID] ON [dbo].[TrayState]([WareLocation_ID])
CREATE TABLE [dbo].[TrayPro] (
    [ID] [int] NOT NULL IDENTITY,
    [optdate] [datetime] NOT NULL,
    [TrayStateID] [int] NOT NULL,
    [prosn] [nvarchar](60),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.TrayPro] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_TrayStateID] ON [dbo].[TrayPro]([TrayStateID])
CREATE TABLE [dbo].[WareArea] (
    [ID] [int] NOT NULL IDENTITY,
    [WareNo] [nvarchar](50),
    [War_ID] [int],
    [WareHouse_ID] [int],
    [WareAreaState] [bit],
    [InstockRule] [nvarchar](10),
    [protype] [nvarchar](10),
    [InstockWay] [nvarchar](10),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.WareArea] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_War_ID] ON [dbo].[WareArea]([War_ID])
CREATE INDEX [IX_WareHouse_ID] ON [dbo].[WareArea]([WareHouse_ID])
CREATE TABLE [dbo].[WareAreaClass] (
    [ID] [int] NOT NULL IDENTITY,
    [AreaClass] [nvarchar](50),
    [SortIndex] [int],
    [Remark] [nvarchar](50),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.WareAreaClass] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[WareHouse] (
    [ID] [int] NOT NULL IDENTITY,
    [WHName] [nvarchar](50),
    [WHPosition] [nvarchar](50),
    [WHState] [bit],
    [Remark] [nvarchar](50),
    [AGVModelCode] [nvarchar](20),
    [AGVServerIP] [nvarchar](20),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.WareHouse] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[AGVRunModel] (
    [ID] [int] NOT NULL IDENTITY,
    [WareHouse_ID] [int] NOT NULL,
    [TiShengJi_ID] [int],
    [AGVModelCode] [nvarchar](20),
    [AGVModelName] [nvarchar](50),
    [ModelDesc] [nvarchar](100),
    [SendOrderPath] [nvarchar](50),
    [ApiRuturnPath] [nvarchar](50),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.AGVRunModel] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_WareHouse_ID] ON [dbo].[AGVRunModel]([WareHouse_ID])
CREATE INDEX [IX_TiShengJi_ID] ON [dbo].[AGVRunModel]([TiShengJi_ID])
CREATE TABLE [dbo].[TiShengJiInfo] (
    [ID] [int] NOT NULL IDENTITY,
    [TsjName] [nvarchar](20),
    [TsjIp] [nvarchar](25),
    [TsjPort] [int] NOT NULL,
    [InputTime] [datetime] NOT NULL,
    [Floors] [nvarchar](200),
    [TsjPosition_1F] [nvarchar](20),
    [TsjPosition_2F] [nvarchar](20),
    [TsjPosition_3F] [nvarchar](20),
    [TsjInModel_1F] [nvarchar](20),
    [TsjInModel_2F] [nvarchar](20),
    [TsjInModel_3F] [nvarchar](20),
    [TsjOutModel_1F] [nvarchar](20),
    [TsjOutModel_2F] [nvarchar](20),
    [TsjOutModel_3F] [nvarchar](20),
    [AGVServerIP] [nvarchar](20),
    [IsOpen] [int] NOT NULL,
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.TiShengJiInfo] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[WareLoactionLockHis] (
    [ID] [int] NOT NULL IDENTITY,
    [WareLocaNo] [nvarchar](50),
    [PreState] [nvarchar](20),
    [Locker] [nvarchar](20),
    [LockTime] [datetime],
    [UnLockTime] [datetime],
    [MissionID] [int],
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.WareLoactionLockHis] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_MissionID] ON [dbo].[WareLoactionLockHis]([MissionID])
CREATE TABLE [dbo].[DeviceStatesInfo] (
    [ID] [int] NOT NULL IDENTITY,
    [deviceCode] [nvarchar](50),
    [payLoad] [nvarchar](50),
    [devicePostionRec] [nvarchar](50),
    [devicePosition] [nvarchar](50),
    [battery] [nvarchar](50),
    [deviceName] [nvarchar](50),
    [deviceStatusInt] [int],
    [deviceStatus] [nvarchar](50),
    [recTime] [datetime] NOT NULL,
    [devicePostionX] [nvarchar](10),
    [devicePostionY] [nvarchar](10),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.DeviceStatesInfo] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[FaHuoPlan] (
    [ID] [int] NOT NULL IDENTITY,
    [danjutype] [nvarchar](100),
    [danjuno] [nvarchar](100),
    [itemno] [nvarchar](100),
    [itemname] [nvarchar](100),
    [spec] [nvarchar](100),
    [saleunit] [nvarchar](100),
    [salequt] [decimal](18, 2),
    [outqut] [decimal](18, 2),
    [boxnum] [decimal](18, 2),
    [fhdate] [datetime],
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.FaHuoPlan] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[StockPlan] (
    [ID] [int] NOT NULL IDENTITY,
    [PlanNo] [nvarchar](50),
    [proname] [nvarchar](50),
    [batchNo] [nvarchar](50),
    [probiaozhun] [nvarchar](50),
    [spec] [nvarchar](50),
    [count] [decimal](18, 2),
    [plantime] [datetime] NOT NULL,
    [planUser] [nvarchar](50),
    [states] [nvarchar](50),
    [color] [nvarchar](50),
    [mark] [nvarchar](10),
    [position] [nvarchar](20),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.StockPlan] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[StockRecord] (
    [ID] [int] NOT NULL IDENTITY,
    [MissionNo] [nvarchar](20),
    [TrayNo] [nvarchar](50),
    [ProName] [nvarchar](100),
    [BatchNo] [nvarchar](50),
    [ProCount] [decimal](18, 2) NOT NULL,
    [StockType] [nvarchar](20),
    [StockTypeDesc] [nvarchar](30),
    [StartLocation] [nvarchar](20),
    [EndLocation] [nvarchar](20),
    [OrderTime] [datetime] NOT NULL,
    [FinishTime] [datetime] NOT NULL,
    [RecordTime] [datetime] NOT NULL,
    [OrderUser] [nvarchar](10),
    [OrderAGV] [nvarchar](20),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.StockRecord] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[TiShengJiState] (
    [ID] [int] NOT NULL IDENTITY,
    [TsjIp] [nvarchar](25),
    [InputTime] [datetime],
    [deviceState] [nvarchar](20),
    [carState] [nvarchar](20),
    [carTarget] [nvarchar](20),
    [CarCount] [int],
    [F1Count] [int],
    [F2Count] [int],
    [F3Count] [int],
    [CarState2] [nvarchar](20),
    [F1State] [nvarchar](20),
    [F2State] [nvarchar](20),
    [F3State] [nvarchar](20),
    [F1DuiJieWei] [nvarchar](20),
    [F2DuiJieWei] [nvarchar](20),
    [F3DuiJieWei] [nvarchar](20),
    [OrderReceive] [nvarchar](20),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.TiShengJiState] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[TouLiaoRecord] (
    [ID] [int] NOT NULL IDENTITY,
    [RecTime] [datetime],
    [prosn] [nvarchar](20),
    [userID] [nvarchar](10),
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.TouLiaoRecord] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[TrayWeightRecord] (
    [ID] [int] NOT NULL IDENTITY,
    [Prosn] [nvarchar](50),
    [BatchNo] [nvarchar](50),
    [Position] [nvarchar](50),
    [Proname] [nvarchar](200),
    [Spec] [nvarchar](50),
    [Biaozhun] [nvarchar](50),
    [Result] [nvarchar](10),
    [TrayCount] [decimal](18, 2) NOT NULL,
    [TrayWeight] [decimal](18, 2) NOT NULL,
    [BoxName] [nvarchar](100),
    [Color] [nvarchar](20),
    [Itemno] [nvarchar](50),
    [RecTime] [datetime] NOT NULL,
    [Rec_UserID] [int],
    [Reserve1] [nvarchar](50),
    [Reserve2] [nvarchar](50),
    [Reserve3] [nvarchar](50),
    [Reserve4] [nvarchar](50),
    [Reserve5] [nvarchar](50),
    CONSTRAINT [PK_dbo.TrayWeightRecord] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[RolePowers] (
    [PowerID] [int] NOT NULL,
    [RoleID] [int] NOT NULL,
    CONSTRAINT [PK_dbo.RolePowers] PRIMARY KEY ([PowerID], [RoleID])
)
CREATE INDEX [IX_PowerID] ON [dbo].[RolePowers]([PowerID])
CREATE INDEX [IX_RoleID] ON [dbo].[RolePowers]([RoleID])
CREATE TABLE [dbo].[RoleUsers] (
    [RoleID] [int] NOT NULL,
    [UserID] [int] NOT NULL,
    CONSTRAINT [PK_dbo.RoleUsers] PRIMARY KEY ([RoleID], [UserID])
)
CREATE INDEX [IX_RoleID] ON [dbo].[RoleUsers]([RoleID])
CREATE INDEX [IX_UserID] ON [dbo].[RoleUsers]([UserID])
CREATE TABLE [dbo].[TitleUsers] (
    [TitleID] [int] NOT NULL,
    [UserID] [int] NOT NULL,
    CONSTRAINT [PK_dbo.TitleUsers] PRIMARY KEY ([TitleID], [UserID])
)
CREATE INDEX [IX_TitleID] ON [dbo].[TitleUsers]([TitleID])
CREATE INDEX [IX_UserID] ON [dbo].[TitleUsers]([UserID])
ALTER TABLE [dbo].[AGVMissionInfo_Floor] ADD CONSTRAINT [FK_dbo.AGVMissionInfo_Floor_dbo.AGVMissionInfo_MissionFloor_ID] FOREIGN KEY ([MissionFloor_ID]) REFERENCES [dbo].[AGVMissionInfo] ([ID])
ALTER TABLE [dbo].[AGVMissionInfo_Floor] ADD CONSTRAINT [FK_dbo.AGVMissionInfo_Floor_dbo.TiShengJiRunRecord_TiShengJiRecord_ID] FOREIGN KEY ([TiShengJiRecord_ID]) REFERENCES [dbo].[TiShengJiRunRecord] ([ID])
ALTER TABLE [dbo].[Depts] ADD CONSTRAINT [FK_dbo.Depts_dbo.Depts_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Depts] ([ID])
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_dbo.Users_dbo.Depts_DeptID] FOREIGN KEY ([DeptID]) REFERENCES [dbo].[Depts] ([ID])
ALTER TABLE [dbo].[Onlines] ADD CONSTRAINT [FK_dbo.Onlines_dbo.Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Menus] ADD CONSTRAINT [FK_dbo.Menus_dbo.Menus_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Menus] ([ID])
ALTER TABLE [dbo].[Menus] ADD CONSTRAINT [FK_dbo.Menus_dbo.Powers_ViewPowerID] FOREIGN KEY ([ViewPowerID]) REFERENCES [dbo].[Powers] ([ID])
ALTER TABLE [dbo].[WareLocation] ADD CONSTRAINT [FK_dbo.WareLocation_dbo.TrayState_TrayState_ID] FOREIGN KEY ([TrayState_ID]) REFERENCES [dbo].[TrayState] ([ID])
ALTER TABLE [dbo].[WareLocation] ADD CONSTRAINT [FK_dbo.WareLocation_dbo.WareArea_WareArea_ID] FOREIGN KEY ([WareArea_ID]) REFERENCES [dbo].[WareArea] ([ID])
ALTER TABLE [dbo].[WareLocation] ADD CONSTRAINT [FK_dbo.WareLocation_dbo.WareLoactionLockHis_LockHis_ID] FOREIGN KEY ([LockHis_ID]) REFERENCES [dbo].[WareLoactionLockHis] ([ID])
ALTER TABLE [dbo].[WareLocation] ADD CONSTRAINT [FK_dbo.WareLocation_dbo.Users_header_ID] FOREIGN KEY ([header_ID]) REFERENCES [dbo].[Users] ([ID])
ALTER TABLE [dbo].[TrayState] ADD CONSTRAINT [FK_dbo.TrayState_dbo.WareLocation_WareLocation_ID] FOREIGN KEY ([WareLocation_ID]) REFERENCES [dbo].[WareLocation] ([ID])
ALTER TABLE [dbo].[TrayPro] ADD CONSTRAINT [FK_dbo.TrayPro_dbo.TrayState_TrayStateID] FOREIGN KEY ([TrayStateID]) REFERENCES [dbo].[TrayState] ([ID])
ALTER TABLE [dbo].[WareArea] ADD CONSTRAINT [FK_dbo.WareArea_dbo.WareAreaClass_War_ID] FOREIGN KEY ([War_ID]) REFERENCES [dbo].[WareAreaClass] ([ID])
ALTER TABLE [dbo].[WareArea] ADD CONSTRAINT [FK_dbo.WareArea_dbo.WareHouse_WareHouse_ID] FOREIGN KEY ([WareHouse_ID]) REFERENCES [dbo].[WareHouse] ([ID])
ALTER TABLE [dbo].[AGVRunModel] ADD CONSTRAINT [FK_dbo.AGVRunModel_dbo.TiShengJiInfo_TiShengJi_ID] FOREIGN KEY ([TiShengJi_ID]) REFERENCES [dbo].[TiShengJiInfo] ([ID])
ALTER TABLE [dbo].[AGVRunModel] ADD CONSTRAINT [FK_dbo.AGVRunModel_dbo.WareHouse_WareHouse_ID] FOREIGN KEY ([WareHouse_ID]) REFERENCES [dbo].[WareHouse] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[WareLoactionLockHis] ADD CONSTRAINT [FK_dbo.WareLoactionLockHis_dbo.AGVMissionInfo_MissionID] FOREIGN KEY ([MissionID]) REFERENCES [dbo].[AGVMissionInfo] ([ID])
ALTER TABLE [dbo].[RolePowers] ADD CONSTRAINT [FK_dbo.RolePowers_dbo.Powers_PowerID] FOREIGN KEY ([PowerID]) REFERENCES [dbo].[Powers] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[RolePowers] ADD CONSTRAINT [FK_dbo.RolePowers_dbo.Roles_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[RoleUsers] ADD CONSTRAINT [FK_dbo.RoleUsers_dbo.Roles_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[RoleUsers] ADD CONSTRAINT [FK_dbo.RoleUsers_dbo.Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[TitleUsers] ADD CONSTRAINT [FK_dbo.TitleUsers_dbo.Titles_TitleID] FOREIGN KEY ([TitleID]) REFERENCES [dbo].[Titles] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[TitleUsers] ADD CONSTRAINT [FK_dbo.TitleUsers_dbo.Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE
CREATE TABLE [dbo].[__MigrationHistory] (
    [MigrationId] [nvarchar](150) NOT NULL,
    [ContextKey] [nvarchar](300) NOT NULL,
    [Model] [varbinary](max) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
)
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'202301130642384_Change230113_01', N'GeLiData_WMS.Migrations.Configuration',  0x1F8B0800000000000400ED7D5B6FDCB896EEFB00E73F187E3A33E889637BA7B177E0CCC0A9C4897BEC4EB59D4BEF27832ED155EA5649B57549E219CC2F9B87F949E72F1C8ABAF1B27853512AC71602042E52FC162F8BEB422E92FFEF7FFEF7E4DFBFAFA3BDAF38CDC2247EB57FF8ECF9FE1E8E174910C6CB57FB457EF7AF7FDDFFF77FFB3FFF74F236587FDFFBDC7C775C7E474AC6D9ABFD559E6F5E1E1C648B155EA3ECD93A5CA44996DCE5CF16C9FA0005C9C1D1F3E77F3B383C3CC004629F60EDED9D5C15711EAE31FD417ECE927881377981A2CB24C05156A7939C6B8ABAF72B5AE36C8316F8D5FE3B7C11BE4139BAF97279FD36CED3FBFDBDD32844A422D738BADBDF43719CE42827D57CF929C3D7799AC4CBEB0D4940D1C7FB0D26DFDDA128C375F55F769FDBB6E4F951D99283AE6003B528B23C593B021E1ED75D732016EFD5C1FB6DD791CE23DD13E6F765AB6907BEDA3F7DF7F93442E9FA2259EEEF89F45ECEA2B4FC96EFE2674C999FF6D89C9F5A76205C53FEFB696F56447991E257312EF214453FEDCD8BDB285CFC07BEFF98FC89E3577111456C0D491D491E974092E669B2C1697E7F85EFEA7A9FBFD9DF3BE0CB1D8805DB624C99AA39E7717E7CB4BFF72B218E6E23DC3200D3F4EB3C49F13B1CE314E53898A33CC72919BFF300D32E94A80BB402FC355CE05F8B754392B01D9940FB7B97E8FB058E97F9EAD5FE0B3263CEC2EF386812EA5A7C8A4332DD48993C2DB089102A47E20DCE161A4287CF9F7B23551211BAD1502AC5E83C702B5212BAC228388BD0D2A9E462458610476F68EF775407EDFCEBA4481778685275CBCA1F1A52472F7EF6C65384EF1B4AE5DF1FC3B5B1608A17D567623169A659CD1E7D5BFD8DE0BB14056E4C7D85339C7EC5874357AFA67334129DE391E8FC65243A2FFCD23939E8F4A749AB5E8659A909CFE3BBC445B132C526DDAAA0F5E5BD490AFA60A17A247E4D8626F4210D700A8B4D7DC18F29BAD756CFCB54BA44E99F43D3B8CE519A5F248BDACA1D81D83CC9C2D1885D864110E151DB57911CAB956FE360ACD6115223B66ADC916B098EC69D380E08BBE43A616A246465D195942EB3A5DE49F1C3FC39DE9690558BAE8A78A4AECB5B7DE0A61F889A5DFC398F507C2369670B8DF42E4D8A8DD6853A7AF1C2877E29D77748DA0267D92C0974FDE945DD12236B86527DC37CD0298805AAA572E8D14A398B9224751DE62BBC1E41B79F67B4724E352BA50519A779422A3574FD4E37E1152676743C0AB5C9717C8A8E23A1F32BFA1A2EA91521CB23C6E9BBA927CB158EE8C7D92ADC54EBD8CFA00F6F444FF32C4DD65749A4C0153EBFF988D225CE495B13FB32CD729707C7B8696C2FF7B82A3C39C94AB13B4BC8877181DDD65E3F5EFF7233867BFD31BC5E91E2BF10E1BB48D2C055774DDEF9E49D4FDEF9E49D4FDEF9E49D439426EF7CF2CE27EF7CF2CE27EF7CF2CEFD79E763F9E56D4889835FDEF8F2B62DEB1CB022AE7C30B075F26722E15A80742DB42C22AD3ED896DB6A05026AB6E5FA835C745A7D50D0FA98FD31CA2242F6C7F9466B03F921324FD2DCD479BD97111C91CA75855952C45B5688FAA19CEE1F6A9088DF340A9DF3380B036C5C76F1B32A54E4E3119BAC92C92AE9B96730A0F6166D1457ADDF4B7BBFC19B3CB356D8F4EB49472B681914F4CF762CEBAA7688263D8F03FC7D3BED65E11CFB98D9739492BE3439EECA694AF9EF109C9834EBA6F9A09B7D6CBA34C5B84C577B9F163BEA5F19D15A076B6A5B994F4462669ABAD4F9625568B2A25BAABCADA44B4DD552BAD0AF27E9D24FBA582A4447B1F0768DC2C8C32AB623D939CAB26FD4931CBBBD71F9714BF77542E6038A9D61C82813EF64E8C5D0D92A8C89A564600C3F1B0D6FE3651466AB5168CD5749AEF7017C50F9EDB7A1CDD859B2DEA0F8DED71C3238507777E10293AE8B3D48096B526FBFE743537B9FACC769D60C47D128844E8320C55936B8B135924DD728B019F221B0F5B45E8769BE0AD0BD73EC08FA13576CDB6767F10265F945B20CE33E856729EEB9A1599A61DB99CA7D0D42D834E58D45DB8A7C88A3524D8155A910DB2FBACA7019927DCAE7BADAED25065C1D9A23F70C932CF50C9BE7DA331FC35C55932A4BAE0A9B0E2CF7E7FD2BF38578665DE08772ACF8CFC401637315A3C67DB29573D1B28DA57B517F3F39182A5A73A298CC9AC98314D7C8524743FBD326E8295C4B6E34F7EEF6EEB7A58813E7322C007BCD935ADE59CE12FAF5344714B476E2840F65C729B9799E7C53B173957553F354C7CD6CBAC4CC5CA6BF952D0B752DEA204895F79A544D1759CEAAEAF3695A3DA4694523FEFC503645E613BB6CF0F58CD1A5C4258E0BAD90A83F9084044D97A62697E9CFA6B712588ABAF0D2AC979CA8FBC0524CD0AF2729F190A4C4F91A2DF1A754B780E767FED6BC3D0AAD9156863CED4DDA6D1A0A853E87F81B9DC6BD5750E86484371B2B21D57CD00915365D122A5CA6AB80A3C5E0CD46ABCA88261958537FF6A15EF42BEC435E2FF40C46CC5D9C9DEAF349E03E24813BBA1DA37630AC56E1E4B01D6089AE1733F36B6D962CCD169A185B41ABE9A4E18F9336946E2EC2C1FD8C9616A77387A4769A62E47A0C678551809D0FEFD0E331E39C126CBAD174DCCDCB1E3E21F4E7FB3073ED8ED7285FAC86E7DE321C9776836BFDCEB30F1B1C3B15998272A7A05C05F781CA99DF48EA3EEDD434FC85A4B0159FB95AE8A62D00879D33783300DC5C73D9DE2BA5B5B22BA92857D511FC40AA26FC55BF8D48B428B36AE9681E7FB0908213806FF53C0115D8CAB063B8D5D651694A4C269D82163D39F261FB93238EDE4AB2C903F05E67D7E34C74A7CFC331248E731DB5F6264DE211D6C3C31CAFE3C16D976C837557B87B3948746B34C3FC04F7DD26DF8D919702990AD6ED46F424D21E2AF362F0121EBB0D51F29FAB4267C8FB21657618BC70416A5AB1207F4EA7D626037910039924C16B579D21DB7CC52C608999F22A96F485AB3D6C0C28EB48A8CC4EF80B4D5DFD0597B59DE660A091EF27F34C41CB979DD48EB573EC96AC87329D66B03CAD3789EC4964FB5BD37014DA5200B052ACF7DE7FA8D60C1CF61ECA02931454D02A3B68943D0757CFAFACD8FBA4C89C177A9B21E716CBDB63738645E2382BAF6CBB2AB4D151BEBC8EFC7E333899BA455FBA032A43519A34CBA459E099388B50A65E316DBFB8E9A4BBBCCECB7FA15CE8153EEBB3D24B858EB2B6954852D594CF056B297CE2FD504CCF957368B959BDBEBE95F2AED9C15183D352931A57D062FA755829A20AA0DB3A8266D24093061A4C03F9553E2A59A9D051BD8565AD891C04252D310949052DE3A3827E7C9DF7A38505BDEFE3E38C238BCB7BC1CA7BA547BA50FABA142CE9F97C6852938A99548CC47C57454C795D757570937FF3AD13EADC65C1F207D0B33DC057DE636E9CDC1B480F2A3CA05E3A90EB5AFB07849A32931E54D0D22CADB9EE7D341742F6089E1D533D505A63287F4AE80DCE74711E9EDED9C071406F1C26C3BF1A5C9B6FC2AB825ED13F02B149C74E3A5621667417F4B7EA51F81AD6B4DC47D065FC8A2F5D630E1895EFCF38D0555736217A1E5BE33AD1F5FAFCE9717B9DD69C6ECE1737A93645EEE5F61D7AFFB46E11D4CF6965DAEACAC9BF393C1B61205B6A47A3523B1E83DA792DB946E9C886D828FDD8101BA51B3F14F988FDD8521BA5235B6AC3F7E4888B49E09134E7B3C993B1FCF48C65C703DBC22924A773DB5CD9C9A453D01AEFF8F63CC5A663C05E845339E2DA0BC3BD51E9755762DCB764F35A89EB4B8D93987D7A62D6B0EE6F7A35103AA5A97E34D0FCB5E24CA8B6C8964FF17C0D1795B8C99CD601C48293DE50D00A68471956C1BDCC900DBA27AC32F875E85583880359F6CE95F6BCA35F7AA3ECB8DFD2F1D585D27A6CD418FB15413B510B3251C5651CFBB243D733C50B2FEB431C7BFE3E74483447EDEF5300F664A3ECD0153C43EF8BA47C8CDE5A8BB72526F5AD9AE028FEA3301EE3F0224A4A4ADA2B0AFCD031DE84E0918C5EBFF92164B873C1131114E1220E750F0FF923F48FA2A5F3062FC2358AF6F7E6443F86744AEE1FFE757FEF7A814A76376AF0A4C83DA2DD26DFE362ED0BED6E059FCE9D74E0A4037BEBC0EBF22C9C930E6C4B4C3A5041ABEC9CE1973D37E35C0464BE3EC7576B2CAE9C19E3D2212F3416EC2551DB0AFE0DE1A7DC87AF570295F7D40DDEC374996BF83ED65F85E48588E164C00F7507D2640A4CA680C914A81EB1773306AA329339A0A0556F3E6875A89FB090F27AC511B65B13C33AAC1FCF6EA4EB93C9CF999BB27654BA748A7CD4AF887819FE969021DAFED80F3194E6DD61FA615BF6360EC622450F10F809BC0CE3305B7981AA04AC1728DA3E830DE8C5A8A1844EDF7D9E8C9AC9A8D9A151D3C6DF3BDEE5CC159B4C1B05AD5102EA35B1F0B6DBB183EBDE054AC7A2D39E831D94D00CA5D045D7069577D8A3CC518F32C7EE6566F508E904B8979E3B3B1C8513CE8EC621733C0E99C33745F84B88BFE070F88E1B8FD4F168A4A8B945EC441C7E1D7CB026936B32B9742657525C8428715C49E24A4D0697726815515FC6DD15ED45CA5EC44291B10F794E315D93F0D985F049D13D51B7CB55EE2A7F8482930852D09A1B6489172E1A6BF177A428E9B971AFDECF29ECEB11B6B65F8FB44D4F0409996143AB9372DA0FBBFEDF0996C148BCB6787AC8CBA28061C7DDCF19E0515E9E521A52EE4BF3F419E1E9F4DC64C5F4B6624EB32C5984543BD7F485336BF4120FF1A41CDF80B771B0D7DE7D632ADB9D9BAB5A57DFBC2315230D263238DC10BB8654FBD5FEBF48FDE6465658BB94C98A049F3F7B7628D124B6124E4B630545B324CE88F515C6B96C5885F122DCA0C8BD7A0294A595560E6F4B54CC798337382E0D2CF731B2A94D5DAC8281ABD6D640302D4D9D7972C070A79E69DB4D93AB22AE8C68B1491553A938C8B23CC4BC72512B4E72A76DE460DB89D38B891D7B68044676EC379B1A759015DECEF89990C8B31BFAFFA19269D98F20CEA4398ECCC861021C07627A6231A83D23F011D4641BB27354B670672C52BDFA5C3D0C9B297984FB0A6292FA5D6A7E400FC50E38F910BFC111CEF1DE293DE05E6E6A650B14C8D60CB13302BBCA00DCD5B46618FE02FB620406039B6D43B7B1EE77C25F97382EB21BFABF5A04B11F41DC45731C45108709300988E98945A0F68CC02150937F0011344FBE959C5D0D886A38D98F2016A9F21D7984031D9947A0068DC02350936DC87E0EF1375A76D76C5236C3C826F4237B36D179851C22C023557DF4800EADACE854EA545525E61BA88D363552E2012D04757BFF167E0C737313D98F607729776C248738422BA547369515533F93CCB4B77BD153B4B1344D56BFE5C92397D51BC8DF53356E0CCF4ED57C2B1F8E7D0278378B11F0CBD5E6C1861F81B3E025EDF4D13F7FCDC0F38FD40DB51AA6AFCFA8DC05F5B74D05B8723B6333BEFB5AD650F101FC39C4663A3ED0719A82824274818CEC8FCDF4951981CDF4FDED24C976CB63C0B36C3A0ED0BDD1C6F318F34EA293F16F7A8654A632908ED43777241E53F784A528DB1D77A91F8A500DBDC5AB11DCEA78F76491D36E92E1BD0968F57FF02D247D9D46603663DF3BADB03F0CAEEBDEF9B0E106E0D10FAFDC06BC38C6CBB2FA499201176BB7E64EA90D2373A63446B6065DF708D7CE34ADF0E09B4EFFA95E7FB3E0170BEDAA7A3A7B54CD0A377124AD0AF7C00FC44BF2B3E32673CAE87EAAC6DCD25CDBCAFFF46CB1F576FD3C196CBD7DDF1660C76EA7F6226DBD836877ABB6C816C2B309AE5EA9D5BDDC3B880DB2AFDD682EABCDF8D85486B9DC7FF7EB23101359AD63407D32D49A09444B2927B51362A075144D05C75E51D18C8B4D559A223B0EDFB052D0F2A7D6811C06D50C20EF462FAB9B38025FA97BC186F80A13CF6A878B2855F8927E538EF9C66FA0D8569B727D990568CD085C0234D8866A596C24DEA822BC49999C94C069A386A95B5C9E26030EA491E6D467D2B23A6A5C1CEF12F21AE7DDDAC96984D2F545B2DCDFEB22CA3BA3A9CB95384742E20C2C088CFBC009AF098535A0369F19B0E548D30C808602810DC0F5C493A0EA7443E97A8E49A5EB7443E936D64E2ADFE61810EA7005A97C9D6E28DD047448C59B0C43F93AC4472A5EA71B8734872BDF6418CAF33A5142E1B34D75E976C3E4EA74791628341C00C4A039166DAAFC7BB03D55962546BDAFA304AAF32DD0EA352C10A9CE338B86667910AA12B7706B2B0914424BC8B76222C19057F092F09551B0888F45013246FCC480C9BC59218131790614E6D66F0985C9B34169842C8C632982C55BBAD4436A390DF92B2864383EDF6252F367CAC1D9CD7F226032D6825E5D4AEFA1312535EA53BBFA23EE75581F0F6B7B42A5DA2583AADF7930251D79B384EF488B4EB63D95047474AF034D5C27B81E69623A02B47734FDED7A82C9716C7BF43C777E06E85EF5F91AC0C9114ED830B507ED343502D07ED8D2736F307F1A0468B1E6B808B024201E18616A0C5A971A08A0D12A03D3BDD9DC2105A0D5EA430C5C8DC1630C4C85419B528D003419B64ADD1BCC85DC030D5687E473D50583F299EAC236B81A62F816D74E85B2C5407439545D3EBEBC578BF980720602F67BDC5BCC4692030D56069A73958542CD4D755502008D859DCC3E3A32D7B6561D742EA82020EC9C5369905BA78618B2C5528039D46C7D143A5F71651C3ADB012A47D200057484CA9FDCA62B788759D71FEA2573454BC085F32D7A065C2E67F0B4BEBF7B1F29C27C813EB20908566F3EC921C1966DB281547011EC52F5EB23204C55D147A68056A9419A9056A18F944B1B36980A5E02D75DDCFB481D6C09FB79369199A2D76588CDE4ED7E664D46EFCB19A23121C7C5930307870A1AFA4B1153A86C951C55B86D3FC97184024BC1EB66FDE69D10B4A69873BAD036696E2882DB6C1A61C21A768EC181571A3164ABCDF4315A168DB1401B4B95694387946ACD3EE008D04756214752BBB54BAEAE54C6586F3246BC988C0663948C5AD7EBE264B636247481316EC3D67B69C534A34DF11CB6111D3D565A469EC56CC0817269CDE4AF4111093D16D6BC396BCDFD68EDEE799B777270BD58E135AA134E0EC8270B42BC4051A5969B8C4BB4D984F132EB4AD6297BD71BB420559EFDEBF5FEDEF7751467AFF65779BE7979709051E8ECD93A5CA44996DCE5CF16C9FA0005C9C1D1F3E77F3B383C3C585718070BAE37C5BDFE96529EA4688985DCF2FAC4009F856996977100B7A8D49BB3602D7DC6C40A28F6251A427238803C54CD464553A6FCBB2AC7DE6F4BBE4BEF9F6990BA8E3C236D5B971119F41E595E92AA4B93F2E5F59628056EAE9D2551B18ED5F121EAD2D50B1FBF966F2CB3204CB23D5699B4AE9ECE62B1986447ACB2A701AC2AD9018BD808E7810054A739D6E80AA3E02C424BA0565D963DE66285E21847D51EA6584529D3B1AE8D3C926ADA6438D7B3FC01D6B1CA70E514EA34CB9C022E61A8B1D2E61E5216A94D749E095213D974C716BE4BE96925A98975BA3D5A77D9298BD5A53A231D8148D2A5AB1648C720D2710FA4BF80487FE981F402447A01219D1C08425994FF0792021074B1A8536C358EDEF077543A1A303BBDA3051846F57C792F4FB626CD1E85791B94056292EDB198A70B592C26D91EAB794A94056AD21C5A471F4FE61A4653EC11846726592821CB11B3BBF95DC2ECB21C312FC32088B0A6B6E207BDF03535173FB0C7E75ED86471B90C273CB89E5C86139EAE7781EC1ED8CA1AF7EFD76B1C07F5FA3A375A5DB21BD665B6949168A2132FE558066A531D145611038DEB521DEB24CB2E26D905AB0E60BB11253C9FE32859DFA549B111AD5C3EC74132963E1E495BE02C2BDD42414A4AB9F6C8441BCE502AD6B34BB547A2EFF908384D9AB386EBAEA606F49CFADE6A0DEF11975DD42E4D9A3DCA7956073B718640A6886ED3F01C9986A48FE7C9759ECA13B4CB7118C74D7885F3228D014C316F32C527535C1F8FD8DF2087219DCD7215CC30C6F97956AEA785718103716A33190EE6F0F52F37B2B9DFA53A2001979B739816979FABD12777428F30B913933B31B913933B31B913933B31B913933B6185F494DC09E8D0D176DE047020DBDD97B00119C693F898FD0118FE4DA213CEF9464229939C30E6499A4B2855E2EE2CFBFAFD4CD1B8AF931DAD524080B2E94E761880D5A53A48F5380B030CF9307C8EC30814B90A52C89A64F19395C555F8D576D217C4B010B88A72C3C85859C0BA4AD76B2201CFE3007F170447973CB625D83D0AC4E2A89F0ADA1997A962045DB80CC4B0E03245B987CA656FD7288C04755225B9F045967DA31704F07CD1A4BAE8B7F299E240546F75A23DCEBBF2FE23414B3669F628B3557998180840E2325C5AB78CC26C25E371190EFDBE4A7241D3D649F618BFFDC60394BF1D7A28596F507C0F30119FE36045DCDD850B4C9A118B961C9BD10BEFEDF75C0949F3EC51DF276BA88E4CB2431FE22802B09864078F3808525C9E11E35CE12671F49583F20EB2320E1889B281CFB1477C1DA6F92A40F73C5A97EA60E3A33F7135FEB2D320E6D9A35EA02CBF4896612C830A590EFC916270E18F4DB7476B6E8D63915437C9ED4C7FAB2F9370D1E00A140B1DAE2C39D0CEDE9C4C5160E2B2E90E3CA8E0BF3EBCF7691380BCC7A63BA0D5AF8E72488A974877C67BAAEB1E5C380FC4B0E03B45B9876A3BBA69891D8D677DFDC676030A83588CA8AAE0431D52BA4F23C330C92E610179246AD62AE991B198EA4E1C170E03312C184C51EEA1F2D7F91A2DF1A7547016BA5497BA7C0D97440949605CC6D826AFCF451BF7E5161512F7522F0BA67DC277871B36F9F63A1806B1DA97810B3ED429F5438848C3F1659781D541590CAFBEF83083DCD014F745D87477B49B8B503CABC3E5F44004449798E7867ADABC2823429EAA9E9A51E331F7E1B3689A6BF2D55834DC000A65E232DC7B1088F511B25C1CB9EE2509DE9353BF30A1467B8DF2C54A64C036D16D73B47B3B54DC1F55BF2AAA463CCF3E6CB0300C4DDAB455C895794A5B85BA7BC69C0C01158E8D2DA02E3B8CA6A05BE41F80A0E20F2E28C9260F2439D4263A2C9ED37530201482CB709792F43E11482B68DFBE56E36ED22496ECA836D11E27CCF13A16446493668F926DB070DF4295628F700BC9EA5B77597D9B7C97EDCB36D11E67914462CC499DE43446B7214AFE7355C4D23875190E78A0FADEF4D0DD296040A7CE9EDFA47F9C907E0CFD3307AF4775D53E008AA5EE014B0EA3797CE98C56652ACD4367D99EC912239B6CC3A73C3735D73DBA2E25403096CB0870D1E19610A0E503E7A503C8EE7234B7BADB2F211B4EFD44B11EB1EC4D85FBCC643945B8D2D75A0A711382CB701244B974FB559BE85CAF2F6214039B3E89B6272FDAAACB9EFDC83710CB41C829CA0F23E9989BAEB98539F505D86AAC8717433BCD5727A41F63BEAA2EE5769DAB208EE53C55941DC81AF17251D897F7F01A3C9BEE8206590FEF9DED063FF3BCBC29A23CA22A9F5CE5739C10AF4BE64FCFE71260973149A2272B89B81BFDB71346ECCB95EEE2485B7A38F7C8AF4BD21ECF545F67E2BEE7E85F24D072B230E673EC116921F9C26026D9C1F8C27140CF5FCE51BE120C303ECBA1C59BF0AAA0C7BA254C216B12844F56109ADE95E975D4BBE745AE86F203ED223EC203DEE7F1A6C8E5487426D91E8B9E9616FCCB26CDB165959D7A7378063490C9EB877AA4413DEA8D7AAC413D76453DAF5F06023A80CDEA8509349FCDEA8509349ECD72C2FC50E4CAC67379FD5081E67379FD50810EE0F276E7904C7140D6484F499BDB3D1DE41E41AA45B45C7431A20CE7EDF88B279DA7185839E952DD2216C563D74D9A1B0A747EAD49B547FA14C3586CBA836F54DFFC095FD0E57A35D724A51C901EBE94AADE84A13326F3E07698E02CE493196218E154BDCE222F72B0E90EBBAEE89E0859E1F8789BE85A2B62E796E2FA4A0CD093737B20032BD9629E5308608E53618BB84D74AD9DEC0DF67B43276879AA203C9543904C663F5C35A85B509FCF37886ACEF85DC335BFF746FDBB06F5EF935279C24AE50CBD2F92F232D62DB58912C7428D68CA0EA43F50FC472147DA30C98E586244779B387660382D2149E22E75CC10F30C45B88843418477A96E48FF2800209AE810805AE4124C93E614F21E8B4F273669F628772B3916B6499BE4F19395C7EDE5D85BCA63258E853CD6941D461E97B4C4858626CD297A52167D1BF7233ABE8EC66C3C1F47D95E222FE463560BD703561B322EB964F676A96E48E5DD3C325295EAD033D40315FAA64E73E99DED8F20C9713DAE513DFE0E1D4DBAC109E907D10DF03DDE7DB443EF6BC5B5A587D1103E5FE8F1F3AA0EF9292F79B489F638BE0ECD939FC029DA2ED521A8A61C5EF9156E26B907961CF02364B960FA7F8DC8F7CB3B3EEF8A3F0BE3305BC9606CBA8BB42BE7AD8CC6A63BB65356E24CB223D6E9BBCF00144D9D34DF93D57C6D889397DB1AB46016FACF04308C0AF41129E533C2A95B3607D7FD9D7776172805C0BA5427A48F285D62D1D5E992EDB1662805D46A97EA20C60F01A036D101E708C23972C73986708E9D7166F5081D495DD424BBF411C0026DA24B1F413847EE38C710CEB13BCEE19B22FC25C45F7028B68DC970699F0AEFA81FDEB10AEFB8171E55DAC4A2C0E157C8026A732695FE74557A525C8428F1E2CE6AB16C14BABEFC30FAFC0ADA3EBE72DF3EDE78B8BD823E2828B4A6499BE6E8D39DA329BA27927FB9CAFD4C53039CCD4C35420CB443214FB2B9EB24F3B6D203AE10F789009A433B2673F71D936B6967E2DA7167E235B851F2BAC72E09994645944B538BA6B9AD11FA7A9FAFE35919AC4977E829E8C2B5D7EE17AECDE4DD8E99EB6EC73910B370EE1CB3E04B11922237D0FB0F6CFAA4CC1EBF323BCDB26411D2255BC5530D37D52B1816EF31345FC28F2EA81EF2082489DD61DD5C2745BA80969DACD496F22AF5B2575ABA8E55AA97457A56A9C470ABD1C9013846F6C35855BB7A07CFFC4C4AF321F8168AEA213FB1C318A42D87D0BDBF8C15DA6E0055C271C001AC6EC0B71941FE4BF8267DDB3164B1B61C440AE56114B92A3DBC616C64ED2C897314C638153F6985799DD2FECE9A8472C8D01257974674E5AE172BBC46B429D9062DA8F911E0B330CDF292016E5186AB4FF6F748DDBF86014E8959799F11F3E259F9C1B3EB7F44B328C4A52DD67C7089E2F00E67F9C7E44F1CBFDA3F7A7E48D4EB6914A2ACBC0F20BADBDFFBBE8EE2ECE5A2C8F2648DE238C969D35FEDAFF27CF3F2E020A314B367EB7041ACFBE42E7FB648D60728480E08D6F1C1E1E1010ED60762F11AD60AE5F9DF1A942C0BB8BB2B1867AA1EE0D3779F4FC9B0AF2F9225CF1327FF8125D66A06F80ADF31FC71208CB758F004E0A9B206AFF6C3B263E9747B87C9B8A31C07737A3620EEDED6DBDFFBB588A2F2E9CC57FB7728CA24734D84AF8F049491921595F82B4A172B44ACCE4BF4FD02C7CB7CF56AFFC57316384F0B236E393BD6D53EB206F7F0F9F37EC8D5AE37D32F8E182946E7C136006525AE300ACE22B4DC0287F4481C97B76C94A3D0D5C8E720D452D52F725DEFCAC5D1201FBDF8B91FE7D0E5FC0AB88CC0AD22D9DC80DA831F6A189729626A6ACFE17997A2601B56EEFC1FAF75EB9CA121608F8781FDCB30B02FB682659D279372698E954A27F77E58FDD25C10A79311CEE3C2049979C5658281B6133D4DF09A576EBCA471AB5E2185E82CFFD8DD1AA87FECCB3008223C64ED2B0A03B5818B63F38D3C5C9D07EDF5167F28BEC17150870A54B8FE602FB3A5C9D4EDC1833966703DC15E15F1107D90B78F036F273BDBB32E37826AEA21CBE913A606B3FAE8C50B67495C7ACE246D81B3AC3ADBEE550D114360865253B59D61E9F6B11EF4B0AF2AA617696D3760CD75AB5E27FC79466BB645B5CAB94DDF87BBCE53CF952B2F2FC4F4F2C201C027BFE071FA053715433F0EEFE03C2B9731C3B8C0DB2CC67CBCFEE566002FA30D2AAE021B44E926AE39BEA497CBBFDAFF2F5AFCE5DEF9EF3732C24F7B542FBDDC7BBEF7DF93D733793D93D733793D93D733793D93D7F368BC1E0BBB4028BE8D5130794D93D734794D80D7D4D9DE450C4526FFB03E537BA5BB5F67A73AB8A8D5183D30AB0BDF413162D5582BDFC4AEDBBA40E1FED5A1F631271B3D753F310187803D8FB330C0167E590FAFB1C807C39EA4F06391C26FF026CF1E89E0354ADD9F9FBB0B94EE51C2FE52C9CA06746694394A4947B91BB74D397BABD69A9B80A8D2C7CB4D2FDCB9E9ED1A85918B3F6E853A4759F6ADB49F7CD7362E3F6E616F4377CE27E31160BDD27477EE66AB302642D238403D1621DEC6CB28CC564340CF57496ED2C3CEA0BFFDE659FBCC92F506C5F7CE8C6A6591DCDD957728AF92D865623922BFFD9E7B067F9FAC07A9F40C47D110B8A74190E22CF3AD6E8651628D789E2127F96503FD3A4CF355503EDCBDE5AE09FA13570CE6631DF10265F945B20C631F60B3147B5ADE2CED407753A22A358021F1218E4A19FF484C89F33999933693D299996C18C9AA8A9F3681274E6ACED4B87152554AC5494013AC59093871F8C332D21036A90FB96E3D18D519CA69349498746BC811D82E56228F4C4B923B3109AC59E712C7C5C4399A2AAFD1127F4AF5567B8F41FE157D0D97A4D903400F63527A59B2196F7145A6FD39C4DFDA43EB6EE499A2035865D5C9DB690E0ECAD0D6A3D1BC2B4803521EC798B04F257A557F0DF0CD45080CBA7A62FDB72FBA9C40F2D8A853E201F60870608A6E23A9561891A23DE8B705B7A14EE3170609C86A868D0B48F2B47A59BF4ADAA3D3BA92DBF45A7B7D965FFB3245F7B4B7FA44E13265B76959F32C71EFA098695B133F926DCD96A51E8962A4BBF81F1C76F1AD50934D1E18CEF1DB0581D0D5BAAD83375883A6A74E638A6F234936CD857E5EA740F3369D57D0EA4123AFD11DB7161AA2C7F6CF6D73E35E0DEB4197D68F0D79D5CF1BF6A929BFC836C68AFB70A5A0D3F17FD7E8FB3F4F0AF0292B40528147A2FE7C29AAD62CD8C24875DAA500C54BA697003FF765AD69DAFEF0D3B6F1CB1FC9BC2D9B33C45A4E3FEB704BA3B06CCCFBA4C8FAB8B86CD96DEB50F207B73021046659C61DD317E90AC3A6502FF329BFDFF846ADEBFBA58BE6F0043C89CEC7263A6711CA1ECB1E45D71EBF2363DA22DBD9C1B6693A3EA6E94835DE23998A16F7E4F53065DE0FB581F17E5B0B6190E95DDE94521E521EE674F275C9DEE9F9DC33F224931E8B4C223C7255C494031F8B541AD827B15B55694EF26E7303CFF67BC1034A160A3D80FCA7B83697703B1B78380E686712165BF916E29BF0AAA0E7FBFD634FB2F6B1C8DA76623FA2BB929FD4C97EE685E32D57DDE9C179BD0BDB23789436B1329E6F0ECFFC8F490B7E3424F8F100E0E795893348AF34D843744A833D449F7C28F2E13AA5051FA2575A70EFDD329CCBA48DC4B28CA49D0C81C7610854613968518ABB3A82F191980383452BCF536C8E3C759F9565EF1B0EE3F703F5726C30F685D4DCC5DBF7D6B76DEF7B9B04D7E3105CD59B4F7422668FC889A95E4B32AE93B88FCE06DD1339EFFB147F555D622F9793F6CA10EBB805FC10ABF0B774B0F45BD67DAB3CC06A54D0327C41185EE53CBA2279AEA4DF47C36ABEFADD73540107FEF7296461D2307B90863943EF8BA4BC53F8B1A81614FF515844FFB8CFA712D8103ADF03D62220BF2FAA493AF7C03546FAF7C144112EE2507F65524FDC7F142D6C8017E11A45E5B3B3E4AF8CBE1F7BF857C2800B54221EB9C227453E20FA6DF23DEEDE5BF58D7EB73285324F4AE0292981F662F947A204CAA6785F11D90C7220CBE6A053AFBA5A1D201AE0AC973BE4823DB4E75BD46D082BE43E8CF512A8BC21CA777F52DFDE7B8F9ACEA4B9631A43B11ECC61B4492F3D2ABDF4A82EE61FEA7DAE419ED5223F8D4B3C3D0CF3616E7F203F677D1589E5BDFEE5E680C9B3741FBA16D7188475DC03DBF615B35ECF110C84ECEF4187B3300EB39517A84A0C7981A2ED33DA12EEFA94E29EBEFB3CE9D3499FEE41FAB40D887B54F7920C10BB661578E6B60BE15B6D2C503A10EC47942EB17E39CE1D778652DDE52C361067875B231C6D8D70BC2DC2AC1E37BDDC73EFE0B3C321D8E1EC6810D4E341500FDF14E12F21FE8243EFBD3018F2F150C8D41820460B0EBFFAEEE8C92078340641525C842879542EF6957977DE7209D570494ACF7746DF6841A7BDF8273C1B53744F14C172953FAA093937CEA41E0FAB0CB386344C0CD6DC62E7A8C7E9936BFF5B31AF87D93422D3A8884C1BFC7D963C875DF1EBE6E360245E0B17127A5A629D1937807A1CA818E2EA480B656DABF46F80575F264DFA343569F5C64BEF6777DA970F0ECCDF9644FA28DD9EAF2B185E56B09B2C7595DD2857A5067B1EA9F7BB9DF000809F3612C275AC46EA3099F0437BC8AA7A7DA3F750D1E2038F554BC3ADCFEA623FF2689D6659B2082999BA42E5B511F581A2F82EB9A187A06FF8446118DFC6C15EC9B762D9A651D738BA7B26665D12E32EDC44E18254EAD5FEF367CF0EA59E52E1DE702F9A03E8CD073C8D7F910810C6C1E5B337218A66499CE52922FD2F7359182FC20D8AE0360A9F5BFA57E580B4C062CE1BBC299F9F8D735DEB6DE8D6A5AA21842BD1D212668CA96B4E0E18CED13354BB817455C4958F7A636E1133F872796EE8A1EC1F96B980C63C6406EBAA5B8DEBCE788C3E074F9FF5CC0E958C54BD19CF8E6C9DE2C42EB44C59DA8CE4892980B7EE87E183AE6936C4BAA7C67632E4D4A0B8811F5CED46AB323BD891AA53F8913A145B7CF2217E83239CE3BD537A02BCDC83CB162890F53951AC818A7A533B967E9B3608AF0066D630BC0276BC82146C968DC427F4FDC51BFABF5A34548F34B2C354A73889065A46120D2092A7E1061E971C66B8BBA6FD00A2A1F2E46FA09737BBC1AA9F746547AA49721FF4D1061C7A8876C011B722C4BDFBB8CB01A76B0237D749912EC440A51EE33E904E609799E44AD4E93F38EF2897D214E41E10F7D4814CAAD193C70D1CB147C73BF6633932EB2857F2C6E09C8A652A4BD4207676CB38CC6AA95405D020FED1D846B51CFCD0B9C6206E76EBBEEC8A6B4673621CB966A77E4CB58C6E276CEA07AFF965B27C3C71C32EF9CB95189275A0A7BE87E11DE5B686829C7A1F6374EE79D0426777BC339AD871659DDDCA9DF69563F86D3E66E4DAC76BB971EB5245FE513BD50D291188A60D2338E09777076200A82715A4B8B7FF76CC01EC53B54A36E03E620790CF705A6471642D4F3CA16EEE706C61CF83D2B3C33B610EAE16AAE7AB7B0FA4812D7AB1DA8F28309C39917FD77E679CD13E0476A37821921F4BE9DDB07630991C670EA16421C001C586E245B7E1B843EE5B05ADE619CA9DF004F392CD8DEEA50560F75F8A2811725CF7FCDB177584ADFE2E7DD81DFE11A347E0C78314E4F8577176CE25DF14EFB1F153BCFA429CE375EA286ECBEE384AF162DDC3E026FEA1A79DE9A1AA0A563AC881951EA8EE198D195CF5CE43E084B2BEF67E8CF5F03D6043D56994C6B653DB21D9B103C3BDF3B0EB7057E8E909996984FC4712EC6A7C76434196793D61F77EB0F9ED10AF83FDC0854F9FE11C5B0E35337FC7E18C568AC9727DFE0173C5682BEDCE7CB0C2C4F0DFA1835C05324387657CC7315B72D18F16C36CCF5A6595461AE7EAEC0D29939312386D0FDC06F82C4CB3FC0DCAD12D025CDCB2D435CE3BB7F83442E9FA2259EEEF75C7793A8BA3CBBD5EACF01ABDDA0F6E4BB3A43A15C47D20B185448A335F206ADC070A82DC374E349B631606CACD6766FACD97865A40C749A43A401F413580BE33D0AFA7B244B24E87A8D45906E07A764BC0753A045C671980DB807609BACD81C0DB4C037C1D262581D7E910749D65006EA23E25E42603826EF20CD87508B0045DA743C87596913D73B83B9A0C980D739B0EE115BF4481CF86E8F05F985AD2EDF9C88DE9F2C0F674D91644E89E314882E6A808D04C8B1EAB5603C0DEAAB2543D55E55A52A8F73E9464EA7C1DADFA130B82F5621748ACCE5311AAB3CDA2BE5B1485247C97AB10ECDD07B6F25CA1C8847CAD14B75263A0BFA6984CC257EA39257C68D421C20B70903A113F81358BF8958172F732904C92C9836831D90622DDCB133211260F22C264DB10511A005CAE9290A5CA176E58D5F0A84E260ADF98687297B80124F97C9022FF898524E6EEA9824532FF894A36F35F59E9799519C1E5AA8C094BADCF06C6C2F68AC6D82AF3ED0C2E2E124E61086808D10F604A8CFBA2B7CBC515D23DA6A4C64ED72EAC6A9756DB3E30B9151A14DEA500B0142EC201DF2D165D667BE41DE8B65EA7E5B946AB3D18DA64B34FF220BA903BD10DF493FAC437B456C2D417F496E432D5314E53B91E0DE3CF2D032DD31C6C861670982A82939A2B23F86AB494CA11736F1A77D4166899FA282E5749CE79A255047D23B98C3466B04FE5DE30EE4029D030F58153AE92BCC7496B086B963EDDD1BF59DCB14975EBD4A72B3D341250D24C51D8A1DFB2C1CD95E5860643C1F25CD5E54A83D5DD417381E369406B4D87D8B66EAC6C167505613B64BBA6AA07D674F26A6BF93A4653A18340A02161382F24180DB938B0F0AA11504A6E2F97EEB7C1EAC1351E71D97A74476AAE74E0026AACFE54065F6D71A5ADAAB46A114D2ACB2CA0B525A1D5B16D9ACA2F1DEADAABDBD2536E6F32B5D72D526ED36D3D9AAF889D079A6F1365EF6DCC7B755DCFE60301E28AE69B42C9A52648CBA66D1B94ABA1200658DC43F3D5B1D0B0376D13380D3B82920BAD5DD114DD3F71A5B6F1FA944BB0DB754517F06BE8064564B03486DC6A763B88E042F50E9A0E44A72A26802E86D55BA387667A38045333E55DE4BD75C57726EE0C81864AC96F1F9E38C02A9A664783E927FD16C596CA112461509316A1677E1BB913AE02E2C6946B4A9653A98F693C5673D9F828E5AA201803B4E5A2A065A7689AD45CC1DB8601B5792707D53A7D9D407EE6498A9698EA988CA69E1C109D53DE725FFD7A83B370D9419C10CC182FB8B0A3F69B727E37D14F428D9A4F843B892F718E0294A3D3340FEF08CB93EC0526B2225EEEEF7D4651413E79BBBEC5C179FCA1C837454E9A8CD7B7D13DDB196514958EFEC98154E7930F9BF257E6A309A49A61F930C087F8751146415BEF33E06A640544199E55BF68528E655EBE6CB2BC6F917E4D624BA0BAFBDAA8B28F78BD89CA2DD50FF1352A1FDA72AF1BE1BD0BBC448BD20FFB1A06E5ABA02A10F340F0DD7EF22644CB14ADB31AA32B4F7E121E0ED6DFFFEDFF03F8676C8610B00200 , N'6.4.4')
 */