using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPad
{
    public class CfgFileSerializer
    {
        public Dictionary<string, string> Serialize(string fileContent)
        {
            var result = new Dictionary<string, string>();
            if(string.IsNullOrWhiteSpace(fileContent))
                return result;
            var lines = fileContent.Split('\n');
            foreach (var line in lines)
            {
                if(string.IsNullOrWhiteSpace(line))
                    continue;
                if (!line.Contains(","))
                    throw new ArgumentException(@"Each line should contain a comma which seperates a connectionstring from its name","fileContent");
                var strings = line.Split(',');
                result.Add(strings[0].Trim(), strings[1].Trim());
            }
            return result;
        }

        public string Deserialize(Dictionary<string, string> cfg)
        {
            var result =  new StringBuilder();
            foreach (var cfgLine in cfg)
            {
                var line = string.Format("{0}, {1}", cfgLine.Key, cfgLine.Value);
                result.Append(line);
                result.Append("\n");
            }
            return result.ToString();
        }
    }
}