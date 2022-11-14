using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Dlubal.RFEM5;
using Stabiliteettilaskenta.Utility;

namespace Stabiliteettilaskenta.RFem
{
    class RFemLoadings
    {
        public void XmlSerializeLoadCases(string folderPath, string modelName = "")
        {
            if (!RFemConnection.ConnectModel())
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();

            ILoads iLoads = iModel.GetLoads();
            LoadCase[] loadCases = iLoads.GetLoadCases();

            //foreach (LoadCase loadCase in loadCases)
            //{
            //    TxtFile.XmlSerialize("loadCases.xml", loadCase, typeof(LoadCase));
            //}

            //LoadCombination[] loadCombinations = iLoads.GetLoadCombinations();

            //foreach (LoadCombination loadCombination in loadCombinations)
            //{
            //    TxtFile.XmlSerialize("loadCombinations.xml", loadCombination, typeof(LoadCombination));
            //}

            //ResultCombination[] resultCombinations = iLoads.GetResultCombinations();

            //foreach (ResultCombination resultCombination in resultCombinations)
            //{
            //    TxtFile.XmlSerialize("resultCombinations.xml", resultCombination, typeof(ResultCombination));
            //}

            iModel.GetApplication().UnlockLicense();


            //XmlSerializer serializer = new XmlSerializer(typeof(LoadCase));
            //TextWriter writer = new StringWriter();
            //serializer.Serialize(writer, loadCases[0]);
            //string xml = writer.ToString();
            string xml = loadCases[0].XmlSerialize();
            //TextReader reader = new StringReader(xml);
            //LoadCase lc = (LoadCase)serializer.Deserialize(reader);
            LoadCase lc = TxtFile.XmlDeserialize<LoadCase>(xml);
        }

        public void XmlSerializeCrossSections(string folderPath, string modelName = "")
        {
            if (!RFemConnection.ConnectModel())
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();

            IModelData iModelData = iModel.GetModelData();
            Material[] materials = iModelData.GetMaterials();
            CrossSection[] crossSections = iModelData.GetCrossSections();

            foreach (Material material in materials)
            {
                TxtFile.XmlSerialize("materials.xml", material, typeof(Material));
            }

            foreach (CrossSection crossSection in crossSections)
            {
                TxtFile.XmlSerialize("crossSections.xml", crossSection, typeof(CrossSection));
            }

            iModel.GetApplication().UnlockLicense();
        }

        public void XmlDeserializeLoadingData()
        {
            LoadCase loadCase;

            XmlSerializer serializer = new XmlSerializer(typeof(LoadCase));
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            FileStream fs = new FileStream("Testi.xml", FileMode.Open);
            loadCase = (LoadCase)serializer.Deserialize(fs);
            fs.Close();
        }

        protected void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        protected void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
        }
    }
}
