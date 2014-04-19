using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.IO.IsolatedStorage;

namespace OnlineBus
{
    public partial class CitysPage : PhoneApplicationPage
    {
        private List<City> m_citys;
        private List<string> m_keys;
        private IsolatedStorageSettings m_appSettings;

        public CitysPage()
        {
            InitializeComponent();

            tbkCity.Text = "城市-" + WebService.GetCity();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            m_keys = new List<string>();
            m_citys = new List<City>();
            XMLUtils.ParseXMLForCitys(m_keys, m_citys);
            BindData();
        }

        private void BindData()
        {
            List<ProvinceGroup<City>> DataSource = ProvinceGroup<City>.CreateGroups(m_citys, m_keys,(City c) => { return c.ProvinceOfCity; });
            llsCitys.ItemsSource = DataSource;
        }

        private void llsCitys_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var listSelector = sender as LongListSelector;
            if (listSelector.SelectedItem == null)
                return;
            City selectedCity = listSelector.SelectedItem as City;
            SaveCity(selectedCity.CityName);

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            while (App.RootFrame.BackStack.Count() > 0)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void SaveCity(string city)
        {
            m_appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (m_appSettings.Contains("city"))
            {
                m_appSettings["city"] = city;
            }
            else
            {
                m_appSettings.Add("city", city);
            }
            m_appSettings.Save();

            WebService.City = city;
        }
    }
}