using Dlubal.RFEM5;
using GH_GetRFemResults.GH;
using GH_GetRFemResults.Utilities;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_GetRFemResults.RFem
{
    public static class GetSupportReactions
    {
        public static bool localCS = true;
        public static List<int> GetSupportNodes(IModel iModel, string nodeFilter = "")
        {
            IModelData iModelData = iModel.GetModelData();

            List<NodalSupport> rfSupports = iModelData.GetNodalSupports().ToList();
            List<int> supportNodes = new List<int>();
            HashSet<int> filterNodes = new HashSet<int>();

            if (nodeFilter != "")
            {
                string[] subList = nodeFilter.Split(',');

                foreach (string nodeNo in subList)
                {
                    if (nodeNo.Contains("-"))
                    {
                        string[] boundaries = nodeNo.Split('-');
                        int start = int.Parse(boundaries[0]);
                        int end = int.Parse(boundaries[1]);

                        int i = start;
                        while (i <= end)
                        {
                            filterNodes.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterNodes.Add(int.Parse(nodeNo));
                    }
                }
            }

            foreach (NodalSupport support in rfSupports)
            {
                string nodeList = support.NodeList;                
                string[] subList = nodeList.Split(',');

                foreach (string nodeNo in subList)
                {
                    if (nodeNo.Contains("-"))
                    {
                        string[] boundaries = nodeNo.Split('-');
                        int start = int.Parse(boundaries[0]);
                        int end = int.Parse(boundaries[1]);

                        int i = start;
                        while (i <= end)
                        {
                            if (nodeFilter == "" || filterNodes.Contains(i))
                            {
                                supportNodes.Add(i);
                            }

                            i++;
                        }
                    }
                    else
                    {
                        int no = int.Parse(nodeNo);
                        if (nodeFilter == "" || filterNodes.Contains(no))
                        {
                            supportNodes.Add(no);
                        }
                        //supportNodes.Add(int.Parse(nodeNo));
                    }
                }
            }
            return supportNodes;
        }

        public static List<int> GetSupportLines(IModel iModel, string lineFilter = "")
        {
            IModelData iModelData = iModel.GetModelData();

            List<LineSupport> rfSupports = iModelData.GetLineSupports().ToList();
            List<int> supportLines = new List<int>();
            HashSet<int> filterLines = new HashSet<int>();

            if (lineFilter != "")
            {
                string[] subList = lineFilter.Split(',');

                foreach (string nodeNo in subList)
                {
                    if (nodeNo.Contains("-"))
                    {
                        string[] boundaries = nodeNo.Split('-');
                        int start = int.Parse(boundaries[0]);
                        int end = int.Parse(boundaries[1]);

                        int i = start;
                        while (i <= end)
                        {
                            filterLines.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterLines.Add(int.Parse(nodeNo));
                    }
                }
            }

            foreach (LineSupport support in rfSupports)
            {
                string lineList = support.LineList;
                string[] subList = lineList.Split(',');

                foreach (string lineNo in subList)
                {
                    if (lineNo.Contains("-"))
                    {
                        string[] boundaries = lineNo.Split('-');
                        int start = int.Parse(boundaries[0]);
                        int end = int.Parse(boundaries[1]);

                        int i = start;
                        while (i <= end)
                        {
                            if (lineFilter == "" || filterLines.Contains(i))
                            {
                                supportLines.Add(i);
                            }
                            i++;
                        }
                    }
                    else
                    {
                        int no = int.Parse(lineNo);
                        if (lineFilter == "" || filterLines.Contains(no))
                        {
                            supportLines.Add(no);
                        }
                        //supportLines.Add(int.Parse(lineNo));
                    }
                }
            }
            return supportLines;
        }

        public static DataTree<Vector3d> GetNodalSupportForces(IModel iModel, IResults iResults, int loadingIndex, string nodeFilter = "")
        {
            iModel.GetApplication().LockLicense();
            
            // Save force vectors into a tree;
            DataTree<Vector3d> oReactions = new DataTree<Vector3d>();

            List<int> supportNodes = GetSupportNodes(iModel, nodeFilter);

            foreach (int supportNode in supportNodes)
            {
                NodalSupportForces[] nodalSupportForces = iResults.GetNodalSupportForces(supportNode, ItemAt.AtNo, localCS);
                ResultsValueType valueType = nodalSupportForces[0].Type; // Get force types to avoid duplicate control points

                foreach (NodalSupportForces result in nodalSupportForces)
                {
                    GH_Path gh_path = new GH_Path(supportNode, (int)result.Type, loadingIndex);
                    Vector3d force = new Vector3d(result.Forces.X, result.Forces.Y, result.Forces.Z);
                    oReactions.Add(force, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oReactions;
        }

        public static DataTree<Vector3d> GetLineSupportForces(IModel iModel, IResults iResults, int loadingIndex, string lineFilter = "")
        {
            iModel.GetApplication().LockLicense();

            // Save force vectors into a tree;
            DataTree<Vector3d> oReactions = new DataTree<Vector3d>();

            List<int> supportLines = GetSupportLines(iModel, lineFilter);

            foreach (int supportLine in supportLines)
            {
                LineSupportForces[] lineSupportForces = iResults.GetLineSupportForces(supportLine, ItemAt.AtNo, localCS);
                ResultsValueType valueType = lineSupportForces[0].Type; // Get force types to avoid duplicate control points

                foreach (LineSupportForces result in lineSupportForces)
                {
                    GH_Path gh_path = new GH_Path(supportLine, (int)result.Type, loadingIndex);
                    Vector3d force = new Vector3d(result.Forces.X, result.Forces.Y, result.Forces.Z);
                    oReactions.Add(force, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oReactions;
        }

        public static DataTree<Vector3d> GetNodalSupportMoments(IModel iModel, IResults iResults, int loadingIndex, string nodeFilter = "")
        {
            iModel.GetApplication().LockLicense();

            // Save force vectors into a tree;
            DataTree<Vector3d> oReactions = new DataTree<Vector3d>();

            List<int> supportNodes = GetSupportNodes(iModel, nodeFilter);

            foreach (int supportNode in supportNodes)
            {
                NodalSupportForces[] nodalSupportForces = iResults.GetNodalSupportForces(supportNode, ItemAt.AtNo, localCS);
                ResultsValueType valueType = nodalSupportForces[0].Type; // Get force types to avoid duplicate control points

                foreach (NodalSupportForces result in nodalSupportForces)
                {
                    GH_Path gh_path = new GH_Path(supportNode, (int)result.Type, loadingIndex);
                    Vector3d moment = new Vector3d(result.Moments.X, result.Moments.Y, result.Moments.Z);
                    oReactions.Add(moment, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oReactions;
        }

        public static DataTree<Vector3d> GetLineSupportMoments(IModel iModel, IResults iResults, int loadingIndex, string lineFilter = "")
        {
            iModel.GetApplication().LockLicense();

            // Save force vectors into a tree;
            DataTree<Vector3d> oReactions = new DataTree<Vector3d>();

            List<int> supportLines = GetSupportLines(iModel, lineFilter);

            foreach (int supportLine in supportLines)
            {
                LineSupportForces[] lineSupportForces = iResults.GetLineSupportForces(supportLine, ItemAt.AtNo, localCS);
                ResultsValueType valueType = lineSupportForces[0].Type; // Get force types to avoid duplicate control points

                foreach (LineSupportForces result in lineSupportForces)
                {
                    GH_Path gh_path = new GH_Path(supportLine, (int)result.Type, loadingIndex);
                    Vector3d moment = new Vector3d(result.Moments.X, result.Moments.Y, result.Moments.Z);
                    oReactions.Add(moment, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oReactions;
        }

        public static DataTree<Point3d> GetNodalSupportLocations(IModel iModel, IResults iResults, int loadingIndex, string nodeFilter = "")
        {
            iModel.GetApplication().LockLicense();

            IModelData iModelData = iModel.GetModelData();

            List<int> supportNodes = GetSupportNodes(iModel, nodeFilter);

            DataTree<Point3d> oPoints = new DataTree<Point3d>();
            foreach (int supportNode in supportNodes)
            {
                NodalSupportForces[] nodalSupportForces = iResults.GetNodalSupportForces(supportNode, ItemAt.AtNo, localCS);
                ResultsValueType valueType = nodalSupportForces[0].Type; // Get force types to avoid duplicate control points

                foreach (NodalSupportForces result in nodalSupportForces)
                {
                    GH_Path gh_path = new GH_Path(supportNode, (int)result.Type, loadingIndex);
                    Node node = iModelData.GetNode(result.NodeNo, ItemAt.AtNo).GetData();
                    Point3d location = new Point3d(node.X, node.Y, node.Z);
                    oPoints.Add(location, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oPoints;
        }

        public static DataTree<Point3d> GetLineSupportLocations(IModel iModel, IResults iResults, int loadingIndex, string lineFilter = "")
        {
            iModel.GetApplication().LockLicense();

            IModelData iModelData = iModel.GetModelData();

            List<int> supportLines = GetSupportLines(iModel, lineFilter);

            DataTree<Point3d> oPoints = new DataTree<Point3d>();
            foreach (int supportLine in supportLines)
            {
                LineSupportForces[] lineSupportForces = iResults.GetLineSupportForces(supportLine, ItemAt.AtNo, localCS);
                ResultsValueType valueType = lineSupportForces[0].Type;

                ILine iLine = iModelData.GetLine(supportLine, ItemAt.AtNo);
                Curve baseline = Utilities_RFem.RfLineToGhCurve(iLine.GetData(), iModelData);

                foreach (LineSupportForces result in lineSupportForces)
                {
                    GH_Path gh_path = new GH_Path(supportLine, (int)result.Type, loadingIndex);
                    Point3d location = baseline.PointAtNormalizedLength(Math.Min(result.Location / baseline.GetLength(), 1.0));
                    oPoints.Add(location, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oPoints;
        }

        public static List<NodalSupportForces[]> GetNodalSupportForceList(IModel iModel, IResults iResults, string nodeFilter = "")
        {
            List<NodalSupportForces[]> oValues = new List<NodalSupportForces[]>();

            List<int> supportNodes = GetSupportNodes(iModel, nodeFilter);

            foreach (int supportNode in supportNodes)
            {
                NodalSupportForces[] nodalSupportForces = iResults.GetNodalSupportForces(supportNode, ItemAt.AtNo, localCS);
                oValues.Add(nodalSupportForces);
            }
            return oValues;
        }

        public static List<LineSupportForces[]> GetLineSupportForceList(IModel iModel, IResults iResults, string lineFilter = "")
        {
            List<LineSupportForces[]> oValues = new List<LineSupportForces[]>();

            List<int> supportLines = GetSupportLines(iModel, lineFilter);

            foreach (int supportLine in supportLines)
            {
                LineSupportForces[] lineSupportForces = iResults.GetLineSupportForces(supportLine, ItemAt.AtNo, localCS);
                oValues.Add(lineSupportForces);
            }
            return oValues;
        }

        public static DataTree<float> GetNodalSupportReactionsFloat(IModel iModel, IResults iResults, SupportReactions _resultType, int loadingIndex, List<NodalSupportForces[]> supportForceList)
        {
            DataTree<float> oValues = new DataTree<float>();

            //List<NodalSupportForces[]> supportForceList = GetNodalSupportForceList(iModel, iResults);

            foreach (NodalSupportForces[] nodalSupportForces in supportForceList)
            {
                ResultsValueType valueType = nodalSupportForces[0].Type; // Get force types to avoid duplicate control points

                foreach (NodalSupportForces result in nodalSupportForces)
                {
                    GH_Path gh_path = new GH_Path(result.NodeNo, (int)result.Type, loadingIndex);
                    
                    float value = 0f;

                    switch (_resultType)
                    {
                        case SupportReactions.Fx:
                            value = (float)result.Forces.X;
                            break;
                        case SupportReactions.Fy:
                            value = (float)result.Forces.Y;
                            break;
                        case SupportReactions.Fz:
                            value = (float)result.Forces.Z;
                            break;
                        case SupportReactions.Mx:
                            value = (float)result.Moments.X;
                            break;
                        case SupportReactions.My:
                            value = (float)result.Moments.Y;
                            break;
                        case SupportReactions.Mz:
                            value = (float)result.Moments.Z;
                            break;
                        default:
                            break;
                    }

                    oValues.Add(value, gh_path);
                }
            }
            return oValues;
        }

        public static DataTree<float> GetLineSupportReactionsFloat(IModel iModel, IResults iResults, SupportReactions _resultType, int loadingIndex, List<LineSupportForces[]> supportForceList)
        {
            DataTree<float> oValues = new DataTree<float>();

            //List<LineSupportForces[]> supportForceList = GetLineSupportForceList(iModel, iResults);

            foreach (LineSupportForces[] lineSupportForces in supportForceList)
            {
                ResultsValueType valueType = lineSupportForces[0].Type; // Get force types to avoid duplicate control points

                foreach (LineSupportForces result in lineSupportForces)
                {
                    GH_Path gh_path = new GH_Path(result.LineNo, (int)result.Type, loadingIndex);
                    float value = 0f;

                    switch (_resultType)
                    {
                        case SupportReactions.Fx:
                            value = (float)result.Forces.X;
                            break;
                        case SupportReactions.Fy:
                            value = (float)result.Forces.Y;
                            break;
                        case SupportReactions.Fz:
                            value = (float)result.Forces.Z;
                            break;
                        case SupportReactions.Mx:
                            value = (float)result.Moments.X;
                            break;
                        case SupportReactions.My:
                            value = (float)result.Moments.Y;
                            break;
                        case SupportReactions.Mz:
                            value = (float)result.Moments.Z;
                            break;
                        default:
                            break;
                    }
                    oValues.Add(value, gh_path);
                }
            }
            return oValues;
        }
    }

    public enum SupportReactions
    {
        Fx,
        Fy,
        Fz,
        Mx,
        My,
        Mz
    }
}
