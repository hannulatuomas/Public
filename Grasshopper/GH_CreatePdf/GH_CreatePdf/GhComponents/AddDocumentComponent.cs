using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using PdfSharp.Pdf;

namespace GH_CreatePdf.GhComponents
{
    public class AddDocumentComponent : GH_Component
    {
        PdfDocument document;
        /// <summary>
        /// Initializes a new instance of the AddDocumentComponent class.
        /// </summary>
        public AddDocumentComponent()
          : base("AddPdfDocument", "PdfDocument",
              "",
              "CreatePdf", "Document")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Title", "Title", "", GH_ParamAccess.item);
            pManager.AddTextParameter("Author", "Author", "", GH_ParamAccess.item);
            pManager.AddTextParameter("Subject", "Subject", "", GH_ParamAccess.item);
            pManager.AddTextParameter("Keywords", "Keywords", "", GH_ParamAccess.list);

            pManager.AddBooleanParameter("Clear", "Clear", "", GH_ParamAccess.item, false);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfDocument", "Document", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string title = "";
            string author = "";
            string subject = "";
            string keyword = "";
            List<string> keywords = new List<string>();
            document = new PdfDocument();

            bool clear = false;

            if (DA.GetData(0, ref title)) document.Info.Title = title;
            if (DA.GetData(1, ref author)) document.Info.Author = author;
            if (DA.GetData(2, ref subject)) document.Info.Subject = subject;
            if (DA.GetDataList(3, keywords))
            {
                for (int i = 0; i < keywords.Count - 1; i++)
                {
                    keyword += keywords[i] + ", ";
                }
                keyword += keywords[keywords.Count - 1];
                document.Info.Keywords = keyword;
            }

            DA.GetData(4, ref clear);

            if (clear)
            {
                document = new PdfDocument();
            }
            
            DA.SetData(0, document);
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
            get { return new Guid("571ea5c2-3966-48d3-8bd9-f813eef7004f"); }
        }
    }
}