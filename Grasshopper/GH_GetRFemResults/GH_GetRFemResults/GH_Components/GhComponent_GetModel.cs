using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_GUI;
using System.Windows.Forms;
using Dlubal.RFEM5;
using System.Runtime.InteropServices;
using GH_GetRFemResults.RFem;

namespace GH_GetRFemResults.GH_Components
{
    public class GhComponent_GetModel : GH_ExtendableComponent
    {
        IApplication app;
        MenuCheckBox checkBox;
        MenuStaticText menuStaticText;
        MenuDropDown modelsDropDown;
        IModel selectedModel;
        bool wait = true;
        bool run = false;
        string componentName = "GetModel";

        /// <summary>
        /// Initializes a new instance of the GetModel class.
        /// </summary>
        public GhComponent_GetModel()
          : base("GetModel", "GetModel",
              "",
              "RFemResults", "ModelData")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //pManager.AddBooleanParameter("Run", "Run", "", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("IModel", "Model", "", GH_ParamAccess.item);
        }

        protected override void Setup(GH_ExtendableComponentAttributes attr)
        {
            float componentWidth = 120f;
            int menuHeight = 50;
            float sideMarginal = 5f;
            float topMarginal = 20f;

            GH_MenuPanel gH_MenuPanel = new GH_MenuPanel();
            gH_MenuPanel.Pivot = new System.Drawing.PointF(sideMarginal, topMarginal);
            gH_MenuPanel.Height = menuHeight;
            gH_MenuPanel.Width = componentWidth - 2 * sideMarginal;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu();
            gH_ExtendableMenu.Expand();
            gH_ExtendableMenu.Name = "Run Component";
            gH_ExtendableMenu.Height = gH_MenuPanel.Height + 2 * sideMarginal;
            gH_ExtendableMenu.Header = "Run Component";

            this.checkBox = new MenuCheckBox(20, 15);
            checkBox._valueChanged += new ValueChangeEventHandler(this.checkBox_valueChanged);
            checkBox.Pivot = new System.Drawing.PointF(sideMarginal, sideMarginal);
            checkBox.Name = "Run";
            checkBox.Header = "Run Component";

            menuStaticText = new MenuStaticText();
            menuStaticText.Pivot = new System.Drawing.PointF(checkBox.Pivot.X + 30f, checkBox.Pivot.Y);
            menuStaticText.Text = "Click to run";
            menuStaticText.Header = "Click to run component";

            modelsDropDown = new MenuDropDown("Models");
            modelsDropDown.Pivot = new System.Drawing.PointF(checkBox.Pivot.X, checkBox.Pivot.Y + 20f);
            modelsDropDown._valueChanged += new ValueChangeEventHandler(this.dropDown_valueChanged);
            modelsDropDown.Width = gH_MenuPanel.Width - 2 * sideMarginal;
            modelsDropDown.Header = "Select open model";
            modelsDropDown.addItem("No Models");
            modelsDropDown.VisibleItemCount = Math.Min(10, modelsDropDown.Items.Count);

            gH_MenuPanel.AddControl(checkBox);
            gH_MenuPanel.AddControl(menuStaticText);
            gH_MenuPanel.AddControl(modelsDropDown);
            gH_ExtendableMenu.AddControl(gH_MenuPanel);
            attr.AddMenu(gH_ExtendableMenu);
            attr.MinWidth = componentWidth;
        }

        private void dropDown_valueChanged(object sender, EventArgs e)
        {
            int value = ((MenuDropDown)sender).Value;
            string name = ((MenuDropDown)sender).Items[value].content;
            this.selectedModel = SelectModel(name);
            this.ExpireSolution(true);
        }

        private void checkBox_valueChanged(object sender, EventArgs e)
        {
            bool value = ((MenuCheckBox)sender).Active;
            if (value && !run)
            {
                this.run = true;
                menuStaticText.Text = "Running...";
                this.OnObjectChanged(GH_ObjectEventType.Options);
                wait = true;
                this.ExpireSolution(true);
            }
            else if (value && run)
            {
                checkBox.Active = false;
                run = false;
                menuStaticText.Text = "Click to run";
                this.OnObjectChanged(GH_ObjectEventType.Options);
            }
        }

        void UpdateDropDownMenu(MenuDropDown dropDownMenu, List<string> values)
        {
            dropDownMenu.VisibleItemCount = Math.Min(10, values.Count);
            int index = 0;
            if (values.Count == dropDownMenu.Items.Count)
            {
                index = dropDownMenu.Value;
            }
            dropDownMenu.clear();
            if (values.Count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    dropDownMenu.addItem(values[i]);
                }
                if (dropDownMenu.Items.Count > index)
                {
                    dropDownMenu.Value = index;
                }
            }
            dropDown_valueChanged(dropDownMenu, EventArgs.Empty);
        }

        private List<string> GetOpenModels()
        {
            app = Marshal.GetActiveObject("RFEM5.Application") as IApplication;
            app.LockLicense();

            int modelCount = app.GetModelCount();
            List<string> modelNames = new List<string>();

            for (int i = 0; i < modelCount; i++)
            {
                IModel model = app.GetModel(i);
                string modelName = model.GetName();
                modelNames.Add(modelName);
            }

            app.UnlockLicense();
            return modelNames;
        }

        private IModel SelectModel(string name)
        {
            //IApplication app = Marshal.GetActiveObject("RFEM5.Application") as IApplication;
            app.LockLicense();
            IModel iModel = null;

            int modelCount = app.GetModelCount();

            for (int i = 0; i < modelCount; i++)
            {
                iModel = app.GetModel(i);
                string modelName = iModel.GetName();
                if (modelName == name)
                {
                    app.UnlockLicense();
                    return iModel;
                }
            }
            iModel = app.GetModel(0);

            app.UnlockLicense();
            return iModel;
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (wait)
            {
                wait = false;
                this.ExpireSolution(true);
                return;
            }
            else
            {
                wait = true;
            }
            if (run)
            {
                List<string> modelNames = GetOpenModels();
                UpdateDropDownMenu(modelsDropDown, modelNames);
            }
            checkBox_valueChanged(checkBox, EventArgs.Empty);

            RfemModel model = new RfemModel(selectedModel);

            DA.SetData(0, model);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Resource1.RFemModelIcon.ToBitmap();
                //return null;
            }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cdf34e4b-4ae7-4dad-97dd-d81d2c16980d"); }
        }
    }
}