using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;

namespace GH_GUI
{
    public class MenuRadioButton : GH_Attr_Widget
    {
        private int height;

        private bool _active;

        public event ValueChangeEventHandler _valueChanged;

        public bool Active
        {
            get
            {
                return this._active;
            }
            set
            {
                this._active = value;
            }
        }

        public MenuRadioButton(int w, int h)
        {
            this._active = false;
            this.width = (float)w;
            this.height = h;
        }

        public override bool Write(GH_IWriter writer)
        {
            GH_IWriter gH_IWriter = writer.CreateChunk("RadioButton", this.Index);
            gH_IWriter.SetBoolean("Active", this._active);
            return true;
        }

        public override bool Read(GH_IReader reader)
        {
            GH_IReader gH_IReader = reader.FindChunk("RadioButton", this.Index);
            this._active = gH_IReader.GetBoolean("Active");
            return true;
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
            SolidBrush brush;
            SolidBrush brush2;
            Pen pen;
            if (this._enabled)
            {
                brush = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
                brush2 = new SolidBrush(Color.FromArgb(num, 255, 255, 255));
                pen = new Pen(brush, 2f);
            }
            else
            {
                brush = new SolidBrush(Color.FromArgb(num, 50, 50, 50));
                brush2 = new SolidBrush(Color.FromArgb(num, 150, 150, 150));
                pen = new Pen(brush, 2f);
            }
            Graphics graphics = canvas.Graphics;
            if (!this._active)
            {
                Rectangle rect = new Rectangle((int)base.Pivot.X + (int)this.parent.Transform.X, (int)base.Pivot.Y + (int)this.parent.Transform.Y, (int)this.width, this.height);
                graphics.FillEllipse(brush2, rect);
                graphics.DrawEllipse(pen, rect);
                return;
            }
            Rectangle rect2 = new Rectangle((int)base.Pivot.X + (int)this.parent.Transform.X, (int)base.Pivot.Y + (int)this.parent.Transform.Y, (int)this.width, this.height);
            Rectangle rect3 = new Rectangle((int)base.Pivot.X + (int)this.parent.Transform.X + 2, (int)base.Pivot.Y + (int)this.parent.Transform.Y + 2, (int)this.width - 4, this.height - 4);
            graphics.FillEllipse(brush2, rect2);
            graphics.FillEllipse(brush, rect3);
            graphics.DrawEllipse(pen, rect2);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Pivot.X, base.Pivot.Y + this.parent.Pivot.Y, this.width, (float)this.height);
            if (rectangleF.Contains(e.CanvasLocation))
            {
                this._active = !this._active;
            }
            if (this._valueChanged != null)
            {
                this._valueChanged(this, new EventArgs());
            }
            return GH_ObjectResponse.Release;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Handled;
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            new PointF(this.pivot.X + this.parent.Pivot.X, this.pivot.Y + this.parent.Pivot.Y);
            return GH_ObjectResponse.Capture;
        }

        public override bool Contains(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Transform.X, base.Pivot.Y + this.parent.Transform.Y, this.width, (float)this.height);
            return rectangleF.Contains(pt);
        }

        public override GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Transform.X, base.Pivot.Y + this.parent.Transform.Y, this.width, (float)this.height);
            if (rectangleF.Contains(pt))
            {
                return this;
            }
            return null;
        }

        public override void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            e.Icon = null;
            e.Title = this._name + " (RadioButton)";
            e.Text = this._header;
            if (this._active)
            {
                e.Description = "ON";
                return;
            }
            e.Description = "OFF";
        }
    }
}
