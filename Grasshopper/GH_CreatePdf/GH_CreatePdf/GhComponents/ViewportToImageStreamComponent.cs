using System;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Display;
using Rhino.Render;
using System.Drawing;
using System.Drawing.Imaging;

namespace GH_CreatePdf.GhComponents
{
    public class ViewportToImageStreamComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ViewportToImageStreamComponent class.
        /// </summary>
        public ViewportToImageStreamComponent()
          : base("ViewportToImageStream", "ViewportToStream",
              "",
              "CreatePdf", "AddContent")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Save", "Save", "", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ImageStream", "ImageStream", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            MemoryStream stream = null;
            bool save = false;

            DA.GetData(0, ref save);

            if (save)
            {
                Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;
                RhinoView view = doc.Views.ActiveView;
                RhinoViewport viewport = view.ActiveViewport;

                //ViewCaptureSettings settings = new ViewCaptureSettings();
                //settings.Document = doc;
                //settings.SetViewport(viewport);
                //settings.DrawSelectedObjectsOnly = false;
                //settings.SetModelScaleToFit(false);

                ViewCapture viewCapture = new ViewCapture();
                viewCapture.Width = viewport.Size.Width;
                viewCapture.Height = viewport.Size.Height;
                viewCapture.TransparentBackground = true;

                Bitmap bitmap = viewCapture.CaptureToBitmap(view);
                //bitmap = ViewCapture.CaptureToBitmap(settings);

                if (bitmap != null)
                {
                    stream = new MemoryStream();
                    bitmap.Save(stream, ImageFormat.Png);
                }
            }

            DA.SetData(0, stream);
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
            get { return new Guid("91b899e3-05a3-46a8-8f13-44bd9d03a859"); }
        }
    }
}