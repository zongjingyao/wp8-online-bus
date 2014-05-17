using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace OnlineBus
{
    public partial class BusRoutesPage : PhoneApplicationPage
    {
        private string m_strStart = "";
        private string m_strEnd = "";       
        private PhoneApplicationService m_myService = PhoneApplicationService.Current;
        private Bus m_selectedBus;
        private ObservableCollection<Bus>[] m_buses;
        private int m_formerIndex = 0;
        private Button[] m_buttons;
        
        public BusRoutesPage()
        {
            InitializeComponent();

            tbkCity.Text = "城市-" + WebService.GetCity();
            m_buses = new ObservableCollection<Bus>[4]{null,null,null,null};
            m_buttons = new Button[] { btnLessChange,btnLessWalk,btnLessTime,btnSubWayFirst };
            m_formerIndex = 0;
            btnLessChange.BorderThickness = new Thickness(0, 0, 0, 3);
            btnLessChange.IsEnabled = false;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(llsBuses.ItemsSource == null)
            {
                m_strStart = NavigationContext.QueryString["start"];
                m_strEnd = NavigationContext.QueryString["end"];

                tbkRoute.Text = m_strStart + "→" + m_strEnd;
                WebService.GetBusRoutes(m_strStart,m_strEnd,1,webClient_Completed);
            }
        }

        private void webClient_Completed(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                using (StreamReader reader = new StreamReader(e.Result))
                {
                    string contents = reader.ReadToEnd();
                    ObservableCollection<Bus> buses = XMLUtils.parseXMLForBusRoutes(contents);
                    llsBuses.ItemsSource = buses;
                    llsBuses.Visibility = Visibility.Visible;
                    
                    m_buses[m_formerIndex] = buses;
                }
            }
            catch
            {
                MessageBox.Show("无法定位站点", "异常", MessageBoxButton.OK);
            }
            finally
            {
                progressBar.Visibility = Visibility.Collapsed;
            }          
        }

        private void llsBuses_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var listSelector = sender as LongListSelector;
            if (listSelector.SelectedItem == null)
                return;
            m_selectedBus = listSelector.SelectedItem as Bus;
            Debug.WriteLine(m_selectedBus.Segments.First().LineName+"\t"+m_selectedBus.Segments.Last().LineName);

            NavigationService.Navigate(new Uri("/RouteDetailPage.xaml?start=" + m_strStart + "&end=" + m_strEnd, UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            m_myService.State["selectedBus"] = m_selectedBus;
            base.OnNavigatedFrom(e);
        }

        private void btnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Button currentBtn = (Button)sender;
            m_buttons[m_formerIndex].IsEnabled = true;
            currentBtn.BorderThickness = new Thickness(0, 0, 0, 3);
            m_buttons[m_formerIndex].BorderThickness = new Thickness(0, 0, 0, 0);
            currentBtn.IsEnabled = false;
            m_formerIndex = int.Parse(currentBtn.Tag.ToString());

            if(m_buses[m_formerIndex] != null)
            {
                llsBuses.ItemsSource = m_buses[m_formerIndex];
            }
            else
            {
                int rc = m_formerIndex + 1;
                if(rc == 4)
                {
                    rc++;
                }
                llsBuses.Visibility = Visibility.Collapsed;
                progressBar.Visibility = Visibility.Visible;
                WebService.GetBusRoutes(m_strStart, m_strEnd, rc, webClient_Completed);
            }

        }
    }
}