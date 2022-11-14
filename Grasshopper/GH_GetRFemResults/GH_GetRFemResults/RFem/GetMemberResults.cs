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
using System.Windows.Forms;

namespace GH_GetRFemResults.RFem
{
    public static class GetMemberResults
    {
        static DataTree<Point3d> controlPoints = new DataTree<Point3d>();

        public static DataTree<Curve> GetMembers(IModel iModel, string memberFilter)
        {            
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            DataTree<Curve> oCurves = new DataTree<Curve>();
            List<Member> rfMembers = iModelData.GetMembers().ToList();

            HashSet<int> filterMembers = GetFilters(memberFilter);

            foreach (Member member in rfMembers)
            {
                if (memberFilter != "" && !filterMembers.Contains(member.No))
                {
                    continue;
                }

                GH_Path gh_path = new GH_Path(member.No);
                ILine iLine = iModelData.GetLine(member.LineNo, ItemAt.AtNo);
                Curve baseline = Utilities_RFem.RfLineToGhCurve(iLine.GetData(), iModelData);

                oCurves.Add(baseline, gh_path);
            }
            iModel.GetApplication().UnlockLicense();
            return oCurves;
        }

        public static DataTree<Point3d> GetControlPoints(IModel iModel, string memberFilter)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            // Get control points
            controlPoints.Clear();
            List<Member> rfMembers = iModelData.GetMembers().ToList();

            HashSet<int> filterMembers = GetFilters(memberFilter);

            foreach (Member member in rfMembers)
            {
                if (memberFilter != "" && !filterMembers.Contains(member.No))
                {
                    continue;
                }

                GH_Path pts_path = new GH_Path(member.No);
                controlPoints.RemovePath(pts_path);

                ILine iLine = iModelData.GetLine(member.LineNo, ItemAt.AtNo);
                Dlubal.RFEM5.Line line = iLine.GetData();

                foreach (Point3D controlPoint in line.ControlPoints)
                {
                    Point3d point = new Point3d();
                    point.X = controlPoint.X;
                    point.Y = controlPoint.Y;
                    point.Z = controlPoint.Z;
                    controlPoints.Add(point, pts_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return controlPoints;
        }

        public static DataTree<Point3d> GetDeformationLocations(IModel iModel, IResults iResults, int loadingIndex, string memberFilter)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            List<Member> rfMembers = iModelData.GetMembers().ToList();
            
            DataTree<Point3d> oPoints = new DataTree<Point3d>();

            HashSet<int> filterMembers = GetFilters(memberFilter);

            foreach (Member member in rfMembers)
            {
                if (memberFilter != "" && !filterMembers.Contains(member.No))
                {
                    continue;
                }

                ILine iLine = iModelData.GetLine(member.LineNo, ItemAt.AtNo);
                Curve baseline = Utilities_RFem.RfLineToGhCurve(iLine.GetData(), iModelData);

                // Get deformations
                MemberDeformations[] memberResults = iResults.GetMemberDeformations(member.No, ItemAt.AtNo, MemberAxesType.GlobalAxes); // We can't sort this list                

                foreach (MemberDeformations result in memberResults)
                {
                    GH_Path gh_path = new GH_Path(member.No, (int)result.Type, loadingIndex);
                    oPoints.Add(baseline.PointAtNormalizedLength(Math.Min(result.Location / baseline.GetLength(), 1.0)), gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oPoints;
        }

        public static DataTree<Vector3d> GetMemberDisplacements(IModel iModel, IResults iResults, int loadingIndex, string memberFilter)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();
            
            // Get control points
            controlPoints.Clear();
            List<Member> rfMembers = iModelData.GetMembers().ToList();
            // Save deformation vectors into a tree;
            DataTree<Vector3d> oDisplacements = new DataTree<Vector3d>();

            HashSet<int> filterMembers = GetFilters(memberFilter);

            foreach (Member member in rfMembers)
            {
                if (memberFilter != "" && !filterMembers.Contains(member.No))
                {
                    continue;
                }

                // Add also control points. We are just going to get one set of control points for each curve regardless the result type                
                GH_Path pts_path = new GH_Path(member.No);
                controlPoints.RemovePath(pts_path);
                ILine iLine = iModelData.GetLine(member.LineNo, ItemAt.AtNo);
                Curve baseline = Utilities_RFem.RfLineToGhCurve(iLine.GetData(), iModelData);

                // Get deformations
                MemberDeformations[] memberResults = iResults.GetMemberDeformations(member.No, ItemAt.AtNo, MemberAxesType.GlobalAxes); // We can't sort this list                
                ResultsValueType valueType = memberResults[0].Type; // Get deformation types to avoid duplicate control points
                foreach (MemberDeformations result in memberResults)
                {
                    GH_Path gh_path = new GH_Path(member.No, (int)result.Type, loadingIndex);
                    Vector3d displacement = new Vector3d(result.Displacements.X, result.Displacements.Y, result.Displacements.Z);
                    oDisplacements.Add(displacement, gh_path);
                    // Get control points
                    if (result.Type == valueType)
                    {
                        controlPoints.Add(baseline.PointAtNormalizedLength(Math.Min(result.Location / baseline.GetLength(), 1.0)), pts_path);
                    }
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oDisplacements;
        }

        public static DataTree<Vector3d> GetMemberInternalForces(IModel iModel, IResults iResults, int loadingIndex, string memberFilter)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            List<Member> rfMembers = iModelData.GetMembers().ToList();
            // Save force vectors into a tree;
            DataTree<Vector3d> oForces = new DataTree<Vector3d>();

            HashSet<int> filterMembers = GetFilters(memberFilter);

            foreach (Member member in rfMembers)
            {
                if (memberFilter != "" && !filterMembers.Contains(member.No))
                {
                    continue;
                }

                ILine iLine = iModelData.GetLine(member.LineNo, ItemAt.AtNo);
                Curve baseline = Utilities_RFem.RfLineToGhCurve(iLine.GetData(), iModelData);
                // Get forces
                MemberForces[] memberResults = iResults.GetMemberInternalForces(member.No, ItemAt.AtNo, false); // We can't sort this list                
                ResultsValueType valueType = memberResults[0].Type; // Get force types to avoid duplicate control points
                foreach (var result in memberResults)
                {
                    GH_Path gh_path = new GH_Path(member.No, (int)result.Type, loadingIndex);
                    Vector3d force = new Vector3d(result.Forces.X, result.Forces.Y, result.Forces.Z);
                    oForces.Add(force, gh_path);

                }
            }
            iModel.GetApplication().UnlockLicense();
            return oForces;
        }

        public static DataTree<float> GetMemberInternalForce(IModel iModel, IResults iResults, MemberInternalForce _force, int loadingIndex, string memberFilter)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            List<Member> rfMembers = iModelData.GetMembers().ToList();
            // Save force vectors into a tree;
            DataTree<float> oForce = new DataTree<float>();

            HashSet<int> filterMembers = GetFilters(memberFilter);

            foreach (Member member in rfMembers)
            {
                if (memberFilter != "" && !filterMembers.Contains(member.No))
                {
                    continue;
                }

                ILine iLine = iModelData.GetLine(member.LineNo, ItemAt.AtNo);
                Curve baseline = Utilities_RFem.RfLineToGhCurve(iLine.GetData(), iModelData);
                // Get forces
                MemberForces[] memberResults = iResults.GetMemberInternalForces(member.No, ItemAt.AtNo, true); // We can't sort this list                
                ResultsValueType valueType = memberResults[0].Type; // Get force types to avoid duplicate control points
                float force;
                foreach (var result in memberResults)
                {
                    GH_Path gh_path = new GH_Path(member.No, (int)result.Type, loadingIndex);
                    switch (_force)
                    {
                        case MemberInternalForce.AxialForceN:
                            force = (float)result.Forces.X;
                            break;
                        case MemberInternalForce.ShearForceVy:
                            force = (float)result.Forces.Y;
                            break;
                        case MemberInternalForce.ShearForceVz:
                            force = (float)result.Forces.Z;
                            break;
                        case MemberInternalForce.MomentMx:
                            force = (float)result.Moments.X;
                            break;
                        case MemberInternalForce.MomentMy:
                            force = (float)result.Moments.Y;
                            break;
                        case MemberInternalForce.MomentMz:
                            force = (float)result.Moments.Z;
                            break;
                        default:
                            force = 0;
                            break;
                    }
                    oForce.Add(force, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oForce;
        }

        public static DataTree<Vector3d> GetMemberMoments(IModel iModel, IResults iResults, int loadingIndex, string memberFilter)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            List<Member> rfMembers = iModelData.GetMembers().ToList();
            // Save moment vectors into a tree;
            DataTree<Vector3d> oMoments = new DataTree<Vector3d>();

            HashSet<int> filterMembers = GetFilters(memberFilter);

            foreach (var member in rfMembers)
            {
                if (memberFilter != "" && !filterMembers.Contains(member.No))
                {
                    continue;
                }

                ILine iLine = iModelData.GetLine(member.LineNo, ItemAt.AtNo);
                Curve baseline = Utilities_RFem.RfLineToGhCurve(iLine.GetData(), iModelData);
                // Get moments
                MemberForces[] memberResults = iResults.GetMemberInternalForces(member.No, ItemAt.AtNo, false); // We can't sort this list                
                ResultsValueType valueType = memberResults[0].Type; // Get deformation types to avoid duplicate control points
                foreach (var result in memberResults)
                {
                    GH_Path gh_path = new GH_Path(member.No, (int)result.Type, loadingIndex);
                    Vector3d moment = new Vector3d(result.Moments.X, result.Moments.Y, result.Moments.Z);
                    oMoments.Add(moment, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oMoments;
        }

        public static DataTree<Curve> GetDeformedMembers(DataTree<Vector3d> memberDisplacements, double scale)
        {
            DataTree<Curve> oCurves = new DataTree<Curve>();

            // Same tree structure as displacements
            foreach (GH_Path path in memberDisplacements.Paths)
            {
                GH_Path gh_path = new GH_Path(path);
                GH_Path member_path = new GH_Path(path.Indices[0]);
                // Get deformed control points
                List<Point3d> ctrlPoints = controlPoints.Branch(member_path);
                List<Vector3d> deformations = memberDisplacements.Branch(path);
                List<Point3d> deformedPoints = new List<Point3d>();
                for (int i = 0; i < ctrlPoints.Count; i++)
                {
                    deformedPoints.Add(new Point3d(ctrlPoints[i] + scale * deformations[i]));
                }
                // Add curve to tree
                Curve memberShape = Curve.CreateControlPointCurve(deformedPoints);
                oCurves.Add(memberShape, gh_path);
            }
            return oCurves;
        }

        public static DataTree<Point3d> GetDeformedPoints(DataTree<Vector3d> memberDisplacements, double scale)
        {
            DataTree<Point3d> oPoints = new DataTree<Point3d>();
            // Same tree structure as displacements
            foreach (GH_Path path in memberDisplacements.Paths)
            {
                GH_Path gh_path = new GH_Path(path);
                GH_Path member_path = new GH_Path(path.Indices[0]);
                // Get deformed control points
                List<Point3d> ctrlPoints = controlPoints.Branch(member_path);
                List<Vector3d> deformations = memberDisplacements.Branch(path);

                for (int i = 0; i < ctrlPoints.Count; i++)
                {
                    Point3d deformedPoint = new Point3d(ctrlPoints[i] + scale * deformations[i]);
                    oPoints.Add(deformedPoint, gh_path);
                }
            }
            return oPoints;
        }

        public static DataTree<Point3d> GetForceLocations(IModel iModel, IResults iResults, int loadingIndex, string memberFilter)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            List<Member> rfMembers = iModelData.GetMembers().ToList();
            
            DataTree<Point3d> oPoints = new DataTree<Point3d>();

            HashSet<int> filterMembers = GetFilters(memberFilter);

            foreach (Member member in rfMembers)
            {
                if (memberFilter != "" && !filterMembers.Contains(member.No))
                {
                    continue;
                }

                ILine iLine = iModelData.GetLine(member.LineNo, ItemAt.AtNo);
                Curve baseline = Utilities_RFem.RfLineToGhCurve(iLine.GetData(), iModelData);
                // Get forces
                MemberForces[] memberResults = iResults.GetMemberInternalForces(member.No, ItemAt.AtNo, false); // We can't sort this list                
                ResultsValueType valueType = memberResults[0].Type; // Get force types to avoid duplicate control points
                foreach (var result in memberResults)
                {
                    GH_Path gh_path = new GH_Path(member.No, (int)result.Type, loadingIndex);
                    Point3d location = baseline.PointAtNormalizedLength(Math.Min(result.Location / baseline.GetLength(), 1.0));
                    oPoints.Add(location, gh_path);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return oPoints;
        }

        public static IResults GetResults(IModel iModel, LoadingCase loadingCase)
        {
            iModel.GetApplication().LockLicense();
            ICalculation calculation = iModel.GetCalculation();
            IResults iResults;

            try
            {
                iResults = calculation.GetResultsInFeNodes(loadingCase.loadingType, loadingCase.loadingNumber);
            }
            catch (Exception exception)
            {
                ErrorInfo[] errors = calculation.Calculate(loadingCase.loadingType, loadingCase.loadingNumber);
                if (errors.Length > 0)
                {
                    MessageBox.Show("Error during the calculation: " + errors[0].Description);
                    iModel.GetApplication().UnlockLicense();
                    return null;
                }
                iResults = calculation.GetResultsInFeNodes(loadingCase.loadingType, loadingCase.loadingNumber);
            }
            iModel.GetApplication().UnlockLicense();
            return iResults;
        }

        public static HashSet<int> GetFilters(string filter)
        {
            HashSet<int> filterNumbers = new HashSet<int>();
            if (filter != "")
            {
                string[] subList = filter.Split(',');

                foreach (string item in subList)
                {
                    if (item.Contains("-"))
                    {
                        string[] boundaries = item.Split('-');
                        int start = int.Parse(boundaries[0]);
                        int end = int.Parse(boundaries[1]);

                        int i = start;
                        while (i <= end)
                        {
                            filterNumbers.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterNumbers.Add(int.Parse(item));
                    }
                }
            }
            return filterNumbers;
        }
    }

    public enum MemberInternalForce
    {
        AxialForceN,
        ShearForceVy,
        ShearForceVz,
        MomentMx,
        MomentMy,
        MomentMz,
        ForceVector,
        MomentVector
    }
}
