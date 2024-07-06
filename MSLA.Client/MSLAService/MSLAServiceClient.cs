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
using System.Collections.Generic;

namespace MSLA.Client.MSLAService
{
    public partial class MSLAServiceClient
    {
        private Guid _Request_ID = Guid.NewGuid();

        public Guid Request_ID
        {
            get { return _Request_ID; }
        }

        #region DataFetch

        public event DataFetchCompletedHandler DataFetchCompleted;

        public delegate void DataFetchCompletedHandler(object sender, DataFetchCompletedEventArgs e);

        public void onDataFetchCompleted(DataFetchCompletedEventArgs e)
        {
            if (DataFetchCompleted != null)
            {
                DataFetchCompleted(this, e);
            }
        }

        public class DataFetchCompletedEventArgs
            : EventArgs
        {
            private Data.DataTable _dtResult;
            public readonly DateTime CompletedTime;
            public DataFetchCompletedEventArgs(Data.DataTable dt)
            {
                CompletedTime = DateTime.Now;
                _dtResult = dt;
            }

            internal DataFetchCompletedEventArgs()
            {
                CompletedTime = DateTime.Now;
            }

            internal void setdtResult(Data.DataTable dt)
            {
                _dtResult = dt;
            }

            public Data.DataTable dtResult
            { get { return _dtResult; } }

        }

        #endregion

        #region ExecCMM

        public event ExecCMMCompletedHandler ExecCMMCompleted;
        public delegate void ExecCMMCompletedHandler(object sender, ExecCMMCompletedEventArgs e);

        public void onExecCMMCompleted(ExecCMMCompletedEventArgs e)
        {
            if (ExecCMMCompleted != null)
            {
                ExecCMMCompleted(this, e);
            }
        }

        public class ExecCMMCompletedEventArgs
            : EventArgs
        {
            public readonly System.Collections.Generic.List<MSLA.Client.MSLAService.DataParameter> ParamCollection;
            public ExecCMMCompletedEventArgs(System.Collections.Generic.List<MSLA.Client.MSLAService.DataParameter> ParamList)
            {
                ParamCollection = ParamList;
            }
        }

        #endregion

        #region FetchBO

        public event FetchBOCompletedHandler FetchBOCompleted;

        public delegate void FetchBOCompletedHandler(object sender, FetchBOCompletedEventArgs e);

        public void onFetchBOCompleted(FetchBOCompletedEventArgs e)
        {
            if (FetchBOCompleted != null)
            {
                FetchBOCompleted(this, e);
            }
        }

        public class FetchBOCompletedEventArgs
            : EventArgs
        {
            public readonly SimpleBOMaster ResultBO;
            public FetchBOCompletedEventArgs(SimpleBOMaster myBO)
            {
                ResultBO = myBO;
            }
        }

        #endregion

        #region SaveBO

        public event SaveBOCompletedHandler SaveBOCompleted;

        public delegate void SaveBOCompletedHandler(object sender, SaveBOCompletedEventArgs e);

        public void onSaveBOCompleted(SaveBOCompletedEventArgs e)
        {
            if (SaveBOCompleted != null)
            {
                SaveBOCompleted(this, e);
            }
        }

        public class SaveBOCompletedEventArgs
            : EventArgs
        {
            public readonly SimpleBOMaster ResultBO;
            public SaveBOCompletedEventArgs(SimpleBOMaster myBO)
            {
                ResultBO = myBO;
            }
        }

        #endregion

        #region DeleteBO

        public event DeleteBOCompletedHandler DeleteBOCompleted;

        public delegate void DeleteBOCompletedHandler(object sender, DeleteBOCompletedEventArgs e);

        public void onDeleteBOCompleted(DeleteBOCompletedEventArgs e)
        {
            if (DeleteBOCompleted != null)
            {
                DeleteBOCompleted(this, e);
            }
        }

        public class DeleteBOCompletedEventArgs
            : EventArgs
        {
            public readonly SimpleBOMaster ResultBO;
            public DeleteBOCompletedEventArgs(SimpleBOMaster myBO)
            {
                ResultBO = myBO;
            }
        }

        #endregion

        #region ResolveTablesFromFile

        public event ResolveFileCompletedHandler ResolveFileCompleted;
        public delegate void ResolveFileCompletedHandler(object sender, ResolveFileCompletedEventArgs e);

        public void onResolveFileCompleted(ResolveFileCompletedEventArgs e)
        {
            if (ResolveFileCompleted != null)
            {
                ResolveFileCompleted(this, e);
            }
        }

        public class ResolveFileCompletedEventArgs
            : EventArgs
        {
            public readonly Dictionary<string, MSLA.Client.Data.DataTable> TableCollection;
            public ResolveFileCompletedEventArgs(Dictionary<string, MSLA.Client.Data.DataTable> Tables)
            {
                TableCollection = Tables;
            }
        }

        #endregion

        #region HandleException

        public event HandleExCompletedHandler HandleExCompleted;

        public delegate void HandleExCompletedHandler(object sender, HandleExCompletedEventArgs e);

        public void onHandleExceptionCompleted(HandleExCompletedEventArgs e)
        {
            if (HandleExCompleted != null)
            {
                HandleExCompleted(this, e);
            }
        }

        public class HandleExCompletedEventArgs
            : EventArgs
        {
            public readonly String ResultError;
            public HandleExCompletedEventArgs(string Error)
            {
                ResultError = Error;
            }
        }

        #endregion

        #region GetResultSet

        public event GetRsltSetCompletedHandler GetRsltSetCompleted;
        public delegate void GetRsltSetCompletedHandler(object sender, GetRsltSetCompletedEventArgs e);

        public void onGetResultSetCompleted(GetRsltSetCompletedEventArgs e)
        {
            if (GetRsltSetCompleted != null)
            {
                GetRsltSetCompleted(this, e);
            }
        }

        public class GetRsltSetCompletedEventArgs
            : EventArgs
        {
            public readonly Dictionary<Int64, String> result;
            public GetRsltSetCompletedEventArgs(Dictionary<Int64, String> results)
            {
                result = results;
            }
        }

        #endregion

        #region GetSelectValue

        public event GetSelectValCompletedHandler GetSelectValCompleted;
        public delegate void GetSelectValCompletedHandler(object sender, GetSelectValCompletedEventArgs e);

        public void onGetSelectValCompleted(GetSelectValCompletedEventArgs e)
        {
            if (GetSelectValCompleted != null)
            {
                GetSelectValCompleted(this, e);
            }
        }

        public class GetSelectValCompletedEventArgs
            : EventArgs
        {
            public readonly Int64 result;
            public GetSelectValCompletedEventArgs(Int64 results)
            {
                result = results;
            }
        }

        #endregion

        #region GetSelectText

        public event GetSelectTxtCompletedHandler GetSelectTxtCompleted;
        public delegate void GetSelectTxtCompletedHandler(object sender, GetSelectTxtCompletedEventArgs e);

        public void onGetSelectTxtCompleted(GetSelectTxtCompletedEventArgs e)
        {
            if (GetSelectTxtCompleted != null)
            {
                GetSelectTxtCompleted(this, e);
            }
        }

        public class GetSelectTxtCompletedEventArgs
            : EventArgs
        {
            public readonly String result;
            public GetSelectTxtCompletedEventArgs(String results)
            {
                result = results;
            }
        }

        #endregion

        #region GetFeed

        public event GetFeedCompletedHandler GetFeedCompleted;
        public delegate void GetFeedCompletedHandler(object sender, GetFeedCompletedEventArgs e);

        public void onGetFeedCompleted(GetFeedCompletedEventArgs e)
        {
            if (GetFeedCompleted != null)
            {
                GetFeedCompleted(this, e);
            }
        }

        public class GetFeedCompletedEventArgs
            : EventArgs
        {
            public readonly string result;

            public GetFeedCompletedEventArgs(string results)
            {
                result = results;
            }
        }

        #endregion

        #region GetFeedItems

        public event GetFeedItemsCompletedHandler GetFeedItemsCompleted;
        public delegate void GetFeedItemsCompletedHandler(object sender, GetFeedItemsCompletedEventArgs e);

        public void onGetFeedItemsCompleted(GetFeedItemsCompletedEventArgs e)
        {
            if (GetFeedItemsCompleted != null)
            {
                GetFeedItemsCompleted(this, e);
            }
        }

        public class GetFeedItemsCompletedEventArgs
            : EventArgs
        {
            public readonly List<MSLAService.FeedItem> result = new List<FeedItem>();

            public GetFeedItemsCompletedEventArgs(List<MSLAService.FeedItem> results)
            {
                result = results;
            }
        }

        #endregion

        #region Notification Subscription Service

        public event NotificationSubCompletedHandler NotificationSubCompleted;
        public delegate void NotificationSubCompletedHandler(object sender, NotificationSubCompletedEventArgs e);

        public void onNotificationSubCompleted(NotificationSubCompletedEventArgs e)
        {
            if (NotificationSubCompleted != null)
            {
                NotificationSubCompleted(this, e);
            }
        }

        public class NotificationSubCompletedEventArgs
            : EventArgs
        {
            public readonly Boolean result;
            public NotificationSubCompletedEventArgs(Boolean results)
            {
                result = results;
            }
        }

        #endregion

        #region Notification Contract Subscription Service

        public event ContractSubCompletedHandler ContractSubCompleted;
        public delegate void ContractSubCompletedHandler(object sender, ContractSubCompletedEventArgs e);

        public void onContractSubCompleted(ContractSubCompletedEventArgs e)
        {
            if (ContractSubCompleted != null)
            {
                ContractSubCompleted(this, e);
            }
        }

        public class ContractSubCompletedEventArgs
            : EventArgs
        {
            public readonly Boolean result;
            public ContractSubCompletedEventArgs(Boolean results)
            {
                result = results;
            }
        }

        #endregion

        #region Get Notification Contract Subscription Fields

        public event GetContractCompletedHandler GetContractCompleted;
        public delegate void GetContractCompletedHandler(object sender, GetContractCompletedEventArgs e);

        public void onGetContractCompleted(GetContractCompletedEventArgs e)
        {
            if (GetContractCompleted != null)
            {
                GetContractCompleted(this, e);
            }
        }

        public class GetContractCompletedEventArgs
            : EventArgs
        {
            public readonly Data.DataTable result;
            public GetContractCompletedEventArgs(Data.DataTable results)
            {
                result = results;
            }
        }

        #endregion

        #region ExecScalar

        public event ExecScalarCompletedHandler ExecScalarCompleted;
        public delegate void ExecScalarCompletedHandler(object sender, ExecScalarCompletedEventArgs e);

        public void onExecScalarCompleted(ExecScalarCompletedEventArgs e)
        {
            if (ExecScalarCompleted != null)
            {
                ExecScalarCompleted(this, e);
            }
        }

        public class ExecScalarCompletedEventArgs
            : EventArgs
        {
            public readonly object Result;
            public ExecScalarCompletedEventArgs(object result)
            {
                Result = result;
            }
        }

        #endregion

        #region Get Service Status Information

        public event GetServiceInformationCompletedHandler GetServiceInformationCompleted;
        public delegate void GetServiceInformationCompletedHandler(object sender, GetServiceInformationCompletedEventArgs e);

        public void onGetServiceInformationCompleted(GetServiceInformationCompletedEventArgs e)
        {
            if (GetServiceInformationCompleted != null)
            {
                GetServiceInformationCompleted(this, e);
            }
        }

        public class GetServiceInformationCompletedEventArgs
            : EventArgs
        {
            public readonly Data.DataTable result;
            public GetServiceInformationCompletedEventArgs(Data.DataTable results)
            {
                result = results;
            }
        }

        #endregion

        #region Get Service Log Information

        public event GetWinServiceLogCompletedHandler GetWinServiceLogCompleted;
        public delegate void GetWinServiceLogCompletedHandler(object sender, GetWinServiceLogCompletedEventArgs e);

        public void onGetWinServiceLogCompleted(GetWinServiceLogCompletedEventArgs e)
        {
            if (GetWinServiceLogCompleted != null)
            {
                GetWinServiceLogCompleted(this, e);
            }
        }

        public class GetWinServiceLogCompletedEventArgs
            : EventArgs
        {
            public readonly string result;
            public GetWinServiceLogCompletedEventArgs(string results)
            {
                result = results;
            }
        }

        #endregion

        #region Get Service Log From ID Information

        public event GetWinServiceLogFromIDCompletedHandler GetWinServiceLogFromIDCompleted;
        public delegate void GetWinServiceLogFromIDCompletedHandler(object sender, GetWinServiceLogFromIDCompletedEventArgs e);

        public void onGetWinServiceLogFromIDCompleted(GetWinServiceLogFromIDCompletedEventArgs e)
        {
            if (GetWinServiceLogFromIDCompleted != null)
            {
                GetWinServiceLogFromIDCompleted(this, e);
            }
        }

        public class GetWinServiceLogFromIDCompletedEventArgs
            : EventArgs
        {
            public readonly string result;
            public GetWinServiceLogFromIDCompletedEventArgs(string results)
            {
                result = results;
            }
        }

        #endregion

    }

}
