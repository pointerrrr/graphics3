﻿using System;
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
        public Matrix4 view, projection;
        

        public SceneGraph()
        {
            view =  Matrix4.Identity;
            projection = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            wood = new Texture("../../assets/wood.jpg");
            // load teapot
            // lijst.Add(new Mesh("../../assets/teapot.obj",Matrix4.CreateTranslation(-16,0,-15), wood));
           // lijst.Add(new Mesh("../../assets/teapot.obj", Matrix4.CreateTranslation(16,0,0), lijst[0], wood));
           // lijst.Add(new Mesh("../../assets/teapot.obj", Matrix4.CreateTranslation(16,0,0),lijst[1], wood));
           // lijst.Add(new Mesh("../../assets/summoner_rift.obj", Matrix4.CreateTranslation(0,-18,0)));


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
