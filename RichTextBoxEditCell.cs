using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPS_Editor
{
    public class RichTextBoxCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(RichTextBoxEditingControl);
        public override Type ValueType => typeof(string);
        public override object DefaultNewRowValue => string.Empty;
    }
    public class RichTextBoxEditingControl : RichTextBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        public RichTextBoxEditingControl()
        {
            this.Multiline = true;
        }

        public object EditingControlFormattedValue
        {
            get => this.Text;
            set
            {
                if (value is string)
                {
                    this.Text = (string)value;
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.Text;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle cellStyle)
        {
            this.Font = cellStyle.Font;
            this.ForeColor = cellStyle.ForeColor;
            this.BackColor = cellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                this.SelectAll();
            }
        }

        public bool RepositionEditingControlOnValueChange => false;

        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }

        public Cursor EditingPanelCursor => base.Cursor;

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            valueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }
    }
}
