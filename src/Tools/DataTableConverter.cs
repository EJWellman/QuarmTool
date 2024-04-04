using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EQTool.Tools
{
	public static class DataTableConverter
	{
		public static DataTable ToDataTable<T>(this IList<T> data)
		{
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
			DataTable dt = new DataTable();

			for (int i = 0; i < props.Count; i++)
			{
				PropertyDescriptor prop = props[i];
				dt.Columns.Add(prop.Name, prop.PropertyType);
			}

			object[] values = new object[props.Count];

			foreach (T obj in data)
			{
				for (int i = 0; i < values.Length; i++)
				{
					values[i] = props[i].GetValue(obj);
				}
				dt.Rows.Add(values);
			}
			return dt;
		}

		public static void SetItemFromRow<T>(T item, DataRow row)
			where T : new()
		{
			// go through each column
			foreach (DataColumn c in row.Table.Columns)
			{
				// find the property for the column
				PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

				// if exists, set the value
				if (p != null && row[c] != DBNull.Value)
				{
					p.SetValue(item, row[c], null);
				}
			}
		}

		// function that creates an object from the given data row
		public static T CreateItemFromRow<T>(DataRow row)
			where T : new()
		{
			// create a new object
			T item = new T();

			// set the item
			SetItemFromRow(item, row);

			// return 
			return item;
		}
	}
}
