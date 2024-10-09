using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPS_Editor
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public class IPSEntry : INotifyPropertyChanged
    {
        private int _offset;
        private byte[] _data;

        public int Offset
        {
            get => _offset;
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    OnPropertyChanged(nameof(Offset));
                    OnPropertyChanged(nameof(OffsetHex));
                }
            }
        }

        public byte[] Data
        {
            get => _data;
            set
            {
                if (_data != value)
                {
                    _data = value;
                    OnPropertyChanged(nameof(Data));
                    OnPropertyChanged(nameof(Size));
                    OnPropertyChanged(nameof(HexPreview));
                    OnPropertyChanged(nameof(TranslatedPreview));
                }
            }
        }

        public string OffsetHex => $"0x{Offset:X6}";
        public int Size => Data.Length;
        public string HexPreview 
        {
            get 
            {
                return BitConverter.ToString(Data, 0, Data.Length);
            }
            set
            {
                string[] hexValues = value.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                List<byte> bytes = new List<byte>();
                foreach (string hex in hexValues)
                {
                    if (byte.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out byte result))
                    {
                        bytes.Add(result);
                    }
                    else
                    {
                        throw new FormatException($"Invalid hex value: {hex}");
                    }
                }
                Data = bytes.ToArray();
            }
        } 
        public string TranslatedPreview => Helpers.TranslateFromHex(Data);

        public IPSEntry(int offset, byte[] data)
        {
            Offset = offset;
            Data = data;
        }

        public void UpdateFromHex(string hexString)
        {
            Data = hexString.Split(' ').Select(s => Convert.ToByte(s, 16)).ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
