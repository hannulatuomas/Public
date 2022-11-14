using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dlubal.RFEM5;
using GH_GetRFemResults.GH;
using GH_GetRFemResults.RFem;
using GH_GetRFemResults.Utilities;
using GH_GUI;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace GH_GetRFemResults.GH_Components
{
    public class GhComponent_GetSupportReactions : GH_SwitcherComponent
    {
        private List<SubComponent> subcomponents = new List<SubComponent>();
        public override string UnitMenuName => "Type of Component";
        protected override string DefaultEvaluationUnit => subcomponents[0].name();

        public IModel iModel;
        public List<LoadingCase> loadingCase = new List<LoadingCase>();
        MenuCheckBox checkBox;
        MenuStaticText menuStaticText;
        bool wait = true;
        bool run_sub = false;
        bool run_main = false;
        public IResults iResults;
        public string objectList;

        /// <summary>
        /// Initializes a new instance of the GhComponent_GetSupportReactions class.
        /// </summary>
        public GhComponent_GetSupportReactions()
          : base("GhComponent_GetSupportReactions", "GetSupportReactions",
              "Get support reactions",
              "RFemResults", "Results")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("LoadingCase", "LoadingCase", "", GH_ParamAccess.list);
            pManager.AddTextParameter("objectList", "objectList", "", GH_ParamAccess.item, "");
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void Setup(GH_SwitcherComponentAttributes attr)
        {
            float componentWidth = 120f;
            int menuHeight = 50;
            float sideMarginal = 5f;
            float topMarginal = 20f;

            GH_MenuPanel gH_MenuPanel_run = new GH_MenuPanel();
            gH_MenuPanel_run.Pivot = new System.Drawing.PointF(sideMarginal, topMarginal);
            gH_MenuPanel_run.Height = menuHeight;
            gH_MenuPanel_run.Width = componentWidth - 2 * sideMarginal;
            gH_MenuPanel_run.AdjustWidth = true;

            GH_ExtendableMenu gH_ExtendableMenu_run = new GH_ExtendableMenu();
            gH_ExtendableMenu_run.Expand();
            gH_ExtendableMenu_run.Name = "Run Component";
            gH_ExtendableMenu_run.Height = gH_MenuPanel_run.Height + 2 * sideMarginal;
            gH_ExtendableMenu_run.Header = "Run Component";

            this.checkBox = new MenuCheckBox(20, 15);
            checkBox._valueChanged += new ValueChangeEventHandler(this.checkBox_run_valueChanged);
            checkBox.Pivot = new System.Drawing.PointF(sideMarginal, sideMarginal);
            checkBox.Name = "Run";
            checkBox.Header = "Run Component";

            menuStaticText = new MenuStaticText();
            menuStaticText.Pivot = new System.Drawing.PointF(checkBox.Pivot.X + 30f, checkBox.Pivot.Y);
            menuStaticText.Text = "Click to run";
            menuStaticText.Header = "Click to run component";

            gH_MenuPanel_run.AddControl(checkBox);
            gH_MenuPanel_run.AddControl(menuStaticText);
            gH_ExtendableMenu_run.AddControl(gH_MenuPanel_run);
            attr.AddMenu(gH_ExtendableMenu_run);
            attr.MinWidth = componentWidth;

            base.Setup(attr);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            subcomponents.Add(new GhSubComponent_GetNodalSupportReactions(this));
            subcomponents.Add(new GhSubComponent_GetLineSupportReactions(this));

            foreach (SubComponent item in subcomponents)
            {
                item.registerEvaluationUnits(mngr);
            }
        }

        private void checkBox_run_valueChanged(object sender, EventArgs e)
        {
            bool value = ((MenuCheckBox)sender).Active;

            if (value && !run_sub)
            {
                this.run_sub = true;
                menuStaticText.Text = "Running...";
                this.OnObjectChanged(GH_ObjectEventType.Options);
                wait = true;
                run_main = true;
                this.ExpireSolution(true);
            }
            else if (value && run_sub)
            {
                checkBox.Active = false;
                run_sub = false;
                menuStaticText.Text = "Click to run";
                this.OnObjectChanged(GH_ObjectEventType.Options);
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit _unit)
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

            if (run_sub && iModel != null)
            {
                // Run SubComponents
                if (_unit != null)
                {
                    try
                    {
                        foreach (SubComponent item in subcomponents)
                        {
                            if (_unit.Name.Equals(item.name()))
                            {
                                item.SolveInstance(DA, out string msg, out GH_RuntimeMessageLevel level);
                                if (msg != "")
                                {
                                    this.AddRuntimeMessage(level, msg + "It may cause errors.");
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Invalid sub-component");
                        return;
                    }
                }
            }
            checkBox_run_valueChanged(checkBox, EventArgs.Empty);

            loadingCase.Clear();

            if (!DA.GetDataList<LoadingCase>(0, loadingCase)) return;
            DA.GetData(1, ref objectList);

            if (loadingCase.Count > 0)
            {
                this.iModel = loadingCase[0].model;
            }
            else
            {
                MessageBox.Show("Invalid LoadingCase");
                return;
            }
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

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c71a3d0d-ddfe-4ef7-8346-ddf1859adb8d"); }
        }
    }
}