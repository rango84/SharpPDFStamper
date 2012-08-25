using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SharpPDFStamper
{
    public class Config
    {
        //Leaving this for later

        public static string PDFDIR { get { return ReadConfig("PDFDIR"); } }
        public static string TemplatesDIR { get { return ReadConfig("TemplatesDIR"); } }
        private static string ReadConfig(string Key)
        {
            return ConfigurationManager.AppSettings[Key];
        }

    }

    
}

