using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GH_GUI
{
    public class MenuDropDown : GH_Attr_Widget
    {
        private string tag;

        private MenuDropDownWindow _window;

        private float height;

        public bool expanded;

        private static int default_item_index;

        private int current_value;

        private int last_valid_value;

        private int _visibleItemCount = 4;

        private List<MenuDropItem> items;

        public event ValueChangeEventHandler _valueChanged;

        public int Value
        {
            get
            {
                return this.current_value;
            }
            set
            {
                this.current_value = Math.Max(value, 0);
                this.last_valid_value = ((value >= 0) ? value : 0);
            }
        }

        public int lastValidValue
        {
            get
            {
                return this.last_valid_value;
            }
        }

        public List<MenuDropItem> Items
        {
            get
            {
                return this.items;
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
                this._window.Width = value;
            }
        }

        private bool Empty
        {
            get
            {
                return this.items.Count == 0;
            }
        }

        public int VisibleItemCount
        {
            get
            {
                return this._visibleItemCount;
            }
            set
            {
                if (this._visibleItemCount < 1)
                {
                    this._visibleItemCount = 1;
                    return;
                }
                this._visibleItemCount = value;
            }
        }

        public int findIndex(string name)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i].content.Equals(name))
                {
                    return i;
                }
            }
            return -1;
        }

        public MenuDropDown(string tag)
        {
            this.tag = tag;
            this.items = new List<MenuDropItem>();
            this._window = new MenuDropDownWindow();
            this._window.Parent = this;
            this._window.Pivot = new PointF(0f, 23f);
            this._window._DropParent = this;
        }

        public void init()
        {
            this.update();
        }

        public void addItem(string cont)
        {
            MenuDropItem item = new MenuDropItem(cont, this.items.Count);
            this.items.Add(item);
            this.update();
        }

        public void addItem(string cont, object data)
        {
            MenuDropItem menuDropItem = new MenuDropItem(cont, this.items.Count);
            menuDropItem.data = data;
            this.items.Add(menuDropItem);
            this.update();
        }

        private void update()
        {
            if (this.items.Count == 0)
            {
                this.current_value = 0;
            }
            this._window.update();
            this.height = this._window.Height;
        }

        public override void LayoutInternal()
        {
            this.transfromation = new PointF((float)((int)this.parent.Transform.X + (int)base.Pivot.X), (float)((int)this.parent.Transform.Y + (int)base.Pivot.Y));
            this._window.LayoutInternal();
        }

        public void clear()
        {
            this.items.Clear();
            this.update();
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
            SolidBrush brush = new SolidBrush(Color.FromArgb(num, 90, 90, 90));
            SolidBrush brush2 = new SolidBrush(Color.FromArgb(num, 150, 150, 150));
            SolidBrush brush3 = new SolidBrush(Color.FromArgb(num, 0, 0, 0));
            SolidBrush brush4 = new SolidBrush(Color.FromArgb(num, 255, 255, 255));
            Pen pen = new Pen(brush3);
            Rectangle rect = new Rectangle((int)this.parent.Transform.X + (int)base.Pivot.X, (int)this.parent.Transform.Y + (int)base.Pivot.Y, (int)this.Width, 20);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            Font font = new Font("Arial", 10f, FontStyle.Italic);
            if (this.Empty)
            {
                graphics.DrawRectangle(pen, rect);
                graphics.FillRectangle(brush2, rect);
                graphics.DrawString("empty", font, brush, this.parent.Transform.X + base.Pivot.X + this.Width / 2f, this.parent.Transform.Y + base.Pivot.Y + 2f, stringFormat);
                return;
            }
            graphics.DrawRectangle(pen, rect);
            graphics.FillRectangle(brush4, rect);
            Rectangle rect2 = new Rectangle((int)this.parent.Transform.X + (int)base.Pivot.X + (int)this.Width - 10, (int)this.parent.Transform.Y + (int)base.Pivot.Y, 10, 20);
            graphics.FillRectangle(brush4, rect);
            graphics.DrawRectangle(pen, rect2);
            graphics.DrawString(this.items[this.current_value].content, font, brush3, this.parent.Transform.X + base.Pivot.X + this.Width / 2f, this.parent.Transform.Y + base.Pivot.Y + 2f, stringFormat);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this.Empty)
            {
                return GH_ObjectResponse.Release;
            }
            if (this.expanded)
            {
                return GH_ObjectResponse.Capture;
            }
            return GH_ObjectResponse.Ignore;
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Capture;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this.Empty)
            {
                return GH_ObjectResponse.Handled;
            }
            if (!this.expanded)
            {
                this.Expand();
                return GH_ObjectResponse.Capture;
            }
            if (this.Contains(e.CanvasLocation))
            {
                int num = (int)((e.CanvasLocation.Y - this.parent.Transform.Y - base.Pivot.Y) / 20f);
                this.current_value = num;
                this.Colapse(true);
                return GH_ObjectResponse.Handled;
            }
            this.Colapse(false);
            return GH_ObjectResponse.Handled;
        }

        public void Expand()
        {
            this.expanded = true;
            this.TopCollection.ActiveWidget = this._window;
            this.update();
        }

        public void Colapse(bool update)
        {
            this.expanded = false;
            this.TopCollection.ActiveWidget = null;
            this.TopCollection.MakeAllInActive();
            if (update && this._valueChanged != null)
            {
                this._valueChanged(this, new EventArgs());
            }
        }

        public override bool Contains(PointF pt)
        {
            if (this.expanded)
            {
                new RectangleF(this.parent.Transform.X + base.Pivot.X, this.parent.Transform.Y + base.Pivot.Y, this.Width, 20f);
                RectangleF rectangleF = new RectangleF(this.parent.Transform.X + base.Pivot.X, this.parent.Transform.Y + base.Pivot.Y + 22f, this.Width, this.height);
                if (rectangleF.Contains(pt))
                {
                    return true;
                }
            }
            else
            {
                RectangleF rectangleF2 = new RectangleF(this.parent.Transform.X + base.Pivot.X, this.parent.Transform.Y + base.Pivot.Y, this.Width, 20f);
                if (rectangleF2.Contains(pt))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Write(GH_IWriter writer)
        {
            GH_IWriter gH_IWriter = writer.CreateChunk("MenuDropDown", this.Index);
            gH_IWriter.SetInt32("ActiveItemIndex", this.current_value);
            return true;
        }

        public override bool Read(GH_IReader reader)
        {
            GH_IReader gH_IReader = reader.FindChunk("MenuDropDown", this.Index);
            try
            {
                this.current_value = gH_IReader.GetInt32("ActiveItemIndex");
            }
            catch
            {
                this.current_value = MenuDropDown.default_item_index;
            }
            return true;
        }
    }
}
