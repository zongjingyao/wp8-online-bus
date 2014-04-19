using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OnlineBus
{
    public class Bus
    {
        private string m_strDistance;
        private string m_strTime;
        private string m_strFootDistance;
        private string m_strLastFootDistance;
        private ObservableCollection<Segment> m_segments;
        private int m_changeBusCount;

        public string Distance
        {
            get { return m_strDistance; }
            set { m_strDistance = value; }
        }

        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }

        public string FootDistance
        {
            get { return m_strFootDistance; }
            set { m_strFootDistance = value; }
        }

        public string LastFootDistance
        {
            get { return m_strLastFootDistance; }
            set { m_strLastFootDistance = value; }
        }

        public ObservableCollection<Segment> Segments
        {
            get { return m_segments; }
            set { m_segments = value; m_changeBusCount = m_segments.Count - 1; }
        }

        public int ChangeBusCount
        {
            get { return m_changeBusCount; }
            set { m_changeBusCount = value; }
        }
    }

    public class Segment
    {
        private string m_strStartStat;
        private string m_strEndStat;
        private string m_strLineName;
        private string[] m_stats;
        private string m_strLineDistance;
        private string m_strFootDistance;   

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

        public string LineName
        {
            get { return m_strLineName; }
            set 
            {
                if(value.Contains('('))
                    m_strLineName = value.Substring(0, value.IndexOf('('));
            }
        }
        
        public string[] Stats
        {
            get { return m_stats; }
            set { m_stats = value; }
        }

        public string LineDistance
        {
            get { return m_strLineDistance; }
            set { m_strLineDistance = value; }
        }

        public string FootDistance
        {
            get { return m_strFootDistance; }
            set { m_strFootDistance = value; }
        }
    }
}
