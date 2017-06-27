using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Template_P3
{
    class SceneGraph
    {
        Shader shader;
        public Mesh car, tireRA, tireRF, tireLA, tireLF;	//car parts that need to be specified
        Texture wood;                           // texture to use for rendering
        public List<Mesh> lijst = new List<Mesh>(); //Datastructure for the meshes
        const float PI = 3.1415926535f;
        public float rotation = PI;            //initial rotation of the car
        public Matrix4 view, projection;
        

        public SceneGraph()
        {
            view =  Matrix4.Identity;
            projection = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            wood = new Texture("../../assets/wood.jpg");

            //Load different textures for car parts
            Texture tire = new Texture("../../assets/car/tire.jpg");
            Texture frame = new Texture("../../assets/car/black.png");
            Texture metal = new Texture("../../assets/car/metal.jpg");
            Texture chair = new Texture("../../assets/car/chair.jpg");
            Texture feuxl = new Texture("../../assets/car/feuxl.jpg");

            // load Car meshes
            car = new Mesh("../../assets/car/frame.obj", Matrix4.CreateRotationY(rotation), frame);
            tireRA = new Mesh("../../assets/car/test.obj", Matrix4.CreateRotationY(rotation), car, -2.6f, -1.8f, -4f, tire);
            tireRF = new Mesh("../../assets/car/test.obj", Matrix4.CreateRotationY(rotation),car,-2.6f, -1.8f, 4.6f, tire);
            tireLA = new Mesh("../../assets/car/rechts.obj", Matrix4.CreateRotationY(rotation) ,car ,2.6f, -1.8f, 4.6f, tire);
            tireLF = new Mesh("../../assets/car/rechts.obj", Matrix4.CreateRotationY(rotation), car, 2.6f, -1.8f, -4f, tire);
           

            //load and add Teapot meshes
            lijst.Add(new Mesh("../../assets/teapot.obj", Matrix4.CreateTranslation(-16,0,-15) , wood));
            lijst.Add(new Mesh("../../assets/teapot.obj", Matrix4.CreateTranslation(16,0,0), lijst[0], wood));
            //add car parts
            lijst.Add(car);
            lijst.Add(tireRA);
            lijst.Add(tireRF);
            lijst.Add(tireLA);
            lijst.Add(tireLF);
            lijst.Add(new Mesh("../../assets/car/back lights.obj", Matrix4.Identity, car, feuxl));
            lijst.Add(new Mesh("../../assets/car/metal parts.obj", Matrix4.Identity, car, metal));
            lijst.Add(new Mesh("../../assets/car/seats.obj", Matrix4.Identity, car, chair));
            lijst.Add(new Mesh("../../assets/car/steering wheel.obj", Matrix4.Identity, car, wood));

        }

        public void render (bool car)
        {
            foreach(Mesh mesh in lijst)
            {
               mesh.transform = mesh.modelmatrix * view;
                mesh.MV = mesh.transform;
                mesh.transform *= projection;
                mesh.Render(shader);
            }
        }

        public void update()
        {
            foreach(Mesh mesh in lijst)
            {
                mesh.update();
            }
        }

    }
}
