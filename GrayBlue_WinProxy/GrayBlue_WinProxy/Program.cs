using System;
using System.Threading.Tasks;

namespace GrayBlue_WinProxy {
    class Program {
        static void Main(string[] args) {
            GrayBlueUWPCore.Plugin.Instance.Scan().ContinueWith(x => {
                var lst = x.Result;
                Console.WriteLine(string.Join(",", lst));
            });
            Console.ReadKey();
        }
    }
}
