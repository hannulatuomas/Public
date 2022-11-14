using GH_IO.Serialization;
using Grasshopper.Kernel;
using System.Collections.Generic;

namespace GH_GUI
{
    public abstract class GH_ExtendableComponent : GH_Component
    {
        public enum GH_ComponentState
        {
            COMPUTED,
            EMPTY
        }

        private GH_RuntimeMessageLevel m_state;

        protected internal GH_ExtendableComponent(string sName, string sAbbreviation, string sDescription, string sCategory, string sSubCategory) : base(sName, sAbbreviation, sDescription, sCategory, sSubCategory)
        {
            base.Phase = GH_SolutionPhase.Blank;
        }

        protected virtual void Setup(GH_ExtendableComponentAttributes attr)
        {
        }

        protected virtual void OnComponentLoaded()
        {
        }

        protected virtual void OnComponentReset(GH_ExtendableComponentAttributes attr)
        {
        }

        public override void ComputeData()
        {
            base.ComputeData();
            if (this.m_state != base.RuntimeMessageLevel && base.RuntimeMessageLevel == GH_RuntimeMessageLevel.Warning)
            {
                List<IGH_Param> input = base.Params.Input;
                int count = input.Count;
                bool flag = true;
                for (int i = 0; i < count; i++)
                {
                    if (input[i].SourceCount == 0 && !input[i].Optional && input[i].VolatileData.IsEmpty)
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag && base.RuntimeMessageLevel == GH_RuntimeMessageLevel.Warning)
                {
                    this.OnComponentReset((GH_ExtendableComponentAttributes)base.Attributes);
                }
            }
            this.m_state = base.RuntimeMessageLevel;
        }

        public override bool Read(GH_IReader reader)
        {
            bool result = base.Read(reader);
            this.OnComponentLoaded();
            return result;
        }

        public override void CreateAttributes()
        {
            GH_ExtendableComponentAttributes gH_ExtendableComponentAttributes = new GH_ExtendableComponentAttributes(this);
            this.m_attributes = gH_ExtendableComponentAttributes;
            this.Setup(gH_ExtendableComponentAttributes);
        }

        public bool outputConnected(int ind)
        {
            return base.Params.Output[ind].Recipients.Count != 0;
        }

        public bool outputInUse(int ind)
        {
            return (base.Params.Output[ind] is IGH_PreviewObject && !base.Hidden) || base.Params.Output[ind].Recipients.Count != 0;
        }

        public bool outputInUse()
        {
            for (int i = 0; i < base.Params.Output.Count; i++)
            {
                if (this.outputInUse(i))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
