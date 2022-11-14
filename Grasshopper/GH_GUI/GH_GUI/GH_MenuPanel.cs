using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GH_GUI
{
    public class GH_MenuPanel : GH_Attr_Widget
    {
        private int height;

        private bool _parentWidth;

        private List<GH_Attr_Widget> _controls;

        public GH_Capsule _menu;

        public GH_Attr_Widget _activeControl;

        public bool AdjustWidth
        {
            get
            {
                return this._parentWidth;
            }
            set
            {
                this._parentWidth = value;
            }
        }

        public int Height
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

        public GH_MenuPanel()
        {
            this.width = 0f;
            this.height = 0;
            this._controls = new List<GH_Attr_Widget>();
        }

        public void AddControl(GH_Attr_Widget _control)
        {
            this._controls.Add(_control);
            _control.Index = this._controls.Count;
            _control.Parent = this;
        }

        public override bool Write(GH_IWriter writer)
        {
            GH_IWriter writer2 = writer.CreateChunk("Panel", this.Index);
            for (int i = 0; i < this._controls.Count; i++)
            {
                this._controls[i].Write(writer2);
            }
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            GH_IReader reader2 = reader.FindChunk("Panel", this.Index);
            for (int i = 0; i < this._controls.Count; i++)
            {
                this._controls[i].Read(reader2);
            }
            return base.Read(reader);
        }

        public override void LayoutInternal()
        {
            this.transfromation = new PointF((float)((int)this.parent.Transform.X + (int)this.pivot.X), (float)((int)this.parent.Transform.Y + (int)this.pivot.Y));
            int count = this._controls.Count;
            if (this._parentWidth)
            {
                this.width = this.parent.Width - this.pivot.X - 4f;
                for (int i = 0; i < count; i++)
                {
                    this._controls[i].Width = this.width - 6f;
                    this._controls[i].Style = this.style;
                    this._controls[i].Palette = this.palette;
                    this._controls[i].LayoutInternal();
                }
            }
            else
            {
                for (int j = 0; j < count; j++)
                {
                    this._controls[j].Style = this.style;
                    this._controls[j].Palette = this.palette;
                    this._controls[j].LayoutInternal();
                }
            }
            Rectangle rectangle = new Rectangle((int)this.parent.Transform.X + (int)this.pivot.X, (int)this.parent.Transform.Y + (int)this.pivot.Y, (int)this.width, this.height);
            this._menu = GH_Capsule.CreateTextCapsule(rectangle, rectangle, this.palette, "", new Font(new FontFamily("Arial"), 8f, FontStyle.Bold), GH_Orientation.horizontal_center);
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
            if (num < 0)
            {
            }
            num = GH_Canvas.ZoomFadeLow;
            int r = (int)this.style.Fill.R;
            int g = (int)this.style.Fill.G;
            int b = (int)this.style.Fill.B;
            int red = 80;
            int green = 80;
            int blue = 80;
            GH_PaletteStyle style = new GH_PaletteStyle(Color.FromArgb(num, r, g, b), Color.FromArgb(num, red, green, blue));
            this._menu.Render(canvas.Graphics, style);
            for (int i = 0; i < this._controls.Count; i++)
            {
                this._controls[i].Render(canvas, channel);
            }
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
            RectangleF rectangleF = new RectangleF(this.parent.Transform.X + this.pivot.X, this.parent.Transform.Y + this.pivot.Y, this.width, (float)this.height);
            if (rectangleF.Contains(e.CanvasLocation))
            {
                int count = this._controls.Count;
                for (int i = 0; i < count; i++)
                {
                    if (this._controls[i].Contains(e.CanvasLocation) && this._controls[i].Enabled)
                    {
                        GH_ObjectResponse gH_ObjectResponse = this._controls[i].RespondToMouseDown(sender, e);
                        if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
                        {
                            this._activeControl = this._controls[i];
                        }
                        return gH_ObjectResponse;
                    }
                }
            }
            else if (this._activeControl != null)
            {
                this._activeControl.RespondToMouseDown(sender, e);
                this._activeControl = null;
                return GH_ObjectResponse.Handled;
            }
            return GH_ObjectResponse.Ignore;
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
            RectangleF rectangleF = new RectangleF(this.parent.Transform.X + this.pivot.X, this.parent.Transform.Y + this.pivot.Y, this.width, (float)this.height);
            if (rectangleF.Contains(e.CanvasLocation))
            {
                int count = this._controls.Count;
                for (int i = 0; i < count; i++)
                {
                    if (this._controls[i].Contains(e.CanvasLocation) && this._controls[i].Enabled)
                    {
                        return this._controls[i].RespondToMouseDoubleClick(sender, e);
                    }
                }
            }
            return GH_ObjectResponse.Ignore;
        }

        public override GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(this.parent.Transform.X + this.pivot.X, this.parent.Transform.Y + this.pivot.Y, this.width, (float)this.height);
            if (rectangleF.Contains(pt))
            {
                int count = this._controls.Count;
                for (int i = 0; i < count; i++)
                {
                    GH_Attr_Widget gH_Attr_Widget = this._controls[i].IsTtipPoint(pt);
                    if (gH_Attr_Widget != null)
                    {
                        return gH_Attr_Widget;
                    }
                }
                if (this._showToolTip)
                {
                    return this;
                }
            }
            return null;
        }

        public override void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            e.Icon = null;
            e.Title = this._name + " (Group)";
            e.Text = this._header;
            e.Description = this._description;
        }

        public override bool Contains(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Transform.X, base.Pivot.Y + this.parent.Transform.Y, this.width, (float)this.height);
            return rectangleF.Contains(pt);
        }
    }
}
