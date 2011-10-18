using System;

namespace DynamicPad
{
    public class ConnectionStringRepository
    {
        public void Add(string connectionStringName, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new ArgumentNullException("connectionStringName");
        }
    }
}