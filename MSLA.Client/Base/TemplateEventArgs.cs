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

namespace MSLA.Client.Base
{      
    public delegate void TemplateEventDelegate(Object Sender, TemplateEventArgs e);

    public class TemplateEventArgs
        :EventArgs
    {
        public Boolean Cancel = false;
    }
}
