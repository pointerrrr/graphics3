using System.Collections.Generic;
using OpenTK;

namespace Template_P3
{
    class SceneGraph
    {
        const float PI = 3.1415926535f;

        public Matrix4 view, projection;
        public static List<Light> lights = new List<Light>();
        public List<Mesh> lijst = new List<Mesh>();
        public Mesh car;						// a mesh to draw using OpenGL
        public float rotation = PI / 2;
        public bool brake = false;

        private Shader shader;                  // the shader
        private Texture wood;                   // texture to use for rendering
        private Mesh[] wheels = new Mesh[4];    // wheels of the car
        private float rotation_of_car;

        public SceneGraph()
        {
            view =  Matrix4.Identity;
            projection = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            // load the textures
            wood = new Texture("../../assets/wood.jpg");
            Texture tire = new Texture("../../assets/car/tire.jpg");
            Texture frame = new Texture("../../assets/car/black.png");
            Texture metal = new Texture("../../assets/car/metal.jpg");
            Texture chair = new Texture("../../assets/car/chair.jpg");
            Texture feuxl = new Texture("../../assets/car/feuxl.jpg");
            Texture skybox = new Texture("../../assets/wall.jpg");
            
            // start adding meshes to the scenegraph
            lijst.Add(new Mesh("../../assets/teapot.obj", Matrix4.CreateTranslation(-16,0,-15) , wood, 100f, 0.5f));
            lijst.Add(new Mesh("../../assets/teapot.obj", Matrix4.CreateTranslation(16,0,0), lijst[0], wood, 100f, 0.7f));
            lijst.Add(new Mesh("../../assets/box.obj",  Matrix4.CreateTranslation(-5, -3, -17), skybox, 1000f, 0.9f));
            car = new Mesh("../../assets/car/frame.obj", Matrix4.CreateRotationY(PI), frame, 100f, 0.5f);
            lijst.Add(car);
            lijst.Add(new Mesh("../../assets/car/back lights.obj", Matrix4.Identity, car, feuxl));
            lijst.Add(new Mesh("../../assets/car/metal parts.obj", Matrix4.Identity, car, metal, 100f, 0.25f));
            lijst.Add(new Mesh("../../assets/car/seats.obj", Matrix4.Identity, car, chair));
            lijst.Add(new Mesh("../../assets/car/steering wheel.obj", Matrix4.Identity, car, wood));
            wheels[0] = new Mesh("../../assets/car/test.obj", Matrix4.CreateRotationX(rotation) * Matrix4.CreateTranslation(-2.6f, -1.8f, -4f), car, tire);
            wheels[1] = new Mesh("../../assets/car/test.obj", Matrix4.CreateRotationX(rotation) * Matrix4.CreateTranslation(-2.6f, -1.8f, 4.6f), car, tire);
            wheels[2] = new Mesh("../../assets/car/rechts.obj", Matrix4.CreateRotationX(rotation) * Matrix4.CreateTranslation(2.6f, -1.8f, 4.6f), car, tire);
            wheels[3] = new Mesh("../../assets/car/rechts.obj", Matrix4.CreateRotationX(rotation) * Matrix4.CreateTranslation(2.6f, -1.8f, -4f), car, tire);
            // wheels
            lijst.Add(wheels[0]);
            lijst.Add(wheels[1]);
            lijst.Add(wheels[2]);
            lijst.Add(wheels[3]);

            // light sources
            Matrix4 position1 = Matrix4.CreateTranslation(0, 10, -10) * view;
            Matrix4 position2 = Matrix4.CreateTranslation(0, 10, 10) * view;
            Matrix4 position3 = Matrix4.CreateTranslation(2, -1, -8.5f) * view;
            Matrix4 position4 = Matrix4.CreateTranslation(-2, -1, -8.5f) * view;
            Matrix4 position5 = Matrix4.CreateTranslation(2, 0, 7) * view;
            Matrix4 position6 = Matrix4.CreateTranslation(-2, 0, 7) * view;

            Vector3 intensity1 = new Vector3(1000, 000, 0);
            Vector3 intensity2 = new Vector3(0, 1000, 000);
            Vector3 intensity3 = new Vector3(100, 100, 100);
            Vector3 intensity4 = new Vector3(100, 100, 100);
            Vector3 intensity5 = new Vector3(25, 0, 0);
            Vector3 intensity6 = new Vector3(25, 0, 0);
            lights.Add(new Light(position1, intensity1));
            lights.Add(new Light(position2, intensity2));
            lights.Add(new Light(position3, intensity3, car));
            lights.Add(new Light(position4, intensity4, car));
            lights.Add(new Light(position5, intensity5, car));
            lights.Add(new Light(position6, intensity6, car));
        }

        // rotate the wheels of the car
        public void rotateWheels(float gonnarotate)
        {
            rotation += gonnarotate;
            wheels[0].origin = Matrix4.CreateRotationX(rotation) * Matrix4.CreateTranslation(2.6f, -1.8f, -4f) ;
            wheels[1].origin = Matrix4.CreateRotationX(rotation) * Matrix4.CreateTranslation(2.6f, -1.8f, 4.6f) ;
            wheels[2].origin = Matrix4.CreateRotationX(rotation) * Matrix4.CreateTranslation(-2.6f, -1.8f, 4.6f);
            wheels[3].origin = Matrix4.CreateRotationX(rotation) * Matrix4.CreateTranslation(-2.6f, -1.8f, -4f) ;
        }

        // drive the car around
        public void RotateCar()
        {
            if (!brake)
            {
                // rotate the car around the teapots (roughly)
                car.modelmatrix = Matrix4.CreateRotationY(PI / 2) * Matrix4.CreateTranslation(0, 0, 25) * Matrix4.CreateRotationY(rotation_of_car) * Matrix4.CreateTranslation(-7, 0, -17);
                rotation_of_car += 0.025f;
                rotateWheels(0.25f);
            }
            update();
        }

        // render the meshes
        public void render ()
        {
            foreach(Mesh mesh in lijst)
            {
                // set the matrices correct
                mesh.transform = mesh.modelmatrix * view;
                mesh.MV = mesh.transform;
                mesh.transform *= projection;
                mesh.Render(shader, view);
            }
        } // render

        // update all the matrices of the meshes
        public void update()
        {
            foreach(Mesh mesh in lijst)
            {
                mesh.update();
            }
            // update the car backlights
            if (brake)
            {
                Light light1 = lights[4];
                light1.intensity = new Vector3(100, 0, 0);
                Light light2 = lights[5];
                light2.intensity = new Vector3(100, 0, 0);
            }
            else
            {
                Light light1 = lights[4];
                light1.intensity = new Vector3(25, 0, 0);
                Light light2 = lights[5];
                light2.intensity = new Vector3(25, 0, 0);
            }
        } // update
    }
}
