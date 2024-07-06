using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using Newtonsoft.Json;

namespace MSLA.API.Providers
{

    public class  CustomAuthorize : System.Web.Http.AuthorizeAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }   
    public class CustomUserStore
    {
    }

    public class User
    {
        public Int64 fldUserId { get; set; } 
        public string fldUserName { get; set; } 
        public string fldFullUserName { get; set; }
        public string fldEmailAddress { get; set; }
        public bool fldActiveUser { get; set; } 
    }

    public class Helper
    {
        public static string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }

        //public static string GetString(string input)
        //{
        //    HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

        //    byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

        //    byte[] byteHash = hashAlgorithm.(byteValue);

        //    return Convert.ToBase64String(byteHash);
        //}
    }

    public class Menu
    {
        public Int64 fldWebMenu_Id { get; set; }
        public Int64 fldModule_ID { get; set; }
        public Int64 fldParentMenu_ID { get; set; }
        public string fldMenuName { get; set; }
        public string fldstateName { get; set; }
        public bool fldisabstract { get; set; }
        public string fldtemplateUrl { get; set; }
        public string fldurl { get; set; }
        public string fldcontrollerName { get; set; }
        public string fldcontrollerNameAs { get; set; }
        public string fldMenuType { get; set; }

        public string fldIcon { get; set; }
        public decimal fldPriority { get; set; }

        public List<Menu> fldChildren { get; set; }
    }

     
    public class MenuData
    { 
        public string name { get; set; }
        public string icon { get; set; } 
        public string type { get; set; }
        public decimal priority { get; set; }
        public string state { get; set; } 
        public List<MenuData> children { get; set; }
        public Int64 fldParentMenu_ID { get; set; }
    }

    public class MenuFinal 
    {
        [JsonProperty("Menudatas")]
        public List<MenuData> Menudata { get; set; }
    }

    //public class Feedbacks
    //{
    //    public Int64 user_ID { get; set; } 
    //    public string webClientID { get; set; }

    //    public string menu { get; set; }
    //    public string description { get; set; } 
    //    public DateTime updatedOn { get; set; } 

    //    public Feedbacks()
    //    { 
    //    }
    //}

}