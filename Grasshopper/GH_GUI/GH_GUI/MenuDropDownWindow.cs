using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;

namespace GH_GUI
{
    public class MenuDropDownWindow : GH_Attr_Widget
    {
        private float height;

        private int _tempActive = -1;

        private int _tempStart;

        private double _ratio = 1.0;

        public MenuDropDown _DropParent;

        private MenuScrollBar _scrollBar;

        private Rectangle _resizeBox;

        private float _widthOffset;

        private bool _resizeActive;

        private int _maxLen;

        public float Height
        {
            get
            {
                return this.height;
            }
        }

        public MenuDropDownWindow()
        {
            this._scrollBar = new MenuScrollBar();
            this._scrollBar.Width = 10f;
            this._scrollBar.Parent = this;
        }

        public void clear()
        {
            this._tempActive = -1;
            this._tempStart = 0;
        }

        public void update()
        {
            int count = this._DropParent.Items.Count;
            int visibleItemCount = this._DropParent.VisibleItemCount;
            int num = Math.Min(visibleItemCount, count);
            if (this._DropParent.lastValidValue > count)
            {
                this._DropParent.Value = -1;
            }
            this._maxLen = num;
            this._ratio = (double)num / (double)count;
            this.height = (float)(num * 20);
            this._scrollBar.Height = this.height;
            this._scrollBar._ratio = this._ratio;
            this._scrollBar.numItems = count;
            this._scrollBar.numVisibleItems = num;
            this._scrollBar.SetSlider(this._tempStart, count);
            int[] arg_B3_0 = this._scrollBar.CurrentInterval;
        }

        public override void LayoutInternal()
        {
            PointF transfromation = new PointF((float)((int)this.parent.Transform.X + (int)base.Pivot.X), (float)((int)this.parent.Transform.Y + (int)base.Pivot.Y));
            this.transfromation = transfromation;
            this.update();
            this._scrollBar.Pivot = new PointF(this.width - 10f, 0f);
            this._scrollBar.Height = this.height;
            this._scrollBar._ratio = this._ratio;
            this._scrollBar.LayoutInternal();
            this._resizeBox = new Rectangle((int)base.Transform.X + (int)this.width + (int)this._widthOffset - 10, (int)base.Transform.Y + (int)this.height, 10, 10);
        }

        public override void Render(GH_Canvas canvas, GH_CanvasChannel channel)
        {
            Graphics graphics = canvas.Graphics;
            Rectangle rect = new Rectangle((int)base.Transform.X, (int)base.Transform.Y, (int)this.Width + (int)this._widthOffset, (int)this.height);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            Pen pen = new Pen(Brushes.Black);
            StringFormat stringFormat2 = new StringFormat();
            stringFormat2.Alignment = StringAlignment.Center;
            Font font = new Font("Arial", 10f, FontStyle.Italic);
            graphics.DrawRectangle(pen, rect);
            graphics.FillRectangle(Brushes.White, rect);
            int maxLen = this._maxLen;
            int num = 0;
            for (int i = this._tempStart; i < this._tempStart + maxLen; i++)
            {
                if (i == this._tempActive)
                {
                    Rectangle rect2 = new Rectangle((int)base.Transform.X, (int)base.Transform.Y + 20 * num, (int)this.Width + (int)this._widthOffset, 20);
                    graphics.FillRectangle(Brushes.Blue, rect2);
                    graphics.DrawString(this._DropParent.Items[i].content, font, new SolidBrush(Color.FromArgb(255, 255, 255, 255)), base.Transform.X + (this.Width + this._widthOffset) / 2f, base.Transform.Y + (float)(20 * num), stringFormat);
                }
                else if (i == this._DropParent.Value)
                {
                    Rectangle rect3 = new Rectangle((int)base.Transform.X, (int)base.Transform.Y + 20 * num, (int)this.Width + (int)this._widthOffset, 20);
                    graphics.FillRectangle(Brushes.LightBlue, rect3);
                    graphics.DrawString(this._DropParent.Items[i].content, font, new SolidBrush(Color.FromArgb(255, 0, 0, 0)), base.Transform.X + (this.Width + this._widthOffset) / 2f, (float)((int)base.Transform.Y + 20 * num), stringFormat);
                }
                else
                {
                    stringFormat.Alignment = StringAlignment.Center;
                    graphics.DrawString(this._DropParent.Items[i].content, font, new SolidBrush(Color.FromArgb(255, 0, 0, 0)), base.Transform.X + (this.Width + this._widthOffset) / 2f, base.Transform.Y + (float)(20 * num), stringFormat);
                }
                num++;
            }
            this._scrollBar.Render(canvas, channel);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(120, 80, 80, 80)), this._resizeBox);
            graphics.DrawString("+", font, new SolidBrush(Color.FromArgb(255, 0, 0, 0)), (float)(this._resizeBox.Location.X + 5), (float)(this._resizeBox.Location.Y - 3), stringFormat);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this._resizeActive || this._scrollBar._active)
            {
                this._scrollBar._active = false;
                this._resizeActive = false;
                return GH_ObjectResponse.Capture;
            }
            return GH_ObjectResponse.Release;
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this._scrollBar._active)
            {
                this._scrollBar.RespondToMouseMove(sender, e);
                int[] currentInterval = this._scrollBar.CurrentInterval;
                this._tempStart = currentInterval[0];
            }
            else if (this._resizeActive)
            {
                float num = e.CanvasLocation.Y - this.transfromation.Y;
                if (num > 20f)
                {
                    int num2 = (int)(num / 20f);
                    if (num2 + this._tempStart < this._DropParent.Items.Count)
                    {
                        this._DropParent.VisibleItemCount = num2;
                        this.update();
                    }
                    else if (num2 <= this._DropParent.Items.Count)
                    {
                        this._tempStart = this._DropParent.Items.Count - num2;
                        this._DropParent.VisibleItemCount = num2;
                        this.update();
                    }
                }
            }
            else if (this.Contains(e.CanvasLocation))
            {
                this._tempActive = this._tempStart + (int)((e.CanvasLocation.Y - base.Transform.Y) / 20f);
            }
            else
            {
                this._tempActive = -1;
            }
            return GH_ObjectResponse.Capture;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this._scrollBar.Contains(e.CanvasLocation))
            {
                this._scrollBar._active = true;
                this._scrollBar.SetClick(e.CanvasLocation);
                return GH_ObjectResponse.Capture;
            }
            this._scrollBar._active = false;
            if (this._resizeBox.Contains(new Point((int)e.CanvasLocation.X, (int)e.CanvasLocation.Y)))
            {
                this._resizeActive = true;
                return GH_ObjectResponse.Capture;
            }
            if (this.Contains(e.CanvasLocation))
            {
                this._DropParent.Value = this._tempStart + (int)((e.CanvasLocation.Y - base.Transform.Y) / 20f);
                this._tempActive = -1;
                ((MenuDropDown)this.parent).Colapse(true);
                return GH_ObjectResponse.Release;
            }
            ((MenuDropDown)this.parent).Colapse(false);
            return GH_ObjectResponse.Release;
        }

        public override bool Contains(PointF pt)
        {
            RectangleF rectangleF = new RectangleF(base.Transform.X, base.Transform.Y, this.width + this._widthOffset, this.height);
            return rectangleF.Contains(pt);
        }
    }
}
