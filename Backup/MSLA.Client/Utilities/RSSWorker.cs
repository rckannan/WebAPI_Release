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
using System.Diagnostics;
using System.Xml.Linq;
using System.Windows.Browser;
using System.Windows.Media.Imaging;
using MSLA.Client.MSLAService;

namespace MSLA.Client
{
    public class RSSWorker
    {
        private static String _strFeed = string.Empty;
        private static IEnumerable<FeedDetail> _FeedList = null;

        public static Boolean isReady
        {
            get
            {
                if (_strFeed != string.Empty && _FeedList != null)
                    return true;
                else
                    return false;
            }
        }

        public static IEnumerable<FeedDetail> FeedList
        {
            get { return _FeedList; }
            set { }
        }

        public static void GetFeed(Int64 Category_ID, SimpleUserInfo UserInfo, MSLAServiceClient.GetFeedCompletedHandler GetFeedHandler)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAServiceClient();
            wsClient.GetFeedCompleted += GetFeedHandler;
            wsClient.GetCategoryFeedCompleted += new EventHandler<GetCategoryFeedCompletedEventArgs>(wsClient_GetFeedCompleted);
            wsClient.GetCategoryFeedAsync(Category_ID, UserInfo, wsClient.Request_ID);
        }

        public static void wsClient_GetFeedCompleted(object sender, MSLAService.GetCategoryFeedCompletedEventArgs e)
        {
            _strFeed = e.Result;
            SegRSS();
            MSLAServiceClient.GetFeedCompletedEventArgs args = new MSLAServiceClient.GetFeedCompletedEventArgs(e.Result);
            (sender as MSLAServiceClient).onGetFeedCompleted(args);
        }

        private static void SegRSS()
        {
            if (_strFeed != String.Empty)
            {
                XDocument channel = XDocument.Parse(_strFeed);
                IEnumerable<FeedDetail> Feeds =
                    from item in channel.Descendants("item")
                    where item.Element("title") != null
                    select new FeedDetail
                    {
                        Title = item.Element("title").Value,
                        Description = item.Element("description").Value,
                        Link = item.Element("link").Value,
                        PublishedDate = DateTime.Parse(item.Element("date").Value),
                    };
                _FeedList = Feeds;
            }
        }

        public class FeedDetail
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Link { get; set; }
            public DateTime PublishedDate { get; set; }
        }


        public static void GetFeedItems(Int64 Category_ID, SimpleUserInfo UserInfo, MSLAServiceClient.GetFeedItemsCompletedHandler GetFeedItemsHandler)
        {
            MSLAService.MSLAServiceClient wsClient = new MSLAServiceClient();
            wsClient.GetFeedItemsCompleted += GetFeedItemsHandler;
            wsClient.GetCategoryFeedItemsCompleted += new EventHandler<GetCategoryFeedItemsCompletedEventArgs>(wsClient_GetFeedItemsCompleted);
            wsClient.GetCategoryFeedItemsAsync(Category_ID, UserInfo, wsClient.Request_ID);
        }

        public static void wsClient_GetFeedItemsCompleted(object sender, MSLAService.GetCategoryFeedItemsCompletedEventArgs e)
        {
            List<MSLAService.FeedItem> myFeeds = new List<FeedItem>();
            myFeeds = e.Result;
            MSLAServiceClient.GetFeedItemsCompletedEventArgs args = new MSLAServiceClient.GetFeedItemsCompletedEventArgs(myFeeds);
            (sender as MSLAServiceClient).onGetFeedItemsCompleted(args);
        }


    }
}


/*
 
         private static void SegRSS()
        {
            if (_strFeed != String.Empty)
            {
                XDocument channel = XDocument.Parse(_strFeed);
                IEnumerable<FeedDetail> Feeds =
                    from item in channel.Descendants("item")
                    where item.Element("title") != null
                    select new FeedDetail
                    {
                        Title = item.Element("title").Value,
                        Description = item.Element("description").Value,
                        Link = item.Element("link").Value,
                        PublishedDate = DateTime.Parse(item.Element("date").Value),
                        //RSSElement = item,
                    };
                _FeedList = Feeds;
            }
        }

        public class FeedDetail
        {
            //private XElement _RSSElement;
            public string Title { get; set; }
            public string Description { get; set; }
            public string Link { get; set; }
            public DateTime PublishedDate { get; set; }
            //public string Guid { get; set; }
            //public string ThumbUrl { get; set; }
            //public BitmapImage Thumbnail { get; set; }

            //public XElement RSSElement
            //{
            //    set
            //    {
            //        //string ImgStr = HttpUtility.HtmlDecode(value.ToString());
            //        //ImgStr = HttpUtility.HtmlDecode(ImgStr);                   
            //        _RSSElement = value;//XElement.Parse(ImgStr);
            //        Title = value.Element("title").Value;
            //        Description = value.Element("description").Value;
            //        Link = value.Element("link").Value;
            //        //Guid = string.Empty;//_RSSElement.Element("guid").Value;
            //        //XElement ThumbXml = new XElement(_RSSElement);//_RSSElement.Element("description").Element("img");
            //        //ThumbUrl = string.Empty;//ThumbXml.Attribute("src").Value;
            //        //Thumbnail = new BitmapImage();//(new Uri(ThumbXml.Attribute("src").Value));
            //    }
            //    get { return _RSSElement; }
            //}
        }
 
 */