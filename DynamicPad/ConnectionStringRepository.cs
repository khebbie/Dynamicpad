using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
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
            string filecontent = InternalReadFile();
            _connectionStrings = _cfgFileSerializer.Serialize(filecontent);
        }

        public void Add(string connectionStringName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new ArgumentNullException("connectionStringName");
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            //var cfgFile = new FileInfo(CfgFileUtility.GetPathToFile());
            //if (!cfgFile.Exists)
            //    cfgFile.Create();
            _connectionStrings.Add(connectionStringName, connectionString);
            var textToWrite = _cfgFileSerializer.Deserialize(_connectionStrings);
            WriteFile(textToWrite);
        }

        private static void WriteFile(string textToWrite)
        {
            IsolatedStorageFile isoStore = GetIsolatedStorageFile();

            IsolatedStorageFileStream isoStream2 =
            new IsolatedStorageFileStream(CfgFileUtility.GetPathToFile(),
            FileMode.Create, isoStore);

            StreamWriter sw = new StreamWriter(isoStream2);
            sw.Write(textToWrite);
            sw.Close();
            isoStream2.Close();
        }
  
        private static IsolatedStorageFile GetIsolatedStorageFile()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
            return isoStore;
        }

        private string InternalReadFile()
        {
            IsolatedStorageFile isoStore = GetIsolatedStorageFile();
            var file = new IsolatedStorageFileStream("MyApp.preferences", FileMode.Open, isoStore);
            StreamReader reader = new StreamReader(CfgFileUtility.GetPathToFile());
            String prefs = reader.ReadToEnd();
            Console.WriteLine(prefs);
            file.Close();
            return prefs;
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