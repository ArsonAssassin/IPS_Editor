using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPS_Editor
{
    public partial class RowEditForm : Form
    {
        public event EventHandler<IPSEntrySavedArgs> RowSaved;
        public IPSEntry Entry;
        public RowEditForm(IPSEntry entry)
        {
            InitializeComponent();
            Entry = entry;

            textBox1.Text = Entry.Offset.ToString();
            textBox2.Text = Entry.Size.ToString();
            textBox3.Text = Entry.HexPreview;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var offset = int.Parse(textBox1.Text);
            Entry.Offset = offset;
            Entry.UpdateFromHex(textBox3.Text.Replace("-", " "));
            textBox2.Text = Entry.Size.ToString();

            IPSEntrySavedArgs args = new IPSEntrySavedArgs() { Data = Entry };
            RowSaved?.Invoke(this, args);
        }
    }
}
