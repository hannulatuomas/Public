using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace GH_GetRFemResults
{
    public class GH_GetRFemResultsInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "GetRFemResults";
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
                return new Guid("07f58da9-9e63-4111-856f-6e3657b3d5f4");
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
