using System;
using System.Collections.Generic;
using System.IO;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GH_CreatePdf.GhComponents
{
    public class GhSendEMailComponent : GH_Component
    {
        string status = "Message has not been sent";

        /// <summary>
        /// Initializes a new instance of the GhSendEMailComponent class.
        /// </summary>
        public GhSendEMailComponent()
          : base("SendEMail", "SendEMail",
              "",
              "CreatePdf", "Document")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("toAddress", "toAddress", "", GH_ParamAccess.item);
            pManager.AddTextParameter("subject", "subject", "", GH_ParamAccess.item);
            pManager.AddTextParameter("message", "message", "", GH_ParamAccess.item, "");
            pManager.AddTextParameter("attachmentPath", "attachmentPath", "", GH_ParamAccess.item, "");
            pManager.AddGenericParameter("attachmentStream", "attachmentStream", "", GH_ParamAccess.item);
            pManager.AddTextParameter("streamAttachmentName", "streamAttachmentName", "", GH_ParamAccess.item, "");
            pManager.AddBooleanParameter("SendMessage", "Send", "", GH_ParamAccess.item, false);

            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "Status", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string toAddress = "";
            string subject = "";
            string message = "";
            string attachmentPath = "";
            Stream attachmentStream = null;
            string attachmentName = "";  
            bool send = false;

            if (!DA.GetData(0, ref toAddress)) return;
            if (!(toAddress.Length > 0 && toAddress.Contains("@"))) return;
            if (!DA.GetData(1, ref subject)) return;
            if (!DA.GetData(2, ref message)) return;
            DA.GetData(3, ref attachmentPath);
            DA.GetData(4, ref attachmentStream);
            DA.GetData(5, ref attachmentName);
            DA.GetData(6, ref send);

            if (attachmentName == "")
            {
                attachmentStream = null;
            }

            if (send)
            {
                string fromAddress = "";
                string password = "****";

                EmailManager.SendEmail(fromAddress, password, toAddress, subject, message, attachmentPath, attachmentStream, attachmentName);
                status = "Message has been sent to: " + toAddress + " at: " + DateTime.Now.ToString();
                
                if (attachmentStream != null)
                {
                    attachmentStream.Close();
                }
            }

            DA.SetData(0, status);
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
            get { return new Guid("6864a8d0-c8cd-4859-952b-c8162789d900"); }
        }
    }
}