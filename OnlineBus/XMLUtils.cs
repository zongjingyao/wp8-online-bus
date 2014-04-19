using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using Windows.Storage;

namespace BusTest
{
    public class XMLUtils
    {
        public static void ParseXMLForCitys(List<string> keys, List<City> citys)
        {
            XDocument xdoc = XDocument.Load("./Assets/citys.xml");
            var temp_provinces = from temp_province in xdoc.Descendants("province")
                                 select new
                                 {
                                     name = temp_province.Element("name").Value,
                                     citys = temp_province.Element("citys").Value,
                                 };
            foreach (var temp_province in temp_provinces)
            {
                keys.Add(temp_province.name);
                foreach (string strCity in temp_province.citys.Split(','))
                {
                    City city = new City(temp_province.name, strCity);
                    citys.Add(city);
                }
            }
        }

        public static ObservableCollection<Line> parseXMLForLine(string xmlFile)
        {
            XDocument xdoc = XDocument.Parse(xmlFile);

            var temp_lines = from temp_line in xdoc.Descendants("line")
                             select new
                             {
                                 name = temp_line.Element("name").Value,
                                 info = temp_line.Element("info").Value,
                                 stats = temp_line.Element("stats").Value,
                             };
            ObservableCollection<Line> lines = new ObservableCollection<Line>();
            foreach (var temp_line in temp_lines)
            {
                Line line = new Line();
                line.LineName = temp_line.name;
                line.Info = temp_line.info;
                ObservableCollection<Station> stats = new ObservableCollection<Station>();
                foreach (string stat in temp_line.stats.Split(';'))
                {
                    stats.Add(new Station(stat));
                }
                line.Stats = stats;
                lines.Add(line);
            }

            return lines;
        }

        public static ObservableCollection<Station> parseXMLForStat(string xmlFile)
        {
            XDocument xdoc = XDocument.Parse(xmlFile);

            var temp_stats = from temp_stat in xdoc.Descendants("stat")
                             select new
                             {
                                 name = temp_stat.Element("name").Value,
                                 xy = temp_stat.Element("xy").Value,
                                 line_names = temp_stat.Element("line_names").Value,
                             };
            ObservableCollection<Station> stats = new ObservableCollection<Station>();
            foreach (var temp_stat in temp_stats)
            {
                Station stat = new Station();
                stat.StationName = temp_stat.name;
                if (temp_stat.xy.Length > 0)
                {
                    string[] xy = temp_stat.xy.Split(',');
                    stat.Longitude = float.Parse(xy[0]);
                    stat.Latitude = float.Parse(xy[1]);
                }              

                ObservableCollection<Line> lines = new ObservableCollection<Line>();
                foreach (string line_name in temp_stat.line_names.Split(';'))
                {
                    Line line = new Line();
                    int index = line_name.IndexOf('(');
                    line.LineName = line_name.Substring(0, index);
                    line.Info = line_name.Substring(index + 1, line_name.LastIndexOf(')') - index - 1);
                    lines.Add(line);
                }
                stat.Lines = lines;
                stats.Add(stat);
            }

            return stats;
        }

        public static ObservableCollection<Bus> parseXMLForBusRoutes(string xmlFile)
        {
            ObservableCollection<Bus> buses = new ObservableCollection<Bus>();
            XDocument xdoc = XDocument.Parse(xmlFile);

            var temp_buses = from temp_bus in xdoc.Descendants("bus")
                             select new
                             {
                                 dist = temp_bus.Element("dist").Value,
                                 time = temp_bus.Element("time").Value,
                                 last_foot_dist = temp_bus.Element("last_foot_dist").Value,
                                 foot_dist = temp_bus.Element("foot_dist").Value,

                                 segments = from segment in temp_bus.Descendants("segment")
                                            select new
                                            {
                                                start_stat = segment.Element("start_stat").Value,
                                                end_stat = segment.Element("end_stat").Value,
                                                line_name = segment.Element("line_name").Value,
                                                stats = segment.Element("stats").Value,
                                                line_dist = segment.Element("line_dist").Value,
                                                foot_dist = segment.Element("foot_dist").Value
                                            },
                             };
            foreach (var temp_bus in temp_buses)
            {
                Bus bus = new Bus();
                bus.Distance = temp_bus.dist;
                bus.Time = temp_bus.time;
                bus.FootDistance = temp_bus.foot_dist;
                bus.LastFootDistance = temp_bus.last_foot_dist;

                ObservableCollection<Segment> list_segments = new ObservableCollection<Segment>();
                foreach (var segment in temp_bus.segments)
                {
                    Segment s = new Segment();
                    s.StartStat = segment.start_stat;
                    s.EndStat = segment.end_stat;
                    s.LineName = segment.line_name;
                    s.Stats = segment.stats.Split(';');
                    s.LineDistance = segment.line_dist;
                    s.FootDistance = segment.foot_dist;

                    list_segments.Add(s);
                }
                bus.Segments = list_segments;

                buses.Add(bus);
            }
            return buses;
        }

        public static ObservableCollection<Station> parseXMLForNearbyStat(string xmlFile)
        {
            XDocument xdoc = XDocument.Parse(xmlFile);

            var temp_stats = from temp_stat in xdoc.Descendants("stat")
                             select new
                             {
                                 name = temp_stat.Element("name").Value,
                                 xy = temp_stat.Element("xy").Value,
                                 dist = temp_stat.Element("dist").Value,
                                 line_names = temp_stat.Element("line_names").Value,
                             };
            ObservableCollection<Station> stats = new ObservableCollection<Station>();
            foreach (var temp_stat in temp_stats)
            {
                Station stat = new Station();
                stat.StationName = temp_stat.name;
                if (temp_stat.xy.Length > 0)
                {
                    string[] xy = temp_stat.xy.Split(',');
                    stat.Longitude = float.Parse(xy[0]);
                    stat.Latitude = float.Parse(xy[1]);
                }
                stat.Dist = temp_stat.dist;

                ObservableCollection<Line> lines = new ObservableCollection<Line>();
                foreach (string line_name in temp_stat.line_names.Split(';'))
                {
                    Line line = new Line();
                    int index = line_name.IndexOf('(');
                    line.LineName = line_name.Substring(0, index);
                    line.Info = line_name.Substring(index + 1, line_name.LastIndexOf(')') - index - 1);
                    lines.Add(line);
                }
                stat.Lines = lines;
                stats.Add(stat);
            }

            return stats;
        } 
    }
}
