using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using PdfSharp.Pdf;

namespace GH_CreatePdf.GhComponents
{
    public class SavePdfFileComponent : GH_Component
    {
        PdfDocument document;
        string fileSaved = "Not saved";
        /// <summary>
        /// Initializes a new instance of the SavePdfFileComponent class.
        /// </summary>
        public SavePdfFileComponent()
          : base("SavePdfToFile", "SavePdfToFile",
              "",
              "CreatePdf", "Document")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfDocument", "Document", "", GH_ParamAccess.item);
            pManager.AddTextParameter("FilePath", "Path", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Save", "Save", "", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("FileSaved", "FileSaved", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string path = "";
            bool save = false;

            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            if (!DA.GetData(1, ref path)) return;
            DA.GetData(2, ref save);

            if (save)
            {
                document.Save(path);
                fileSaved = "File saved to: " + path + " at: " + DateTime.Now.ToString();
            }

            DA.SetData(0, fileSaved);
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
            get { return new Guid("037adb74-72c8-45c4-aa5c-76112715cac4"); }
        }
    }
}