using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using Dlubal.RFEM5;
using System.Runtime.InteropServices;

namespace GH_GetRFemResults.RFem
{
    public class SurfaceResult_old
    {
        //public SurfaceResultData GetData()
        //{
        //    SurfaceResultData resultData = new SurfaceResultData();



        //    return resultData;
        //}

        public void GetResults()
        {
            IModel iModel = ConnectRFem();

            LoadingType loadingType = LoadingType.LoadCaseType;
            int loadingNo = 1;

            ICalculation iCalculation = iModel.GetCalculation();
            IResults iResults = iCalculation.GetResultsInFeNodes(loadingType, loadingNo);

            int surfNo = 1;

            SurfaceBasicStresses[] basicStresses = (SurfaceBasicStresses[])iResults.GetSurfaceBasicStresses(surfNo, ItemAt.AtNo);
            //basicStresses[0].SigmaXNegative;

            iModel.GetApplication().UnlockLicense();
        }

        public IModel ConnectRFem(string _modelName = "")
        {
            IApplication app = Marshal.GetActiveObject("RFEM5.Application") as IApplication;
            app.LockLicense();
            IModel model;

            if (_modelName == "")
            {
                model = app.GetModel(0);
                return model;
            }
            else
            {
                int modelCount = app.GetModelCount();
                for (int i = 0; i < modelCount; i++)
                {
                    model = app.GetModel(i);
                    string modelName = model.GetName();

                    if (modelName.Contains(_modelName))
                    {
                        return model;
                    }
                }

                model = app.GetModel(0);
                return model;
            }
        }

        private DataTree<Mesh> CreateFEMeshes(IModel _iModel, ref List<string> msg)
        {
            IModelData iModelData = _iModel.GetModelData();

            DataTree<Mesh> feMeshes = new DataTree<Mesh>();
            IFeMesh rfMesh = _iModel.GetCalculation().GetFeMesh();
            Dlubal.RFEM5.Surface[] surfaces = (Dlubal.RFEM5.Surface[])iModelData.GetSurfaces();
            // Create fe mesh from each surface element
            foreach (Dlubal.RFEM5.Surface surface in surfaces)
            {
                // Get surface number
                int no = surface.No;
                // Create mesh -> add vertices and faces
                Mesh feMesh = new Mesh();
                FeMeshNode[] feSurfacenodeArr = (FeMeshNode[])rfMesh.GetSurfaceNodes(no, ItemAt.AtNo);
                List<FeMeshNode> feSurfacenodeList = feSurfacenodeArr.OrderBy(o => o.No).ToList();
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

        private DataTree<Point3d> GetMeshPoints(IModel _iModel)
        {
            IModelData iModelData = _iModel.GetModelData();

            DataTree<Point3d> oPoints = new DataTree<Point3d>();
            IFeMesh rfMesh = _iModel.GetCalculation().GetFeMesh();
            Dlubal.RFEM5.Surface[] surfaces = (Dlubal.RFEM5.Surface[])iModelData.GetSurfaces();

            foreach (Dlubal.RFEM5.Surface surface in surfaces)
            {
                // Get surface number
                int no = surface.No;
                
                FeMeshNode[] feSurfacenodeArr = (FeMeshNode[])rfMesh.GetSurfaceNodes(no, ItemAt.AtNo);
                List<FeMeshNode> feSurfacenodeList = feSurfacenodeArr.OrderBy(o => o.No).ToList();

                foreach (FeMeshNode node in feSurfacenodeList)
                {
                    GH_Path gh_path = new GH_Path(no, 1);
                    Point3d point = new Point3d(node.X, node.Y, node.Z);
                    oPoints.Add(point, gh_path);
                }
            }
            return oPoints;
        }

        private DataTree<Vector3d> GetMeshDisplacements(IModel _iModel, LoadingType _loadingType, int _loadingNo)
        {
            DataTree<Vector3d> oDisplacements = new DataTree<Vector3d>();
            IResults iResults = _iModel.GetCalculation().GetResultsInFeNodes(_loadingType, _loadingNo);
            // Save deformation vectors into a tree
            SurfaceDeformations[] surfaceDeformationArr = (SurfaceDeformations[])iResults.GetSurfacesDeformations(false);
            List<SurfaceDeformations> surfaceDeformationList = surfaceDeformationArr.OrderBy(o => o.LocationNo).ToList(); // Sort according to nodes so there are no errors when applying displacements
            
            foreach (SurfaceDeformations result in surfaceDeformationList)
            {
                GH_Path gh_path = new GH_Path(result.SurfaceNo, (int)result.Type);
                Vector3d displacement = new Vector3d(result.Displacements.X, result.Displacements.Y, result.Displacements.Z);
                oDisplacements.Add(displacement, gh_path);
            }
            return oDisplacements;
        }

        private DataTree<Vector3d> GetMeshBasicStress(IModel _iModel, LoadingType _loadingType, int _loadingNo)
        {
            DataTree<Vector3d> oStresses = new DataTree<Vector3d>();
            // Save results into a tree
            IResults iResults = _iModel.GetCalculation().GetResultsInFeNodes(_loadingType, _loadingNo);
            SurfaceBasicStresses[] surfaceBasicStressArr = (SurfaceBasicStresses[])iResults.GetSurfacesBasicStresses();
            List<SurfaceBasicStresses> surfaceBasicStressList = surfaceBasicStressArr.OrderBy(o => o.LocationNo).ToList();
            
            foreach (SurfaceBasicStresses result in surfaceBasicStressList)
            {
                GH_Path gh_path = new GH_Path(result.SurfaceNo, (int)result.Type);
                Vector3d stress = new Vector3d(result.SigmaXPositive, result.SigmaYPositive, result.LocationNo);
                oStresses.Add(stress, gh_path);
            }
            return oStresses;
        }

        private DataTree<Mesh> GetDeformedMeshes(IModel _iModel, DataTree<Vector3d> _meshdisplacements, DataTree<Mesh> _startingMesh, double scale, ref List<string> msg)
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
    }

    //public class SurfaceResultData
    //{
    //    SurfaceBasicStresses basicStresses = new SurfaceBasicStresses();
    //    SurfaceInternalForces internalForces = new SurfaceInternalForces();
    //    SurfacePrincipalStresses principalStresses = new SurfacePrincipalStresses();
    //    SurfaceEquivalentStresses equivalentStresses = new SurfaceEquivalentStresses();        
    //    SurfaceContactStresses contactStresses = new SurfaceContactStresses();
    //    SurfaceOtherStresses otherStresses = new SurfaceOtherStresses();
    //    SurfaceDeformations surfaceDeformations = new SurfaceDeformations();        
    //    SurfaceBasicStrains basicStrains = new SurfaceBasicStrains();
    //    SurfaceMaximumStrains maximumStrains = new SurfaceMaximumStrains();        
    //    SurfacePlasticStrains plasticStrains = new SurfacePlasticStrains();
    //    SurfacePrincipalStrains principalStrains = new SurfacePrincipalStrains();

    //    void GetStresses()
    //    {
    //        // Basic Internal Forces
    //        mX = internalForces.MomentMx;
    //        mY = internalForces.MomentMy;
    //        mXY = internalForces.MomentMxy;
    //        vX = internalForces.ShearForceVx;
    //        vY = internalForces.ShearForceVy;
    //        nX = internalForces.AxialForceNx;
    //        nY = internalForces.AxialForceNy;
    //        nXY = internalForces.AxialForceNxy;

    //        // Principal Internal Forces
    //        mX = internalForces.MomentM1;
    //        mX = internalForces.MomentM2;
    //        mX = internalForces.MomentAlphaB;
    //        mX = internalForces.MomentTMaxB;
    //        mX = internalForces.ShearForceVMaxB;
    //        mX = internalForces.ShearForceBetaB;
    //        mX = internalForces.AxialForceN1;
    //        mX = internalForces.AxialForceN2;
    //        mX = internalForces.AxialForceAlphaM;
    //        mX = internalForces.AxialForceVMaxM;

    //        // Design Internal Forces
    //        nXd = internalForces.AxialForceNxD;
    //        nYd = internalForces.AxialForceNyD;
    //        nCd = internalForces.AxialForceNcD;
    //        mXdn = internalForces.MomentMxDNegative;
    //        mXd = internalForces.MomentMxDPositive;
    //        mYdn = internalForces.MomentMyDNegative;
    //        mYd = internalForces.MomentMyDPositive;
    //        mCdn = internalForces.MomentMcDNegative;
    //        mCd = internalForces.MomentMcDPositive;

    //        // Internal Forces Info
    //        coordinates = internalForces.Coordinates;
    //        location = internalForces.LocationNo;
    //        no = internalForces.SurfaceNo;
    //        type = internalForces.Type;

    //        // Basic Stresses            
    //        sigmaX = basicStresses.SigmaXPositive;
    //        sigmaXn = basicStresses.SigmaXNegative;
    //        sigmaY = basicStresses.SigmaYPositive;
    //        sigmaYn = basicStresses.SigmaYNegative;
    //        tauXY = basicStresses.TauXYPositive;
    //        tauXYn = basicStresses.TauXYNegative;
    //        tauXZ = basicStresses.TauXZ;
    //        tauYZ = basicStresses.TauYZ;
    //        coordinates = basicStresses.Coordinates;
    //        no = basicStresses.SurfaceNo;
    //        location = basicStresses.LocationNo;
    //        type = basicStresses.Type;

    //        // Principal Stresses
    //        principalStresses.AlphaM;
    //        principalStresses.AlphaNegative;
    //        principalStresses.AlphaPositive;            
    //        principalStresses.Sigma1m;
    //        principalStresses.Sigma1Negative;
    //        principalStresses.Sigma1Positive;
    //        principalStresses.Sigma2m;
    //        principalStresses.Sigma2Negative;
    //        principalStresses.Sigma2Positive;            
    //        principalStresses.TauMax;
    //        principalStresses.Coordinates;
    //        principalStresses.LocationNo;
    //        principalStresses.SurfaceNo;
    //        principalStresses.Type;
            
    //        // Other Stresses           
    //        otherStresses.SigmaXb;
    //        otherStresses.SigmaXm;
    //        otherStresses.SigmaYb;
    //        otherStresses.SigmaYm;
    //        otherStresses.TauXYb;
    //        otherStresses.TauXYm;
    //        otherStresses.Coordinates;
    //        otherStresses.LocationNo;
    //        otherStresses.SurfaceNo;
    //        otherStresses.Type;

    //        // Equivalent Stresses
    //        equivalentStresses.SigmaEqvMax;
    //        equivalentStresses.SigmaEqvPositive;
    //        equivalentStresses.SigmaEqvNegative;
    //        equivalentStresses.SigmaEqvM;
    //        equivalentStresses.Coordinates;
    //        equivalentStresses.LocationNo;
    //        equivalentStresses.SurfaceNo;
    //        equivalentStresses.Type;

    //        // Contact Stresses
    //        contactStresses.StressSigmaZ;
    //        contactStresses.StressTauXZ;
    //        contactStresses.StressTauYZ;
    //        contactStresses.Coordinates;
    //        contactStresses.LocationNo;
    //        contactStresses.SurfaceNo;
    //        contactStresses.Type;
    //    }

    //    // Basic Internal Forces
    //    double mX;
    //    double mY;
    //    double mXY;
    //    double vX;
    //    double vY;
    //    double nX;
    //    double nY;
    //    double nXY;

    //    // Principal Internal Forces
    //    double m1;
    //    double m2;
    //    double alphaB;
    //    double mTMaxB;
    //    double vMaxB;
    //    double betaB;
    //    double n1;
    //    double n2;
    //    double alphaM;
    //    double vMaxM;

    //    // Design Internal Forces
    //    double nXd;
    //    double nYd;
    //    double nCd;
    //    double mXd;
    //    double mXdn;
    //    double mYd;
    //    double mYdn;
    //    double mCd;
    //    double mCdn;

    //    // Internal Forces Other Data
    //    internalForces.Coordinates;
    //    internalForces.LocationNo;
    //    internalForces.SurfaceNo;
    //    internalForces.Type;

    //    // BasicStresses        
    //    double sigmaX;
    //    double sigmaXn;
    //    double sigmaY;
    //    double sigmaYn;
    //    double tauXY;
    //    double tauXYn;
    //    double tauXZ;
    //    double tauYZ;
    //    int no;
    //    int location;
    //    Point3D coordinates;
    //    ResultsValueType type;
    //}
}
