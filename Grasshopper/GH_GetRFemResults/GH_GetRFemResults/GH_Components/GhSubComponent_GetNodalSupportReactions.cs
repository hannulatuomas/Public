using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class GhSubComponent_GetNodalSupportReactions : SubComponent
    {
        List<LoadingCase> loadingCase = new List<LoadingCase>();
        IModel iModel;
        GhComponent_GetSupportReactions parent;
        EvaluationUnit unit;
        List<string> resultNames = new List<string>();
        int checkBoxCount;
        MenuCheckBox[] checkBoxes;
        MenuStaticText[] checkBoxLabels;
        GH_ExtendableMenu gH_ExtendableMenu_outputs;

        bool Fx = false;
        bool Fy = false;
        bool Fz = false;
        bool Mx = false;
        bool My = false;
        bool Mz = false;

        public GhSubComponent_GetNodalSupportReactions(GhComponent_GetSupportReactions _parent)
        {
            this.parent = _parent;
            this.resultNames = Enum.GetNames(typeof(SupportReactions)).ToList();
            checkBoxCount = resultNames.Count;
            checkBoxes = new MenuCheckBox[checkBoxCount];
            checkBoxLabels = new MenuStaticText[checkBoxCount];
        }

        public override string name()
        {
            return "GetNodalSupportReactions";
        }
        
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), "NodalSupportReactions", "Get nodal support reactions");
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

            // Add Output for locations
            int count = unit.Outputs.Count;
            IGH_Param param = new Param_GenericObject();
            param.Access = GH_ParamAccess.tree;
            unit.RegisterOutputParam(param, "ReactionLocation", "Location", "");
            gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
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
                case "Fx":
                    Fx = checkBoxes[0].Active;
                    break;
                case "Fy":
                    Fy = checkBoxes[1].Active;
                    break;
                case "Fz":
                    Fz = checkBoxes[2].Active;
                    break;
                case "Mx":
                    Mx = checkBoxes[3].Active;
                    break;
                case "My":
                    My = checkBoxes[4].Active;
                    break;
                case "Mz":
                    Mz = checkBoxes[5].Active;
                    break;
                default:
                    break;
            }
        }

        DataTree<float>[] GetResults(List<LoadingCase> _loadingCase, out DataTree<Point3d> _locations)
        {
            _locations = null;

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
                string nodeFilter = parent.objectList;

                // Get locations out

                DataTree<Point3d> locations = GetSupportReactions.GetNodalSupportLocations(iModel, parent.iResults, _loadingCase[i].loadingIndex, nodeFilter);
                if (_locations == null)
                {
                    _locations = locations;
                }
                else
                {
                    _locations.MergeTree(locations);
                }

                DataTree<float> result;
                List<NodalSupportForces[]> supportForceList = GetSupportReactions.GetNodalSupportForceList(iModel, iResults, nodeFilter);

                if (Fx)
                {
                    result = GetSupportReactions.GetNodalSupportReactionsFloat(iModel, iResults, SupportReactions.Fx, _loadingCase[i].loadingIndex, supportForceList);
                    if (resultList[0] == null)
                    {
                        resultList[0] = result;
                    }
                    else
                    {
                        resultList[0].MergeTree(result);
                    }
                }
                if (Fy)
                {
                    result = GetSupportReactions.GetNodalSupportReactionsFloat(iModel, iResults, SupportReactions.Fy, _loadingCase[i].loadingIndex, supportForceList);
                    if (resultList[1] == null)
                    {
                        resultList[1] = result;
                    }
                    else
                    {
                        resultList[1].MergeTree(result);
                    }
                }
                if (Fz)
                {
                    result = GetSupportReactions.GetNodalSupportReactionsFloat(iModel, iResults, SupportReactions.Fz, _loadingCase[i].loadingIndex, supportForceList);
                    if (resultList[2] == null)
                    {
                        resultList[2] = result;
                    }
                    else
                    {
                        resultList[2].MergeTree(result);
                    }
                }
                if (Mx)
                {
                    result = GetSupportReactions.GetNodalSupportReactionsFloat(iModel, iResults, SupportReactions.Mx, _loadingCase[i].loadingIndex, supportForceList);
                    if (resultList[3] == null)
                    {
                        resultList[3] = result;
                    }
                    else
                    {
                        resultList[3].MergeTree(result);
                    }
                }
                if (My)
                {
                    result = GetSupportReactions.GetNodalSupportReactionsFloat(iModel, iResults, SupportReactions.My, _loadingCase[i].loadingIndex, supportForceList);
                    if (resultList[4] == null)
                    {
                        resultList[4] = result;
                    }
                    else
                    {
                        resultList[4].MergeTree(result);
                    }
                }
                if (Mz)
                {
                    result = GetSupportReactions.GetNodalSupportReactionsFloat(iModel, iResults, SupportReactions.Mz, _loadingCase[i].loadingIndex, supportForceList);
                    if (resultList[5] == null)
                    {
                        resultList[5] = result;
                    }
                    else
                    {
                        resultList[5].MergeTree(result);
                    }
                }
            }

            iModel.GetApplication().UnlockLicense();
            return resultList;
        }

        public void VariableParameterMaintenance()
        {
            bool _Fx = false;
            bool _Fy = false;
            bool _Fz = false;
            bool _Mx = false;
            bool _My = false;
            bool _Mz = false;

            // Get Active Outputs
            for (int i = 0; i < unit.Outputs.Count; i++)
            {
                if (unit.Outputs[i].Parameter.Name == "Fx")
                {
                    _Fx = true;
                }
                if (unit.Outputs[i].Parameter.Name == "Fy")
                {
                    _Fy = true;
                }
                if (unit.Outputs[i].Parameter.Name == "Fz")
                {
                    _Fz = true;
                }
                if (unit.Outputs[i].Parameter.Name == "Mx")
                {
                    _Mx = true;
                }
                if (unit.Outputs[i].Parameter.Name == "My")
                {
                    _My = true;
                }
                if (unit.Outputs[i].Parameter.Name == "Mz")
                {
                    _Mz = true;
                }
            }

            // Nx
            if (Fx && !_Fx)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "Fx", "Fx", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!Fx && _Fx)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "Fx", gH_ExtendableMenu_outputs);
            }
            // Ny
            if (Fy && !_Fy)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "Fy", "Fy", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!Fy && _Fy)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "Fy", gH_ExtendableMenu_outputs);
            }
            // Nxy
            if (Fz && !_Fz)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "Fz", "Fz", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!Fz && _Fz)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "Fz", gH_ExtendableMenu_outputs);
            }
            // Mx
            if (Mx && !_Mx)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "Mx", "Mx", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!Mx && _Mx)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "Mx", gH_ExtendableMenu_outputs);
            }
            // My
            if (My && !_My)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "My", "My", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!My && _My)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "My", gH_ExtendableMenu_outputs);
            }
            // Mxy
            if (Mz && !_Mz)
            {
                int count = unit.Outputs.Count;
                IGH_Param param = new Param_GenericObject();
                param.Access = GH_ParamAccess.tree;
                unit.RegisterOutputParam(param, "Mz", "Mz", "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else if (!Mz && _Mz)
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, "Mz", gH_ExtendableMenu_outputs);
            }

            parent.UpdateUnit(unit, false, false);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            DataTree<Point3d> _locations = null;
            DataTree<float>[] resultList = null;
            loadingCase = parent.loadingCase;

            if (loadingCase.Count > 0)
            {
                this.iModel = loadingCase[0].model;
            }
            if (iModel != null)
            {
                resultList = GetResults(loadingCase, out _locations);
            }

            if (resultList != null && (resultList.Length > 0 || _locations != null))
            {
                int parentOutputCount = parent.Params.Output.Count;
                int unitOutputs = unit.Outputs.Count;
                for (int i = 0; i < unitOutputs; i++)
                {
                    int index = (parentOutputCount - unitOutputs) + i;
                    string name = unit.Outputs[i].Parameter.Name;

                    switch (name)
                    {
                        case "ReactionLocation":
                            DA.SetDataTree(index, _locations);
                            break;
                        case "Fx":
                            DA.SetDataTree(index, resultList[0]);
                            break;
                        case "Fy":
                            DA.SetDataTree(index, resultList[1]);
                            break;
                        case "Fz":
                            DA.SetDataTree(index, resultList[2]);
                            break;
                        case "Mx":
                            DA.SetDataTree(index, resultList[3]);
                            break;
                        case "My":
                            DA.SetDataTree(index, resultList[4]);
                            break;
                        case "Mz":
                            DA.SetDataTree(index, resultList[5]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
