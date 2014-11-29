using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace From_Nothing
{
    class Program
    {
        public static FNGame ActiveGame { get; private set; }
        
        static void Main(string[] args)
        {
            ActiveGame = new FNGame();
            ActiveGame.Initialize();
            ActiveGame.Run();
        }
    }
}
