using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace GeLiPage_WMS.Business.Helper
{
    public class ExcelUtils
    {
      
        //复制要导出的excel文件
        HSSFWorkbook xlsWorkBook=null;
        public ExcelUtils()
        {
            ////读入刚复制的要导出的excel文件
            //using (FileStream file = new FileStream(Form1.dataPath+@"模板\XlsModel.xls", FileMode.Open, FileAccess.Read))
            //{
            //    xlsWorkBook = new HSSFWorkbook(file);
            //    file.Close();
            //}
        }

        #region 读取Excel

        public DataTable GetDtInPath(string path)
        {
            try
            {
                List<DataTable> list = new List<DataTable>();
                DataTable dtAll = null;
                foreach (string temp in Directory.GetFiles(path))
                {
                    if (temp.EndsWith(".xls"))
                    {
                        DataTable dt = ReadExcel(temp);
                        list.Add(dt);
                    }
                }
                if (list.Count > 0)
                {
                    dtAll = new DataTable();
                    dtAll.Columns.Add("程序序号");
                    dtAll.Columns.Add("程序名称");
                    dtAll.Columns.Add("测试结果");
                    dtAll.Columns.Add("检测值");
                    dtAll.Columns.Add("单位");
                    dtAll.Columns.Add("测试日期", typeof(DateTime));
                    dtAll.Columns.Add("测试时间");
                }
                DataTable newDataTable = null;
                if (list.Count > 0)
                {
                    newDataTable = list[0].Clone();

                    object[] obj = new object[newDataTable.Columns.Count];

                    foreach (DataTable temp in list)
                    {
                        for (int i = 0; i < temp.Rows.Count; i++)
                        {
                            temp.Rows[i].ItemArray.CopyTo(obj, 0);
                            newDataTable.Rows.Add(obj);
                        }
                    }
                }
               
                return newDataTable;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            
            

        }

        public static DataTable ReadExcel(string excelPath)
        {
            
            //@"F:\项目\工作计划\GPS位置获取\模板_1.xls"
            //定义excel模板路径
            string xlsModelPath = excelPath;
            //复制要导出的excel文件
            HSSFWorkbook xlsWorkBook;
            //读入刚复制的要导出的excel文件
            using (FileStream file = new FileStream(xlsModelPath, FileMode.Open, FileAccess.Read))
            {
                xlsWorkBook = new HSSFWorkbook(file);
                file.Close();
            }
            HSSFSheet sheet1 = (HSSFSheet)xlsWorkBook.GetSheetAt(0);
            DataTable dt=GetValue(sheet1);
            

            return dt;

        }


        /// <summary>
        /// 对Excel插入值
        /// </summary>
        /// <param name="sheet1">sheet表对象</param>
        /// <param name="row">行数</param>
        /// <param name="cell">列数</param>
        /// <param name="value">插入值</param>
        private static DataTable GetValue(HSSFSheet sheet1)
        {
            DataTable dt = new DataTable();
            dt = new DataTable();
            dt.Columns.Add("物料名称");
           
            HSSFRow hrow = null;
            HSSFCell hcell = null;
            for (int rowIndex = 1; rowIndex < 60000; rowIndex++)
            {
                hrow = (HSSFRow)sheet1.GetRow(rowIndex);
               
                if (hrow != null)
                {
                    DataRow dr = dt.NewRow();
                   
                    for (int columIndex = 0; columIndex < dt.Columns.Count; columIndex++)
                    {
                        hcell = (HSSFCell)hrow.GetCell(columIndex);
                        //Console.WriteLine(hcell.ToString());
                        //if (columIndex== dt.Columns.Count - 1)
                        //{
                        //    Console.WriteLine(hcell.ToString());
                        //    dr[columIndex] = DateTime.Parse(hcell.ToString());
                        //}

                        //else
                        //if (hcell == null)
                        //{
                        //    if (dt.Columns[columIndex].ColumnName == "测试日期")
                        //        dr[columIndex] = DateTime.Parse("1900-01-01");
                        //    else
                        //        dr[columIndex] = string.Empty;
                        //}
                        //else
                        //{
                        //    if (dt.Columns[columIndex].ColumnName == "测试日期")
                        //        dr[columIndex] = DateTime.Parse(hcell.ToString());
                        //    else
                        //        dr[columIndex] = hcell.ToString();
                        //}
                        dr[columIndex] = hcell.ToString();
                    }
                    dt.Rows.Add(dr);
                }
                else
                    break;
                    
            }
           
            return dt;
        }



        #endregion

        #region 修改Excel

      
        public class FileTimeInfo
        {
            public string FileName;  //文件名
            public DateTime FileCreateTime; //创建时间
        }
        private FileTimeInfo GetLatestFileTimeInfo(string dir, string ext)
        {
            List<FileTimeInfo> list = new List<FileTimeInfo>();
            DirectoryInfo d = new DirectoryInfo(dir);
            foreach (FileInfo file in d.GetFiles())
            {
                if (file.Extension.ToUpper() == ext.ToUpper())
                {
                    list.Add(new FileTimeInfo()
                    {
                        FileName = file.FullName,
                        FileCreateTime = file.CreationTime
                    });
                }
            }
            var f = from x in list
                    orderby x.FileCreateTime
                    select x;
            return f.LastOrDefault();
        }

        #endregion
        public string ReckonSeconds(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts = (dt2 - dt1).Duration();

            double second = 0;
            if (ts.Hours > 0)
            {
                second += ts.Hours * 3600;
            }
            if (ts.Minutes > 0)
            {
                second += ts.Minutes * 60;
            }
            second += ts.Seconds;
            second += (ts.Milliseconds * 0.001);

            return second.ToString();

        }
    }
}
