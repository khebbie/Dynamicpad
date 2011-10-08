using System;

namespace DynamicPad
{
    public class Log
    {
        private readonly Action<string> _appender;
        private readonly Action _clearText;

        public Log(Action<string> appender, Action clearText)
        {
            _appender = appender;
            _clearText = clearText;
        }

        public void Print(string message)
        {
            _appender(message);
        }

        public void Clear()
        {
            _clearText();
        }
    }
}