using System;
using System.Collections.Generic;
using System.Text;

namespace XMiniApp.DependencyServices
{
    public interface IDeviceLinkedService
    {
        string GetDeviceID();
        void LongAlert(string message);

        void ShortAlert(string message);
    }
}
