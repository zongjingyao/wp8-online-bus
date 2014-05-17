using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using OnlineBus.Resources;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Com.AMap.Api.Services.Results;
using Com.AMap.Api.Services;
using Com.AMap.Api.Maps;
using Com.AMap.Api.Maps.Model;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace OnlineBus
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Line m_selectedLine = null;
        private Station m_selectedStat = null;
        private ObservableCollection<Station> m_nearbyStats = null;
        private PhoneApplicationService m_myService = PhoneApplicationService.Current;
        private LatLng m_location;
        private AMapGeolocator m_mylocation = null;
        private string DEFAULT_DIST = "1000";
        private AMapPositionChangedEventArgs m_args;
        private bool m_bIsCheckedCity = false;

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            ptMain.Title = "城市-" + WebService.GetCity();

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string strStart = ptbxStart.Text;
            string strEnd = ptbxEnd.Text;
            if (strStart.Trim().Length <= 0 || strEnd.Trim().Length <= 0)
            {
                MessageBox.Show("请输入站点名", "提醒", MessageBoxButton.OK);
                return;
            }
            SaveHistory(strStart,strEnd);

            NavigationService.Navigate(new Uri("/BusRoutesPage.xaml?start=" + strStart + "&end=" + strEnd, UriKind.Relative));
        }

        private void LineWebClient_Completed(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                using (StreamReader reader = new StreamReader(e.Result))
                {
                    string contents = reader.ReadToEnd();
                    ObservableCollection<Line> lines = XMLUtils.parseXMLForLine(contents);
                    if (lines.Count == 0)
                    {
                        MessageBox.Show("无此线路");
                    }
                    else
                    {
                        llsLines.ItemsSource = lines;
                    }
                }
            }
            catch
            {
                MessageBox.Show("数据获取失败，请检查您的网络！", "错误", MessageBoxButton.OK);
            }
            finally
            {
                pgbLine.Visibility = Visibility.Collapsed;
                btnSearchForLine.IsEnabled = true;
            }
        }

        private void llsLines_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var listSelector = sender as LongListSelector;
            if (listSelector.SelectedItem == null)
                return;
            m_selectedLine = listSelector.SelectedItem as Line;

            NavigationService.Navigate(new Uri("/LineDetailPage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (m_selectedLine != null)
            {
                m_myService.State["selectedLine"] = m_selectedLine;
            }

            if (m_selectedStat != null)
            {
                m_myService.State["selectedStat"] = m_selectedStat;
            }

            if (m_nearbyStats != null)
            {
                m_myService.State["nearbyStats"] = m_nearbyStats;
            }

            if (m_args != null)
            {
                m_myService.State["args"] = m_args;
            }

            base.OnNavigatedFrom(e);
        }

        private void btnSearchForLine_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string strLineName = ptbxLineName.Text;
            if (strLineName.Trim().Length == 0)
            {
                MessageBox.Show("请输入线路名", "提醒", MessageBoxButton.OK);
            }
            else
            {
                pgbLine.Visibility = Visibility.Visible;
                btnSearchForLine.IsEnabled = false;
                WebService.GetBusLines(strLineName, LineWebClient_Completed);
            }
        }

        private void btnSearchForStat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string strStatName = ptbxStatName.Text.Trim();
            if (strStatName.Length == 0)
            {
                MessageBox.Show("请输入站点名", "提醒", MessageBoxButton.OK);
            }
            else
            {
                pgbStat.Visibility = Visibility.Visible;
                btnSearchForStat.IsEnabled = false;
                WebService.GetStats(strStatName, StatWebClient_Completed);
            }
        }

        private void StatWebClient_Completed(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                using (StreamReader reader = new StreamReader(e.Result))
                {
                    string contents = reader.ReadToEnd();
                    ObservableCollection<Station> stats = XMLUtils.parseXMLForStat(contents);
                    if (stats.Count == 0)
                    {
                        MessageBox.Show("无此站点");
                    }
                    else
                    {
                        llsStats.ItemsSource = stats;
                        gdSearchedStat.Visibility = Visibility.Visible;
                        gdNearbyStat.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch
            {
                MessageBox.Show("数据获取失败，请检查您的网络！", "错误", MessageBoxButton.OK);
            }
            finally
            {
                pgbStat.Visibility = Visibility.Collapsed;
                btnSearchForStat.IsEnabled = true;
            }
        }

        private void llsStats_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var listSelector = sender as LongListSelector;
            if (listSelector.SelectedItem == null)
                return;
            m_selectedStat = listSelector.SelectedItem as Station;

            NavigationService.Navigate(new Uri("/StatDetailPage.xaml", UriKind.Relative));
        }

        private void btnChooseCity_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CitysPage.xaml", UriKind.Relative));
        }

        private void btnAbout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确定要退出吗？", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void ptbxLineName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (ptbxLineName.Text.Trim().Length == 0)
            {
                llsLines.ItemsSource = null;
            }
        }

        private void ptbxStatName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (ptbxStatName.Text.Trim().Length == 0)
            {
                llsStats.ItemsSource = null;
                if (llsNearbyStats.ItemsSource != null)
                {
                    gdSearchedStat.Visibility = Visibility.Collapsed;
                    gdNearbyStat.Visibility = Visibility.Visible;
                }
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            BindHistoryData();

            m_mylocation = new AMapGeolocator();
            m_mylocation.Start();
            //触发位置改变事件
            m_mylocation.PositionChanged += mylocation_PositionChanged;
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_mylocation != null)
            {
                m_mylocation.PositionChanged -= mylocation_PositionChanged;
                m_mylocation.Stop();
            }
        }

        private async void mylocation_PositionChanged(AMapGeolocator sender, AMapPositionChangedEventArgs args)
        {
            m_location = args.LngLat;
            m_args = args;
            Debug.WriteLine("定位精度：" + args.Accuracy + "米");
            Debug.WriteLine("定位经纬度：" + args.LngLat);
            flashNearbyStat();

            //if (!m_bIsCheckedCity)
            {
                Debug.WriteLine("开始检查城市");
                await CheckCity(args.LngLat.longitude, args.LngLat.latitude);
            }
        }

        private async Task CheckCity(double lon, double lat)
        {
            AMapReGeoCodeResult rcc = await AMapReGeoCodeSearch.GeoCodeToAddress(lon, lat);

            this.Dispatcher.BeginInvoke(() =>
            {
                if (rcc.Erro == null && rcc.ReGeoCode != null)
                {
                    AMapReGeoCode regeocode = rcc.ReGeoCode;
                    AMapAddressComponent addressComponent = regeocode.Address_component;
                    string strCurrentCity = addressComponent.City;
                    string strLocation = "";
                    strLocation += addressComponent.Province + addressComponent.City + addressComponent.District
                        + addressComponent.Township + addressComponent.Stree_number.Street;
                    if (addressComponent.Stree_number.Number.Length > 0)
                    {
                        strLocation += addressComponent.Stree_number.Number + "号";
                    }
                    tbkLocation.Text = strLocation;
                    Debug.WriteLine(strLocation);
                    if (strCurrentCity.Contains("市"))
                    {
                        strCurrentCity = strCurrentCity.Replace("市", "");
                    }
                    Debug.WriteLine("当前城市：" + strCurrentCity);
                    Debug.WriteLine("设置城市：" + WebService.GetCity());

                    if (!WebService.GetCity().Equals(strCurrentCity))
                    {
                        List<string> keys = new List<string>();
                        List<City> citys = new List<City>();
                        XMLUtils.ParseXMLForCitys(keys, citys);
                        bool isFind = false;
                        foreach (City city in citys)
                        {
                            if (strCurrentCity.Equals(city.CityName))
                            {
                                isFind = true;
                                break;
                            }
                        }
                        if (isFind)
                        {
                            if (MessageBox.Show("是否切换到当前城市？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                IsolatedStorageSettings appSettings;
                                appSettings = IsolatedStorageSettings.ApplicationSettings;
                                if (appSettings.Contains("city"))
                                {
                                    appSettings["city"] = strCurrentCity;
                                }
                                else
                                {
                                    appSettings.Add("city", strCurrentCity);
                                }
                                appSettings.Save();
                                WebService.City = strCurrentCity;
                                ptMain.Title = "城市-" + WebService.GetCity();
                            }
                        }
                        else
                        {
                            Debug.WriteLine("不支持当前城市！");
                        }
                    }
                }
                else
                {
                    //MessageBox.Show(rcc.Erro.Message);
                }

            });

            m_bIsCheckedCity = true;
        }

        private void flashNearbyStat()
        {
            WebService.GetNearbyStats(m_location.longitude.ToString(),
                                        m_location.latitude.ToString(),
                                        DEFAULT_DIST,
                                        NearByStatWebClient_Completed);
        }

        private void NearByStatWebClient_Completed(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                using (StreamReader reader = new StreamReader(e.Result))
                {
                    string contents = reader.ReadToEnd();
                    m_nearbyStats = XMLUtils.parseXMLForNearbyStat(contents);
                    if (m_nearbyStats.Count != 0)
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            llsNearbyStats.ItemsSource = m_nearbyStats;
                            if (gdSearchedStat.Visibility != Visibility.Visible)
                            {
                                gdNearbyStat.Visibility = Visibility.Visible;
                            }
                        });
                    }
                }
            }
            catch
            {
                //MessageBox.Show("数据获取失败，请检查您的网络！", "错误", MessageBoxButton.OK);
            }
            finally
            {
                
            }
        }

        private void btnMap_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NearByStatsPage.xaml", UriKind.Relative));
        }

        private void btnFav_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FavoritePage.xaml", UriKind.Relative));
        }

        private void SaveHistory(string strStart,string strEnd)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    IsolatedStorageFileStream location;
                    if (!storage.FileExists("HistoryRoutes.dat"))
                    {
                        location = new IsolatedStorageFileStream("HistoryRoutes.dat", System.IO.FileMode.CreateNew, storage);
                        location.Dispose();
                    }

                    location = new IsolatedStorageFileStream("HistoryRoutes.dat", System.IO.FileMode.Open, storage);
                    string strTemp = WebService.GetCity() + "," + strStart + "," + strEnd + ";";
                    StreamReader sr = new StreamReader(location);
                    string content = sr.ReadToEnd();
                    Debug.WriteLine("old:"+content);
                    sr.Close();
                    location.Dispose();
                    if (content.Contains(strTemp))
                    {
                        content = content.Replace(strTemp, "");
                    }
                    content = content.Insert(0, strTemp);

                    string[] units = content.Split(';');
                    if(units.Length > 11)
                    {
                        content = "";
                        for(int i = 0; i < 10; i++)
                        {
                            content += units[i] + ";";
                        }
                    }

                    Debug.WriteLine("new:" + content);
                    location = new IsolatedStorageFileStream("HistoryRoutes.dat", System.IO.FileMode.Create, storage);
                    StreamWriter sw = new StreamWriter(location);
                    sw.Write(content);
                    sw.Close();
                    location.Dispose();
                    
                }
                catch (Exception e1)
                {
                    Debug.WriteLine(e1.Message);
                }

            }
        }

        private void BindHistoryData()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    ObservableCollection<Route> routes = new ObservableCollection<Route>();
                    IsolatedStorageFileStream location = new IsolatedStorageFileStream("HistoryRoutes.dat", System.IO.FileMode.Open, storage);
                    StreamReader sr = new StreamReader(location);
                    string content = sr.ReadToEnd();
                    string[] strRoutes = content.Split(';');
                    foreach (string strTempRoute in strRoutes)
                    {
                        Route route = new Route();
                        string[] strRoute = strTempRoute.Split(',');
                        if (strRoute[0] == WebService.GetCity())
                        {
                            route.StartStat = strRoute[1];
                            route.EndStat = strRoute[2];
                            route.Info = strRoute[1] + "-" + strRoute[2];
                            routes.Add(route);
                        }
                    }
                    llsHistory.ItemsSource = routes;
                    location.Dispose();
                }
                catch (Exception e1)
                {
                    Debug.WriteLine(e1.Message);
                }

            }
        }

        private void llsHistory_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var listSelector = sender as LongListSelector;
            if (listSelector.SelectedItem == null)
                return;
            Route history = listSelector.SelectedItem as Route;
            SaveHistory(history.StartStat, history.EndStat);
            NavigationService.Navigate(new Uri("/BusRoutesPage.xaml?start=" + history.StartStat + "&end=" + history.EndStat, UriKind.Relative));
        }
    }
}