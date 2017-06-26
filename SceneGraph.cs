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
        Mesh mesh, floor;						// a mesh to draw using OpenGL
        Texture wood;                           // texture to use for rendering
        public List<Mesh> lijst = new List<Mesh>();
        const float PI = 3.1415926535f;
        public Matrix4 view, projection;
        

        public SceneGraph()
        {
            view =  Matrix4.Identity;
            projection = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            wood = new Texture("../../assets/wood.jpg");
            Texture tire = new Texture("../../assets/car/tire.jpg");
            Texture frame = new Texture("../../assets/car/black.png");
            Texture metal = new Texture("../../assets/car/metal.jpg");
            Texture chair = new Texture("../../assets/car/chair.jpg");
            Texture feuxl = new Texture("../../assets/car/feuxl.jpg");
            // load teapot
            Mesh car = new Mesh("../../assets/car/frame.obj", /*Matrix4.Identity*/ Matrix4.CreateRotationY(PI), frame);
            lijst.Add(new Mesh("../../assets/teapot.obj", Matrix4.CreateTranslation(-16,0,-15) , wood));
            lijst.Add(new Mesh("../../assets/teapot.obj", Matrix4.CreateTranslation(16,0,0), lijst[0], wood));
            lijst.Add(car);
            
            

            lijst.Add(new Mesh("../../assets/car/back lights.obj", Matrix4.Identity, car, feuxl));
            lijst.Add(new Mesh("../../assets/car/metal parts.obj", Matrix4.Identity, car, metal));
            lijst.Add(new Mesh("../../assets/car/seats.obj", Matrix4.Identity, car, chair));
            lijst.Add(new Mesh("../../assets/car/steering wheel.obj", Matrix4.Identity, car, wood));
            lijst.Add(new Mesh("../../assets/car/wheel.obj",  Matrix4.CreateTranslation(0, 0, 0.66f)*Matrix4.CreateRotationY((float)Math.PI) , car, tire));
            lijst.Add(new Mesh("../../assets/car/wheel.obj",  Matrix4.CreateTranslation(0, 0, 9.25f)*Matrix4.CreateRotationY((float)Math.PI) , car, tire));
            lijst.Add(new Mesh("../../assets/car/wheel.obj", Matrix4.Identity, car, tire));
            lijst.Add(new Mesh("../../assets/car/wheel.obj", Matrix4.CreateTranslation(0, 0, -8.5f), car, tire));

        }

        public void render (Matrix4 camera)
        {
            foreach(Mesh mesh in lijst)
            {
               // mesh.modelmatrix = view * mesh.modelmatrix;
                mesh.transform = mesh.modelmatrix * view;
                mesh.MV = mesh.modelmatrix;
                mesh.transform *= projection;
                mesh.Render(shader);
            }
            // render scene to render target

           // camera *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            //mesh.Render(shader, camera, MV, wood);

        }

    }
}
