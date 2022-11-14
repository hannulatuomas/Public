using Grasshopper.Kernel;
using System;

namespace GH_GUI
{
    public abstract class SubComponent
    {
        public abstract string name();

        public abstract void registerEvaluationUnits(EvaluationUnitManager mngr);

        public abstract void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level);

        public virtual void OnComponentLoaded()
        {
        }
    }
}
