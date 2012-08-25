using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SharpPDFStamper
{
    public static class Config
    {
        //CONFIG KEYS
        public static string TemplateFile { get { return ReadConfig("TemplateFile"); } }
        public static string TemplateExtension { get { return ReadConfig("TemplateExtension"); } }
        private static string ReadConfig(string Key)
        {
            string Value = ConfigurationManager.AppSettings[Key];
            return Value;
        }

    }

    
}

