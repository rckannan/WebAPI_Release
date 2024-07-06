using System;

namespace MSLA.Server.Exceptions.API
{
    public class ExceptionLog
    {
        public Int64 flduser_ID { get; set; }
        public string fldWebClient_Id { get; set; }

        public string fldEx { get; set; }
        public string fldstatus { get; set; }

        public string fldstatusText { get; set; }
        public string fldstack { get; set; }

        public string fldstackArg { get; set; }
        public DateTime fldtimestamp { get; set; }

        public string fldmenu { get; set; }

        public ExceptionLog()
        {
            fldWebClient_Id = string.Empty;
        }
    }
}