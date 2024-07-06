namespace MSLA.Server.Login.API
{
    public class WebClient
    { 
        public string fldWebClientId { get; set; }
      
        public string fldSecretCode { get; set; }
      
        public string fldName { get; set; }
        public int fldApplicationType { get; set; }
        public bool fldActive { get; set; }
        public int fldRefreshTokenLifeTime { get; set; }
      
        public string fldAllowedOrigin { get; set; }
    }
}