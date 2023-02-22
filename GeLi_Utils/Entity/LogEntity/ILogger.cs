using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeLi_Utils.Entity
{
    public interface ILogger
    {
        void Trace(string message, params string[] args);
        void Debug(string message, params string[] args);
        void Info(string message, params string[] args);
        void Warn(string message, params string[] args);
        void Error(string message, params string[] args);
    }
}
