namespace Keyboard_Layout_Editor {
    partial class MainEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.AllKeysList = new System.Windows.Forms.ListBox();
			this.KeysContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.nonPrintableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.MainSplit = new System.Windows.Forms.SplitContainer();
			this.PhysicalGroup = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.AddKey = new System.Windows.Forms.Button();
			this.DeleteKey = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.KeyValueEditor = new System.Windows.Forms.ListView();
			this.HeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.HeaderModifiers = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.CharacterImageList = new System.Windows.Forms.ImageList(this.components);
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.ModifiedByGroup = new System.Windows.Forms.GroupBox();
			this.ModifierGroup = new System.Windows.Forms.GroupBox();
			this.DeviceCode = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.KeyIsModifier = new System.Windows.Forms.CheckBox();
			this.ToggleModification = new System.Windows.Forms.CheckBox();
			this.ModifierIndex = new System.Windows.Forms.ComboBox();
			this.LedIndex = new System.Windows.Forms.ComboBox();
			this.Friendly = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.ScanCode = new System.Windows.Forms.NumericUpDown();
			this.IsExtended = new System.Windows.Forms.CheckBox();
			this.KeyboardMenus = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toRawBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.toTI83PlusFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.keySetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tI83PlusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aSCIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.DescriptionPanel = new System.Windows.Forms.Panel();
			this.DescriptionText = new System.Windows.Forms.TextBox();
			this.DescriptionLabel = new System.Windows.Forms.Label();
			this.OpenXmlDialog = new System.Windows.Forms.OpenFileDialog();
			this.SaveXmlDialog = new System.Windows.Forms.SaveFileDialog();
			this.ExportAssemblyDialog = new System.Windows.Forms.SaveFileDialog();
			this.ExportBinaryDialog = new System.Windows.Forms.SaveFileDialog();
			this.bBCBASICToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.KeysContext.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MainSplit)).BeginInit();
			this.MainSplit.Panel1.SuspendLayout();
			this.MainSplit.Panel2.SuspendLayout();
			this.MainSplit.SuspendLayout();
			this.PhysicalGroup.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.ModifierGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.DeviceCode)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ScanCode)).BeginInit();
			this.KeyboardMenus.SuspendLayout();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.DescriptionPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// AllKeysList
			// 
			this.AllKeysList.AllowDrop = true;
			this.AllKeysList.BackColor = System.Drawing.SystemColors.Control;
			this.AllKeysList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.AllKeysList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AllKeysList.FormattingEnabled = true;
			this.AllKeysList.Location = new System.Drawing.Point(3, 16);
			this.AllKeysList.Name = "AllKeysList";
			this.AllKeysList.Size = new System.Drawing.Size(154, 315);
			this.AllKeysList.TabIndex = 0;
			this.AllKeysList.SelectedIndexChanged += new System.EventHandler(this.AllKeysList_SelectedIndexChanged);
			this.AllKeysList.DragDrop += new System.Windows.Forms.DragEventHandler(this.AllKeysList_DragDrop);
			this.AllKeysList.DragOver += new System.Windows.Forms.DragEventHandler(this.AllKeysList_DragOver);
			this.AllKeysList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AllKeysList_MouseDown);
			// 
			// KeysContext
			// 
			this.KeysContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.toolStripMenuItem3,
            this.nonPrintableToolStripMenuItem,
            this.toolStripMenuItem4});
			this.KeysContext.Name = "KeysContext";
			this.KeysContext.Size = new System.Drawing.Size(150, 60);
			this.KeysContext.Opening += new System.ComponentModel.CancelEventHandler(this.KeysContext_Opening);
			// 
			// noneToolStripMenuItem
			// 
			this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
			this.noneToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.noneToolStripMenuItem.Tag = "255";
			this.noneToolStripMenuItem.Text = "(None)";
			this.noneToolStripMenuItem.Click += new System.EventHandler(this.ChosenKey);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(146, 6);
			// 
			// nonPrintableToolStripMenuItem
			// 
			this.nonPrintableToolStripMenuItem.Name = "nonPrintableToolStripMenuItem";
			this.nonPrintableToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.nonPrintableToolStripMenuItem.Text = "Non-Printable";
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(146, 6);
			// 
			// MainSplit
			// 
			this.MainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.MainSplit.Location = new System.Drawing.Point(3, 27);
			this.MainSplit.Name = "MainSplit";
			// 
			// MainSplit.Panel1
			// 
			this.MainSplit.Panel1.Controls.Add(this.PhysicalGroup);
			// 
			// MainSplit.Panel2
			// 
			this.MainSplit.Panel2.Controls.Add(this.groupBox1);
			this.MainSplit.Panel2.Controls.Add(this.tableLayoutPanel1);
			this.MainSplit.Size = new System.Drawing.Size(637, 379);
			this.MainSplit.SplitterDistance = 160;
			this.MainSplit.TabIndex = 1;
			// 
			// PhysicalGroup
			// 
			this.PhysicalGroup.Controls.Add(this.AllKeysList);
			this.PhysicalGroup.Controls.Add(this.tableLayoutPanel2);
			this.PhysicalGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PhysicalGroup.Location = new System.Drawing.Point(0, 0);
			this.PhysicalGroup.Name = "PhysicalGroup";
			this.PhysicalGroup.Size = new System.Drawing.Size(160, 379);
			this.PhysicalGroup.TabIndex = 1;
			this.PhysicalGroup.TabStop = false;
			this.PhysicalGroup.Text = "Physical Keys";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.AddKey, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.DeleteKey, 1, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 331);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(154, 45);
			this.tableLayoutPanel2.TabIndex = 4;
			// 
			// AddKey
			// 
			this.AddKey.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AddKey.Location = new System.Drawing.Point(3, 3);
			this.AddKey.Name = "AddKey";
			this.AddKey.Size = new System.Drawing.Size(71, 39);
			this.AddKey.TabIndex = 0;
			this.AddKey.Text = "&Add";
			this.AddKey.UseVisualStyleBackColor = true;
			this.AddKey.Click += new System.EventHandler(this.AddKey_Click);
			// 
			// DeleteKey
			// 
			this.DeleteKey.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DeleteKey.Location = new System.Drawing.Point(80, 3);
			this.DeleteKey.Name = "DeleteKey";
			this.DeleteKey.Size = new System.Drawing.Size(71, 39);
			this.DeleteKey.TabIndex = 0;
			this.DeleteKey.Text = "&Delete";
			this.DeleteKey.UseVisualStyleBackColor = true;
			this.DeleteKey.Click += new System.EventHandler(this.DeleteKey_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.KeyValueEditor);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 180);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(473, 199);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Key Values";
			// 
			// KeyValueEditor
			// 
			this.KeyValueEditor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.HeaderValue,
            this.HeaderModifiers});
			this.KeyValueEditor.ContextMenuStrip = this.KeysContext;
			this.KeyValueEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.KeyValueEditor.FullRowSelect = true;
			this.KeyValueEditor.GridLines = true;
			this.KeyValueEditor.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.KeyValueEditor.HideSelection = false;
			this.KeyValueEditor.LargeImageList = this.CharacterImageList;
			this.KeyValueEditor.Location = new System.Drawing.Point(3, 16);
			this.KeyValueEditor.MultiSelect = false;
			this.KeyValueEditor.Name = "KeyValueEditor";
			this.KeyValueEditor.Size = new System.Drawing.Size(467, 180);
			this.KeyValueEditor.SmallImageList = this.CharacterImageList;
			this.KeyValueEditor.TabIndex = 0;
			this.KeyValueEditor.UseCompatibleStateImageBehavior = false;
			this.KeyValueEditor.View = System.Windows.Forms.View.Details;
			// 
			// HeaderValue
			// 
			this.HeaderValue.Text = "Value";
			this.HeaderValue.Width = 86;
			// 
			// HeaderModifiers
			// 
			this.HeaderModifiers.Text = "Modifiers";
			this.HeaderModifiers.Width = 308;
			// 
			// CharacterImageList
			// 
			this.CharacterImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.CharacterImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.CharacterImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.ModifiedByGroup, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.ModifierGroup, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(473, 180);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// ModifiedByGroup
			// 
			this.ModifiedByGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ModifiedByGroup.Location = new System.Drawing.Point(239, 3);
			this.ModifiedByGroup.Name = "ModifiedByGroup";
			this.ModifiedByGroup.Size = new System.Drawing.Size(231, 174);
			this.ModifiedByGroup.TabIndex = 1;
			this.ModifiedByGroup.TabStop = false;
			this.ModifiedByGroup.Text = "Modified By";
			// 
			// ModifierGroup
			// 
			this.ModifierGroup.Controls.Add(this.DeviceCode);
			this.ModifierGroup.Controls.Add(this.label2);
			this.ModifierGroup.Controls.Add(this.groupBox2);
			this.ModifierGroup.Controls.Add(this.Friendly);
			this.ModifierGroup.Controls.Add(this.label3);
			this.ModifierGroup.Controls.Add(this.label1);
			this.ModifierGroup.Controls.Add(this.ScanCode);
			this.ModifierGroup.Controls.Add(this.IsExtended);
			this.ModifierGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ModifierGroup.Location = new System.Drawing.Point(3, 3);
			this.ModifierGroup.Name = "ModifierGroup";
			this.ModifierGroup.Size = new System.Drawing.Size(230, 174);
			this.ModifierGroup.TabIndex = 2;
			this.ModifierGroup.TabStop = false;
			this.ModifierGroup.Text = "General";
			// 
			// DeviceCode
			// 
			this.DeviceCode.Hexadecimal = true;
			this.DeviceCode.Location = new System.Drawing.Point(105, 68);
			this.DeviceCode.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.DeviceCode.Name = "DeviceCode";
			this.DeviceCode.Size = new System.Drawing.Size(41, 20);
			this.DeviceCode.TabIndex = 9;
			this.DeviceCode.ValueChanged += new System.EventHandler(this.DeviceCode_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 70);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Device code";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.KeyIsModifier);
			this.groupBox2.Controls.Add(this.ToggleModification);
			this.groupBox2.Controls.Add(this.ModifierIndex);
			this.groupBox2.Controls.Add(this.LedIndex);
			this.groupBox2.Location = new System.Drawing.Point(5, 96);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(219, 72);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Modifier";
			// 
			// KeyIsModifier
			// 
			this.KeyIsModifier.AutoSize = true;
			this.KeyIsModifier.Location = new System.Drawing.Point(6, 22);
			this.KeyIsModifier.Name = "KeyIsModifier";
			this.KeyIsModifier.Size = new System.Drawing.Size(102, 17);
			this.KeyIsModifier.TabIndex = 0;
			this.KeyIsModifier.Text = "Key is a modifier";
			this.KeyIsModifier.UseVisualStyleBackColor = true;
			this.KeyIsModifier.CheckedChanged += new System.EventHandler(this.KeyIsModifier_CheckedChanged);
			// 
			// ToggleModification
			// 
			this.ToggleModification.AutoSize = true;
			this.ToggleModification.Enabled = false;
			this.ToggleModification.Location = new System.Drawing.Point(6, 47);
			this.ToggleModification.Name = "ToggleModification";
			this.ToggleModification.Size = new System.Drawing.Size(118, 17);
			this.ToggleModification.TabIndex = 1;
			this.ToggleModification.Text = "Toggle modification";
			this.ToggleModification.UseVisualStyleBackColor = true;
			this.ToggleModification.CheckedChanged += new System.EventHandler(this.ToggleModification_CheckedChanged);
			// 
			// ModifierIndex
			// 
			this.ModifierIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ModifierIndex.Enabled = false;
			this.ModifierIndex.FormattingEnabled = true;
			this.ModifierIndex.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
			this.ModifierIndex.Location = new System.Drawing.Point(152, 20);
			this.ModifierIndex.Name = "ModifierIndex";
			this.ModifierIndex.Size = new System.Drawing.Size(61, 21);
			this.ModifierIndex.TabIndex = 2;
			this.ModifierIndex.SelectedIndexChanged += new System.EventHandler(this.ModifierIndex_SelectedIndexChanged);
			// 
			// LedIndex
			// 
			this.LedIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LedIndex.Enabled = false;
			this.LedIndex.FormattingEnabled = true;
			this.LedIndex.Items.AddRange(new object[] {
            "(None)",
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
			this.LedIndex.Location = new System.Drawing.Point(152, 45);
			this.LedIndex.MaxDropDownItems = 9;
			this.LedIndex.Name = "LedIndex";
			this.LedIndex.Size = new System.Drawing.Size(61, 21);
			this.LedIndex.TabIndex = 3;
			this.LedIndex.SelectedIndexChanged += new System.EventHandler(this.LedIndex_SelectedIndexChanged);
			// 
			// Friendly
			// 
			this.Friendly.Location = new System.Drawing.Point(105, 42);
			this.Friendly.Name = "Friendly";
			this.Friendly.Size = new System.Drawing.Size(119, 20);
			this.Friendly.TabIndex = 6;
			this.Friendly.Leave += new System.EventHandler(this.Friendly_Leave);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 45);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Name";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "PS/2 scancode";
			// 
			// ScanCode
			// 
			this.ScanCode.Hexadecimal = true;
			this.ScanCode.Location = new System.Drawing.Point(105, 15);
			this.ScanCode.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.ScanCode.Name = "ScanCode";
			this.ScanCode.Size = new System.Drawing.Size(41, 20);
			this.ScanCode.TabIndex = 4;
			this.ScanCode.ValueChanged += new System.EventHandler(this.ScanCode_ValueChanged);
			// 
			// IsExtended
			// 
			this.IsExtended.AutoSize = true;
			this.IsExtended.Location = new System.Drawing.Point(152, 16);
			this.IsExtended.Name = "IsExtended";
			this.IsExtended.Size = new System.Drawing.Size(71, 17);
			this.IsExtended.TabIndex = 0;
			this.IsExtended.Text = "Extended";
			this.IsExtended.UseVisualStyleBackColor = true;
			this.IsExtended.CheckedChanged += new System.EventHandler(this.IsExtended_CheckedChanged);
			// 
			// KeyboardMenus
			// 
			this.KeyboardMenus.Dock = System.Windows.Forms.DockStyle.None;
			this.KeyboardMenus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.keySetsToolStripMenuItem});
			this.KeyboardMenus.Location = new System.Drawing.Point(0, 0);
			this.KeyboardMenus.Name = "KeyboardMenus";
			this.KeyboardMenus.Size = new System.Drawing.Size(643, 24);
			this.KeyboardMenus.TabIndex = 2;
			this.KeyboardMenus.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exportToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.saveToolStripMenuItem.Text = "&Save...";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 6);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toRawBinaryToolStripMenuItem,
            this.toTextToolStripMenuItem,
            this.toolStripMenuItem5,
            this.toTI83PlusFileToolStripMenuItem});
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.exportToolStripMenuItem.Text = "&Export";
			this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
			// 
			// toRawBinaryToolStripMenuItem
			// 
			this.toRawBinaryToolStripMenuItem.Name = "toRawBinaryToolStripMenuItem";
			this.toRawBinaryToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.toRawBinaryToolStripMenuItem.Text = "Binary...";
			this.toRawBinaryToolStripMenuItem.Click += new System.EventHandler(this.toRawBinaryToolStripMenuItem_Click);
			// 
			// toTextToolStripMenuItem
			// 
			this.toTextToolStripMenuItem.Name = "toTextToolStripMenuItem";
			this.toTextToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.toTextToolStripMenuItem.Text = "Assembly source...";
			this.toTextToolStripMenuItem.Click += new System.EventHandler(this.toTextToolStripMenuItem_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(198, 6);
			// 
			// toTI83PlusFileToolStripMenuItem
			// 
			this.toTI83PlusFileToolStripMenuItem.Name = "toTI83PlusFileToolStripMenuItem";
			this.toTI83PlusFileToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
			this.toTI83PlusFileToolStripMenuItem.Text = "TI-83 and TI-83 Plus files";
			this.toTI83PlusFileToolStripMenuItem.Click += new System.EventHandler(this.toTI83PlusFileToolStripMenuItem_Click);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// keySetsToolStripMenuItem
			// 
			this.keySetsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tI83PlusToolStripMenuItem,
            this.aSCIIToolStripMenuItem,
            this.bBCBASICToolStripMenuItem});
			this.keySetsToolStripMenuItem.Name = "keySetsToolStripMenuItem";
			this.keySetsToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
			this.keySetsToolStripMenuItem.Text = "&Key Sets";
			this.keySetsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.keySetsToolStripMenuItem_DropDownOpening);
			// 
			// tI83PlusToolStripMenuItem
			// 
			this.tI83PlusToolStripMenuItem.Name = "tI83PlusToolStripMenuItem";
			this.tI83PlusToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.tI83PlusToolStripMenuItem.Text = "&TI-83 Plus";
			this.tI83PlusToolStripMenuItem.Click += new System.EventHandler(this.tI83PlusToolStripMenuItem_Click);
			// 
			// aSCIIToolStripMenuItem
			// 
			this.aSCIIToolStripMenuItem.Name = "aSCIIToolStripMenuItem";
			this.aSCIIToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.aSCIIToolStripMenuItem.Text = "&ASCII";
			this.aSCIIToolStripMenuItem.Click += new System.EventHandler(this.aSCIIToolStripMenuItem_Click);
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.AutoScroll = true;
			this.toolStripContainer1.ContentPanel.Controls.Add(this.MainSplit);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.DescriptionPanel);
			this.toolStripContainer1.ContentPanel.Padding = new System.Windows.Forms.Padding(3);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(643, 409);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(643, 433);
			this.toolStripContainer1.TabIndex = 3;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.KeyboardMenus);
			// 
			// DescriptionPanel
			// 
			this.DescriptionPanel.Controls.Add(this.DescriptionText);
			this.DescriptionPanel.Controls.Add(this.DescriptionLabel);
			this.DescriptionPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.DescriptionPanel.Location = new System.Drawing.Point(3, 3);
			this.DescriptionPanel.Name = "DescriptionPanel";
			this.DescriptionPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.DescriptionPanel.Size = new System.Drawing.Size(637, 24);
			this.DescriptionPanel.TabIndex = 1;
			// 
			// DescriptionText
			// 
			this.DescriptionText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DescriptionText.Location = new System.Drawing.Point(60, 0);
			this.DescriptionText.Name = "DescriptionText";
			this.DescriptionText.Size = new System.Drawing.Size(577, 20);
			this.DescriptionText.TabIndex = 1;
			// 
			// DescriptionLabel
			// 
			this.DescriptionLabel.AutoSize = true;
			this.DescriptionLabel.Dock = System.Windows.Forms.DockStyle.Left;
			this.DescriptionLabel.Location = new System.Drawing.Point(0, 0);
			this.DescriptionLabel.Name = "DescriptionLabel";
			this.DescriptionLabel.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this.DescriptionLabel.Size = new System.Drawing.Size(60, 17);
			this.DescriptionLabel.TabIndex = 0;
			this.DescriptionLabel.Text = "Description";
			// 
			// OpenXmlDialog
			// 
			this.OpenXmlDialog.Filter = "Emerson Keyboard Layouts (*.emkbd)|*.emkbd";
			// 
			// SaveXmlDialog
			// 
			this.SaveXmlDialog.Filter = "Emerson Keyboard Layouts (*.emkbd)|*.emkbd";
			// 
			// ExportAssemblyDialog
			// 
			this.ExportAssemblyDialog.Filter = "Assembly source code (*.inc)|*.inc|Text files (*.txt)|*.txt";
			// 
			// ExportBinaryDialog
			// 
			this.ExportBinaryDialog.Filter = "Raw binary (*.bin)|*.bin";
			// 
			// bBCBASICToolStripMenuItem
			// 
			this.bBCBASICToolStripMenuItem.Name = "bBCBASICToolStripMenuItem";
			this.bBCBASICToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.bBCBASICToolStripMenuItem.Text = "&BBC BASIC";
			this.bBCBASICToolStripMenuItem.Click += new System.EventHandler(this.bBCBASICToolStripMenuItem_Click);
			// 
			// MainEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(643, 433);
			this.Controls.Add(this.toolStripContainer1);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.KeyboardMenus;
			this.Name = "MainEditor";
			this.Text = "Layout File Editor";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainEditor_FormClosed);
			this.KeysContext.ResumeLayout(false);
			this.MainSplit.Panel1.ResumeLayout(false);
			this.MainSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.MainSplit)).EndInit();
			this.MainSplit.ResumeLayout(false);
			this.PhysicalGroup.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ModifierGroup.ResumeLayout(false);
			this.ModifierGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.DeviceCode)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ScanCode)).EndInit();
			this.KeyboardMenus.ResumeLayout(false);
			this.KeyboardMenus.PerformLayout();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.DescriptionPanel.ResumeLayout(false);
			this.DescriptionPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox AllKeysList;
        private System.Windows.Forms.SplitContainer MainSplit;
        private System.Windows.Forms.MenuStrip KeyboardMenus;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Panel DescriptionPanel;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.TextBox DescriptionText;
        private System.Windows.Forms.GroupBox ModifiedByGroup;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox ModifierGroup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox PhysicalGroup;
        private System.Windows.Forms.CheckBox KeyIsModifier;
        private System.Windows.Forms.CheckBox ToggleModification;
        private System.Windows.Forms.ComboBox LedIndex;
        private System.Windows.Forms.ComboBox ModifierIndex;
        private System.Windows.Forms.ContextMenuStrip KeysContext;
        private System.Windows.Forms.ListView KeyValueEditor;
        private System.Windows.Forms.ColumnHeader HeaderValue;
        private System.Windows.Forms.ColumnHeader HeaderModifiers;
        private System.Windows.Forms.ImageList CharacterImageList;
        private System.Windows.Forms.NumericUpDown ScanCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox IsExtended;
        private System.Windows.Forms.TextBox Friendly;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button DeleteKey;
        private System.Windows.Forms.Button AddKey;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem nonPrintableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.OpenFileDialog OpenXmlDialog;
        private System.Windows.Forms.SaveFileDialog SaveXmlDialog;
		private System.Windows.Forms.ToolStripMenuItem toTextToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toRawBinaryToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem toTI83PlusFileToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog ExportAssemblyDialog;
		private System.Windows.Forms.SaveFileDialog ExportBinaryDialog;
		private System.Windows.Forms.ToolStripMenuItem keySetsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tI83PlusToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aSCIIToolStripMenuItem;
		private System.Windows.Forms.NumericUpDown DeviceCode;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolStripMenuItem bBCBASICToolStripMenuItem;
	}
}

