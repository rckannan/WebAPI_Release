using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


namespace MSLA.Server.Utilities
{  /// <summary>The XML Reader Class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class XMLReader
        : IDisposable
    {
        private String _RootNode = String.Empty;
        private XmlDocument _Doc;

        /// <summary>Constructor</summary>
        /// <param name="RootNode">The Root Node</param>
        /// <param name="XMLValue">The XML Value</param>
        public XMLReader(String RootNode, String XMLValue)
        {
            this._RootNode = RootNode;
            InitialiseMe(XMLValue);
        }

        public XMLReader(XmlDocument XMLFile)
        {
            _Doc = XMLFile;
        }

        private void InitialiseMe(String XMLValue)
        {
            MemoryStream MStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(XMLValue));
            _Doc = new XmlDocument();
            _Doc.Load(MStream);
        }

        /// <summary>Gets the Message Type</summary>
        public String GetMessageType()
        {
            String Value = String.Empty;
            using (XmlNodeReader reader = new XmlNodeReader(_Doc))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "MessageType")
                        {
                            Value = reader.ReadInnerXml();
                        }
                    }
                }
                reader.Close();
            }
            return Value;
        }

        /// <summary>Gets the Value of the Node</summary>
        /// <param name="NodeName">The Name of the Node</param>
        public String GetValue(String NodeName)
        {
            String Value = String.Empty;
            using (XmlNodeReader reader = new XmlNodeReader(_Doc))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == NodeName)
                        {
                            Value = reader.ReadInnerXml();
                        }
                    }
                }
                reader.Close();
            }
            return Value;
        }

        public object GetValue(string ParentNode, string NodeName)
        {
            string Value = string.Empty;

            XmlNodeList nodes = default(XmlNodeList);
            XmlNode oFoundNode = default(XmlNode);
            nodes = _Doc.GetElementsByTagName(ParentNode);

            oFoundNode = FindNode(NodeName, nodes);
            if ((nodes != null))
            {
                Value = oFoundNode.InnerXml;
            }
            return Value;
        }

        private XmlNode FindNode(string Name, System.Xml.XmlNodeList oParentNodes)
        {
            XmlNode foundnode = null;
            foreach (XmlNode oChild in oParentNodes)
            {
                if (oChild.Name == Name)
                {
                    return oChild;
                }
                else
                {
                    foundnode = FindNode(Name, oChild.ChildNodes);
                }
                if ((foundnode == null) == false)
                    break; // TODO: might not be correct. Was : Exit For
            }
            return foundnode;
        }


        #region " IDisposable Support "
        private Boolean disposedValue = false;
        /// <summary>Dispose method</summary>
        /// <param name="disposing">True for Dispose</param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: free unmanaged resources when explicitly called;
                }
                _Doc = null;
            }
            this.disposedValue = true;
        }

        /// <summary>The Dispose Method</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}