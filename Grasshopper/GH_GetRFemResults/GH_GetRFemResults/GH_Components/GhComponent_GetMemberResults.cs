using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dlubal.RFEM5;
using GH_GetRFemResults.GH;
using GH_GetRFemResults.RFem;
using GH_GUI;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace GH_GetRFemResults.GH_Components
{
    public class GhComponent_GetMemberResults : GH_SwitcherComponent
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
        float scale = 1;
        DataTree<Curve> members;
        DataTree<Curve> deformedMembers;
        DataTree<Vector3d> memberDisplacements;
        DataTree<Point3d> points;
        DataTree<Point3d> defPoints;
        DataTree<Point3d> resPoints;
        public IResults iResults;
        public string objectList;

        /// <summary>
        /// Initializes a new instance of the GhComponent_GetResults class.
        /// </summary>
        public GhComponent_GetMemberResults()
          : base("GetMemberResults", "MemberResults",
              "Description",
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

            GH_MenuPanel gH_MenuPanel_deformation = new GH_MenuPanel();
            gH_MenuPanel_deformation.Pivot = new System.Drawing.PointF(sideMarginal, topMarginal);
            gH_MenuPanel_deformation.Height = menuHeight + 55;
            gH_MenuPanel_deformation.Width = componentWidth - 2 * sideMarginal;
            gH_MenuPanel_deformation.AdjustWidth = true;

            GH_ExtendableMenu gH_ExtendableMenu_deformation = new GH_ExtendableMenu();
            gH_ExtendableMenu_deformation.Expand();
            gH_ExtendableMenu_deformation.Name = "Deformation Outputs";
            gH_ExtendableMenu_deformation.Height = gH_MenuPanel_deformation.Height + 2 * sideMarginal;
            gH_ExtendableMenu_deformation.Header = "Deformation Outputs";

            int checkBoxSize = 10;
            MenuCheckBox checkBox_member = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_member.Pivot = new System.Drawing.PointF(gH_MenuPanel_deformation.Pivot.X, 10f);
            checkBox_member.Name = "UndeformedMember";
            checkBox_member.Header = "Undeformed Member";
            checkBox_member.Active = false;
            checkBox_member._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_member = new MenuStaticText();
            checkBoxLabel_member.Name = "Undeformed Member";
            checkBoxLabel_member.Header = "Undeformed Member";
            checkBoxLabel_member.Text = "Undeformed Member";
            checkBoxLabel_member.Pivot = new System.Drawing.PointF(checkBox_member.Pivot.X + 20, checkBox_member.Pivot.Y);

            MenuCheckBox checkBox_deformedMember = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_deformedMember.Pivot = new System.Drawing.PointF(checkBox_member.Pivot.X, checkBox_member.Pivot.Y + 15f);
            checkBox_deformedMember.Name = "DeformedMember";
            checkBox_deformedMember.Header = "Deformed Member";
            checkBox_deformedMember.Active = false;
            checkBox_deformedMember._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_deformedMember = new MenuStaticText();
            checkBoxLabel_deformedMember.Name = "Deformed Member";
            checkBoxLabel_deformedMember.Header = "Deformed Member";
            checkBoxLabel_deformedMember.Text = "Deformed Member";
            checkBoxLabel_deformedMember.Pivot = new System.Drawing.PointF(checkBox_deformedMember.Pivot.X + 20, checkBox_deformedMember.Pivot.Y);

            MenuCheckBox checkBox_memberDisplacements = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_memberDisplacements.Pivot = new System.Drawing.PointF(checkBox_deformedMember.Pivot.X, checkBox_deformedMember.Pivot.Y + 15f);
            checkBox_memberDisplacements.Name = "MemberDisplacements";
            checkBox_memberDisplacements.Header = "MemberDisplacements";
            checkBox_memberDisplacements.Active = false;
            checkBox_memberDisplacements._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_memberDisplacements = new MenuStaticText();
            checkBoxLabel_memberDisplacements.Name = "MemberDisplacements";
            checkBoxLabel_memberDisplacements.Header = "MemberDisplacements";
            checkBoxLabel_memberDisplacements.Text = "MemberDisplacements";
            checkBoxLabel_memberDisplacements.Pivot = new System.Drawing.PointF(checkBox_memberDisplacements.Pivot.X + 20, checkBox_memberDisplacements.Pivot.Y);

            MenuCheckBox checkBox_controlPoints = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_controlPoints.Pivot = new System.Drawing.PointF(checkBox_memberDisplacements.Pivot.X, checkBox_memberDisplacements.Pivot.Y + 15f);
            checkBox_controlPoints.Name = "ControlPoints";
            checkBox_controlPoints.Header = "ControlPoints";
            checkBox_controlPoints.Active = false;
            checkBox_controlPoints._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_controlPoints = new MenuStaticText();
            checkBoxLabel_controlPoints.Name = "ControlPoints";
            checkBoxLabel_controlPoints.Header = "ControlPoints";
            checkBoxLabel_controlPoints.Text = "ControlPoints";
            checkBoxLabel_controlPoints.Pivot = new System.Drawing.PointF(checkBox_controlPoints.Pivot.X + 20, checkBox_controlPoints.Pivot.Y);

            MenuCheckBox checkBox_deformedPoints = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_deformedPoints.Pivot = new System.Drawing.PointF(checkBox_controlPoints.Pivot.X, checkBox_controlPoints.Pivot.Y + 15f);
            checkBox_deformedPoints.Name = "DeformedPoints";
            checkBox_deformedPoints.Header = "DeformedPoints";
            checkBox_deformedPoints.Active = false;
            checkBox_deformedPoints._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_deformedPoints = new MenuStaticText();
            checkBoxLabel_deformedPoints.Name = "DeformedPoints";
            checkBoxLabel_deformedPoints.Header = "DeformedPoints";
            checkBoxLabel_deformedPoints.Text = "DeformedPoints";
            checkBoxLabel_deformedPoints.Pivot = new System.Drawing.PointF(checkBox_deformedPoints.Pivot.X + 20, checkBox_deformedPoints.Pivot.Y);

            MenuCheckBox checkBox_resultLocations = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_resultLocations.Pivot = new System.Drawing.PointF(checkBox_deformedPoints.Pivot.X, checkBox_deformedPoints.Pivot.Y + 15f);
            checkBox_resultLocations.Name = "ResultLocations";
            checkBox_resultLocations.Header = "ResultLocations";
            checkBox_resultLocations.Active = false;
            checkBox_resultLocations._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_resultLocations = new MenuStaticText();
            checkBoxLabel_resultLocations.Name = "ResultLocations";
            checkBoxLabel_resultLocations.Header = "ResultLocations";
            checkBoxLabel_resultLocations.Text = "ResultLocations";
            checkBoxLabel_resultLocations.Pivot = new System.Drawing.PointF(checkBox_resultLocations.Pivot.X + 20, checkBox_resultLocations.Pivot.Y);

            gH_MenuPanel_deformation.AddControl(checkBox_member);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_member);
            gH_MenuPanel_deformation.AddControl(checkBox_deformedMember);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_deformedMember);
            gH_MenuPanel_deformation.AddControl(checkBox_memberDisplacements);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_memberDisplacements);
            gH_MenuPanel_deformation.AddControl(checkBox_controlPoints);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_controlPoints);
            gH_MenuPanel_deformation.AddControl(checkBox_deformedPoints);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_deformedPoints);
            gH_MenuPanel_deformation.AddControl(checkBox_resultLocations);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_resultLocations);
            gH_ExtendableMenu_deformation.AddControl(gH_MenuPanel_deformation);
            attr.AddMenu(gH_ExtendableMenu_deformation);

            base.Setup(attr);
        }

        private void checkBox_outputs_valueChanged(object sender, EventArgs e)
        {
            MenuCheckBox checkBox = (MenuCheckBox)sender;
            string name = checkBox.Name;
            bool value = checkBox.Active;
            VariableParameterMaintenance(name, value);
        }

        protected override void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
            subcomponents.Add(new GhSubComponent_GetMemberInternalForces(this));
            subcomponents.Add(new SubComponent_ShowBasicInternalForces());

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

        private void VariableParameterMaintenance(string _name, bool _value)
        {
            if (_value)
            {
                IGH_Param param = new Param_GenericObject();
                param.Name = _name;
                param.NickName = _name;
                param.Access = GH_ParamAccess.tree;
                Params.RegisterOutputParam(param, 0);
                if (_name == "DeformedMember")
                {
                    IGH_Param inParam = new Param_Number();
                    inParam.Name = "scale";
                    inParam.NickName = "scale";
                    inParam.Access = GH_ParamAccess.item;
                    inParam.Optional = true;
                    Params.RegisterInputParam(inParam, this.Params.Input.Count);
                }
            }
            else
            {
                for (int i = 0; i < this.Params.Output.Count; i++)
                {
                    if (this.Params.Output[i].Name == _name)
                    {
                        this.Params.Output.RemoveAt(i);

                        if (_name == "DeformedMember")
                        {
                            for (int j = 0; j < this.Params.Input.Count; j++)
                            {
                                if (this.Params.Input[j].Name == "scale")
                                {
                                    this.Params.Input.RemoveAt(j);
                                }
                            }
                        }
                    }
                }
            }
            this.UpdateUnit(activeUnit, false, false);
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
            if (this.Params.Input[this.Params.Input.Count - 1].Name == "scale")
            {
                double _scale = 0.0;
                if (DA.GetData(this.Params.Input.Count - 1, ref _scale)) scale = (float)_scale;
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
            if (run_main && iModel != null)
            {
                run_main = false;
                int freeOutputs = this.Params.Output.Count - this.activeUnit.Outputs.Count;
                List<string> msg = new List<string>();

                if (members == null)
                {
                    members = new DataTree<Curve>();
                }
                if (deformedMembers == null)
                {
                    deformedMembers = new DataTree<Curve>();
                }
                if (memberDisplacements == null)
                {
                    memberDisplacements = new DataTree<Vector3d>();
                }
                if (points == null)
                {
                    points = new DataTree<Point3d>();
                }
                if (defPoints == null)
                {
                    defPoints = new DataTree<Point3d>();
                }
                if (resPoints == null)
                {
                    resPoints = new DataTree<Point3d>();
                }
                // Clear Old Data
                members.Clear();
                deformedMembers.Clear();
                memberDisplacements.Clear();
                points.Clear();
                defPoints.Clear();
                resPoints.Clear();

                for (int i = 0; i < freeOutputs; i++)
                {
                    string name = this.Params.Output[i].Name;

                    if (name == "UndeformedMember")
                    {
                        members = GetMemberResults.GetMembers(iModel, objectList);
                        DA.SetDataTree(i, members);
                    }
                    else if (name == "DeformedMember")
                    {
                        DataTree<Curve> result = new DataTree<Curve>();
                        foreach (LoadingCase loading in loadingCase)
                        {
                            iResults = GetMemberResults.GetResults(iModel, loading);
                            memberDisplacements = GetMemberResults.GetMemberDisplacements(iModel, iResults, loading.loadingIndex, objectList);
                            result = GetMemberResults.GetDeformedMembers(memberDisplacements, scale);
                            deformedMembers.MergeTree(result);
                        }
                        DA.SetDataTree(i, deformedMembers);
                    }
                    else if (name == "MemberDisplacements")
                    {
                        DataTree<Vector3d> result = new DataTree<Vector3d>();
                        if (memberDisplacements.DataCount == 0)
                        {
                            foreach (LoadingCase loading in loadingCase)
                            {
                                iResults = GetMemberResults.GetResults(iModel, loading);
                                result = GetMemberResults.GetMemberDisplacements(iModel, iResults, loading.loadingIndex, objectList);
                                memberDisplacements.MergeTree(result);
                            }
                        }
                        DA.SetDataTree(i, memberDisplacements);
                    }                   
                    else if (name == "ControlPoints")
                    {
                        points = GetMemberResults.GetControlPoints(iModel, objectList);
                        DA.SetDataTree(i, points);
                    }
                    else if (name == "DeformationLocations")
                    {
                        // Does not exist
                        DataTree<Point3d> result = new DataTree<Point3d>();
                        DataTree<Point3d> locations = new DataTree<Point3d>();
                        foreach (LoadingCase loading in loadingCase)
                        {
                            iResults = GetMemberResults.GetResults(iModel, loading);
                            result = GetMemberResults.GetDeformationLocations(iModel, iResults, loading.loadingIndex, objectList);
                            locations.MergeTree(result);
                        }
                        DA.SetDataTree(i, locations);
                    }
                    else if (name == "DeformedPoints")
                    {
                        DataTree<Point3d> result = new DataTree<Point3d>();
                        DataTree<Vector3d> dispResult = new DataTree<Vector3d>();
                        if (memberDisplacements.DataCount == 0)
                        {
                            foreach (LoadingCase loading in loadingCase)
                            {
                                iResults = GetMemberResults.GetResults(iModel, loading);
                                dispResult = GetMemberResults.GetMemberDisplacements(iModel, iResults, loading.loadingIndex, objectList);
                                memberDisplacements.MergeTree(dispResult);
                            }
                        }
                        foreach (LoadingCase loading in loadingCase)
                        {
                            iResults = GetMemberResults.GetResults(iModel, loading);
                            result = GetMemberResults.GetDeformedPoints(memberDisplacements, scale);
                            defPoints.MergeTree(result);
                        }
                        DA.SetDataTree(i, defPoints);
                    }
                    else if (name == "ResultLocations")
                    {
                        DataTree<Point3d> result = new DataTree<Point3d>();
                        foreach (LoadingCase loading in loadingCase)
                        {
                            iResults = GetMemberResults.GetResults(iModel, loading);
                            result = GetMemberResults.GetForceLocations(iModel, iResults, loading.loadingIndex, objectList);
                            resPoints.MergeTree(result);
                        }
                        DA.SetDataTree(i, resPoints);
                    }
                }
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
            get { return GH_Exposure.quarternary; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("de53ad7f-f8bb-4ba8-94ef-d8ec9d7ec6f7"); }
        }
    }
}