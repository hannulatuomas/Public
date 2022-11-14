using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;

namespace GH_GUI
{
    public class MenuCheckBox : GH_Attr_Widget
    {
        private int height;

        private bool _active;

        private Size _size;

        public event ValueChangeEventHandler _valueChanged;

        public Size Size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;
            }
        }

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

        public MenuCheckBox(int w, int h)
        {
            this._size = new Size(w, h);
            this._active = false;
            this.width = (float)w;
            this.height = h;
        }

        public override bool Write(GH_IWriter writer)
        {
            GH_IWriter gH_IWriter = writer.CreateChunk("Checkbox", this.Index);
            gH_IWriter.SetBoolean("Active", this._active);
            return true;
        }

        public override bool Read(GH_IReader reader)
        {
            GH_IReader gH_IReader = reader.FindChunk("Checkbox", this.Index);
            this._active = gH_IReader.GetBoolean("Active");
            return true;
        }

        public override void Render(GH_Canvas canvas, GH_CanvasChannel channel)
        {
            Graphics graphics = canvas.Graphics;
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
            Pen pen = new Pen(Color.FromArgb(num, 0, 0, 0), 2f);
            SolidBrush brush = new SolidBrush(Color.FromArgb(num, 255, 255, 255));
            SolidBrush brush2 = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
            if (!this._active)
            {
                Rectangle rect = new Rectangle((int)base.Pivot.X + (int)this.parent.Transform.X, (int)base.Pivot.Y + (int)this.parent.Transform.Y, this._size.Width, this._size.Height);
                graphics.FillRectangle(brush, rect);
                graphics.DrawRectangle(pen, rect);
                return;
            }
            Rectangle rect2 = new Rectangle((int)base.Pivot.X + (int)this.parent.Transform.X, (int)base.Pivot.Y + (int)this.parent.Transform.Y, this._size.Width, this._size.Height);
            Rectangle rect3 = new Rectangle((int)base.Pivot.X + (int)this.parent.Transform.X + 2, (int)base.Pivot.Y + (int)this.parent.Transform.Y + 2, this._size.Width - 4, this._size.Height - 4);
            graphics.FillRectangle(brush, rect2);
            graphics.FillRectangle(brush2, rect3);
            graphics.DrawRectangle(pen, rect2);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Transform.X, base.Pivot.Y + this.parent.Transform.Y, this.width, (float)this.height);
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
            new PointF(this.pivot.X + this.parent.Transform.X, this.pivot.Y + this.parent.Transform.Y);
            return GH_ObjectResponse.Capture;
        }

        public override bool Contains(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Transform.X, base.Pivot.Y + this.parent.Transform.Y, (float)this._size.Width, (float)this._size.Height);
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
            e.Title = this._name + " (Checkbox)";
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
