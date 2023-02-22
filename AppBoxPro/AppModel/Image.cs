using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace GeLiPage_WMS
{
    public class Image:IKeyID
    {
        [Key]
        public int ID { get; set; }

        public string type { get; set; }

        public string name { get; set; }

        //public Reimbursement reimbursement { get; set; }
    }
}