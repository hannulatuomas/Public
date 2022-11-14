using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GH_GUI
{
    public abstract class GH_SwitcherComponent : GH_Component
    {
        protected EvaluationUnitManager evalUnits;

        protected EvaluationUnit activeUnit;

        protected RuntimeComponentData staticData;

        public RuntimeComponentData StaticData
        {
            get
            {
                return this.staticData;
            }
        }

        public List<EvaluationUnit> EvalUnits
        {
            get
            {
                return this.evalUnits.Units;
            }
        }

        public EvaluationUnit ActiveUnit
        {
            get
            {
                return this.activeUnit;
            }
        }

        protected virtual string DefaultEvaluationUnit
        {
            get
            {
                return null;
            }
        }

        public virtual string UnitMenuName
        {
            get
            {
                return "Evaluation Units";
            }
        }

        public virtual string UnitMenuHeader
        {
            get
            {
                return "Select evaluation unit";
            }
        }

        public virtual bool UnitlessExistence
        {
            get
            {
                return false;
            }
        }

        protected internal GH_SwitcherComponent(string sName, string sAbbreviation, string sDescription, string sCategory, string sSubCategory) : base(sName, sAbbreviation, sDescription, sCategory, sSubCategory)
        {
            base.Phase = GH_SolutionPhase.Blank;
            this.SetupEvaluationUnits();
        }

        protected override void PostConstructor()
        {
            this.evalUnits = new EvaluationUnitManager(this);
            this.RegisterEvaluationUnits(this.evalUnits);
            base.PostConstructor();
            this.staticData = new RuntimeComponentData(this);
        }

        private void SetupEvaluationUnits()
        {
            if (this.activeUnit != null)
            {
                throw new ArgumentException("Invalid switcher state. No evaluation unit must be active at this point.");
            }
            EvaluationUnit evaluationUnit = this.GetUnit(this.DefaultEvaluationUnit);
            if (evaluationUnit == null && !this.UnitlessExistence)
            {
                if (this.EvalUnits.Count == 0)
                {
                    throw new ArgumentException("Switcher has no evaluation units registered and UnitlessExistence is false.");
                }
                evaluationUnit = this.EvalUnits[0];
            }
            if (base.OnPingDocument() != null)
            {
                Rhino.RhinoApp.WriteLine("Component belongs to a document at a stage where it should not belong to one.");
            }
            this.SwitchUnit(evaluationUnit, false, false);
        }

        public EvaluationUnit GetUnit(string name)
        {
            return this.evalUnits.GetUnit(name);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.SolveInstance(DA, this.activeUnit);
        }

        protected abstract void SolveInstance(IGH_DataAccess DA, EvaluationUnit unit);

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            if (this.evalUnits.Units.Count > 0)
            {
                GH_DocumentObject.Menu_AppendSeparator(menu);
                ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Units");
                foreach (EvaluationUnit current in this.evalUnits.Units)
                {
                    ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem(toolStripMenuItem.DropDown, current.Name, new EventHandler(this.Menu_ActivateUnit), null, true, current.Active);
                    toolStripMenuItem2.Tag = current;
                }
                GH_DocumentObject.Menu_AppendSeparator(menu);
            }
        }

        private void Menu_ActivateUnit(object sender, EventArgs e)
        {
            try
            {
                EvaluationUnit evaluationUnit = (EvaluationUnit)((ToolStripMenuItem)sender).Tag;
                if (evaluationUnit != null)
                {
                    this.SwitchUnit(evaluationUnit, true, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetReadState()
        {
            if (this.activeUnit != null)
            {
                this.staticData.UnregisterUnit(this.activeUnit);
                this.activeUnit.Active = false;
                this.activeUnit = null;
            }
            GH_Document gH_Document = base.OnPingDocument();
            if (gH_Document != null)
            {
                gH_Document.Modified();
                gH_Document.DestroyAttributeCache();
                gH_Document.DestroyObjectTable();
            }
            if (base.Attributes != null)
            {
                ((GH_SwitcherComponentAttributes)base.Attributes).OnSwitchUnit();
            }
            this.Name = this.staticData.CachedName;
            this.NickName = this.staticData.CachedNickname;
            this.Description = this.staticData.CachedDescription;
            base.SetIconOverride(this.staticData.CachedIcon);
            if (base.Attributes != null)
            {
                base.Attributes.ExpireLayout();
            }
        }

        public void ClearUnit(bool recompute = true, bool recordEvent = true)
        {
            if (!this.UnitlessExistence)
            {
                return;
            }
            if (this.activeUnit != null)
            {
                if (recordEvent)
                {
                    base.RecordUndoEvent("Switch Unit", new GH_SwitchAction(this, null));
                }
                this.staticData.UnregisterUnit(this.activeUnit);
                this.activeUnit.Active = false;
                this.activeUnit = null;
            }
            GH_Document gH_Document = base.OnPingDocument();
            if (gH_Document != null)
            {
                gH_Document.Modified();
                gH_Document.DestroyAttributeCache();
                gH_Document.DestroyObjectTable();
            }
            if (base.Attributes != null)
            {
                ((GH_SwitcherComponentAttributes)base.Attributes).OnSwitchUnit();
            }
            this.Name = this.staticData.CachedName;
            this.NickName = this.staticData.CachedNickname;
            this.Description = this.staticData.CachedDescription;
            base.SetIconOverride(this.staticData.CachedIcon);
            if (base.Attributes != null)
            {
                base.Attributes.ExpireLayout();
            }
            if (recompute)
            {
                this.ExpireSolution(true);
            }
        }

        public virtual void SwitchUnit(string unitName, bool recompute = true, bool recordEvent = true)
        {
            EvaluationUnit unit = this.evalUnits.GetUnit(unitName);
            if (unit != null)
            {
                this.SwitchUnit(unit, recompute, recordEvent);
            }
        }

        protected virtual void SwitchUnit(EvaluationUnit unit, bool recompute = true, bool recordEvent = true)
        {
            if (unit == null)
            {
                return;
            }
            if (this.activeUnit != null && this.activeUnit == unit)
            {
                return;
            }
            if (recordEvent)
            {
                base.RecordUndoEvent("Switch Unit", new GH_SwitchAction(this, unit.Name));
            }
            if (this.activeUnit != null)
            {
                this.staticData.UnregisterUnit(this.activeUnit);
                this.activeUnit.Active = false;
                this.activeUnit = null;
            }
            this.staticData.RegisterUnit(unit);
            this.activeUnit = unit;
            this.activeUnit.Active = true;
            GH_Document gH_Document = base.OnPingDocument();
            if (gH_Document != null)
            {
                gH_Document.Modified();
                gH_Document.DestroyAttributeCache();
                gH_Document.DestroyObjectTable();
            }
            if (base.Attributes != null)
            {
                ((GH_SwitcherComponentAttributes)base.Attributes).OnSwitchUnit();
            }
            if (unit.DisplayName != null)
            {
                base.SetIconOverride(unit.Icon);
            }
            if (base.Attributes != null)
            {
                base.Attributes.ExpireLayout();
            }
            if (recompute)
            {
                this.ExpireSolution(true);
            }
        }

        public virtual void UpdateUnit(EvaluationUnit unit, bool recompute = true, bool recordEvent = true)
        {
            if (unit == null)
            {
                return;
            }
            if (recordEvent)
            {
                base.RecordUndoEvent("Switch Unit", new GH_SwitchAction(this, unit.Name));
            }
            if (this.activeUnit != null)
            {
                this.staticData.UnregisterUnit(this.activeUnit);
                this.activeUnit.Active = false;
                this.activeUnit = null;
            }
            this.staticData.RegisterUnit(unit);
            this.activeUnit = unit;
            this.activeUnit.Active = true;
            GH_Document gH_Document = base.OnPingDocument();
            if (gH_Document != null)
            {
                gH_Document.Modified();
                gH_Document.DestroyAttributeCache();
                gH_Document.DestroyObjectTable();
            }
            if (base.Attributes != null)
            {
                ((GH_SwitcherComponentAttributes)base.Attributes).OnSwitchUnit();
            }
            if (unit.DisplayName != null)
            {
                base.SetIconOverride(unit.Icon);
            }
            if (base.Attributes != null)
            {
                base.Attributes.ExpireLayout();
            }
            if (recompute)
            {
                this.ExpireSolution(true);
            }
        }

        protected virtual void RegisterEvaluationUnits(EvaluationUnitManager mngr)
        {
        }

        private void _Setup()
        {
            this.Setup((GH_SwitcherComponentAttributes)base.Attributes);
        }

        protected virtual void Setup(GH_SwitcherComponentAttributes attr)
        {
        }

        protected virtual void OnComponentLoaded()
        {
        }

        protected virtual void OnComponentReset(GH_ExtendableComponentAttributes attr)
        {
        }

        public override bool Write(GH_IWriter writer)
        {
            this.staticData.PrepareWrite(this.activeUnit);
            bool result = base.Write(writer);
            this.staticData.RestoreWrite(this.activeUnit);
            if (this.activeUnit != null)
            {
                GH_IWriter gH_IWriter = writer.CreateChunk("ActiveUnit");
                gH_IWriter.SetString("unitname", this.activeUnit.Name);
            }
            try
            {
                GH_IWriter gH_IWriter2 = writer.CreateChunk("EvalUnits");
                gH_IWriter2.SetInt32("count", this.evalUnits.Units.Count);
                for (int i = 0; i < this.evalUnits.Units.Count; i++)
                {
                    EvaluationUnit evaluationUnit = this.evalUnits.Units[i];
                    GH_IWriter writer2 = gH_IWriter2.CreateChunk("unit", i);
                    evaluationUnit.Write(writer2);
                }
            }
            catch (Exception ex)
            {
                Rhino.RhinoApp.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
            return result;
        }

        public override bool Read(GH_IReader reader)
        {
            bool flag = true;
            this.SetReadState();
            flag &= base.Read(reader);
            string text = null;
            if (reader.ChunkExists("ActiveUnit"))
            {
                GH_IReader gH_IReader = reader.FindChunk("ActiveUnit");
                text = gH_IReader.GetString("unitname");
            }
            if (reader.ChunkExists("EvalUnits"))
            {
                GH_IReader gH_IReader2 = reader.FindChunk("EvalUnits");
                int num = -1;
                if (gH_IReader2.TryGetInt32("count", ref num))
                {
                    for (int i = 0; i < num; i++)
                    {
                        if (gH_IReader2.ChunkExists("unit", i))
                        {
                            GH_IReader gH_IReader3 = gH_IReader2.FindChunk("unit", i);
                            string @string = gH_IReader3.GetString("name");
                            if (text != null)
                            {
                                @string.Equals(text);
                            }
                            EvaluationUnit unit = this.evalUnits.GetUnit(@string);
                            if (unit != null)
                            {
                                unit.Read(gH_IReader3);
                            }
                        }
                    }
                }
            }
            if (text != null)
            {
                this.SwitchUnit(this.GetUnit(text), false, false);
            }
            for (int j = 0; j < this.EvalUnits.Count; j++)
            {
                if (!this.EvalUnits[j].Active)
                {
                    this.EvalUnits[j].NewParameterIds();
                }
            }
            this.OnComponentLoaded();
            return flag;
        }

        public override void CreateAttributes()
        {
            GH_SwitcherComponentAttributes gH_SwitcherComponentAttributes = new GH_SwitcherComponentAttributes(this);
            this.m_attributes = gH_SwitcherComponentAttributes;
            this.Setup(gH_SwitcherComponentAttributes);
        }
    }
}
