using System;
using System.Collections.Generic;
using System.Text;

namespace Keyboard_Layout_Editor {
    class PhysicalKey : IComparable {

        public PhysicalKey() {
            for (int i = 0; i < 256; ++i) this.Values[i] = 0xFF;
        }

        public byte ScanCode = 0x00;
        public bool IsExtended = false;
        public string FriendlyName = "";

        public byte[] Values = new byte[256];
        public bool[] IsNonPrintable = new bool[256];

        public byte ModifierMask = 0;

        public byte LedIndex = 0xFF;

        public bool IsModifier = false;
        private int modifierIndex = 0;
        public int ModifierIndex {
            get {
                if (!IsModifier) throw new Exception("This key is not a modifier.");
                return modifierIndex;
            }
            set {
                if (!IsModifier) throw new Exception("This key is not a modifier.");
                if (value < 0 || value > 7) throw new IndexOutOfRangeException("Modifier range must be 0..7.");
                modifierIndex = value;
            }
        }

        private bool isToggle = false;
        public bool IsToggle {
            get {
                if (!IsModifier) throw new Exception("This key is not a modifier.");
                return isToggle;
            }
            set {
                if (!IsModifier) throw new Exception("This key is not a modifier.");
                isToggle = value;
            }
        }

        public override string ToString() {
            if (this.FriendlyName == "") {
                return this.ScanCode.ToString("X2");
            } else {
                return string.Format("{0} ({2}{1:X2})", this.FriendlyName, this.ScanCode, this.IsExtended ? "+" : "");
            }
        }

        public int CompareTo(object obj) {
            PhysicalKey o = (PhysicalKey)obj;
            string a = this.ToString();
            string b = o.ToString();            
            if (a.Length == b.Length) {
                return a.CompareTo(b);
            } else {
                return a.Length.CompareTo(b.Length);
            }
        }

     }
}
