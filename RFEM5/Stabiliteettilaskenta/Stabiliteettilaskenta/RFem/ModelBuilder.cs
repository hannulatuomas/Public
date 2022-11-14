using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Stabiliteettilaskenta.Utility;

namespace Stabiliteettilaskenta.RFem
{
    public class ModelBuilder
    {
        public void WriteProfiles(string modelName = "")
        {
            //Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            //excelApp.Visible = true;
            //Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add();
            //Microsoft.Office.Interop.Excel.Worksheet worksheet = excelApp.ActiveSheet;

            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();
            //CrossSection[] crossSections = iModelData.GetCrossSections();
            Material[] materials = iModelData.GetMaterials();
            iModel.GetApplication().UnlockLicense();
            int row = 1;
            
            string subFolder = TxtFile.RootFolder + @"\Resources";
            //string fileName = "CrossSectionLibrary";
            string fileName = "MaterialLibrary";
            List<string> materialStrings = new List<string>();
            foreach (Material material in materials)
            {
                //worksheet.Cells[row, "A"] = crossSection.Description;
                //TxtFile.WriteToTextFile();
                //string profile = "Rolled I: {" + "RfProfile : (" + crossSection.Description + "), TeklaProfile : [" + "Unknown" + "]}";
                string mat = "Steel : {" + "RfMaterial : (" + material.Description + "), TeklaMaterial : [" + "Unknown" + "]}";
                materialStrings.Add(mat);
                row++;
            }

            //TxtFile.WriteToTextFile(subFolder, fileName, materialStrings.ToArray());
        }
        public void AddMaterial(string modelName = "")
        {
            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();
            //Material[] materials = iModelData.GetMaterials();
            //CrossSection[] crossSections = iModelData.GetCrossSections();

            //iModel.GetApplication().UnlockLicense();

            //string[] dataStrings = new string[materials.Length + crossSections.Length];
            //int i = 0;

            //foreach (Material material in materials)
            //{
            //    dataStrings[i] = material.XmlSerialize<Material>();
            //    i++;
            //}
            //foreach (CrossSection crossSection in crossSections)
            //{
            //    dataStrings[i] = crossSection.XmlSerialize<CrossSection>();
            //    i++;
            //}

            //TxtFile.WriteToTextFile(@"C:\Users\TuHan\OneDrive - A-Insinöörit Oy\Työpöytä", "TestiData.xml", dataStrings);

            //string materialStringSteel = "NameID|Baustahl S 355@TypeID|STEEL@NormID|SFS EN 1993-1-1";
            //string materialStringConcrete = "NameID|Beton C30/37@TypeID|CONCRETE@NormID|EN 1992-1-1";

            //string profileStringRect = "Rechteck 680/680";
            //string profileStringTube = "QRO 80x6 (EN 10219-2, 2006)";
            //string profileStringBox = "Kasten(A) 500/20/16/400/800/500/20/12/12";

            List<string> text = TxtFile.ReadFromTextFile(@"C:\Users\TuHan\OneDrive - A-Insinöörit Oy\Työpöytä", "TestiData.xml");
            string xml1 = text[0];
            string xml2 = text[1];

            string xml3 = text[2];
            string xml4 = text[3];
            string xml5 = text[4];
            string xml6 = text[5];

            Material mat = TxtFile.XmlDeserialize< Material>(xml1);
            //Material mat1 = mat;
            Material mat1 = new Material();
            //mat1.Description = mat.Description;
            mat1.Description = "Concrete C30/37";
            //mat1.ModelType = mat.ModelType;
            //mat1.PoissonRatio = mat.PoissonRatio;
            //mat1.SpecificWeight = mat.SpecificWeight;
            //mat1.ElasticityModulus = mat.ElasticityModulus;
            //mat1.PartialSafetyFactor = mat.PartialSafetyFactor;
            //mat1.ShearModulus = mat.ShearModulus;
            //mat1.ThermalExpansion = mat.ThermalExpansion;
            //mat1.No = 1;

            mat = TxtFile.XmlDeserialize<Material>(xml2);
            //Material mat2 = mat;
            Material mat2 = new Material();
            mat2.Description = "Steel S 355";
            //mat2.ModelType = mat.ModelType;
            //mat2.PoissonRatio = mat.PoissonRatio;
            //mat2.SpecificWeight = mat.SpecificWeight;
            //mat2.ElasticityModulus = mat.ElasticityModulus;
            //mat2.PartialSafetyFactor = mat.PartialSafetyFactor;
            //mat2.ShearModulus = mat.ShearModulus;
            //mat2.ThermalExpansion = mat.ThermalExpansion;
            //mat2.No = 2;

            Material[] materials = new Material[] { mat1, mat2 };

            iModelData.PrepareModification();
            foreach (Material material in materials)
            {
                iModelData.SetMaterial(material);
            }
            iModelData.FinishModification();

            //CrossSection cs1 = TxtFile.XmlDeserialize<CrossSection>(xml3);
            //CrossSection cs2 = TxtFile.XmlDeserialize<CrossSection>(xml4);
            //CrossSection cs3 = TxtFile.XmlDeserialize<CrossSection>(xml5);
            //CrossSection cs4 = TxtFile.XmlDeserialize<CrossSection>(xml6);
            //CrossSection cs1 = new CrossSection();
            //cs1.TextID = profileStringRect;
            //cs1.MaterialNo = 2;
            //CrossSection cs2 = new CrossSection();
            //cs2.TextID = profileStringTube;
            //cs2.MaterialNo = 1;
            //CrossSection cs3 = new CrossSection();
            //cs3.TextID = profileStringBox;
            //cs3.MaterialNo = 1;

            //CrossSection[] crossSections = new CrossSection[] { cs1, cs2, cs3, cs4 };

            //iModelData.PrepareModification();
            //foreach (CrossSection crossSection in crossSections)
            //{
            //    iModelData.SetCrossSection(crossSection);
            //}
            //iModelData.FinishModification();

            iModel.GetApplication().UnlockLicense();

            return;
        }
        public void AddMember(Vector3 startPoint, Vector3 endPoint, string modelName = "", int _crossSection = 1, int _startHingeNo = 0, int _endHingeNo = 1)
        {
            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;
            int startNode = AddNode(iModel, startPoint);
            int endNode = AddNode(iModel, endPoint);
            if (startNode != endNode)
            {
                int line = AddLine(iModel, startNode, endNode);
                int member = AddMember(iModel, line, _crossSection, _startHingeNo, _endHingeNo);
            }
        }

        public Member[] GetMembers(string modelName = "")
        {
            if (!RFemConnection.ConnectModel(modelName))
            {
                return null;
            }

            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();
            Member[] members =  iModelData.GetMembers();

            iModel.GetApplication().UnlockLicense();

            return members;
        }

        public List<Vector3[]> GetMemberCoordinates(Member[] members, string modelName = "")
        {
            List<Vector3[]> coordinates = new List<Vector3[]>();

            if (!RFemConnection.ConnectModel(modelName))
            {
                return coordinates;
            }

            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            foreach (Member member in members)
            {
                int lineNumber = member.LineNo;
                ILine iLine = iModelData.GetLine(lineNumber, ItemAt.AtNo);
                Dlubal.RFEM5.Line line = iLine.GetData();
                int pointCount = line.ControlPoints.Length - 1;
                Vector3 startPoint = new Vector3((float)line.ControlPoints[0].X * 1000, (float)line.ControlPoints[0].Y * 1000, (float)line.ControlPoints[0].Z * 1000);
                Vector3 endPoint = new Vector3((float)line.ControlPoints[pointCount].X * 1000, (float)line.ControlPoints[pointCount].Y * 1000, (float)line.ControlPoints[pointCount].Z * 1000);

                coordinates.Add(new Vector3[] { startPoint, endPoint });
            }

            iModel.GetApplication().UnlockLicense();

            return coordinates;
        }

        #region Add ModelData

        int AddNode(IModel _iModel, Vector3 _point)
        {
            IModel iModel = _iModel;
            Node node = new Node();
            node.X = _point.X;
            node.Y = _point.Y;
            node.Z = _point.Z;

            iModel.GetApplication().LockLicense();
            IModelData iModelData = _iModel.GetModelData();

            Node[] nodes = iModelData.GetNodes();
            int i = 0;
            int n = 1;

            while (i < nodes.Length)
            {
                if (nodes[i].No != n)
                {
                    break;
                }
                n++;
                i++;
            }
            node.No = n;

            iModelData.PrepareModification();
            iModelData.SetNode(node);
            iModelData.FinishModification();
            int no = iModelData.GetLastObjectNo(ModelObjectType.NodeObject);

            if (n != no)
            {
                for (int j = 0; j < nodes.Length; j++)
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (nodes[j].X == _point.X)
                    {
                        x = true;
                    }
                    if (nodes[j].Y == _point.Y)
                    {
                        y = true;
                    }
                    if (nodes[j].Z == _point.Z)
                    {
                        z = true;
                    }
                    if (x && y && z)
                    {
                        no = nodes[j].No;
                        break;
                    }
                }
            }

            _iModel.GetApplication().UnlockLicense();

            return no;
        }

        int AddLine(IModel _iModel, int _startNode, int _endNode)
        {
            bool checkZeroLength = true;
            float tolerance = 0.005f;

            string nodes = _startNode.ToString() + "," + _endNode.ToString();
            Dlubal.RFEM5.Line line = new Dlubal.RFEM5.Line();
            line.NodeList = nodes;

            _iModel.GetApplication().LockLicense();
            IModelData iModelData = _iModel.GetModelData();

            Dlubal.RFEM5.Line[] lines = iModelData.GetLines();
            int i = 0;
            int n = 1;

            while (i < lines.Length)
            {
                if (lines[i].No != n)
                {
                    break;
                }
                n++;
                i++;
            }
            line.No = n;

            iModelData.PrepareModification();
            iModelData.SetLine(line);
            iModelData.FinishModification();
            int no = iModelData.GetLastObjectNo(ModelObjectType.LineObject);

            if (n != no)
            {
                for (int j = 0; j < lines.Length; j++)
                {
                    string nodeList = lines[j].NodeList;
                    string[] subs = nodeList.Split(',');
                    bool sNode = false;
                    bool eNode = false;

                    foreach (string s in subs)
                    {
                        if (int.Parse(s) == _startNode)
                        {
                            sNode = true;
                        }
                        else if (int.Parse(s) == _endNode)
                        {
                            eNode = true;
                        }
                        if (sNode && eNode)
                        {
                            no = lines[j].No;
                            break;
                        }
                    }
                    if (sNode && eNode)
                    {
                        break;
                    }
                    if (checkZeroLength && lines[j].Length < tolerance)
                    {
                        iModelData.PrepareModification();
                        iModelData.DeleteObjects(ModelObjectType.LineObject, lines[j].No.ToString());
                        iModelData.FinishModification();
                    }
                }
            }

            _iModel.GetApplication().UnlockLicense();

            return no;
        }

        int AddMember(IModel _iModel, int _line, int _crossSection = 1, int _startHingeNo = 0, int _endHingeNo = 1)
        {
            Dlubal.RFEM5.Member member = new Dlubal.RFEM5.Member();
            member.LineNo = _line;
            member.StartCrossSectionNo = _crossSection;
            if (_startHingeNo > 0)
            {
                member.StartHingeNo = _startHingeNo;
            }
            if (_endHingeNo > 0)
            {
                member.EndHingeNo = _endHingeNo;
            }

            _iModel.GetApplication().LockLicense();
            IModelData iModelData = _iModel.GetModelData();

            Dlubal.RFEM5.Member[] members = iModelData.GetMembers();
            int i = 0;
            int n = 1;

            while (i < members.Length)
            {
                if (members[i].No != n)
                {
                    break;
                }
                n++;
                i++;
            }
            member.No = n;


            iModelData.PrepareModification();
            iModelData.SetMember(member);
            iModelData.FinishModification();

            int no = iModelData.GetLastObjectNo(ModelObjectType.MemberObject);

            if (n != no)
            {
                for (int j = 0; j < members.Length; j++)
                {
                    if (members[j].LineNo == _line)
                    {
                        no = members[j].LineNo;
                        break;
                    }
                }

                SetOfMembers[] sets = iModelData.GetSetsOfMembers();
                bool setExists = false;

                for (int j = 0; j < sets.Length; j++)
                {
                    if (sets[j].MemberList == no.ToString())
                    {
                        setExists = true;
                        break;
                    }
                }
                if (!setExists)
                {
                    SetOfMembers memberSet = new SetOfMembers();
                    memberSet.MemberList = no.ToString();
                    memberSet.Type = SetOfMembersType.ContinuousMembers;

                    iModelData.PrepareModification();
                    iModelData.SetSetOfMembers(memberSet);
                    iModelData.FinishModification();
                }
            }
            else
            {
                SetOfMembers memberSet = new SetOfMembers();
                memberSet.MemberList = no.ToString();
                memberSet.Type = SetOfMembersType.ContinuousMembers;

                iModelData.PrepareModification();
                iModelData.SetSetOfMembers(memberSet);
                iModelData.FinishModification();
            }

            _iModel.GetApplication().UnlockLicense();

            return no;
        }

        #endregion

        #region LoadData

        public void AddLoadCase(string modelName = "")
        {
            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }

            LoadCase loadCase = new LoadCase();
            LoadCombination loadCombination = new LoadCombination();
            ResultCombination resultCombination = new ResultCombination();

            MemberLoad memberLoad = new MemberLoad();
            
            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();
            
            ILoads iLoads = iModel.GetLoads();
            iLoads.PrepareModification();
            ILoadCase iLoadCase = iLoads.SetLoadCase(loadCase);
            ///
            // Add Loads to ILoadCase
            IMemberLoad iMemberLoad = iLoadCase.SetMemberLoad(memberLoad);
            ///
            ILoadCombination iLoadCombination = iLoads.SetLoadCombination(loadCombination);
            AnalysisParameters parameters = iLoadCombination.GetAnalysisParameters();
            IResultCombination iResultCombination = iLoads.SetResultCombination(resultCombination);
            iLoads.FinishModification();
            
            iModel.GetApplication().UnlockLicense();
        }

        #endregion
    }
}
