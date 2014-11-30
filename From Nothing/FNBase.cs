using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrlichtLime.GUI;
using IrrlichtLime.Scene;
using IrrlichtLime.Video;

namespace From_Nothing
{
    /// <summary>
    /// To avoid a singleton, every class can derive this class
    /// and get private variables pointing to the nessisary objects
    /// </summary>
    abstract class FNBase
    {
        protected readonly VideoDriver _driver;
        protected readonly SceneManager _scene;
        protected readonly GUIEnvironment _gui;

        public FNBase(FNGame game)
        {
            _driver = game.Driver;
            _scene = game.Scene;
            _gui = game.Gui;

            game.OnUpdate += OnUpdate;
        }

        public abstract void OnUpdate();
    }
}
