using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OnlineBus
{
    public class Line
    {
        private string m_strLineName; 
        private string m_strInfo;      
        private ObservableCollection<Station> m_stats;
        
        public string LineName
        {
            get { return m_strLineName; }
            set { m_strLineName = value; }
        }

        public string Info
        {
            get { return m_strInfo; }
            set { m_strInfo = value; }
        }

        public ObservableCollection<Station> Stats
        {
            get { return m_stats; }
            set { m_stats = value; }
        }
    }
}
