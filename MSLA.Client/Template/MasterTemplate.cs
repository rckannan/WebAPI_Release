using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace MSLA.Client.Templates
{
    public class MasterTemplate
        : UserControl
    {
        #region Private Variables

        private List<String> _lstSync;
        private DataGrid _dgvBrokenRule;
        private Button _cmdSave;
        private Button _cmdDelete;
        private Label _lblStatus;
        private MSLA.Client.MSLAService.SimpleBOMaster _simpleBO;
        private MSLA.Client.MSLAService.MasterCriteriaBase _BOCriteria;
        private MSLA.Client.MSLAService.SimpleUserInfo _userInfo;
        private String _MasterType;
        private Int64 _Master_ID = -1;
        private Dictionary<string, object> _PropertyCollection;
        private Boolean _ContentReplace = true;



        protected Boolean ContentReplace
        {
            get { return _ContentReplace; }
            set { _ContentReplace = value; }
        }
        #endregion

        #region Public Events

        public event Base.TemplateEventDelegate BeforeSave;
        public event EventHandler AfterSave;
        public event EventHandler AfterDelete;

        #endregion

        #region Protected Constructor

        protected MasterTemplate()
        {
            _userInfo = MSLA.Client.Login.LogonInfo.myUserInfo;
            _lstSync = new List<string>();
        }

        /// <summary>This is the only constructor available.</summary>
        /// <param name="MasterType">This is the master type as mentioned in the DocMaster table</param>
        /// <param name="MasterID">This is the master ID, pass -1 for new master.</param>
        protected MasterTemplate(string MasterType, Int64 MasterID, Dictionary<string, object> PropertyCollection)
            : this()
        {
            _MasterType = MasterType;
            _Master_ID = MasterID;
            _PropertyCollection = PropertyCollection;
        }

        /// <summary>This is the only constructor available.</summary>
        /// <param name="MasterType">This is the master type as mentioned in the DocMaster table</param>
        /// <param name="MasterID">This is the master ID, pass -1 for new master.</param>
        protected MasterTemplate(string MasterType, Int64 MasterID)
            : this()
        {
            _MasterType = MasterType;
            _Master_ID = MasterID;
        }

        #endregion

        #region Protected Properties

        protected MSLA.Client.MSLAService.SimpleBOMaster BOMaster
        {
            get { return _simpleBO; }
        }

        protected Button SaveButton
        {
            set
            {
                _cmdSave = value;
                _cmdSave.Click -= new RoutedEventHandler(_cmdSave_Click);
                _cmdSave.Click += new RoutedEventHandler(_cmdSave_Click);
            }
        }

        protected Button DeleteButton
        {
            set
            {
                _cmdDelete = value;
                if (_cmdDelete != null)
                {
                    _cmdDelete.Click -= new RoutedEventHandler(_cmdDelete_Click);
                    _cmdDelete.Click += new RoutedEventHandler(_cmdDelete_Click);
                }
            }
        }

        protected DataGrid BrokenRuleGrid
        {
            set { _dgvBrokenRule = value; }
        }

        protected Label Status
        {
            set { _lblStatus = value; }
        }

        #endregion

        #region Helper Methods

        private void setBOMaster()
        {
            try
            {
                this.AddToSync("FetchMaster");
                _BOCriteria = new MSLA.Client.MSLAService.MasterCriteriaBase(_Master_ID, _MasterType, _userInfo, _PropertyCollection);
                MSLA.Client.MSLAService.SimpleBOMaster.FetchBOMaster(_BOCriteria, new MSLAService.MSLAServiceClient.FetchBOCompletedHandler(BOMaster_FetchBOMasterCompleted));
                _cmdSave.IsEnabled = false;
                if (_cmdDelete != null)
                {
                    _cmdDelete.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void AddColumns()
        {
            _dgvBrokenRule.Columns.Clear();
            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Header = "Broken Rules";
            textColumn.Binding = new Binding("_RuleDesc");
            textColumn.Width = new DataGridLength(_dgvBrokenRule.Width - 2);
            _dgvBrokenRule.AutoGenerateColumns = false;
            _dgvBrokenRule.Columns.Add(textColumn);
            _dgvBrokenRule.IsReadOnly = true;
        }

        private void BOMaster_FetchBOMasterCompleted(object sender, MSLAService.MSLAServiceClient.FetchBOCompletedEventArgs e)
        {
            _simpleBO = e.ResultBO;
            this.RemoveFromSync("FetchMaster");
            DoForwardBinding();
            _cmdSave.IsEnabled = true;
            if (_cmdDelete != null)
            {
                _cmdDelete.IsEnabled = true;
            }
        }

        void BOMaster_DeleteBOMasterCompleted(object sender, MSLAService.MSLAServiceClient.DeleteBOCompletedEventArgs e)
        {
            _simpleBO = e.ResultBO;
            OnDeleteClick(e);

            if (_simpleBO.BrokenDeleteRules.Count > 0)
            {
                if (_dgvBrokenRule != null)
                {
                    AddColumns();
                    _dgvBrokenRule.ItemsSource = _simpleBO.BrokenDeleteRules;
                }
                if (_lblStatus != null)
                {
                    _lblStatus.Content = "Master Document has broken business rules. Please rectify before saving.";
                }
            }
            else
            {
                if (_dgvBrokenRule != null)
                {
                    _dgvBrokenRule.ItemsSource = null;
                }
                if (_lblStatus != null)
                {
                    _lblStatus.Content = "Successfully Deleted.";
                }
                OnAfterDelete(e);
            }
            DoForwardBinding();

        }

        void BOMaster_SaveBOMasterCompleted(object sender, MSLAService.MSLAServiceClient.SaveBOCompletedEventArgs e)
        {
            _simpleBO = e.ResultBO;
            OnSaveClick(e);

            if (_simpleBO.BrokenSaveRules.Count > 0)
            {
                if (_dgvBrokenRule != null)
                {
                    AddColumns();
                    _dgvBrokenRule.ItemsSource = _simpleBO.BrokenSaveRules;
                }
                if (_lblStatus != null)
                {
                    _lblStatus.Content = "Master Document has broken business rules. Please rectify before saving.";
                }
            }
            else
            {
                if (_dgvBrokenRule != null)
                {
                    _dgvBrokenRule.ItemsSource = null;
                }
                if (_lblStatus != null)
                {
                    _lblStatus.Content = SaveSucceededMessage();
                }
                OnAfterSave(e);
            }

            DoForwardBinding();
        }

        #endregion

        #region Virtual Methods

        protected virtual void InitializeMe()
        {
            setBOMaster();
        }

        protected virtual void DoForwardBinding()
        {

        }

        /// <summary>AfterSave method will be available for successfull Save</summary>
        /// <param name="e">EventArgs</param>
        protected virtual void OnSaveClick(EventArgs e)
        {

        }

        protected virtual void OnAfterSave(EventArgs e)
        {
            if (AfterSave != null)
            {
                AfterSave(this, e);
            }
        }

        protected virtual void OnDeleteClick(EventArgs e)
        {

        }

        protected virtual void OnAfterDelete(EventArgs e)
        {
            if (AfterDelete != null)
            {
                AfterDelete(this, e);
            }
        }

        protected virtual void OnBeforeSave(Base.TemplateEventArgs e)
        {
            if (BeforeSave != null)
            {
                BeforeSave(this, e);
            }
        }

        protected virtual string SaveSucceededMessage()
        {
            return "Successfully saved.";
        }

        #endregion

        #region Button Click Events

        protected void _cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result;
            Result = MessageBox.Show("Are you sure you want to delete this record?", "Delete Confirmation", MessageBoxButton.OKCancel);
            if (Result == MessageBoxResult.OK)
            {
                MSLA.Client.MSLAService.SimpleBOMaster.DeleteBOMaster(_simpleBO, new MSLAService.MSLAServiceClient.DeleteBOCompletedHandler(BOMaster_DeleteBOMasterCompleted));
            }
        }

        protected void _cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Base.TemplateEventArgs BeforeSaveArgs = new Base.TemplateEventArgs();
            this.OnBeforeSave(BeforeSaveArgs);
            if (BeforeSaveArgs.Cancel == false)
            {
                MSLA.Client.MSLAService.SimpleBOMaster.SaveBOMaster(_simpleBO, new MSLAService.MSLAServiceClient.SaveBOCompletedHandler(BOMaster_SaveBOMasterCompleted));
            }
            else
            {
                if (_lblStatus != null)
                {
                    _lblStatus.Content = string.Empty;
                }
            }
        }

        #endregion

        #region SyncList


        public event EventHandler SyncCompleted;

        MSLA.Client.SmartTools.waitEffect anim;
        private UIElement ctrl;

        /// <summary>Add the methodname or a unique name to the list.Remove this name in async completed event.</summary>
        /// <param name="newSync"></param>
        public void AddToSync(String newSync)
        {
            lock (_lstSync)
            {
                if (_lstSync.Count == 0)
                {
                    ctrl = this.Content;
                    anim = new SmartTools.waitEffect();
                    anim.Width = 400;
                    anim.Height = 300;

                    if (!_ContentReplace)
                    {
                        if (ctrl is Grid)
                        {
                            (ctrl as Grid).Children.Add(anim);
                            Canvas.SetZIndex(anim, 999);
                            anim.Visibility = Visibility.Visible;
                            anim.Width = (ctrl as Grid).Width;
                            anim.Height = (ctrl as Grid).Height;
                        }
                        else
                        {
                            this.Content = anim;
                        }
                    }
                    else
                    {
                        this.Content = anim;
                        if (ctrl is Grid)
                        {
                            anim.Width = (ctrl as Grid).Width;
                            anim.Height = (ctrl as Grid).Height;
                        }
                    }
                }
                _lstSync.Add(newSync);
            }
        }

        /// <summary>Removes an already existing methodname. If all method names are removed, it invokes sync completed event.</summary>
        /// <param name="RemoveSync"></param>
        public void RemoveFromSync(String RemoveSync)
        {
            lock (_lstSync)
            {
                if (_lstSync.Contains(RemoveSync))
                {
                    _lstSync.Remove(RemoveSync);
                    if (_lstSync.Count == 0)
                    {
                        if (SyncCompleted != null)
                        {
                            SyncCompleted.Invoke(this, new EventArgs());
                        }
                        if (_ContentReplace)
                        {
                            this.Content = ctrl;
                        }
                        else
                        {
                            if (ctrl is Grid)
                            {
                                (ctrl as Grid).Children.Remove(anim);
                            }
                            else
                            {
                                this.Content = ctrl;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
