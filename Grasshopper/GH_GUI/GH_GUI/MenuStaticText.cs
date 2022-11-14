using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;

namespace GH_GUI
{
    public class MenuStaticText : GH_Attr_Widget
    {
        private Font _font;

        private string _text;

        public Font Font
        {
            get
            {
                return this._font;
            }
            set
            {
                this._font = value;
            }
        }

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

        public MenuStaticText()
        {
            this.width = 0f;
            this._font = new Font(new FontFamily("Arial"), 8f, FontStyle.Bold);
        }

        public override bool Write(GH_IWriter writer)
        {
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
            SolidBrush brush = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
            PointF point = new PointF(this.parent.Transform.X + this.pivot.X, this.parent.Transform.Y + this.pivot.Y);
            graphics.DrawString(this._text, this._font, brush, point);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Release;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Handled;
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Capture;
        }
    }
}
