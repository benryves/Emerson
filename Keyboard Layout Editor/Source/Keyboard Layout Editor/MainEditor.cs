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
            "Escape"
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
            return string.Format("{0:X2}{1}", id, id < AsciiCodes.Length ? string.Format(" ({0})", AsciiCodes[id].ToUpper()) : id == 127 ? " (DEL)" : "");
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

            List<int> Punctuation = new List<int>(new int[]{ 0x21, 0x22, 0x23, 0x26, 0x27, 0x28, 0x29, 0x2C, 0x2E, 0x2F, 0x3A, 0x3B, 0x3F, 0x40, 0x5C, 0x5D, 0x5E, 0x5F, 0x60, 0x7B, 0x7C, 0x7D, 0x7E, 0xB9, 0xBA, 0xC1 });
            List<int> Mathematical = new List<int>(new int[] { 0x01, 0x02, 0x03, 0x04, 0x08, 0x09, 0x0D, 0x10, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x25, 0x2A, 0x2B, 0x2C, 0x2D, 0x3C, 0x3D, 0x3E, 0xCB, 0xCC, 0xD2, 0xD4, 0xD7, 0xD8, 0xDA, 0xDB, 0xDD });
            List<int> Arrows = new List<int>(new int[] { 0x05, 0x06, 0x07, 0x1C, 0x1E, 0x1F, 0xCF, 0xEF, 0xF0 });
            using (Bitmap B = Properties.Resources.ASCIIMap) {
                using (Bitmap D = new Bitmap(16, 16)) {

                    ColorPalette P = B.Palette;
                    P.Entries[0] = Color.FromKnownColor(KnownColor.WindowText);
                    B.Palette = P;


                    Graphics G = Graphics.FromImage(D);
                    G.InterpolationMode = InterpolationMode.NearestNeighbor;
                    G.PixelOffsetMode = PixelOffsetMode.Half;




                    for (int i = 0; i < 256; ++i) {
                        G.Clear(Color.Transparent);
                        G.DrawImage(B, new Rectangle(3, 1, 10, 16), new Rectangle((i >> 5) * 5, (i % 32) * 8, 5, 8), GraphicsUnit.Pixel);

                        CharacterImageList.Images.Add((Image)D.Clone());

                        ToolStripMenuItem T = new ToolStripMenuItem(GetAsciiName(i), (Image)D.Clone(), ChosenKey);
                        T.Tag = i;

                        if (i < AsciiCodes.Length || i==0x7F) {
                            ToolStripMenuItem T2 = new ToolStripMenuItem(T.Text, T.Image, ChosenKey);
                            T2.Tag = i;
                            ControlChars.Add(T2);
                        }
                        if (i >= 'A' && i <= 'Z') {
                            UpperCaseChars.Add(T);
                        } else if (i >= 'a' && i <= 'z') {
                            LowerCaseChars.Add(T);
                        } else if ((i >= '0' && i <= '9') || (i>=0x80 && i<=0x89) || i==0x0E || i==0x11 || i==0x12 || i==0x1D || i==0x24||i==0xD3 ||i==0xD5) {
                            NumericChars.Add(T);
                        } else if (i >= 0x8A && i <= 0xB8) {
                            AccentedChars.Add(T);
                        } else if ((i >= 0xBB && i <= 0xCA && i != 0xC1) || i == 0x5B || i==0xD9) {
                            GreekChars.Add(T);
                        } else if (Punctuation.Contains(i)) {
                            PunctuationChars.Add(T);
                        } else if (Mathematical.Contains(i)) {
                            MathematicalChars.Add(T);
                        } else if ((i >= 0xE0 && i<=0xE7) || i==0xF1) {
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

            ToAdd = new ToolStripMenuItem("Latin Lowercase");
            ToAdd.DropDownItems.AddRange(LowerCaseChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("Latin Uppercase");
            ToAdd.DropDownItems.AddRange(UpperCaseChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("Latin Accented");
            ToAdd.DropDownItems.AddRange(AccentedChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("Greek");
            ToAdd.DropDownItems.AddRange(GreekChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("Numeric");
            ToAdd.DropDownItems.AddRange(NumericChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("General/Punctuation");
            ToAdd.DropDownItems.AddRange(PunctuationChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("Mathematical");
            ToAdd.DropDownItems.AddRange(MathematicalChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("Cursors");
            ToAdd.DropDownItems.AddRange(CursorChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("Arrows");
            ToAdd.DropDownItems.AddRange(ArrowChars.ToArray());
            KeysContext.Items.Add(ToAdd);

            ToAdd = new ToolStripMenuItem("Other");
            ToAdd.DropDownItems.AddRange(OtherChars.ToArray());
            KeysContext.Items.Add(ToAdd);

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

        private void InitialiseLists() {
            AllKeys.Sort();
            AllKeysList.Items.Clear();
            AllKeysList.Items.AddRange(this.AllKeys.ToArray());
            if (AllKeysList.Items.Count == 0) {
                AddKey_Click(null, null);
            }
            AllKeysList.SelectedIndex = 0;
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

        private void exportToolStripMenuItem_Click(object sender, EventArgs e) {

            List<ushort> DataOffsets = new List<ushort>();
            StringBuilder KeyData = new StringBuilder(1024);

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

            KeyData.AppendLine("; =====================================");
            KeyData.AppendLine(string.Format(".db \"{0}\", 0 ; Description", DescriptionText.Text.Replace("\"", "\\\"")));
            foreach (char c in DescriptionText.Text) {
                OutputData.Add((byte)c);
            }
            OutputData.Add(0x00);
            KeyData.AppendLine("; =====================================");

            DataOffsets.Add((ushort)StandardKeys.Count);
            DataOffsets.Add((ushort)ExtendedKeys.Count);

            // --- //
            DataOffsets.Add((ushort)OutputData.Count);
            // --- //

            foreach (byte B in DifferentMasks) {
                KeyData.AppendLine(string.Format(".db %{0}", ToBinary(B, 8)));
                OutputData.Add(B);
            }
            KeyData.AppendLine("; -------------------------------------");
            int Offset = 0;
            int BitmaskIndex = 0;

            // --- //
            DataOffsets.Add((ushort)OutputData.Count);
            // --- //

            foreach (byte B in DifferentMasks) {
                ushort value = (ushort)(Offset + ((DifferentMasks.Count - BitmaskIndex) * 2) - 1);
                KeyData.AppendLine(string.Format(".dw ${0:X4}", value));
                OutputData.Add((byte)(value & 0xFF));
                OutputData.Add((byte)(value >> 8));
                Offset += ModifierLookup[B].Count;
                ++BitmaskIndex;
            }
            KeyData.AppendLine("; -------------------------------------");
            foreach (byte B in DifferentMasks) {
                KeyData.Append(".db ");
                bool First = true;
                foreach (Byte B2 in ModifierLookup[B]) {
                    if (First) {
                        First = false;
                    } else {
                        KeyData.Append(", ");
                    }
                    KeyData.Append(string.Format("%{0}", ToBinary(B2, 8)));
                    OutputData.Add(B2);
                }
                KeyData.AppendLine();
            }
            // --- //
            DataOffsets.Add((ushort)OutputData.Count);
            // --- //

            KeyData.AppendLine("; =====================================");
            foreach (PhysicalKey PK in StandardKeys) {
                KeyData.AppendLine(string.Format(".db ${0:X2} ; {1}", PK.ScanCode, PK.FriendlyName));
                OutputData.Add(PK.ScanCode);
            }
            KeyData.AppendLine("; -------------------------------------");

            // --- //
            DataOffsets.Add((ushort)OutputData.Count);
            // --- //

            foreach (PhysicalKey PK in ExtendedKeys) {
                KeyData.AppendLine(string.Format(".db ${0:X2} ; {1}", PK.ScanCode, PK.FriendlyName));
                OutputData.Add(PK.ScanCode);
            }
            KeyData.AppendLine("; =====================================");

            // --- //
            DataOffsets.Add((ushort)OutputData.Count);
            // --- //

            int TotalOffset = 0;

            List<int> Sizes = new List<int>();

            int TotalKeys = StandardKeys.Count + ExtendedKeys.Count;

            int KeyIndex = 0;

            for (int i = 0; i < 2; ++i) {
                foreach (PhysicalKey PK in new List<PhysicalKey>[] { StandardKeys, ExtendedKeys }[i]) {

                    int ExtendedInfo = 0;
                    if (PK.IsModifier) {
                        ExtendedInfo |= 8;
                        if (PK.IsToggle) ExtendedInfo |= 4;
                    }

                    KeyData.AppendLine(string.Format(".dw (%{1} << 12) | ${0:X4}", TotalOffset + ((TotalKeys - KeyIndex) * 2) - 1, ToBinary(ExtendedInfo, 4)));
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
                    ++KeyIndex;
                }
            }
            KeyData.AppendLine("; =====================================");

            // --- //
            DataOffsets.Add((ushort)OutputData.Count);
            // --- //

            List<bool> NonPrintableBits = new List<bool>();
            int k = 0;
            for (int i = 0; i < 2; ++i) {
                foreach (PhysicalKey PK in new List<PhysicalKey>[] { StandardKeys, ExtendedKeys }[i]) {
                    int Size = Sizes[k++];

                    if (PK.IsModifier) {
                        NonPrintableBits.Add(false);
                        KeyData.Append(string.Format(".db %{0}", ToBinary(1 << PK.ModifierIndex, 8)));
                        OutputData.Add((byte)(1 << PK.ModifierIndex));
                        if (PK.IsToggle) {
                            NonPrintableBits.Add(false);
                            KeyData.Append(string.Format(", %{0}", ToBinary(1 << PK.LedIndex, 8)));
                            OutputData.Add((byte)(1 << PK.LedIndex));
                        }
                        KeyData.AppendLine(" ; Modifier data for next key.");
                    }
                    NonPrintableBits.Add(false);
                    KeyData.Append(string.Format(".db %{0}", ToBinary(PK.ModifierMask, 8)));
                    OutputData.Add(PK.ModifierMask);

                    List<int> AddedModifications = new List<int>();
                    for (int m = 0; m < 256; ++m) {
                        int Modifier = m & PK.ModifierMask;
                        if (!AddedModifications.Contains(Modifier)) {
                            KeyData.Append(string.Format(", ${0:X2}", PK.Values[Modifier]));
                            OutputData.Add(PK.Values[Modifier]);
                            NonPrintableBits.Add(PK.IsNonPrintable[Modifier]);
                            AddedModifications.Add(Modifier);
                        }
                    }

                    KeyData.AppendLine(" ; " + PK.ToString());
                }
            }

            KeyData.AppendLine("; =====================================");

            // --- //
            DataOffsets.Add((ushort)OutputData.Count);
            // --- //

            byte[] NonPrintableBytes = new byte[(int)Math.Ceiling(NonPrintableBits.Count / 8m)];
            for (int i = 0; i < NonPrintableBits.Count; ++i) {
                if (NonPrintableBits[i]) {
                    NonPrintableBytes[i >> 3] |= (byte)(0x80 >> (i & 7));
                }
            }
            foreach (byte b in NonPrintableBytes) {
                KeyData.AppendLine(string.Format(".db %{0}", ToBinary(b, 8)));
                OutputData.Add(b);
            }


            //Console.WriteLine(DataOffsets.Count);

            string Offsets = ".dw ";
            for (int i = 0; i < DataOffsets.Count; ++i) {
                DataOffsets[i] += (ushort)(DataOffsets.Count * 2);
                if (i != 0) Offsets += ", ";
                Offsets += "$" + ((int)(DataOffsets[i])).ToString("X4");
            }

            //Clipboard.SetText(Offsets + "\r\n" + KeyData.ToString());

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

                        byte[] OutputBinary = new byte[OutputData.Count + DataOffsets.Count * 2 + 8];
                        for (int j = 0; j < 8; ++j) OutputBinary[j] = (byte)"EmerKey1"[j];
                        OutputData.CopyTo(OutputBinary, DataOffsets.Count * 2 + 8);
                        
                        for (int j = 0; j < DataOffsets.Count; ++j) {
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
                    SaveXml(OpenXmlDialog.FileName);
                } catch (Exception ex) {
                    MessageBox.Show(this, "Couldn't save file: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}