using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Security
{
    public class SimpleUserInfo
    {
        string _MainDBName = string.Empty;
        string _UserName = string.Empty;
        Guid _Session_ID = new Guid();
        long _User_ID = -1;

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public Guid Session_ID
        {
            get { return _Session_ID; }
            set { _Session_ID = value; }
        }

        public long User_ID
        {
            get { return _User_ID; }
            set { _User_ID = value; }
        }

        public string MainDBName
        {
            get { return _MainDBName; }
            set { _MainDBName = value; }
        }
    }
}
