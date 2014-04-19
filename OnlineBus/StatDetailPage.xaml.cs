using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;

namespace OnlineBus
{
    public partial class StatDetailPage : PhoneApplicationPage
    {
        private PhoneApplicationService m_myService = PhoneApplicationService.Current;
        private Line m_selectedLine = null;

        public StatDetailPage()
        {
            InitializeComponent();

            tbkCity.Text = "城市-" + WebService.GetCity();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            object temp_stat;

            if (m_myService.State.ContainsKey("selectedStat"))
            {
                if (m_myService.State.TryGetValue("selectedStat", out temp_stat))
                {
                    Station stat = (Station)temp_stat;
                    tbkStatName.Text = stat.StationName;
                    llsLines.ItemsSource = stat.Lines;
                }
            }

            base.OnNavigatedTo(e);
        }

        private void llsLines_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var listSelector = sender as LongListSelector;
            if (listSelector.SelectedItem == null)
                return;
            Line selectedLine = listSelector.SelectedItem as Line;
            string strLineName = selectedLine.LineName;

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (m_selectedLine != null)
            {
                m_myService.State["selectedLine"] = m_selectedLine;
            }

            base.OnNavigatedFrom(e);
        }
    }
}