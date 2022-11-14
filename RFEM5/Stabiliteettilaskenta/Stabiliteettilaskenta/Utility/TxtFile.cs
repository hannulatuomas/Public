using System;
using System.Collections.Generic;
using Scripting;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Stabiliteettilaskenta.Utility
{
    static class TxtFile
    {
        public static string RootFolder
        {
            get
            {
                string baseFolder = AppContext.BaseDirectory;
                int binIndex = baseFolder.IndexOf(@"\bin");
                return baseFolder.Substring(0, binIndex);
            }
        }

        static void CreateTextFile(string _folder, string _name)
        {
            FileSystemObject FSO = new FileSystemObject();
            string path = _folder + @"\" + _name + ".txt";

            TextStream fileStream;

            if (FSO.FileExists(path))
            {
                MessageBox.Show("File already exists!");
                return;
            }
            fileStream = FSO.CreateTextFile(path);
            fileStream.Close();
        }

        public static void WriteToTextFile(string _folder, string _name, string[] _textArr)
        {
            int lines = _textArr.Length;
            bool newFile = false;

            FileSystemObject FSO = new FileSystemObject();
            string path = _folder + @"\" + _name + ".txt";

            TextStream fileStream;

            if (!FSO.FileExists(path))
            {
                DialogResult result = MessageBox.Show("File doesn't exist. Do you want to make it?", "File doesn't exist", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    CreateTextFile(_folder, _name);
                    newFile = true;
                }
                else
                {
                    return;
                }
            }

            if (newFile == false)
            {
                DialogResult result = MessageBox.Show("File exists. Do you want to overwrite it?", "File exists", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    fileStream = FSO.OpenTextFile(path, IOMode.ForWriting);
                }
                else if (result == DialogResult.No)
                {
                    fileStream = FSO.OpenTextFile(path, IOMode.ForAppending);
                }
                else
                {
                    return;
                }
            }
            else
            {
                fileStream = FSO.OpenTextFile(path, IOMode.ForAppending);
            }

            for (int i = 0; i < lines; i++)
            {
                fileStream.WriteLine(_textArr[i]);
            }

            fileStream.Close();
            MessageBox.Show("Text file made: " + path, "File made");
        }

        public static List<string> ReadFromTextFile(string _folder, string _name)
        {
            List<string> text = new List<string>();

            FileSystemObject FSO = new FileSystemObject();
            //string path = _folder + @"\" + _name + ".txt";
            string path = _folder + @"\" + _name;

            TextStream fileStream;

            if (FSO.FileExists(path) == false)
            {
                MessageBox.Show("Could not read text file. File: " + path + " doesn't exist!", "File doesn't exist");
                return null;
            }

            fileStream = FSO.OpenTextFile(path, IOMode.ForReading);

            int i = 0;

            while (!fileStream.AtEndOfStream)
            {
                text.Add(fileStream.ReadLine());
                i++;
            }

            fileStream.Close();
            MessageBox.Show("Found " + i.ToString() + " lines.", "Reading completed");

            return text;
        }

        public static void XmlSerialize(string fileName, object item, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            TextWriter writer = new StreamWriter(fileName, true);

            serializer.Serialize(writer, item);

            writer.Close();
        }

        public static string XmlSerialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                TextWriter stringWriter = new StringWriter();
                using (XmlWriter writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    string s = stringWriter.ToString();
                    writer.Close();
                    stringWriter.Close();
                    return s;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static T XmlDeserialize<T>(string xml)
        {
            try
            {
                T value;

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                TextReader reader = new StringReader(xml);
                value = (T)serializer.Deserialize(reader);
                reader.Close();

                return (T)value;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        public static string GetMappedString(string strSource, string oldString, string newString)
        {
            return strSource.Replace(oldString, newString);
        }

        public static string[] Alphabets()
        {
            string[] alphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            return alphabets;
        }
    }
}
