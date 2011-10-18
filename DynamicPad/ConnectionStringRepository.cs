using System;
using System.IO;
using DynamicPadTests;

namespace DynamicPad
{
    public class ConnectionStringRepository
    {
        public void Add(string connectionStringName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new ArgumentNullException("connectionStringName");
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            var cfgFile = new FileInfo(CfgFileUtility.GetPathToFile());
            if (!cfgFile.Exists)
                cfgFile.Create();
        }
    }
}