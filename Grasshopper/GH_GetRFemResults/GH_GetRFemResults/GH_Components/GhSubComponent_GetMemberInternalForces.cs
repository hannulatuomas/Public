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
    class GhSubComponent_GetMemberInternalForces : SubComponent
    {
        List<LoadingCase> loadingCase = new List<LoadingCase>();
        IModel iModel;
        IResults iResults;
        GhComponent_GetMemberResults parent;
        EvaluationUnit unit;
        List<string> resultNames = new List<string>();
        int checkBoxCount;
        MenuCheckBox[] checkBoxes;
        MenuStaticText[] checkBoxLabels;
        GH_ExtendableMenu gH_ExtendableMenu_outputs;

        public GhSubComponent_GetMemberInternalForces(GhComponent_GetMemberResults _parent)
        {
            this.parent = _parent;
            this.resultNames = Enum.GetNames(typeof(MemberInternalForce)).ToList();
            checkBoxCount = resultNames.Count;
            checkBoxes = new MenuCheckBox[checkBoxCount];
            checkBoxLabels = new MenuStaticText[checkBoxCount];
        }

        public override string name()
        {
            return "GetMemberInternalForces";
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
            string name = checkBox.Name;
            bool value = checkBox.Active;
            VariableParameterMaintenance(name, value);
        }

        private void VariableParameterMaintenance(string _name, bool _value)
        {
            if (_value)
            {
                int count = unit.Outputs.Count;

                IGH_Param param = new Param_GenericObject();
                param.Name = _name;
                param.NickName = _name;
                param.Access = GH_ParamAccess.tree;

                unit.RegisterOutputParam(param, _name, _name, "");
                gH_ExtendableMenu_outputs.RegisterOutputPlug(unit.Outputs[count]);
            }
            else
            {
                Utilities_GhComponent.RemoveOutputParameter(unit, _name, gH_ExtendableMenu_outputs);
            }
            parent.UpdateUnit(unit, false, false);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            loadingCase = parent.loadingCase;

            if (loadingCase.Count > 0)
            {
                this.iModel = loadingCase[0].model;
            }
            if (iModel != null)
            {
                int parentOutputCount = parent.Params.Output.Count;
                int unitOutputs = unit.Outputs.Count;
                for (int i = 0; i < unitOutputs; i++)
                {
                    int index = (parentOutputCount - unitOutputs) + i;
                    string name = unit.Outputs[i].Parameter.Name;

                    switch (name)
                    {
                        case "AxialForceN":
                            DataTree<float> resultN = new DataTree<float>();
                            DataTree<float> forceN = new DataTree<float>();
                            foreach (LoadingCase loading in loadingCase)
                            {
                                iResults = GetMemberResults.GetResults(iModel, loading);
                                forceN = GetMemberResults.GetMemberInternalForce(iModel, iResults, MemberInternalForce.AxialForceN, loading.loadingIndex, parent.objectList);
                                resultN.MergeTree(forceN);
                            }
                            DA.SetDataTree(index, resultN);
                            break;
                        case "ShearForceVy":
                            DataTree<float> resultVy = new DataTree<float>();
                            DataTree<float> forceVy = new DataTree<float>();
                            foreach (LoadingCase loading in loadingCase)
                            {
                                iResults = GetMemberResults.GetResults(iModel, loading);
                                forceVy = GetMemberResults.GetMemberInternalForce(iModel, iResults, MemberInternalForce.ShearForceVy, loading.loadingIndex, parent.objectList);
                                resultVy.MergeTree(forceVy);
                            }
                            DA.SetDataTree(index, resultVy);
                            break;
                        case "ShearForceVz":
                            DataTree<float> resultVz = new DataTree<float>();
                            DataTree<float> forceVz = new DataTree<float>();
                            foreach (LoadingCase loading in loadingCase)
                            {
                                iResults = GetMemberResults.GetResults(iModel, loading);
                                forceVz = GetMemberResults.GetMemberInternalForce(iModel, iResults, MemberInternalForce.ShearForceVz, loading.loadingIndex, parent.objectList);
                                resultVz.MergeTree(forceVz);
                            }
                            DA.SetDataTree(index, resultVz);
                            break;
                        case "MomentMx":
                            DataTree<float> resultMx = new DataTree<float>();
                            DataTree<float> momentX = new DataTree<float>();
                            foreach (LoadingCase loading in loadingCase)
                            {
                                iResults = GetMemberResults.GetResults(iModel, loading);
                                momentX = GetMemberResults.GetMemberInternalForce(iModel, iResults, MemberInternalForce.MomentMx, loading.loadingIndex, parent.objectList);
                                resultMx.MergeTree(momentX);
                            }
                            DA.SetDataTree(index, resultMx);
                            break;
                        case "MomentMy":
                            DataTree<float> resultMy = new DataTree<float>();
                            DataTree<float> momentY = new DataTree<float>();
                            foreach (LoadingCase loading in loadingCase)
                            {
                                iResults = GetMemberResults.GetResults(iModel, loading);
                                momentY = GetMemberResults.GetMemberInternalForce(iModel, iResults, MemberInternalForce.MomentMy, loading.loadingIndex, parent.objectList);
                                resultMy.MergeTree(momentY);
                            }
                            DA.SetDataTree(index, resultMy);
                            break;
                        case "MomentMz":
                            DataTree<float> resultMz = new DataTree<float>();
                            DataTree<float> momentZ = new DataTree<float>();
                            foreach (LoadingCase loading in loadingCase)
                            {
                                iResults = GetMemberResults.GetResults(iModel, loading);
                                momentZ = GetMemberResults.GetMemberInternalForce(iModel, iResults, MemberInternalForce.MomentMz, loading.loadingIndex, parent.objectList);
                                resultMz.MergeTree(momentZ);
                            }
                            DA.SetDataTree(index, resultMz);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
