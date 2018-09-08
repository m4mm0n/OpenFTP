using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Utils.CRC;

namespace Explorator
{
    public class QueueReadyItem : ListViewItem
    {
        public QueueReadyItem(string name, bool showProgressBar, string currentSpeed, long totalSize,
            string itemSource, string itemDestination, ListViewEx owner)
        {
            this.Text = name;
            if (showProgressBar)
                AddProgressBar(owner, name, totalSize);
            else
                this.SubItems.Add("Queued");
            this.SubItems.Add(currentSpeed);
            this.SubItems.Add(Utils.GetFileSize(totalSize));
            this.SubItems.Add(itemSource);
            this.SubItems.Add(itemDestination);
        }



        private void AddProgressBar(ListViewEx _owner, string name, long fileSize)
        {
            var p = new PercentageBar()
            {
                AutomaticallyFormatPercentageToBytes = true,
                ShowPercentage = true,
                MaximumBytes = fileSize,
                Maximum = fileSize,
                Minimum = 0,
                CurrentBytes = 0,
                Name = CRC32
                    .QuickCompute(
                        Encoding.ASCII.GetBytes(name + fileSize.ToString("X") + DateTime.Now.Ticks.ToString()),
                        SlicingType.SliceBy8).ToString("X8")
            };
            _owner.AddControl(p, this);
        }
    }
}
