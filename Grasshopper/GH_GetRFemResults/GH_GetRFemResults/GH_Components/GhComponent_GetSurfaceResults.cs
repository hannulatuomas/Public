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
    public class GhComponent_GetSurfaceResults : GH_SwitcherComponent
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
        float scale = 1f;
        DataTree<Mesh> mesh;
        DataTree<Vector3d> meshdisplacements;
        DataTree<Mesh> deformedMesh;
        DataTree<Point3d> meshPoints;
        public DataTree<Point3d> childrenMeshPoints;
        public IResults iResults;
        public string objectList;

        /// <summary>
        /// Initializes a new instance of the GhComponent_GetResults class.
        /// </summary>
        public GhComponent_GetSurfaceResults()
          : base("GetSurfaceResults", "SurfaceResults",
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
            //pManager.AddGenericParameter("UndeformedMesh", "UndeformedMesh", "", GH_ParamAccess.tree);
            //pManager.AddGenericParameter("DeformedMesh", "DeformedMesh", "", GH_ParamAccess.tree);
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
            gH_MenuPanel_deformation.Height = menuHeight + 20;
            gH_MenuPanel_deformation.Width = componentWidth - 2 * sideMarginal;
            gH_MenuPanel_deformation.AdjustWidth = true;

            GH_ExtendableMenu gH_ExtendableMenu_deformation = new GH_ExtendableMenu();
            gH_ExtendableMenu_deformation.Expand();
            gH_ExtendableMenu_deformation.Name = "Deformation Outputs";
            gH_ExtendableMenu_deformation.Height = gH_MenuPanel_run.Height + 20 + 2 * sideMarginal;
            gH_ExtendableMenu_deformation.Header = "Deformation Outputs";

            int checkBoxSize = 10;
            MenuCheckBox checkBox_mesh = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_mesh.Pivot = new System.Drawing.PointF(gH_MenuPanel_deformation.Pivot.X, 10f);
            checkBox_mesh.Name = "UndeformedMesh";
            checkBox_mesh.Header = "Undeformed Mesh";
            checkBox_mesh.Active = false;
            checkBox_mesh._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_mesh = new MenuStaticText();
            checkBoxLabel_mesh.Name = "Undeformed Mesh";
            checkBoxLabel_mesh.Header = "Undeformed Mesh";
            checkBoxLabel_mesh.Text = "Undeformed Mesh";
            checkBoxLabel_mesh.Pivot = new System.Drawing.PointF(checkBox_mesh.Pivot.X + 20, checkBox_mesh.Pivot.Y);

            MenuCheckBox checkBox_deformedMesh = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_deformedMesh.Pivot = new System.Drawing.PointF(checkBox_mesh.Pivot.X, checkBox_mesh.Pivot.Y + 15f);
            checkBox_deformedMesh.Name = "DeformedMesh";
            checkBox_deformedMesh.Header = "Deformed Mesh";
            checkBox_deformedMesh.Active = false;
            checkBox_deformedMesh._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_deformedMesh = new MenuStaticText();
            checkBoxLabel_deformedMesh.Name = "Deformed Mesh";
            checkBoxLabel_deformedMesh.Header = "Deformed Mesh";
            checkBoxLabel_deformedMesh.Text = "Deformed Mesh";
            checkBoxLabel_deformedMesh.Pivot = new System.Drawing.PointF(checkBox_deformedMesh.Pivot.X + 20, checkBox_deformedMesh.Pivot.Y);

            MenuCheckBox checkBox_meshPoints = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_meshPoints.Pivot = new System.Drawing.PointF(checkBox_deformedMesh.Pivot.X, checkBox_deformedMesh.Pivot.Y + 15f);
            checkBox_meshPoints.Name = "MeshPoints";
            checkBox_meshPoints.Header = "MeshPoints";
            checkBox_meshPoints.Active = false;
            checkBox_meshPoints._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_meshPoints = new MenuStaticText();
            checkBoxLabel_meshPoints.Name = "MeshPoints";
            checkBoxLabel_meshPoints.Header = "MeshPoints";
            checkBoxLabel_meshPoints.Text = "MeshPoints";
            checkBoxLabel_meshPoints.Pivot = new System.Drawing.PointF(checkBox_meshPoints.Pivot.X + 20, checkBox_meshPoints.Pivot.Y);

            MenuCheckBox checkBox_meshDisplacements = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBox_meshDisplacements.Pivot = new System.Drawing.PointF(checkBox_meshPoints.Pivot.X, checkBox_meshPoints.Pivot .Y + 15f);
            checkBox_meshDisplacements.Name = "MeshDisplacements";
            checkBox_meshDisplacements.Header = "MeshDisplacements";
            checkBox_meshDisplacements.Active = false;
            checkBox_meshDisplacements._valueChanged += new ValueChangeEventHandler(checkBox_outputs_valueChanged);

            MenuStaticText checkBoxLabel_meshDisplacements = new MenuStaticText();
            checkBoxLabel_meshDisplacements.Name = "MeshDisplacements";
            checkBoxLabel_meshDisplacements.Header = "MeshDisplacements";
            checkBoxLabel_meshDisplacements.Text = "MeshDisplacements";
            checkBoxLabel_meshDisplacements.Pivot = new System.Drawing.PointF(checkBox_meshDisplacements.Pivot.X + 20, checkBox_meshDisplacements.Pivot.Y);

            gH_MenuPanel_deformation.AddControl(checkBox_mesh);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_mesh);
            gH_MenuPanel_deformation.AddControl(checkBox_deformedMesh);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_deformedMesh);
            gH_MenuPanel_deformation.AddControl(checkBox_meshPoints);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_meshPoints);
            gH_MenuPanel_deformation.AddControl(checkBox_meshDisplacements);
            gH_MenuPanel_deformation.AddControl(checkBoxLabel_meshDisplacements);
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
            subcomponents.Add(new GhSubComponent_GetBasicInternalForces(this));
            //subcomponents.Add(new SubComponent_ShowBasicInternalForces());

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
                if (_name == "DeformedMesh")
                {
                    IGH_Param inParam = new Param_Number();
                    inParam.Name = "scale";
                    inParam.NickName = "scale";
                    inParam.Access = GH_ParamAccess.item;
                    inParam.Optional = true;
                    Params.RegisterInputParam(inParam);
                }
            }
            else
            {
                for (int i = 0; i < this.Params.Output.Count; i++)
                {
                    if (this.Params.Output[i].Name == _name)
                    {
                        this.Params.Output.RemoveAt(i);
                        
                        if (_name == "DeformedMesh")
                        {
                            if (this.Params.Input[this.Params.Input.Count - 1].Name == "scale")
                            {
                                this.Params.Input.RemoveAt(this.Params.Input.Count - 1);
                            }
                        }
                    }
                }
            }
            this.UpdateUnit(activeUnit, false, false);
        }

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
                if(DA.GetData(this.Params.Input.Count - 1, ref _scale)) scale = (float)_scale;
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
                                childrenMeshPoints = null;
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

                // Clear Old Data
                if (mesh != null)
                {
                    mesh.Clear();
                }
                if (meshdisplacements != null)
                {
                    meshdisplacements.Clear();
                }
                if (deformedMesh != null)
                {
                    deformedMesh.Clear();
                }
                if (meshPoints != null)
                {
                    meshPoints.Clear();
                }

                for (int i = 0; i < freeOutputs; i++)
                {
                    string name = this.Params.Output[i].Name;

                    if (name == "UndeformedMesh")
                    {
                        iModel.GetApplication().LockLicense();
                        mesh = GetSurfaceResults.CreateFEMeshes(iModel, ref msg, objectList);
                        iModel.GetApplication().UnlockLicense();
                        if (childrenMeshPoints != null)
                        {

                        }
                        DA.SetDataTree(i, mesh);
                    }
                    else if (name == "DeformedMesh")
                    {
                        if (mesh == null || mesh.DataCount == 0)
                        {
                            iModel.GetApplication().LockLicense();
                            mesh = GetSurfaceResults.CreateFEMeshes(iModel, ref msg, objectList);
                            iModel.GetApplication().UnlockLicense();
                        }
                        if (meshdisplacements == null || meshdisplacements.DataCount == 0)
                        {
                            for (int j = 0; j < loadingCase.Count; j++)
                            {
                                if (iResults == null)
                                {
                                    iModel.GetApplication().LockLicense();
                                    ICalculation calculation = iModel.GetCalculation();
                                    try
                                    {
                                        iResults = calculation.GetResultsInFeNodes(loadingCase[j].loadingType, loadingCase[j].loadingNumber);
                                    }
                                    catch (Exception exception)
                                    {
                                        ErrorInfo[] errors = calculation.Calculate(loadingCase[j].loadingType, loadingCase[j].loadingNumber);
                                        if (errors.Length > 0)
                                        {
                                            MessageBox.Show("Error during the calculation: " + errors[0].Description);
                                            iModel.GetApplication().UnlockLicense();
                                            return;
                                        }
                                        iResults = calculation.GetResultsInFeNodes(loadingCase[j].loadingType, loadingCase[j].loadingNumber);
                                    }
                                    iModel.GetApplication().UnlockLicense();
                                }
                                iModel.GetApplication().LockLicense();
                                if (meshdisplacements == null || meshdisplacements.DataCount > 0)
                                {
                                    meshdisplacements = GetSurfaceResults.GetMeshDisplacements(iResults, loadingCase[j].loadingIndex, objectList);
                                }
                                else
                                {
                                    meshdisplacements.MergeTree(GetSurfaceResults.GetMeshDisplacements(iResults, loadingCase[j].loadingIndex, objectList));
                                }
                                
                                iModel.GetApplication().UnlockLicense();
                            }
                        }
                        deformedMesh = GetSurfaceResults.GetDeformedMeshes(meshdisplacements, mesh, scale, ref msg);
                        DA.SetDataTree(i, deformedMesh);
                    }
                    else if (name == "MeshPoints")
                    {
                        iModel.GetApplication().LockLicense();
                        meshPoints = GetSurfaceResults.GetMeshPoints(iModel, objectList);
                        iModel.GetApplication().UnlockLicense();
                        if (childrenMeshPoints != null)
                        {
                            DA.SetDataTree(i, childrenMeshPoints);
                        }
                        else
                        {
                            DA.SetDataTree(i, meshPoints);
                        }
                    }
                    else if (name == "MeshDisplacements")
                    {
                        for (int j = 0; j < loadingCase.Count; j++)
                        {
                            if (iResults == null)
                            {
                                iModel.GetApplication().LockLicense();
                                ICalculation calculation = iModel.GetCalculation();
                                try
                                {
                                    iResults = calculation.GetResultsInFeNodes(loadingCase[j].loadingType, loadingCase[j].loadingNumber);
                                }
                                catch (Exception exception)
                                {
                                    ErrorInfo[] errors = calculation.Calculate(loadingCase[j].loadingType, loadingCase[j].loadingNumber);
                                    if (errors.Length > 0)
                                    {
                                        MessageBox.Show("Error during the calculation: " + errors[0].Description);
                                        iModel.GetApplication().UnlockLicense();
                                        return;
                                    }
                                    iResults = calculation.GetResultsInFeNodes(loadingCase[j].loadingType, loadingCase[j].loadingNumber);
                                }
                                iModel.GetApplication().UnlockLicense();
                            }
                            iModel.GetApplication().LockLicense();
                            if (meshdisplacements == null)
                            {
                                meshdisplacements = GetSurfaceResults.GetMeshDisplacements(iResults, loadingCase[j].loadingIndex, objectList);
                            }
                            else
                            {
                                meshdisplacements.MergeTree(GetSurfaceResults.GetMeshDisplacements(iResults, loadingCase[j].loadingIndex, objectList));
                            }
                            iModel.GetApplication().UnlockLicense();
                        }
                        DA.SetDataTree(i, meshdisplacements);
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
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f340f04f-897a-48b6-9ebd-74d48809667f"); }
        }
    }
}