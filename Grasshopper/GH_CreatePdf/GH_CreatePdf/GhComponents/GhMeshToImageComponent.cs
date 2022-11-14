using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;

namespace GH_CreatePdf.GhComponents
{
    public class GhMeshToImageComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhMeshToImageComponent class.
        /// </summary>
        public GhMeshToImageComponent()
          : base("MeshToImage", "MeshToImage",
              "",
              "CreatePdf", "AddContent")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Width", "Width", "", GH_ParamAccess.item, 512);
            //pManager.AddIntegerParameter("Height", "Height", "", GH_ParamAccess.item, 512);
            pManager.AddMeshParameter("Mesh", "Mesh", "", GH_ParamAccess.list);
            pManager.AddMeshParameter("Mesh", "Mesh", "", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Save", "Save", "", GH_ParamAccess.item, false);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ImageStream", "Stream", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Mesh> meshList_1 = new List<Mesh>();
            List<Mesh> meshList_2 = new List<Mesh>();
            List<Mesh> meshList_3 = new List<Mesh>();
            Image image = null;
            int width = 512;
            int height = 512;
            MemoryStream stream = null;
            bool save = false;

            if (!DA.GetDataList(0, meshList_1)) return;
            DA.GetData(1, ref width);
            //DA.GetData(2, ref height);
            if (!DA.GetDataList(2, meshList_2)) meshList_2 = null;
            if (!DA.GetDataList(3, meshList_3)) meshList_3 = null;
            DA.GetData(4, ref save);

            image = new Bitmap(width, height);

            if (save)
            {
                image = Utility.MeshToImage.ConstructImageFromMeshList(width, meshList_1, meshList_2, meshList_3);

                if (image != null)
                {
                    stream = new MemoryStream();
                    image.Save(stream, ImageFormat.Png);
                }
            }

            DA.SetData(0, stream);
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
            get { return new Guid("7c0ea1d2-d7a4-4e0d-afa5-98a7de877224"); }
        }
    }
}