using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using PdfSharp.Pdf;

namespace GH_CreatePdf.GhComponents
{
    public class AddPageComponent : GH_Component
    {
        PdfDocument document;
        List<PdfPage> pages;
        /// <summary>
        /// Initializes a new instance of the AddPageComponent class.
        /// </summary>
        public AddPageComponent()
          : base("AddPdfPages", "PdfPages",
              "",
              "CreatePdf", "AddContent")
        {
            pages = new List<PdfPage>();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfDocument", "Document", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Count", "Count", "", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("PageSize", "PageSize", "A0 = 1, A1 = 2, A2 = 3, A3 = 4, A4 = 5, A5 = 6,", GH_ParamAccess.item, 5);
            pManager.AddNumberParameter("Width", "Width", "Owerrides PageSide", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "Height", "Owerrides PageSide", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation", "Orientation", "Page orientation. Portrait = 0, Landscape = 1", GH_ParamAccess.item, 0);

            pManager.AddBooleanParameter("AddPages", "AddPages", "", GH_ParamAccess.item, false);
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfPages", "Pages", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("PdfDocument", "Document", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("PageWidth", "PageWidth", "[mm]", GH_ParamAccess.list);
            pManager.AddNumberParameter("PageHeight", "PageHeight", "[mm]", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double count = 1;
            double pageSizeNum = 5;
            PdfSharp.PageSize pageSize;
            double width = 0;
            double height = 0;
            double orientation = 0;
            List<double> pageWidthList = new List<double>();
            List<double> pageHeightList = new List<double>();
            bool addPages = false;

            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            DA.GetData(1, ref count);
            DA.GetData(2, ref pageSizeNum);
            DA.GetData(3, ref width);
            DA.GetData(4, ref height);
            DA.GetData(5, ref orientation);
            DA.GetData(6, ref addPages);

            if (addPages)
            {
                pages.Clear();

                for (int i = 0; i < (int)count; i++)
                {
                    PdfPage _page = document.AddPage();

                    if (width > 0 && height > 0)
                    {
                        _page.Width = width;
                        _page.Height = height;
                    }
                    else
                    {
                        pageSize = (PdfSharp.PageSize)((int)pageSizeNum);
                        _page.Size = pageSize;
                    }

                    _page.Orientation = (PdfSharp.PageOrientation)((int)orientation);
                    pageWidthList.Add( _page.Width.Millimeter);
                    pageHeightList.Add(_page.Height.Millimeter);

                    pages.Add(_page);
                }
            }
            
            DA.SetDataList(0, pages);
            DA.SetData(1, document);
            DA.SetDataList(2, pageWidthList);
            DA.SetDataList(3, pageHeightList);
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
            get { return new Guid("2526177c-4a49-4ed3-a6e7-d1aba1fa39e2"); }
        }
    }
}