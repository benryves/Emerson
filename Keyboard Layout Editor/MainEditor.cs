/*
 * Yes, this code is awful.
 * It works, to some extent. It was very quickly cobbled together, and it shows.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Keyboard_Layout_Editor {
	public partial class MainEditor : Form {

		string[] NonPrintable = new string[]{
			"F1","F2","F3","F4","F5","F6","F7","F8","F9","F10","F11","F12","-",
			"Up","Down","Left","Right","-",
			"Page Up","Page Down",
			"Insert","-",
			"Print Screen","-",
			"Home", "End","-",
			"Ctrl (Left)", "Ctrl (Right)",
			"Alt", "AltGr (Alt Right)",
			"Shift (Left)", "Shift (Right)","-",
			"Windows (Left)", "Windows (Right)","-",
			"Caps Lock","Num Lock", "Scroll Lock",
			"Application","-",
			"Power","Sleep","Wake","-",
			"Escape", "Pause",
		};

		string[] BbcBasicFunctionKeys = new string[] {
			"Print Screen",  "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15",
			"Windows", "Menu", "", "", "", "", "Insert", "Delete", "Home","End","Page Down", "Page Up", "Left","Right","Down","Up",
		};

		List<string> NonPrintableNames = new List<string>();

		List<PhysicalKey> AllKeys = new List<PhysicalKey>();

		public void SaveXml(string filename) {

			XmlWriterSettings S = new XmlWriterSettings();
			S.Indent = true;

			using (XmlWriter X = XmlWriter.Create(filename, S)) {
				X.WriteStartDocument();
				X.WriteStartElement("keyboard");
				X.WriteAttributeString("version", "1");
				X.WriteAttributeString("title", DescriptionText.Text);

				foreach (PhysicalKey PK in AllKeys) {
					X.WriteStartElement("key");
					X.WriteAttributeString("friendlyname", PK.FriendlyName);
					X.WriteAttributeString("scancode", PK.ScanCode.ToString());
					if (PK.DeviceCode != 0) X.WriteAttributeString("devicecode", PK.DeviceCode.ToString());
					if (PK.SecondaryDeviceCode != 0) X.WriteAttributeString("secondarydevicecode", PK.SecondaryDeviceCode.ToString());
					X.WriteAttributeString("isextended", PK.IsExtended.ToString());
					X.WriteAttributeString("ismodifier", PK.IsModifier.ToString());
					if (PK.IsModifier) {
						X.WriteAttributeString("modifierindex", PK.ModifierIndex.ToString());
						X.WriteAttributeString("istoggle", PK.IsToggle.ToString());
						if (PK.IsToggle) {
							X.WriteAttributeString("led", PK.LedIndex.ToString());
						}
					} else {
						X.WriteAttributeString("modifiermask", PK.ModifierMask.ToString());
					}

					List<int> WrittenModifications = new List<int>();
					for (int i = 0; i < 256; ++i) {
						int Modification = i & PK.ModifierMask;
						if (!WrittenModifications.Contains(Modification)) {
							WrittenModifications.Add(Modification);
							X.WriteStartElement("value");
							X.WriteAttributeString("modifier", Modification.ToString());
							X.WriteAttributeString("value", PK.Values[Modification].ToString());
							X.WriteAttributeString("isnonprintable", PK.IsNonPrintable[Modification].ToString());
							X.WriteEndElement();
						}
					}

					X.WriteEndElement();
				}

				X.WriteEndElement();
				X.WriteEndDocument();
				X.Flush();
			}
		}

		public void LoadXml(string filename) {
			AllKeys.Clear();
			XmlDocument X = new XmlDocument();
			X.Load(filename);

			foreach (XmlAttribute A in X.DocumentElement.Attributes) {
				switch (A.Name) {
					case "title":
						DescriptionText.Text = A.Value;
						break;
				}
			}

			XmlNodeList NL = X.GetElementsByTagName("key");

			foreach (XmlNode N in NL) {
				PhysicalKey PK = new PhysicalKey();
				foreach (XmlAttribute A in N.Attributes) {
					switch (A.Name) {
						case "friendlyname":
							PK.FriendlyName = A.Value;
							break;
						case "scancode":
							PK.ScanCode = byte.Parse(A.Value);
							break;
						case "devicecode":
							PK.DeviceCode = byte.Parse(A.Value);
							break;
						case "secondarydevicecode":
							PK.SecondaryDeviceCode = byte.Parse(A.Value);
							break;
						case "isextended":
							PK.IsExtended = bool.Parse(A.Value);
							break;
						case "ismodifier":
							PK.IsModifier = bool.Parse(A.Value);
							break;
						case "modifierindex":
							PK.ModifierIndex = int.Parse(A.Value);
							break;
						case "istoggle":
							PK.IsToggle = bool.Parse(A.Value);
							break;
						case "modifiermask":
							PK.ModifierMask = byte.Parse(A.Value);
							break;
						case "led":
							PK.LedIndex = byte.Parse(A.Value);
							break;
					}
				}

				foreach (XmlNode V in N.ChildNodes) {
					int Modifier = 0;
					byte Value = 0xFF;
					bool IsNonPrintable = false;
					if (V.Name == "value") {
						foreach (XmlAttribute A in V.Attributes) {
							switch (A.Name) {
								case "modifier":
									Modifier = int.Parse(A.Value);
									break;
								case "value":
									Value = byte.Parse(A.Value);
									break;
								case "isnonprintable":
									IsNonPrintable = bool.Parse(A.Value);
									break;
							}
						}
					}
					PK.Values[Modifier] = Value;
					PK.IsNonPrintable[Modifier] = IsNonPrintable;
				}

				AllKeys.Add(PK);
			}
			InitialiseLists();
			UpdateModifiedByBoxes();
		}

		private CheckBox[] ModifiedByBoxes = new CheckBox[8];


		string[] AsciiCodes = new string[] { "nul", "soh", "stx", "etx", "eot", "enq", "ack", "bel", "bs", "ht", "lf", "vt", "ff", "cr", "so", "si", "dle", "dc1", "dc2", "dc3", "dc4", "nak", "syn", "etb", "can", "em", "eof", "esc", "fs", "gs", "rs", "us" };

		private string GetAsciiName(int id) {
			if (KeyValueSet == KeyValueSets.BBCBasic && ((id & 0xF0) == 0x80 || (id & 0xF0) == 0xC0)) {
				var key = BbcBasicFunctionKeys[((id & 0xF0) == 0x80) ? (id - 0x80) : (id - 0xC0 + 0x10)];
				return string.Format("{0:X2}{1}", id, key.Length > 0 ? (" (" + key + ")") : "");
			} else {
				return string.Format("{0:X2}{1}", id, id < AsciiCodes.Length ? string.Format(" ({0})", AsciiCodes[id].ToUpper()) : id == 127 ? " (DEL)" : "");
			}
			}

			public MainEditor() {
			InitializeComponent();

			int np = 0;
			foreach (string s in NonPrintable) {
				if (s == "-") {
					nonPrintableToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
				} else {
					NonPrintableNames.Add(s);
					ToolStripMenuItem T = new ToolStripMenuItem(string.Format("{0} ({1:X2})", s, np));
					T.Tag = (np++);
					T.Click += new EventHandler(ChosenKey);
					nonPrintableToolStripMenuItem.DropDownItems.Add(T);
				}
			}

			InitialiseKeyValueSelections();

			for (int i = 0; i < 8; ++i) {
				ModifiedByBoxes[i] = new CheckBox();
				ModifiedByBoxes[i].Dock = DockStyle.Top;
				ModifiedByBoxes[i].Padding = new Padding(10, 0, 0, 0);
				ModifiedByBoxes[i].Height = 18;
				ModifiedByGroup.Controls.Add(ModifiedByBoxes[i]);
				ModifiedByBoxes[i].BringToFront();
				ModifiedByBoxes[i].Click += new EventHandler(MainEditor_CheckedChanged);
			}
			UpdateModifiedByBoxes();

			InitialiseLists();

			//LoadXml("test2.xml");


		}

		enum KeyValueSets {
			TI83Plus,
			ASCII,
			BBCBasic,
		};

		KeyValueSets keyValueSet;
		KeyValueSets KeyValueSet {
			get {
				return this.keyValueSet;
			}
			set {
				if (this.keyValueSet != value) {
					this.keyValueSet = value;
					this.InitialiseKeyValueSelections();
				}
			}
		}

		void InitialiseKeyValueSelections() {

			List<ToolStripItem> contextMenuItemsToRemove = new List<ToolStripItem>();
			foreach (ToolStripItem item in KeysContext.Items) {
				if (!(item == noneToolStripMenuItem || item == toolStripMenuItem3 || item == nonPrintableToolStripMenuItem)) {
					contextMenuItemsToRemove.Add(item);
				}
			}
			foreach (var item in contextMenuItemsToRemove) {
				KeysContext.Items.Remove(item);
				item.Dispose();
			}

			List<ToolStripItem> UpperCaseChars = new List<ToolStripItem>();
			List<ToolStripItem> LowerCaseChars = new List<ToolStripItem>();
			List<ToolStripItem> NumericChars = new List<ToolStripItem>();
			List<ToolStripItem> AccentedChars = new List<ToolStripItem>();
			List<ToolStripItem> GreekChars = new List<ToolStripItem>();
			List<ToolStripItem> PunctuationChars = new List<ToolStripItem>();
			List<ToolStripItem> OtherChars = new List<ToolStripItem>();
			List<ToolStripItem> MathematicalChars = new List<ToolStripItem>();
			List<ToolStripItem> CursorChars = new List<ToolStripItem>();
			List<ToolStripItem> ArrowChars = new List<ToolStripItem>();

			List<ToolStripItem> ControlChars = new List<ToolStripItem>();

			List<int> Punctuation = new List<int>(new int[] { 0x21, 0x22, 0x23, 0x26, 0x27, 0x28, 0x29, 0x2C, 0x2E, 0x2F, 0x3A, 0x3B, 0x3F, 0x40, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x7B, 0x7C, 0x7D, 0x7E, 0xB9, 0xBA, 0xC1 });
			List<int> Mathematical = new List<int>(new int[] { 0x01, 0x02, 0x03, 0x04, 0x08, 0x09, 0x0D, 0x10, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x25, 0x2A, 0x2B, 0x2C, 0x2D, 0x3C, 0x3D, 0x3E, 0xCB, 0xCC, 0xD2, 0xD4, 0xD7, 0xD8, 0xDA, 0xDB, 0xDD });
			List<int> Arrows = new List<int>(new int[] { 0x05, 0x06, 0x07, 0x1C, 0x1E, 0x1F, 0xCF, 0xEF, 0xF0 });

			if (KeyValueSet == KeyValueSets.ASCII || KeyValueSet == KeyValueSets.BBCBasic) {

				Punctuation.Add((int)'[');
				Punctuation.Add((int)'$');
				Punctuation.RemoveAll(p => p <= 32 || p > 127);
				Punctuation.Sort();

				Mathematical.RemoveAll(p => p <= 32 || p > 127);
				Arrows.RemoveAll(p => p <= 32 || p > 127);
			}

			CharacterImageList.Images.Clear();

			using (Bitmap B = (KeyValueSet == KeyValueSets.TI83Plus) ? Properties.Resources.TI83PlusMap : Properties.Resources.ASCIIMap) {

				using (Bitmap D = new Bitmap(16, 16)) {

					ColorPalette P = B.Palette;
					P.Entries[0] = Color.FromKnownColor(KnownColor.WindowText);
					B.Palette = P;


					Graphics G = Graphics.FromImage(D);
					G.InterpolationMode = InterpolationMode.NearestNeighbor;
					G.PixelOffsetMode = PixelOffsetMode.Half;

					for (int i = 0; i < 256; ++i) {
						G.Clear(Color.Transparent);


						if (B.Width == 40) {
							G.DrawImage(B, new Rectangle(3, 1, 10, 16), new Rectangle((i >> 5) * 5, (i % 32) * 8, 5, 8), GraphicsUnit.Pixel);
						} else {
							G.DrawImage(B, new Rectangle(3, 1, 10, 16), new Rectangle(i * 6, 0, 5, 8), GraphicsUnit.Pixel);
						}


						CharacterImageList.Images.Add((Image)D.Clone());

						ToolStripMenuItem T = new ToolStripMenuItem(GetAsciiName(i), (Image)D.Clone(), ChosenKey);
						T.Tag = i;

						if (i < AsciiCodes.Length || i == 0x7F) {
							ToolStripMenuItem T2 = new ToolStripMenuItem(T.Text, T.Image, ChosenKey);
							T2.Tag = i;
							ControlChars.Add(T2);
						}
						if (i >= 'A' && i <= 'Z') {
							UpperCaseChars.Add(T);
						} else if (i >= 'a' && i <= 'z') {
							LowerCaseChars.Add(T);
						} else if ((i >= '0' && i <= '9') || (KeyValueSet == KeyValueSets.TI83Plus && ((i >= 0x80 && i <= 0x89) || i == 0x0E || i == 0x11 || i == 0x12 || i == 0x1D || i == 0x24 || i == 0xD3 || i == 0xD5))) {
							NumericChars.Add(T);
						} else if (KeyValueSet == KeyValueSets.TI83Plus && i >= 0x8A && i <= 0xB8) {
							AccentedChars.Add(T);
						} else if (KeyValueSet == KeyValueSets.TI83Plus && ((i >= 0xBB && i <= 0xCA && i != 0xC1) || i == 0x5B || i == 0xD9)) {
							GreekChars.Add(T);
						} else if (Punctuation.Contains(i)) {
							PunctuationChars.Add(T);
						} else if (Mathematical.Contains(i)) {
							MathematicalChars.Add(T);
						} else if (KeyValueSet == KeyValueSets.TI83Plus && ((i >= 0xE0 && i <= 0xE7) || i == 0xF1)) {
							CursorChars.Add(T);
						} else if (Arrows.Contains(i)) {
							ArrowChars.Add(T);
						} else {
							OtherChars.Add(T);
						}
					}
				}
			}



			ToolStripMenuItem ToAdd;

			ToAdd = new ToolStripMenuItem("Control");
			ToAdd.DropDownItems.AddRange(ControlChars.ToArray());
			KeysContext.Items.Add(ToAdd);

			KeysContext.Items.Add(new ToolStripSeparator());

			if (LowerCaseChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("Latin Lowercase");
				ToAdd.DropDownItems.AddRange(LowerCaseChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (UpperCaseChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("Latin Uppercase");
				ToAdd.DropDownItems.AddRange(UpperCaseChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (AccentedChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("Latin Accented");
				ToAdd.DropDownItems.AddRange(AccentedChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (GreekChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("Greek");
				ToAdd.DropDownItems.AddRange(GreekChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (NumericChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("Numeric");
				ToAdd.DropDownItems.AddRange(NumericChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (PunctuationChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("General/Punctuation");
				ToAdd.DropDownItems.AddRange(PunctuationChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (MathematicalChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("Mathematical");
				ToAdd.DropDownItems.AddRange(MathematicalChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (CursorChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("Cursors");
				ToAdd.DropDownItems.AddRange(CursorChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (Arrows.Count > 0) {
				ToAdd = new ToolStripMenuItem("Arrows");
				ToAdd.DropDownItems.AddRange(ArrowChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}

			if (KeyValueSet == KeyValueSets.BBCBasic) {
				KeysContext.Items.Add(new ToolStripSeparator());
				ToAdd = new ToolStripMenuItem("Function Keys");

				for (int i = 0x80, j = 0; j < BbcBasicFunctionKeys.Length; ++i, ++j) {
					if (i == 0x90) i = 0xC0;
					if (BbcBasicFunctionKeys[j].Length == 0) continue;
					ToolStripMenuItem T = new ToolStripMenuItem(GetAsciiName(i), null, ChosenKey);
					T.Tag = i;
					ToAdd.DropDownItems.Add(T);
				}
				KeysContext.Items.Add(ToAdd);
				if (OtherChars.Count > 0) {
					KeysContext.Items.Add(new ToolStripSeparator());
				}
			}

			if (OtherChars.Count > 0) {
				ToAdd = new ToolStripMenuItem("Other");
				ToAdd.DropDownItems.AddRange(OtherChars.ToArray());
				KeysContext.Items.Add(ToAdd);
			}
		}

		private void InitialiseLists() {
			AllKeys.Sort();
			AllKeysList.Items.Clear();
			AllKeysList.Items.AddRange(this.AllKeys.ToArray());
			/*if (AllKeysList.Items.Count == 0) {
				AddKey_Click(null, null);
			}*/
			if (AllKeysList.Items.Count > 0) {
				AllKeysList.SelectedIndex = 0;
			} else {
				AllKeysList.SelectedItem = null;
			}
			AllKeysList_SelectedIndexChanged(this, new EventArgs());
		}


		private void ChosenKey(object sender, EventArgs e) {
			if (KeyValueEditor.SelectedItems.Count != 1 || SelectedKey == null) return;
			ToolStripItem T = (ToolStripItem)sender;

			int Value = Convert.ToInt32(T.Tag);
			int Index = (int)(KeyValueEditor.SelectedItems[0].Tag);
			SelectedKey.Values[Index] = (byte)Value;
			SelectedKey.IsNonPrintable[Index] = (T.OwnerItem == nonPrintableToolStripMenuItem);
			UpdateKeyValues();
		}

		void MainEditor_CheckedChanged(object sender, EventArgs e) {
			PhysicalKey PK = SelectedKey;
			if (PK == null) return;
			int Mask = 0;
			for (int i = 0; i < 8; ++i) if (ModifiedByBoxes[i].Checked) Mask |= (1 << i);
			PK.ModifierMask = (byte)Mask;
			UpdateKeyValues();
		}

		private void UpdateModifiedByBoxes() {

			bool[] Enabled = new bool[8];

			foreach (PhysicalKey PK in this.AllKeys) {
				if (PK.IsModifier) {
					if (Enabled[PK.ModifierIndex]) {
						ModifiedByBoxes[PK.ModifierIndex].Text += ", " + PK.FriendlyName;
					} else {
						Enabled[PK.ModifierIndex] = true;
						ModifiedByBoxes[PK.ModifierIndex].Text = PK.FriendlyName;
					}
				}
			}

			for (int i = 0; i < 8; ++i) {
				ModifiedByBoxes[i].Enabled = Enabled[i];
				if (!Enabled[i]) {
					ModifiedByBoxes[i].Checked = false;
				}
			}

			PhysicalKey K = SelectedKey;
			if (K != null) {
				for (int i = 0; i < 8; ++i) {
					if (ModifiedByBoxes[i].Enabled && (K.ModifierMask & (1 << i)) != 0) {
						ModifiedByBoxes[i].Checked = true;
					} else {
						ModifiedByBoxes[i].Checked = false;
					}
				}
			}
		}

		private PhysicalKey SelectedKey {
			get { return (PhysicalKey)AllKeysList.SelectedItem; }
		}

		private void UpdateKeyValues() {

			PhysicalKey PK = SelectedKey;

			int ModifierMask = PK.ModifierMask;
			List<ListViewItem> Items = new List<ListViewItem>();


			List<int> DoneModifiers = new List<int>();
			for (int i = 0; i < 256; ++i) {
				int Modification = i & ModifierMask;
				if (!DoneModifiers.Contains(Modification)) {
					string Mod = string.Format("[{0}] ", SelectedKey.FriendlyName);
					bool First = true;
					for (int j = 0; j < 8; ++j) {
						if ((Modification & (1 << j)) != 0) {
							if (First) {
								First = false;
							} else {
								Mod += "+";
							}
							Mod += "[" + ModifiedByBoxes[j].Text + "]";
						}
					}

					ListViewItem LVI = new ListViewItem(GetAsciiName(PK.Values[Modification]), PK.Values[Modification]);
					if (PK.Values[Modification] == 0xFF) {
						LVI.Text = "(None)";
						LVI.ImageIndex = -1;
					} else if (PK.IsNonPrintable[Modification]) {
						LVI.Text = string.Format("{0} ({1:X2})", NonPrintableNames[PK.Values[Modification]], PK.Values[Modification]);
						LVI.ImageIndex = -1;
					}
					LVI.Tag = Modification;
					LVI.SubItems.Add(new ListViewItem.ListViewSubItem(LVI, Mod));
					Items.Add(LVI);
					DoneModifiers.Add(Modification);
				}
			}

			this.KeyValueEditor.Items.Clear();
			this.KeyValueEditor.Items.AddRange(Items.ToArray());

		}

		private void AllKeysList_SelectedIndexChanged(object sender, EventArgs e) {
			if (Working) return;
			if (AllKeysList.SelectedItem == null) {
				MainSplit.Panel2.Enabled = false;
			} else {
				MainSplit.Panel2.Enabled = true;
				KeyIsModifier.Checked = SelectedKey.IsModifier;
				ToggleModification.Enabled = SelectedKey.IsModifier;
				ToggleModification.Checked = SelectedKey.IsModifier && SelectedKey.IsToggle;
				KeyIsModifier_CheckedChanged(sender, e);
				ToggleModification_CheckedChanged(sender, e);
				MainEditor_CheckedChanged(sender, e);
				ScanCode.Value = SelectedKey.ScanCode;
				IsExtended.Checked = SelectedKey.IsExtended;
				Friendly.Text = SelectedKey.FriendlyName;
				DeviceCode.Value = SelectedKey.DeviceCode;
				SecondaryDeviceCode.Value = SelectedKey.SecondaryDeviceCode;
			}
		}

		private void KeyIsModifier_CheckedChanged(object sender, EventArgs e) {
			SelectedKey.IsModifier = KeyIsModifier.Checked;
			ToggleModification.Enabled = SelectedKey.IsModifier;
			ToggleModification.Checked = SelectedKey.IsModifier && SelectedKey.IsToggle;
			ModifiedByGroup.Enabled = !SelectedKey.IsModifier;
			ModifierIndex.Enabled = SelectedKey.IsModifier;
			ModifierIndex.SelectedIndex = SelectedKey.IsModifier ? SelectedKey.ModifierIndex : 0;
			LedIndex.SelectedIndex = (SelectedKey.LedIndex + 1) & 0xFF;
			UpdateModifiedByBoxes();
		}

		private void MainEditor_FormClosed(object sender, FormClosedEventArgs e) {
			//SaveXml("test2.xml");
		}

		private void ToggleModification_CheckedChanged(object sender, EventArgs e) {
			if (SelectedKey.IsModifier) SelectedKey.IsToggle = ToggleModification.Checked;
			this.LedIndex.Enabled = ToggleModification.Checked;
		}

		private void ModifierIndex_SelectedIndexChanged(object sender, EventArgs e) {
			if (SelectedKey.IsModifier) {
				SelectedKey.ModifierIndex = ModifierIndex.SelectedIndex;
				UpdateModifiedByBoxes();
			}
		}

		private void KeysContext_Opening(object sender, CancelEventArgs e) {
			if (this.KeyValueEditor.SelectedItems.Count != 1) e.Cancel = true;
		}

		private struct CompiledData {
			public ushort[] DataOffsetsBinary;
			public byte[] DataBinary;
			public byte[] DataFullBinary;
			public string DataOffsetsAsm;
			public string DataAsm;
			public string DataFullAsm;
			public string DataOffsetsC;
			public string DataC;
			public string DataFullC;
		}

		private CompiledData GetExportData() {

			// Do we have any device codes?
			bool IncludeDeviceCode = false;
			foreach (var PK in AllKeys) {
				if (PK.DeviceCode != 0) {
					IncludeDeviceCode = true;
					break;
				}
			}

			List<ushort> DataOffsets = new List<ushort>();

			StringBuilder DataAsm = new StringBuilder(1024);

			StringBuilder DataC = new StringBuilder(1024);
			StringBuilder DataOffsetsC = new StringBuilder(1024);

			List<byte> OutputData = new List<byte>();

			List<PhysicalKey> StandardKeys = new List<PhysicalKey>();
			List<PhysicalKey> ExtendedKeys = new List<PhysicalKey>();

			List<byte> DifferentMasks = new List<byte>();

			foreach (PhysicalKey PK in AllKeys) {
				if (!DifferentMasks.Contains(PK.ModifierMask) && PK.ModifierMask != 0) DifferentMasks.Add(PK.ModifierMask);
				if (!PK.IsExtended) {
					StandardKeys.Add(PK);
				} else {
					ExtendedKeys.Add(PK);
				}
			}

			List<byte>[] ModifierLookup = new List<byte>[256];

			int SizeOfLookup = 0;

			foreach (byte B in DifferentMasks) {
				SizeOfLookup += 3; // Definition, 2 for offset
				ModifierLookup[B] = new List<byte>();
				for (int i = 0; i < 256; ++i) {
					byte Modifier = (byte)(i & B);
					if (!ModifierLookup[B].Contains(Modifier)) {
						ModifierLookup[B].Add(Modifier);
						++SizeOfLookup;
					}
				}
			}

			DataAsm.AppendLine("; =====================================");
			DataAsm.AppendLine(string.Format(".db \"{0}\", 0 ; Description", DescriptionText.Text.Replace("\"", "\\\"")));
			foreach (char c in DescriptionText.Text) {
				OutputData.Add((byte)c);
			}
			OutputData.Add(0x00);
			DataAsm.AppendLine("; =====================================");

			DataOffsets.Add((ushort)StandardKeys.Count);
			DataOffsets.Add((ushort)ExtendedKeys.Count);

			DataOffsetsC.AppendLine(string.Format("#define KEYBOARD_SCANCODE_COUNT ({0})", StandardKeys.Count));
			DataOffsetsC.AppendLine(string.Format("#define KEYBOARD_EXTENDED_SCANCODE_COUNT ({0})", ExtendedKeys.Count));
			DataOffsetsC.AppendLine(string.Format("#define KEYBOARD_MASK_COMBINATION_COUNT ({0})", DifferentMasks.Count));

			// --- //
			DataOffsets.Add((ushort)OutputData.Count);
			// --- //

			DataC.AppendLine("const uint8_t keyboard_masks[] PROGMEM = {");
			foreach (byte B in DifferentMasks) {
				DataAsm.AppendLine(string.Format(".db %{0}", ToBinary(B, 8)));
				DataC.AppendLine(string.Format("\t0b{0},", ToBinary(B, 8)));
				OutputData.Add(B);
			}
			DataAsm.AppendLine("; -------------------------------------");
			DataC.AppendLine("};");

			int Offset = 0;
			int BitmaskIndex = 0;

			// --- //
			DataOffsets.Add((ushort)OutputData.Count);
			// --- //

			DataOffsetsC.AppendLine("const uint8_t keyboard_masks_offset[] PROGMEM = {");
			foreach (byte B in DifferentMasks) {
				ushort value = (ushort)(Offset + ((DifferentMasks.Count - BitmaskIndex) * 2) - 1);
				DataAsm.AppendLine(string.Format(".dw ${0:X4}", value));
				OutputData.Add((byte)(value & 0xFF));
				OutputData.Add((byte)(value >> 8));
				DataOffsetsC.AppendLine(string.Format("\t0x{0:X2},", Offset));
				Offset += ModifierLookup[B].Count;
				++BitmaskIndex;
			}
			DataOffsetsC.AppendLine("};");

			DataAsm.AppendLine("; -------------------------------------");
			DataC.AppendLine("const uint8_t keyboard_masks_table[] PROGMEM = {");
			foreach (byte B in DifferentMasks) {
				DataAsm.Append(".db ");
				DataC.Append("\t");
				bool First = true;
				foreach (Byte B2 in ModifierLookup[B]) {
					if (First) {
						First = false;
					} else {
						DataC.Append(" ");
						DataAsm.Append(", ");
					}
					DataAsm.Append(string.Format("%{0}", ToBinary(B2, 8)));
					DataC.Append(string.Format("0b{0},", ToBinary(B2, 8)));
					OutputData.Add(B2);
				}
				DataAsm.AppendLine();
				DataC.AppendLine();
			}
			DataC.AppendLine("};");


			if (IncludeDeviceCode) {
				DataAsm.AppendLine("; =====================================");
				foreach (var PK in StandardKeys) {
					DataAsm.AppendLine(string.Format(".db {0} ; {1} Device Code", PK.DeviceCode, PK.FriendlyName));
					OutputData.Add(PK.DeviceCode);
				}
				DataAsm.AppendLine("; -------------------------------------");
				foreach (var PK in ExtendedKeys) {
					DataAsm.AppendLine(string.Format(".db {0} ; {1} Device Code", PK.DeviceCode, PK.FriendlyName));
					OutputData.Add(PK.DeviceCode);
				}
			}

			DataAsm.AppendLine("; =====================================");
			{
				DataC.AppendLine("const uint8_t keyboard_scancodes[] PROGMEM = {");
				DataC.AppendLine("\t/* Standard */");
				{
					// --- //
					DataOffsets.Add((ushort)OutputData.Count);
					// --- //
					foreach (PhysicalKey PK in StandardKeys) {
						DataAsm.AppendLine(string.Format(".db ${0:X2} ; {1}", PK.ScanCode, PK.FriendlyName));
						DataC.AppendLine(string.Format("\t0x{0:X2}, /* {1} */", PK.ScanCode, PK.FriendlyName));
						OutputData.Add(PK.ScanCode);
					}
				}
				DataAsm.AppendLine("; -------------------------------------");
				DataC.AppendLine("\t/* Extended */");
				{
					// --- //
					DataOffsets.Add((ushort)OutputData.Count);
					// --- //
					foreach (PhysicalKey PK in ExtendedKeys) {
						DataAsm.AppendLine(string.Format(".db ${0:X2} ; {1}", PK.ScanCode, PK.FriendlyName));
						DataC.AppendLine(string.Format("\t0x{0:X2}, /* {1} */", PK.ScanCode, PK.FriendlyName));
						OutputData.Add(PK.ScanCode);
					}
				}
				DataC.AppendLine("};");
			}
			DataAsm.AppendLine("; =====================================");

			// --- //
			DataOffsets.Add((ushort)OutputData.Count);
			// --- //

			int TotalOffset = 0;
			int TotalOffsetIncludingPhysical = 0;

			List<int> Sizes = new List<int>();

			int TotalKeys = StandardKeys.Count + ExtendedKeys.Count;

			int KeyIndex = 0;

			DataOffsetsC.AppendLine("const uint16_t keyboard_data_offsets[] PROGMEM = {");
			for (int i = 0; i < 2; ++i) {
				foreach (PhysicalKey PK in new List<PhysicalKey>[] { StandardKeys, ExtendedKeys }[i]) {

					int ExtendedInfo = 0;
					if (PK.IsModifier) {
						ExtendedInfo |= 8;
						if (PK.IsToggle) ExtendedInfo |= 4;
					}

					DataAsm.AppendLine(string.Format(".dw (%{1} << 12) | ${0:X4}", TotalOffset + ((TotalKeys - 2) * 2) - 1, ToBinary(ExtendedInfo, 4)));
					DataOffsetsC.AppendLine(string.Format("\t(0b{1} << 12) | 0x{0:X4}, /* {2} */", TotalOffsetIncludingPhysical, ToBinary(ExtendedInfo, 4), PK.FriendlyName));

					ushort value = (ushort)((TotalOffset + ((TotalKeys - KeyIndex) * 2) - 1) | (ExtendedInfo << 12));
					OutputData.Add((byte)(value & 0xFF));
					OutputData.Add((byte)(value >> 8));

					int Size = 0;

					if (PK.IsModifier) {
						++Size;
						if (PK.IsToggle) ++Size;
					}
					++Size; // Status bitmask.
					int NumBits = 0;
					for (int j = 0; j < 8; ++j) if ((PK.ModifierMask & (1 << j)) != 0) ++NumBits;
					Size += (1 << NumBits);

					Sizes.Add(Size);
					TotalOffset += Size;

					TotalOffsetIncludingPhysical += Size;
					++TotalOffsetIncludingPhysical;
					if (PK.DeviceCode != 0 && PK.SecondaryDeviceCode != 0) {
						++TotalOffsetIncludingPhysical;
					}

					++KeyIndex;
				}

			}
			DataOffsetsC.AppendLine("};");
			DataAsm.AppendLine("; =====================================");

			// --- //
			DataOffsets.Add((ushort)OutputData.Count);
			// --- //

			DataC.AppendLine("const uint8_t keyboard_data[] PROGMEM = {");
			List<bool> NonPrintableBitsAsm = new List<bool>();
			List<bool> NonPrintableBitsC = new List<bool>();
			int k = 0;
			for (int i = 0; i < 2; ++i) {
				foreach (PhysicalKey PK in new List<PhysicalKey>[] { StandardKeys, ExtendedKeys }[i]) {

					DataC.AppendFormat("\t/* {0} */ ", PK.ToString());

					if (PK.DeviceCode == 0) {
						DataC.Append("0, ");
						NonPrintableBitsC.Add(false);
					} else {
						DataC.AppendFormat("/* INKEY({0}) */ {1}{2}, ", -PK.DeviceCode, PK.DeviceCode, PK.SecondaryDeviceCode != 0 ? " | 0x80" : "");
						NonPrintableBitsC.Add(false);
						if (PK.SecondaryDeviceCode != 0) {
							DataC.AppendFormat("/* INKEY({0}) */ {1}, ", -PK.SecondaryDeviceCode, PK.SecondaryDeviceCode);
							NonPrintableBitsC.Add(false);
						}
					}

					int Size = Sizes[k++];

					if (PK.IsModifier) {
						NonPrintableBitsAsm.Add(false);
						NonPrintableBitsC.Add(false);
						DataAsm.Append(string.Format(".db %{0}", ToBinary(1 << PK.ModifierIndex, 8)));
						DataC.AppendFormat("0b{0}, ", ToBinary(1 << PK.ModifierIndex, 8));
						OutputData.Add((byte)(1 << PK.ModifierIndex));
						if (PK.IsToggle) {
							NonPrintableBitsAsm.Add(false);
							NonPrintableBitsC.Add(false);
							DataAsm.Append(string.Format(", %{0}", ToBinary(1 << PK.LedIndex, 8)));
							DataC.AppendFormat("0b{0}, ", ToBinary(1 << PK.LedIndex, 8));
							OutputData.Add((byte)(1 << PK.LedIndex));
						}
						DataAsm.AppendLine(" ; Modifier data for next key.");
						DataC.Append("/* <- Modifier */ ");
					}
					NonPrintableBitsAsm.Add(false);
					NonPrintableBitsC.Add(false);
					DataAsm.Append(string.Format(".db %{0}", ToBinary(PK.ModifierMask, 8)));
					DataC.AppendFormat("0b{0}, ", ToBinary(PK.ModifierMask, 8));
					OutputData.Add(PK.ModifierMask);

					List<int> AddedModifications = new List<int>();
					for (int m = 0; m < 256; ++m) {
						int Modifier = m & PK.ModifierMask;
						if (!AddedModifications.Contains(Modifier)) {
							DataAsm.Append(string.Format(", ${0:X2}", PK.Values[Modifier]));
							DataC.AppendFormat("0x{0:X2}, ", PK.Values[Modifier]);
							OutputData.Add(PK.Values[Modifier]);
							NonPrintableBitsAsm.Add(PK.IsNonPrintable[Modifier]);
							NonPrintableBitsC.Add(PK.IsNonPrintable[Modifier]);
							AddedModifications.Add(Modifier);
						}
					}

					DataAsm.AppendLine(" ; " + PK.ToString());
					DataC.AppendLine();
				}
			}
			DataC.AppendLine("};");

			DataAsm.AppendLine("; =====================================");

			// --- //
			DataOffsets.Add((ushort)OutputData.Count);
			// --- //

			byte[] NonPrintableBytesAsm = new byte[(int)Math.Ceiling(NonPrintableBitsAsm.Count / 8m)];
			for (int i = 0; i < NonPrintableBitsAsm.Count; ++i) {
				if (NonPrintableBitsAsm[i]) {
					NonPrintableBytesAsm[i >> 3] |= (byte)(0x80 >> (i & 7));
				}
			}
			foreach (byte b in NonPrintableBytesAsm) {
				DataAsm.AppendLine(string.Format(".db %{0}", ToBinary(b, 8)));
				OutputData.Add(b);
			}

			byte[] NonPrintableBytesC = new byte[(int)Math.Ceiling(NonPrintableBitsC.Count / 8m)];
			for (int i = 0; i < NonPrintableBitsC.Count; ++i) {
				if (NonPrintableBitsC[i]) {
					NonPrintableBytesC[i >> 3] |= (byte)(0x80 >> (i & 7));
				}
			}

			DataC.AppendLine("const uint8_t keyboard_unprintable_data[] PROGMEM = {");
			foreach (byte b in NonPrintableBytesC) {
				DataC.AppendLine(string.Format("\t0b{0},", ToBinary(b, 8)));
				OutputData.Add(b);
			}
			DataC.AppendLine("};");


			//Console.WriteLine(DataOffsets.Count);

			string DataOffsetsAsm = ".dw ";
			for (int i = 0; i < DataOffsets.Count; ++i) {
				if (i >= 2) {
					DataOffsets[i] += (ushort)(DataOffsets.Count * 2);
				}
				if (i != 0) DataOffsetsAsm += ", ";
				DataOffsetsAsm += "$" + ((int)(DataOffsets[i])).ToString("X4");
			}

			//Clipboard.SetText(Offsets + "\r\n" + KeyData.ToString());

			byte[] OutputBinary = new byte[OutputData.Count + DataOffsets.Count * 2];
			OutputData.CopyTo(OutputBinary, DataOffsets.Count * 2);

			for (int j = 0; j < DataOffsets.Count; ++j) {
				OutputBinary[j * 2 + 0] = (byte)(DataOffsets[j] & 0xFF);
				OutputBinary[j * 2 + 1] = (byte)(DataOffsets[j] >> 8);
			}

			return new CompiledData {
				DataOffsetsBinary = DataOffsets.ToArray(),
				DataBinary = OutputData.ToArray(),
				DataFullBinary = OutputBinary,
				DataOffsetsAsm = DataOffsetsAsm,
				DataAsm = DataAsm.ToString(),
				DataFullAsm = DataOffsetsAsm + Environment.NewLine + DataAsm.ToString(),
				DataOffsetsC = DataOffsetsC.ToString(),
				DataC = DataC.ToString(),
				DataFullC = DataOffsetsC.ToString() + DataC.ToString(),
			};
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e) {

		}

		private string ToBinary(int input, int length) {
			return ToBinary((uint)input, length);
		}
		private string ToBinary(uint input, int length) {
			char[] output = new char[length];
			for (int i = 0; i < length; ++i) {
				output[length - i - 1] = ((input & 1) != 0) ? '1' : '0';
				input >>= 1;
			}
			return new string(output);
		}

		private void LedIndex_SelectedIndexChanged(object sender, EventArgs e) {
			SelectedKey.LedIndex = (byte)(LedIndex.SelectedIndex - 1);
		}

		private void AddKey_Click(object sender, EventArgs e) {
			PhysicalKey K = new PhysicalKey();
			K.FriendlyName = "<New>";
			this.AllKeys.Add(K);
			this.AllKeys.Sort();
			this.AllKeysList.Items.Clear();
			this.AllKeysList.Items.AddRange(this.AllKeys.ToArray());
			this.AllKeysList.SelectedItem = K;

		}

		private void DeleteKey_Click(object sender, EventArgs e) {
			int i = this.AllKeysList.SelectedIndex;
			this.AllKeys.Remove((PhysicalKey)this.AllKeysList.SelectedItem);
			this.AllKeysList.Items.Remove(this.AllKeysList.SelectedItem);
			if (i >= 0 && i < this.AllKeysList.Items.Count) {
				this.AllKeysList.SelectedIndex = i;
			}
		}

		private void Friendly_Leave(object sender, EventArgs e) {
			PhysicalKey PK = SelectedKey;
			PK.FriendlyName = Friendly.Text;
			AllKeys.Sort();
			this.AllKeysList.Items.Clear();
			this.AllKeysList.Items.AddRange(this.AllKeys.ToArray());
			this.AllKeysList.SelectedItem = PK;
		}

		private void UpdateSelected() {
			Working = true;
			PhysicalKey PK = SelectedKey;
			int i = AllKeysList.SelectedIndex;
			AllKeysList.Items.RemoveAt(i);
			AllKeysList.Items.Insert(i, PK);
			Working = false;
			AllKeysList.SelectedIndex = i;
		}

		bool Working = false;
		private void ScanCode_ValueChanged(object sender, EventArgs e) {
			SelectedKey.ScanCode = (byte)ScanCode.Value;
			UpdateSelected();
		}

		private void DeviceCode_ValueChanged(object sender, EventArgs e) {
			SelectedKey.DeviceCode = (byte)DeviceCode.Value;
			UpdateSelected();
		}

		private void SecondaryDeviceCode_ValueChanged(object sender, EventArgs e) {
			SelectedKey.SecondaryDeviceCode = (byte)SecondaryDeviceCode.Value;
			UpdateSelected();
		}

		private void IsExtended_CheckedChanged(object sender, EventArgs e) {
			SelectedKey.IsExtended = IsExtended.Checked;
			UpdateSelected();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e) {
			if (OpenXmlDialog.ShowDialog() == DialogResult.OK) {
				try {
					LoadXml(OpenXmlDialog.FileName);
				} catch (Exception ex) {
					MessageBox.Show(this, "Couldn't load file: " + ex.Message, "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
			if (SaveXmlDialog.ShowDialog() == DialogResult.OK) {
				try {
					SaveXml(SaveXmlDialog.FileName);
				} catch (Exception ex) {
					MessageBox.Show(this, "Couldn't save file: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void toTI83PlusFileToolStripMenuItem_Click(object sender, EventArgs e) {

			var CompiledData = this.GetExportData();

			var DataOffsets = CompiledData.DataOffsetsBinary;
			var OutputData = CompiledData.DataBinary;


			int Success = 0;


			#region TI binary

			for (int i = 0; i < 2; ++i) {

				try {

					string BinaryFile = Path.Combine(Application.StartupPath, string.Format("{0} Keyboard Layout (Emerson)", this.DescriptionText.Text.Trim()) + "." + (i == 0 ? "8xp" : "83p"));

					if (File.Exists(BinaryFile)) {
						if (MessageBox.Show(this, BinaryFile + " already exists. Overwrite?", "Export", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) continue;
					} else {
						File.Delete(BinaryFile);
					}

					// Write a TI-compatible binary:

					using (BinaryWriter BW = new BinaryWriter(new FileStream(BinaryFile, FileMode.OpenOrCreate), Encoding.ASCII)) {

						// 8 byte type signature

						switch (i) {
							case 0:
								BW.Write("**TI83F*".ToCharArray()); break;
							case 1:
								BW.Write("**TI83**".ToCharArray()); break;
						}

						// 3 byte signature

						BW.Write((byte)0x1A);
						BW.Write((byte)0x0A);
						BW.Write((byte)0x00);

						// 42-byte comment

						BW.Write(("Emerson Keyboard Layout [Version 1]".PadRight(42, (char)0x00).ToCharArray()));

						#region Forming variable entry

						// Build up the variable entry:

						List<byte> FormattedVariable = new List<byte>();

						int VariableNameLength = 8;

						switch (i) {
							case 0:
								FormattedVariable.Add(0x0D);
								FormattedVariable.Add(0x00);
								break;
							case 1:
								FormattedVariable.Add(0x0B);
								FormattedVariable.Add(0x00);
								break;
						}


						// Total size of the data

						byte[] OutputBinary = new byte[OutputData.Length + DataOffsets.Length * 2 + 8];
						for (int j = 0; j < 8; ++j) OutputBinary[j] = (byte)"EmerKey1"[j];
						OutputData.CopyTo(OutputBinary, DataOffsets.Length * 2 + 8);

						for (int j = 0; j < DataOffsets.Length; ++j) {
							OutputBinary[j * 2 + 8] = (byte)(DataOffsets[j] & 0xFF);
							OutputBinary[j * 2 + 9] = (byte)(DataOffsets[j] >> 8);
						}

						int TotalSize = OutputBinary.Length; // (BinaryEndLocation - BinaryStartLocation) + 1;

						int HeaderSize = 2;

						FormattedVariable.Add((byte)((TotalSize + HeaderSize) & 0xFF));
						FormattedVariable.Add((byte)((TotalSize + HeaderSize) >> 8));

						// Type ID byte
						FormattedVariable.Add((byte)0x06);

						// Format variable name
						string VariableName = "EMRSNKYB".PadRight(VariableNameLength, (char)0x00);
						for (int l = 0; l < VariableNameLength; l++) {
							FormattedVariable.Add((byte)VariableName[l]);
						}


						if (i == 0) {
							FormattedVariable.Add((byte)(0x00));
							FormattedVariable.Add(0x00);
						}

						// Size (again)
						FormattedVariable.Add((byte)((TotalSize + HeaderSize) & 0xFF));
						FormattedVariable.Add((byte)((TotalSize + HeaderSize) >> 8));

						// Program header (2 bytes for size):
						FormattedVariable.Add((byte)(TotalSize & 0xFF));
						FormattedVariable.Add((byte)(TotalSize >> 8));

						// Write the binary itself

						for (int l = 0; l < OutputBinary.Length; ++l) {
							FormattedVariable.Add(OutputBinary[l]);
						}

						#endregion

						// Write size

						BW.Write((byte)(FormattedVariable.Count & 0xFF));
						BW.Write((byte)(FormattedVariable.Count >> 8));

						ushort CheckSum = 0;
						for (int l = 0; l < FormattedVariable.Count; l++) {
							byte b = FormattedVariable[l];
							CheckSum += b;
						}
						BW.Write(FormattedVariable.ToArray());

						BW.Write((byte)(CheckSum & 0xFF));
						BW.Write((byte)(CheckSum >> 8));
					}
					++Success;
				} catch (Exception ex) {
					MessageBox.Show(this, "Error: " + ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			#endregion

			if (Success > 0) {
				MessageBox.Show(this, "Exported " + Success.ToString() + " layout file" + (Success == 1 ? "" : "s") + " successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void toRawBinaryToolStripMenuItem_Click(object sender, EventArgs e) {
			if (this.ExportBinaryDialog.ShowDialog(this) == DialogResult.OK) {
				var data = this.GetExportData();
				try {
					File.WriteAllBytes(this.ExportBinaryDialog.FileName, data.DataFullBinary);
				} catch (Exception ex) {
					MessageBox.Show(this, "Could not export: " + ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void toTextToolStripMenuItem_Click(object sender, EventArgs e) {
			if (this.ExportAssemblyDialog.ShowDialog(this) == DialogResult.OK) {
				var data = this.GetExportData();
				try {
					File.WriteAllText(this.ExportAssemblyDialog.FileName, data.DataFullAsm);
				} catch (Exception ex) {
					MessageBox.Show(this, "Could not export: " + ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}


		private void toTextCToolStripMenuItem_Click(object sender, EventArgs e) {
			if (this.ExportCDialog.ShowDialog(this) == DialogResult.OK) {
				var data = this.GetExportData();
				try {
					File.WriteAllText(this.ExportCDialog.FileName, data.DataFullC);
				} catch (Exception ex) {
					MessageBox.Show(this, "Could not export: " + ex.Message, "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void tI83PlusToolStripMenuItem_Click(object sender, EventArgs e) {
			this.KeyValueSet = KeyValueSets.TI83Plus;
		}

		private void aSCIIToolStripMenuItem_Click(object sender, EventArgs e) {
			this.KeyValueSet = KeyValueSets.ASCII;
		}

		private void bBCBASICToolStripMenuItem_Click(object sender, EventArgs e) {
			this.KeyValueSet = KeyValueSets.BBCBasic;
		}

		private void keySetsToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
			this.tI83PlusToolStripMenuItem.Checked = this.KeyValueSet == KeyValueSets.TI83Plus;
			this.aSCIIToolStripMenuItem.Checked = this.KeyValueSet == KeyValueSets.ASCII;
			this.bBCBASICToolStripMenuItem.Checked = this.KeyValueSet == KeyValueSets.BBCBasic;
		}

		private void AllKeysList_MouseDown(object sender, MouseEventArgs e) {
			if (this.AllKeysList.SelectedItem == null) return;
			this.AllKeysList.DoDragDrop(this.AllKeysList.SelectedItem, DragDropEffects.Move);
			this.AllKeysList_SelectedIndexChanged(sender, e);
		}

		private void AllKeysList_DragOver(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent("Keyboard_Layout_Editor.PhysicalKey")) {
				e.Effect = DragDropEffects.Move;
			}
		}

		private void AllKeysList_DragDrop(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent("Keyboard_Layout_Editor.PhysicalKey")) {
				if (this.AllKeysList.Items.Count > 0 && this.AllKeysList.SelectedItem != null) {
					Point point = this.AllKeysList.PointToClient(new Point(e.X, e.Y));
					int index = this.AllKeysList.IndexFromPoint(point);
					if (index < 0) index = this.AllKeysList.Items.Count - 1;
					PhysicalKey data = this.AllKeysList.SelectedItem as PhysicalKey;
					this.AllKeysList.Items.Remove(data);
					this.AllKeysList.Items.Insert(index, data);
					this.AllKeys.Remove(data);
					this.AllKeys.Insert(index, data);
					this.AllKeysList.SelectedItem = data;
				}
			}
		}

	}
}