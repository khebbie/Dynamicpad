using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Massive;
using Microsoft.CSharp.RuntimeBinder;

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
            _appender("\n");
        }

        public void Clear()
        {
            _clearText();
        }

        public void Dump(ExpandoObject obj)
        {
            foreach (var val in obj)
            {
                Print(val.Key + ": " +  val.Value);
            }
        }

        public void DumpEnumerable(IEnumerable<dynamic> values)
        {
            foreach (var value in values)
            {
                Dump(value);
            }
        }
        public void DumpModel(DynamicModel model)
        {
            Dump(model);
        }

        public void Dump(DynamicObject obj)
        {
            foreach (var memberName in obj.GetDynamicMemberNames())
            {
                _appender(GetDynamicMember(obj, memberName).ToString());
                Print("\n");
            }
        }

        static object GetDynamicMember(object obj, string memberName)
        {
            var binder = Binder.GetMember(CSharpBinderFlags.None, memberName, obj.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, obj);
        }
    }
}