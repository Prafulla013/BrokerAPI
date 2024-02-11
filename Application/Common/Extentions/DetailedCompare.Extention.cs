using System.Collections.Generic;
using System.Reflection;

namespace Application.Common.Extentions
{
    public static class DetailedCompareExtension
    {
        public static List<Variance> DetailedCompare<T>(this T object1, T object2)
        {
            List<Variance> variances = new List<Variance>();
            PropertyInfo[] fi = object1.GetType().GetProperties(BindingFlags.Public 
                                | BindingFlags.Instance 
                                | BindingFlags.NonPublic 
                                | BindingFlags.Static);
            foreach (PropertyInfo f in fi)
            {
                Variance v = new Variance();
                v.Field = f.Name;
                v.previouValue = f.GetValue(object1);
                v.currentValue = f.GetValue(object2);
                if (!Equals(v.previouValue, v.currentValue))
                    variances.Add(v);
            }
            return variances;
        }
    }
    public class Variance
    {
        public string Field { get; set; }
        public object previouValue { get; set; }
        public object currentValue { get; set; }
    }
}