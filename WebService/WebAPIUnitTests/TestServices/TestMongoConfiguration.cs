using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace WebAPIUnitTests.TestServices
{
    public class TestMongoConfiguration : IConfiguration
    {
        public TestMongoConfiguration(string mongoCollectionName)
        {
            Values = new Dictionary<string, string>
            {
                {"Database:ConnectionString", "mongodb://localhost:27017"},
                {"Database:DatabaseName", "toermalienTestDb"},
                {"Database:ReceiverModulesCollectionName", mongoCollectionName},
            };
        }

        private IDictionary<string, string> Values { get; }

        public IConfigurationSection GetSection(string key) => throw new NotImplementedException();

        public IEnumerable<IConfigurationSection> GetChildren() => throw new NotImplementedException();

        public IChangeToken GetReloadToken() => throw new NotImplementedException();

        public string this[string key]
        {
            get => Values[key];
            set => Values[key] = value;
        }
    }
}