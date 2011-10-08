using System.IO;

namespace DynamicPad
{
    public static class ObjectDumpExtensions
    {
        public static string Dump(this object element)
        {
            var memoryStream = new MemoryStream();
            TextWriter textWriter = new StreamWriter(memoryStream);
            ObjectDumper.Write(element, 3, textWriter);
            return textWriter.ToString();
        }
    }
}