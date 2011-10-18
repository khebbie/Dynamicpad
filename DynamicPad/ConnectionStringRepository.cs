using System;
using System.Collections.Generic;
using System.IO;
using DynamicPadTests;

namespace DynamicPad
{
    public class ConnectionStringRepository
    {
        private readonly CfgFileSerializer _cfgFileSerializer;
        Dictionary<string,string> _connectionStrings = new Dictionary<string, string>(); 

        public ConnectionStringRepository()
        {
            _cfgFileSerializer = new CfgFileSerializer();
            ReadFile();
        }

        private void ReadFile()
        {
            string filecontent = ReadFile(CfgFileUtility.GetPathToFile());
           _connectionStrings = _cfgFileSerializer.Serialize(filecontent);
        }

        public void Add(string connectionStringName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new ArgumentNullException("connectionStringName");
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            var cfgFile = new FileInfo(CfgFileUtility.GetPathToFile());
            if (!cfgFile.Exists)
                cfgFile.Create();
            _connectionStrings.Add(connectionStringName, connectionString);
            var textToWrite = _cfgFileSerializer.Deserialize(_connectionStrings);
            WriteFile(textToWrite);
        }

        private static void WriteFile(string textToWrite)
        {
            TextWriter tw = new StreamWriter(CfgFileUtility.GetPathToFile());

            tw.Write(textToWrite);
            tw.Close();
        }

        private string ReadFile(string fileName)
        {
            EnsureCfgFileExist(fileName);
            var streamReader = new StreamReader(fileName);
            var result = streamReader.ReadToEnd();
            streamReader.Close();
            return result;
        }

        private void EnsureCfgFileExist(string filename)
        {
            var cfgFile = new FileInfo(filename);
            if (!cfgFile.Exists)
                cfgFile.Create();
        }

        public string Get(string connectionStringName)
        {
            return _connectionStrings[connectionStringName];
        }
    }
}