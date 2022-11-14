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
    public class AddImageStreamComponent : GH_Component
    {
        const float pt = 0.3528f;
        PdfDocument document;
        PdfPage page;

        /// <summary>
        /// Initializes a new instance of the AddImageStreamComponent class.
        /// </summary>
        public AddImageStreamComponent()
          : base("AddImageStream", "AddImageStream",
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
            pManager.AddGenericParameter("ImageStream", "ImageStream", "", GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "Point", "Insertion point for the Image [mm]. \nZ value does not matter. \nPoint [0,0] = upper left corner of the page and point [PageWidth,PageHeight] = lower right corner of the page.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "Width", "", GH_ParamAccess.item, 0f);
            pManager.AddNumberParameter("Height", "Height", "", GH_ParamAccess.item, 0f);

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
            MemoryStream imageStream = null;
            Point3d point = new Point3d();
            bool addImage = false;
            double imageWidth = 0f;
            double imageHeight = 0f;

            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            if (!DA.GetData(1, ref page)) return;
            if (!DA.GetData(2, ref imageStream)) imageStream = null;
            if (!DA.GetData(3, ref point)) return;
            DA.GetData(4, ref imageWidth);
            DA.GetData(5, ref imageHeight);

            DA.GetData(6, ref addImage);

            imageWidth = imageWidth / pt;
            imageHeight = imageHeight / pt;

            if (addImage && imageStream != null)
            {
                if (imageStream.Length > 0)
                {
                    XImage xImage = XImage.FromStream(imageStream);
                    imageStream.Close();

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

                    if (imageWidth == 0f)
                    {
                        imageWidth = xImage.PointWidth;
                    }

                    if (imageHeight == 0f)
                    {
                        imageHeight = imageWidth * (xImage.PointHeight / xImage.PointWidth);
                    }

                    graphics.DrawImage(xImage, xPoint.X, xPoint.Y, imageWidth, imageHeight);
                }
                else
                {
                    imageStream.Close();
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
            get { return new Guid("869c337a-cada-4924-bfdd-b80b494e1364"); }
        }
    }
}