using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GH_GUI
{
    public abstract class GH_Attr_Widget
    {
        protected RectangleF bounds;

        protected float width;

        protected GH_Attr_Widget parent;

        protected PointF pivot;

        protected PointF transfromation;

        protected GH_PaletteStyle style;

        protected GH_Palette palette;

        protected int _index;

        protected bool _enabled = true;

        protected string _description;

        protected string _header;

        protected string _name;

        protected bool _showToolTip = true;

        public virtual bool ShowToolTip
        {
            get
            {
                return this._showToolTip;
            }
            set
            {
                this._showToolTip = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public virtual string Desciption
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        public virtual string Header
        {
            get
            {
                return this._header;
            }
            set
            {
                this._header = value;
            }
        }

        public virtual GH_Attr_Widget Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }

        public virtual float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public virtual int Index
        {
            get
            {
                return this._index;
            }
            set
            {
                this._index = value;
            }
        }

        public virtual bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                this._enabled = value;
            }
        }

        public virtual GH_MenuCollection TopCollection
        {
            get
            {
                GH_Attr_Widget gH_Attr_Widget = this.Parent.parent;
                while (!(gH_Attr_Widget is GH_ExtendableMenu) && gH_Attr_Widget.Parent != null)
                {
                    gH_Attr_Widget = gH_Attr_Widget.Parent;
                }
                if (gH_Attr_Widget != null)
                {
                    return ((GH_ExtendableMenu)gH_Attr_Widget).Collection;
                }
                return null;
            }
        }

        public GH_PaletteStyle Style
        {
            get
            {
                return this.style;
            }
            set
            {
                this.style = value;
            }
        }

        public GH_Palette Palette
        {
            get
            {
                return this.palette;
            }
            set
            {
                this.palette = value;
            }
        }

        public PointF Transform
        {
            get
            {
                return this.transfromation;
            }
            set
            {
                this.transfromation = value;
            }
        }

        public PointF Pivot
        {
            get
            {
                return this.pivot;
            }
            set
            {
                this.pivot = value;
            }
        }

        public abstract void Render(GH_Canvas canvas, GH_CanvasChannel channel);

        public virtual bool Contains(PointF pt)
        {
            return false;
        }

        public virtual GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Ignore;
        }

        public virtual GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Ignore;
        }

        public virtual GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Ignore;
        }

        public virtual GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            return GH_ObjectResponse.Ignore;
        }

        public virtual GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
        {
            return GH_ObjectResponse.Ignore;
        }

        public virtual bool Write(GH_IWriter writer)
        {
            return true;
        }

        public virtual bool Read(GH_IReader reader)
        {
            return true;
        }

        public virtual void LayoutInternal()
        {
        }

        public virtual GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            return null;
        }

        public virtual void TooltipSetup(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
        }
    }
}
