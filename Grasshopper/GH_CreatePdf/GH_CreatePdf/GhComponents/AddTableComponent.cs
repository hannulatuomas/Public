using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Grasshopper;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using System.Linq;

namespace GH_CreatePdf.GhComponents
{
    public class AddTableComponent : GH_Component
    {
        const float pt = 0.3528f;
        PdfDocument document;
        PdfPage page;
        XRect[] rectangles = null;

        /// <summary>
        /// Initializes a new instance of the AddTableComponent class.
        /// </summary>
        public AddTableComponent()
          : base("AddTable", "AddTable",
              "",
              "CreatePdf", "AddContent")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfDocument", "Document", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("PdfPage", "Page", "", GH_ParamAccess.item);
            
            pManager.AddPointParameter("Point", "Point", "Insertion point for the Text [mm]. \nZ value does not matter. \nPoint [0, 0] = upper left corner of the page and point [PageWidth, PageHeight] = lower right corner of the page.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Rows", "Rows", "", GH_ParamAccess.item, 1);
            pManager.AddIntegerParameter("Columns", "Columns", "", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("TableHeight", "Height", "", GH_ParamAccess.item, 150);
            pManager.AddNumberParameter("TableWidth", "Width", "", GH_ParamAccess.item, 150);

            pManager.AddGenericParameter("Font", "Font", "", GH_ParamAccess.item);

            pManager.AddBooleanParameter("AddTable", "AddTable", "", GH_ParamAccess.item, false);

            

            pManager[7].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfPage", "Page", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("PdfDocument", "Document", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("TableCells", "Cells", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d point = new Point3d();
            bool addTable = false;
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            XFont font = new XFont("Microsoft Sans Serif", 18, XFontStyle.Regular, options);

            int rows = 1;
            int columns = 1;
            double height = 150;
            double width = 150;


            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            if (!DA.GetData(1, ref page)) return;
            if (!DA.GetData(2, ref point)) return;
            DA.GetData(3, ref rows);
            DA.GetData(4, ref columns);
            DA.GetData(5, ref height);
            DA.GetData(6, ref width);

            DA.GetData(7, ref font);
            DA.GetData(8, ref addTable);
            
            

            if (addTable)
            {
                XPoint xPoint = new XPoint(point.X / pt, point.Y / pt);
                XGraphics graphics;
                XPen pen = XPens.Black;

                if (rows * columns < 1) return;
                rectangles = new XRect[rows * columns];
                
                for (int i = 0; i < rows; i++)
                {
                    double y = point.Y / pt + i * ((height / pt) / rows);

                    for (int j = 0; j < columns; j++)
                    {
                        double x = point.X / pt + j * ((width / pt) / columns);
                        rectangles[j + (columns * i)] = new XRect(x, y, ((width / pt) / columns), ((height / pt) / rows));
                    }
                }

                if (page.Tag as XGraphics == null)
                {
                    graphics = XGraphics.FromPdfPage(page);
                    page.Tag = graphics;
                }
                else
                {
                    graphics = (XGraphics)page.Tag;
                }

                //graphics.DrawString(text, font, XBrushes.Black, xPoint);
                graphics.DrawRectangles(pen, rectangles);
            }

            DA.SetData(0, page);
            DA.SetData(1, document);
            if(rectangles != null && rectangles.Length > 0) DA.SetDataList(2, rectangles.ToList());
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
            get { return new Guid("cfef45e5-9f8b-44a5-8b56-97b7c77ddfbd"); }
        }
    }
}