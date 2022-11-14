using Grasshopper.Kernel;
using Grasshopper.Kernel.Undo;
using System;
using System.Collections.Generic;

namespace GH_GUI
{
    public class GH_SwitchAction : GH_UndoAction
    {
        private class GH_SwitcherConnectivity
        {
            private Guid componentId;

            private bool noUnit;

            private string unit;

            private List<GH_SwitchAction.GH_SwitcherParamState> inputs;

            private List<GH_SwitchAction.GH_SwitcherParamState> outputs;

            public string Unit
            {
                get
                {
                    return this.unit;
                }
                set
                {
                    this.unit = value;
                }
            }

            public List<GH_SwitchAction.GH_SwitcherParamState> Inputs
            {
                get
                {
                    return this.inputs;
                }
                set
                {
                    this.inputs = value;
                }
            }

            public List<GH_SwitchAction.GH_SwitcherParamState> Outputs
            {
                get
                {
                    return this.outputs;
                }
                set
                {
                    this.outputs = value;
                }
            }

            public Guid ComponentId
            {
                get
                {
                    return this.componentId;
                }
            }

            public GH_SwitcherConnectivity()
            {
                this.inputs = new List<GH_SwitchAction.GH_SwitcherParamState>();
                this.outputs = new List<GH_SwitchAction.GH_SwitcherParamState>();
            }

            public static GH_SwitchAction.GH_SwitcherConnectivity Create(GH_SwitcherComponent component)
            {
                GH_SwitchAction.GH_SwitcherConnectivity gH_SwitcherConnectivity = new GH_SwitchAction.GH_SwitcherConnectivity();
                gH_SwitcherConnectivity.componentId = component.InstanceGuid;
                EvaluationUnit activeUnit = component.ActiveUnit;
                if (activeUnit != null)
                {
                    gH_SwitcherConnectivity.noUnit = false;
                    gH_SwitcherConnectivity.unit = activeUnit.Name;
                }
                else if (component.UnitlessExistence)
                {
                    gH_SwitcherConnectivity.noUnit = true;
                }
                foreach (IGH_Param current in component.Params.Input)
                {
                    GH_SwitchAction.GH_SwitcherParamState gH_SwitcherParamState = new GH_SwitchAction.GH_SwitcherParamState(GH_ParameterSide.Input, current.InstanceGuid);
                    gH_SwitcherConnectivity.inputs.Add(gH_SwitcherParamState);
                    foreach (IGH_Param current2 in current.Sources)
                    {
                        gH_SwitcherParamState.Targets.Add(current2.InstanceGuid);
                    }
                }
                foreach (IGH_Param current3 in component.Params.Output)
                {
                    GH_SwitchAction.GH_SwitcherParamState gH_SwitcherParamState2 = new GH_SwitchAction.GH_SwitcherParamState(GH_ParameterSide.Output, current3.InstanceGuid);
                    gH_SwitcherConnectivity.outputs.Add(gH_SwitcherParamState2);
                    foreach (IGH_Param current4 in current3.Recipients)
                    {
                        gH_SwitcherParamState2.Targets.Add(current4.InstanceGuid);
                    }
                }
                return gH_SwitcherConnectivity;
            }

            public void Apply(GH_SwitcherComponent component, GH_Document document)
            {
                if (this.noUnit)
                {
                    component.ClearUnit(true, false);
                }
                else
                {
                    component.SwitchUnit(this.unit, true, false);
                }
                for (int i = 0; i < this.inputs.Count; i++)
                {
                    GH_SwitchAction.GH_SwitcherParamState gH_SwitcherParamState = this.inputs[i];
                    int num = component.Params.IndexOfInputParam(gH_SwitcherParamState.ParameterId);
                    if (num != -1)
                    {
                        IGH_Param iGH_Param = component.Params.Input[num];
                        for (int j = 0; j < gH_SwitcherParamState.Targets.Count; j++)
                        {
                            IGH_Param iGH_Param2 = document.FindParameter(gH_SwitcherParamState.Targets[j]);
                            if (iGH_Param2 != null)
                            {
                                iGH_Param.AddSource(iGH_Param2);
                            }
                        }
                    }
                }
                for (int k = 0; k < this.outputs.Count; k++)
                {
                    GH_SwitchAction.GH_SwitcherParamState gH_SwitcherParamState2 = this.outputs[k];
                    int num2 = component.Params.IndexOfOutputParam(gH_SwitcherParamState2.ParameterId);
                    if (num2 != -1)
                    {
                        IGH_Param source = component.Params.Output[num2];
                        for (int l = 0; l < gH_SwitcherParamState2.Targets.Count; l++)
                        {
                            IGH_Param iGH_Param3 = document.FindParameter(gH_SwitcherParamState2.Targets[l]);
                            if (iGH_Param3 != null)
                            {
                                iGH_Param3.AddSource(source);
                            }
                        }
                    }
                }
            }
        }

        private class GH_SwitcherParamState
        {
            private GH_ParameterSide side;

            private Guid paramId;

            private List<Guid> targets;

            public Guid ParameterId
            {
                get
                {
                    return this.paramId;
                }
                set
                {
                    this.paramId = value;
                }
            }

            public List<Guid> Targets
            {
                get
                {
                    return this.targets;
                }
                set
                {
                    this.targets = value;
                }
            }

            public GH_SwitcherParamState(GH_ParameterSide side, Guid paramId)
            {
                this.side = side;
                this.paramId = paramId;
                this.targets = new List<Guid>();
            }
        }

        private GH_SwitchAction.GH_SwitcherConnectivity oldState;

        private string newUnit;

        public override bool ExpiresSolution
        {
            get
            {
                return true;
            }
        }

        public GH_SwitchAction(GH_SwitcherComponent component, string newUnit)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            this.oldState = GH_SwitchAction.GH_SwitcherConnectivity.Create(component);
            this.newUnit = newUnit;
        }

        protected override void Internal_Redo(GH_Document doc)
        {
            IGH_DocumentObject iGH_DocumentObject = doc.FindObject(this.oldState.ComponentId, true);
            if (iGH_DocumentObject == null || !(iGH_DocumentObject is GH_SwitcherComponent))
            {
                throw new GH_UndoException("Switcher component with id[" + this.oldState.ComponentId + "] not found");
            }
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)iGH_DocumentObject;
            if (this.newUnit != null)
            {
                gH_SwitcherComponent.SwitchUnit(this.newUnit, true, false);
                return;
            }
            gH_SwitcherComponent.ClearUnit(false, true);
        }

        protected override void Internal_Undo(GH_Document doc)
        {
            IGH_DocumentObject iGH_DocumentObject = doc.FindObject(this.oldState.ComponentId, true);
            if (iGH_DocumentObject == null || !(iGH_DocumentObject is GH_SwitcherComponent))
            {
                throw new GH_UndoException("Switcher component with id[" + this.oldState.ComponentId + "] not found");
            }
            GH_SwitcherComponent component = (GH_SwitcherComponent)iGH_DocumentObject;
            this.oldState.Apply(component, doc);
        }
    }
}
