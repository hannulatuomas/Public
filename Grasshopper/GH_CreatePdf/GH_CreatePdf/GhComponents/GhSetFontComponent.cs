using System;
using System.Collections.Generic;
using System.Drawing;
using GH_CreatePdf.Utility;
using Grasshopper.Kernel;
//using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;
using Rhino.Geometry;

namespace GH_CreatePdf.GhComponents
{
    public class GhSetFontComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhSetFontComponent class.
        /// </summary>
        public GhSetFontComponent()
          : base("SetFont", "Font",
              "",
              "CreatePdf", "Document")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("FontName", "Name", "", GH_ParamAccess.item, "Microsoft Sans Serif");
            pManager.AddNumberParameter("Size", "Size", "[pt]", GH_ParamAccess.item, 18);
            pManager.AddNumberParameter("Style", "Style", "0 = Regular, 1= Bold, 2 = Italic, 4 = Underline, 8 = Strikeout", GH_ParamAccess.item, 0);
            pManager.AddColourParameter("Color", "Color", "Font color", GH_ParamAccess.item, Color.Black);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Font", "Font", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "Microsoft Sans Serif";
            double size = 18.0;
            double styleNum = 0;
            Color color = Color.Black;

            DA.GetData(0, ref name);
            DA.GetData(1, ref size);
            DA.GetData(2, ref styleNum);
            DA.GetData(3, ref color);

            XFontStyle style;

            if ((int)styleNum == 5)
            {
                style = XFontStyle.Bold | XFontStyle.Underline;
            }
            else
            {
                style = (XFontStyle)((int)styleNum);
            }                    
            
            XFont font = new XFont(name, size, style);
            
            FontStruct fontStruct = new FontStruct(font, color);

            DA.SetData(0, fontStruct);
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
            get { return new Guid("23c28a54-c7ab-4c45-8301-9c9443a42d1a"); }
        }
    }
}