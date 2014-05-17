using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.IO.IsolatedStorage;

namespace OnlineBus
{
    public class WebService
    {
        private static string TYPE_TRANSFER = "transfer";
        private static string TYPE_LINES = "lines";
        private static string TYPE_STATS = "stats";
        private static string TYPE_STATS_XY = "stats_xy";
        private static string m_strKey = "e7a17c174ed60c1f4384b5a2a9f3f0d5";
        private static string m_strBaseUrl = "http://openapi.aibang.com/bus/";
        private static string m_strCity = null;
        private static string DEFAULT_CITY = "南京";
        private static IsolatedStorageSettings m_appSetting;

        public static string City
        {
            get { return WebService.m_strCity; }
            set { WebService.m_strCity = value; }
        }       

        public delegate void CallBackDelegate(object sender, OpenReadCompletedEventArgs e);

        public static string GetCity()
        {
            if(m_strCity == null)
            {
                m_appSetting = IsolatedStorageSettings.ApplicationSettings;
                if (!m_appSetting.Contains("city"))
                {
                    m_strCity = DEFAULT_CITY;
                }
                else
                {
                    m_strCity = m_appSetting["city"].ToString();
                }       
            }
            
            return m_strCity;
        }

        public static void GetBusRoutes(string strStart, string strEnd, int rc, CallBackDelegate callBack)
        {
            GetCity();
            string strUri = m_strBaseUrl + TYPE_TRANSFER + "?app_key=" + m_strKey + "&city=" + m_strCity + "&start_addr=" + strStart + "&end_addr=" + strEnd + "&rc=" + rc;
            DoWebClient(strUri, callBack);
        }

        public static void GetBusLines(string strLineName, CallBackDelegate callBack)
        {
            GetCity();
            string strUri = m_strBaseUrl + TYPE_LINES + "?app_key=" + m_strKey + "&city=" + m_strCity + "&q=" + strLineName;
            DoWebClient(strUri, callBack);
        }

        public static void GetStats(string strStatName, CallBackDelegate callBack)
        {
            GetCity();
            string strUri = m_strBaseUrl + TYPE_STATS + "?app_key=" + m_strKey + "&city=" + m_strCity + "&q=" + strStatName;
            DoWebClient(strUri, callBack);
        }

        private static void DoWebClient(string strUri, CallBackDelegate callBack)
        {
            Debug.WriteLine(strUri);
            Uri uri = new Uri(strUri);
            WebClient webClient = new WebClient();
            webClient.OpenReadAsync(uri);//在不阻止调用线程的情况下，从资源返回数据
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(callBack);//异步操作完成时发生
        }

        public static void GetNearbyStats(string lng, string lat, string dist, CallBackDelegate callBack)
        {
            GetCity();
            string strUri = m_strBaseUrl + TYPE_STATS_XY + "?app_key=" + m_strKey + "&city=" + m_strCity + "&lng=" + lng + "&lat=" + lat + "&dist=" + dist;
            DoWebClient(strUri, callBack);
        }
    }
}
