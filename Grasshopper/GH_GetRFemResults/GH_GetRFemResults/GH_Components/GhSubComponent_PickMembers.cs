using Dlubal.RFEM5;
using GH_GUI;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_GetRFemResults.GH_Components
{
    class GhSubComponent_PickMembers : SubComponent
    {
        GhComponent_ObjectPicker parent;

        public override string name()
        {
            return "PickMembers";
        }

        public GhSubComponent_PickMembers(GhComponent_ObjectPicker _parent)
        {
            this.parent = _parent;
        }

        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), name(), "");
            mngr.RegisterUnit(evaluationUnit);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            IModel iModel = parent.iModel;
            parent.objectList = RFem.Select.PickSurfaces(iModel);
        }
    }
}
