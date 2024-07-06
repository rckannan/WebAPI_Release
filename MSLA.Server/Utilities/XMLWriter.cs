using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Utilities
{
    /// <summary>The XML Writer Class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class XMLWriter
    {
        private String _RootNode = String.Empty;
        private StringBuilder _XML;
        private String _MsgTerminator;
        private System.Xml.XmlTextWriter _Writer;
        private System.IO.MemoryStream _MStream;
        private String _XmlTextWriter;

        /// <summary>Set whether the Messge Terminator is required</summary>
        public Boolean SetMessageTerminator = false;

        /// <summary>Constructor</summary>
        /// <param name="RootNode">The root node</param>
        public XMLWriter(String RootNode)
            : this(RootNode, String.Empty, String.Empty)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="RootNode">The Root Node</param>
        /// <param name="MsgType">The Message to be encoded</param>
        public XMLWriter(String RootNode, String MsgType)
            : this(RootNode, MsgType, String.Empty)
        {
        }

        /// <summary>Constructor</summary>
        /// <param name="RootNode">The Root Node</param>
        /// <param name="MsgType">The Message to be encoded</param>
        /// <param name="MsgTermin">The Message Terminator</param>
        public XMLWriter(String RootNode, String MsgType, String MsgTermin)
        {
            _RootNode = RootNode;
            _MsgTerminator = MsgTermin;
            SetMessageTerminator = true;
            InitialiseMe(MsgType);
        }



        private void InitialiseMe(String MsgType)
        {
            _XML = new StringBuilder();
            InsertElementForXML();
            InsertElement(_RootNode);
            if (MsgType != string.Empty)
            {
                SetValue("MessageType", MsgType);
            }
        }

        private void InsertElement(String Node)
        {
            _XML.Append("<");
            _XML.Append(Node);
            _XML.Append(">");
        }

        private void InsertEndElement(String Node)
        {
            _XML.Append("</");
            _XML.Append(Node);
            _XML.Append(">");
        }


        private void InsertValue(String Value)
        {
            _XML.Append(Value);
        }

        /// <summary>Sets a value into the node</summary>
        /// <param name="Node">The Node</param>
        /// <param name="Value">The Value</param>
        public void SetValue(String Node, String Value)
        {
            InsertElement(Node);
            InsertValue(Value);
            InsertEndElement(Node);
        }
        //********************************************For XML TextWriter**************************************************
        private void InsertElementForXML()
        {
            _MStream = new System.IO.MemoryStream();
            _Writer = new System.Xml.XmlTextWriter(_MStream, null);
            _Writer.WriteStartElement(_RootNode);
            _Writer.Formatting = System.Xml.Formatting.Indented;

        }

        /// <summary>Sets a value into the node which uses XMLTextWriter</summary>
        /// <param name="Node">The Node</param>
        /// <param name="Value">The Value</param>
        /// 

        public void SetValueForXML(String Node, String Value)
        {
            _Writer.WriteElementString(Node, Value);
        }

        /// <summary>Gets the Constructed XML String using XmlTextWriter</summary>
        public String GetXMLForExtendedColumns
        {
            get
            {
                _Writer.WriteEndElement();
                _Writer.Flush();
                _MStream.Position = 0;
                System.IO.StreamReader sr = new System.IO.StreamReader(_MStream);
                _XmlTextWriter = sr.ReadToEnd();

                if (SetMessageTerminator)
                {
                    return _XmlTextWriter + _MsgTerminator;
                }
                else
                {
                    return _XmlTextWriter;
                }
            }
        }

        //********************************************End For XML TextWriter*****************************************


        /// <summary>Gets the Constructed XML String</summary>
        public String GetXML
        {
            get
            {
                if (SetMessageTerminator)
                {
                    return _XML.ToString() + "</" + _RootNode + ">" + _MsgTerminator;
                }
                else
                {
                    return _XML.ToString() + "</" + _RootNode + ">";
                }
            }
        }

        /// <summary>Gets the XML String in Bytes</summary>
        public Byte[] GetBytes
        {
            get
            {
                if (SetMessageTerminator)
                {
                    return Encoding.ASCII.GetBytes(_XML.ToString() + "</" + _RootNode + ">" + MSLA.Server.Constants.MessageTerminator);
                }
                else
                {
                    return Encoding.ASCII.GetBytes(_XML.ToString() + "</" + _RootNode + ">");
                }
            }

        }

    }
}
