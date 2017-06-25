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
        List<Mesh> lijst = new List<Mesh>();

        public SceneGraph()
        {
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            wood = new Texture("../../assets/wood.jpg");
            // load teapot
             lijst.Add(new Mesh("../../assets/teapot.obj", -10,-4,-15));
            lijst.Add(new Mesh("../../assets/floor.obj",10, 4,-15, lijst[0]));
            lijst.Add(new Mesh("../../assets/teapot.obj", 0, 0, -15, lijst[1]));
        }

        public void render (Matrix4 camera)
        {
            foreach(Mesh mesh in lijst)
            {
                mesh.Render(shader, camera, wood);
            }
            // render scene to render target

           // camera *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            //mesh.Render(shader, camera, MV, wood);

        }

    }
}
