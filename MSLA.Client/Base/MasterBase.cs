using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;

namespace MSLA.Client.MSLAService
{
    #region SimpleBOMaster

    [System.Runtime.Serialization.KnownTypeAttribute(typeof(System.DBNull))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(MSLA.Client.MSLAService.SimpleTable))]
    public partial class SimpleBOMaster
    {
        #region Property Bag implementation

        private PropValue _propValueHolder = null;
        public PropValue PropertyValue
        {
            get
            {
                if (_propValueHolder == null)
                { _propValueHolder = new PropValue(this); }
                return _propValueHolder;
            }
        }

        public class PropValue
        {
            private readonly MSLAService.SimpleBOMaster Parent;

            public PropValue(MSLAService.SimpleBOMaster parent)
            {
                this.Parent = parent;
            }

            public object this[string item]
            {
                get
                {
                    object a;
                    this.Parent.PropertyBag.TryGetValue(item, out a);
                    return a;
                }
                set
                {
                    this.Parent.PropertyBag.Remove(item);
                    this.Parent.PropertyBag.Add(item, value);
                }
            }
        }

        #endregion

        #region Fetch implementation

        public static void FetchBOMaster(MasterCriteriaBase myMasterCriteria, MSLAService.MSLAServiceClient.FetchBOCompletedHandler FetchBOCompletedAddress)
        {
                MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
                wsClient.FetchBOCompleted += FetchBOCompletedAddress;
                wsClient.FetchBOMasterCompleted += new EventHandler<MSLAService.FetchBOMasterCompletedEventArgs>(wsClient_FetchBOMasterCompleted);
                wsClient.FetchBOMasterAsync(myMasterCriteria, wsClient.Request_ID);           
        }

        static void wsClient_FetchBOMasterCompleted(object sender, MSLAService.FetchBOMasterCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                SimpleBOMaster resultBO = e.Result;
                resultBO = SwapPropertyBag(e.Result);
                MSLAService.MSLAServiceClient.FetchBOCompletedEventArgs args = new MSLAService.MSLAServiceClient.FetchBOCompletedEventArgs(resultBO);
                (sender as MSLAService.MSLAServiceClient).onFetchBOCompleted(args);
            }
        }

        #endregion

        #region Save implementation

        public static void SaveBOMaster(SimpleBOMaster myBO, MSLAService.MSLAServiceClient.SaveBOCompletedHandler SaveBOCompledAddress)
        {
            myBO = SwapPropertyBag(myBO);
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.SaveBOCompleted += SaveBOCompledAddress;
            wsClient.SaveBOMasterCompleted += new EventHandler<MSLAService.SaveBOMasterCompletedEventArgs>(wsClient_SaveBOMasterCompleted);
            wsClient.SaveBOMasterAsync(myBO, wsClient.Request_ID);
        }

        static void wsClient_SaveBOMasterCompleted(object sender, MSLAService.SaveBOMasterCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                SimpleBOMaster resultBO = e.Result;

                resultBO = SwapPropertyBag(e.Result);

                MSLAService.MSLAServiceClient.SaveBOCompletedEventArgs args = new MSLAService.MSLAServiceClient.SaveBOCompletedEventArgs(resultBO);
                (sender as MSLAService.MSLAServiceClient).onSaveBOCompleted(args);
            }
        }

        #endregion

        #region Delete implementation

        public static void DeleteBOMaster(SimpleBOMaster myBO, MSLAService.MSLAServiceClient.DeleteBOCompletedHandler DeleteBOCompledAddress)
        {
            myBO = SwapPropertyBag(myBO);
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.DeleteBOCompleted += DeleteBOCompledAddress;
            wsClient.DeleteBOMasterCompleted += new EventHandler<MSLAService.DeleteBOMasterCompletedEventArgs>(wsClient_DeleteBOMasterCompleted);
            wsClient.DeleteBOMasterAsync(myBO, wsClient.Request_ID);
        }

        static void wsClient_DeleteBOMasterCompleted(object sender, MSLAService.DeleteBOMasterCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                SimpleBOMaster resultBO = e.Result;

                resultBO = SwapPropertyBag(e.Result);

                MSLAService.MSLAServiceClient.DeleteBOCompletedEventArgs args = new MSLAService.MSLAServiceClient.DeleteBOCompletedEventArgs(resultBO);
                (sender as MSLAService.MSLAServiceClient).onDeleteBOCompleted(args);
            }
        }

        #endregion

        #region Helper Methods

        private static SimpleBOMaster SwapPropertyBag(SimpleBOMaster myBO)
        {
            Dictionary<string, object> propBag = new Dictionary<string, object>();
            foreach (KeyValuePair<String, object> keyval in myBO.PropertyBag)
            {
                if (keyval.Value is Data.DataTable)
                {
                    propBag.Add(keyval.Key, Data.DataConnect.ResolveToSimpleTable(keyval.Value as Data.DataTable));
                }
                else if (keyval.Value is SimpleTable)
                {
                    propBag.Add(keyval.Key, Data.DataConnect.GetResolvedTable(keyval.Value as SimpleTable));
                }
                else
                {
                    propBag.Add(keyval.Key, keyval.Value);
                }
            }
            myBO.PropertyBag = propBag;
            return myBO;
        }

        #endregion

    }

    #endregion

    #region MasterCriteria Class

    public partial class MasterCriteriaBase
        {
            public MasterCriteriaBase(long Master_ID, string MasterType, MSLAService.SimpleUserInfo UserInfo , System.Collections.Generic.Dictionary<string, object> PropCollection)
            {
                this.DocMaster_ID = Master_ID;
                this.DocMasterType = MasterType;
                this.UserInfo = UserInfo;
                //this.User_ID = UserID;
                //this.Session_ID = UserSession_ID;
                this.PropertyCollection = PropCollection;
            }
        }

    #endregion

    #region SimpleTable

        [System.Runtime.Serialization.KnownTypeAttribute(typeof(System.DBNull))]
        public partial class SimpleTable
        {

        }

    #endregion

}
