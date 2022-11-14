using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using GH_GUI;
using System.Windows.Forms;
using Grasshopper.Kernel.Parameters;

namespace GH_GetRFemResults.GH_Components
{
    public class GhComponent_ShowSurfaceResult : GH_SwitcherComponent //IGH_VariableParameterComponent
    {
        private List<SubComponent> subcomponents = new List<SubComponent>();
        public override string UnitMenuName => "Type of Component";
        protected override string DefaultEvaluationUnit => subcomponents[0].name();

        /// <summary>
        /// Initializes a new instance of the GhComponent_ShowSurfaceResult class.
        /// </summary>
        public GhComponent_ShowSurfaceResult()
          : base("GhComponent_ShowSurfaceResult", "ShowSurfaceResult",
              "Show surface results",
              "RFemResults", "SurfaceResults")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("name1", "nick1", "description", GH_ParamAccess.item);
            //pManager.AddIntegerParameter("name2", "nick2", "description", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            subcomponents.Add(new SubComponent_ShowBasicInternalForces());

            foreach (SubComponent item in subcomponents)
            {
                item.registerEvaluationUnits(mngr);
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        //protected override void SolveInstance(IGH_DataAccess DA)
        //{

        //}

        protected override void SolveInstance(IGH_DataAccess DA, EvaluationUnit _unit)
        {
            if (_unit != null)
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
                        return;
                    }
                }
                throw new Exception("Invalid sub-component");
            }
        }

        public void checkBox_valueChanged(object sender, EventArgs e)
        {
            bool isChecked = ((MenuCheckBox)sender).Active;
            string name = ((MenuCheckBox)sender).Name;
            if (isChecked)
            {
                MessageBox.Show("CheckBox " + name + " is active");
                //Params.RegisterInputParam(new Param_Number {Name = "New", NickName = "new", Description = "", Access = GH_ParamAccess.item }, Params.Output.Count);
            }
            else
            {
                MessageBox.Show("CheckBox " + name + " is not active");
                //Params.UnregisterInputParameter(Params.Output[Params.Output.Count - 1]);
            }
            //Params.OnParametersChanged();
            //ExpireSolution(true);
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
            get { return GH_Exposure.hidden; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("47ccf49d-009c-46f4-85e9-2fcad7be2a90"); }
        }
    }
}