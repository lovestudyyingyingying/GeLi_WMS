using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeLiPage_WMS.GeLiPage
{
    public partial class ImageLabelMain_UC : System.Web.UI.UserControl
    {
        #region 变量

        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set { _ImagePath = value; }
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _TitleColor;
        public string TitleColor
        {
            get { return _TitleColor; }
            set { _TitleColor = value; }
        }

        private string _TitleSize;
        public string TitleSize
        {
            get { return _TitleSize; }
            set { _TitleSize = value; }
        }

        private string _Value;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private string _ValueColor;
        public string ValueColor
        {
            get { return _ValueColor; }
            set { _ValueColor = value; }
        }

        private string _ValueSize;
        public string ValueSize
        {
            get { return _ValueSize; }
            set { _ValueSize = value; }
        }

        #endregion 变量

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                Image1.ImageUrl = ImagePath;
                labTitle.Text = Title;
                labTitle.CssStyle = $"font-size:{TitleSize};color:{TitleColor}";

                labValue.Text = Value;
                labValue.CssStyle = $"font-size:{ValueSize};color:{ValueColor}";
            }
        }
    }
}