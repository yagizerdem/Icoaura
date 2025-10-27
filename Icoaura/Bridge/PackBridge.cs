using Icoaura.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Icoaura.Bridge
{

    [System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class PackBridge
    {
        private readonly PackController _packController;
        public PackBridge(PackController packController)
        {
            _packController = packController;
        }

        public void GetAllPackConfigs()
        {
            
        }
         
    }
}
