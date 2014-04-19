using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Phone.Globalization;

namespace BusTest
{
    public class City
    {
        private string m_strCityName;
        private string m_provincName;

        public City()
        {

        }

        public City(string province, string cityName)
        {
            m_provincName = province;
            m_strCityName = cityName;
        }

        public string CityName
        {
            get { return m_strCityName; }
            set { m_strCityName = value; }
        }

        public string ProvinceOfCity
        {
            get { return m_provincName; }
            set { m_provincName = value; }
        }
    }

    public class ProvinceGroup<T> : List<T>
    {
        public delegate string GetKeyDelegate(T item);
        public string Key { get; private set; }

        public ProvinceGroup(string key)
        {
            Key = key;
        }

        private static List<ProvinceGroup<T>> CreateGroups(List<string> keys)
        {
            List<ProvinceGroup<T>> list = new List<ProvinceGroup<T>>();

            foreach (string key in keys)
            {
                list.Add(new ProvinceGroup<T>(key));
            }

            return list;
        }

        public static List<ProvinceGroup<T>> CreateGroups(IEnumerable<T> items, List<string> keys, GetKeyDelegate getKey)
        {
            List<ProvinceGroup<T>> list = CreateGroups(keys);

            foreach (T item in items)
            {
                int index = keys.IndexOf(getKey(item));
                if (index >= 0 && index < list.Count)
                {
                    list[index].Add(item);
                }
            }

            return list;
        }
    }
}
