using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace GH_GUI
{
    public class GH_ExtendableMenu : GH_Attr_Widget
    {
        private List<ExtendedPlug> inputs;

        private List<ExtendedPlug> outputs;

        private string name;

        private GH_MenuCollection collection;

        private GH_Capsule _menu;

        private List<GH_Attr_Widget> _controls;

        private GH_Attr_Widget _activeControl;

        private bool _expanded;

        private float height;

        private float currentHeight;

        public List<ExtendedPlug> Inputs
        {
            get
            {
                return this.inputs;
            }
        }

        public List<ExtendedPlug> Outputs
        {
            get
            {
                return this.outputs;
            }
        }

        public bool Expanded
        {
            get
            {
                return this._expanded;
            }
        }

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

        public float CurrentHeight
        {
            get
            {
                return this.currentHeight;
            }
        }

        public override string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public float TotalHeight
        {
            get
            {
                if (this._expanded)
                {
                    int num = Math.Max(this.inputs.Count, this.outputs.Count) * 20;
                    if (num > 0)
                    {
                        num += 5;
                    }
                    return 15f + this.height + (float)num;
                }
                return 15f;
            }
        }

        public GH_ExtendableMenu()
        {
            this.inputs = new List<ExtendedPlug>();
            this.outputs = new List<ExtendedPlug>();
            this.pivot = default(PointF);
            this._controls = new List<GH_Attr_Widget>();
        }

        public void RegisterInputPlug(ExtendedPlug plug)
        {
            plug.IsMenu = true;
            this.inputs.Add(plug);
        }

        public void RegisterOutputPlug(ExtendedPlug plug)
        {
            plug.IsMenu = true;
            this.outputs.Add(plug);
        }

        public void Expand()
        {
            if (!this._expanded)
            {
                this._expanded = true;
            }
        }

        public void Collapse()
        {
            if (this._expanded)
            {
                this._expanded = false;
            }
        }

        public void AddControl(GH_Attr_Widget control)
        {
            control.Parent = this;
            control.Index = this._controls.Count;
            this._controls.Add(control);
        }

        public void MakeAllInActive()
        {
            int count = this._controls.Count;
            for (int i = 0; i < count; i++)
            {
                if (this._controls[i] is GH_MenuPanel)
                {
                    ((GH_MenuPanel)this._controls[i])._activeControl = null;
                }
            }
            this._activeControl = null;
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this._activeControl != null)
            {
                GH_ObjectResponse gH_ObjectResponse = this._activeControl.RespondToMouseUp(sender, e);
                if (gH_ObjectResponse == GH_ObjectResponse.Release)
                {
                    this._activeControl = null;
                    return gH_ObjectResponse;
                }
                if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
                {
                    return gH_ObjectResponse;
                }
                this._activeControl = null;
            }
            return GH_ObjectResponse.Ignore;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            RectangleF rectangleF = new RectangleF(this.pivot.X, this.pivot.Y, this.width, 15f);
            if (rectangleF.Contains(e.CanvasLocation))
            {
                if (this.Expanded)
                {
                    this._activeControl = null;
                }
                this._expanded = !this._expanded;
                return GH_ObjectResponse.Handled;
            }
            if (this._expanded)
            {
                RectangleF rectangleF2 = new RectangleF(this.pivot.X, this.pivot.Y + 15f, this.width, this.height);
                if (rectangleF2.Contains(e.CanvasLocation))
                {
                    for (int i = 0; i < this.inputs.Count; i++)
                    {
                        if (this.inputs[i].Parameter.Attributes.Bounds.Contains(e.CanvasLocation))
                        {
                            return this.inputs[i].Parameter.Attributes.RespondToMouseDown(sender, e);
                        }
                    }
                    for (int j = 0; j < this._controls.Count; j++)
                    {
                        if (this._controls[j].Contains(e.CanvasLocation))
                        {
                            this._activeControl = this._controls[j];
                            return this._controls[j].RespondToMouseDown(sender, e);
                        }
                    }
                }
                else if (this._activeControl != null)
                {
                    this._activeControl.RespondToMouseDown(sender, e);
                    this._activeControl = null;
                    return GH_ObjectResponse.Handled;
                }
            }
            return GH_ObjectResponse.Ignore;
        }

        public override GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(this.pivot.X, this.pivot.Y, this.width, 15f);
            if (rectangleF.Contains(pt))
            {
                return this;
            }
            if (this._expanded)
            {
                RectangleF rectangleF2 = new RectangleF(this.pivot.X, this.pivot.Y + 15f, this.width, this.height);
                if (rectangleF2.Contains(pt))
                {
                    int count = this._controls.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (this._controls[i].Contains(pt))
                        {
                            GH_Attr_Widget gH_Attr_Widget = this._controls[i].IsTtipPoint(pt);
                            if (gH_Attr_Widget != null)
                            {
                                return gH_Attr_Widget;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public override void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            e.Icon = null;
            e.Title = "Menu (" + this.name + ")";
            e.Text = this._header;
            if (this._header != null)
            {
                e.Text += "\n";
            }
            if (this._expanded)
            {
                e.Text += "Click to close menu";
            }
            else
            {
                e.Text += "Click to open menu";
            }
            e.Description = this._description;
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this._activeControl != null)
            {
                return this._activeControl.RespondToMouseMove(sender, e);
            }
            return GH_ObjectResponse.Ignore;
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            RectangleF rectangleF = new RectangleF(this.pivot.X, this.pivot.Y, this.width, 15f);
            if (rectangleF.Contains(e.CanvasLocation))
            {
                return GH_ObjectResponse.Handled;
            }
            if (this._expanded)
            {
                RectangleF rectangleF2 = new RectangleF(this.pivot.X, this.pivot.Y + 15f, this.width, this.height);
                if (rectangleF2.Contains(e.CanvasLocation))
                {
                    int count = this._controls.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (this._controls[i].Contains(e.CanvasLocation))
                        {
                            return this._controls[i].RespondToMouseDoubleClick(sender, e);
                        }
                    }
                }
            }
            return GH_ObjectResponse.Ignore;
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Expanded", this._expanded);
            for (int i = 0; i < this._controls.Count; i++)
            {
                this._controls[i].Write(writer);
            }
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            this._expanded = reader.GetBoolean("Expanded");
            for (int i = 0; i < this._controls.Count; i++)
            {
                this._controls[i].Read(reader);
            }
            return base.Read(reader);
        }

        public override void LayoutInternal()
        {
            Rectangle rectangle = new Rectangle((int)this.pivot.X + 1, (int)this.pivot.Y, (int)this.width - 2, 13);
            this._menu = GH_Capsule.CreateTextCapsule(rectangle, rectangle, GH_Palette.Black, this.name, new Font(new FontFamily("Arial"), 8f, FontStyle.Bold), GH_Orientation.horizontal_center, 2, 5);
            this.transfromation = new PointF(this.pivot.X, this.pivot.Y);
            int count = this._controls.Count;
            for (int i = 0; i < count; i++)
            {
                this._controls[i].Style = this.style;
                this._controls[i].Palette = this.palette;
                this._controls[i].LayoutInternal();
            }
        }

        public override void Render(GH_Canvas canvas, GH_CanvasChannel channel)
        {
            float zoom = canvas.Viewport.Zoom;
            int num = 255;
            if (zoom < 1f)
            {
                float num2 = (zoom - 0.5f) * 2f;
                num = (int)((float)num * num2);
            }
            this._menu.Render(canvas.Graphics, false, false, false);
            if (this._expanded && num > 0)
            {
                this.RenderMenuParameters(canvas, canvas.Graphics);
                for (int i = 0; i < this._controls.Count; i++)
                {
                    this._controls[i].Render(canvas, channel);
                }
            }
        }

        public void RenderMenuParameters(GH_Canvas canvas, Graphics graphics)
        {
            if (Math.Max(this.inputs.Count, this.outputs.Count) == 0)
            {
                return;
            }
            int zoomFadeLow = GH_Canvas.ZoomFadeLow;
            if (zoomFadeLow >= 5)
            {
                StringFormat format = GH_TextRenderingConstants.FarCenter;
                canvas.SetSmartTextRenderingHint();
                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(zoomFadeLow, this.style.Text));
                foreach (ExtendedPlug current in this.inputs)
                {
                    IGH_Param parameter = current.Parameter;
                    RectangleF bounds = parameter.Attributes.Bounds;
                    if (bounds.Width >= 1f)
                    {
                        graphics.DrawString(parameter.NickName, GH_FontServer.Standard, solidBrush, bounds, format);
                        GH_LinkedParamAttributes obj = (GH_LinkedParamAttributes)parameter.Attributes;
                        FieldInfo field = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (field != null)
                        {
                            object value = field.GetValue(obj);
                            if (value != null)
                            {
                                GH_StateTagList gH_StateTagList = (GH_StateTagList)value;
                                gH_StateTagList.RenderStateTags(graphics);
                            }
                        }
                    }
                }
                format = GH_TextRenderingConstants.NearCenter;
                foreach (ExtendedPlug current2 in this.outputs)
                {
                    IGH_Param parameter2 = current2.Parameter;
                    RectangleF bounds2 = parameter2.Attributes.Bounds;
                    if (bounds2.Width >= 1f)
                    {
                        graphics.DrawString(parameter2.NickName, GH_FontServer.Standard, solidBrush, bounds2, format);
                        GH_LinkedParamAttributes obj2 = (GH_LinkedParamAttributes)parameter2.Attributes;
                        FieldInfo field2 = typeof(GH_LinkedParamAttributes).GetField("m_renderTags", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (field2 != null)
                        {
                            object value2 = field2.GetValue(obj2);
                            if (value2 != null)
                            {
                                GH_StateTagList gH_StateTagList2 = (GH_StateTagList)value2;
                                gH_StateTagList2.RenderStateTags(graphics);
                            }
                        }
                    }
                }
                solidBrush.Dispose();
            }
        }
    }
}
