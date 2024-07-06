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


namespace MSLA.Client.Data
{
    public class DataCommand
    {
        
        private MSLAService.EnDataCommandType _CommandType;
        private string _CommandText;
        private List<MSLAService.DataParameter> _Parameters;
        private int _CommandTimeout = 30;
        private MSLAService.DBConnectionType _ConnectionType;

        /// <summary>Constructor</summary>
        public DataCommand()
        {
            _Parameters = new List<MSLAService.DataParameter>();
        }


        #region "Public Properties"
        /// <summary>The Command Type</summary>
        public MSLAService.EnDataCommandType CommandType
        {
            get { return _CommandType; }
            set { _CommandType = value; }
        }

        /// <summary>Command Text</summary>
        public String CommandText
        {
            get { return _CommandText; }
            set { _CommandText = value; }
        }

        /// <summary>Command Timeout. Default is 30 secs</summary>
        public int CommandTimeout
        {
            get { return _CommandTimeout; }
            set { _CommandTimeout = value; }
        }

        /// <summary>Command Parameter Collection</summary>
        public List<MSLAService.DataParameter> Parameters
        {
            get
            {
                return _Parameters;
            }
        }

        /// <summary>Database Connection Type</summary>
        public MSLAService.DBConnectionType ConnectionType
        {
            get { return _ConnectionType; }
            set { _ConnectionType = value; }
        }

        #endregion

    }
}
