﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DynamicPad.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=udv-db;Initial Catalog=Integration_NextRelease_Mercury_Listing;User I" +
            "D=MercuryApp;Password=MercuryApp22;Transaction Binding=Explicit Unbind;;Multiple" +
            "ActiveResultSets=True")]
        public string ConnectionStringListing {
            get {
                return ((string)(this["ConnectionStringListing"]));
            }
            set {
                this["ConnectionStringListing"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=.\\SQLExpress;Integrated Security=true; ;initial catalog=MassiveTest;")]
        public string ConnectionStringPerson {
            get {
                return ((string)(this["ConnectionStringPerson"]));
            }
            set {
                this["ConnectionStringPerson"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=udv-db;Initial Catalog=Integration_NextRelease_Mercury_User\r\n;User ID" +
            "=MercuryApp;Password=MercuryApp22;Transaction Binding=Explicit Unbind;;MultipleA" +
            "ctiveResultSets=True\r\n")]
        public string ConnectionStringUser {
            get {
                return ((string)(this["ConnectionStringUser"]));
            }
            set {
                this["ConnectionStringUser"] = value;
            }
        }
    }
}
