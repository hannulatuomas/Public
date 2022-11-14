using Grasshopper.Kernel;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GH_GUI
{
	public class RuntimeComponentData
	{
		private GH_SwitcherComponent component;

		private string cachedName;

		private string cachedNickname;

		private string cachedDescription;

		private Bitmap cachedIcon;

		private List<IGH_Param> stcInputs;

		private List<IGH_Param> stcOutputs;

		private List<IGH_Param> dynInputs;

		private List<IGH_Param> dynOutputs;

		public string CachedName
		{
			get
			{
				return this.cachedName;
			}
		}

		public string CachedNickname
		{
			get
			{
				return this.cachedNickname;
			}
		}

		public string CachedDescription
		{
			get
			{
				return this.cachedDescription;
			}
		}

		public Bitmap CachedIcon
		{
			get
			{
				return this.cachedIcon;
			}
		}

		public List<IGH_Param> StaticInputs
		{
			get
			{
				return this.stcInputs;
			}
		}

		public List<IGH_Param> StaticOutputs
		{
			get
			{
				return this.stcOutputs;
			}
		}

		public List<IGH_Param> DynamicInputs
		{
			get
			{
				return this.dynInputs;
			}
		}

		public List<IGH_Param> DynamicOutputs
		{
			get
			{
				return this.dynOutputs;
			}
		}

		public RuntimeComponentData(GH_SwitcherComponent component)
		{
			this.component = component;
			this.cachedName = component.Name;
			this.cachedNickname = component.NickName;
			this.cachedDescription = component.Description;
			this.cachedIcon = component.Icon_24x24;
			this.dynInputs = new List<IGH_Param>();
			this.dynOutputs = new List<IGH_Param>();
			this.stcInputs = new List<IGH_Param>();
			this.stcOutputs = new List<IGH_Param>();
			foreach (IGH_Param current in component.Params.Input)
			{
				this.stcInputs.Add(current);
			}
			foreach (IGH_Param current2 in component.Params.Output)
			{
				this.stcOutputs.Add(current2);
			}
		}

		private void UnregisterParameter(IGH_Param param, bool input, bool isolate)
		{
			if (input)
			{
				this.component.Params.UnregisterInputParameter(param, isolate);
			}
			else
			{
				this.component.Params.UnregisterOutputParameter(param, isolate);
			}
			if (param.Attributes != null)
			{
				if (param.Attributes.Parent != null)
				{
					RhinoApp.WriteLine("This should not happen");
				}
				param.Attributes.Parent = this.component.Attributes;
			}
		}

		public void PrepareWrite(EvaluationUnit unit)
		{
			if (unit == null)
			{
				return;
			}
			foreach (ExtendedPlug current in unit.Inputs)
			{
				this.UnregisterParameter(current.Parameter, true, false);
			}
			foreach (ExtendedPlug current2 in unit.Outputs)
			{
				this.UnregisterParameter(current2.Parameter, false, false);
			}
		}

		public void RestoreWrite(EvaluationUnit unit)
		{
			if (unit == null)
			{
				return;
			}
			foreach (ExtendedPlug current in unit.Inputs)
			{
				this.component.Params.RegisterInputParam(current.Parameter);
			}
			foreach (ExtendedPlug current2 in unit.Outputs)
			{
				this.component.Params.RegisterOutputParam(current2.Parameter);
			}
		}

		public void UnregisterUnit(EvaluationUnit unit)
		{
			this.stcInputs.Clear();
			this.stcOutputs.Clear();
			this.dynInputs.Clear();
			this.dynOutputs.Clear();
			foreach (ExtendedPlug current in unit.Inputs)
			{
				this.UnregisterParameter(current.Parameter, true, true);
			}
			foreach (ExtendedPlug current2 in unit.Outputs)
			{
				this.UnregisterParameter(current2.Parameter, false, true);
			}
			foreach (IGH_Param current3 in this.component.Params.Input)
			{
				this.stcInputs.Add(current3);
			}
			foreach (IGH_Param current4 in this.component.Params.Output)
			{
				this.stcOutputs.Add(current4);
			}
		}

		public void RegisterUnit(EvaluationUnit unit)
		{
			this.stcInputs.Clear();
			this.stcOutputs.Clear();
			this.dynInputs.Clear();
			this.dynOutputs.Clear();
			foreach (IGH_Param current in this.component.Params.Input)
			{
				this.stcInputs.Add(current);
			}
			foreach (IGH_Param current2 in this.component.Params.Output)
			{
				this.stcOutputs.Add(current2);
			}
			foreach (ExtendedPlug current3 in unit.Inputs)
			{
				this.component.Params.RegisterInputParam(current3.Parameter);
				if (current3.IsMenu)
				{
					this.dynInputs.Add(current3.Parameter);
				}
				else
				{
					this.stcInputs.Add(current3.Parameter);
				}
			}
			foreach (ExtendedPlug current4 in unit.Outputs)
			{
				this.component.Params.RegisterOutputParam(current4.Parameter);
				if (current4.IsMenu)
				{
					this.dynOutputs.Add(current4.Parameter);
				}
				else
				{
					this.stcOutputs.Add(current4.Parameter);
				}
			}
		}

		public List<IGH_Param> GetComponentInputs()
		{
			List<IGH_Param> list = new List<IGH_Param>(this.stcInputs);
			list.AddRange(this.dynInputs);
			return list;
		}

		public List<IGH_Param> GetComponentInputSection()
		{
			return this.stcInputs;
		}

		public List<IGH_Param> GetComponentOutputs()
		{
			List<IGH_Param> list = new List<IGH_Param>(this.stcOutputs);
			list.AddRange(this.dynOutputs);
			return list;
		}

		public List<IGH_Param> GetComponentOutputSection()
		{
			return this.stcOutputs;
		}
	}
}
