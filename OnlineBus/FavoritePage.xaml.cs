using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;

namespace OnlineBus
{
    public partial class FavoritePage : PhoneApplicationPage
    {
        public FavoritePage()
        {
            InitializeComponent();

            BindData();
        }

        private void BindData()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    ObservableCollection<Route> routes = new ObservableCollection<Route>();
                    IsolatedStorageFileStream location = new IsolatedStorageFileStream("favoriteRoutes.dat", System.IO.FileMode.Open, storage);
                    StreamReader sr = new StreamReader(location);
                    string content = sr.ReadToEnd();
                    string[] strRoutes = content.Split(';');
                    foreach(string strTempRoute in strRoutes)
                    {
                        Route route = new Route();
                        string[] strRoute = strTempRoute.Split(',');
                        if(strRoute[0] == WebService.GetCity())
                        {
                            route.StartStat = strRoute[1];
                            route.EndStat = strRoute[2];
                            route.Info = strTempRoute + ";";
                            routes.Add(route);
                        }
                    }
                    llsFavRoutes.ItemsSource = routes;
                    location.Dispose();
                }
                catch (Exception e1)
                {
                    Debug.WriteLine(e1.Message);
                }

            }
        }

        private void llsFavRoutes_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var listSelector = sender as LongListSelector;
            if (listSelector.SelectedItem == null)
                return;
            Route route = listSelector.SelectedItem as Route;
            NavigationService.Navigate(new Uri("/BusRoutesPage.xaml?start=" + route.StartStat + "&end=" + route.EndStat, UriKind.Relative));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            MenuItem menu = sender as MenuItem;
            string strTemp = menu.Tag.ToString();

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    IsolatedStorageFileStream location;

                    location = new IsolatedStorageFileStream("favoriteRoutes.dat", System.IO.FileMode.Open, storage);
                    StreamReader sr = new StreamReader(location);
                    string content = sr.ReadToEnd();
                    sr.Close();
                    location.Dispose();
                    content = content.Replace(strTemp, "");

                    location = new IsolatedStorageFileStream("favoriteRoutes.dat", System.IO.FileMode.Truncate, storage);
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

            llsFavRoutes.ItemsSource = null;
            BindData();
        }
    }

    public class Route
    {
        private string m_strStartStat;
        private string m_strEndStat;
        private string m_strInfo;

        public string Info
        {
            get { return m_strInfo; }
            set { m_strInfo = value; }
        }

        public string StartStat
        {
            get { return m_strStartStat; }
            set { m_strStartStat = value; }
        }
        
        public string EndStat
        {
            get { return m_strEndStat; }
            set { m_strEndStat = value; }
        }
    }
}