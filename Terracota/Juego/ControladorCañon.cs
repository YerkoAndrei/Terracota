using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using System.Collections;
using Stride.Core.Diagnostics;

namespace Terracota
{
    public class ControladorCañon : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override void Start()
        {
            // Initialization of the script.
            Log.Warning("a");
            Log.Debug("b");
            Console.WriteLine("console");

            //var imprimir = Imprimir();
            //imprimir.Start();
        }

        public override void Update()
        {
            // Do stuff every new frame
        }

        private async Task Imprimir()
        {
            await Task.Delay(1000);
            Console.WriteLine("tt");
        }
    }
}
