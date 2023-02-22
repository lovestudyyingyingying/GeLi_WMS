using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

namespace GeLiPage_WMS
{
    /// <summary>
    /// HTML转PDF帮助类
    /// </summary>
    public class HtmlToPdfHelper
    {
        /// <summary>
        /// 准备好的html字符串
        /// </summary>
        private string m_HtmlString;

        ///<summary>
        ///PDF 保存文件名
        ///</summary>
        private string m_PdfFilename;


        /// <summary>
        /// PDF保存目录（绝对路径）
        /// </summary>
        private string m_PDFSaveFloder;

        /// <summary>
        /// 图片XY字典，例如 [{img1, 100,200},{img2,20,30}}
        /// </summary>
        private Dictionary<string, Tuple<float, float>> m_ImageXYDic = null;

        public HtmlToPdfHelper(string htmlString, string pdfSaveFloder, Dictionary<string, Tuple<float, float>> imageXYDic,string pdffilename)
        {
            m_HtmlString = htmlString;
            m_PDFSaveFloder = pdfSaveFloder;
            m_ImageXYDic = imageXYDic;
            m_PdfFilename = pdffilename;
        }

        //生成PDF
        public bool BuilderPDF()
        {
            try
            {
                string pdfSavePath = Path.Combine(m_PDFSaveFloder,m_PdfFilename +".pdf");//Guid.NewGuid().ToString()
                if (!Directory.Exists(m_PDFSaveFloder))
                {
                    Directory.CreateDirectory(m_PDFSaveFloder);
                }
                using (FileStream fs = new FileStream(pdfSavePath, FileMode.OpenOrCreate))
                {
                    byte[] htmlByte = ConvertHtmlTextToPDF(m_HtmlString);
                    fs.Write(htmlByte, 0, htmlByte.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("保存PDF到磁盘时异常", ex);
            }
        }
        public float GetPdfSize(float size)

        {

            return (size / 10) / (float)2.54 * (float)72;

        }
        //将html字符串转为字节数组（代码来自百度）
        private byte[] ConvertHtmlTextToPDF(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return null;
            }
            //避免當htmlText無任何html tag標籤的純文字時，轉PDF時會掛掉，所以一律加上<p>標籤
            //htmlText = "<p>" + htmlText + "</p>";

            try
            {
                MemoryStream outputStream = new MemoryStream(); //要把PDF寫到哪個串流
                byte[] data = Encoding.UTF8.GetBytes(htmlText); //字串轉成byte[]
                MemoryStream msInput = new MemoryStream(data);

                // 设置页面
                iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(GetPdfSize(75), GetPdfSize(100), 1);

                pageSize.BackgroundColor = new iTextSharp.text.BaseColor(0xFF, 0xFF, 0xFF);
                Document doc = new Document(pageSize, 0, 0, 0, 0);
                //document.SetMargins(0f, 0f, 0f, 0f);
                doc.SetPageSize(pageSize);


                //Document doc = new Document(); //要寫PDF的文件，建構子沒填的話預設直式A4
                PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);  

                //指定文件預設開檔時的縮放為100%
                PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
                //開啟Document文件 
                doc.Open();

                #region 图片的处理
                CssFilesImpl cssFiles = new CssFilesImpl();
                cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
                var cssResolver = new StyleAttrCSSResolver(cssFiles);

                var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor(m_ImageXYDic)); // use new processor

                var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors

                var charset = Encoding.UTF8;
                var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(doc, writer));
                var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                var worker = new XMLWorker(pipeline, true);
                var xmlParser = new XMLParser(true, worker, charset);
                xmlParser.Parse(new StringReader(htmlText));
                #endregion

                //使用XMLWorkerHelper把Html parse到PDF檔裡
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());

                //將pdfDest設定的資料寫到PDF檔
                PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
                writer.SetOpenAction(action);

                doc.Close();
                msInput.Close();
                outputStream.Close();

                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("转PDF时异常", ex);
            }
        }

        //字体工厂（代码来自百度）
        public class UnicodeFontFactory : FontFactoryImp
        {
            private static readonly string arialFontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                , "C:\\WINDOWS\\FONTS\\simsun.ttc,0");//arial unicode MS是完整的unicode字型。

            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color,
                bool cached)
            {
                BaseFont baseFont = BaseFont.CreateFont(arialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                return new Font(baseFont, size, style, color);
            }
        }

        //自定义的图片处理类（代码来自百度）
        public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
        {
            private float _offsetX=0;
            private float _offsetY=0;

            private Dictionary<string, Tuple<float, float>> _imageXYDict;
            public CustomImageTagProcessor(Dictionary<string, Tuple<float, float>> imageXYDict)
            {
                _imageXYDict = imageXYDict;
            }

            protected void SetImageXY(string imageId)
            {
                if (_imageXYDict == null)
                {
                    return;
                }
                Tuple<float, float> xyTuple = null;
                _imageXYDict.TryGetValue(imageId, out xyTuple);

                if (xyTuple != null)
                {
                    _offsetX = xyTuple.Item1;
                    _offsetY = xyTuple.Item2;
                }
            }

            public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
            {
                IDictionary<string, string> attributes = tag.Attributes;
                string src;
                if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
                    return new List<IElement>(1);

                if (string.IsNullOrEmpty(src))
                    return new List<IElement>(1);

                string imageId;
                if(!attributes.TryGetValue(HTML.Attribute.ID, out imageId))
                    return new List<IElement>(1);

                if (string.IsNullOrEmpty(imageId))
                    return new List<IElement>(1);

                SetImageXY(imageId);

                if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
                {
                    // data:[][;charset=][;base64],
                    var base64Data = src.Substring(src.IndexOf(",") + 1);
                    var imagedata = Convert.FromBase64String(base64Data);
                    var image = iTextSharp.text.Image.GetInstance(imagedata);

                    var list = new List<IElement>();
                    var htmlPipelineContext = GetHtmlPipelineContext(ctx);
                    list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext), _offsetX, _offsetY, true), tag, htmlPipelineContext));
                    return list;
                }
                else
                {
                    return base.End(ctx, tag, currentContent);
                }
            }
        }
    }
}
