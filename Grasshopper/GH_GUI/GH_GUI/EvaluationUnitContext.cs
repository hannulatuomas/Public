using System;

namespace GH_GUI
{
    public class EvaluationUnitContext
    {
        private EvaluationUnit unit;

        private GH_MenuCollection collection;

        public GH_MenuCollection Collection
        {
            get
            {
                return this.collection;
            }
            set
            {
                this.collection = value;
            }
        }

        public EvaluationUnitContext(EvaluationUnit unit)
        {
            this.unit = unit;
            this.collection = new GH_MenuCollection();
        }
    }
}
