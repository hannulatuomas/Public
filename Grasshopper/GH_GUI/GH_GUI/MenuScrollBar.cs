using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;

namespace GH_GUI
{
    public class MenuScrollBar : GH_Attr_Widget
    {
        private Rectangle _content;

        private Rectangle _drag;

        private float _height;

        public bool _active;

        public int numItems;

        public int numVisibleItems;

        private PointF _clickPos;

        private float _localTop;

        private float _localBottom;

        private float _dragHeight;

        public double _ratio;

        private float _currentHight;

        private int _startIndex;

        private int _endIndex;

        public int[] CurrentInterval
        {
            get
            {
                return new int[]
                {
                    this._startIndex,
                    this._endIndex
                };
            }
        }

        public float Height
        {
            get
            {
                return this._height;
            }
            set
            {
                this._height = value;
            }
        }

        public void SetClick(PointF click)
        {
            this._clickPos = click;
            this._dragHeight = this._height * (float)this._ratio;
            float num = this._clickPos.Y - this.transfromation.Y;
            this._localTop = num - this._currentHight;
            this._localBottom = this._dragHeight - this._localTop;
        }

        public override void Render(GH_Canvas canvas, GH_CanvasChannel channel)
        {
            Graphics graphics = canvas.Graphics;
            graphics.FillRectangle(Brushes.Gray, this._content);
            graphics.FillRectangle(Brushes.Black, this._drag);
        }

        public override void LayoutInternal()
        {
            PointF transfromation = new PointF((float)((int)this.parent.Transform.X + (int)base.Pivot.X), (float)((int)this.parent.Transform.Y + (int)base.Pivot.Y));
            this.transfromation = transfromation;
            this._content = new Rectangle((int)this.transfromation.X, (int)this.transfromation.Y, (int)this.width, (int)this._height);
            this._drag = new Rectangle((int)this.transfromation.X, (int)this.transfromation.Y + (int)this._currentHight, (int)this.width, (int)((double)this._height * this._ratio));
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            float y = e.CanvasLocation.Y;
            float num = y - this.transfromation.Y;
            float num2 = num - this._localTop;
            float num3 = num + this._localBottom;
            if (num2 < 0f)
            {
                this._currentHight = 0f;
                this._startIndex = 0;
                this._endIndex = this.numVisibleItems;
            }
            else if (num3 > this._height)
            {
                this._currentHight = this._height - this._dragHeight;
                this._startIndex = this.numItems - this.numVisibleItems;
                this._endIndex = this.numItems;
            }
            else
            {
                this._currentHight = num2;
                this._startIndex = (int)(this._currentHight / this._height * (float)this.numItems);
                this._endIndex = this._startIndex + this.numVisibleItems;
            }
            return GH_ObjectResponse.Capture;
        }

        public void Update()
        {
            if (this._currentHight == 0f)
            {
                this._startIndex = 0;
                this._endIndex = this.numVisibleItems;
                return;
            }
            if (this._currentHight == this._height - this._dragHeight)
            {
                this._startIndex = this.numItems - this.numVisibleItems;
                this._endIndex = this.numItems;
                return;
            }
            this._startIndex = (int)(this._currentHight / this._height * (float)this.numItems);
            this._endIndex = this._startIndex + this.numVisibleItems;
        }

        public void SetSlider(int start, int length)
        {
            this._startIndex = start;
            this._endIndex = start + length;
            double num = (double)start / (double)this.numItems * (double)this._height;
            this._currentHight = (float)num;
            this._dragHeight = (float)(this._ratio * (double)this._height);
        }

        public override bool Contains(PointF pt)
        {
            return this._content.Contains(new Point((int)pt.X, (int)pt.Y));
        }
    }
}
