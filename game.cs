using System;
using OpenTK;
using OpenTK.Input;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3 {

    class Game
    {
        // member variables
        public Surface screen; // background surface for printing etc.
        public bool pressed, car;

        private const float PI = 3.1415926535f;			// PI
        private SceneGraph scene;                       // the scenegraph, which holds all lights and meshes
        private Shader postproc;						// shader to use for post processing
        private float speed = 1;
        // camera values
        private Vector3 cam_pos;
        private float cam_x, cam_z = -90;
        // rendering variables
        private RenderTarget target;					// intermediate render target
	    private ScreenQuad quad;						// screen filling quad for post processing
	    private bool useRenderTarget = true;
        // for control
        private KeyboardState oldKeyboardState, newKeyboardState;
        
        // initialize
        public void Init()
	    {
            // give some info
            Console.WriteLine("Press Left-Shift to reset view");
            Console.WriteLine("Press right shift to reset speed");
            Console.WriteLine("Press Tab to add a model");
            Console.WriteLine("To brake the car hold space");
	        Console.WriteLine("To move: WASDQE, to rotate the camera: arrow keys");
            // make the scenegraph
            scene = new SceneGraph();
		    // create shaders
		    postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );
		    // create the render target
		    target = new RenderTarget( screen.width, screen.height );
		    quad = new ScreenQuad();
   	    } // Init

	    // tick for background surface
	    public void Tick()
	    {
		    screen.Clear( 0 );
	    } // Tick

        // for controlling the game
        public void Control(KeyboardState keyState)
        {
            // control handling
            newKeyboardState = keyState;
            Vector3 direction = new Vector3((float)Math.Sin(cam_x) * (float)Math.Sin(cam_z), (float)Math.Cos(cam_z), (float)Math.Cos(cam_x) * (float)Math.Sin(cam_z));
            if (keyState[Key.Left])
                cam_x -= 0.1f;
            if (keyState[Key.Right])
                cam_x += 0.1f;
            if (keyState[Key.Up])
                cam_z -= 0.1f;
            if (keyState[Key.Down])
                cam_z += 0.1f;
            if (keyState[Key.W])
                cam_pos -= Vector3.Normalize(new Vector3(-direction.X, 0, direction.Z)) * speed;
            if (keyState[Key.S])
                cam_pos += Vector3.Normalize(new Vector3(-direction.X, 0, direction.Z)) * speed;
            if (keyState[Key.A])
                cam_pos -= Vector3.Normalize(new Vector3(direction.Z, 0, direction.X)) * speed;
            if (keyState[Key.D])
                cam_pos += Vector3.Normalize(new Vector3(direction.Z, 0, direction.X)) * speed;
            if (keyState[Key.Q])
                cam_pos -= new Vector3(0, 1, 0) * speed;
            if (keyState[Key.E])
                cam_pos += new Vector3(0, 1, 0) * speed;
            if (keyState[Key.ShiftLeft])
            {
                cam_x = 0;
                cam_z = -90;
                cam_pos = new Vector3(0,0,0);
                car = false;
            }
            if (NewKeyPress(Key.J))
            { speed /= 2;}
            if (NewKeyPress(Key.K))
            { speed *= 2;  }
            if (speed < 0)
                speed = 0;
            if (keyState[Key.ControlLeft])
                speed = 1;
            if (NewKeyPress(Key.Tab))
            LoadModel();
            if (keyState[Key.Space])
                scene.brake = true;
            else
                scene.brake = false;
            oldKeyboardState = keyState;
        } // Control

        // load in a model at runtime
        private void LoadModel()
        {
            Console.WriteLine("Please enter in the Location of obj file. Example: C:\\Users\\Documents\\mesh.obj");
            string loc = Console.ReadLine();
            Console.WriteLine("Please enter the location of the texture file.Example: C:\\Users\\Documents\\mesh.jpg. if none please do not enter anything.");
            string tex = Console.ReadLine();
            Console.WriteLine("Please enter the desired specularity of the object (has to be higher than zero)");
            string spec = Console.ReadLine();
            Console.WriteLine("Please enter the desired specularity percentage of the object (between 0 and 1)");
            string specpercentage = Console.ReadLine();
            Console.WriteLine("please enter the coordinates in the following configuration (all integers) X -coordinate Y-coordinate Z-coordinate");
            try
            {
                string[] arr = Console.ReadLine().Split(' ');
                int tx = int.Parse(arr[0]), ty = int.Parse(arr[1]), tz = int.Parse(arr[2]);
                if (string.IsNullOrEmpty(tex))
                    scene.lijst.Add(new Mesh(loc, Matrix4.CreateTranslation(tx, ty, tz), null, float.Parse(spec), 1 - float.Parse(specpercentage)));
                else
                    scene.lijst.Add(new Mesh(loc, Matrix4.CreateTranslation(tx, ty, tz), new Texture(tex), float.Parse(spec), float.Parse(specpercentage)));
                var keyboard = Keyboard.GetState();
                Control(keyboard);
            }
            catch (Exception)
            {
                Console.WriteLine("something went wrong please try again.");
                var keyboard = Keyboard.GetState();
                Control(keyboard);
            }
        } // LoadModel

        // tick for OpenGL rendering code
	    public void RenderGL()
	    {
            scene.RotateCar();
            // recreate the view matrix
            scene.view = Matrix4.Identity;
            scene.view *= Matrix4.CreateTranslation(cam_pos);
            scene.view *= Matrix4.CreateRotationY(cam_x);
            scene.view *= Matrix4.CreateRotationX(cam_z + 90);
            
            // update rotation
            if (useRenderTarget)
		    {
			    // enable render target
			    target.Bind();

                scene.render();

			    // render quad
			    target.Unbind();
			    quad.Render( postproc, target.GetTextureID() );
		    }
		    else
		    {
                // render scene directly to the screen
                scene.render();
		    }
	    } // RenderGL

        // check if a key was already pressed or not
        public bool NewKeyPress(Key key)
        {
            return oldKeyboardState[key] && newKeyboardState[key] != oldKeyboardState[key];
        } // NewKeyPress
    }
} // namespace Template_P3