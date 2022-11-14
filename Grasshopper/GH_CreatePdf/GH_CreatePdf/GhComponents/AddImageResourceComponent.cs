using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Grasshopper.Kernel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Rhino.Geometry;

namespace GH_CreatePdf.GhComponents
{
    public class AddImageResourceComponent : GH_Component
    {
        const float pt = 0.3528f;
        PdfDocument document;
        List<PdfPage> pages;

        /// <summary>
        /// Initializes a new instance of the AddImageResourceComponent class.
        /// </summary>
        public AddImageResourceComponent()
          : base("AddImageResource", "AddImageResource",
              "Adds a predefined image to the list of pages",
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
            pManager.AddGenericParameter("PdfPage", "Page", "", GH_ParamAccess.list);
            pManager.AddPointParameter("Point", "Point", "Insertion point for the Image [mm]. \nZ value does not matter. \nPoint [0,0] = upper left corner of the page and point [PageWidth,PageHeight] = lower right corner of the page.", GH_ParamAccess.item, new Point3d(17.5f, 10f, 0));

            pManager.AddIntegerParameter("Resource", "Resource", "", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("AddImage", "AddImage", "", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("PdfPage", "Page", "", GH_ParamAccess.list);
            pManager.AddGenericParameter("PdfDocument", "Document", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d point = new Point3d();
            bool addImage = false;
            int resourceIndex = 0;
            pages.Clear();

            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            if (!DA.GetDataList(1, pages)) return;
            if (!DA.GetData(2, ref point)) return;
            DA.GetData(3, ref resourceIndex);
            DA.GetData(4, ref addImage);

            if (addImage)
            {
                foreach (PdfPage page in pages)
                {
                    XImage xImage = null;
                    int width = (int)(page.Width.Millimeter / pt);
                    width = (int)(10 / pt);
                    int height = (int)Math.Ceiling(page.Height.Millimeter / pt);

                    Image bm = new Bitmap(width, height);
                    Graphics gr = Graphics.FromImage(bm);
                    Color color = Color.FromArgb(29, 55, 84);
                    gr.FillRectangle(new SolidBrush(color), 0, 0, bm.Width, bm.Height);
                    ImageFormat format = ImageFormat.Png;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        bm.Save(stream, format);
                        stream.Position = 0;
                        xImage = XImage.FromStream(stream);
                    }

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

                    graphics.DrawImage(xImage, point.X / pt, point.Y / pt, bm.Width, bm.Height);
                }
            }

            DA.SetDataList(0, pages);
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
            get { return new Guid("e3653909-8fc0-4997-b5c7-cdb744be70ce"); }
        }
    }
}