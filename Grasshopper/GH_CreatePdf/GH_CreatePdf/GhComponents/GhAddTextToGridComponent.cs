using System;
using System.Collections.Generic;
using System.Drawing;
using GH_CreatePdf.Utility;
using Grasshopper.Kernel;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using Rhino.Geometry;

namespace GH_CreatePdf.GhComponents
{
    public class GhAddTextToGridComponent : GH_Component
    {
        PdfDocument document;
        PdfPage page;
        /// <summary>
        /// Initializes a new instance of the GhAddTextToGridComponent class.
        /// </summary>
        public GhAddTextToGridComponent()
          : base("AddTextToGrid", "TextToGrid",
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
            pManager.AddTextParameter("Text", "Text", "", GH_ParamAccess.list);

            pManager.AddGenericParameter("GridCell", "GridCell", "", GH_ParamAccess.list);

            pManager.AddGenericParameter("Font", "Font", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Alignment", "Alignment", "Paragraph Alignment: 0 = Left, 1 = Right, 2 = Center", GH_ParamAccess.item, 0);

            pManager.AddBooleanParameter("AddText", "AddText", "", GH_ParamAccess.item, false);

            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfPage", "Page", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("PdfDocument", "Document", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<XRect> gridCells = new List<XRect>();
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            List <FontStruct> fontStructList = new List<FontStruct>();
            XFont font = new XFont("Microsoft Sans Serif", 18, XFontStyle.Regular, options);
            Color color = Color.Black;
            
            List<string> textList = new List<string>();
            bool addText = false;
            int alignment = 0;

            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            if (!DA.GetData(1, ref page)) return;
            if (!DA.GetDataList(2, textList)) return;

            if (!DA.GetDataList(3, gridCells)) return;
            DA.GetDataList(4, fontStructList);
            DA.GetData(5, ref alignment);
            DA.GetData(6, ref addText);

            if (addText)
            {
                XBrush brush = XBrushes.Black;
                XGraphics graphics;
                if (page.Tag as XGraphics == null)
                {
                    graphics = XGraphics.FromPdfPage(page);
                    page.Tag = graphics;
                }
                else
                {
                    graphics = (XGraphics)page.Tag;
                }

                XTextFormatter tf = new XTextFormatter(graphics);

                if (alignment == 1)
                {
                    tf.Alignment = XParagraphAlignment.Right;
                }
                else if (alignment == 2)
                {
                    tf.Alignment = XParagraphAlignment.Center;
                }
                else
                {
                    tf.Alignment = XParagraphAlignment.Left;
                }

                for (int i = 0; i < Math.Min(textList.Count, gridCells.Count); i++)
                {
                    if (fontStructList.Count > i)
                    {
                        font = fontStructList[i].font;
                        color = fontStructList[i].color;
                        brush = new XSolidBrush(XColor.FromArgb(color.A, color.R, color.G, color.B));
                    }
                    tf.DrawString(textList[i], font, brush, gridCells[i]);
                }
            }

            DA.SetData(0, page);
            DA.SetData(1, document);
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
            get { return new Guid("33034066-73ab-4d5a-a72c-c12ec35b723b"); }
        }
    }
}