using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSLA.Server.Data;
using System.Collections.ObjectModel;

namespace MSLA.Server.ApiUtilities
{
    public interface ICacheCollectionRepository
    {
        ObservableCollection<CachedItem> GetCacheItem();

        List<MSLA.Server.Tools.AutoCompleteCollection> GetResultSet(String collectionMember, String Filter, DBConnectionType cnType, String valMember, String dispMember, String qText, Guid Request_ID);

         void ForceClear(String collectionMember);

         void ResetAll();

    }

    public class CacheCollectionRepository : ICacheCollectionRepository, IDisposable
    {
        private static CacheCollectionRepository _repository; 
        //private Boolean _allowCache = true;
        private   ObservableCollection<CachedItem> _cachedItems;

        private   CacheCollectionRepository() { _cachedItems = new ObservableCollection<CachedItem>(); }

        private static void createInstance()
        {
            _repository = new CacheCollectionRepository();
        }

        public static CacheCollectionRepository getInstance()
        {
            if (_repository == null) createInstance();  
            return _repository;
        } 

        public void ForceClear(string collectionMember)
        {
            var obj = _cachedItems.Where(x => x.collectionMember == collectionMember).FirstOrDefault();
            if (obj != null) _cachedItems.Remove(obj); 
        }

        public void ResetAll()
        {
            _cachedItems.Clear();
        }

        public void Dispose()
        {
            _cachedItems.Clear();
            _cachedItems = null;
        } 

        /// <summary>
        /// Get the Result set for Drop down. if requested data is available in cache, bring it to client, other wise take from DB again and save the same in cache. 
        /// </summary>
        /// <param name="collectionMember">Table name with schema</param>
        /// <param name="Filter">filter conditions</param>
        /// <param name="cnType">database type to connect</param>
        /// <param name="valMember">Value member</param>
        /// <param name="dispMember">Display member</param>
        /// <param name="qText"></param>
        /// <param name="Request_ID">Session ID</param>
        /// <returns></returns>
        public List<MSLA.Server.Tools.AutoCompleteCollection> GetResultSet(string collectionMember, string Filter, DBConnectionType cnType, string valMember, string dispMember, string qText, Guid Request_ID)
        {
            if (_repository == null) _repository = new CacheCollectionRepository();

            //if requested data is available in cache, bring it to client, other wise take from DB again and save the same in cache.
            CachedItem itm = new CachedItem()
            {
                cnType = cnType,
                collectionMember = collectionMember,
                displayMember = dispMember,
                Filter = Filter,
                valueMember = valMember
            };

            var cacheitm = hasItem(itm);

            if (cacheitm != null)
            {
                return cacheitm.resultSet;
            }
            else
            {
                var cacheObj = MSLA.Server.Tools.AutoCompleteBoxWorker.getResultSetApi(collectionMember, Filter, cnType, valMember, dispMember, qText, Server.Login.LogonService.FetchLogonInfo(Request_ID));
                itm.resultSet = cacheObj;
                _cachedItems.Add(itm);
                return itm.resultSet;
            } 
        }

        private CachedItem hasItem(CachedItem newItem)
        {
            CachedItem res = _cachedItems.Where(x => x.collectionMember == newItem.collectionMember)
                .Where(x => x.displayMember == newItem.displayMember)
                .Where(x => x.cnType == newItem.cnType)
                .Where(x => x.valueMember == newItem.valueMember)
                .Where(x => x.Filter == newItem.Filter).FirstOrDefault();

            return res ?? null;  
        }

        public ObservableCollection<CachedItem> GetCacheItem()
        {
            return _cachedItems;
        }
    }

    public class CachedItem : IDisposable
        
    {
        public String collectionMember = string.Empty;
        public String Filter = string.Empty;
        public DBConnectionType cnType = DBConnectionType.CompanyDB;
        public String valueMember = string.Empty;
        public String displayMember = string.Empty;
        //public Guid CacheID;
        public List<MSLA.Server.Tools.AutoCompleteCollection> resultSet = new List<MSLA.Server.Tools.AutoCompleteCollection>(); 

        public void Dispose()
        {
            resultSet = null;
        }
    }
}
