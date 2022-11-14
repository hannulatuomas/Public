using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GH_GUI
{
    public class MenuRadioButtonGroup : GH_Attr_Widget
    {
        private List<MenuRadioButton> _buttons;

        private int minActive = 1;

        private int maxActive = 1;

        private List<MenuRadioButton> actives;

        private MenuRadioButton _activeControl;

        public event ValueChangeEventHandler _valueChanged;

        public override bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                this._enabled = value;
                int count = this._buttons.Count;
                for (int i = 0; i < count; i++)
                {
                    this._buttons[i].Enabled = this._enabled;
                }
            }
        }

        public override GH_Attr_Widget Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
                int count = this._buttons.Count;
                for (int i = 0; i < count; i++)
                {
                    this._buttons[i].Parent = value;
                }
            }
        }

        public int MaxActive
        {
            get
            {
                return this.maxActive;
            }
            set
            {
                if (value < 1)
                {
                    this.maxActive = 1;
                }
                else
                {
                    this.maxActive = value;
                }
                this.UpdateSettings();
            }
        }

        public int MinActive
        {
            get
            {
                return this.minActive;
            }
            set
            {
                if (value < 0)
                {
                    this.minActive = 0;
                }
                else
                {
                    this.minActive = value;
                }
                this.UpdateSettings();
            }
        }

        public MenuRadioButtonGroup()
        {
            this._buttons = new List<MenuRadioButton>();
            this.actives = new List<MenuRadioButton>();
        }

        public void AddButton(MenuRadioButton button)
        {
            if (button.Active && this.actives.Count < this.maxActive)
            {
                this.actives.Add(button);
            }
            if (this.parent != null)
            {
                button.Parent = this.parent;
            }
            button.Index = this._buttons.Count;
            button.Enabled = this._enabled;
            this._buttons.Add(button);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            int count = this._buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (this._buttons[i].Contains(e.CanvasLocation) && this._buttons[i] == this._activeControl)
                {
                    if (this._buttons[i].Active)
                    {
                        if (this.actives.Count - 1 >= this.minActive)
                        {
                            this._buttons[i].Active = false;
                            this.actives.Remove(this._buttons[i]);
                        }
                    }
                    else
                    {
                        if (this.actives.Count == this.maxActive)
                        {
                            this.actives[0].Active = false;
                            this.actives.RemoveAt(0);
                        }
                        this._buttons[i].Active = true;
                        this.actives.Add(this._buttons[i]);
                    }
                    if (this._valueChanged != null)
                    {
                        this._valueChanged(this, new EventArgs());
                    }
                    this.Update();
                }
            }
            return GH_ObjectResponse.Release;
        }

        public override bool Write(GH_IWriter writer)
        {
            GH_IWriter gH_IWriter = writer.CreateChunk("RadioButtonGroup", this.Index);
            int count = this._buttons.Count;
            gH_IWriter.SetInt32("Count", count);
            GH_IWriter gH_IWriter2 = gH_IWriter.CreateChunk("Active");
            for (int i = 0; i < count; i++)
            {
                gH_IWriter2.SetBoolean("button", i, this._buttons[i].Active);
            }
            return true;
        }

        public override bool Read(GH_IReader reader)
        {
            GH_IReader gH_IReader = reader.FindChunk("RadioButtonGroup", this.Index);
            int count = this._buttons.Count;
            gH_IReader.GetInt32("Count");
            GH_IReader gH_IReader2 = gH_IReader.FindChunk("Active");
            this.actives.Clear();
            for (int i = 0; i < count; i++)
            {
                bool boolean = gH_IReader2.GetBoolean("button", i);
                this._buttons[i].Active = boolean;
                if (boolean)
                {
                    this.actives.Add(this._buttons[i]);
                }
            }
            return true;
        }

        public override bool Contains(PointF pt)
        {
            int count = this._buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (this._buttons[i].Contains(pt))
                {
                    return true;
                }
            }
            return false;
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            int count = this._buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (this._buttons[i].Contains(e.CanvasLocation))
                {
                    this._activeControl = this._buttons[i];
                    this._buttons[i].RespondToMouseDown(sender, e);
                }
            }
            return GH_ObjectResponse.Handled;
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            new PointF(this.pivot.X + this.parent.Pivot.X, this.pivot.Y + this.parent.Pivot.Y);
            return GH_ObjectResponse.Capture;
        }

        public override GH_Attr_Widget IsTtipPoint(PointF pt)
        {
            int count = this._buttons.Count;
            for (int i = 0; i < count; i++)
            {
                GH_Attr_Widget gH_Attr_Widget = this._buttons[i].IsTtipPoint(pt);
                if (gH_Attr_Widget != null)
                {
                    return gH_Attr_Widget;
                }
            }
            return null;
        }

        public override void Render(GH_Canvas canvas, GH_CanvasChannel channel)
        {
            int count = this._buttons.Count;
            for (int i = 0; i < count; i++)
            {
                this._buttons[i].Render(canvas, channel);
            }
        }

        public List<MenuRadioButton> GetActive()
        {
            new List<MenuRadioButton>();
            return this.actives;
        }

        public List<bool> GetPattern()
        {
            List<bool> list = new List<bool>();
            int count = this._buttons.Count;
            for (int i = 0; i < count; i++)
            {
                list.Add(this._buttons[i].Active);
            }
            return list;
        }

        public List<int> GetActiveInt()
        {
            List<int> list = new List<int>();
            int count = this._buttons.Count;
            for (int i = 0; i < count; i++)
            {
                if (this._buttons[i].Active)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        public bool SetActive(int index)
        {
            if (index < this._buttons.Count)
            {
                if (!this._buttons[index].Active)
                {
                    this.actives.Add(this._buttons[index]);
                    this._buttons[index].Active = true;
                }
                return true;
            }
            return false;
        }

        private void UpdateSettings()
        {
            if (this.minActive > this.maxActive)
            {
                this.minActive = this.maxActive;
            }
        }

        private void Update()
        {
            int arg_0B_0 = this._buttons.Count;
            int count = this.actives.Count;
            if (count > this.maxActive)
            {
                int num = count - this.maxActive;
                for (int i = 0; i < num; i++)
                {
                    MenuRadioButton menuRadioButton = this.actives[0];
                    this.actives.RemoveAt(0);
                    menuRadioButton.Active = false;
                }
                return;
            }
            if (count < this.minActive)
            {
                int num2 = count - this.maxActive;
                int num3 = 0;
                for (int j = 0; j < num2; j++)
                {
                    if (!this.actives.Contains(this._buttons[num3]))
                    {
                        MenuRadioButton menuRadioButton2 = this._buttons[num3];
                        menuRadioButton2.Active = true;
                        this.actives.Add(menuRadioButton2);
                        j--;
                    }
                    num3++;
                }
            }
        }
    }
}
