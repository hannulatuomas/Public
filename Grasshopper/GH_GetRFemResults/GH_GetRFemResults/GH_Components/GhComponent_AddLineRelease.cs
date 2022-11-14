using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using GH_GetRFemResults.RFem;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GH_GetRFemResults.GH_Components
{
    public class GhComponent_AddLineRelease : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhComponent_AddLineRelease class.
        /// </summary>
        public GhComponent_AddLineRelease()
          : base("AddLineRelease", "AddLineRelease",
              "",
              "RFemResults", "LineRelease")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "Model", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("LineId", "LineId", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("SurfaceId", "SurfaceId", "", GH_ParamAccess.list);
            pManager.AddTextParameter("TranslationalRelease", "TranslationalRelease", "Give Spring constants as: Ux [kN/m2], Uy [kN/m2], Yz [kN/m2]. Give -1 if fixed.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("Run", "Run", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            RfemModel model = new RfemModel(null);
            bool run = false;
            string springConstants = "";
            int lineId = 0;
            List<int> surfaceIds = new List<int>();

            if (!DA.GetData(4, ref run)) return;
            if (!DA.GetData(0, ref model) || run == false) return;
            if (model.iModel == null) return;
            IModel iModel = model.iModel;

            iModel.GetApplication().LockLicense();
            IModelData modelData = iModel.GetModelData();
            LineRelease[] lineReleases = modelData.GetLineReleases();
            LineReleaseType[] lineReleaseTypes = modelData.GetLineReleaseTypes();
            int releaseNo = 1;
            int releaseTypeNo = 1;

            if (lineReleases.Length > 0)
            {
                releaseNo = lineReleases[lineReleases.Length - 1].No + 1;
            }
            if (lineReleaseTypes.Length > 0)
            {
                releaseTypeNo = lineReleaseTypes[lineReleaseTypes.Length - 1].No + 1;
            }
            iModel.GetApplication().UnlockLicense();

            // Set LineReleaseType
            LineReleaseType lineReleaseType = new LineReleaseType();
            LineRelease lineRelease = new LineRelease();

            if (DA.GetData(3, ref springConstants))
            {
                string[] translationalConstants = springConstants.Split(',');
                double translationalConstantX = 0;
                double translationalConstantY = 0;
                double translationalConstantZ = 0;
                bool newReleaseType = true;

                try
                {
                    translationalConstantX= double.Parse(translationalConstants[0]);
                    if (translationalConstantX >= 0) translationalConstantX = translationalConstantX * 1000;
                    translationalConstantY = double.Parse(translationalConstants[1]);
                    if (translationalConstantY >= 0) translationalConstantY = translationalConstantY * 1000;
                    translationalConstantZ = double.Parse(translationalConstants[2]);
                    if (translationalConstantZ >= 0) translationalConstantZ = translationalConstantZ * 1000;
                }
                catch (Exception)
                {
                    return;
                }

                foreach (LineReleaseType type in lineReleaseTypes)
                {
                    if (type.TranslationalConstantX == translationalConstantX && type.TranslationalConstantY == translationalConstantY && type.TranslationalConstantZ == translationalConstantZ)
                    {
                        lineReleaseType = type;
                        newReleaseType = false;
                        break;
                    }
                }
                if (newReleaseType)
                {
                    lineReleaseType.Comment = "";
                    lineReleaseType.ID = "";
                    lineReleaseType.IsValid = true;
                    lineReleaseType.No = releaseTypeNo;
                    lineReleaseType.TranslationalConstantX = translationalConstantX;
                    lineReleaseType.TranslationalConstantY = translationalConstantY;
                    lineReleaseType.TranslationalConstantZ = translationalConstantZ;
                    lineReleaseType.TranslationalNonlinearityX = NonlinearityType.NoneNonlinearityType;
                    lineReleaseType.TranslationalNonlinearityY = NonlinearityType.NoneNonlinearityType;
                    lineReleaseType.TranslationalNonlinearityZ = NonlinearityType.NoneNonlinearityType;
                    lineReleaseType.RotationalConstantX = -1;
                    lineReleaseType.RotationalNonlinearityX = NonlinearityType.NoneNonlinearityType;

                    iModel.GetApplication().LockLicense();
                    modelData.PrepareModification();
                    modelData.SetLineReleaseType(lineReleaseType);
                    modelData.FinishModification();
                    iModel.GetApplication().UnlockLicense();
                }
            }

            // Set LineRelease
            if (DA.GetData(1, ref lineId) && DA.GetDataList(2, surfaceIds))
            {
                string surfaceNumbers = "";
                for (int i = 0; i < surfaceIds.Count; i++)
                {
                    if (i == 0)
                    {
                        surfaceNumbers = surfaceIds[i].ToString();
                    }
                    else
                    {
                        surfaceNumbers += ", " + surfaceIds[i].ToString();
                    }
                }
                
                lineRelease.AxisSystem = LineReleaseAxisSystem.OriginalLineAxisSystem;
                lineRelease.Location = ReleaseLocation.OriginalLocationType;
                lineRelease.No = releaseNo;
                lineRelease.LineNo = lineId;
                lineRelease.ReleasedSurfaces = surfaceNumbers;
                lineRelease.TypeNo = lineReleaseType.No;

                iModel.GetApplication().LockLicense();
                modelData.PrepareModification();
                modelData.SetLineRelease(lineRelease);
                modelData.FinishModification();
                iModel.GetApplication().UnlockLicense();
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5ed9a4c2-6ef7-4af3-bf27-15ca4bcf1d82"); }
        }
    }
}