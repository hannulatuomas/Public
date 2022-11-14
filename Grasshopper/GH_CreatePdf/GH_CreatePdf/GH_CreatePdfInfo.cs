using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace GH_CreatePdf
{
    public class GH_CreatePdfInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "GHCreatePdf";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("834cd8fa-86e5-477f-8e5e-4e45743a4652");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
