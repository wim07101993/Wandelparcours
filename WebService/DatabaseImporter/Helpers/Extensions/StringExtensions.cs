using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DatabaseImporter.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static T DeserializeJson<T>(this string This)
            => JsonConvert.DeserializeObject<T>(This);

        public static IEnumerable<T> DeserializeCsv<T>(this string This, char delimiter = ',',
            bool withFieldsRow = true)
        {
            var rows = This.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);

            if (withFieldsRow && rows.Length < 2)
                return null;

            var properties = typeof(T)
                .GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            var csvRows = rows.Select(x => x.Split(delimiter)).ToList();
            var values = rows.Select(x => Activator.CreateInstance<T>()).ToList();

            if (withFieldsRow)
            {
                var propertyDictionary = properties.ToDictionary(x => x.Name, x => x);
                var fields = csvRows.First();
                for (var fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
                {
                    var property = propertyDictionary[fields[fieldIndex]];
                    for (var rowIndex = 1; rowIndex < rows.Length; rowIndex++)
                    {
                        var strValue = csvRows[rowIndex][fieldIndex];
                        var value = JsonConvert.DeserializeObject(strValue, property.PropertyType);
                        property.SetValue(values[rowIndex], value);
                    }
                }

                values.Remove(values.First());
            }
            else
            {
                for (var fieldIndex = 0; fieldIndex < csvRows[0].Length; fieldIndex++)
                {
                    PropertyInfo property = null;
                    foreach (var prop in properties)
                    {
                        try
                        {
                            var value = JsonConvert
                                .DeserializeObject(csvRows[0][fieldIndex], prop.PropertyType);
                            property = prop;

                            property.SetValue(values[0], value);
                        }
                        catch
                        {
                            // IGNORED
                        }
                    }

                    if (property == null)
                        continue;

                    for (var rowIndex = 1; rowIndex < values.Count; rowIndex++)
                    {
                        var value = JsonConvert.DeserializeObject(csvRows[rowIndex][fieldIndex], property.PropertyType);
                        property.SetValue(values[rowIndex], value);
                    }
                }
            }

            return values;
        }
    }
}