using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Results;
using Microsoft.Ajax.Utilities;

namespace MSLA.API.Providers
{
    public class AuthRepository1 : IDisposable
    {
        //public WebClient  FindWebClient(string webClientId)
        //{
        //    var cmd = new SqlCommand
        //    {
        //        CommandText = "select * from Main.tblWebClients where fldWebClient_ID = '" + webClientId + "'",
        //        CommandType = CommandType.Text
        //    };

        //    var dt = DataConnect.DataConnect.FillDt(ref cmd);
        //    var client = new WebClient();

        //    if (dt.Rows.Count <= 0) return null;
        //    client.fldWebClientId = Convert.ToString(dt.Rows[0]["fldWebClient_Id"]);
        //    client.fldSecretCode = Convert.ToString(dt.Rows[0]["fldSecretCode"]);
        //    client.fldAllowedOrigin = Convert.ToString(dt.Rows[0]["fldAllowedOrigin"]);
        //    client.fldName = Convert.ToString(dt.Rows[0]["fldName"]);
        //    client.fldActive = Convert.ToBoolean(dt.Rows[0]["fldActive"]);
        //    client.fldApplicationType = Convert.ToInt32(dt.Rows[0]["fldApplicationType"]);
        //    client.fldRefreshTokenLifeTime = Convert.ToInt32(dt.Rows[0]["fldRefreshTokenLifeTime"]);
        //    return client;
        //}

        public User FindUser(string fldUser )
        {
            var cmd = new SqlCommand
            {
                CommandText = "select * from Main.tblUser where fldUserName = '" + fldUser + "'"  ,
                CommandType = CommandType.Text
            };

            var dt = DataConnect.DataConnect.FillDt(ref cmd);
            var user = new User();

            if (dt.Rows.Count <= 0) return null;
            user.fldUserId = Convert.ToInt64(dt.Rows[0]["fldUser_Id"]);
            user.fldEmailAddress = Convert.ToString(dt.Rows[0]["fldEmailAddress"]);
            user.fldFullUserName = Convert.ToString(dt.Rows[0]["fldFullUserName"]);
            user.fldActiveUser  = Convert.ToBoolean(dt.Rows[0]["fldActiveUser"]);
            user.fldUserName = Convert.ToString(dt.Rows[0]["fldUserName"]);
            return user;
        }

        public Task<bool> AddRefreshToken(RefreshToken token)
        {
            var cmd = new SqlCommand
            {
                CommandText = "Main.spRefreshTokenAddUpdate",
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@user_ID", token.fldUserId);
            cmd.Parameters.AddWithValue("@UserName", token.fldSubject);
            cmd.Parameters.AddWithValue("@WebClient_Id", token.fldWebClientId);
            cmd.Parameters.AddWithValue("@IssuedUtc", token.fldIssuedUtc);
            cmd.Parameters.AddWithValue("@ExpiresUtc", token.fldExpiresUtc);
            cmd.Parameters.AddWithValue("@ProtectedTicket", token.fldProtectedTicket);
            cmd.Parameters.AddWithValue("@RemoteIP", token.fldRemoteIP);
            var parm = new SqlParameter("@RefreshToken_Id", SqlDbType.BigInt) {Direction = ParameterDirection.Output};
            cmd.Parameters.Add(parm);

            return Task.Factory.StartNew(() =>
            {
                DataConnect.DataConnect.ExecCmm(ref cmd);

                return Convert.ToBoolean(Convert.ToInt64(cmd.Parameters["@RefreshToken_Id"].Value) > 0);
            });

               
        }

        public RefreshToken FindRefreshToken(Int64 fldUser_ID)
        {
            var cmd = new SqlCommand
            {
                CommandText = "select * from Main.tblRefreshTokens where fldUser_ID =  " + fldUser_ID,
                CommandType = CommandType.Text
            };

            var dt = DataConnect.DataConnect.FillDt(ref cmd);
            var user = new RefreshToken();

            if (dt.Rows.Count <= 0) return null;
            user.fldUserId = Convert.ToInt64(dt.Rows[0]["fldUser_Id"]);
            user.fldProtectedTicket = Convert.ToString(dt.Rows[0]["fldProtectedTicket"]);
            user.fldExpiresUtc = Convert.ToDateTime(dt.Rows[0]["fldExpiresUtc"]);
            user.fldIssuedUtc = Convert.ToDateTime(dt.Rows[0]["fldIssuedUtc"]);
            user.fldSubject = Convert.ToString(dt.Rows[0]["fldUserName"]);
            user.fldWebClientId = Convert.ToString(dt.Rows[0]["fldWebClient_Id"]);
            user.fldRemoteIP = Convert.ToString(dt.Rows[0]["fldRemoteIP"]);
            return user;
        }

        public List<RefreshToken> GetRefreshToken ()
        {
            var cmd = new SqlCommand
            {
                CommandText = "select * from Main.tblRefreshTokens",
                CommandType = CommandType.Text
            };

            var dt = DataConnect.DataConnect.FillDt(ref cmd);
           

            if (dt.Rows.Count <= 0) return null;

            return (from DataRow dr in dt.Rows
                select new RefreshToken
                {
                    fldUserId = Convert.ToInt64(dr["fldUser_Id"]), fldProtectedTicket = Convert.ToString(dr["fldProtectedTicket"]),
                    fldExpiresUtc = Convert.ToDateTime(dr["fldExpiresUtc"]), fldIssuedUtc = Convert.ToDateTime(dr["fldIssuedUtc"]),
                    fldSubject = Convert.ToString(dr["fldUserName"]), fldWebClientId = Convert.ToString(dr["fldWebClient_Id"]),
                    fldRemoteIP = Convert.ToString(dr["fldRemoteIP"])
                }).ToList();
        }

        public Task<bool>    RemoveRefreshToken(Int64 fldUser_ID)
        {
            try
            {
                var cmd = new SqlCommand
                {
                    CommandText = "Delete from Main.tblRefreshTokens where fldUser_ID =  " + fldUser_ID,
                    CommandType = CommandType.Text
                };

                return Task.Factory.StartNew(() =>
                {
                    DataConnect.DataConnect.ExecCmm(ref cmd);
                    return true;
                });

              
            }
            catch (Exception)
            {
                //return false;
                return null;
            }
           
        }

        public MenuFinal GetMenus()
        {
            var menus = GetStates();
            //List<MenuData> mnu = new List<MenuData>();
            MenuFinal mnu = new MenuFinal();
            mnu.Menudata = new List<MenuData>();
            mnu.Menudata.AddRange(menus
                          .Where(c => c.fldParentMenu_ID == -1)
                         .Select(c => new MenuData()
                         {
                             fldParentMenu_ID = c.fldParentMenu_ID,
                             icon = c.fldIcon,
                            
                             priority = c.fldPriority,
                           
                             state = c.fldstateName,
                            
                             name = c.fldMenuName,
                             type = c.fldMenuType, 
                             children = GetChildrenMenu(menus, c.fldWebMenu_Id)

                         })
                          .ToList());

            HieararchyWalk(menus);

            return mnu;
        }

        private static List<MenuData> GetChildrenMenu(List<Menu> comments, Int64 parentId)
        {
            return comments
                    .Where(c => c.fldParentMenu_ID == parentId)
                    .Select(c => new MenuData()
                    {
                        fldParentMenu_ID = c.fldParentMenu_ID,
                        icon = c.fldIcon,

                        priority = c.fldPriority,

                        state = c.fldstateName,

                        name = c.fldMenuName,
                        type = c.fldMenuType,
                        children = GetChildrenMenu(comments, c.fldWebMenu_Id)

                    })
                    .ToList();
        }

        public static void HieararchyWalkMenu(List<Menu> hierarchy)
        {
            if (hierarchy != null)
            {
                foreach (var item in hierarchy)
                {
                    HieararchyWalk(item.fldChildren);
                }
            }
        }

        public List<Menu> GetStates()
        {
            var cmd = new SqlCommand
            {
                CommandText = "select * from Main.tblWebMenu" ,
                CommandType = CommandType.Text
            };

            var dt = DataConnect.DataConnect.FillDt(ref cmd);
            var menus = new List<Menu>();

            if (dt.Rows.Count <= 0) return null;
            foreach (DataRow drrow in dt.Rows)
            {
                menus.Add(new Menu()
                {
                    fldWebMenu_Id = Convert.ToInt64(drrow["fldWebMenu_Id"]),
                    fldModule_ID = Convert.ToInt64(drrow["fldModule_ID"]),
                    fldParentMenu_ID = Convert.ToInt64(drrow["fldParentMenu_ID"]),
                    fldMenuName = Convert.ToString(drrow["fldMenuName"]),
                    fldstateName = Convert.ToString(drrow["fldstateName"]),
                    fldisabstract = Convert.ToBoolean(drrow["fldisabstract"]),
                    fldtemplateUrl = Convert.ToString(drrow["fldtemplateUrl"]),
                    fldurl = Convert.ToString(drrow["fldurl"]),
                    fldcontrollerName = Convert.ToString(drrow["fldcontrollerName"]),
                    fldcontrollerNameAs = Convert.ToString(drrow["fldcontrollerNameAs"]),
                    fldMenuType = Convert.ToString(drrow["fldMenuType"]),
                    fldIcon = Convert.ToString(drrow["fldIcon"]),
                    fldPriority = Convert.ToDecimal(drrow["fldPriority"]) 
                });
            } 
         
            return menus;
        }

        public List<Menu> ProcessStates()
        {
            List<Menu> _menustruct= new List<Menu>();
            var menus = GetStates();

            _menustruct = menus
                           .Where(c => c.fldParentMenu_ID == -1)
                          .Select(c => new Menu()
                          {
                              fldMenuName = c.fldMenuName,
                              fldModule_ID = c.fldModule_ID,

                              fldParentMenu_ID = c.fldParentMenu_ID,
                              fldMenuType = c.fldMenuType,
                              fldPriority = c.fldPriority,
                              fldWebMenu_Id = c.fldWebMenu_Id,
                              fldstateName = c.fldstateName,
                              fldisabstract = c.fldisabstract,
                              fldurl = c.fldurl,
                              fldcontrollerName = c.fldcontrollerName,

                              fldcontrollerNameAs = c.fldcontrollerNameAs,
                              fldtemplateUrl = c.fldtemplateUrl,

                              fldChildren = GetChildren(menus, c.fldWebMenu_Id) 
                              
                          })
                           .ToList();

            HieararchyWalk(_menustruct);

            return _menustruct;
        }

        private static List<Menu> GetChildren(List<Menu> comments, Int64 parentId)
        {
            return comments
                    .Where(c => c.fldParentMenu_ID == parentId)
                    .Select(c => new Menu()
                    {
                        fldMenuName = c.fldMenuName,
                        fldModule_ID = c.fldModule_ID,

                        fldParentMenu_ID = c.fldParentMenu_ID,
                        fldMenuType = c.fldMenuType,
                        fldPriority = c.fldPriority,
                        fldWebMenu_Id = c.fldWebMenu_Id,
                        fldstateName = c.fldstateName,
                        fldisabstract = c.fldisabstract,
                        fldurl = c.fldurl,
                        fldcontrollerName = c.fldcontrollerName,

                        fldcontrollerNameAs = c.fldcontrollerNameAs,
                        fldtemplateUrl = c.fldtemplateUrl,

                        fldChildren = GetChildren(comments, c.fldWebMenu_Id)

                    })
                    .ToList();
        }

        public static void HieararchyWalk(List<Menu> hierarchy)
        {
            if (hierarchy != null)
            {
                foreach (var item in hierarchy)
                { 
                    HieararchyWalk(item.fldChildren);
                }
            }
        }

        public Task<bool> AddExceptionLogs(ExceptionLog logs)
        {
            var cmd = new SqlCommand
            {
                CommandText = "Main.spWebExceptionLogsAdd",
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@User_ID", logs.user_ID);
            cmd.Parameters.AddWithValue("@WebClient_Id", logs.fldWebClient_Id);
            cmd.Parameters.AddWithValue("@LoggedOn", Convert.ToDateTime(logs.timestamp) );
            cmd.Parameters.AddWithValue("@Exception", logs.Ex);
            cmd.Parameters.AddWithValue("@status", logs.status);
            cmd.Parameters.AddWithValue("@statusText", logs.statusText);
            cmd.Parameters.AddWithValue("@stack", logs.stack);
            cmd.Parameters.AddWithValue("@stackDetail", logs.stackArg);
            cmd.Parameters.AddWithValue("@StateName", logs.menu);
            return Task.Factory.StartNew(() =>
            {
                DataConnect.DataConnect.ExecCmm(ref cmd);
                return true;
            }); 
        }

        public Task<bool> AddFeebBack(Feedbacks logs)
        {
            var cmd = new SqlCommand
            {
                CommandText = "Main.spWebClientFeedbackAdd",
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@User_ID", logs.user_ID);
            cmd.Parameters.AddWithValue("@WebClient_Id", logs.webClientID);
            cmd.Parameters.AddWithValue("@LoggedOn", Convert.ToDateTime(logs.updatedOn));
            cmd.Parameters.AddWithValue("@Menu", logs.menu);
            cmd.Parameters.AddWithValue("@Comments", logs.description); 
            return Task.Factory.StartNew(() =>
            {
                DataConnect.DataConnect.ExecCmm(ref cmd);
                return true;
            }); 
        }

        public void Dispose()
        {
           
        }
    }
}