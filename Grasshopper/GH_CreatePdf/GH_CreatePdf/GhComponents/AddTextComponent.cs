using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Text;
using System.Globalization;
using System.Drawing;
using GH_CreatePdf.Utility;

namespace GH_CreatePdf.GhComponents
{
    public class AddTextComponent : GH_Component
    {
        const float pt = 0.3528f;
        PdfDocument document;
        PdfPage page;

        /// <summary>
        /// Initializes a new instance of the AddTextComponent class.
        /// </summary>
        public AddTextComponent()
          : base("AddText", "AddText",
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
            pManager.AddPointParameter("Point", "Point", "Insertion point for the Text [mm]. \nZ value does not matter. \nPoint [0, 0] = upper left corner of the page and point [PageWidth, PageHeight] = lower right corner of the page.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Font", "Font", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Alignment", "Alignment", "Paragraph Alignment: 0 = Left, 1 = Right", GH_ParamAccess.item, 0);

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
            List<string> textList = new List<string>();
            List<Point3d> pointList = new List<Point3d>();
            bool addText = false;
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            FontStruct fontStruct = new FontStruct();
            XFont font = new XFont("Microsoft Sans Serif", 18, XFontStyle.Regular, options);
            Color color = Color.Black;
            int alignment = 0;

            if (!DA.GetData(0, ref document)) return;
            if (document == null) return;
            if (!DA.GetData(1, ref page)) return;
            if (!DA.GetDataList(2, textList)) return;
            if (!DA.GetDataList(3, pointList)) return;
            if(DA.GetData(4, ref fontStruct))
            {
                font = fontStruct.font;
                color = fontStruct.color;
            }
            DA.GetData(5, ref alignment);
            DA.GetData(6, ref addText);

            if (addText)
            {
                XBrush brush = new XSolidBrush(XColor.FromArgb(color.A, color.R, color.G, color.B));
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

                for (int i = 0; i < textList.Count; i++)
                {
                    string text = textList[i];
                    XPoint xPoint = new XPoint();

                    if (pointList.Count > i)
                    {
                        xPoint = new XPoint(pointList[i].X / pt, pointList[i].Y / pt);
                    }

                    if (text.Contains("0x") || text.Contains("0X"))
                    {
                        string[] words = text.Split(' ');
                        string newText = "";

                        foreach (string word in words)
                        {
                            if (word.StartsWith("0x") || word.StartsWith("0X"))
                            {
                                int length = word.Length;

                                if (length == 6)
                                {
                                    StringBuilder sb = new StringBuilder(length);
                                    string temp = "";

                                    temp = word.Substring(2);

                                    byte[] bytes = new byte[2];
                                    bytes[1] = byte.Parse(int.Parse(temp.Substring(0, 2), NumberStyles.HexNumber).ToString());
                                    bytes[0] = byte.Parse(int.Parse(temp.Substring(2, 2), NumberStyles.HexNumber).ToString());
                                    sb.Append(Encoding.Unicode.GetString(bytes));

                                    newText += sb.ToString() + " ";
                                }
                                else
                                {
                                    newText += word + " ";
                                }
                            }
                            else
                            {
                                newText += word + " ";
                            }
                        }

                        text = newText;
                        textList[i] = text;
                    }

                    XStringFormat format;

                    if (alignment == 1)
                    {
                        format = XStringFormats.BaseLineRight;
                    }
                    else
                    {
                        format = XStringFormats.BaseLineLeft;
                    }

                    graphics.DrawString(text, font, brush, xPoint, format);
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
            get { return new Guid("d42904e8-d7c7-4de0-a201-7d0f266749c7"); }
        }
    }
}