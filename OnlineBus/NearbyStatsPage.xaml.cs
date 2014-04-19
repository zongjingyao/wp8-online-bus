using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Com.AMap.Api.Maps;
using Com.AMap.Api.Maps.Model;
using Com.AMap.Api.Services.Results;
using Com.AMap.Api.Services;
using BusTest;

namespace OnlineBus
{
    public partial class NearbyStatsPage : PhoneApplicationPage
    {
        private PhoneApplicationService m_myService = PhoneApplicationService.Current;
        private ObservableCollection<Station> m_nearbyStats = null;
        private AMapPositionChangedEventArgs m_args;
         AMap m_amap;
        private AMapMarker m_marker;//标注点
        private AMapCircle m_circle;//圆

        public NearbyStatsPage()
        {
            InitializeComponent();

            m_amap = new AMap();
            m_amap.MarkerClickListener += amap_MarkerClickListener;
            this.ContentPanel.Children.Add(m_amap);
        }

        private void amap_MarkerClickListener(AMapMarker sender, AMapEventArgs args)
        {
            sender.ShowInfoWindow(new AInfoWindow()
            {
                Title = sender.Title,
                //ContentText = sender.Snippet,
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            object temp_nearbyStat;
            object temp_args;

            if (m_myService.State.ContainsKey("nearbyStats"))
            {
                if (m_myService.State.TryGetValue("nearbyStats", out temp_nearbyStat))
                {
                    m_nearbyStats = (ObservableCollection<Station>)temp_nearbyStat;
                }
            }

            if (m_myService.State.ContainsKey("args"))
            {
                if (m_myService.State.TryGetValue("args", out temp_args))
                {
                    m_args = (AMapPositionChangedEventArgs)temp_args;
                }
            }

            base.OnNavigatedTo(e);
        }

        private void AddToMap()
        {
            //添加圆
            m_circle = m_amap.AddCircle(new AMapCircleOptions()
            {
                Center = m_args.LngLat,//圆点位置
                Radius = (float)m_args.Accuracy,//半径
                FillColor = Color.FromArgb(80, 100, 150, 255),
                StrokeWidth = 2,//边框粗细
                StrokeColor = Color.FromArgb(80, 0, 0, 255),//边框颜色

            });

            //添加点标注，用于标注地图上的点
            m_marker = m_amap.AddMarker(
            new AMapMarkerOptions()
            {
                Position = m_args.LngLat,//图标的位置
                Title = "我的位置",
                Snippet = m_args.LngLat.ToString(),
                IconUri = new Uri("./Assets/Image/marker_gps_no_sharing.png", UriKind.Relative),//图标的URL
                Anchor = new Point(0.5, 0.5),//图标中心点
            });

            foreach (Station stat in m_nearbyStats)
            {
                AMapMarkerOptions opt = new AMapMarkerOptions()
                {
                    Position = new LatLng(stat.Latitude,stat.Longitude),
                    Title = stat.StationName,
                    Snippet = stat.StationName,
                    IconUri = new Uri("./Assets/Image/BLUE.png", UriKind.Relative),
                };
                m_amap.AddMarker(opt);
            }

            //设置当前地图的经纬度和缩放级别
            m_amap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(m_args.LngLat, 15));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            AddToMap();
        }
    }
}