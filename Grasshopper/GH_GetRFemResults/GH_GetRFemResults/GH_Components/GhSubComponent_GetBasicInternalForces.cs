using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GhSubComponent_GetBasicInternalForces : SubComponent
    {
        List<LoadingCase> loadingCase = new List<LoadingCase>();
        IModel iModel;
        GhComponent_GetSurfaceResults parent;
        EvaluationUnit unit;
        List<string> resultNames = new List<string>();
        int checkBoxCount;
        MenuCheckBox[] checkBoxes;
        MenuStaticText[] checkBoxLabels;
        GH_ExtendableMenu gH_ExtendableMenu_outputs;

        bool AxialForceNx = false;
        bool AxialForceNy = false;
        bool AxialForceNxy = false;
        bool ShearForceVx = false;
        bool ShearForceVy = false;
        bool MomentMx = false;
        bool MomentMy = false;
        bool MomentMxy = false;

        public GhSubComponent_GetBasicInternalForces(GhComponent_GetSurfaceResults _parent)
        {
            this.parent = _parent;
            this.resultNames = Enum.GetNames(typeof(SurfaceBasicInternalForce)).ToList();
            checkBoxCount = resultNames.Count;
            checkBoxes = new MenuCheckBox[checkBoxCount];
            checkBoxLabels = new MenuStaticText[checkBoxCount];
        }

        public override string name()
        {
            return "GetBasicInternalForces";
        }

        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), "BasicInternalForces", "Get basic internal forces of surfaces");
            mngr.RegisterUnit(evaluationUnit);
            this.unit = evaluationUnit;
            Setup(evaluationUnit);
        }

        protected void Setup(EvaluationUnit unit)
        {
            float componentWidth = 120f;
            int menuHeight = 135;
            float sideMarginal = 5f;
            float topMarginal = 20f;

            GH_MenuPanel gH_MenuPanel = new GH_MenuPanel();
            gH_MenuPanel.Pivot = new System.Drawing.PointF(sideMarginal, topMarginal);
            gH_MenuPanel.Height = menuHeight;
            gH_MenuPanel.Width = componentWidth - 2 * sideMarginal;
            gH_MenuPanel.AdjustWidth = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu();
            gH_ExtendableMenu.Expand();
            gH_ExtendableMenu.Name = "Select Outputs";
            gH_ExtendableMenu.Height = gH_MenuPanel.Height + 2 * sideMarginal;
            gH_ExtendableMenu.Header = "Select Outputs";

            // Outputs            
            for (int i = 0; i < checkBoxCount; i++)
            {
                int checkBoxSize = 10;
                checkBoxes[i] = new MenuCheckBox(checkBoxSize, checkBoxSize);
                checkBoxes[i].Pivot = new System.Drawing.PointF(gH_MenuPanel.Pivot.X, 10f + i * 15f);
                checkBoxes[i].Name = resultNames[i];
                checkBoxes[i].Header = resultNames[i];
                checkBoxes[i].Active = false;
                checkBoxes[i]._valueChanged += new ValueChangeEventHandler(checkBox_valueChanged);
                checkBoxLabels[i] = new MenuStaticText();
                checkBoxLabels[i].Name = resultNames[i];
                checkBoxLabels[i].Header = resultNames[i];
                checkBoxLabels[i].Text = resultNames[i];
                checkBoxLabels[i].Pivot = new System.Drawing.PointF(checkBoxes[i].Pivot.X + 20, checkBoxes[i].Pivot.Y);

                gH_MenuPanel.AddControl(checkBoxes[i]);
                gH_MenuPanel.AddControl(checkBoxLabels[i]);
            }

            gH_ExtendableMenu.AddControl(gH_MenuPanel);
     
            unit.AddMenu(gH_ExtendableMenu);

            // Menu for Outputs
            gH_ExtendableMenu_outputs = new GH_ExtendableMenu();
            gH_ExtendableMenu_outputs.Expand();
            gH_ExtendableMenu_outputs.Name = "Outputs";
            gH_ExtendableMenu_outputs.Height = sideMarginal;
            gH_ExtendableMenu_outputs.Header = "Outputs";

            unit.AddMenu(gH_ExtendableMenu_outputs);
        }

        private void checkBox_valueChanged(object sender, EventArgs e)
        {
            MenuCheckBox checkBox = (MenuCheckBox)sender;
            UpdateCheckBoxValues(checkBox);
            VariableParameterMaintenance();
        }

        private void UpdateCheckBoxValues(MenuCheckBox _checkBox)
        {
            string checkBoxName = _checkBox.Name;
            switch (checkBoxName)
            {
                case "AxialForceNx":
                    AxialForceNx = checkBoxes[0].Active;
                    break;
                case "AxialForceNy":
                    AxialForceNy = checkBoxes[1].Active;
                    break;
                case "AxialForceNxy":
                    AxialForceNxy = checkBoxes[2].Active;
                    break;                
                case "ShearForceVx":
                    ShearForceVx = checkBoxes[3].Active;
                    break;
                case "ShearForceVy":
                    ShearForceVy = checkBoxes[4].Active;
                    break;
                case "MomentMx":
                    MomentMx = checkBoxes[5].Active;
                    break;
                case "MomentMy":
                    MomentMy = checkBoxes[6].Active;
                    break;
                case "MomentMxy":
                    MomentMxy = checkBoxes[7].Active;
                    break;
                default:
                    break;
            }
        }

        DataTree<float>[] GetResults(List<LoadingCase> _loadingCase)
        {
            if (_loadingCase[0].name == "" || iModel == null)
            {
                return null;
            }
            DataTree<float>[] resultList = new DataTree<float>[checkBoxCount];

            iModel.GetApplication().LockLicense();
            ICalculation calculation = iModel.GetCalculation();
            IResults iResults;

            for (int i = 0; i < _loadingCase.Count; i++)
            {
                try
                {
                    iResults = calculation.GetResultsInFeNodes(_loadingCase[i].loadingType, _loadingCase[i].loadingNumber);
                }
                catch (Exception exception)
                {
                    ErrorInfo[] errors = calculation.Calculate(_loadingCase[i].loadingType, _loadingCase[i].loadingNumber);
                    if (errors.Length > 0)
                    {
                        MessageBox.Show("Error during the calculation: " + errors[0].Description);
                        iModel.GetApplication().UnlockLicense();
                        return null;
                    }
                    iResults = calculation.GetResultsInFeNodes(_loadingCase[i].loadingType, _loadingCase[i].loadingNumber);
                }

                parent.iResults = iResults;
                DataTree<float> result;
                parent.childrenMeshPoints = null;

                if (AxialForceNx)
                {
                    result = GetSurfaceResults.GetSurfaceInternalForcesFloat(iResults, SurfaceBasicInternalForce.AxialForceNx, _loadingCase[i].loadingIndex, out parent.childrenMeshPoints, parent.objectList);
                    if (resultList[0] == null)
                    {
                        resultList[0] = result;
                    }
                    else
                    {
                        resultList[0].MergeTree(result);
                    }
                }
                if (AxialForceNy)
                {
                    result = GetSurfaceResults.GetSurfaceInternalForcesFloat(iResults, SurfaceBasicInternalForce.AxialForceNy, _loadingCase[i].loadingIndex, out parent.childrenMeshPoints, parent.objectList);
                    if (resultList[1] == null)
                    {
                        resultList[1] = result;
                    }
                    else
                    {
                        resultList[1].MergeTree(result);
                    }
                }
                if (AxialForceNxy)
                {
                    result = GetSurfaceResults.GetSurfaceInternalForcesFloat(iResults, SurfaceBasicInternalForce.AxialForceNxy, _loadingCase[i].loadingIndex, out parent.childrenMeshPoints, parent.objectList);
                    if (resultList[2] == null)
                    {
                        resultList[2] = result;
                    }
                    else
                    {
                        resultList[2].MergeTree(result);
                    }
                }
                if (ShearForceVx)
                {
                    result = GetSurfaceResults.GetSurfaceInternalForcesFloat(iResults, SurfaceBasicInternalForce.ShearForceVx, _loadingCase[i].loadingIndex, out parent.childrenMeshPoints, parent.objectList);
                    if (resultList[3] == null)
                    {
                        resultList[3] = result;
                    }
                    else
                    {
                        resultList[3].MergeTree(result);
                    }
                }
                if (ShearForceVy)
                {
                    result = GetSurfaceResults.GetSurfaceInternalForcesFloat(iResults, SurfaceBasicInternalForce.ShearForceVy, _loadingCase[i].loadingIndex, out parent.childrenMeshPoints, parent.objectList);
                    if (resultList[4] == null)
                    {
                        resultList[4] = result;
                    }
                    else
                    {
                        resultList[4].MergeTree(result);
                    }
                }
                if (MomentMx)
                {
                    result = GetSurfaceResults.GetSurfaceInternalForcesFloat(iResults, SurfaceBasicInternalForce.MomentMx, _loadingCase[i].loadingIndex, out parent.childrenMeshPoints, parent.objectList);
                    if (resultList[5] == null)
                    {
                        resultList[5] = result;
                    }
                    else
                    {
                        resultList[5].MergeTree(result);
                    }
                }
                if (MomentMy)
                {
                    result = GetSurfaceResults.GetSurfaceInternalForcesFloat(iResults, SurfaceBasicInternalForce.MomentMy, _loadingCase[i].loadingIndex, out parent.childrenMeshPoints, parent.objectList);
                    if (resultList[6] == null)
                    {
                        resultList[6] = result;
                    }
                    else
                    {
                        resultList[6].MergeTree(result);
                    }
                }
                if (MomentMxy)
                {
                    result = GetSurfaceResults.GetSurfaceInternalForcesFloat(iResults, SurfaceBasicInternalForce.MomentMxy, _loadingCase[i].loadingIndex, out parent.childrenMeshPoints, parent.objectList);
                    if (resultList[7] == null)
                    {
                        resultList[7] = result;
                    }
                    else
                    {
                        resultList[7].MergeTree(result);
                    }
                }
            }

            iModel.GetApplication().UnlockLicense();
            return resultList;
        }

        public void VariableParameterMaintenance()
        {
            bool _axialForceNx = false;
            bool _axialForceNy = false;
            bool _axialForceNxy = false;
            bool _shearForceVx = false;
            bool _shearForceVy = false;
            bool _momentMx = false;
            bool _momentMy = false;
            bool _momentMxy = false;

            // Get Active Outputs
            for (int i = 0; i < unit.Outputs.Count; i++)
            {
                if (unit.Outputs[i].Parameter.Name == "AxialForceNx")
                {
                    _axialForceNx = true;
                }
                if (unit.Outputs[i].Parameter.Name == "AxialForceNy")
                {
                    _axialForceNy = true;
                }
                if (unit.Outputs[i].Parameter.Name == "AxialForceNxy")
                {
                    _axialForceNxy = true;
                }
                if (unit.Outputs[i].Parameter.Name == "ShearForceVx")
                {
                    _shearForceVx = true;
                }
                if (unit.Outputs[i].Parameter.Name == "ShearForceVy")
                {
                    _shearForceVy = true;
                }
                if (unit.Outputs[i].Parameter.Name == "MomentMx")
                {
                    _momentMx = true;
                }
                if (unit.Outputs[i].Parameter.Name == "MomentMy")
                {
                    _momentMy = true;
                }
                if (unit.Outputs[i].Parameter.Name == "MomentMxy")
                {
                    _momentMxy = true;
                }
            }

            // Nx
            if (AxialForceNx && !_axialForceNx)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "AxialForceNx", "AxialForceNx", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!AxialForceNx && _axialForceNx)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "AxialForceNx", gH_ExtendableMenu_outputs);
            }
            // Ny
            if (AxialForceNy && !_axialForceNy)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "AxialForceNy", "AxialForceNy", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!AxialForceNy && _axialForceNy)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "AxialForceNy", gH_ExtendableMenu_outputs);
            }
            // Nxy
            if (AxialForceNxy && !_axialForceNxy)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "AxialForceNxy", "AxialForceNxy", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!AxialForceNxy && _axialForceNxy)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "AxialForceNxy", gH_ExtendableMenu_outputs);
            }
            // Vx
            if (ShearForceVx && !_shearForceVx)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "ShearForceVx", "ShearForceVx", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!ShearForceVx && _shearForceVx)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "ShearForceVx", gH_ExtendableMenu_outputs);
            }
            // Vy
            if (ShearForceVy && !_shearForceVy)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "ShearForceVy", "ShearForceVy", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!ShearForceVy && _shearForceVy)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "ShearForceVy", gH_ExtendableMenu_outputs);
            }
            // Mx
            if (MomentMx && !_momentMx)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "MomentMx", "MomentMx", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!MomentMx && _momentMx)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "MomentMx", gH_ExtendableMenu_outputs);
            }
            // My
            if (MomentMy && !_momentMy)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "MomentMy", "MomentMy", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!MomentMy && _momentMy)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "MomentMy", gH_ExtendableMenu_outputs);
            }
            // Mxy
            if (MomentMxy && !_momentMxy)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "MomentMxy", "MomentMxy", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!MomentMxy && _momentMxy)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "MomentMxy", gH_ExtendableMenu_outputs);
            }

            parent.UpdateUnit(unit, false, false);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            DataTree<float>[] resultList = null;
            loadingCase = parent.loadingCase;

            if (loadingCase.Count > 0)
            {
                this.iModel = loadingCase[0].model;
            }
            if (iModel != null)
            {
                resultList = GetResults(loadingCase);
            }

            if (resultList != null && resultList.Length > 0)
            {
                int parentOutputCount = parent.Params.Output.Count;
                int unitOutputs = unit.Outputs.Count;
                for (int i = 0; i < unitOutputs; i++)
                {
                    int index = (parentOutputCount - unitOutputs) + i;
                    string name = unit.Outputs[i].Parameter.Name;

                    switch (name)
                    {
                        case "AxialForceNx":
                            DA.SetDataTree(index, resultList[0]);
                            break;
                        case "AxialForceNy":
                            DA.SetDataTree(index, resultList[1]);
                            break;
                        case "AxialForceNxy":
                            DA.SetDataTree(index, resultList[2]);
                            break;
                        case "ShearForceVx":
                            DA.SetDataTree(index, resultList[3]);
                            break;
                        case "ShearForceVy":
                            DA.SetDataTree(index, resultList[4]);
                            break;
                        case "MomentMx":
                            DA.SetDataTree(index, resultList[5]);
                            break;
                        case "MomentMy":
                            DA.SetDataTree(index, resultList[6]);
                            break;
                        case "MomentMxy":
                            DA.SetDataTree(index, resultList[7]);
                            break;                 
                        default:
                            break;
                    }
                }
            }
        }
    }
}