using GeLiData_WMS;
using GeLiData_WMS.Dao;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLiData_WMSEntry
{
    public class Model_Data:DbContext
    {
        public Model_Data()
              : base("data source=127.0.0.1;initial catalog=GeLi_WMS;persist security info=True;user id=tzhuser;password=tzhuser;MultipleActiveResultSets=True;App=EntityFramework")
        {
            //this.Database.Log = s =>
            //{
            //    Debug.WriteLine(s);
            //};
        }
        public Model_Data(string connectionString) : base(connectionString)
        {
            //this.Database.Log = s =>
            //{
            //    Debug.WriteLine(s);
            //};
        }
        
        public virtual DbSet<AGVAlarmLog> AGVAlarmLog { get; set; }
        public virtual DbSet<AGVMissionInfo> AGVMissionInfo { get; set; }
        public virtual DbSet<AGVMissionInfo_Floor> AGVMissionInfo_Floor { get; set; }
       
        public virtual DbSet<Depts> Depts { get; set; }
        public virtual DbSet<DeviceStatesInfo> DeviceStatesInfo { get; set; }
        public virtual DbSet<FaHuoPlan> FaHuoPlan { get; set; }

        public virtual DbSet<Menus> Menus { get; set; }
        public virtual DbSet<Onlines> Onlines { get; set; }
        public virtual DbSet<Powers> Powers { get; set; }

        public virtual DbSet<Roles> Roles { get; set; }

        public virtual DbSet<StockPlan> StockPlan { get; set; }

        public virtual DbSet<TiShengJiState> TiShengJiState { get; set; }
        public virtual DbSet<Titles> Titles { get; set; }
        public virtual DbSet<TouLiaoRecord> TouLiaoRecord { get; set; }
        public virtual DbSet<TrayPro> TrayPro { get; set; }
        public virtual DbSet<TrayState> TrayState { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<WareArea> WareArea { get; set; }
        public virtual DbSet<WareAreaClass> WareAreaClass { get; set; }
        public virtual DbSet<WareHouse> WareHouse { get; set; }
        public virtual DbSet<WareLocation> WareLocation { get; set; }

        public virtual DbSet<TrayWeightRecord> TrayWeightRecord { get; set; }
        public virtual DbSet<TiShengJiInfo> TiShengJiInfo { get; set; }
        public virtual DbSet<StockRecord> StockRecord { get; set; }
        public virtual DbSet<MaPanJiState> MaPanJiState { get; set; }
        public virtual DbSet<MaPanJiInfo> MaPanJiInfo { get; set; }
        public virtual DbSet<ProcessTypeParam> ProcessTypeParam { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<AGVMissionInfo>()
                .Property(e => e.SendState)
                .IsUnicode(false);

            modelBuilder.Entity<AGVMissionInfo>()
                .Property(e => e.StateMsg)
                .IsUnicode(false);

            modelBuilder.Entity<AGVMissionInfo>()
                .Property(e => e.RunState)
                .IsUnicode(false);

            modelBuilder.Entity<AGVMissionInfo_Floor>()
                .Property(e => e.SendState)
                .IsUnicode(false);

            modelBuilder.Entity<AGVMissionInfo_Floor>()
                .Property(e => e.StateMsg)
                .IsUnicode(false);

            modelBuilder.Entity<AGVMissionInfo_Floor>()
                .Property(e => e.RunState)
                .IsUnicode(false);

          

            modelBuilder.Entity<Depts>()
                .HasMany(e => e.Depts1)
                .WithOptional(e => e.Depts2)
                .HasForeignKey(e => e.ParentID);

            modelBuilder.Entity<Depts>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.Depts)
                .HasForeignKey(e => e.DeptID);

            modelBuilder.Entity<Menus>()
                .HasMany(e => e.Menus1)
                .WithOptional(e => e.Menus2)
                .HasForeignKey(e => e.ParentID);

            modelBuilder.Entity<Powers>()
                .HasMany(e => e.Menus)
                .WithOptional(e => e.Powers)
                .HasForeignKey(e => e.ViewPowerID);

            modelBuilder.Entity<Powers>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.Powers)
                .Map(m => m.ToTable("RolePowers").MapLeftKey("PowerID").MapRightKey("RoleID"));

           

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Roles)
                .Map(m => m.ToTable("RoleUsers").MapLeftKey("RoleID").MapRightKey("UserID"));

           

            modelBuilder.Entity<Titles>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Titles)
                .Map(m => m.ToTable("TitleUsers").MapLeftKey("TitleID").MapRightKey("UserID"));

            modelBuilder.Entity<TrayState>()
                .Property(e => e.boxName)
                .IsFixedLength();

            modelBuilder.Entity<TrayState>()
                .HasMany(e => e.TrayPro)
                .WithRequired(e => e.TrayState)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Onlines)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.WareLocation)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.header_ID);

            modelBuilder.Entity<WareArea>()
                .HasMany(e => e.WareLocation)
                .WithOptional(e => e.WareArea)
                .HasForeignKey(e => e.WareArea_ID);

            modelBuilder.Entity<WareAreaClass>()
                .HasMany(e => e.WareArea)
                .WithOptional(e => e.WareAreaClass)
                .HasForeignKey(e => e.War_ID);

            modelBuilder.Entity<WareHouse>()
                .HasMany(e => e.WareArea)
                .WithOptional(e => e.WareHouse)
                .HasForeignKey(e => e.WareHouse_ID);

            modelBuilder.Entity<WareLocation>()
               .HasOptional(b => b.TrayState)
                 .WithMany()
                 .HasForeignKey(b => b.TrayState_ID);

      
        }

    }
}
