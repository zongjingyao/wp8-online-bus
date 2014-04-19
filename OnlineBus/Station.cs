using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OnlineBus
{
    public class Station
    {
        private string m_stationName;//站台名
        private float m_fLongitude;//经度
        private float m_fLatitude;//纬度 
        private string m_strDist;
        private ObservableCollection<Line> m_lines;

        public string Dist
        {
            get { return m_strDist; }
            set { m_strDist = value; }
        }
        public string StationName
        {
            get { return m_stationName; }
            set { m_stationName = value; }
        }

        public Station(string stat)
        {
            m_stationName = stat;
        }

        public Station()
        {

        }

        public float Longitude
        {
            get { return m_fLongitude; }
            set { m_fLongitude = value; }
        }

        public float Latitude
        {
            get { return m_fLatitude; }
            set { m_fLatitude = value; }
        }

        public ObservableCollection<Line> Lines
        {
            get { return m_lines; }
            set { m_lines = value; }
        }
    }
}
