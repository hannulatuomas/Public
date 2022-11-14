using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GH_GUI
{
    public class MenuSlider : GH_Attr_Widget
    {
        private double _step;

        private double currentVal;

        private float accWidth;

        private double maxValue = 1.0;

        private double minValue;

        private RectangleF Control;

        private string _number_format = "{0:0.00}";

        public event ValueChangeEventHandler _valueChanged;

        public string numberFormat
        {
            get
            {
                return this._number_format;
            }
            set
            {
                this._number_format = value;
            }
        }

        public double Step
        {
            get
            {
                return this._step;
            }
            set
            {
                this._step = value;
            }
        }

        public double Value
        {
            get
            {
                return this.currentVal;
            }
            set
            {
                if (value > this.maxValue)
                {
                    this.currentVal = this.maxValue;
                    return;
                }
                if (value < this.minValue)
                {
                    this.currentVal = this.minValue;
                    return;
                }
                this.currentVal = value;
            }
        }

        public double MaxValue
        {
            get
            {
                return this.maxValue;
            }
            set
            {
                this.maxValue = value;
                if (this.currentVal > this.maxValue)
                {
                    this.currentVal = this.maxValue;
                }
            }
        }

        public double MinValue
        {
            get
            {
                return this.minValue;
            }
            set
            {
                this.minValue = value;
                if (this.currentVal < this.minValue)
                {
                    this.currentVal = this.minValue;
                }
            }
        }

        public override float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
                this.accWidth = this.width - 10f;
            }
        }

        public MenuSlider(float width)
        {
            this.currentVal = 0.5;
            this.Width = width;
            this.accWidth = width - 10f;
        }

        public override bool Write(GH_IWriter writer)
        {
            GH_IWriter gH_IWriter = writer.CreateChunk("Slider", this.Index);
            gH_IWriter.SetDouble("MinValue", this.minValue);
            gH_IWriter.SetDouble("MaxValue", this.maxValue);
            gH_IWriter.SetDouble("CurrentValue", this.currentVal);
            return true;
        }

        public override bool Read(GH_IReader reader)
        {
            GH_IReader gH_IReader = reader.FindChunk("Slider", this.Index);
            this.minValue = gH_IReader.GetDouble("MinValue");
            this.maxValue = gH_IReader.GetDouble("MaxValue");
            this.currentVal = gH_IReader.GetDouble("CurrentValue");
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
            int alpha = (int)((double)num * 0.33);
            Pen pen;
            Pen pen2;
            SolidBrush brush;
            SolidBrush brush2;
            if (this._enabled)
            {
                pen = new Pen(Color.FromArgb(alpha, 0, 0, 0));
                pen2 = new Pen(Color.FromArgb(num, 0, 0, 0), 2f);
                brush = new SolidBrush(Color.FromArgb(num, 255, 255, 255));
                brush2 = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
            }
            else
            {
                pen = new Pen(Color.FromArgb(alpha, 50, 50, 50));
                pen2 = new Pen(Color.FromArgb(num, 50, 50, 50), 2f);
                brush = new SolidBrush(Color.FromArgb(num, 150, 150, 150));
                brush2 = new SolidBrush(Color.FromArgb(num, 50, 50, 50));
            }
            this.Update();
            graphics.DrawLine(pen, this.transfromation.X + 5f, this.transfromation.Y + 5f, this.transfromation.X + this.Width - 5f, this.transfromation.Y + 5f);
            graphics.FillEllipse(brush, this.Control);
            graphics.DrawEllipse(pen2, this.Control);
            Font font = new Font("Arial", 8f, FontStyle.Regular);
            string text = string.Format(this._number_format, this.currentVal);
            float num3 = this.Control.X - (this.parent.Transform.X + this.pivot.X);
            SizeF sizeF = graphics.MeasureString(text, font);
            PointF point;
            if (num3 + sizeF.Width + 14f < this.Width)
            {
                point = new PointF(this.Control.Location.X + 14f, this.Control.Location.Y - 4f);
            }
            else
            {
                point = new PointF(this.Control.Location.X - 2f - sizeF.Width, this.Control.Location.Y - 4f);
            }
            graphics.DrawString(text, font, brush2, point);
        }

        public override void LayoutInternal()
        {
            this.transfromation = new PointF((float)((int)this.parent.Transform.X + (int)base.Pivot.X), (float)((int)this.parent.Transform.Y + (int)base.Pivot.Y));
        }

        private void Update()
        {
            if (this._step == 0.0)
            {
                double num = this.maxValue - this.minValue;
                double num2 = (this.currentVal - this.minValue) / num;
                float num3 = (float)((double)this.accWidth * num2);
                this.Control = new RectangleF(this.transfromation.X + num3, this.transfromation.Y, 10f, 10f);
                return;
            }
            this.currentVal = (double)((int)((this.currentVal + this._step / 2.0) / this._step)) * this._step;
            double num4 = this.maxValue - this.minValue;
            double num5 = (this.currentVal - this.minValue) / num4;
            float num6 = (float)((double)this.accWidth * num5);
            this.Control = new RectangleF(this.transfromation.X + num6, this.transfromation.Y, 10f, 10f);
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
            PointF pointF = new PointF(this.pivot.X + this.parent.Transform.X, this.pivot.Y + this.parent.Transform.Y);
            if (e.CanvasLocation.X < pointF.X + 5f)
            {
                this.currentVal = this.minValue;
            }
            else if (e.CanvasLocation.X > pointF.X + 5f + this.accWidth)
            {
                this.currentVal = this.maxValue;
            }
            else
            {
                double num = (double)(e.CanvasLocation.X - (pointF.X + 5f));
                double num2 = num / (double)this.accWidth;
                double num3 = this.maxValue - this.minValue;
                double num4 = num3 * num2;
                this.currentVal = num4 + this.minValue;
            }
            this.Update();
            this.newChangedEvent();
            return GH_ObjectResponse.Capture;
        }

        public void newChangedEvent()
        {
            if (this._valueChanged != null)
            {
                this._valueChanged(this, new EventArgs());
            }
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_MenuSliderForm gH_MenuSliderForm = new GH_MenuSliderForm(this);
            GH_WindowsFormUtil.CenterFormOnCursor(gH_MenuSliderForm, true);
            gH_MenuSliderForm.ShowDialog(Instances.DocumentEditor);
            return GH_ObjectResponse.Ignore;
        }

        public override GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(this.transfromation.X, this.transfromation.Y, this.width, 10f);
            if (rectangleF.Contains(pt))
            {
                return this;
            }
            return null;
        }

        public override void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            e.Icon = null;
            e.Title = this._name + " (Slider)";
            e.Text = this._header;
            e.Description = string.Concat(new object[]
            {
                "Value: ",
                string.Format(this._number_format, this.currentVal),
                "\nMinValue: ",
                this.minValue,
                "\nMaxValue: ",
                this.maxValue
            });
        }

        public override bool Contains(PointF pt)
        {
            return this.Control.Contains(pt);
        }
    }
}
