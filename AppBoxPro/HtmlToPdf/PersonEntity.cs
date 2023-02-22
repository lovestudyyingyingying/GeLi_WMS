using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf.codec;

namespace GeLiPage_WMS
{
    /// <summary>
    /// 测试类
    /// </summary>
    public class PersonEntity
    {
        /// <summary>
        /// html模版绝对路径
        /// </summary>
        public string m_HtmlTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model/person.html");

        /// <summary>
        /// PDF生成的目录（绝对路径）
        /// </summary>
        public string m_PdfSaveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"pdf");

        public PersonEntity()
        {
        }

        /// <summary>
        /// 生成PDF
        /// </summary>
        public void BuildPDF(string filename)
        {
  
            using (StreamReader reader = new StreamReader(m_HtmlTemplatePath))
            {
                string htmlStr = reader.ReadToEnd();//读取html模版

                string iamgeBase64Str1 = ImageToBase64String(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model/img1.jpg"));
                string iamgeBase64Str2 = ImageToBase64String(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Model/img2.jpg"));

                htmlStr = htmlStr.Replace("@PersonName", "张三");
                htmlStr = htmlStr.Replace("@PersonImage1", iamgeBase64Str1);
                //htmlStr = htmlStr.Replace("@PersonImage2", iamgeBase64Str2);

                Dictionary<string, Tuple<float, float>> imageXYDic = new Dictionary<string, Tuple<float, float>>();
                imageXYDic.Add("img1", new Tuple<float,float>(10,20));
                imageXYDic.Add("img2", new Tuple<float, float>(200, 300));

                HtmlToPdfHelper pdfHelper = new HtmlToPdfHelper(htmlStr, m_PdfSaveFolder, imageXYDic,filename);

                pdfHelper.BuilderPDF();//生成PDF
            }
        }

        //图片转为base64字符串
        public string ImageToBase64String(string imagePath)
        {
            try
            {
                Bitmap bitmap = new Bitmap(imagePath);

                MemoryStream ms = new MemoryStream();

                bitmap.Save(ms, ImageFormat.Jpeg);
                byte[] bytes = new byte[ms.Length];

                ms.Position = 0;
                ms.Read(bytes, 0, (int) ms.Length);
                ms.Close();

                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("图片转base64字符串时异常", ex);
            }
        }
    }
}
