using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Rhino.Geometry;

namespace GH_CreatePdf.GhComponents
{
    public class GhGridLayoutComponent : GH_Component
    {
        const float pt = 0.3528f;
        /// <summary>
        /// Initializes a new instance of the GhGridLayoutComponent class.
        /// </summary>
        public GhGridLayoutComponent()
          : base("GridLayout", "GridLayout",
              "",
              "CreatePdf", "Document")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfPage", "Page", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rows", "Rows", "", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Columns", "Columns", "", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("TopMargin", "TopMargin", "[mm]", GH_ParamAccess.item, 10f);
            pManager.AddNumberParameter("LeftMargin", "LeftMargin", "[mm]", GH_ParamAccess.item, 17.5f);
            pManager.AddNumberParameter("BottomMargin", "BottomMargin", "[mm]", GH_ParamAccess.item, 7.5f);
            pManager.AddNumberParameter("RightMargin", "RightMargin", "[mm]", GH_ParamAccess.item, 17.5f);
            pManager.AddNumberParameter("GridPadding", "GridPadding", "[mm]", GH_ParamAccess.item, 2f);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddGenericParameter("PdfPage", "Page", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grid", "Grid", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PdfPage page = null;
            double fRows = 0;
            double fColumns = 0;
            double[] margins = new double[4];
            double padding = 0;

            List<XRect> flatGrid = new List<XRect>();

            if (!DA.GetData(0, ref page)) return;
            if (page == null) return;
            DA.GetData(1, ref fRows);
            DA.GetData(2, ref fColumns);
            DA.GetData(3, ref margins[0]);
            DA.GetData(4, ref margins[1]);
            DA.GetData(5, ref margins[2]);
            DA.GetData(6, ref margins[3]);
            DA.GetData(7, ref padding);

            int rows = (int)fRows;
            int columns = (int)fColumns;

            float xPage = (float)page.Width.Millimeter / pt;
            float yPage = (float)page.Height.Millimeter / pt;
            float top = (float)margins[0] / pt;
            float left = (float)margins[1] / pt;
            float bottom = (float)margins[2] / pt;
            float right = (float)margins[3] / pt;
            padding = padding / pt;

            float cellWidth = (xPage - (left + right)) / columns;
            float cellHeight = (yPage - (top + bottom)) / rows;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    float xCell = (float)((left + padding) + x * cellWidth);
                    float yCell = (float)((top + padding) + y * cellHeight);

                    XRect rect = new XRect(xCell, yCell, cellWidth - 2*padding, cellHeight - 2*padding);

                    flatGrid.Add(rect);
                }
            }

            //DA.SetData(0, page);
            DA.SetDataList(0, flatGrid);
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
            get { return new Guid("f0a92c1f-51a7-49ad-8f77-4bbdba36a9c1"); }
        }
    }
}