using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Rhino;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GH_GUI
{
    public class GH_MenuCollection
    {
        private List<GH_ExtendableMenu> _menus;

        private PointF pivot;

        private float width;

        private GH_PaletteStyle style;

        private GH_Palette palette;

        private GH_Attr_Widget _activeWidget;

        public List<GH_ExtendableMenu> Menus
        {
            get
            {
                return this._menus;
            }
            set
            {
                this._menus = value;
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

        public float Width
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

        public GH_Attr_Widget ActiveWidget
        {
            get
            {
                return this._activeWidget;
            }
            set
            {
                this._activeWidget = value;
            }
        }

        public float Height
        {
            get
            {
                float num = 0f;
                for (int i = 0; i < this._menus.Count; i++)
                {
                    num += this._menus[i].TotalHeight;
                }
                return num;
            }
        }

        public GH_MenuCollection()
        {
            this._menus = new List<GH_ExtendableMenu>();
        }

        public void Merge(GH_MenuCollection other)
        {
            for (int i = 0; i < other._menus.Count; i++)
            {
                this.AddMenu(other._menus[i]);
            }
        }

        public void GetMenuPlugs(ref List<ExtendedPlug> inputs, ref List<ExtendedPlug> outputs, bool onlyVisible)
        {
            for (int i = 0; i < this._menus.Count; i++)
            {
                if (!onlyVisible || (onlyVisible && this._menus[i].Expanded))
                {
                    inputs.AddRange(this._menus[i].Inputs);
                    outputs.AddRange(this._menus[i].Outputs);
                }
            }
        }

        public void AddMenu(GH_ExtendableMenu _menu)
        {
            _menu.Index = this._menus.Count;
            _menu.Collection = this;
            this._menus.Add(_menu);
        }

        public void CollapseAllMenu()
        {
            this.MakeAllInActive();
            for (int i = 0; i < this._menus.Count; i++)
            {
                this._menus[i].Collapse();
            }
        }

        public void MakeAllInActive()
        {
            for (int i = 0; i < this._menus.Count; i++)
            {
                this._menus[i].MakeAllInActive();
            }
        }

        public GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            for (int i = 0; i < this._menus.Count; i++)
            {
                GH_ObjectResponse gH_ObjectResponse = this._menus[i].RespondToMouseDoubleClick(sender, e);
                if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
                {
                    return gH_ObjectResponse;
                }
            }
            return GH_ObjectResponse.Ignore;
        }

        public GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this._activeWidget != null)
            {
                return this._activeWidget.RespondToMouseUp(sender, e);
            }
            for (int i = 0; i < this._menus.Count; i++)
            {
                GH_ObjectResponse gH_ObjectResponse = this._menus[i].RespondToMouseUp(sender, e);
                if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
                {
                    return gH_ObjectResponse;
                }
            }
            return GH_ObjectResponse.Ignore;
        }

        public GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this._activeWidget != null)
            {
                return this._activeWidget.RespondToMouseMove(sender, e);
            }
            int count = this._menus.Count;
            for (int i = 0; i < count; i++)
            {
                GH_ObjectResponse gH_ObjectResponse = this._menus[i].RespondToMouseMove(sender, e);
                if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
                {
                    return gH_ObjectResponse;
                }
            }
            return GH_ObjectResponse.Ignore;
        }

        public GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (this._activeWidget != null)
            {
                return this._activeWidget.RespondToMouseDown(sender, e);
            }
            int count = this._menus.Count;
            for (int i = 0; i < count; i++)
            {
                GH_ObjectResponse gH_ObjectResponse = this._menus[i].RespondToMouseDown(sender, e);
                if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
                {
                    return gH_ObjectResponse;
                }
            }
            return GH_ObjectResponse.Ignore;
        }

        public GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            if (this._activeWidget != null)
            {
                return null;
            }
            int count = this._menus.Count;
            for (int i = 0; i < count; i++)
            {
                GH_Attr_Widget gH_Attr_Widget = this._menus[i].IsTtipPoint(pt);
                if (gH_Attr_Widget != null)
                {
                    return gH_Attr_Widget;
                }
            }
            return null;
        }

        public GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
        {
            if (this._activeWidget != null)
            {
                return this._activeWidget.RespondToKeyDown(sender, e);
            }
            return GH_ObjectResponse.Ignore;
        }

        public bool Write(GH_IWriter writer)
        {
            GH_IWriter gH_IWriter = writer.CreateChunk("Collection");
            for (int i = 0; i < this._menus.Count; i++)
            {
                GH_IWriter writer2 = gH_IWriter.CreateChunk("Menu", i);
                this._menus[i].Write(writer2);
            }
            return true;
        }

        public bool Read(GH_IReader reader)
        {
            GH_IReader gH_IReader = reader.FindChunk("Collection");
            if (gH_IReader == null)
            {
                Rhino.RhinoApp.WriteLine("Invalid menu collection");
                return false;
            }
            for (int i = 0; i < this._menus.Count; i++)
            {
                GH_IReader gH_IReader2 = gH_IReader.FindChunk("Menu", i);
                if (gH_IReader2 == null)
                {
                    return false;
                }
                this._menus[i].Read(gH_IReader2);
            }
            return true;
        }

        public void Layout()
        {
            int num = 0;
            int count = this._menus.Count;
            for (int i = 0; i < count; i++)
            {
                this._menus[i].Pivot = new PointF(this.pivot.X, this.pivot.Y + (float)num);
                this._menus[i].Width = this.width;
                this._menus[i].Style = this.style;
                this._menus[i].Palette = this.palette;
                this._menus[i].LayoutInternal();
                num += (int)this._menus[i].TotalHeight;
            }
        }

        public void Render(GH_Canvas canvas, GH_CanvasChannel channel)
        {
            int count = this._menus.Count;
            for (int i = 0; i < count; i++)
            {
                this._menus[i].Render(canvas, channel);
            }
            if (this._activeWidget != null)
            {
                this._activeWidget.Render(canvas, channel);
            }
        }
    }
}
