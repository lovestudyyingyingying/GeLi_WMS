using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace GeLiPage_WMS
{
    public class LocalAppContext : DbContext
    {
        public LocalAppContext()
            : base("LOCAL")
        {
            //Database.SetInitializer<DbContext>(new DropCreateDatabaseAlways<DbContext>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //这句是不要将EF生成的sql表名不要被复数 就是表名后面不要多加个S
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<WeiXinSetting> weiXinSettings { get; set; }
    }
}