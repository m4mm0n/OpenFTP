using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils.ProgressReporter
{
    public interface IProgressReport<T>
    {
        void Report(T value);
        void Report(T value1, T value2);
    }
}
