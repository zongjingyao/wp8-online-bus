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
        
        public BusRoutesPage()
        {
            InitializeComponent();

            tbkCity.Text = "城市-" + WebService.GetCity();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(llsBuses.ItemsSource == null)
            {
                m_strStart = NavigationContext.QueryString["start"];
                m_strEnd = NavigationContext.QueryString["end"];

                tbkRoute.Text = m_strStart + "→" + m_strEnd;
                WebService.GetBusRoutes(m_strStart,m_strEnd,webClient_Completed);
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
    }
}