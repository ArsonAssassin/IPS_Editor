namespace IPS_Editor
{
    public partial class Form1 : Form
    {
        private List<IPSEntry> ipsEntries = new List<IPSEntry>();
        private BindingSource bindingSource = new BindingSource();
        private bool isScrolling = false;
        public Form1()
        {
            InitializeComponent();
            SetupDataGridViews();
            SetupContextMenu();
            dataGridView1.Scroll += DataGridView_Scroll;
            dataGridView2.Scroll += DataGridView_Scroll;
        }
        private void SetupContextMenu()
        {
            contextMenuStrip1 = new ContextMenuStrip();
            // Add menu items
            //contextMenuStrip1.Items.Add("Copy", null, CopyMenuItem_Click);
            //contextMenuStrip1.Items.Add("Paste", null, PasteMenuItem_Click);
            //contextMenuStrip1.Items.Add("Edit", null, EditMenuItem_Click);
            contextMenuStrip1.Items.Add("Delete", null, DeleteMenuItem_Click);
            contextMenuStrip1.Items.Add("Add Row After", null, AddRowMenuItem_Click);

            // Assign the context menu to the DataGridView
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            dataGridView2.ContextMenuStrip = contextMenuStrip1;

            // Handle the Opening event to customize menu items based on the clicked cell
            // contextMenuStrip1.Opening += CellContextMenu_Opening;
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                int rowIndex = dataGridView1.CurrentCell.RowIndex;

                if (rowIndex >= 0 && rowIndex < ipsEntries.Count)
                {
                    ipsEntries.RemoveAt(rowIndex);

                    // Refresh both DataGridViews
                    dataGridView1.Refresh();
                    dataGridView2.Refresh();
                }
            }
        }
        private void AddRowMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                var clickedEntry = ipsEntries[rowIndex];

                var newEntry = new IPSEntry(clickedEntry.Offset + 1, new byte[] { });

                ipsEntries.Insert(rowIndex + 1, newEntry);

                // Refresh both DataGridViews
                dataGridView1.Refresh();
                dataGridView2.Refresh();

            }
        }
        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if (isScrolling) return;

            isScrolling = true;

            DataGridView scrolledGrid = (DataGridView)sender;
            DataGridView otherGrid = (scrolledGrid == dataGridView1) ? dataGridView2 : dataGridView1;

            // Synchronize vertical scrolling
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                otherGrid.FirstDisplayedScrollingRowIndex = scrolledGrid.FirstDisplayedScrollingRowIndex;
            }

            // Synchronize horizontal scrolling
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                otherGrid.HorizontalScrollingOffset = scrolledGrid.HorizontalScrollingOffset;
            }

            isScrolling = false;
        }
        private void SetupDataGridViews()
        {
            bindingSource = new BindingSource();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;

            // Enable editing
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;
            bindingSource.DataSource = ipsEntries;
            dataGridView1.DataSource = bindingSource;
            dataGridView2.DataSource = bindingSource;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Offset",
                DataPropertyName = "OffsetHex",
                HeaderText = "Offset",
                ReadOnly = false
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Size",
                DataPropertyName = "Size",
                HeaderText = "Size",
                Width = 40,
                ReadOnly = false
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HexValues",
                DataPropertyName = "HexPreview",
                HeaderText = "Hex Values",
                ReadOnly = false
            });

            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Offset",
                DataPropertyName = "OffsetHex",
                HeaderText = "Offset",
                ReadOnly = false
            });
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Size",
                DataPropertyName = "Size",
                HeaderText = "Size",
                Width = 40,
                ReadOnly = false
            });
            dataGridView2.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TranslatedText",
                DataPropertyName = "TranslatedPreview",
                HeaderText = "Translation",
                Width = 350,
                ReadOnly = true
            });

            bindingSource.DataError += BindingSource_DataError;



            //bindingSource.DataSource = ipsEntries;
            //dataGridView1.DataSource = bindingSource;
            //dataGridView2.DataSource = bindingSource;
        }

        private void BindingSource_DataError(object sender, BindingManagerDataErrorEventArgs e)
        {
            MessageBox.Show($"Data error: {e.Exception.Message}");
        }
        private void openIPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "IPS files (*.ips)|*.ips";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadIPSFile(openFileDialog.FileName);
                    bindingSource.ResetBindings(false);
                    if (ipsEntries.Count > 0)
                    {
                        dataGridView1.Rows[0].Selected = true;
                    }
                }
            }
        }

        private void LoadIPSFile(string filePath)
        {
            ipsEntries.Clear();
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                string header = new string(reader.ReadChars(5));
                if (header != "PATCH")
                {
                    MessageBox.Show("Invalid IPS file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                while (reader.BaseStream.Position < reader.BaseStream.Length - 3)
                {
                    int offset = (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte();
                    int size = (reader.ReadByte() << 8) | reader.ReadByte();

                    if (offset == 0x454F46) // "EOF"
                        break;

                    byte[] data = reader.ReadBytes(size);
                    ipsEntries.Add(new IPSEntry(offset, data));
                }
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Open Preview
            var content = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            Form2 form = new Form2(content);
            form.Show();
        }

        private void exportIPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "IPS files (*.ips)|*.ips";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveIPSFile(saveFileDialog.FileName);
                }
            }
        }
        private void SaveIPSFile(string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                writer.Write("PATCH".ToCharArray());

                foreach (IPSEntry entry in ipsEntries)
                {
                    writer.Write((byte)((entry.Offset >> 16) & 0xFF));
                    writer.Write((byte)((entry.Offset >> 8) & 0xFF));
                    writer.Write((byte)(entry.Offset & 0xFF));

                    writer.Write((byte)((entry.Data.Length >> 8) & 0xFF));
                    writer.Write((byte)(entry.Data.Length & 0xFF));

                    writer.Write(entry.Data);
                }

                writer.Write("EOF".ToCharArray());
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var data = ipsEntries[e.RowIndex];
            using (RowEditForm form = new RowEditForm(data))
            {
                form.RowSaved += (s, args) => Form_RowSaved(s, args, e.RowIndex);
                form.ShowDialog();
            }
        }

        private void Form_RowSaved(object? sender, IPSEntrySavedArgs e, int rowIndex)
        {
            ipsEntries[rowIndex] = e.Data;
        }
    }
}
