using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace GH_GUI
{
    public class GH_SwitcherComponentAttributes : GH_ComponentAttributes
    {
        private int offset;

        private float _minWidth;

        private GH_Attr_Widget _activeToolTip;

        protected GH_MenuCollection unitMenuCollection;

        protected GH_MenuCollection collection;

        private GH_MenuCollection composedCollection;

        private MenuDropDown _UnitDrop;

        public float MinWidth
        {
            get
            {
                return this._minWidth;
            }
            set
            {
                this._minWidth = value;
            }
        }

        public GH_SwitcherComponentAttributes(GH_SwitcherComponent component) : base(component)
        {
            this.collection = new GH_MenuCollection();
            this.composedCollection = new GH_MenuCollection();
            this.composedCollection.Merge(this.collection);
            this.CreateUnitDropDown();
            this.InitializeUnitParameters();
        }

        public void InitializeUnitParameters()
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            if (gH_SwitcherComponent.EvalUnits != null)
            {
                foreach (EvaluationUnit current in gH_SwitcherComponent.EvalUnits)
                {
                    foreach (ExtendedPlug current2 in current.Inputs)
                    {
                        if (current2.Parameter.Attributes == null)
                        {
                            current2.Parameter.Attributes = new GH_LinkedParamAttributes(current2.Parameter, this);
                        }
                    }
                    foreach (ExtendedPlug current3 in current.Outputs)
                    {
                        if (current3.Parameter.Attributes == null)
                        {
                            current3.Parameter.Attributes = new GH_LinkedParamAttributes(current3.Parameter, this);
                        }
                    }
                }
            }
        }

        private void ComposeMenu()
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            this.composedCollection = new GH_MenuCollection();
            EvaluationUnit activeUnit = gH_SwitcherComponent.ActiveUnit;
            if (activeUnit != null && activeUnit.Context.Collection != null)
            {
                this.composedCollection.Merge(gH_SwitcherComponent.ActiveUnit.Context.Collection);
            }
            if (this.collection != null)
            {
                this.composedCollection.Merge(this.collection);
            }
            if (this.unitMenuCollection != null)
            {
                this.composedCollection.Merge(this.unitMenuCollection);
            }
        }

        public void CreateUnitDropDown()
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            if (gH_SwitcherComponent.EvalUnits == null || gH_SwitcherComponent.EvalUnits.Count == 0 || (gH_SwitcherComponent.EvalUnits.Count == 1 && !gH_SwitcherComponent.UnitlessExistence))
            {
                return;
            }
            GH_MenuPanel gH_MenuPanel = new GH_MenuPanel();
            gH_MenuPanel.Pivot = new PointF(3f, 15f);
            gH_MenuPanel.Height = 25;
            gH_MenuPanel.AdjustWidth = true;
            gH_MenuPanel.Header = "Unit selection";
            string text = gH_SwitcherComponent.UnitMenuName;
            if (text == null)
            {
                text = "Evaluation Units";
            }
            string text2 = gH_SwitcherComponent.UnitMenuHeader;
            if (text2 == null)
            {
                text2 = "Select evaluation unit";
            }
            this.unitMenuCollection = new GH_MenuCollection();
            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu();
            gH_ExtendableMenu.Name = text;
            gH_ExtendableMenu.Height = (float)(gH_MenuPanel.Height + 4 - 2);
            gH_ExtendableMenu.Header = text2;
            gH_ExtendableMenu.AddControl(gH_MenuPanel);
            this._UnitDrop = new MenuDropDown("units");
            this._UnitDrop.VisibleItemCount = 10;
            this._UnitDrop.Pivot = new PointF(3f, 2f);
            this._UnitDrop._valueChanged += new ValueChangeEventHandler(this._UnitDrop__valueChanged);
            this._UnitDrop.Header = "Evaluation unit selector";
            gH_MenuPanel.AddControl(this._UnitDrop);
            List<EvaluationUnit> evalUnits = gH_SwitcherComponent.EvalUnits;
            if (gH_SwitcherComponent.UnitlessExistence)
            {
                this._UnitDrop.addItem("--NONE--", null);
            }
            for (int i = 0; i < evalUnits.Count; i++)
            {
                this._UnitDrop.addItem(evalUnits[i].Name, evalUnits[i]);
            }
            gH_ExtendableMenu.Expand();
            this.unitMenuCollection.AddMenu(gH_ExtendableMenu);
        }

        private void _UnitDrop__valueChanged(object sender, EventArgs e)
        {
            try
            {
                MenuDropDown menuDropDown = (MenuDropDown)sender;
                MenuDropItem menuDropItem = menuDropDown.Items[menuDropDown.Value];
                if (menuDropItem.data != null)
                {
                    EvaluationUnit evaluationUnit = (EvaluationUnit)menuDropItem.data;
                    ((GH_SwitcherComponent)base.Owner).SwitchUnit(evaluationUnit.Name, true, true);
                }
                else
                {
                    ((GH_SwitcherComponent)base.Owner).ClearUnit(true, true);
                }
            }
            catch (Exception ex)
            {
                Rhino.RhinoApp.WriteLine("Error selection:" + ex.StackTrace);
            }
        }

        public void OnSwitchUnit()
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            EvaluationUnit activeUnit = gH_SwitcherComponent.ActiveUnit;
            this.ComposeMenu();
            if (activeUnit != null)
            {
                if (this._UnitDrop != null)
                {
                    int num = this._UnitDrop.findIndex(activeUnit.Name);
                    if (num != -1)
                    {
                        this._UnitDrop.Value = num;
                        return;
                    }
                }
            }
            else if (((GH_SwitcherComponent)base.Owner).UnitlessExistence)
            {
                if (this._UnitDrop == null)
                {
                    return;
                }
                this._UnitDrop.Value = 0;
            }
        }

        public void AddMenu(GH_ExtendableMenu menu)
        {
            this.collection.AddMenu(menu);
        }

        public override bool Write(GH_IWriter writer)
        {
            try
            {
                this.collection.Write(writer);
            }
            catch (Exception ex)
            {
                Rhino.RhinoApp.WriteLine("FAIL Write: " + ex.Message + " " + ex.StackTrace);
            }
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            try
            {
                this.collection.Read(reader);
            }
            catch (Exception ex)
            {
                Rhino.RhinoApp.WriteLine("FAIL Read " + ex.Message + " " + ex.StackTrace);
            }
            return base.Read(reader);
        }

        protected override void PrepareForRender(GH_Canvas canvas)
        {
            base.PrepareForRender(canvas);
            this.LayoutMenuCollection();
        }

        protected void LayoutBaseComponent()
        {
            GH_SwitcherComponent arg_0B_0 = (GH_SwitcherComponent)base.Owner;
            this.Pivot = GH_Convert.ToPoint(this.Pivot);
            this.m_innerBounds = this.LayoutComponentBox2(base.Owner);
            int add_offset = 0;
            int num = this.ComputeW_ico(base.Owner);
            if (this.MinWidth > (float)num)
            {
                add_offset = (int)((double)(this.MinWidth - (float)num) / 2.0);
            }
            this.LayoutInputParams2(base.Owner, this.m_innerBounds, add_offset);
            this.LayoutOutputParams2(base.Owner, this.m_innerBounds, add_offset);
            this.Bounds = this.LayoutBounds2(base.Owner, this.m_innerBounds);
        }

        private int ComputeW_ico(IGH_Component owner)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            int num = 24;
            int num2 = 0;
            int num3 = 0;
            foreach (IGH_Param current in gH_SwitcherComponent.StaticData.GetComponentInputs())
            {
                int val = current.StateTags.Count * 20;
                num3 = Math.Max(num3, val);
                num2 = Math.Max(num2, GH_FontServer.StringWidth(current.NickName, GH_FontServer.Standard));
            }
            num2 = Math.Max(num2 + 6, 12);
            num2 += num3;
            int num4 = 0;
            int num5 = 0;
            foreach (IGH_Param current2 in gH_SwitcherComponent.StaticData.GetComponentOutputs())
            {
                int val2 = current2.StateTags.Count * 20;
                num5 = Math.Max(num5, val2);
                num4 = Math.Max(num4, GH_FontServer.StringWidth(current2.NickName, GH_FontServer.Standard));
            }
            num4 = Math.Max(num4 + 6, 12);
            num4 += num5;
            return num2 + num + num4 + 6;
        }

        public RectangleF LayoutBounds2(IGH_Component owner, RectangleF bounds)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            foreach (IGH_Param current in gH_SwitcherComponent.StaticData.GetComponentInputSection())
            {
                bounds = RectangleF.Union(bounds, current.Attributes.Bounds);
            }
            foreach (IGH_Param current2 in gH_SwitcherComponent.StaticData.GetComponentOutputSection())
            {
                bounds = RectangleF.Union(bounds, current2.Attributes.Bounds);
            }
            bounds.Inflate(2f, 2f);
            return bounds;
        }

        public RectangleF LayoutComponentBox2(IGH_Component owner)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            int num = Math.Max(gH_SwitcherComponent.StaticData.GetComponentInputSection().Count, gH_SwitcherComponent.StaticData.GetComponentOutputSection().Count) * 20;
            num = Math.Max(num, 24);
            int num2 = 24;
            if (!GH_Attributes<IGH_Component>.IsIconMode(owner.IconDisplayMode))
            {
                num = Math.Max(num, GH_Convert.ToSize(GH_FontServer.MeasureString(owner.NickName, GH_FontServer.Large)).Width + 6);
            }
            RectangleF @in = new RectangleF(owner.Attributes.Pivot.X - 0.5f * (float)num2, owner.Attributes.Pivot.Y - 0.5f * (float)num, (float)num2, (float)num);
            return GH_Convert.ToRectangle(@in);
        }

        public void LayoutInputParams2(IGH_Component owner, RectangleF componentBox, int add_offset)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            List<IGH_Param> componentInputSection = gH_SwitcherComponent.StaticData.GetComponentInputSection();
            int count = componentInputSection.Count;
            if (count != 0)
            {
                int num = 0;
                int num2 = 0;
                foreach (IGH_Param current in gH_SwitcherComponent.StaticData.GetComponentInputs())
                {
                    int val = current.StateTags.Count * 20;
                    num2 = Math.Max(num2, val);
                    num = Math.Max(num, GH_FontServer.StringWidth(current.NickName, GH_FontServer.Standard));
                }
                num = Math.Max(num + 6, 12);
                num += num2 + add_offset;
                float num3 = componentBox.Height / (float)count;
                for (int i = 0; i < count; i++)
                {
                    IGH_Param iGH_Param = componentInputSection[i];
                    if (iGH_Param.Attributes == null)
                    {
                        iGH_Param.Attributes = new GH_LinkedParamAttributes(iGH_Param, owner.Attributes);
                    }
                    float num4 = componentBox.X - (float)num;
                    float num5 = componentBox.Y + (float)i * num3;
                    float width = (float)(num - 3);
                    float height = num3;
                    PointF pivot = new PointF(num4 + 0.5f * (float)num, num5 + 0.5f * num3);
                    iGH_Param.Attributes.Pivot = pivot;
                    RectangleF @in = new RectangleF(num4, num5, width, height);
                    iGH_Param.Attributes.Bounds = GH_Convert.ToRectangle(@in);
                }
                bool flag = false;
                for (int j = 0; j < count; j++)
                {
                    IGH_Param iGH_Param2 = componentInputSection[j];
                    GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)iGH_Param2.Attributes;
                    FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                    GH_StateTagList gH_StateTagList = iGH_Param2.StateTags;
                    if (field != null)
                    {
                        if (gH_StateTagList.Count == 0)
                        {
                            gH_StateTagList = null;
                            field.SetValue(gH_LinkedParamAttributes, gH_StateTagList);
                        }
                        if (gH_StateTagList != null)
                        {
                            flag = true;
                            Rectangle rectangle = GH_Convert.ToRectangle(gH_LinkedParamAttributes.Bounds);
                            rectangle.X += num2;
                            rectangle.Width -= num2;
                            gH_StateTagList.Layout(rectangle, GH_StateTagLayoutDirection.Left);
                            rectangle = gH_StateTagList.BoundingBox;
                            if (!rectangle.IsEmpty)
                            {
                                gH_LinkedParamAttributes.Bounds = RectangleF.Union(gH_LinkedParamAttributes.Bounds, rectangle);
                            }
                            field.SetValue(gH_LinkedParamAttributes, gH_StateTagList);
                        }
                    }
                }
                if (flag)
                {
                    float num6 = 3.40282347E+38f;
                    for (int k = 0; k < count; k++)
                    {
                        IGH_Param iGH_Param3 = componentInputSection[k];
                        IGH_Attributes attributes = iGH_Param3.Attributes;
                        num6 = Math.Min(num6, attributes.Bounds.X);
                    }
                    for (int l = 0; l < count; l++)
                    {
                        IGH_Param iGH_Param4 = componentInputSection[l];
                        IGH_Attributes attributes2 = iGH_Param4.Attributes;
                        RectangleF bounds = attributes2.Bounds;
                        bounds.Width = bounds.Right - num6;
                        bounds.X = num6;
                        attributes2.Bounds = bounds;
                    }
                }
            }
        }

        public void LayoutOutputParams2(IGH_Component owner, RectangleF componentBox, int add_offset)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            List<IGH_Param> componentOutputSection = gH_SwitcherComponent.StaticData.GetComponentOutputSection();
            int count = componentOutputSection.Count;
            if (count != 0)
            {
                int num = 0;
                int num2 = 0;
                foreach (IGH_Param current in gH_SwitcherComponent.StaticData.GetComponentOutputs())
                {
                    int val = current.StateTags.Count * 20;
                    num2 = Math.Max(num2, val);
                    num = Math.Max(num, GH_FontServer.StringWidth(current.NickName, GH_FontServer.Standard));
                }
                num = Math.Max(num + 6, 12);
                num += num2 + add_offset;
                float num3 = componentBox.Height / (float)count;
                for (int i = 0; i < count; i++)
                {
                    IGH_Param iGH_Param = componentOutputSection[i];
                    if (iGH_Param.Attributes == null)
                    {
                        iGH_Param.Attributes = new GH_LinkedParamAttributes(iGH_Param, owner.Attributes);
                    }
                    float num4 = componentBox.Right + 3f;
                    float num5 = componentBox.Y + (float)i * num3;
                    float width = (float)num;
                    float height = num3;
                    PointF pivot = new PointF(num4 + 0.5f * (float)num, num5 + 0.5f * num3);
                    iGH_Param.Attributes.Pivot = pivot;
                    RectangleF @in = new RectangleF(num4, num5, width, height);
                    iGH_Param.Attributes.Bounds = GH_Convert.ToRectangle(@in);
                }
                bool flag = false;
                for (int j = 0; j < count; j++)
                {
                    IGH_Param iGH_Param2 = componentOutputSection[j];
                    GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)iGH_Param2.Attributes;
                    FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                    GH_StateTagList gH_StateTagList = iGH_Param2.StateTags;
                    if (field != null)
                    {
                        if (gH_StateTagList.Count == 0)
                        {
                            gH_StateTagList = null;
                            field.SetValue(gH_LinkedParamAttributes, gH_StateTagList);
                        }
                        if (gH_StateTagList != null)
                        {
                            flag = true;
                            Rectangle rectangle = GH_Convert.ToRectangle(gH_LinkedParamAttributes.Bounds);
                            rectangle.Width -= num2;
                            gH_StateTagList.Layout(rectangle, GH_StateTagLayoutDirection.Right);
                            rectangle = gH_StateTagList.BoundingBox;
                            if (!rectangle.IsEmpty)
                            {
                                gH_LinkedParamAttributes.Bounds = RectangleF.Union(gH_LinkedParamAttributes.Bounds, rectangle);
                            }
                            field.SetValue(gH_LinkedParamAttributes, gH_StateTagList);
                        }
                    }
                }
                if (flag)
                {
                    float num6 = -3.40282347E+38f;
                    for (int k = 0; k < count; k++)
                    {
                        IGH_Param iGH_Param3 = componentOutputSection[k];
                        IGH_Attributes attributes = iGH_Param3.Attributes;
                        num6 = Math.Max(num6, attributes.Bounds.Right);
                    }
                    for (int l = 0; l < count; l++)
                    {
                        IGH_Param iGH_Param4 = componentOutputSection[l];
                        IGH_Attributes attributes2 = iGH_Param4.Attributes;
                        RectangleF bounds = attributes2.Bounds;
                        bounds.Width = num6 - bounds.X;
                        attributes2.Bounds = bounds;
                    }
                }
            }
        }

        protected override void Layout()
        {
            this.Pivot = GH_Convert.ToPoint(this.Pivot);
            this.LayoutBaseComponent();
            GH_SwitcherComponent arg_27_0 = (GH_SwitcherComponent)base.Owner;
            List<ExtendedPlug> ins = new List<ExtendedPlug>();
            List<ExtendedPlug> outs = new List<ExtendedPlug>();
            this.composedCollection.GetMenuPlugs(ref ins, ref outs, true);
            this.LayoutMenuInputs(this.m_innerBounds);
            this.LayoutMenuOutputs(this.m_innerBounds);
            this.Bounds = this.LayoutExtBounds(this.m_innerBounds, ins, outs);
            this.FixLayout(outs);
            this.LayoutMenu();
        }

        public RectangleF LayoutExtBounds(RectangleF bounds, List<ExtendedPlug> ins, List<ExtendedPlug> outs)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            foreach (IGH_Param current in gH_SwitcherComponent.StaticData.GetComponentInputs())
            {
                RectangleF bounds2 = current.Attributes.Bounds;
                if (bounds2.X < bounds.X)
                {
                    float num = bounds.X - bounds2.X;
                    bounds.X = bounds2.X;
                    bounds.Width += num;
                }
                if (bounds2.X + bounds2.Width > bounds.X + bounds.Width)
                {
                    float num2 = bounds2.X + bounds2.Width - (bounds.X + bounds.Width);
                    bounds.Width += num2;
                }
            }
            foreach (IGH_Param current2 in gH_SwitcherComponent.StaticData.GetComponentOutputs())
            {
                RectangleF bounds3 = current2.Attributes.Bounds;
                if (bounds3.X < bounds.X)
                {
                    float num3 = bounds.X - bounds3.X;
                    bounds.X = bounds3.X;
                    bounds.Width += num3;
                }
                if (bounds3.X + bounds3.Width > bounds.X + bounds.Width)
                {
                    float num4 = bounds3.X + bounds3.Width - (bounds.X + bounds.Width);
                    bounds.Width += num4;
                }
            }
            bounds.Inflate(2f, 2f);
            return bounds;
        }

        public void LayoutMenuInputs(RectangleF componentBox)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            float num = 0f;
            int num2 = 0;
            foreach (IGH_Param current in gH_SwitcherComponent.StaticData.GetComponentInputs())
            {
                int val = 20 * current.StateTags.Count;
                num2 = Math.Max(num2, val);
                num = Math.Max(num, (float)GH_FontServer.StringWidth(current.NickName, GH_FontServer.Standard));
            }
            num = Math.Max(num + 6f, 12f);
            num += (float)num2;
            float num3 = this.Bounds.Height;
            for (int i = 0; i < this.composedCollection.Menus.Count; i++)
            {
                bool expanded = this.composedCollection.Menus[i].Expanded;
                float num4;
                float num5;
                if (expanded)
                {
                    num4 = 15f + num3 + this.composedCollection.Menus[i].Height;
                    num5 = (float)(Math.Max(this.composedCollection.Menus[i].Inputs.Count, this.composedCollection.Menus[i].Outputs.Count) * 20);
                }
                else
                {
                    num4 = num3 + 5f;
                    num5 = 0f;
                }
                List<ExtendedPlug> inputs = this.composedCollection.Menus[i].Inputs;
                int count = inputs.Count;
                if (count != 0)
                {
                    float num6 = num5 / (float)count;
                    for (int j = 0; j < count; j++)
                    {
                        IGH_Param parameter = inputs[j].Parameter;
                        if (parameter.Attributes == null)
                        {
                            parameter.Attributes = new GH_LinkedParamAttributes(parameter, this);
                        }
                        float num7 = componentBox.X - num;
                        float num8 = num4 + componentBox.Y + (float)j * num6;
                        float width = num - 3f;
                        float height = num6;
                        PointF pivot = new PointF(num7 + 0.5f * num, num8 + 0.5f * num6);
                        parameter.Attributes.Pivot = pivot;
                        RectangleF @in = new RectangleF(num7, num8, width, height);
                        parameter.Attributes.Bounds = GH_Convert.ToRectangle(@in);
                    }
                    for (int k = 0; k < count; k++)
                    {
                        IGH_Param parameter2 = inputs[k].Parameter;
                        GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)parameter2.Attributes;
                        FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                        GH_StateTagList gH_StateTagList = parameter2.StateTags;
                        if (field != null)
                        {
                            if (gH_StateTagList.Count == 0)
                            {
                                gH_StateTagList = null;
                                field.SetValue(gH_LinkedParamAttributes, gH_StateTagList);
                            }
                            if (gH_StateTagList != null)
                            {
                                Rectangle rectangle = GH_Convert.ToRectangle(gH_LinkedParamAttributes.Bounds);
                                rectangle.X += num2;
                                rectangle.Width -= num2;
                                gH_StateTagList.Layout(rectangle, GH_StateTagLayoutDirection.Left);
                                rectangle = gH_StateTagList.BoundingBox;
                                if (!rectangle.IsEmpty)
                                {
                                    gH_LinkedParamAttributes.Bounds = RectangleF.Union(gH_LinkedParamAttributes.Bounds, rectangle);
                                }
                                field.SetValue(gH_LinkedParamAttributes, gH_StateTagList);
                            }
                        }
                        if (!expanded)
                        {
                            gH_LinkedParamAttributes.Bounds = new RectangleF(gH_LinkedParamAttributes.Bounds.X, gH_LinkedParamAttributes.Bounds.Y, 5f, gH_LinkedParamAttributes.Bounds.Height);
                        }
                    }
                }
                num3 += this.composedCollection.Menus[i].TotalHeight;
            }
        }

        public void LayoutMenuOutputs(RectangleF componentBox)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            float num = 0f;
            int num2 = 0;
            foreach (IGH_Param current in gH_SwitcherComponent.StaticData.GetComponentOutputs())
            {
                int val = 20 * current.StateTags.Count;
                num2 = Math.Max(num2, val);
                num = Math.Max(num, (float)GH_FontServer.StringWidth(current.NickName, GH_FontServer.Standard));
            }
            num = Math.Max(num + 6f, 12f);
            num += (float)num2;
            float num3 = this.Bounds.Height;
            for (int i = 0; i < this.composedCollection.Menus.Count; i++)
            {
                bool expanded = this.composedCollection.Menus[i].Expanded;
                float num4;
                float num5;
                if (expanded)
                {
                    num4 = 15f + num3 + this.composedCollection.Menus[i].Height;
                    num5 = (float)(Math.Max(this.composedCollection.Menus[i].Inputs.Count, this.composedCollection.Menus[i].Outputs.Count) * 20);
                }
                else
                {
                    num4 = num3 + 5f;
                    num5 = 0f;
                }
                List<ExtendedPlug> outputs = this.composedCollection.Menus[i].Outputs;
                int count = outputs.Count;
                if (count != 0)
                {
                    float num6 = num5 / (float)count;
                    for (int j = 0; j < count; j++)
                    {
                        IGH_Param parameter = outputs[j].Parameter;
                        if (parameter.Attributes == null)
                        {
                            parameter.Attributes = new GH_LinkedParamAttributes(parameter, this);
                        }
                        float num7 = componentBox.Right + 3f;
                        float num8 = num4 + componentBox.Y + (float)j * num6;
                        float width = num;
                        float height = num6;
                        PointF pivot = new PointF(num7 + 0.5f * num, num8 + 0.5f * num6);
                        parameter.Attributes.Pivot = pivot;
                        RectangleF @in = new RectangleF(num7, num8, width, height);
                        parameter.Attributes.Bounds = GH_Convert.ToRectangle(@in);
                    }
                    for (int k = 0; k < count; k++)
                    {
                        IGH_Param parameter2 = outputs[k].Parameter;
                        GH_LinkedParamAttributes gH_LinkedParamAttributes = (GH_LinkedParamAttributes)parameter2.Attributes;
                        FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                        GH_StateTagList gH_StateTagList = parameter2.StateTags;
                        if (field != null)
                        {
                            if (gH_StateTagList.Count == 0)
                            {
                                gH_StateTagList = null;
                                field.SetValue(gH_LinkedParamAttributes, gH_StateTagList);
                            }
                            if (gH_StateTagList != null)
                            {
                                Rectangle rectangle = GH_Convert.ToRectangle(gH_LinkedParamAttributes.Bounds);
                                rectangle.Width -= num2;
                                gH_StateTagList.Layout(rectangle, GH_StateTagLayoutDirection.Right);
                                rectangle = gH_StateTagList.BoundingBox;
                                if (!rectangle.IsEmpty)
                                {
                                    gH_LinkedParamAttributes.Bounds = RectangleF.Union(gH_LinkedParamAttributes.Bounds, rectangle);
                                }
                                field.SetValue(gH_LinkedParamAttributes, gH_StateTagList);
                            }
                        }
                        if (!expanded)
                        {
                            gH_LinkedParamAttributes.Bounds = new RectangleF(gH_LinkedParamAttributes.Bounds.X + gH_LinkedParamAttributes.Bounds.Width - 5f, gH_LinkedParamAttributes.Bounds.Y, 5f, gH_LinkedParamAttributes.Bounds.Height);
                        }
                    }
                }
                num3 += this.composedCollection.Menus[i].TotalHeight;
            }
        }

        protected void FixLayout(List<ExtendedPlug> outs)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            float width = this.Bounds.Width;
            if (this._minWidth > width)
            {
                this.Bounds = new RectangleF(this.Bounds.X, this.Bounds.Y, this._minWidth, this.Bounds.Height);
            }
            float num = this.Bounds.Width - width;
            foreach (IGH_Param current in gH_SwitcherComponent.StaticData.GetComponentOutputs())
            {
                PointF pivot = current.Attributes.Pivot;
                RectangleF bounds = current.Attributes.Bounds;
                current.Attributes.Pivot = new PointF(pivot.X + num, pivot.Y);
                current.Attributes.Bounds = new RectangleF(bounds.Location.X + num, bounds.Location.Y, bounds.Width, bounds.Height);
            }
            foreach (IGH_Param current2 in gH_SwitcherComponent.StaticData.GetComponentInputs())
            {
                PointF pivot2 = current2.Attributes.Pivot;
                RectangleF bounds2 = current2.Attributes.Bounds;
                current2.Attributes.Pivot = new PointF(pivot2.X + num, pivot2.Y);
                current2.Attributes.Bounds = new RectangleF(bounds2.Location.X + num, bounds2.Location.Y, bounds2.Width, bounds2.Height);
            }
        }

        private void LayoutMenuCollection()
        {
            GH_Palette impliedPalette = GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner);
            GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(impliedPalette, this.Selected, base.Owner.Locked, base.Owner.Hidden);
            this.composedCollection.Style = impliedStyle;
            this.composedCollection.Palette = impliedPalette;
            this.composedCollection.Layout();
        }

        protected void LayoutMenu()
        {
            this.offset = (int)this.Bounds.Height;
            this.composedCollection.Pivot = new PointF(this.Bounds.X, (float)((int)this.Bounds.Y + this.offset));
            this.composedCollection.Width = this.Bounds.Width;
            this.LayoutMenuCollection();
            this.Bounds = new RectangleF(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + this.composedCollection.Height);
        }

        protected override void Render(GH_Canvas iCanvas, Graphics graph, GH_CanvasChannel iChannel)
        {
            if (iChannel != GH_CanvasChannel.Wires)
            {
                if (iChannel != GH_CanvasChannel.Objects)
                {
                    return;
                }
            }
            else
            {
                using (List<IGH_Param>.Enumerator enumerator = base.Owner.Params.Input.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        IGH_Param current = enumerator.Current;
                        current.Attributes.RenderToCanvas(iCanvas, GH_CanvasChannel.Wires);
                    }
                    return;
                }
            }
            this.RenderComponentCapsule2(iCanvas, graph);
            this.composedCollection.Render(iCanvas, iChannel);
        }

        protected void RenderComponentCapsule2(GH_Canvas canvas, Graphics graphics)
        {
            this.RenderComponentCapsule2(canvas, graphics, true, true, true, true, true, true);
        }

        protected void RenderComponentCapsule2(GH_Canvas canvas, Graphics graphics, bool drawComponentBaseBox, bool drawComponentNameBox, bool drawJaggedEdges, bool drawParameterGrips, bool drawParameterNames, bool drawZuiElements)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)base.Owner;
            RectangleF bounds = this.Bounds;
            this.Bounds = bounds;
            if (canvas.Viewport.IsVisible(ref bounds, 10f))
            {
                GH_Palette gH_Palette = GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner);
                if (gH_Palette == GH_Palette.Normal && !base.Owner.IsPreviewCapable)
                {
                    gH_Palette = GH_Palette.Hidden;
                }
                GH_Capsule gH_Capsule = GH_Capsule.CreateCapsule(this.Bounds, gH_Palette);
                bool left = base.Owner.Params.Input.Count == 0;
                bool right = base.Owner.Params.Output.Count == 0;
                gH_Capsule.SetJaggedEdges(left, right);
                GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(gH_Palette, this.Selected, base.Owner.Locked, base.Owner.Hidden);
                if (drawParameterGrips)
                {
                    foreach (IGH_Param current in gH_SwitcherComponent.StaticData.StaticInputs)
                    {
                        gH_Capsule.AddInputGrip(current.Attributes.InputGrip.Y);
                    }
                    foreach (IGH_Param current2 in gH_SwitcherComponent.StaticData.DynamicInputs)
                    {
                        gH_Capsule.AddInputGrip(current2.Attributes.InputGrip.Y);
                    }
                    foreach (IGH_Param current3 in gH_SwitcherComponent.StaticData.StaticOutputs)
                    {
                        gH_Capsule.AddOutputGrip(current3.Attributes.OutputGrip.Y);
                    }
                    foreach (IGH_Param current4 in gH_SwitcherComponent.StaticData.DynamicOutputs)
                    {
                        gH_Capsule.AddOutputGrip(current4.Attributes.OutputGrip.Y);
                    }
                }
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                canvas.SetSmartTextRenderingHint();
                if (GH_Attributes<IGH_Component>.IsIconMode(base.Owner.IconDisplayMode))
                {
                    if (drawComponentBaseBox)
                    {
                        if (base.Owner.Message != null)
                        {
                            gH_Capsule.RenderEngine.RenderMessage(graphics, base.Owner.Message, impliedStyle);
                        }
                        gH_Capsule.Render(graphics, impliedStyle);
                    }
                    if (drawComponentNameBox && base.Owner.Icon_24x24 != null)
                    {
                        if (base.Owner.Locked)
                        {
                            gH_Capsule.RenderEngine.RenderIcon(graphics, base.Owner.Icon_24x24_Locked, this.m_innerBounds, 0, 0);
                        }
                        else
                        {
                            gH_Capsule.RenderEngine.RenderIcon(graphics, base.Owner.Icon_24x24, this.m_innerBounds, 0, 0);
                        }
                    }
                }
                else
                {
                    if (drawComponentBaseBox)
                    {
                        if (base.Owner.Message != null)
                        {
                            gH_Capsule.RenderEngine.RenderMessage(graphics, base.Owner.Message, impliedStyle);
                        }
                        gH_Capsule.Render(graphics, impliedStyle);
                    }
                    if (drawComponentNameBox)
                    {
                        GH_Capsule gH_Capsule2 = GH_Capsule.CreateTextCapsule(this.m_innerBounds, this.m_innerBounds, GH_Palette.Black, base.Owner.NickName, GH_FontServer.Large, GH_Orientation.vertical_center, 3, 6);
                        gH_Capsule2.Render(graphics, this.Selected, base.Owner.Locked, false);
                        gH_Capsule2.Dispose();
                    }
                }
                if (drawComponentNameBox && base.Owner.Obsolete && CentralSettings.CanvasObsoleteTags && canvas.DrawingMode == GH_CanvasMode.Control)
                {
                    GH_GraphicsUtil.RenderObjectOverlay(graphics, base.Owner, this.m_innerBounds);
                }
                if (drawParameterNames)
                {
                    GH_SwitcherComponentAttributes.RenderComponentParameters2(canvas, graphics, base.Owner, impliedStyle);
                }
                if (drawZuiElements)
                {
                    base.RenderVariableParameterUI(canvas, graphics);
                }
                gH_Capsule.Dispose();
            }
        }

        public static void RenderComponentParameters2(GH_Canvas canvas, Graphics graphics, IGH_Component owner, GH_PaletteStyle style)
        {
            GH_SwitcherComponent gH_SwitcherComponent = (GH_SwitcherComponent)owner;
            int zoomFadeLow = GH_Canvas.ZoomFadeLow;
            if (zoomFadeLow >= 5)
            {
                StringFormat format = GH_TextRenderingConstants.FarCenter;
                canvas.SetSmartTextRenderingHint();
                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(zoomFadeLow, style.Text));
                foreach (IGH_Param current in gH_SwitcherComponent.StaticData.StaticInputs)
                {
                    RectangleF bounds = current.Attributes.Bounds;
                    if (bounds.Width >= 1f)
                    {
                        graphics.DrawString(current.NickName, GH_FontServer.Standard, solidBrush, bounds, format);
                        GH_LinkedParamAttributes obj = (GH_LinkedParamAttributes)current.Attributes;
                        FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                        GH_StateTagList gH_StateTagList = (GH_StateTagList)field.GetValue(obj);
                        if (gH_StateTagList != null)
                        {
                            gH_StateTagList.RenderStateTags(graphics);
                        }
                    }
                }
                format = GH_TextRenderingConstants.NearCenter;
                foreach (IGH_Param current2 in gH_SwitcherComponent.StaticData.StaticOutputs)
                {
                    RectangleF bounds2 = current2.Attributes.Bounds;
                    if (bounds2.Width >= 1f)
                    {
                        graphics.DrawString(current2.NickName, GH_FontServer.Standard, solidBrush, bounds2, format);
                        GH_LinkedParamAttributes obj2 = (GH_LinkedParamAttributes)current2.Attributes;
                        FieldInfo field2 = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                        GH_StateTagList gH_StateTagList2 = (GH_StateTagList)field2.GetValue(obj2);
                        if (gH_StateTagList2 != null)
                        {
                            gH_StateTagList2.RenderStateTags(graphics);
                        }
                    }
                }
                solidBrush.Dispose();
            }
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.composedCollection.RespondToMouseUp(sender, e);
            if (gH_ObjectResponse == GH_ObjectResponse.Capture)
            {
                this.ExpireLayout();
                sender.Invalidate();
                return gH_ObjectResponse;
            }
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Invalidate();
                return GH_ObjectResponse.Release;
            }
            return base.RespondToMouseUp(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.composedCollection.RespondToMouseDoubleClick(sender, e);
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Refresh();
                return gH_ObjectResponse;
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }

        public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.composedCollection.RespondToKeyDown(sender, e);
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Refresh();
                return gH_ObjectResponse;
            }
            return base.RespondToKeyDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.composedCollection.RespondToMouseMove(sender, e);
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Refresh();
                return gH_ObjectResponse;
            }
            return base.RespondToMouseMove(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.composedCollection.RespondToMouseDown(sender, e);
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Refresh();
                return gH_ObjectResponse;
            }
            return base.RespondToMouseDown(sender, e);
        }

        public override bool IsTooltipRegion(PointF pt)
        {
            this._activeToolTip = null;
            bool flag = base.IsTooltipRegion(pt);
            if (flag)
            {
                return flag;
            }
            bool flag2 = this.m_innerBounds.Contains(pt);
            if (flag2)
            {
                GH_Attr_Widget gH_Attr_Widget = this.collection.IsTtipPoint(pt);
                if (gH_Attr_Widget != null)
                {
                    this._activeToolTip = gH_Attr_Widget;
                    return true;
                }
            }
            return false;
        }

        public bool GetActiveTooltip(PointF pt)
        {
            GH_Attr_Widget gH_Attr_Widget = this.composedCollection.IsTtipPoint(pt);
            if (gH_Attr_Widget != null)
            {
                this._activeToolTip = gH_Attr_Widget;
                return true;
            }
            return false;
        }

        public override void SetupTooltip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            this.GetActiveTooltip(canvasPoint);
            if (this._activeToolTip != null)
            {
                this._activeToolTip.TooltipSetup(canvasPoint, e);
                return;
            }
            e.Title = this.PathName;
            e.Text = base.Owner.Description;
            e.Description = base.Owner.InstanceDescription;
            e.Icon = base.Owner.Icon_24x24;
            if (base.Owner is IGH_Param)
            {
                IGH_Param iGH_Param = (IGH_Param)base.Owner;
                string text = iGH_Param.TypeName;
                if (iGH_Param.Access == GH_ParamAccess.list)
                {
                    text += "[…]";
                }
                if (iGH_Param.Access == GH_ParamAccess.tree)
                {
                    text += "{…;…;…}";
                }
                e.Title = string.Format("{0} ({1})", this.PathName, text);
            }
        }
    }
}
