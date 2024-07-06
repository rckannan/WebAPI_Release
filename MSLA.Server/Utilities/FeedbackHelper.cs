using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSLA.Server.Utilities
{
    public class FeedbackHelper
    {
        public Int64 flduser_ID { get; set; }
        public string fldwebClientID { get; set; } 
        public string fldmenu { get; set; }
        public string flddescription { get; set; }
        public DateTime fldupdatedOn { get; set; }
    }
}
