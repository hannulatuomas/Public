using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GH_GUI
{
    public class GH_ExtendableComponentAttributes : GH_ComponentAttributes
    {
        public int offset;

        public int currentOffset = 15;

        public bool expanded;

        private float _minWidth;

        private GH_Attr_Widget _activeToolTip;

        public GH_MenuCollection collection;

        public float MinWidth
        {
            get
            {
                return this._minWidth;
            }
            set
            {
                this._minWidth = value;
            }
        }

        public GH_ExtendableComponentAttributes(IGH_Component nComponent) : base(nComponent)
        {
            this.collection = new GH_MenuCollection();
        }

        public void AddMenu(GH_ExtendableMenu _menu)
        {
            this.collection.AddMenu(_menu);
        }

        public override bool Write(GH_IWriter writer)
        {
            try
            {
                this.collection.Write(writer);
            }
            catch (Exception ex)
            {
                Rhino.RhinoApp.WriteLine("FAIL Write: " + ex.Message + " " + ex.StackTrace);
            }
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            try
            {
                this.collection.Read(reader);
            }
            catch (Exception ex)
            {
                Rhino.RhinoApp.WriteLine("FAIL Read " + ex.Message + " " + ex.StackTrace);
            }
            return base.Read(reader);
        }

        protected override void PrepareForRender(GH_Canvas canvas)
        {
            base.PrepareForRender(canvas);
            this.LayoutStyle();
        }

        protected override void Layout()
        {
            this.Pivot = GH_Convert.ToPoint(this.Pivot);
            base.Layout();
            this.FixLayout();
            this.LayoutMenu();
        }

        protected void FixLayout()
        {
            float width = this.Bounds.Width;
            if (this._minWidth > width)
            {
                this.Bounds = new RectangleF(this.Bounds.X, this.Bounds.Y, this._minWidth, this.Bounds.Height);
            }
            float num = this.Bounds.Width - width;
            foreach (IGH_Param current in base.Owner.Params.Output)
            {
                PointF pivot = current.Attributes.Pivot;
                RectangleF bounds = current.Attributes.Bounds;
                current.Attributes.Pivot = new PointF(pivot.X + num, pivot.Y);
                current.Attributes.Bounds = new RectangleF(bounds.Location.X + num, bounds.Location.Y, bounds.Width, bounds.Height);
            }
        }

        private void LayoutStyle()
        {
            this.collection.Palette = GH_CapsuleRenderEngine.GetImpliedPalette(base.Owner);
            this.collection.Style = GH_CapsuleRenderEngine.GetImpliedStyle(this.collection.Palette, this.Selected, base.Owner.Locked, base.Owner.Hidden);
            this.collection.Layout();
        }

        protected void LayoutMenu()
        {
            this.offset = (int)this.Bounds.Height;
            this.collection.Pivot = new PointF(this.Bounds.X, (float)((int)this.Bounds.Y + this.offset));
            this.collection.Width = this.Bounds.Width;
            this.LayoutStyle();
            this.Bounds = new RectangleF(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + this.collection.Height);
        }

        protected override void Render(GH_Canvas iCanvas, Graphics graph, GH_CanvasChannel iChannel)
        {
            base.Render(iCanvas, graph, iChannel);
            if (iChannel == GH_CanvasChannel.Objects)
            {
                this.collection.Render(iCanvas, iChannel);
            }
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.collection.RespondToMouseUp(sender, e);
            if (gH_ObjectResponse == GH_ObjectResponse.Capture)
            {
                this.ExpireLayout();
                sender.Invalidate();
                return gH_ObjectResponse;
            }
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Invalidate();
                return GH_ObjectResponse.Release;
            }
            return base.RespondToMouseUp(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.collection.RespondToMouseDoubleClick(sender, e);
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Refresh();
                return gH_ObjectResponse;
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }

        public override GH_ObjectResponse RespondToKeyDown(GH_Canvas sender, KeyEventArgs e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.collection.RespondToKeyDown(sender, e);
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Refresh();
                return gH_ObjectResponse;
            }
            return base.RespondToKeyDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.collection.RespondToMouseMove(sender, e);
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Refresh();
                return gH_ObjectResponse;
            }
            return base.RespondToMouseMove(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            GH_ObjectResponse gH_ObjectResponse = this.collection.RespondToMouseDown(sender, e);
            if (gH_ObjectResponse != GH_ObjectResponse.Ignore)
            {
                this.ExpireLayout();
                sender.Refresh();
                return gH_ObjectResponse;
            }
            return base.RespondToMouseDown(sender, e);
        }

        public override bool IsTooltipRegion(PointF pt)
        {
            this._activeToolTip = null;
            bool flag = base.IsTooltipRegion(pt);
            if (flag)
            {
                return flag;
            }
            bool flag2 = this.m_innerBounds.Contains(pt);
            if (flag2)
            {
                GH_Attr_Widget gH_Attr_Widget = this.collection.IsTtipPoint(pt);
                if (gH_Attr_Widget != null)
                {
                    this._activeToolTip = gH_Attr_Widget;
                    return true;
                }
            }
            return false;
        }

        public bool getActiveTooltip(PointF pt)
        {
            GH_Attr_Widget gH_Attr_Widget = this.collection.IsTtipPoint(pt);
            if (gH_Attr_Widget != null)
            {
                this._activeToolTip = gH_Attr_Widget;
                return true;
            }
            return false;
        }

        public override void SetupTooltip(PointF canvasPoint, GH_TooltipDisplayEventArgs e)
        {
            this.getActiveTooltip(canvasPoint);
            if (this._activeToolTip != null)
            {
                this._activeToolTip.TooltipSetup(canvasPoint, e);
                return;
            }
            e.Title = this.PathName;
            e.Text = base.Owner.Description;
            e.Description = base.Owner.InstanceDescription;
            e.Icon = base.Owner.Icon_24x24;
            if (base.Owner is IGH_Param)
            {
                IGH_Param iGH_Param = (IGH_Param)base.Owner;
                string text = iGH_Param.TypeName;
                if (iGH_Param.Access == GH_ParamAccess.list)
                {
                    text += "[…]";
                }
                if (iGH_Param.Access == GH_ParamAccess.tree)
                {
                    text += "{…;…;…}";
                }
                e.Title = string.Format("{0} ({1})", this.PathName, text);
            }
        }
    }
}
