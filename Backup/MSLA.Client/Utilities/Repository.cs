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
using MSLA.Client.MSLAService;
using System.Collections.Generic;
using System.Collections;

namespace MSLA.Client
{
    public class Repository
    {
        private static Repository _repository;
        private Repository() { }
        private Boolean _allowCache = true;

        private List<CachedItem> _cachedItems = new List<CachedItem>();

        private static void createInstance()
        {
            _repository = new Repository();
        }

        public static Repository getInstance()
        {
            if (_repository == null)
            {
                createInstance();
            }

            return _repository;
        }

        public void GetResultSet(String collectionMember, String Filter, DBConnectionType cnType,
                 MSLAService.SimpleUserInfo UserInfo, String valMember, String dispMember, String qText,
                 MSLAService.MSLAServiceClient.GetRsltSetCompletedHandler GetRsltCompletedAddress)
        {
            if (_cachedItems == null)
            {
                _cachedItems = new List<CachedItem>();
            }

            if (_allowCache)
            {
                CachedItem newItem = new CachedItem();
                newItem.cnType = cnType;
                newItem.collectionMember = collectionMember;
                newItem.displayMember = dispMember;
                newItem.Filter = Filter;
                newItem.valueMember = valMember;
                CachedItem myItem = hasItem(newItem);
                if (myItem != null && myItem.resultSet.Count != 0)
                {
                    MSLAService.MSLAServiceClient.GetRsltSetCompletedEventArgs args = new MSLAService.MSLAServiceClient.GetRsltSetCompletedEventArgs(myItem.resultSet);
                    GetRsltCompletedAddress.Invoke(this, args);
                }
                else
                {
                    GetResults(collectionMember, Filter, cnType, UserInfo, valMember, dispMember, qText, GetRsltCompletedAddress, newItem);
                }
            }
            else
            {
                GetResults(collectionMember, Filter, cnType, UserInfo, valMember, dispMember, qText, GetRsltCompletedAddress);
            }
        }

        private void GetResults(String collectionMember, String Filter, DBConnectionType cnType,
                 MSLAService.SimpleUserInfo UserInfo, String valMember, String dispMember, String qText,
                 MSLAService.MSLAServiceClient.GetRsltSetCompletedHandler GetRsltCompletedAddress, CachedItem newItem)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetRsltSetCompleted += GetRsltCompletedAddress;
            wsClient.GetResultSetCompleted += new EventHandler<MSLAService.GetResultSetCompletedEventArgs>(wsClient_GetResultSetCompleted);
            newItem.CacheID = wsClient.Request_ID;
            _cachedItems.Add(newItem);
            //wsClient.GetResultSetAsync(collectionMember, Filter, cnType, valMember, dispMember, qText, UserInfo, wsClient.Request_ID);
            wsClient.GetResultSetAsync(collectionMember, Filter, cnType, valMember, dispMember, string.Empty, UserInfo, wsClient.Request_ID);
        }

        private void GetResults(String collectionMember, String Filter, DBConnectionType cnType,
                 MSLAService.SimpleUserInfo UserInfo, String valMember, String dispMember, String qText,
                 MSLAService.MSLAServiceClient.GetRsltSetCompletedHandler GetRsltCompletedAddress)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAService.MSLAServiceClient();
            wsClient.GetRsltSetCompleted += GetRsltCompletedAddress;
            wsClient.GetResultSetCompleted += new EventHandler<MSLAService.GetResultSetCompletedEventArgs>(wsClient_GetResultSetCompleted);
            //wsClient.GetResultSetAsync(collectionMember, Filter, cnType, valMember, dispMember, qText, UserInfo, wsClient.Request_ID);
            wsClient.GetResultSetAsync(collectionMember, Filter, cnType, valMember, dispMember, string.Empty, UserInfo, wsClient.Request_ID);
        }

        private void wsClient_GetResultSetCompleted(object sender, MSLAService.GetResultSetCompletedEventArgs e)
        {
            if (e.Error != null || e.Result == null)
            {
                Exceptions.ExceptionHandler.HandleException((sender as MSLAService.MSLAServiceClient).Request_ID);
            }
            else
            {
                Dictionary<Int64, String> result = new Dictionary<long, string>();
                result = e.Result;
                foreach (CachedItem cItem in _cachedItems)
                {
                    if (cItem.CacheID == (sender as MSLAService.MSLAServiceClient).Request_ID)
                    {
                        cItem.resultSet = result;
                        break;
                    }
                }
                
                MSLAService.MSLAServiceClient.GetRsltSetCompletedEventArgs args = new MSLAService.MSLAServiceClient.GetRsltSetCompletedEventArgs(result);
                (sender as MSLAService.MSLAServiceClient).onGetResultSetCompleted(args);
            }
        }

        private CachedItem hasItem(CachedItem newItem)
        {
            foreach (CachedItem cItem in _cachedItems)
            {
                if (newItem.collectionMember == cItem.collectionMember
                                   && newItem.Filter == cItem.Filter
                                   && newItem.cnType == cItem.cnType
                                   && newItem.valueMember == cItem.valueMember
                                   && newItem.displayMember == cItem.displayMember)
                { return cItem; }
            }
            return null;
        }

        public void ForceRefresh()
        {
            _cachedItems.Clear();
        }

        public void ForceRefresh(String collectionMember)
        {
            foreach (CachedItem cItem in _cachedItems)
            {
                if (cItem.collectionMember == collectionMember)
                {
                    _cachedItems.Remove(cItem);
                    break;
                }
            }
        }

    }


    public class CachedItem
        //: IComparable
    {
        public String collectionMember = string.Empty;
        public String Filter = string.Empty;
        public DBConnectionType cnType = DBConnectionType.CompanyDB;
        public String valueMember = string.Empty;
        public String displayMember = string.Empty;
        public Guid CacheID;
        public Dictionary<Int64, String> resultSet = new Dictionary<long, string>();

        //public int CompareTo(object obj)
        //{
        //    //if (obj is CachedItem)
        //    //{
        //    //    CachedItem cItem = obj as CachedItem;
        //    //    if (this.collectionMember == cItem.collectionMember
        //    //        && this.Filter == cItem.Filter
        //    //        && this.cnType == cItem.cnType
        //    //        && this.valueMember == cItem.valueMember
        //    //        && this.displayMember == cItem.displayMember)
        //    //    { return 1; }
        //    //}
        //    return 0;
        //}


    }

}
