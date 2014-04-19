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
using System.Collections.ObjectModel;

namespace OnlineBus
{
    public partial class LineDetailPage : PhoneApplicationPage
    {
        private PhoneApplicationService m_myService = PhoneApplicationService.Current;
        private Station m_selectedStat = null;
        
        public LineDetailPage()
        {
            InitializeComponent();

            tbkCity.Text = "城市-" + WebService.GetCity();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            object temp_line;

            if (m_myService.State.ContainsKey("selectedLine"))
            {
                if (m_myService.State.TryGetValue("selectedLine", out temp_line))
                {
                    Line line = (Line)temp_line;
                    tbkLineName.Text = line.LineName;
                    tbkInfo.Text = line.Info;
                    llsStats.ItemsSource = line.Stats;
                }
            }

            base.OnNavigatedTo(e);
        }

        private void llsStats_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var listSelector = sender as LongListSelector;
            if (listSelector.SelectedItem == null)
                return;
            Station selectedStat = listSelector.SelectedItem as Station;
            string strStatName = selectedStat.StationName;
            
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

            base.OnNavigatedFrom(e);
        }
    }
}