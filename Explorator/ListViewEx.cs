using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Explorator
{
    public class ListViewEx : ListView
    {
        #region Private

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wPar, IntPtr lPar);

        const int LVM_FIRST = 0x1000;
        const int LVM_GETCOLUMNORDERARRAY = (LVM_FIRST + 59);

        const int WM_PAINT = 0x000F;


        private struct InternalControl
        {
            public Control Control;
            public int Column;
            public int Row;
            public DockStyle DockStyle;
            public ListViewItem Item;
        }

        private ArrayList _internalControls = new ArrayList();

        #endregion

        #region Public

        public ListViewEx()
        { }

        public void AddControl(Control c, int col, int row)
        {
            AddControl(c, col, row, DockStyle.Fill);
        }

        public void AddControl(Control c, ListViewItem ownerItem)
        {
            AddControl(c, ownerItem, DockStyle.Fill);
        }

        public void AddControl(Control c, ListViewItem ownerItem, DockStyle dock)
        {
            if (c == null)
                throw new ArgumentNullException("Control cannot be null!");
            if(ownerItem == null)
                throw new ArgumentNullException("ListViewItem cannot be null!");
            if (this.Items.Count == 0)
                throw new IndexOutOfRangeException("ListViewEx has no items!");
            if(!this.Items.Contains(ownerItem))
                throw new InvalidDataException("ListViewItem not found in the item's list!");

            var nc = new InternalControl()
            {
                Control = c,
                Column = 1,
                DockStyle = dock,
                Item = ownerItem,
                Row = this.Items.IndexOf(ownerItem)
            };

            foreach (InternalControl ic in _internalControls)
            {
                if (ic.Control == c)
                    throw new Exception("Control already exists!");
                if (ic.Control.Name == c.Name)
                    throw new Exception("Control with that name already exists!");
            }

            _internalControls.Add(nc);
            this.Controls.Add(c);
        }

        public void AddControl(Control c, int col, int row, DockStyle dock)
        {
            if (c == null)
                throw new ArgumentNullException("Control cannot be null!");
            if (col >= this.Columns.Count)
                throw new IndexOutOfRangeException("Column is out of range!");
            if (row >= this.Items.Count)
                throw new IndexOutOfRangeException("Row is out of range!");

            var nc = new InternalControl()
            {
                Control = c,
                Column = col,
                DockStyle = dock,
                Row = row,
                Item = this.Items[row]
            };

            foreach (InternalControl ic in _internalControls)
            {
                if(ic.Control == c)
                    throw new Exception("Control already exists!");
                if(ic.Control.Name == c.Name)
                    throw new Exception("Control with that name already exists!");
            }

            _internalControls.Add(nc);
            this.Controls.Add(c);
        }

        public void RemoveControl(Control c)
        {
            if (c == null)
                throw new ArgumentNullException("Control cannot be null!");
            for (var i = 0; i < _internalControls.Count; i++)
            {
                var nc = (InternalControl)_internalControls[i];
                if (nc.Control == c)
                {
                    this.Controls.Remove(c);
                    _internalControls.RemoveAt(i);
                    return;
                }
            }
            throw new ArgumentOutOfRangeException("Control not found!");
        }

        public Control GetAddedControl(ListViewItem item)
        {
            foreach (InternalControl ic in _internalControls)
            {
                if (ic.Item == item)
                    return ic.Control;
            }

            throw new ArgumentOutOfRangeException("Control not found!");
        }

        public Control GetAddedControl(int col, int row)
        {
            foreach (InternalControl control in _internalControls)
            {
                if (control.Column == col && control.Row == row)
                    return control.Control;
            }

            throw new ArgumentOutOfRangeException("Control not found!");
        }

        public Control GetAddedControl(string name)
        {
            foreach (InternalControl ic in _internalControls)
            {
                if (ic.Control.Name == name)
                    return ic.Control;
            }

            throw new ArgumentOutOfRangeException("Control not found!");
        }

        public new View View
        {
            get => base.View;
            set
            {
                foreach (InternalControl control in _internalControls)
                    control.Control.Visible = (value == View.Details);
                base.View = value;
            }
        }

        #endregion

        #region Protected

        protected override void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
        {
            this.Refresh();

            base.OnColumnWidthChanged(e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                    if (this.View != View.Details)
                        break;
                    foreach (InternalControl ic in _internalControls)
                    {
                        var rect = GetSubItemBounds(ic.Item, ic.Column);
                        if (this.HeaderStyle != ColumnHeaderStyle.None && rect.Top < this.FontHeight)
                        {
                            ic.Control.Visible = false;
                            continue;
                        }
                        else
                            ic.Control.Visible = true;

                        switch (ic.DockStyle)
                        {
                            case DockStyle.Fill:
                                break;
                            case DockStyle.Top:
                                rect.Height = ic.Control.Height;
                                break;
                            case DockStyle.Bottom:
                                rect.Offset(0, rect.Height - ic.Control.Height);
                                rect.Height = ic.Control.Height;
                                break;
                            case DockStyle.Right:
                                rect.Offset(rect.Width - ic.Control.Width, 0);
                                rect.Width = ic.Control.Width;
                                break;
                            case DockStyle.Left:
                                rect.Width = ic.Control.Width;
                                break;
                            case DockStyle.None:
                                rect.Size = ic.Control.Size;
                                break;
                        }

                        ic.Control.Bounds = rect;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        protected int[] GetColumnOrder()
        {
            var lParam = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);
            if (SendMessage(Handle, LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lParam) ==
                IntPtr.Zero) return null;
            var order = new int[Columns.Count];
            Marshal.Copy(lParam, order, 0, Columns.Count);
            Marshal.FreeHGlobal(lParam);
            return order;
        }

        protected Rectangle GetSubItemBounds(ListViewItem Item, int SubItem)
        {
            var sItemRect = Rectangle.Empty;
            if (Item == null)
                throw new NullReferenceException("Item");
            var order = GetColumnOrder();
            if (order == null)
                return sItemRect;
            if (SubItem >= order.Length)
                throw new IndexOutOfRangeException("SubItem " + SubItem + " out of range");

            var itemBounds = Item.GetBounds(ItemBoundsPortion.Entire);
            var subX = itemBounds.Left;

            var i = 0;
            for (i = 0; i < order.Length; i++)
            {
                var tmpCol = this.Columns[order[i]];
                if (tmpCol.Index == SubItem)
                    break;
                subX += tmpCol.Width;
            }

            sItemRect = new Rectangle(subX, itemBounds.Top, this.Columns[order[i]].Width, itemBounds.Height);
            return sItemRect;
        }

        #endregion
    }
}
