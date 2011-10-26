using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace DynamicPad
{
    //inspired by http://www.codeproject.com/KB/dotnet/IsolatedStorage.aspx
    public class IsolatedStorageSettings
    {
        private readonly IsolatedStorageFile _isoStore;
        private readonly string _isolatedFileName;

        public IsolatedStorageSettings(string isolatedFileName)
        {
            _isolatedFileName = isolatedFileName;
            _isoStore = GetIsoStoreFile();
        }

        public  string Read()
        {
            if (!DoesFileExistInIsolatedStorage())
                return string.Empty;
            var iStream =
                new IsolatedStorageFileStream(_isolatedFileName,
                                              FileMode.Open, _isoStore);

            var reader = new StreamReader(iStream);
            var sb = new StringBuilder();
            
            String line;

            while ((line = reader.ReadLine()) != null)
            {
                sb.Append(line);
            }

            reader.Close();

            return sb.ToString();
        }

        public void Write(string content)
        {
            var oStream =
                new IsolatedStorageFileStream(_isolatedFileName,
                                              FileMode.Create, _isoStore);

            var writer = new StreamWriter(oStream);
            writer.WriteLine(content);
            writer.Close();
        }

        private static IsolatedStorageFile GetIsoStoreFile()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User
                                                | IsolatedStorageScope.Assembly, null, null);
        }

        private  bool DoesFileExistInIsolatedStorage()
        {
            string[] fileNames = _isoStore.GetFileNames(_isolatedFileName);

            return fileNames.Any(file => file == _isolatedFileName);
        }
    }
}