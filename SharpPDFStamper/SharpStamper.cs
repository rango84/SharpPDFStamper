using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using iTextSharp.text.pdf;

namespace SharpPDFStamper
{
    public class SharpStamper: IDisposable
    {
        private PdfStamper m_Stamper;
        private PdfReader m_Reader;
        private string m_PdfPath;
        private MemoryStream m_Stream;
        private AcroFields m_AcroFields;
        
        public SharpStamper(string Path) 
        {
            m_PdfPath = Path;
            Init();
        }
        private void Init() 
        {
            if(String.IsNullOrEmpty(m_PdfPath))
                throw new Exception("PDF File Path cannot be empty. Please select a File");

            if(!File.Exists(m_PdfPath))
                throw new Exception("The provided file path does not exist");

            m_Reader = new PdfReader(m_PdfPath);
            m_Stream = new MemoryStream();
            m_Stamper = new PdfStamper(m_Reader, m_Stream);
            m_Stamper.FormFlattening = true;
            m_Stamper.Writer.CloseStream = false;
            m_AcroFields = m_Reader.AcroFields;
        }

        public List<String> Fields 
        {
            get 
            {
                List<String> FieldEntries = new List<String>();
                foreach (KeyValuePair<string,AcroFields.Item> Entry  in m_AcroFields.Fields)
                {
                    FieldEntries.Add(Entry.Key.ToString());
                }
                return FieldEntries;
            }
        }

        public void Stamp(Dictionary<string, string> Items) 
        {
            foreach (KeyValuePair<string, string> Item in Items) 
            {
                m_Stamper.AcroFields.SetField(Item.Key, Item.Value);
            }
            m_Stamper.Close();
        }

        public MemoryStream Output 
        {
            get
            {
                return m_Stream;
            }
        }

   
        public void  Dispose()
        {
            if (m_Stream != null)
                m_Stream.Close();
        }

        public string FileName 
        {
            get 
            {
                if (String.IsNullOrEmpty(m_PdfPath))
                    return String.Empty;
                return Path.GetFileName(m_PdfPath);
            }
        }

        public string Extension 
        {
            get 
            {
                if (String.IsNullOrEmpty(m_PdfPath))
                    return String.Empty;
                return Path.GetExtension(m_PdfPath);
            }
        }
    }

}
