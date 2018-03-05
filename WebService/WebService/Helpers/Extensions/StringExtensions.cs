using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebService.Models;

namespace WebService.Helpers.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerCamelCase(this string This)
        {
            switch (This.Length)
            {
                case 0:
                    return This;
                case 1:
                    return This.ToLower();
                default:
                    return $"{char.ToLower(This[0])}{This.Substring(1)}";
            }
        }

        public static bool EqualsWithCamelCasing(this string This, string propertyName)
            => This == propertyName ||
               This.ToLowerCamelCase() == propertyName.ToLowerCamelCase();

        // TODO abstract this method to all object instead of just residents
        public static IEnumerable<Expression<Func<Resident, object>>> ConvertToResidentPropertySelectors(
            this IEnumerable<string> This)
        {
            // create a new list of selectors
            var selectors = new List<Expression<Func<Resident, object>>>();

            // fill the list of selectors by iterating over the properties to update
            foreach (var propertyName in This)
            {
                // if the name of a properties matches a property of a Value, 
                // add the corresponding selector
                if (propertyName.EqualsWithCamelCasing(nameof(Resident.Birthday)))
                    selectors.Add(x => x.Birthday);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Colors)))
                    selectors.Add(x => x.Colors);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Doctor)))
                    selectors.Add(x => x.Doctor);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.FirstName)))
                    selectors.Add(x => x.FirstName);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Images)))
                    selectors.Add(x => x.Images);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.LastName)))
                    selectors.Add(x => x.LastName);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.LastRecordedPosition)))
                    selectors.Add(x => x.LastRecordedPosition);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Locations)))
                    selectors.Add(x => x.Locations);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Music)))
                    selectors.Add(x => x.Music);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Picture)))
                    selectors.Add(x => x.Picture);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Room)))
                    selectors.Add(x => x.Room);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Tags)))
                    selectors.Add(x => x.Tags);
                else if (propertyName.EqualsWithCamelCasing(nameof(Resident.Videos)))
                    selectors.Add(x => x.Videos);
            }

            return selectors;
        }
    }
}