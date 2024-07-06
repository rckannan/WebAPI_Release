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
using System.Windows.Controls.Primitives;

namespace MSLA.Client.SmartTools
{
    public class SmartDatePicker : DatePicker
    {
        //private String _DateFormat = string.Empty;

        public String DateFormat
        {
            get { return string.Empty; }
            set { }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DatePickerTextBox dtpbox = base.GetTemplateChild("TextBox") as DatePickerTextBox;

            //dtpbox.Watermark = DateFormat;

            TextBox dtptextBox = this.GetTemplateChild("TextBox") as TextBox;
            //this.Text = DateTime.Today.ToString(DateFormat);

            if (dtptextBox != null)
            {
                dtptextBox.LostFocus += new RoutedEventHandler(textBox_LostFocus);
                dtptextBox.TextChanged += new TextChangedEventHandler(dtptextBox_TextChanged);
            }
        }

        void dtptextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt.Text == string.Empty)
            {
                if (this.SelectedDate == null)
                {
                    this.SelectedDate = DateTime.Today;
                    this.DisplayDate = DateTime.Today;
                }
                else
                {
                    this.SelectedDate = this.SelectedDate.Value.AddMilliseconds(1);
                }
            }
        }

        void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            string str = txt.Text;
            Int32 result = 0;
            if (str.Length > 1)
            {
                if (str.EndsWith("Y"))
                {
                    int index = str.IndexOf("Y");
                    str = str.Substring(0, index);

                    if (Int32.TryParse(str, out result))
                    {
                        this.SelectedDate = this.SelectedDate.Value.AddYears(Convert.ToInt32(str));
                    }
                    this.DisplayDate = (DateTime)this.SelectedDate;
                }
                else if (str.EndsWith("M"))
                {
                    int index = str.IndexOf("M");
                    str = str.Substring(0, index);

                    if (Int32.TryParse(str, out result))
                    {
                        this.SelectedDate = this.SelectedDate.Value.AddMonths(Convert.ToInt32(str));
                    }
                    this.DisplayDate = (DateTime)this.SelectedDate;
                }
                else if (str.EndsWith("y"))
                {
                    int index = str.IndexOf("y");
                    str = str.Substring(0, index);
                    if (Int32.TryParse(str, out result))
                    {
                        this.SelectedDate = this.SelectedDate.Value.AddYears(Convert.ToInt32(str));
                    }
                    this.DisplayDate = (DateTime)this.SelectedDate;
                }
                else if (str.EndsWith("m"))
                {
                    int index = str.IndexOf("m");
                    str = str.Substring(0, index);

                    if (Int32.TryParse(str, out result))
                    {
                        this.SelectedDate = this.SelectedDate.Value.AddMonths(Convert.ToInt32(str));
                    }
                    this.DisplayDate = (DateTime)this.SelectedDate;
                }
                else if (str.EndsWith("W"))
                {
                    int index = str.IndexOf("W");
                    str = str.Substring(0, index);

                    if (Int32.TryParse(str, out result))
                    {
                        this.SelectedDate = this.SelectedDate.Value.AddDays(Convert.ToInt32(str) * 7);
                    }
                    this.DisplayDate = (DateTime)this.SelectedDate;
                }
                else if (str.EndsWith("w"))
                {
                    int index = str.IndexOf("w");
                    str = str.Substring(0, index);

                    if (Int32.TryParse(str, out result))
                    {
                        this.SelectedDate = this.SelectedDate.Value.AddDays(Convert.ToInt32(str) * 7);
                    }
                    this.DisplayDate = (DateTime)this.SelectedDate;
                }
                else if (str.EndsWith("D"))
                {
                    int index = str.IndexOf("D");
                    str = str.Substring(0, index);

                    if (Int32.TryParse(str, out result))
                    {
                        this.SelectedDate = this.SelectedDate.Value.AddDays(Convert.ToInt32(str));
                    }
                    this.DisplayDate = (DateTime)this.SelectedDate;
                }
                else if (str.EndsWith("d"))
                {
                    int index = str.IndexOf("d");
                    str = str.Substring(0, index);

                    if (Int32.TryParse(str, out result))
                    {
                        this.SelectedDate = this.SelectedDate.Value.AddDays(Convert.ToInt32(str));
                    }
                    this.DisplayDate = (DateTime)this.SelectedDate;
                }
                else
                {
                    DateTime d;
                    if (DateTime.TryParse(txt.Text, out d))
                    {
                        this.SelectedDate = Convert.ToDateTime(d);
                    }
                    if (this.SelectedDate != null)
                    {
                        this.DisplayDate = (DateTime)this.SelectedDate;
                    }
                }
                this.SelectedDate = this.DisplayDate;
            }

            if (this.SelectedDate == null)
            {
                this.SelectedDate = DateTime.Today;
                this.DisplayDate = DateTime.Today;
            }

            this.DisplayDate = (DateTime)this.SelectedDate;
            //fetchDate((DateTime)this.DisplayDate);
            if (this.Text == string.Empty)
            {
                this.SelectedDate = this.SelectedDate.Value.AddMilliseconds(1);
            }
        }

        //private void fetchDate(DateTime date)
        //{
        //    if (MSLA.Client.Login.LogonInfo.myUserInfo != null)
        //    {
        //        MSLA.Client.Data.DataCommand cmm = new Data.DataCommand();
        //        cmm.CommandText = "select System.fnHolidayList(@HolidayDate) fldHoliday";
        //        Client.MSLAService.DataParameter  param = new Client.MSLAService.DataParameter();
        //        param._DBType = MSLAService.DataParameter.EnDataParameterType.DateTime;
        //        param._ParameterName = "@HolidayDate";
        //        param._Value = this.SelectedDate.Value.ToString(MSLA.Client.DateFormat.SQLDateFormat);
        //        param._Size = 0;
        //        param._Direction = MSLAService.DataParameter.EnParameterDirection.InputOutput;
        //        cmm.Parameters.Add(param);
        //        cmm.ConnectionType = Client.MSLAService.DBConnectionType.CompanyDB;
        //        cmm.CommandType = Client.MSLAService.EnDataCommandType.Text;

        //        Client.Data.DataConnect.FillDt(cmm, MSLA.Client.Login.LogonInfo.myUserInfo, new Client.MSLAService.MSLAServiceClient.DataFetchCompletedHandler(DataConnect_DataFetchCompleted));
        //    }
        //}
        //void DataConnect_DataFetchCompleted(object sender, MSLA.Client.MSLAService.MSLAServiceClient.DataFetchCompletedEventArgs e)
        //{
        //    MSLA.Client.Data.DataTable dt = e.dtResult;
        //    if (dt.Rows.Count > 0)
        //    {
        //        this.SelectedDate = (DateTime)dt.Rows[0]["fldHoliday"];
        //        this.DisplayDate = (DateTime)this.SelectedDate;
        //    }
        //}
    }
}
