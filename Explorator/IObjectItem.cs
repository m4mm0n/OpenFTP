using System;
using System.Windows.Forms;

namespace Explorator
{
    public abstract class IObjectItem<T> : ListViewItem, IDisposable
    {
        public virtual bool IsDirectory => false;
        public virtual bool IsValid => false;
        public virtual T ItemInfo { get; set; }
        public virtual long ItemSize => 0L;

        public void Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
