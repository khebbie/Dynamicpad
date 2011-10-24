using System;
using System.IO;

namespace DynamicPadTests
{
    public class CfgFileUtility
    {
        public static string GetPathToFile()
        {
            return "DynamicPad\\ConnectionStrings.cfg";
        }

        public static string GetCfgDirectory()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var pathToDirectory = Path.Combine(folderPath, "DynamicPath");
            return pathToDirectory;
        }
    }
}