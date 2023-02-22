using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeLiPage_WMS.GeLiPage
{
    public partial class WareAreaState : System.Web.UI.UserControl
    {
        private string _WareAreaTitle;
        public string WareAreaTitle
        {
            get { return _WareAreaTitle; }
            set { _WareAreaTitle = value; }
        }

        private string _HadUse;
        public string HadUse
        {
            get { return _HadUse; }
            set { _HadUse = value; }
        }

        private string _NoHadUse;
        public string NoHadUse
        {
            get { return _NoHadUse; }
            set { _NoHadUse = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                
                labWareArea.Text = WareAreaTitle;
                ImageLabel_UC1Used.Value = HadUse;
                ImageLabel_UC2NoUsed.Value = NoHadUse;
            }
        }
    }
}