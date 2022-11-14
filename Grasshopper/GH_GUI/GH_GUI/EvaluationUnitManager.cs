using System;
using System.Collections.Generic;

namespace GH_GUI
{
    public class EvaluationUnitManager
    {
        private GH_SwitcherComponent component;

        private List<EvaluationUnit> units;

        public List<EvaluationUnit> Units
        {
            get
            {
                return this.units;
            }
        }

        public EvaluationUnitManager(GH_SwitcherComponent component)
        {
            this.units = new List<EvaluationUnit>();
            this.component = component;
        }

        public EvaluationUnit GetUnit(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            foreach (EvaluationUnit current in this.units)
            {
                if (current.Name.Equals(name))
                {
                    return current;
                }
            }
            return null;
        }

        public void RegisterUnit(EvaluationUnit unit)
        {
            string name = unit.Name;
            if (name == null)
            {
                return;
            }
            if (this.GetUnit(name) != null)
            {
                throw new ArgumentException("Duplicate evaluation unit[" + name + "] detected");
            }
            unit.Component = this.component;
            this.units.Add(unit);
        }
    }
}
