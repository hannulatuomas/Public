using GH_GUI;
using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_GetRFemResults.Utilities
{
    public static class Utilities_GhComponent
    {
        public static void UpdateDropDownMenu(MenuDropDown dropDownMenu, List<string> values, Action<MenuDropDown, EventArgs> valueChanged)
        {
            dropDownMenu.VisibleItemCount = Math.Min(10, values.Count);
            int index = 0;
            if (values.Count == dropDownMenu.Items.Count)
            {
                index = dropDownMenu.Value;
            }
            dropDownMenu.clear();
            if (values.Count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    dropDownMenu.addItem(values[i]);
                }
                if (dropDownMenu.Items.Count > index)
                {
                    dropDownMenu.Value = index;
                }
            }
            valueChanged(dropDownMenu, EventArgs.Empty);
        }

        public static void RemoveOutputParameter(EvaluationUnit unit, string name, GH_ExtendableMenu extendableMenu = null)
        {
            for (int i = 0; i < unit.Outputs.Count; i++)
            {
                if (unit.Outputs[i].Parameter.Name == name)
                {
                    unit.Outputs.RemoveAt(i);
                }
            }
            if (extendableMenu != null)
            {
                for (int i = 0; i < extendableMenu.Outputs.Count; i++)
                {
                    if (extendableMenu.Outputs[i].Parameter.Name == name)
                    {
                        extendableMenu.Outputs.RemoveAt(i);
                    }
                }
            }
            for (int i = 0; i < unit.Component.Params.Output.Count; i++)
            {
                if (unit.Component.Params.Output[i].Name == name)
                {
                    unit.Component.Params.Output.RemoveAt(i);
                }
            }
        }
    }
}
