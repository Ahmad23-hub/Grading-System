using System;
using System.Collections.Generic;
using System.Text;

namespace library_project
{
    internal class IDigitals
    {
        public interface IDigital
        {
            long FileSizeMb { get; }
            string DownloadUrl { get; }
        }
    }
}
