namespace Stabiliteettilaskenta
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            ACadManager.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_connect = new System.Windows.Forms.Button();
            this.comboBox_ModelName = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.button_unlock = new System.Windows.Forms.Button();
            this.button_writeMembers = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_getSupportReactions = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button_getSupportForces = new System.Windows.Forms.Button();
            this.textBox_LoadingNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_LoadingType = new System.Windows.Forms.ComboBox();
            this.checkBox_lineSupports = new System.Windows.Forms.CheckBox();
            this.checkBox_nodalSupports = new System.Windows.Forms.CheckBox();
            this.tabPage_beamEndForces = new System.Windows.Forms.TabPage();
            this.button_getLineSupportReactions = new System.Windows.Forms.Button();
            this.button_getForceResultant = new System.Windows.Forms.Button();
            this.button_getSupportForceResultant = new System.Windows.Forms.Button();
            this.checkBox_beamEnd = new System.Windows.Forms.CheckBox();
            this.checkBox_beamStart = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_memberNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_loadingNo = new System.Windows.Forms.TextBox();
            this.tabPage_ACad = new System.Windows.Forms.TabPage();
            this.comboBox_deleteLayer = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.button_deleteLayer = new System.Windows.Forms.Button();
            this.button_pasteLayerSettings = new System.Windows.Forms.Button();
            this.button_copyLayerSettings = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox_beamHeight = new System.Windows.Forms.TextBox();
            this.button_beamsToRFem = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_crossSection = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_endHinge = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_startHinge = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_columnLength = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox_columnStartHeight = new System.Windows.Forms.TextBox();
            this.button_columnsToRFem = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox_BeamLayer = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_beamDirY = new System.Windows.Forms.RadioButton();
            this.radioButton_beamDirX = new System.Windows.Forms.RadioButton();
            this.button_markBeams = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_beamLayer = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox_ColumnLayer = new System.Windows.Forms.ComboBox();
            this.button_markColumns = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_columnLayer = new System.Windows.Forms.TextBox();
            this.button_GetLayers = new System.Windows.Forms.Button();
            this.label_Layer = new System.Windows.Forms.Label();
            this.comboBox_ArkLayer = new System.Windows.Forms.ComboBox();
            this.tabPage_RFemModel = new System.Windows.Forms.TabPage();
            this.button_modifyNodes = new System.Windows.Forms.Button();
            this.tabPage_RFemLoads = new System.Windows.Forms.TabPage();
            this.button_getLoadings = new System.Windows.Forms.Button();
            this.tabPage_RFemResults = new System.Windows.Forms.TabPage();
            this.tabPage_TeklaModel = new System.Windows.Forms.TabPage();
            this.button_AnalysisModelFix = new System.Windows.Forms.Button();
            this.button_beamToPanel = new System.Windows.Forms.Button();
            this.button_closeGap = new System.Windows.Forms.Button();
            this.button_setProfiles = new System.Windows.Forms.Button();
            this.button_setToLine = new System.Windows.Forms.Button();
            this.button_moveTsParts = new System.Windows.Forms.Button();
            this.button_testScpipt = new System.Windows.Forms.Button();
            this.button_modifySlabs = new System.Windows.Forms.Button();
            this.button_getProfiles = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_beams_RFemToTekla = new System.Windows.Forms.Button();
            this.checkBox_OnTop = new System.Windows.Forms.CheckBox();
            this.button_maxN = new System.Windows.Forms.Button();
            this.button_getUCS = new System.Windows.Forms.Button();
            this.button_setUCS = new System.Windows.Forms.Button();
            this.button_getSolid = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage_getSupportReactions.SuspendLayout();
            this.tabPage_beamEndForces.SuspendLayout();
            this.tabPage_ACad.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage_RFemModel.SuspendLayout();
            this.tabPage_RFemLoads.SuspendLayout();
            this.tabPage_TeklaModel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(15, 33);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(105, 45);
            this.button_connect.TabIndex = 0;
            this.button_connect.Text = "Connect";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // comboBox_ModelName
            // 
            this.comboBox_ModelName.FormattingEnabled = true;
            this.comboBox_ModelName.Location = new System.Drawing.Point(88, 6);
            this.comboBox_ModelName.Name = "comboBox_ModelName";
            this.comboBox_ModelName.Size = new System.Drawing.Size(378, 21);
            this.comboBox_ModelName.TabIndex = 28;
            this.comboBox_ModelName.Text = "ModelName";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 13);
            this.label12.TabIndex = 27;
            this.label12.Text = "Model Name:";
            // 
            // button_unlock
            // 
            this.button_unlock.Location = new System.Drawing.Point(644, 404);
            this.button_unlock.Name = "button_unlock";
            this.button_unlock.Size = new System.Drawing.Size(80, 23);
            this.button_unlock.TabIndex = 30;
            this.button_unlock.Text = "Unlock";
            this.button_unlock.UseVisualStyleBackColor = true;
            this.button_unlock.Click += new System.EventHandler(this.button_unlock_Click);
            // 
            // button_writeMembers
            // 
            this.button_writeMembers.Location = new System.Drawing.Point(595, 41);
            this.button_writeMembers.Name = "button_writeMembers";
            this.button_writeMembers.Size = new System.Drawing.Size(125, 53);
            this.button_writeMembers.TabIndex = 38;
            this.button_writeMembers.Text = "WriteMembers";
            this.button_writeMembers.UseVisualStyleBackColor = true;
            this.button_writeMembers.Click += new System.EventHandler(this.button_writeMembers_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_getSupportReactions);
            this.tabControl1.Controls.Add(this.tabPage_beamEndForces);
            this.tabControl1.Controls.Add(this.tabPage_ACad);
            this.tabControl1.Controls.Add(this.tabPage_RFemModel);
            this.tabControl1.Controls.Add(this.tabPage_RFemLoads);
            this.tabControl1.Controls.Add(this.tabPage_RFemResults);
            this.tabControl1.Controls.Add(this.tabPage_TeklaModel);
            this.tabControl1.Location = new System.Drawing.Point(33, 100);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(691, 285);
            this.tabControl1.TabIndex = 47;
            // 
            // tabPage_getSupportReactions
            // 
            this.tabPage_getSupportReactions.Controls.Add(this.richTextBox1);
            this.tabPage_getSupportReactions.Controls.Add(this.button_getSupportForces);
            this.tabPage_getSupportReactions.Controls.Add(this.textBox_LoadingNumber);
            this.tabPage_getSupportReactions.Controls.Add(this.label4);
            this.tabPage_getSupportReactions.Controls.Add(this.label5);
            this.tabPage_getSupportReactions.Controls.Add(this.comboBox_LoadingType);
            this.tabPage_getSupportReactions.Controls.Add(this.checkBox_lineSupports);
            this.tabPage_getSupportReactions.Controls.Add(this.checkBox_nodalSupports);
            this.tabPage_getSupportReactions.Location = new System.Drawing.Point(4, 22);
            this.tabPage_getSupportReactions.Name = "tabPage_getSupportReactions";
            this.tabPage_getSupportReactions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_getSupportReactions.Size = new System.Drawing.Size(683, 259);
            this.tabPage_getSupportReactions.TabIndex = 0;
            this.tabPage_getSupportReactions.Text = "GetSupportReactions";
            this.tabPage_getSupportReactions.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Enabled = false;
            this.richTextBox1.Location = new System.Drawing.Point(314, 6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(363, 247);
            this.richTextBox1.TabIndex = 54;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // button_getSupportForces
            // 
            this.button_getSupportForces.Location = new System.Drawing.Point(9, 160);
            this.button_getSupportForces.Name = "button_getSupportForces";
            this.button_getSupportForces.Size = new System.Drawing.Size(125, 53);
            this.button_getSupportForces.TabIndex = 53;
            this.button_getSupportForces.Text = "GetSupportForces";
            this.button_getSupportForces.UseVisualStyleBackColor = true;
            this.button_getSupportForces.Click += new System.EventHandler(this.button_getSupportForces_Click);
            // 
            // textBox_LoadingNumber
            // 
            this.textBox_LoadingNumber.Location = new System.Drawing.Point(100, 27);
            this.textBox_LoadingNumber.Name = "textBox_LoadingNumber";
            this.textBox_LoadingNumber.Size = new System.Drawing.Size(127, 20);
            this.textBox_LoadingNumber.TabIndex = 52;
            this.textBox_LoadingNumber.Text = "6";
            this.textBox_LoadingNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_LoadingNumber.TextChanged += new System.EventHandler(this.textBox_LoadingNumber_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 51;
            this.label4.Text = "Loading Number:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 50;
            this.label5.Text = "Loading Type:";
            // 
            // comboBox_LoadingType
            // 
            this.comboBox_LoadingType.FormattingEnabled = true;
            this.comboBox_LoadingType.Items.AddRange(new object[] {
            "Load Case",
            "Load Combination",
            "Result Combination"});
            this.comboBox_LoadingType.Location = new System.Drawing.Point(100, 62);
            this.comboBox_LoadingType.Name = "comboBox_LoadingType";
            this.comboBox_LoadingType.Size = new System.Drawing.Size(127, 21);
            this.comboBox_LoadingType.TabIndex = 49;
            this.comboBox_LoadingType.Text = "Result Combination";
            // 
            // checkBox_lineSupports
            // 
            this.checkBox_lineSupports.AutoSize = true;
            this.checkBox_lineSupports.Location = new System.Drawing.Point(9, 121);
            this.checkBox_lineSupports.Name = "checkBox_lineSupports";
            this.checkBox_lineSupports.Size = new System.Drawing.Size(88, 17);
            this.checkBox_lineSupports.TabIndex = 48;
            this.checkBox_lineSupports.Text = "LineSupports";
            this.checkBox_lineSupports.UseVisualStyleBackColor = true;
            // 
            // checkBox_nodalSupports
            // 
            this.checkBox_nodalSupports.AutoSize = true;
            this.checkBox_nodalSupports.Checked = true;
            this.checkBox_nodalSupports.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_nodalSupports.Location = new System.Drawing.Point(9, 98);
            this.checkBox_nodalSupports.Name = "checkBox_nodalSupports";
            this.checkBox_nodalSupports.Size = new System.Drawing.Size(96, 17);
            this.checkBox_nodalSupports.TabIndex = 47;
            this.checkBox_nodalSupports.Text = "NodalSupports";
            this.checkBox_nodalSupports.UseVisualStyleBackColor = true;
            // 
            // tabPage_beamEndForces
            // 
            this.tabPage_beamEndForces.Controls.Add(this.button_getLineSupportReactions);
            this.tabPage_beamEndForces.Controls.Add(this.button_getForceResultant);
            this.tabPage_beamEndForces.Controls.Add(this.button_getSupportForceResultant);
            this.tabPage_beamEndForces.Controls.Add(this.checkBox_beamEnd);
            this.tabPage_beamEndForces.Controls.Add(this.checkBox_beamStart);
            this.tabPage_beamEndForces.Controls.Add(this.label2);
            this.tabPage_beamEndForces.Controls.Add(this.textBox_memberNo);
            this.tabPage_beamEndForces.Controls.Add(this.label1);
            this.tabPage_beamEndForces.Controls.Add(this.textBox_loadingNo);
            this.tabPage_beamEndForces.Location = new System.Drawing.Point(4, 22);
            this.tabPage_beamEndForces.Name = "tabPage_beamEndForces";
            this.tabPage_beamEndForces.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_beamEndForces.Size = new System.Drawing.Size(683, 259);
            this.tabPage_beamEndForces.TabIndex = 1;
            this.tabPage_beamEndForces.Text = "MemberEndForces";
            this.tabPage_beamEndForces.UseVisualStyleBackColor = true;
            // 
            // button_getLineSupportReactions
            // 
            this.button_getLineSupportReactions.Location = new System.Drawing.Point(273, 156);
            this.button_getLineSupportReactions.Name = "button_getLineSupportReactions";
            this.button_getLineSupportReactions.Size = new System.Drawing.Size(148, 53);
            this.button_getLineSupportReactions.TabIndex = 52;
            this.button_getLineSupportReactions.Text = "GetLineSupportReactions";
            this.button_getLineSupportReactions.UseVisualStyleBackColor = true;
            this.button_getLineSupportReactions.Click += new System.EventHandler(this.button_getLineSupportReactions_Click);
            // 
            // button_getForceResultant
            // 
            this.button_getForceResultant.Location = new System.Drawing.Point(506, 156);
            this.button_getForceResultant.Name = "button_getForceResultant";
            this.button_getForceResultant.Size = new System.Drawing.Size(148, 53);
            this.button_getForceResultant.TabIndex = 51;
            this.button_getForceResultant.Text = "GetMemberForces";
            this.button_getForceResultant.UseVisualStyleBackColor = true;
            this.button_getForceResultant.Click += new System.EventHandler(this.button_getForceResultant_Click);
            // 
            // button_getSupportForceResultant
            // 
            this.button_getSupportForceResultant.Location = new System.Drawing.Point(52, 156);
            this.button_getSupportForceResultant.Name = "button_getSupportForceResultant";
            this.button_getSupportForceResultant.Size = new System.Drawing.Size(148, 53);
            this.button_getSupportForceResultant.TabIndex = 50;
            this.button_getSupportForceResultant.Text = "GetMemberEndForces";
            this.button_getSupportForceResultant.UseVisualStyleBackColor = true;
            this.button_getSupportForceResultant.Click += new System.EventHandler(this.button_getSupportForceResultant_Click);
            // 
            // checkBox_beamEnd
            // 
            this.checkBox_beamEnd.AutoSize = true;
            this.checkBox_beamEnd.Location = new System.Drawing.Point(125, 117);
            this.checkBox_beamEnd.Name = "checkBox_beamEnd";
            this.checkBox_beamEnd.Size = new System.Drawing.Size(72, 17);
            this.checkBox_beamEnd.TabIndex = 49;
            this.checkBox_beamEnd.Text = "BeamEnd";
            this.checkBox_beamEnd.UseVisualStyleBackColor = true;
            // 
            // checkBox_beamStart
            // 
            this.checkBox_beamStart.AutoSize = true;
            this.checkBox_beamStart.Checked = true;
            this.checkBox_beamStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_beamStart.Location = new System.Drawing.Point(125, 94);
            this.checkBox_beamStart.Name = "checkBox_beamStart";
            this.checkBox_beamStart.Size = new System.Drawing.Size(75, 17);
            this.checkBox_beamStart.TabIndex = 48;
            this.checkBox_beamStart.Text = "BeamStart";
            this.checkBox_beamStart.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 47;
            this.label2.Text = "Sauvat:";
            // 
            // textBox_memberNo
            // 
            this.textBox_memberNo.Location = new System.Drawing.Point(125, 58);
            this.textBox_memberNo.Name = "textBox_memberNo";
            this.textBox_memberNo.Size = new System.Drawing.Size(317, 20);
            this.textBox_memberNo.TabIndex = 46;
            this.textBox_memberNo.Text = "1,2,3";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 45;
            this.label1.Text = "Kuormitusyhdistelyt:";
            // 
            // textBox_loadingNo
            // 
            this.textBox_loadingNo.Location = new System.Drawing.Point(125, 23);
            this.textBox_loadingNo.Name = "textBox_loadingNo";
            this.textBox_loadingNo.Size = new System.Drawing.Size(317, 20);
            this.textBox_loadingNo.TabIndex = 44;
            this.textBox_loadingNo.Text = "1,2,3";
            // 
            // tabPage_ACad
            // 
            this.tabPage_ACad.Controls.Add(this.comboBox_deleteLayer);
            this.tabPage_ACad.Controls.Add(this.label16);
            this.tabPage_ACad.Controls.Add(this.button_deleteLayer);
            this.tabPage_ACad.Controls.Add(this.button_pasteLayerSettings);
            this.tabPage_ACad.Controls.Add(this.button_copyLayerSettings);
            this.tabPage_ACad.Controls.Add(this.label15);
            this.tabPage_ACad.Controls.Add(this.textBox_beamHeight);
            this.tabPage_ACad.Controls.Add(this.button_beamsToRFem);
            this.tabPage_ACad.Controls.Add(this.label14);
            this.tabPage_ACad.Controls.Add(this.textBox_crossSection);
            this.tabPage_ACad.Controls.Add(this.label11);
            this.tabPage_ACad.Controls.Add(this.textBox_endHinge);
            this.tabPage_ACad.Controls.Add(this.label13);
            this.tabPage_ACad.Controls.Add(this.textBox_startHinge);
            this.tabPage_ACad.Controls.Add(this.label10);
            this.tabPage_ACad.Controls.Add(this.textBox_columnLength);
            this.tabPage_ACad.Controls.Add(this.label9);
            this.tabPage_ACad.Controls.Add(this.textBox_columnStartHeight);
            this.tabPage_ACad.Controls.Add(this.button_columnsToRFem);
            this.tabPage_ACad.Controls.Add(this.label8);
            this.tabPage_ACad.Controls.Add(this.comboBox_BeamLayer);
            this.tabPage_ACad.Controls.Add(this.groupBox1);
            this.tabPage_ACad.Controls.Add(this.button_markBeams);
            this.tabPage_ACad.Controls.Add(this.label6);
            this.tabPage_ACad.Controls.Add(this.textBox_beamLayer);
            this.tabPage_ACad.Controls.Add(this.label7);
            this.tabPage_ACad.Controls.Add(this.comboBox_ColumnLayer);
            this.tabPage_ACad.Controls.Add(this.button_markColumns);
            this.tabPage_ACad.Controls.Add(this.label3);
            this.tabPage_ACad.Controls.Add(this.textBox_columnLayer);
            this.tabPage_ACad.Controls.Add(this.button_GetLayers);
            this.tabPage_ACad.Controls.Add(this.label_Layer);
            this.tabPage_ACad.Controls.Add(this.comboBox_ArkLayer);
            this.tabPage_ACad.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ACad.Name = "tabPage_ACad";
            this.tabPage_ACad.Size = new System.Drawing.Size(683, 259);
            this.tabPage_ACad.TabIndex = 2;
            this.tabPage_ACad.Text = "AutoCad";
            this.tabPage_ACad.UseVisualStyleBackColor = true;
            // 
            // comboBox_deleteLayer
            // 
            this.comboBox_deleteLayer.FormattingEnabled = true;
            this.comboBox_deleteLayer.Location = new System.Drawing.Point(470, 232);
            this.comboBox_deleteLayer.Name = "comboBox_deleteLayer";
            this.comboBox_deleteLayer.Size = new System.Drawing.Size(95, 21);
            this.comboBox_deleteLayer.TabIndex = 71;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(395, 235);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(70, 13);
            this.label16.TabIndex = 70;
            this.label16.Text = "Delete Layer:";
            // 
            // button_deleteLayer
            // 
            this.button_deleteLayer.Location = new System.Drawing.Point(571, 226);
            this.button_deleteLayer.Name = "button_deleteLayer";
            this.button_deleteLayer.Size = new System.Drawing.Size(109, 30);
            this.button_deleteLayer.TabIndex = 68;
            this.button_deleteLayer.Text = "DeleteLayer";
            this.button_deleteLayer.UseVisualStyleBackColor = true;
            this.button_deleteLayer.Click += new System.EventHandler(this.button_deleteLayer_Click);
            // 
            // button_pasteLayerSettings
            // 
            this.button_pasteLayerSettings.Location = new System.Drawing.Point(571, 124);
            this.button_pasteLayerSettings.Name = "button_pasteLayerSettings";
            this.button_pasteLayerSettings.Size = new System.Drawing.Size(109, 30);
            this.button_pasteLayerSettings.TabIndex = 67;
            this.button_pasteLayerSettings.Text = "PasteLayerSettings";
            this.button_pasteLayerSettings.UseVisualStyleBackColor = true;
            this.button_pasteLayerSettings.Click += new System.EventHandler(this.button_pasteLayerSettings_Click);
            // 
            // button_copyLayerSettings
            // 
            this.button_copyLayerSettings.Location = new System.Drawing.Point(571, 88);
            this.button_copyLayerSettings.Name = "button_copyLayerSettings";
            this.button_copyLayerSettings.Size = new System.Drawing.Size(109, 30);
            this.button_copyLayerSettings.TabIndex = 66;
            this.button_copyLayerSettings.Text = "CopyLayerSettings";
            this.button_copyLayerSettings.UseVisualStyleBackColor = true;
            this.button_copyLayerSettings.Click += new System.EventHandler(this.button_copyLayerSettings_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(10, 141);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(93, 13);
            this.label15.TabIndex = 65;
            this.label15.Text = "BeamHeight (mm):";
            // 
            // textBox_beamHeight
            // 
            this.textBox_beamHeight.Location = new System.Drawing.Point(142, 138);
            this.textBox_beamHeight.Name = "textBox_beamHeight";
            this.textBox_beamHeight.Size = new System.Drawing.Size(67, 20);
            this.textBox_beamHeight.TabIndex = 64;
            this.textBox_beamHeight.Text = "4500";
            this.textBox_beamHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_beamHeight.TextChanged += new System.EventHandler(this.textBox_beamHeight_TextChanged);
            // 
            // button_beamsToRFem
            // 
            this.button_beamsToRFem.Location = new System.Drawing.Point(119, 219);
            this.button_beamsToRFem.Name = "button_beamsToRFem";
            this.button_beamsToRFem.Size = new System.Drawing.Size(100, 28);
            this.button_beamsToRFem.TabIndex = 63;
            this.button_beamsToRFem.Text = "BeamsToRFem";
            this.button_beamsToRFem.UseVisualStyleBackColor = true;
            this.button_beamsToRFem.Click += new System.EventHandler(this.button_beamsToRFem_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(217, 193);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 13);
            this.label14.TabIndex = 62;
            this.label14.Text = "CrossSection (no):";
            // 
            // textBox_crossSection
            // 
            this.textBox_crossSection.Location = new System.Drawing.Point(316, 190);
            this.textBox_crossSection.Name = "textBox_crossSection";
            this.textBox_crossSection.Size = new System.Drawing.Size(67, 20);
            this.textBox_crossSection.TabIndex = 61;
            this.textBox_crossSection.Text = "1";
            this.textBox_crossSection.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_crossSection.TextChanged += new System.EventHandler(this.textBox_crossSection_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(217, 167);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(78, 13);
            this.label11.TabIndex = 60;
            this.label11.Text = "EndHinge (no):";
            // 
            // textBox_endHinge
            // 
            this.textBox_endHinge.Location = new System.Drawing.Point(316, 164);
            this.textBox_endHinge.Name = "textBox_endHinge";
            this.textBox_endHinge.Size = new System.Drawing.Size(67, 20);
            this.textBox_endHinge.TabIndex = 59;
            this.textBox_endHinge.Text = "1";
            this.textBox_endHinge.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_endHinge.TextChanged += new System.EventHandler(this.textBox_endHinge_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(217, 141);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(81, 13);
            this.label13.TabIndex = 58;
            this.label13.Text = "StartHinge (no):";
            // 
            // textBox_startHinge
            // 
            this.textBox_startHinge.Location = new System.Drawing.Point(316, 138);
            this.textBox_startHinge.Name = "textBox_startHinge";
            this.textBox_startHinge.Size = new System.Drawing.Size(67, 20);
            this.textBox_startHinge.TabIndex = 57;
            this.textBox_startHinge.Text = "0";
            this.textBox_startHinge.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_startHinge.TextChanged += new System.EventHandler(this.textBox_startHinge_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 193);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(103, 13);
            this.label10.TabIndex = 56;
            this.label10.Text = "ColumnLength (mm):";
            // 
            // textBox_columnLength
            // 
            this.textBox_columnLength.Location = new System.Drawing.Point(142, 190);
            this.textBox_columnLength.Name = "textBox_columnLength";
            this.textBox_columnLength.Size = new System.Drawing.Size(67, 20);
            this.textBox_columnLength.TabIndex = 55;
            this.textBox_columnLength.Text = "4500";
            this.textBox_columnLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_columnLength.TextChanged += new System.EventHandler(this.textBox_columnLength_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 167);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(123, 13);
            this.label9.TabIndex = 54;
            this.label9.Text = "ColumnStartHeight (mm):";
            // 
            // textBox_columnStartHeight
            // 
            this.textBox_columnStartHeight.Location = new System.Drawing.Point(142, 164);
            this.textBox_columnStartHeight.Name = "textBox_columnStartHeight";
            this.textBox_columnStartHeight.Size = new System.Drawing.Size(67, 20);
            this.textBox_columnStartHeight.TabIndex = 53;
            this.textBox_columnStartHeight.Text = "0";
            this.textBox_columnStartHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_columnStartHeight.TextChanged += new System.EventHandler(this.textBox_columnStartHeight_TextChanged);
            // 
            // button_columnsToRFem
            // 
            this.button_columnsToRFem.Location = new System.Drawing.Point(13, 219);
            this.button_columnsToRFem.Name = "button_columnsToRFem";
            this.button_columnsToRFem.Size = new System.Drawing.Size(100, 28);
            this.button_columnsToRFem.TabIndex = 27;
            this.button_columnsToRFem.Text = "ColumnsToRFem";
            this.button_columnsToRFem.UseVisualStyleBackColor = true;
            this.button_columnsToRFem.Click += new System.EventHandler(this.button_columnsToRFem_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 88);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 13);
            this.label8.TabIndex = 26;
            this.label8.Text = "Beam Layer:";
            // 
            // comboBox_BeamLayer
            // 
            this.comboBox_BeamLayer.FormattingEnabled = true;
            this.comboBox_BeamLayer.Location = new System.Drawing.Point(90, 85);
            this.comboBox_BeamLayer.Name = "comboBox_BeamLayer";
            this.comboBox_BeamLayer.Size = new System.Drawing.Size(108, 21);
            this.comboBox_BeamLayer.TabIndex = 25;
            this.comboBox_BeamLayer.Text = "AINS_PALKKI";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_beamDirY);
            this.groupBox1.Controls.Add(this.radioButton_beamDirX);
            this.groupBox1.Location = new System.Drawing.Point(504, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(92, 65);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BeamDirection";
            // 
            // radioButton_beamDirY
            // 
            this.radioButton_beamDirY.AutoSize = true;
            this.radioButton_beamDirY.Location = new System.Drawing.Point(6, 42);
            this.radioButton_beamDirY.Name = "radioButton_beamDirY";
            this.radioButton_beamDirY.Size = new System.Drawing.Size(32, 17);
            this.radioButton_beamDirY.TabIndex = 25;
            this.radioButton_beamDirY.Text = "Y";
            this.radioButton_beamDirY.UseVisualStyleBackColor = true;
            // 
            // radioButton_beamDirX
            // 
            this.radioButton_beamDirX.AutoSize = true;
            this.radioButton_beamDirX.Checked = true;
            this.radioButton_beamDirX.Location = new System.Drawing.Point(6, 19);
            this.radioButton_beamDirX.Name = "radioButton_beamDirX";
            this.radioButton_beamDirX.Size = new System.Drawing.Size(32, 17);
            this.radioButton_beamDirX.TabIndex = 24;
            this.radioButton_beamDirX.TabStop = true;
            this.radioButton_beamDirX.Text = "X";
            this.radioButton_beamDirX.UseVisualStyleBackColor = true;
            // 
            // button_markBeams
            // 
            this.button_markBeams.Location = new System.Drawing.Point(398, 44);
            this.button_markBeams.Name = "button_markBeams";
            this.button_markBeams.Size = new System.Drawing.Size(100, 28);
            this.button_markBeams.TabIndex = 21;
            this.button_markBeams.Text = "MarkBeams";
            this.button_markBeams.UseVisualStyleBackColor = true;
            this.button_markBeams.Click += new System.EventHandler(this.button_markBeams_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(213, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "New Layer:";
            // 
            // textBox_beamLayer
            // 
            this.textBox_beamLayer.Location = new System.Drawing.Point(280, 49);
            this.textBox_beamLayer.Name = "textBox_beamLayer";
            this.textBox_beamLayer.Size = new System.Drawing.Size(103, 20);
            this.textBox_beamLayer.TabIndex = 19;
            this.textBox_beamLayer.Text = "AINS_PALKKI";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Column Layer:";
            // 
            // comboBox_ColumnLayer
            // 
            this.comboBox_ColumnLayer.FormattingEnabled = true;
            this.comboBox_ColumnLayer.Location = new System.Drawing.Point(90, 49);
            this.comboBox_ColumnLayer.Name = "comboBox_ColumnLayer";
            this.comboBox_ColumnLayer.Size = new System.Drawing.Size(108, 21);
            this.comboBox_ColumnLayer.TabIndex = 17;
            this.comboBox_ColumnLayer.Text = "AINS_PILARI";
            // 
            // button_markColumns
            // 
            this.button_markColumns.Location = new System.Drawing.Point(398, 10);
            this.button_markColumns.Name = "button_markColumns";
            this.button_markColumns.Size = new System.Drawing.Size(100, 28);
            this.button_markColumns.TabIndex = 16;
            this.button_markColumns.Text = "MarkColumns";
            this.button_markColumns.UseVisualStyleBackColor = true;
            this.button_markColumns.Click += new System.EventHandler(this.button_markColumns_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(213, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "New Layer:";
            // 
            // textBox_columnLayer
            // 
            this.textBox_columnLayer.Location = new System.Drawing.Point(280, 15);
            this.textBox_columnLayer.Name = "textBox_columnLayer";
            this.textBox_columnLayer.Size = new System.Drawing.Size(103, 20);
            this.textBox_columnLayer.TabIndex = 10;
            this.textBox_columnLayer.Text = "AINS_PILARI";
            // 
            // button_GetLayers
            // 
            this.button_GetLayers.Location = new System.Drawing.Point(607, 10);
            this.button_GetLayers.Name = "button_GetLayers";
            this.button_GetLayers.Size = new System.Drawing.Size(73, 62);
            this.button_GetLayers.TabIndex = 9;
            this.button_GetLayers.Text = "GetLayers";
            this.button_GetLayers.UseVisualStyleBackColor = true;
            this.button_GetLayers.Click += new System.EventHandler(this.button_GetLayers_Click);
            // 
            // label_Layer
            // 
            this.label_Layer.AutoSize = true;
            this.label_Layer.Location = new System.Drawing.Point(10, 18);
            this.label_Layer.Name = "label_Layer";
            this.label_Layer.Size = new System.Drawing.Size(61, 13);
            this.label_Layer.TabIndex = 8;
            this.label_Layer.Text = "ARK Layer:";
            // 
            // comboBox_ArkLayer
            // 
            this.comboBox_ArkLayer.FormattingEnabled = true;
            this.comboBox_ArkLayer.Location = new System.Drawing.Point(90, 15);
            this.comboBox_ArkLayer.Name = "comboBox_ArkLayer";
            this.comboBox_ArkLayer.Size = new System.Drawing.Size(108, 21);
            this.comboBox_ArkLayer.TabIndex = 7;
            this.comboBox_ArkLayer.Text = "AR_1233_Pilarit";
            // 
            // tabPage_RFemModel
            // 
            this.tabPage_RFemModel.Controls.Add(this.button_modifyNodes);
            this.tabPage_RFemModel.Location = new System.Drawing.Point(4, 22);
            this.tabPage_RFemModel.Name = "tabPage_RFemModel";
            this.tabPage_RFemModel.Size = new System.Drawing.Size(683, 259);
            this.tabPage_RFemModel.TabIndex = 3;
            this.tabPage_RFemModel.Text = "RFemModel";
            this.tabPage_RFemModel.UseVisualStyleBackColor = true;
            // 
            // button_modifyNodes
            // 
            this.button_modifyNodes.Location = new System.Drawing.Point(25, 30);
            this.button_modifyNodes.Name = "button_modifyNodes";
            this.button_modifyNodes.Size = new System.Drawing.Size(81, 38);
            this.button_modifyNodes.TabIndex = 55;
            this.button_modifyNodes.Text = "Modify Nodes";
            this.button_modifyNodes.UseVisualStyleBackColor = true;
            this.button_modifyNodes.Click += new System.EventHandler(this.button_modifyNodes_Click);
            // 
            // tabPage_RFemLoads
            // 
            this.tabPage_RFemLoads.Controls.Add(this.button_getLoadings);
            this.tabPage_RFemLoads.Location = new System.Drawing.Point(4, 22);
            this.tabPage_RFemLoads.Name = "tabPage_RFemLoads";
            this.tabPage_RFemLoads.Size = new System.Drawing.Size(683, 259);
            this.tabPage_RFemLoads.TabIndex = 6;
            this.tabPage_RFemLoads.Text = "RFemLoads";
            this.tabPage_RFemLoads.UseVisualStyleBackColor = true;
            // 
            // button_getLoadings
            // 
            this.button_getLoadings.Location = new System.Drawing.Point(14, 22);
            this.button_getLoadings.Name = "button_getLoadings";
            this.button_getLoadings.Size = new System.Drawing.Size(125, 53);
            this.button_getLoadings.TabIndex = 50;
            this.button_getLoadings.Text = "GetLoadings";
            this.button_getLoadings.UseVisualStyleBackColor = true;
            this.button_getLoadings.Click += new System.EventHandler(this.button_getLoadings_Click);
            // 
            // tabPage_RFemResults
            // 
            this.tabPage_RFemResults.Location = new System.Drawing.Point(4, 22);
            this.tabPage_RFemResults.Name = "tabPage_RFemResults";
            this.tabPage_RFemResults.Size = new System.Drawing.Size(683, 259);
            this.tabPage_RFemResults.TabIndex = 5;
            this.tabPage_RFemResults.Text = "RFemResults";
            this.tabPage_RFemResults.UseVisualStyleBackColor = true;
            // 
            // tabPage_TeklaModel
            // 
            this.tabPage_TeklaModel.Controls.Add(this.button_getSolid);
            this.tabPage_TeklaModel.Controls.Add(this.button_AnalysisModelFix);
            this.tabPage_TeklaModel.Controls.Add(this.button_beamToPanel);
            this.tabPage_TeklaModel.Controls.Add(this.button_closeGap);
            this.tabPage_TeklaModel.Controls.Add(this.button_setProfiles);
            this.tabPage_TeklaModel.Controls.Add(this.button_setToLine);
            this.tabPage_TeklaModel.Controls.Add(this.button_moveTsParts);
            this.tabPage_TeklaModel.Controls.Add(this.button_testScpipt);
            this.tabPage_TeklaModel.Controls.Add(this.button_modifySlabs);
            this.tabPage_TeklaModel.Controls.Add(this.button_getProfiles);
            this.tabPage_TeklaModel.Controls.Add(this.groupBox2);
            this.tabPage_TeklaModel.Location = new System.Drawing.Point(4, 22);
            this.tabPage_TeklaModel.Name = "tabPage_TeklaModel";
            this.tabPage_TeklaModel.Size = new System.Drawing.Size(683, 259);
            this.tabPage_TeklaModel.TabIndex = 4;
            this.tabPage_TeklaModel.Text = "TeklaModel";
            this.tabPage_TeklaModel.UseVisualStyleBackColor = true;
            // 
            // button_AnalysisModelFix
            // 
            this.button_AnalysisModelFix.Location = new System.Drawing.Point(591, 178);
            this.button_AnalysisModelFix.Name = "button_AnalysisModelFix";
            this.button_AnalysisModelFix.Size = new System.Drawing.Size(89, 38);
            this.button_AnalysisModelFix.TabIndex = 61;
            this.button_AnalysisModelFix.Text = "FixAModel";
            this.button_AnalysisModelFix.UseVisualStyleBackColor = true;
            this.button_AnalysisModelFix.Click += new System.EventHandler(this.button_AnalysisModelFix_Click);
            // 
            // button_beamToPanel
            // 
            this.button_beamToPanel.Location = new System.Drawing.Point(496, 178);
            this.button_beamToPanel.Name = "button_beamToPanel";
            this.button_beamToPanel.Size = new System.Drawing.Size(89, 38);
            this.button_beamToPanel.TabIndex = 60;
            this.button_beamToPanel.Text = "BeamToPanel";
            this.button_beamToPanel.UseVisualStyleBackColor = true;
            this.button_beamToPanel.Click += new System.EventHandler(this.button_beamToPanel_Click);
            // 
            // button_closeGap
            // 
            this.button_closeGap.Location = new System.Drawing.Point(591, 134);
            this.button_closeGap.Name = "button_closeGap";
            this.button_closeGap.Size = new System.Drawing.Size(89, 38);
            this.button_closeGap.TabIndex = 59;
            this.button_closeGap.Text = "Close Gap";
            this.button_closeGap.UseVisualStyleBackColor = true;
            this.button_closeGap.Click += new System.EventHandler(this.button_closeGap_Click);
            // 
            // button_setProfiles
            // 
            this.button_setProfiles.Location = new System.Drawing.Point(591, 71);
            this.button_setProfiles.Name = "button_setProfiles";
            this.button_setProfiles.Size = new System.Drawing.Size(89, 38);
            this.button_setProfiles.TabIndex = 58;
            this.button_setProfiles.Text = "SetProfiles";
            this.button_setProfiles.UseVisualStyleBackColor = true;
            this.button_setProfiles.Click += new System.EventHandler(this.button_setProfiles_Click);
            // 
            // button_setToLine
            // 
            this.button_setToLine.Location = new System.Drawing.Point(591, 27);
            this.button_setToLine.Name = "button_setToLine";
            this.button_setToLine.Size = new System.Drawing.Size(89, 38);
            this.button_setToLine.TabIndex = 57;
            this.button_setToLine.Text = "SetToLine";
            this.button_setToLine.UseVisualStyleBackColor = true;
            this.button_setToLine.Click += new System.EventHandler(this.button_setToLine_Click);
            // 
            // button_moveTsParts
            // 
            this.button_moveTsParts.Location = new System.Drawing.Point(496, 27);
            this.button_moveTsParts.Name = "button_moveTsParts";
            this.button_moveTsParts.Size = new System.Drawing.Size(89, 38);
            this.button_moveTsParts.TabIndex = 56;
            this.button_moveTsParts.Text = "MoveParts";
            this.button_moveTsParts.UseVisualStyleBackColor = true;
            this.button_moveTsParts.Click += new System.EventHandler(this.button_moveTsParts_Click);
            // 
            // button_testScpipt
            // 
            this.button_testScpipt.Location = new System.Drawing.Point(496, 71);
            this.button_testScpipt.Name = "button_testScpipt";
            this.button_testScpipt.Size = new System.Drawing.Size(89, 38);
            this.button_testScpipt.TabIndex = 55;
            this.button_testScpipt.Text = "TestScript";
            this.button_testScpipt.UseVisualStyleBackColor = true;
            this.button_testScpipt.Click += new System.EventHandler(this.button_testScpipt_Click);
            // 
            // button_modifySlabs
            // 
            this.button_modifySlabs.Location = new System.Drawing.Point(496, 134);
            this.button_modifySlabs.Name = "button_modifySlabs";
            this.button_modifySlabs.Size = new System.Drawing.Size(89, 38);
            this.button_modifySlabs.TabIndex = 54;
            this.button_modifySlabs.Text = "Modify Slabs";
            this.button_modifySlabs.UseVisualStyleBackColor = true;
            this.button_modifySlabs.Click += new System.EventHandler(this.button_modifySlabs_Click);
            // 
            // button_getProfiles
            // 
            this.button_getProfiles.Location = new System.Drawing.Point(218, 155);
            this.button_getProfiles.Name = "button_getProfiles";
            this.button_getProfiles.Size = new System.Drawing.Size(81, 38);
            this.button_getProfiles.TabIndex = 53;
            this.button_getProfiles.Text = "Get Profiles";
            this.button_getProfiles.UseVisualStyleBackColor = true;
            this.button_getProfiles.Click += new System.EventHandler(this.button_getProfiles_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_beams_RFemToTekla);
            this.groupBox2.Location = new System.Drawing.Point(18, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(194, 172);
            this.groupBox2.TabIndex = 52;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RFem to Tekla";
            // 
            // button_beams_RFemToTekla
            // 
            this.button_beams_RFemToTekla.Location = new System.Drawing.Point(107, 128);
            this.button_beams_RFemToTekla.Name = "button_beams_RFemToTekla";
            this.button_beams_RFemToTekla.Size = new System.Drawing.Size(81, 38);
            this.button_beams_RFemToTekla.TabIndex = 52;
            this.button_beams_RFemToTekla.Text = "Add Beams";
            this.button_beams_RFemToTekla.UseVisualStyleBackColor = true;
            this.button_beams_RFemToTekla.Click += new System.EventHandler(this.button_beams_RFemToTekla_Click);
            // 
            // checkBox_OnTop
            // 
            this.checkBox_OnTop.AutoSize = true;
            this.checkBox_OnTop.Checked = true;
            this.checkBox_OnTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_OnTop.Location = new System.Drawing.Point(542, 408);
            this.checkBox_OnTop.Name = "checkBox_OnTop";
            this.checkBox_OnTop.Size = new System.Drawing.Size(96, 17);
            this.checkBox_OnTop.TabIndex = 48;
            this.checkBox_OnTop.Text = "Always on Top";
            this.checkBox_OnTop.UseVisualStyleBackColor = true;
            this.checkBox_OnTop.CheckedChanged += new System.EventHandler(this.checkBox_OnTop_CheckedChanged);
            // 
            // button_maxN
            // 
            this.button_maxN.Location = new System.Drawing.Point(464, 41);
            this.button_maxN.Name = "button_maxN";
            this.button_maxN.Size = new System.Drawing.Size(125, 53);
            this.button_maxN.TabIndex = 49;
            this.button_maxN.Text = "Max N";
            this.button_maxN.UseVisualStyleBackColor = true;
            this.button_maxN.Click += new System.EventHandler(this.button_maxN_Click);
            // 
            // button_getUCS
            // 
            this.button_getUCS.Location = new System.Drawing.Point(333, 41);
            this.button_getUCS.Name = "button_getUCS";
            this.button_getUCS.Size = new System.Drawing.Size(125, 53);
            this.button_getUCS.TabIndex = 50;
            this.button_getUCS.Text = "Get UCS";
            this.button_getUCS.UseVisualStyleBackColor = true;
            this.button_getUCS.Click += new System.EventHandler(this.button_getUCS_Click);
            // 
            // button_setUCS
            // 
            this.button_setUCS.Location = new System.Drawing.Point(202, 41);
            this.button_setUCS.Name = "button_setUCS";
            this.button_setUCS.Size = new System.Drawing.Size(125, 53);
            this.button_setUCS.TabIndex = 51;
            this.button_setUCS.Text = "Set UCS";
            this.button_setUCS.UseVisualStyleBackColor = true;
            this.button_setUCS.Click += new System.EventHandler(this.button_setUCS_Click);
            // 
            // button_getSolid
            // 
            this.button_getSolid.Location = new System.Drawing.Point(401, 178);
            this.button_getSolid.Name = "button_getSolid";
            this.button_getSolid.Size = new System.Drawing.Size(89, 38);
            this.button_getSolid.TabIndex = 62;
            this.button_getSolid.Text = "GetSolid";
            this.button_getSolid.UseVisualStyleBackColor = true;
            this.button_getSolid.Click += new System.EventHandler(this.button_getSolid_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_setUCS);
            this.Controls.Add(this.button_getUCS);
            this.Controls.Add(this.button_maxN);
            this.Controls.Add(this.checkBox_OnTop);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_writeMembers);
            this.Controls.Add(this.button_unlock);
            this.Controls.Add(this.comboBox_ModelName);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.button_connect);
            this.Name = "Form1";
            this.Text = "TuHan Skriptejä";
            this.tabControl1.ResumeLayout(false);
            this.tabPage_getSupportReactions.ResumeLayout(false);
            this.tabPage_getSupportReactions.PerformLayout();
            this.tabPage_beamEndForces.ResumeLayout(false);
            this.tabPage_beamEndForces.PerformLayout();
            this.tabPage_ACad.ResumeLayout(false);
            this.tabPage_ACad.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage_RFemModel.ResumeLayout(false);
            this.tabPage_RFemLoads.ResumeLayout(false);
            this.tabPage_TeklaModel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_connect;
        public System.Windows.Forms.ComboBox comboBox_ModelName;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button button_unlock;
        private System.Windows.Forms.Button button_writeMembers;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_getSupportReactions;
        private System.Windows.Forms.Button button_getSupportForces;
        public System.Windows.Forms.TextBox textBox_LoadingNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox comboBox_LoadingType;
        private System.Windows.Forms.CheckBox checkBox_lineSupports;
        private System.Windows.Forms.CheckBox checkBox_nodalSupports;
        private System.Windows.Forms.TabPage tabPage_beamEndForces;
        private System.Windows.Forms.Button button_getSupportForceResultant;
        private System.Windows.Forms.CheckBox checkBox_beamEnd;
        private System.Windows.Forms.CheckBox checkBox_beamStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_memberNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_loadingNo;
        private System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.CheckBox checkBox_OnTop;
        private System.Windows.Forms.Button button_maxN;
        private System.Windows.Forms.TabPage tabPage_ACad;
        private System.Windows.Forms.TabPage tabPage_RFemModel;
        private System.Windows.Forms.TabPage tabPage_TeklaModel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_columnLayer;
        private System.Windows.Forms.Button button_GetLayers;
        private System.Windows.Forms.Label label_Layer;
        private System.Windows.Forms.ComboBox comboBox_ArkLayer;
        private System.Windows.Forms.Button button_markColumns;
        private System.Windows.Forms.Button button_markBeams;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_beamLayer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox_ColumnLayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_beamDirY;
        private System.Windows.Forms.RadioButton radioButton_beamDirX;
        private System.Windows.Forms.Button button_columnsToRFem;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_BeamLayer;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox textBox_columnLength;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox textBox_columnStartHeight;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.TextBox textBox_endHinge;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.TextBox textBox_startHinge;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.TextBox textBox_crossSection;
        private System.Windows.Forms.Button button_beamsToRFem;
        private System.Windows.Forms.Label label15;
        public System.Windows.Forms.TextBox textBox_beamHeight;
        private System.Windows.Forms.ComboBox comboBox_deleteLayer;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button_deleteLayer;
        private System.Windows.Forms.Button button_pasteLayerSettings;
        private System.Windows.Forms.Button button_copyLayerSettings;
        private System.Windows.Forms.TabPage tabPage_RFemResults;
        private System.Windows.Forms.TabPage tabPage_RFemLoads;
        private System.Windows.Forms.Button button_getLoadings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_beams_RFemToTekla;
        private System.Windows.Forms.Button button_getProfiles;
        private System.Windows.Forms.Button button_getUCS;
        private System.Windows.Forms.Button button_setUCS;
        private System.Windows.Forms.Button button_getForceResultant;
        private System.Windows.Forms.Button button_modifySlabs;
        private System.Windows.Forms.Button button_testScpipt;
        private System.Windows.Forms.Button button_moveTsParts;
        private System.Windows.Forms.Button button_setToLine;
        private System.Windows.Forms.Button button_setProfiles;
        private System.Windows.Forms.Button button_modifyNodes;
        private System.Windows.Forms.Button button_closeGap;
        private System.Windows.Forms.Button button_getLineSupportReactions;
        private System.Windows.Forms.Button button_beamToPanel;
        private System.Windows.Forms.Button button_AnalysisModelFix;
        private System.Windows.Forms.Button button_getSolid;
    }
}

