using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;

namespace SharpPDFStamper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string m_File;
        private Dictionary<string, string> m_TemplateFile;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.Filter = "(.pdf)|*.pdf";
            bool? result = Dlg.ShowDialog();
            if (result == true) 
            {
                m_File = Dlg.FileName;
                txtFile.Text = m_File;
                RunStamper();
            }
        }
        private void RunStamper() 
        {
            try
            {
                SharpStamper SharpStamper = new SharpStamper(m_File);
                lstResult.ItemsSource = SharpStamper.Fields;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCreateTemplate_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                StringBuilder Sb = new StringBuilder();
                string NewFile = String.Empty;
                using (SharpStamper Stamper = new SharpStamper(m_File))
                {
                    foreach (String field in Stamper.Fields)
                    {
                        Sb.AppendLine(String.Format("{0}=", field));
                    }
                    NewFile = String.Format("{0}-{1}.ptmpl", Stamper.FileName, DateTime.Now.Ticks);
                    using (StreamWriter sw = new StreamWriter(NewFile))
                    {
                        sw.Write(Sb.ToString());
                    }
                }
                MessageBox.Show("Template Created", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start("notepad.exe", NewFile);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.Filter = "(.ptmpl)|*.ptmpl";
            bool? result = Dlg.ShowDialog();

            try
            {
                if (result == true)
                {
                    using (StreamReader Sr = new StreamReader(Dlg.SafeFileName))
                    {
                        Dictionary<string, string> Items = new Dictionary<string, string>();
                        ReadTemplate(Sr, Items);
                        m_TemplateFile = Items;
                    }
                    StampTemplate();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ReadTemplate(StreamReader Stream, Dictionary<string,string> Items)
        {
            string Line;
            while(!String.IsNullOrEmpty(Line = Stream.ReadLine()))
            {
                string[] Parts = Line.Split('=');
                if (Parts.Length != 2)
                    throw new Exception("Invalid template format");
                Items.Add(Parts[0], Parts[1]);
            }
        }

        private void StampTemplate() 
        {
            string File = String.Empty;
            SharpStamper Stamper = null;
            using (Stamper = new SharpStamper(m_File))
            {
                File = String.Format("{0}_{1}_STAMPED.pdf", Stamper.FileName, DateTime.Now.Ticks);
                Stamper.Stamp(m_TemplateFile);
             
            }
            using (FileStream Fs = new FileStream(File, FileMode.Create))
            {
                byte[] byteStream = Stamper.Output.ToArray();
                Fs.Write(byteStream, 0, byteStream.Length);
            }
            Process.Start(File);
        }
    }
}
