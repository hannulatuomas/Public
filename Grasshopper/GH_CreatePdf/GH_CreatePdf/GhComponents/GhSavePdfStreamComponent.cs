using System;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel;
using PdfSharp.Pdf;
using Rhino.Geometry;

namespace GH_CreatePdf.GhComponents
{
    public class GhSavePdfStreamComponent : GH_Component
    {
        PdfDocument document;
        string fileSaved = "Not saved";

        /// <summary>
        /// Initializes a new instance of the SavePdfStreamComponent class.
        /// </summary>
        public GhSavePdfStreamComponent()
          : base("SavePdfStream", "SavePdfStream",
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
            pManager.AddBooleanParameter("Save", "Save", "", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stream", "Stream", "", GH_ParamAccess.item);
            pManager.AddTextParameter("FileSaved", "FileSaved", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool save = false;
            MemoryStream stream = null;

            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            DA.GetData(1, ref save);

            if (save)
            {
                stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;

                fileSaved = "File saved to: MemoryStream at: " + DateTime.Now.ToString();
            }

            DA.SetData(0, stream);
            DA.SetData(1, fileSaved);
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
            get { return new Guid("66974145-dd4c-4023-a934-876dfe9aa1ce"); }
        }
    }
}