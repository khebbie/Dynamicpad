using System;
using System.Collections.Generic;
using System.Diagnostics;
using DynamicPadTests;

namespace DynamicPad
{
    public class ConnectionStringRepository
    {
        private readonly CfgFileSerializer _cfgFileSerializer;
        Dictionary<string,string> _connectionStrings = new Dictionary<string, string>();
        private readonly IsolatedStorageSettings _isolatedStorageSettings;
        private readonly string _isolatedFileName;

        public ConnectionStringRepository()
        {
            _isolatedFileName = "dynamicPad.config";
            _isolatedStorageSettings = new IsolatedStorageSettings(_isolatedFileName);
            _cfgFileSerializer = new CfgFileSerializer();
            ReadFile();
        }

        private void ReadFile()
        {
            string filecontent = _isolatedStorageSettings.Read();
            _connectionStrings = _cfgFileSerializer.Serialize(filecontent);
        }

        public void Clear()
        {
            _connectionStrings = new Dictionary<string, string>();
        }

        public void Add(string connectionStringName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new ArgumentNullException("connectionStringName");
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            _connectionStrings.Add(connectionStringName, connectionString);
            var textToWrite = _cfgFileSerializer.Deserialize(_connectionStrings);
            _isolatedStorageSettings.Write(textToWrite);
        }
        
        public string Get(string connectionStringName)
        {
            return _connectionStrings[connectionStringName];
        }

        public Dictionary<string, string> All()
        {
            return _connectionStrings;
        }
    }
}