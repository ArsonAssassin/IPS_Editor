using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IPS_Editor
{
    public static class Helpers
    {
        private static readonly Dictionary<byte, string> charTable = new Dictionary<byte, string>
    {
        {0x00, "{endstring}"}, {0x01, "{linebreak}"}, {0x02, "{bartz name}"}, {0x03, "crystal"}, {0x04, "tycoon"},
        {0x05, "{tabwithreturnsize}"}, {0x0B, "monster"}, {0x0C, "{pause}"}, {0x0D, "temple of wind"},
        {0x0E, "flying dragon"}, {0x0F, "{autoclose}"}, {0x10, "{currentgil}"}, {0x11, "{item}"}, {0x12, "{ability}"},
        {0x16, "{bartz name}"}, {0x17, "{pausewithreturntime}"},
        {0x20, "A"}, {0x21, "B"}, {0x22, "C"}, {0x23, "D"}, {0x24, "E"}, {0x25, "F"},
        {0x26, "G"}, {0x27, "H"}, {0x28, "I"}, {0x29, "J"}, {0x2A, "K"}, {0x2B, "L"},
        {0x2C, "M"}, {0x2D, "N"}, {0x2E, "O"}, {0x2F, "P"}, {0x30, "Q"}, {0x31, "R"},
        {0x32, "S"}, {0x33, "T"}, {0x34, "U"}, {0x35, "V"}, {0x36, "W"}, {0x37, "X"},
        {0x38, "Y"}, {0x39, "Z"}, {0x40, "di"}, {0x41, "di"}, {0x42, "du"}, {0x43, "du"},
        {0x44, "de"}, {0x45, "de"}, {0x46, "do"}, {0x47, "do"}, {0x48, "vu"}, {0x49, "pa"},
        {0x4A, "pa"}, {0x4B, "pi"}, {0x4C, "pi"}, {0x4D, "pu"}, {0x4E, "pu"}, {0x4F, "pe"},
        {0x50, "pe"}, {0x51, "po"}, {0x52, "po"},
        {0x53, "0"}, {0x54, "1"}, {0x55, "2"}, {0x56, "3"}, {0x57, "4"},
        {0x58, "5"}, {0x59, "6"}, {0x5A, "7"}, {0x5B, "8"}, {0x5C, "9"},
        {0x60, "A"}, {0x61, "B"}, {0x62, "C"}, {0x63, "D"}, {0x64, "E"}, {0x65, "F"},
        {0x66, "G"}, {0x67, "H"}, {0x68, "I"}, {0x69, "J"}, {0x6A, "K"}, {0x6B, "L"},
        {0x6C, "M"}, {0x6D, "N"}, {0x6E, "O"}, {0x6F, "P"}, {0x70, "Q"}, {0x71, "R"},
        {0x72, "S"}, {0x73, "T"}, {0x74, "U"}, {0x75, "V"}, {0x76, "W"}, {0x77, "X"},
        {0x78, "Y"}, {0x79, "Z"},
        {0x7A, "a"}, {0x7B, "b"}, {0x7C, "c"}, {0x7D, "d"}, {0x7E, "e"}, {0x7F, "f"},
        {0x80, "g"}, {0x81, "h"}, {0x82, "i"}, {0x83, "j"}, {0x84, "k"}, {0x85, "l"},
        {0x86, "m"}, {0x87, "n"}, {0x88, "o"}, {0x89, "p"}, {0x8A, "q"}, {0x8B, "r"},
        {0x8C, "s"}, {0x8D, "t"}, {0x8E, "u"}, {0x8F, "v"}, {0x90, "w"}, {0x91, "x"},
        {0x92, "y"}, {0x93, "z"}, {0x94, "il"}, {0x95, "al"}, {0x96, " "}, {0x97, "li"}, {0x98, "ll"}, {0x99, "'"},
        {0x9A, "\""}, {0x9B, ":"}, {0x9C, ";"}, {0x9D, ","}, {0x9E, "("}, {0x9F, ")"}, {0xA0, "/"},
        {0xA1, "!"}, {0xA2, "?"}, {0xA3, "."}, {0xA4, "ti"}, {0xA5, "it"}, {0xA6, "at"},
        {0xA7, "ta"}, {0xA8, "re"}, {0xA9, "ri"}, {0xAA, "'"}, {0xAB, "\""}, {0xAC, "re"}, {0xAD, "lt"}, {0xAE, "tl"}, {0xAF, "al"},
        {0xB0, "tt"}, {0xBB, "<Scrl>"}, {0xBC, "<Key>"}, {0xBD, "<Shoe>"}, {0xBE, "<Misc>"}, {0xBF, "<Hamr>"}, {0xC0, "<Tent>"},
        {0xC1, "<Ribn>"}, {0xC2, "<Drnk>"}, {0xC3, "<Suit>"}, {0xC4, "<Song>"}, {0xC5, "-"}, {0xC9, "!"}, {0xCD, "%"}, {0xCE, "/"}, {0xCF, ":"},
        {0xD2, "."}, {0xD3, "A"}, {0xD4, "B"}, {0xD5, "X"}, {0xD6, "Y"}, {0xD7, "L"}, {0xD8, "R"}, {0xE2, "+"},
        {0xE3, "Swrd"}, {0xE4, "BlkMgc"}, {0xE5, "WhtMgc"}, {0xE6, "TimeMgc"}, {0xE7, "Knf"}, {0xE8, "Sper"}, {0xE9, "Axe"}, {0xEA, "Katn"}, {0xEB, "Rod"},
        {0xEC, "Stff"}, {0xED, "Bow"}, {0xEE, "Harp"}, {0xEF, "Whip"}, {0xF0, "Bell"}, {0xF1, "Shld"}, {0xF2, "Helm"}, {0xF3, "Armr"}, {0xF4, "Ring"},
        {0xFF, " "}
    };

        public static string TranslateFromHex(byte[] data)
        {
            StringBuilder result = new StringBuilder();

            foreach (byte b in data)
            {
                if (charTable.TryGetValue(b, out string value))
                {
                    result.Append(value);
                }
                else if (b >= 0x20 && b <= 0x7E) // Printable ASCII range
                {
                    result.Append((char)b);
                }
                else
                {
                    result.Append($"<{b:X2}>");
                }
            }

            return result.ToString();
        }
    }
}
