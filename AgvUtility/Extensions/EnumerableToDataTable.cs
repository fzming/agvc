using System.Collections.Generic;
using System.Data;

namespace Utility.Extensions
{
    public static class EnumerableToDataTable
    {
        public static DataTable CreateDataTableForPropertiesOfType<T>()
        {
            var dt = new DataTable();
            var piT = typeof(T).GetProperties();

            foreach (var pi in piT)
            {

                var propertyType = pi.PropertyType.IsGenericType ? pi.PropertyType.GetGenericArguments()[0] : pi.PropertyType;
                var dc = new DataColumn(pi.Name, propertyType);

                if (pi.CanRead)
                {
                    dt.Columns.Add(dc);
                }
            }

            return dt;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            var table = CreateDataTableForPropertiesOfType<T>();
            var piT = typeof(T).GetProperties();

            foreach (var item in items)
            {
                var dr = table.NewRow();

                for (var property = 0; property < table.Columns.Count; property++)
                {
                    if (!piT[property].CanRead) continue;
                    try
                    {
                        dr[property] = piT[property].GetValue(item, null);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                table.Rows.Add(dr);
            }
            return table;
        }
    }
}