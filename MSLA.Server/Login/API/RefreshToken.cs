using System;

namespace MSLA.Server.Login.API
{
    public class RefreshToken
    {
       
        public Int64 fldUserId { get; set; }
      
        public string fldSubject { get; set; }
      
        public string fldWebClientId { get; set; }
        public DateTime fldIssuedUtc { get; set; }
        public DateTime fldExpiresUtc { get; set; }
       
        public string fldProtectedTicket { get; set; }

        public string fldRemoteIP { get; set; }
    }
}