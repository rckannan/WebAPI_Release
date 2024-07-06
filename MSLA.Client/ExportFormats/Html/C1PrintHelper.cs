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
using System.Windows.Browser;

namespace MSLA.Client
{
    public static class PrintHelper
    {
        public static void Print(string html)
        {
            Print("Preview", html, false);
        }

        public static void Print(string previewWinTitle, string html, bool preview)
        {
            Print(previewWinTitle, html, preview, new HtmlPopupWindowOptions() { Width = 800, Height = 600, Status = false, Directories = false, Location = false, Menubar = false, Toolbar = false });
        }

        public static void Print(string previewWinTitle, string html, bool preview, HtmlPopupWindowOptions options)
        {
            if (Application.Current.Host.Source.Scheme == "file")
                throw new InvalidOperationException("In order to Print the application should be executed from a web application");

            if (!Application.Current.Host.Settings.EnableHTMLAccess)
                throw new InvalidOperationException("In order to Print the Silverlight plugin control should have HTMLAccess='true'");

            // clean html
            string cleanedHtml = html.Replace("\"", "'").Replace("\r", "").Replace("\n", "");

            // write javascript code
            string code = string.Format(@"var tmpWin = window.open('', '{0}', 'width={1},height={2},scrollbars={3},resizable={4},location={5},toolbar={6},top={7},left={8},menubar={9},directories={10},status={11}');",
                                                previewWinTitle,
                                                options.Width,
                                                options.Height,
                                                options.Scrollbars ? 1 : 0,
                                                options.Resizeable ? 1 : 0,
                                                options.Location ? 1 : 0,
                                                options.Toolbar ? 1 : 0,
                                                options.Top,
                                                options.Left,
                                                options.Menubar ? 1 : 0,
                                                options.Directories ? 1 : 0,
                                                options.Status ? 1 : 0);
            code += @"if(tmpWin != null){
                        tmpWin.blur();";
            code += string.Format(@"tmpWin.document.write(""{0}"");tmpWin.document.close();", cleanedHtml);
            if (preview)
            {
                code += @"tmpWin.print();";
            }
            code += "tmpWin.close();}";

            // execut javascript code
            var result = HtmlPage.Window.Eval(code);
        }


    }
}
