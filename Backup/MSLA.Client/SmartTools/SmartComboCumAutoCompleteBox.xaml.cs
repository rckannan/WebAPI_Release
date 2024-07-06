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

namespace MSLA.Client.SmartTools
{
    public partial class SmartComboCumAutoCompleteBox : UserControl
    {
        public event EventHandler SelectedItemChanged;
        public event EventHandler ExplicitInitCompleted;
        public event EventHandler SelectedTextRefreshed;


        public SmartComboCumAutoCompleteBox()
        {
            InitializeComponent();
            AutoCombo.IsManualPopup = true;
            AutoCombo.MinimumPrefixLength = 0;

            AutoCombo.IsManualPopup = true;
        }

        public void ResetItemSource()
        {
            AutoCombo.ResetItemSource();
        }

        public Dictionary<Int64, String> SourceData
        {
            get { return AutoCombo.SourceData; }
        }

        public void ExplicitInit()
        {
            AutoCombo.ExplicitInit();
        }

        public String CollectionMember
        {
            get { return AutoCombo.CollectionMember; }
            set { AutoCombo.CollectionMember = value; }
        }

        public String DisplayMember
        {
            get { return AutoCombo.DisplayMember; }
            set { AutoCombo.DisplayMember= value; }
        }

        public String PrefetchFilter
        {
            get { return AutoCombo.PrefetchFilter; }
            set { AutoCombo.PrefetchFilter = value; }
        }

        public String ValueMember
        {
            get { return AutoCombo.ValueMember; }
            set { AutoCombo.ValueMember = value; }
        }

        public MSLAService.DBConnectionType cnType
        {
            get { return AutoCombo.cnType; }
            set { AutoCombo.cnType = value; }
        }

        public Int64 SelectedValue
        {
            get { return AutoCombo.SelectedValue; }
            set { AutoCombo.SelectedValue = value; }
        }

        public String SelectedText
        {
            get { return AutoCombo.SelectedText; }
            //set { AutoCombo.CollectionMember = value; }
        }
        
        private void GetOptions_Click(object sender, RoutedEventArgs e)
        {
            AutoCombo.ShowOptions();
            AutoCombo.Focus();
        }

        private void AutoCombo_SelectedItemChanged(object sender, EventArgs e)
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged.Invoke(this, e);
            }
        }

        private void AutoCombo_ExplicitInitCompleted(object sender, EventArgs e)
        {
            if (ExplicitInitCompleted != null)
            {
                ExplicitInitCompleted.Invoke(this, e);
            }
        }

        private void AutoCombo_SelectedTextRefreshed(object sender, EventArgs e)
        {
            if (SelectedTextRefreshed != null)
            {
                SelectedTextRefreshed.Invoke(this, e);
            }
        }
    }
}
