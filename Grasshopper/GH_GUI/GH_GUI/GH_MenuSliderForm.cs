using Grasshopper.GUI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GH_GUI
{
    public class GH_MenuSliderForm : Form
    {
        private Button btnCancel;

        private Button btnOK;

        private GroupBox grpDomain;

        private Label lblLower;

        private Label lblUpper;

        private Label lblValue;

        private NumericUpDown numLower;

        private NumericUpDown numUpper;

        private NumericUpDown numValue;

        private Panel Panel1;

        private Panel Panel2;

        private Panel pnSep1;

        private TableLayoutPanel tblDomain;

        private TableLayoutPanel tblType;

        private TableLayoutPanel tblValue;

        private MenuSlider _slider;

        private double _minValue;

        private double _maxValue = 1.0;

        private double _value;

        private double _increment = 1.0;

        private int _precision = 3;

        public GH_MenuSliderForm(MenuSlider slider)
        {
            this._slider = slider;
            base.Load += new EventHandler(this.GH_DoubleSliderPopup_Load);
            this.InitializeComponent();
            this.update();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.numLower.Value > this.numUpper.Value)
            {
                this.numLower.Value = this.numUpper.Value - -10m;
            }
            if (this.numValue.Value < this.numLower.Value)
            {
                this.numValue.Value = this.numLower.Value;
            }
            else if (this.numValue.Value > this.numUpper.Value)
            {
                this.numValue.Value = this.numUpper.Value;
            }
            this._slider.MinValue = (double)this.numLower.Value;
            this._slider.MaxValue = (double)this.numUpper.Value;
            this._slider.Value = (double)this.numValue.Value;
            this._slider.newChangedEvent();
        }

        private void GH_DoubleSliderPopup_Load(object sender, EventArgs e)
        {
            GH_WindowsControlUtil.FixTextRenderingDefault(base.Controls);
        }

        private void InitializeComponent()
        {
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.numUpper = new NumericUpDown();
            this.numLower = new NumericUpDown();
            this.Panel2 = new Panel();
            this.numValue = new NumericUpDown();
            this.lblValue = new Label();
            this.tblType = new TableLayoutPanel();
            this.grpDomain = new GroupBox();
            this.tblDomain = new TableLayoutPanel();
            this.lblLower = new Label();
            this.lblUpper = new Label();
            this.tblValue = new TableLayoutPanel();
            this.pnSep1 = new Panel();
            this.Panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numUpper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLower)).BeginInit();
            this.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numValue)).BeginInit();
            this.grpDomain.SuspendLayout();
            this.tblDomain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Dock = DockStyle.Right;
            this.btnOK.Location = new Point(125, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(80, 24);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Dock = DockStyle.Right;
            this.btnCancel.Location = new Point(205, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(80, 24);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // numUpper
            // 
            this.numUpper.DecimalPlaces = this._precision;
            this.numUpper.Increment = (decimal)this._increment;
            this.numUpper.Dock = DockStyle.Fill;
            this.numUpper.Location = new Point(99, 22);
            this.numUpper.Margin = new Padding(0);
            this.numUpper.Maximum = decimal.MaxValue;
            this.numUpper.Minimum = decimal.MinValue;
            this.numUpper.Name = "numUpper";
            this.numUpper.Size = new Size(186, 20);
            this.numUpper.TabIndex = 5;
            this.numUpper.TextAlign = HorizontalAlignment.Center;
            this.numUpper.ThousandsSeparator = true;
            // 
            // numLower
            // 
            this.numLower.DecimalPlaces = this._precision;
            this.numLower.Increment = (decimal)this._increment;
            this.numLower.Dock = DockStyle.Fill;
            this.numLower.Location = new Point(99, 0);
            this.numLower.Margin = new Padding(0);
            this.numLower.Maximum = decimal.MaxValue;
            this.numLower.Minimum = decimal.MinValue;
            this.numLower.Name = "numLower";
            this.numLower.Size = new Size(186, 20);
            this.numLower.TabIndex = 3;
            this.numLower.TextAlign = HorizontalAlignment.Center;
            this.numLower.ThousandsSeparator = true;
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.btnOK);
            this.Panel2.Controls.Add(this.btnCancel);
            this.Panel2.Dock = DockStyle.Bottom;
            this.Panel2.Location = new Point(0, 155);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new Size(285, 24);
            this.Panel2.TabIndex = 9;
            // 
            // numValue
            // 
            this.numValue.DecimalPlaces = 4;
            this.numValue.DecimalPlaces = this._precision;
            this.numValue.Increment = (decimal)this._increment;
            this.numValue.Dock = DockStyle.Fill;
            this.numValue.Location = new Point(99, 44);
            this.numValue.Margin = new Padding(0);
            this.numValue.Maximum = decimal.MaxValue;
            this.numValue.Minimum = decimal.MinValue;
            this.numValue.Name = "numValue";
            this.numValue.Size = new Size(186, 20);
            this.numValue.TabIndex = 11;
            this.numValue.TextAlign = HorizontalAlignment.Center;
            // 
            // lblValue
            // 
            this.lblValue.Dock = DockStyle.Fill;
            this.lblValue.Location = new Point(0, 44);
            this.lblValue.Margin = new Padding(0);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new Size(99, 23);
            this.lblValue.TabIndex = 4;
            this.lblValue.Text = "Value";
            this.lblValue.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tblType
            // 
            this.tblType.Location = new Point(0, 0);
            this.tblType.Name = "tblType";
            this.tblType.Size = new Size(200, 100);
            this.tblType.TabIndex = 0;
            // 
            // grpDomain
            // 
            this.grpDomain.Controls.Add(this.tblDomain);
            this.grpDomain.Dock = DockStyle.Top;
            this.grpDomain.Location = new Point(0, 24);
            this.grpDomain.Name = "grpDomain";
            this.grpDomain.Padding = new Padding(0);
            this.grpDomain.Size = new Size(285, 80);
            this.grpDomain.TabIndex = 12;
            this.grpDomain.TabStop = false;
            this.grpDomain.Text = "Numeric domain";
            // 
            // tblDomain
            // 
            this.tblDomain.ColumnCount = 2;
            this.tblDomain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            this.tblDomain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            this.tblDomain.Controls.Add(this.lblLower, 0, 0);
            this.tblDomain.Controls.Add(this.numLower, 1, 0);
            this.tblDomain.Controls.Add(this.lblUpper, 0, 1);
            this.tblDomain.Controls.Add(this.numUpper, 1, 1);
            this.tblDomain.Controls.Add(this.lblValue, 0, 2);
            this.tblDomain.Controls.Add(this.numValue, 1, 2);
            this.tblDomain.Dock = DockStyle.Fill;
            this.tblDomain.Location = new Point(0, 13);
            this.tblDomain.Name = "tblDomain";
            this.tblDomain.RowCount = 3;
            this.tblDomain.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            this.tblDomain.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            this.tblDomain.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            this.tblDomain.Size = new Size(285, 67);
            this.tblDomain.TabIndex = 0;
            // 
            // lblLower
            // 
            this.lblLower.Dock = DockStyle.Fill;
            this.lblLower.Location = new Point(0, 0);
            this.lblLower.Margin = new Padding(0);
            this.lblLower.Name = "lblLower";
            this.lblLower.Size = new Size(99, 22);
            this.lblLower.TabIndex = 4;
            this.lblLower.Text = "Lower limit";
            this.lblLower.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblUpper
            // 
            this.lblUpper.Dock = DockStyle.Fill;
            this.lblUpper.Location = new Point(0, 22);
            this.lblUpper.Margin = new Padding(0);
            this.lblUpper.Name = "lblUpper";
            this.lblUpper.Size = new Size(99, 22);
            this.lblUpper.TabIndex = 12;
            this.lblUpper.Text = "Upper limit";
            this.lblUpper.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tblValue
            // 
            this.tblValue.Location = new Point(0, 0);
            this.tblValue.Name = "tblValue";
            this.tblValue.Size = new Size(200, 100);
            this.tblValue.TabIndex = 0;
            // 
            // pnSep1
            // 
            this.pnSep1.Dock = DockStyle.Top;
            this.pnSep1.Location = new Point(0, 0);
            this.pnSep1.Name = "pnSep1";
            this.pnSep1.Size = new Size(285, 24);
            this.pnSep1.TabIndex = 14;
            // 
            // Panel1
            // 
            this.Panel1.Dock = DockStyle.Top;
            this.Panel1.Location = new Point(0, 104);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new Size(285, 50);
            this.Panel1.TabIndex = 15;
            // 
            // GH_MenuSliderForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(285, 179);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.grpDomain);
            this.Controls.Add(this.Panel2);
            this.Controls.Add(this.pnSep1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(80, 39);
            this.Name = "GH_MenuSliderForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.StartPosition = FormStartPosition.Manual;
            this.Text = "SliderSettings";
            ((System.ComponentModel.ISupportInitialize)(this.numUpper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLower)).EndInit();
            this.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numValue)).EndInit();
            this.grpDomain.ResumeLayout(false);
            this.tblDomain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void update()
        {
            this._minValue = this._slider.MinValue;
            this._maxValue = this._slider.MaxValue;
            this._value = this._slider.Value;
            this.numLower.Value = new decimal(this._minValue);
            this.numUpper.Value = new decimal(this._maxValue);
            this.numValue.Value = new decimal(this._value);
        }
    }
}
