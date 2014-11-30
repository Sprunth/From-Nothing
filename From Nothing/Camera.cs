using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Scene;

namespace From_Nothing
{
    class Camera : FNBase
    {
        private CameraSceneNode _cam;

        public Camera(FNGame game) : base(game)
        {
            _cam = _scene.AddCameraSceneNodeFPS(_scene.RootNode, 100f, .2f, -1);
            _cam.Position = new Vector3Df(-20, 20, -30);
            _cam.Target = new Vector3Df(0, 0, 0);

            game.OnKeyEvent += game_OnKeyEvent;
        }

        void game_OnKeyEvent(Event evnt)
        {
            switch (evnt.Key.Key)
            {
                case KeyCode.KeyK:
                {
                    if (!evnt.Key.PressedDown)
                        _cam.InputReceiverEnabled = !_cam.InputReceiverEnabled;
                    break;
                }
            }
        }

        public Vector3Df Intersection { get; private set; }
        public Triangle3Df HitTriangle { get; private set; }
        public SceneNode SelectedNode { get; private set; }

        private Line3Df _ray;

        public override void OnUpdate()
        {
            _ray.Start = _cam.Position;
            _ray.End = _ray.Start + (_cam.Target - _ray.Start).Normalize() * 1000;
            Vector3Df intersection;
            Triangle3Df hitTriangle;
            SelectedNode = _scene.SceneCollisionManager.GetSceneNodeAndCollisionPointFromRay(_ray,
                out intersection, out hitTriangle, 1, _scene.RootNode);
            Intersection = intersection;
            HitTriangle = hitTriangle;

            //Console.WriteLine(intersection);
        }
    }
}
