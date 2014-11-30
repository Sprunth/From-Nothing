using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.GUI;
using IrrlichtLime.IO;
using IrrlichtLime.Scene;
using IrrlichtLime.Video;
using Noise;
using Noise.Modules;
using Noise.Utils;
using Color = IrrlichtLime.Video.Color;

namespace From_Nothing
{
    class FNGame
    {
        private IrrlichtDevice device;
        public VideoDriver Driver { get; set; }
        public SceneManager Scene { get; set; }
        public GUIEnvironment Gui { get; set; }

        private AnimatedMeshSceneNode sphere;
        private Camera _cam;

        private TriangleSelector selector;
        private MeshSceneNode selectionCube;

        public delegate void MouseEventHandler(Event evnt);
        public event MouseEventHandler OnMouseEvent;

        public delegate void KeyEventHandler(Event evnt);
        public event KeyEventHandler OnKeyEvent;

        public delegate void UpdateHandler();
        public event UpdateHandler OnUpdate;

        public FNGame()
        {
            var param = new IrrlichtCreationParameters(){
                AntiAliasing = 8,
                DeviceType = DeviceType.Best,
                DriverType = DriverType.OpenGL,
                VSync = true,
                WindowSize = new Dimension2Di(1200, 700)
            };
            device = IrrlichtDevice.CreateDevice(param);

            device.OnEvent += device_OnEvent;

            Driver = device.VideoDriver;
            Scene = device.SceneManager;
            Gui = device.GUIEnvironment;
        }

        bool device_OnEvent(Event evnt)
        {
            switch (evnt.Type)
            {
                case EventType.Key:
                {
                    if (OnKeyEvent != null)
                        OnKeyEvent(evnt);

                    switch (evnt.Key.Key)
                    {
                        case KeyCode.Esc:
                        {
                            device.Close();
                            Environment.Exit(0);
                            break;
                        }
                    }
                    break;
                }
                case EventType.Mouse:
                {
                    if (OnMouseEvent != null)
                        OnMouseEvent(evnt);
                    break;
                }
            }
            return false;
        }

        public void Initialize()
        {
            //var light = Scene.AddLightSceneNode(Scene.RootNode,new Vector3Df(2, 6, 3), new Colorf(200, 200, 180), 20);
            var light = Scene.AddLightSceneNode(null, new Vector3Df(-30, 35, -25));
            light.Radius = 25;
            light.CastShadows = true;
            light.LightType = LightType.Point;

            var sun = Scene.AddLightSceneNode(null, new Vector3Df(40, 100, 40));
            sun.Radius = 50;
            sun.CastShadows = true;
            sun.LightType = LightType.Point;
            

            sphere = Scene.AddAnimatedMeshSceneNode(Scene.AddSphereMesh("sphere", 10f));
            sphere.Scale = new Vector3Df(1);
            sphere.SetMaterialFlag(MaterialFlag.Lighting, true);
            sphere.Position = new Vector3Df(1,11,1);
            //sphere.SetMaterialTexture(0, Driver.GetTexture("Mars.jpg"));
            sphere.SetMaterialTexture(0, GeneratePlanetTexture(new Vector2Dd(300, 300)));
            Scene.MeshManipulator.SetVertexColors(sphere.Mesh, new Color(200, 200, 200));

            var terrain =
                Scene.AddAnimatedMeshSceneNode(Scene.AddHillPlaneMesh("terrain", new Dimension2Df(1, 1),
                    new Dimension2Di(128,128),
                    new Material(), 0));
            terrain.Position = new Vector3Df(0,0,0);
            terrain.SetMaterialFlag(MaterialFlag.Lighting, true);
            Scene.MeshManipulator.SetVertexColors(terrain.Mesh, new Color(220, 220, 220));
            selector = Scene.CreateOctreeTriangleSelector(terrain.Mesh, terrain, 128);
            terrain.TriangleSelector = selector;
            Scene.CreateCollisionResponseAnimator(selector, terrain);

            _cam = new Camera(this);

            selectionCube = Scene.AddCubeSceneNode(10, null, -1);
            selectionCube.SetMaterialFlag(MaterialFlag.Wireframe, true);
            
        }

        public void Run()
        {
            while (device.Run())
            {
                if (OnUpdate != null)
                    OnUpdate();

                sphere.Rotation += new Vector3Df(0, 1, 0);

                device.SetWindowCaption(Driver.FPS.ToString());

                //collision
                
                selectionCube.Position = _cam.Intersection;


                Driver.BeginScene(true, true, new Color(110, 60, 50));

                Scene.DrawAll();
                Gui.DrawAll();

                Driver.EndScene();

                device.Yield();
            }
        }

        private Perlin module;
        private PlanarNoiseMapBuilder heightMapBuilder;
        private NoiseMap heightMap;
        private Random random = new Random();

        private Texture GeneratePlanetTexture(Vector2Dd texSize)
        {
            var imgSize = texSize;
            module = new Perlin(4, 0.5, NoiseQuality.Best, 5, 1.3, random.Next(0, 1024));
            heightMapBuilder = new PlanarNoiseMapBuilder((uint)imgSize.X, (uint)imgSize.Y, 0, module, 2, 6, 1, 5, true);
            heightMap = heightMapBuilder.Build();

            var texColors = new GradientColour();
            texColors.AddGradientPoint(-1, System.Drawing.Color.DimGray);
            texColors.AddGradientPoint(-0.7, System.Drawing.Color.Olive);
            texColors.AddGradientPoint(0.0, System.Drawing.Color.SteelBlue);
            texColors.AddGradientPoint(0.8, System.Drawing.Color.Tan);
            texColors.AddGradientPoint(1, System.Drawing.Color.DarkKhaki);
            var renderer = new ImageBuilder(heightMap, texColors);
            System.Drawing.Image renderedImg = renderer.Render();
            var img = new Bitmap(renderedImg);
            img.Save("planetTex.png", ImageFormat.Png);

            var returnTex = Driver.GetTexture("planetTex.png");
            return returnTex;
        }
    }
}
