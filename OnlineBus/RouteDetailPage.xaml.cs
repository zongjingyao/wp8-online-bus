using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using Microsoft.Phone.Tasks;

namespace OnlineBus
{
    public partial class RouteDetailPage : PhoneApplicationPage
    {
        private PhoneApplicationService m_myService = PhoneApplicationService.Current;
        private Bus m_bus;
        private Station m_selectedStat = null;
        private Line m_selectedLine = null;
        private string m_strStart = "";
        private string m_strEnd = "";   

        public RouteDetailPage()
        {
            InitializeComponent();

            tbkCity.Text = "城市-" + WebService.GetCity();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            object temp_bus;

            m_strStart = NavigationContext.QueryString["start"];
            m_strEnd = NavigationContext.QueryString["end"];

            tbkRoute.Text = m_strStart + "→" + m_strEnd;

            if (m_myService.State.ContainsKey("selectedBus"))
            {
                if (m_myService.State.TryGetValue("selectedBus", out temp_bus))
                {
                    m_bus = (Bus)temp_bus;
                    llsRouteDetail.ItemsSource = m_bus.Segments;
                }
            }

            base.OnNavigatedTo(e);
        }

        private void btnStat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string strStatName = ((Button)sender).Content.ToString();
            WebService.GetStats(strStatName, StatWebClient_Completed);   
        }

        private void StatWebClient_Completed(object sender, OpenReadCompletedEventArgs e)
        {
            using (StreamReader reader = new StreamReader(e.Result))
            {
                string contents = reader.ReadToEnd();
                ObservableCollection<Station> stats = XMLUtils.parseXMLForStat(contents);
                m_selectedStat = stats.First();
                NavigationService.Navigate(new Uri("/StatDetailPage.xaml", UriKind.Relative));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (m_selectedStat != null)
            {
                m_myService.State["selectedStat"] = m_selectedStat;
            }

            if (m_selectedLine != null)
            {
                m_myService.State["selectedLine"] = m_selectedLine;
            }

            base.OnNavigatedFrom(e);
        }

        private void btnLine_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string strLineName = ((Button)sender).Content.ToString();
            WebService.GetBusLines(strLineName, LineWebClient_Completed); 
        }

        private void LineWebClient_Completed(object sender, OpenReadCompletedEventArgs e)
        {
            using (StreamReader reader = new StreamReader(e.Result))
            {
                string contents = reader.ReadToEnd();
                ObservableCollection<Line> lines = XMLUtils.parseXMLForLine(contents);
                m_selectedLine = lines.First();
                NavigationService.Navigate(new Uri("/LineDetailPage.xaml", UriKind.Relative));
            }
        }

        private void appBarBtnFav_Click(object sender, System.EventArgs e)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    IsolatedStorageFileStream location;
                    if (!storage.FileExists("favoriteRoutes.dat"))
                    {
                        location = new IsolatedStorageFileStream("favoriteRoutes.dat", System.IO.FileMode.CreateNew, storage); 
                        location.Dispose();
                    }

                    location = new IsolatedStorageFileStream("favoriteRoutes.dat", System.IO.FileMode.Open, storage);
                    string strTemp = WebService.GetCity() + "," + m_strStart + "," + m_strEnd + ";";
                    StreamReader sr = new StreamReader(location);
                    string content = sr.ReadToEnd();
                    sr.Close();
                    location.Dispose();  
                    if (content.Contains(strTemp))
                    {
                        MessageBox.Show("已收藏");
                    }
                    else 
                    {
                        location = new IsolatedStorageFileStream("favoriteRoutes.dat", System.IO.FileMode.Append, storage);
                        StreamWriter sw = new StreamWriter(location);
                        sw.Write(strTemp);
                        sw.Close();
                        MessageBox.Show("收藏成功");
                        location.Dispose();
                    } 
                }
                catch (Exception e1)
                {
                    Debug.WriteLine(e1.Message);
                }

            }
        }

        private void appBarBtnShare_Click(object sender, System.EventArgs e)
        {
            int time = int.Parse((string)m_bus.Time);
            string strTime;
            if(time < 60)
            {
                strTime = "用时约" + time + "分钟，";
            }
            else
            {
                strTime = "用时约" + time/60 + "小时" + time%60 + "分钟，";
            }

            int distance = int.Parse((string)m_bus.Distance);
            string strDist;
            if(distance <= 1000)
            {
                strDist = "距离约" + distance + "米";
            }
            else
            {
                strDist = "距离约" + (distance/1000.0).ToString("0.0") + "公里";
            }

            string content = "我用#在线公交#分享了一条公交换乘路线：";
            content += m_strStart + "→" + m_strEnd + "：";
            content += "换乘" + m_bus.ChangeBusCount + "次，" + strTime + strDist + "。换乘详细：";
            foreach(Segment seg in m_bus.Segments)
            {
                content += "步行" + seg.FootDistance + "米到" + seg.StartStat;
                content += ",乘" + seg.LineName + "(" + seg.Stats.Length + "站)到" + seg.EndStat + ";";
            }
            Debug.WriteLine(content);
            SmsComposeTask sct = new SmsComposeTask();
            //sct.To = "";
            sct.Body = content;
            sct.Show();

        }
    }
}