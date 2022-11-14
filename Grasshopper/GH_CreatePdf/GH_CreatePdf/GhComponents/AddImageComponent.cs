using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Drawing;
using System.Drawing.Imaging;

namespace GH_CreatePdf.GhComponents
{
    public class AddImageComponent : GH_Component
    {
        const float pt = 0.3528f;
        PdfDocument document;
        PdfPage page;

        /// <summary>
        /// Initializes a new instance of the AddImageComponent class.
        /// </summary>
        public AddImageComponent()
          : base("AddImageFromFile", "AddImageFromFile",
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
            pManager.AddTextParameter("ImageFilePath", "ImageFilePath", "", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "Point", "Insertion point for the Image [mm]. \nZ value does not matter. \nPoint [0,0] = upper left corner of the page and point [PageWidth,PageHeight] = lower right corner of the page.", GH_ParamAccess.item);

            pManager.AddBooleanParameter("AddImage", "AddImage", "", GH_ParamAccess.item, false);
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
            string imageFile = "";         
            Point3d point = new Point3d();
            bool addImage = false;

            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            if (!DA.GetData(1, ref page)) return;
            if (!DA.GetData(2, ref imageFile)) return;
            if (!DA.GetData(3, ref point)) return;
            DA.GetData(4, ref addImage);

            if (addImage)
            {
                if (!File.Exists(imageFile)) return;
                Image image = Image.FromFile(imageFile);
                ImageFormat format = image.RawFormat;
                XImage xImage = null;

                using (MemoryStream stream = new MemoryStream())
                {
                    image.Save(stream, format);
                    stream.Position = 0;
                    xImage = XImage.FromStream(stream);
                }

                XPoint xPoint = new XPoint(point.X / pt, point.Y / pt);
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

                graphics.DrawImage(xImage, xPoint);
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
            get { return new Guid("a522f2ed-f8aa-4c3e-a53c-c2f17aed5dbb"); }
        }
    }
}