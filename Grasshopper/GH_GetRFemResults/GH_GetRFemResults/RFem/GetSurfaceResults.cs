using Dlubal.RFEM5;
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
    public static class GetSurfaceResults
    {
        public static DataTree<Mesh> CreateFEMeshes(IModel _iModel, ref List<string> msg, string surfaceFilter = "")
        {
            IModelData iModelData = _iModel.GetModelData();

            DataTree<Mesh> feMeshes = new DataTree<Mesh>();
            IFeMesh rfMesh = _iModel.GetCalculation().GetFeMesh();

            Dlubal.RFEM5.Surface[] surfaces = (Dlubal.RFEM5.Surface[])iModelData.GetSurfaces();

            HashSet<int> filterSurfaces = new HashSet<int>();
            if (surfaceFilter != "")
            {
                string[] subList = surfaceFilter.Split(',');

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
                            filterSurfaces.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterSurfaces.Add(int.Parse(item));
                    }
                }
            }

            // Create fe mesh from each surface element
            foreach (Dlubal.RFEM5.Surface surface in surfaces)
            {                
                // Get surface number
                int no = surface.No;

                if (surfaceFilter != "" && !filterSurfaces.Contains(no))
                {
                    continue;
                }

                // Create mesh -> add vertices and faces
                Mesh feMesh = new Mesh();
                FeMeshNode[] feSurfacenodeArr = rfMesh.GetSurfaceNodes(no, ItemAt.AtNo);
                List <FeMeshNode> feSurfacenodeList = feSurfacenodeArr.OrderBy(o => o.No).ToList();
                var dicNodes = new Dictionary<int, int>();
                for (int i = 0; i < feSurfacenodeList.Count; i++)
                {
                    var node_location = new Point3d(feSurfacenodeList[i].X, feSurfacenodeList[i].Y, feSurfacenodeList[i].Z);
                    feMesh.Vertices.Add(node_location);
                    dicNodes.Add(feSurfacenodeList[i].No, i);
                }
                FeMesh2DElement[] fe2delements = (FeMesh2DElement[])rfMesh.GetSurfaceElements(no, ItemAt.AtNo);
                foreach (FeMesh2DElement element in fe2delements)
                {
                    int[] node_numbers = element.NodeNumbers;
                    if (node_numbers.Length == 4 && node_numbers[3] != 0)
                    {
                        dicNodes.TryGetValue(node_numbers[0], out int node0);
                        dicNodes.TryGetValue(node_numbers[1], out int node1);
                        dicNodes.TryGetValue(node_numbers[2], out int node2);
                        dicNodes.TryGetValue(node_numbers[3], out int node3);
                        feMesh.Faces.AddFace(node0, node1, node2, node3);
                    }
                    else if (node_numbers.Length == 4 && node_numbers[3] == 0)
                    {
                        dicNodes.TryGetValue(node_numbers[0], out int node0);
                        dicNodes.TryGetValue(node_numbers[1], out int node1);
                        dicNodes.TryGetValue(node_numbers[2], out int node2);
                        feMesh.Faces.AddFace(node0, node1, node2);
                    }
                    else
                    {
                        msg.Add($"Element {element.No} not imported.");
                    }
                }
                // Add mesh to tree
                GH_Path gh_path = new GH_Path(no);
                feMeshes.Add(feMesh, gh_path);
            }
            return feMeshes;
        }

        //public static DataTree<Mesh> CreateFEMeshes(IModel _iModel, DataTree<Point3d> pointTree, ref List<string> msg, string surfaceFilter = "")
        //{
        //    IModelData iModelData = _iModel.GetModelData();

        //    DataTree<Mesh> feMeshes = new DataTree<Mesh>();
        //    IFeMesh rfMesh = _iModel.GetCalculation().GetFeMesh();
        //    FeMesh2DElement[] elements = rfMesh.Get2DElements();

        //    Dlubal.RFEM5.Surface[] surfaces = (Dlubal.RFEM5.Surface[])iModelData.GetSurfaces();

        //    HashSet<int> filterSurfaces = new HashSet<int>();
        //    if (surfaceFilter != "")
        //    {
        //        string[] subList = surfaceFilter.Split(',');

        //        foreach (string item in subList)
        //        {
        //            if (item.Contains("-"))
        //            {
        //                string[] boundaries = item.Split('-');
        //                int start = int.Parse(boundaries[0]);
        //                int end = int.Parse(boundaries[1]);

        //                int i = start;
        //                while (i <= end)
        //                {
        //                    filterSurfaces.Add(i);
        //                    i++;
        //                }
        //            }
        //            else
        //            {
        //                filterSurfaces.Add(int.Parse(item));
        //            }
        //        }
        //    }

        //    Dictionary<int, List<FeMesh2DElement>> elementsInSurfaces = new Dictionary<int, List<FeMesh2DElement>>();
        //    foreach (FeMesh2DElement element in elements)
        //    {
        //        int key = element.SurfaceNo;
        //        FeMesh2DElement value = element;

        //        if (surfaceFilter != "" && !filterSurfaces.Contains(key))
        //        {
        //            continue;
        //        }

        //        if (elementsInSurfaces.ContainsKey(key))
        //        {
        //            elementsInSurfaces[key].Add(value);
        //        }
        //        else
        //        {
        //            elementsInSurfaces.Add(key, new List<FeMesh2DElement>());
        //            elementsInSurfaces[key].Add(value);
        //        }
        //    }

        //    // Create fe mesh from each surface element
        //    foreach (Dlubal.RFEM5.Surface surface in surfaces)
        //    {
        //        // Get surface number
        //        int no = surface.No;

        //        if (surfaceFilter != "" && !filterSurfaces.Contains(no))
        //        {
        //            continue;
        //        }

        //        // Create mesh -> add vertices and faces
        //        Mesh feMesh = new Mesh();
        //        GH_Path gh_path = new GH_Path(no);

        //        List<Point3d> pointList = pointTree.Branch(gh_path);

        //        var dicNodes = new Dictionary<Point3d, int>();
        //        for (int i = 0; i < pointList.Count; i++)
        //        {
        //            feMesh.Vertices.Add(pointList[i]);
        //            dicNodes.Add(pointList[i], i);
        //        }
        //        FeMesh2DElement[] fe2delements = elementsInSurfaces[no].ToArray();
        //        foreach (FeMesh2DElement element in fe2delements)
        //        {
        //            int[] node_numbers = element.NodeNumbers;
        //            if (node_numbers.Length == 4 && node_numbers[3] != 0)
        //            {
        //                feMesh.Faces.AddFace()
        //                dicNodes.TryGetValue(node_numbers[0], out int node0);
        //                dicNodes.TryGetValue(node_numbers[1], out int node1);
        //                dicNodes.TryGetValue(node_numbers[2], out int node2);
        //                dicNodes.TryGetValue(node_numbers[3], out int node3);
        //                FeMeshNode meshNode = new FeMeshNode();
                        
        //                feMesh.Faces.AddFace(node0, node1, node2, node3);
        //            }
        //            else if (node_numbers.Length == 4 && node_numbers[3] == 0)
        //            {
        //                dicNodes.TryGetValue(node_numbers[0], out int node0);
        //                dicNodes.TryGetValue(node_numbers[1], out int node1);
        //                dicNodes.TryGetValue(node_numbers[2], out int node2);
        //                feMesh.Faces.AddFace(node0, node1, node2);
        //            }
        //            else
        //            {
        //                msg.Add($"Element {element.No} not imported.");
        //            }
        //        }
        //        // Add mesh to tree
        //        feMeshes.Add(feMesh, gh_path);
        //    }
        //    return feMeshes;
        //}

        public static DataTree<Point3d> GetMeshPoints(IModel _iModel, string surfaceFilter = "")
        {
            IModelData iModelData = _iModel.GetModelData();

            DataTree<Point3d> oPoints = new DataTree<Point3d>();
            IFeMesh rfMesh = _iModel.GetCalculation().GetFeMesh();
            Dlubal.RFEM5.Surface[] surfaces = (Dlubal.RFEM5.Surface[])iModelData.GetSurfaces();

            HashSet<int> filterSurfaces = new HashSet<int>();
            if (surfaceFilter != "")
            {
                string[] subList = surfaceFilter.Split(',');

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
                            filterSurfaces.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterSurfaces.Add(int.Parse(item));
                    }
                }
            }

            foreach (Dlubal.RFEM5.Surface surface in surfaces)
            {
                // Get surface number
                int no = surface.No;

                if (surfaceFilter != "" && !filterSurfaces.Contains(no))
                {
                    continue;
                }

                FeMeshNode[] feSurfacenodeArr = (FeMeshNode[])rfMesh.GetSurfaceNodes(no, ItemAt.AtNo);
                List<FeMeshNode> feSurfacenodeList = feSurfacenodeArr.OrderBy(o => o.No).ToList();

                foreach (FeMeshNode node in feSurfacenodeList)
                {
                    GH_Path gh_path = new GH_Path(no);
                    Point3d point = new Point3d(node.X, node.Y, node.Z);
                    oPoints.Add(point, gh_path);
                }
            }
            return oPoints;
        }

        public static DataTree<Vector3d> GetMeshDisplacements(IResults _iResults, int _loadingIndex, string surfaceFilter = "")
        {
            DataTree<Vector3d> oDisplacements = new DataTree<Vector3d>();
            // Save deformation vectors into a tree
            SurfaceDeformations[] surfaceDeformationArr = _iResults.GetSurfacesDeformations(false);
            List<SurfaceDeformations> surfaceDeformationList = surfaceDeformationArr.OrderBy(o => o.LocationNo).ToList(); // Sort according to nodes so there are no errors when applying displacements

            HashSet<int> filterSurfaces = new HashSet<int>();
            if (surfaceFilter != "")
            {
                string[] subList = surfaceFilter.Split(',');

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
                            filterSurfaces.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterSurfaces.Add(int.Parse(item));
                    }
                }
            }

            foreach (SurfaceDeformations result in surfaceDeformationList)
            {
                int no = result.SurfaceNo;

                if (surfaceFilter != "" && !filterSurfaces.Contains(no))
                {
                    continue;
                }

                GH_Path gh_path = new GH_Path(result.SurfaceNo, (int)result.Type, _loadingIndex);
                Vector3d displacement = new Vector3d(result.Displacements.X, result.Displacements.Y, result.Displacements.Z);
                oDisplacements.Add(displacement, gh_path);
            }
            return oDisplacements;
        }

        public static DataTree<Mesh> GetDeformedMeshes(DataTree<Vector3d> _meshdisplacements, DataTree<Mesh> _startingMesh, double scale, ref List<string> msg)
        {
            DataTree<Mesh> oMeshes = new DataTree<Mesh>();
            // Same tree structure as displacements
            foreach (GH_Path path in _meshdisplacements.Paths)
            {
                var gh_path = new GH_Path(path);
                var mesh_path = new GH_Path(path.Indices[0]);
                // Get fe mesh as starting mesh
                var mesh = _startingMesh.Branch(mesh_path)[0].DuplicateMesh();
                // Move FE Nodes according to displacements
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    mesh.Vertices[i] += (Point3f)(Point3d)(scale * _meshdisplacements.Branch(path)[i]);
                }
                // Add mesh to tree
                oMeshes.Add(mesh, gh_path);
            }
            return oMeshes;
        }

        public static DataTree<SurfaceResult> GetSurfaceInternalForces(IResults _iResults, SurfaceBasicInternalForce _resultType, int _loadingIndex, string surfaceFilter = "")
        {
            DataTree<SurfaceResult> oValues = new DataTree<SurfaceResult>();
            // Save results into a tree
            SurfaceInternalForces[] surfaceInternalForcesArr = _iResults.GetSurfacesInternalForces();
            List<SurfaceInternalForces> surfaceInternalForcesList = surfaceInternalForcesArr.OrderBy(o => o.LocationNo).ToList();

            HashSet<int> filterSurfaces = new HashSet<int>();
            if (surfaceFilter != "")
            {
                string[] subList = surfaceFilter.Split(',');

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
                            filterSurfaces.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterSurfaces.Add(int.Parse(item));
                    }
                }
            }

            foreach (SurfaceInternalForces result in surfaceInternalForcesList)
            {
                int no = result.SurfaceNo;

                if (surfaceFilter != "" && !filterSurfaces.Contains(no))
                {
                    continue;
                }

                GH_Path gh_path = new GH_Path(result.SurfaceNo, (int)result.Type, _loadingIndex);
                Point3d point = new Point3d(result.Coordinates.X, result.Coordinates.Y, result.Coordinates.Z);
                SurfaceResult value;
                switch (_resultType)
                {
                    case RFem.SurfaceBasicInternalForce.AxialForceNx:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.AxialForceNx, "AxialForceNx");
                        break;
                    case RFem.SurfaceBasicInternalForce.AxialForceNy:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.AxialForceNy, "AxialForceNy");
                        break;
                    case RFem.SurfaceBasicInternalForce.AxialForceNxy:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.AxialForceNxy, "AxialForceNxy");
                        break;
                    case RFem.SurfaceBasicInternalForce.ShearForceVx:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.ShearForceVx, "ShearForceVx");
                        break;
                    case RFem.SurfaceBasicInternalForce.ShearForceVy:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.ShearForceVy, "ShearForceVy");
                        break;                    
                    case RFem.SurfaceBasicInternalForce.MomentMx:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.MomentMx, "MomentMx");
                        break;
                    case RFem.SurfaceBasicInternalForce.MomentMy:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.MomentMy, "MomentMy");
                        break;
                    case RFem.SurfaceBasicInternalForce.MomentMxy:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.MomentMxy, "MomentMxy");
                        break;
                    default:
                        value = new SurfaceResult(0, new Point3d(0f, 0f, 0f), 0f, "");
                        break;
                }
                
                oValues.Add(value, gh_path);
            }
            return oValues;
        }
        public static DataTree<SurfaceResult> GetSurfaceInternalForces(IResults _iResults, SurfacePrincipalInternalForce _resultType, int _loadingIndex, string surfaceFilter = "")
        {
            DataTree<SurfaceResult> oValues = new DataTree<SurfaceResult>();
            // Save results into a tree
            SurfaceInternalForces[] surfaceInternalForcesArr = _iResults.GetSurfacesInternalForces();
            List<SurfaceInternalForces> surfaceInternalForcesList = surfaceInternalForcesArr.OrderBy(o => o.LocationNo).ToList();

            HashSet<int> filterSurfaces = new HashSet<int>();
            if (surfaceFilter != "")
            {
                string[] subList = surfaceFilter.Split(',');

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
                            filterSurfaces.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterSurfaces.Add(int.Parse(item));
                    }
                }
            }

            foreach (SurfaceInternalForces result in surfaceInternalForcesList)
            {
                int no = result.SurfaceNo;

                if (surfaceFilter != "" && !filterSurfaces.Contains(no))
                {
                    continue;
                }

                GH_Path gh_path = new GH_Path(result.SurfaceNo, (int)result.Type, _loadingIndex);
                Point3d point = new Point3d(result.Coordinates.X, result.Coordinates.Y, result.Coordinates.Z);
                SurfaceResult value;
                switch (_resultType)
                {
                    case SurfacePrincipalInternalForce.MomentM1:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.MomentM1, "MomentM1");
                        break;
                    case SurfacePrincipalInternalForce.MomentM2:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.MomentM2, "MomentM2");
                        break;
                    case SurfacePrincipalInternalForce.MomentAlphaB:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.MomentAlphaB, "MomentAlphaB");
                        break;
                    case SurfacePrincipalInternalForce.MomentTMaxB:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.MomentTMaxB, "MomentTMaxB");
                        break;
                    case SurfacePrincipalInternalForce.ShearForceVMaxB:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.ShearForceVMaxB, "ShearForceVMaxB");
                        break;
                    case SurfacePrincipalInternalForce.ShearForceBetaB:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.ShearForceBetaB, "ShearForceBetaB");
                        break;
                    case SurfacePrincipalInternalForce.AxialForceN1:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.AxialForceN1, "AxialForceN1");
                        break;
                    case SurfacePrincipalInternalForce.AxialForceN2:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.AxialForceN2, "AxialForceN2");
                        break;
                    case SurfacePrincipalInternalForce.AxialForceAlphaM:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.AxialForceAlphaM, "AxialForceAlphaM");
                        break;
                    case SurfacePrincipalInternalForce.AxialForceVMaxM:
                        value = new SurfaceResult(result.SurfaceNo, point, (float)result.AxialForceVMaxM, "AxialForceVMaxM");
                        break;
                    default:
                        value = new SurfaceResult(0, new Point3d(0f, 0f, 0f), 0f, "");
                        break;
                }

                oValues.Add(value, gh_path);
            }
            return oValues;
        }

        public static DataTree<float> GetSurfaceInternalForcesFloat(IResults _iResults, SurfaceBasicInternalForce _resultType, int _loadingIndex, out DataTree<Point3d> points, string surfaceFilter = "")
        {
            DataTree<float> oValues = new DataTree<float>();
            points = new DataTree<Point3d>();
            // Save results into a tree
            SurfaceInternalForces[] surfaceInternalForcesArr = _iResults.GetSurfacesInternalForces();
            List<SurfaceInternalForces> surfaceInternalForcesList = surfaceInternalForcesArr.OrderBy(o => o.LocationNo).ToList();
            
            HashSet<int> filterSurfaces = new HashSet<int>();
            if (surfaceFilter != "")
            {
                string[] subList = surfaceFilter.Split(',');

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
                            filterSurfaces.Add(i);
                            i++;
                        }
                    }
                    else
                    {
                        filterSurfaces.Add(int.Parse(item));
                    }
                }
            }

            foreach (SurfaceInternalForces result in surfaceInternalForcesList)
            {
                int no = result.SurfaceNo;

                if (surfaceFilter != "" && !filterSurfaces.Contains(no))
                {
                    continue;
                }

                GH_Path gh_path = new GH_Path(result.SurfaceNo, (int)result.Type, _loadingIndex);
                Point3d point = new Point3d(result.Coordinates.X, result.Coordinates.Y, result.Coordinates.Z);
                //points.Add(new FePoint(point, result.LocationNo));
                points.Add(point, gh_path);
                float value;
                switch (_resultType)
                {
                    case RFem.SurfaceBasicInternalForce.AxialForceNx:
                        value = (float)result.AxialForceNx;
                        break;
                    case RFem.SurfaceBasicInternalForce.AxialForceNy:
                        value = (float)result.AxialForceNy;
                        break;
                    case RFem.SurfaceBasicInternalForce.AxialForceNxy:
                        value = (float)result.AxialForceNxy;
                        break;
                    case RFem.SurfaceBasicInternalForce.ShearForceVx:
                        value = (float)result.ShearForceVx;
                        break;
                    case RFem.SurfaceBasicInternalForce.ShearForceVy:
                        value = (float)result.ShearForceVy;
                        break;
                    case RFem.SurfaceBasicInternalForce.MomentMx:
                        value = (float)result.MomentMx;
                        break;
                    case RFem.SurfaceBasicInternalForce.MomentMy:
                        value = (float)result.MomentMy;
                        break;
                    case RFem.SurfaceBasicInternalForce.MomentMxy:
                        value = (float)result.MomentMxy;
                        break;
                    default:
                        value = 0f;
                        break;
                }

                oValues.Add(value, gh_path);
            }
            return oValues;
        }
    }

    public enum SurfaceBasicInternalForce
    {
        AxialForceNx,
        AxialForceNy,
        AxialForceNxy,
        ShearForceVx,
        ShearForceVy,
        MomentMx,
        MomentMy,
        MomentMxy
    }
    public enum SurfacePrincipalInternalForce
    {
        MomentM1,
        MomentM2,
        MomentAlphaB,
        MomentTMaxB,
        ShearForceVMaxB,
        ShearForceBetaB,
        AxialForceN1,
        AxialForceN2,
        AxialForceAlphaM,
        AxialForceVMaxM
    }

    public struct FePoint
    {
        public Point3d point;
        public int number;

        public FePoint(Point3d point, int number)
        {
            this.point = point;
            this.number = number;
        }
    }
}
