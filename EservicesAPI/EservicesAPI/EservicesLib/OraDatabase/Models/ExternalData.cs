using BaseLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EservicesLib.OraDatabase.Models
{

    public class ED_Template
    {
        public int ID { get; set; }
        public string templateName { get; set; }
        [DateTimeFormat("dd-MM-yyyy HH:mm:ss")]
        public string templateDate { get; set; }

    }

    public class ED_Contact
    {
        public int UN_ID { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
    }


    public class ED_Package
    {
        public int TemplateID { get; set; }
        public string JSON { get; set; }
    }
}
