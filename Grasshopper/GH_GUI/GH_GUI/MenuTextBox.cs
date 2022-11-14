using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GH_GUI
{
    public class MenuTextBox : GH_Attr_Widget
    {
        private int height;

        private bool _active;

        private string _text;

        private Font _font;

        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
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

        public MenuTextBox()
        {
            this._active = false;
            this.width = 40f;
            this.height = 20;
            this._font = new Font(new FontFamily("Arial"), 10f, FontStyle.Italic);
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
            Pen pen = new Pen(Color.FromArgb(num, 0, 0, 0), 1f);
            Pen pen2 = new Pen(Color.FromArgb(num, 100, 100, 100), 1f);
            SolidBrush brush = new SolidBrush(Color.FromArgb(num, 255, 255, 255));
            SolidBrush brush2 = new SolidBrush(Color.FromArgb(num, 150, 150, 150));
            SolidBrush brush3 = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
            SolidBrush brush4 = new SolidBrush(Color.FromArgb(num, 100, 100, 100));
            PointF point = new PointF(this.parent.Transform.X + this.pivot.X, this.parent.Transform.Y + this.pivot.Y + 2f);
            Rectangle rect = new Rectangle((int)base.Pivot.X + (int)this.parent.Transform.X, (int)base.Pivot.Y + (int)this.parent.Transform.Y, (int)this.width, this.height);
            if (!this._active)
            {
                graphics.FillRectangle(brush2, rect);
                graphics.DrawRectangle(pen2, rect);
                graphics.DrawString(this._text, this._font, brush4, point);
                return;
            }
            graphics.FillRectangle(brush, rect);
            graphics.DrawRectangle(pen, rect);
            graphics.DrawString(this._text, this._font, brush3, point);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Transform.X, base.Pivot.Y + this.parent.Transform.Y, this.width, (float)this.height);
            if (rectangleF.Contains(e.CanvasLocation))
            {
                this._active = !this._active;
            }
            else
            {
                this.TopCollection.ActiveWidget = null;
            }
            return GH_ObjectResponse.Capture;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Transform.X, base.Pivot.Y + this.parent.Transform.Y, this.width, (float)this.height);
            if (rectangleF.Contains(e.CanvasLocation))
            {
                this.TopCollection.ActiveWidget = this;
                return GH_ObjectResponse.Capture;
            }
            this.TopCollection.ActiveWidget = null;
            return GH_ObjectResponse.Handled;
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            new PointF(this.pivot.X + this.parent.Transform.X, this.pivot.Y + this.parent.Transform.Y);
            return GH_ObjectResponse.Capture;
        }

        public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
        {
            return GH_ObjectResponse.Ignore;
        }

        public override bool Contains(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(base.Pivot.X + this.parent.Transform.X, base.Pivot.Y + this.parent.Transform.Y, this.width, (float)this.height);
            return rectangleF.Contains(pt);
        }
    }
}
