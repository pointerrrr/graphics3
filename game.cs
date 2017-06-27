using System.Diagnostics;
using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3 {

    class Game
    {
	    // member variables
	    public Surface screen;					// background surface for printing etc.
        private float speed = 1, tireR, x =0;   //variables for speed, TireRotation and the rotation of the car
        public bool pressed, car;               //Booleans for preventing fast presses of some keys and if the car is active;
	    const float PI = 3.1415926535f;			// PI
		
	    Stopwatch timer;                        // timer for measuring frame duration
        int teller = 0;		                    // counter wich is used for the time between key presses
	    Shader postproc;						// shader to use for post processing
        SceneGraph scene;                       //SceneGraph variable

        RenderTarget target;					// intermediate render target
	    ScreenQuad quad;						// screen filling quad for post processing
	    bool useRenderTarget = true;
        private  KeyboardState oldKeyboardState = OpenTK.Input.Keyboard.GetState(); 
        private Vector3 cam_pos;                // camera position
        private float cam_x, cam_z = -90;       // x component and z component of camera

        // initialize
        public void Init()
	    {
            Console.WriteLine("Press Left-Shift to reset view");
            Console.WriteLine("Press right shift to reset speed");
            Console.WriteLine("Press Tab to add an model.");
            Console.WriteLine("To control the car press P");
            Console.WriteLine("To reset the car press R");

            //initialize ScenGraph
            scene = new SceneGraph();

		    // initialize stopwatch
		    timer = new Stopwatch();
		    timer.Reset();
		    timer.Start();

		    // create shaders
		    postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );

		    // create the render target
		    target = new RenderTarget( screen.width, screen.height );
		    quad = new ScreenQuad();
        }

	    // tick for background surface
	    public void Tick()
	    {
		    screen.Clear( 0 );
	    }


        // Method for controling the car and camera
        public void Control(KeyboardState keyState)
        {
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
            Vector3 direction = new Vector3((float)Math.Sin(cam_x) * (float)Math.Sin(cam_z), (float)Math.Cos(cam_z), (float)Math.Cos(cam_x) * (float)Math.Sin(cam_z));

            //car is controled
            if (car)
            {
                //Translation of the car
                if (keyState[Key.D] && (keyState[Key.W] || keyState[Key.S]))
                { scene.car.modelmatrix *= Matrix4.CreateRotationY(-0.01f);  x += 0.01f; }//scene.view *= Matrix4.CreateRotationY(0.01f); }
                if (keyState[Key.A] && (keyState[Key.W] || keyState[Key.S]))
                { scene.car.modelmatrix *= Matrix4.CreateRotationY(0.01f); x -= 0.01f;  }
                if (keyState[Key.W])
                { scene.car.modelmatrix *= (Matrix4.CreateTranslation(x, 0, -1)); tireR =1; updateTire(); scene.update(); }
                if (keyState[Key.S])
                { scene.car.modelmatrix *= (Matrix4.CreateTranslation(-x, 0, 1)); tireR = -1; updateTire(); scene.update(); }

                //Translation of the camera
                if (keyState[Key.Up])
                    cam_pos -= new Vector3(-direction.X, 0, direction.Z) * speed;
                if (keyState[Key.Down])
                    cam_pos += new Vector3(-direction.X, 0, direction.Z) * speed;
                if (keyState[Key.Left])
                    cam_pos -= new Vector3(direction.Z, 0, direction.X) * speed;
                if (keyState[Key.Right])
                    cam_pos += new Vector3(direction.Z, 0, direction.X) * speed;
               
                
            }
            //car is not controlled
            else
            {
                //Rotation of the camera
                if (keyState[Key.Left])
                    cam_x -= 0.1f * speed;
                if (keyState[Key.Right])
                    cam_x += 0.1f * speed;
                if (keyState[Key.Up])
                    cam_z -= 0.1f * speed;
                if (keyState[Key.Down])
                    cam_z += 0.1f * speed;

                //Translation of the camera
                if (keyState[Key.W])
                    cam_pos -= new Vector3(-direction.X, 0, direction.Z) * speed;
                if (keyState[Key.S])
                    cam_pos += new Vector3(-direction.X, 0, direction.Z) * speed;
                if (keyState[Key.A])
                    cam_pos -= new Vector3(direction.Z, 0, direction.X) * speed;
                if (keyState[Key.D])
                    cam_pos += new Vector3(direction.Z, 0, direction.X) * speed;
                if (keyState[Key.Q])
                    cam_pos -= new Vector3(0, 1, 0) * speed;
                if (keyState[Key.E])
                    cam_pos += new Vector3(0, 1, 0) * speed;
            }

            //Reset Camera Postion
            if (keyState[Key.ShiftLeft])
            {
                cam_x = 0;
                cam_z = -90;
                cam_pos = new Vector3(0,0,0);
                car = false;
            }
          
           
            //Decrease speed of camera
            if (keyState[Key.J] && !pressed)
            { speed /= 2; teller = 0; pressed = true; }
            //Increase speed of camera
            if (keyState[Key.K] && !pressed)
            { speed *= 2; teller = 0; pressed = true; }
            if (speed < 0)
                speed = 0;

            //Reset speed
           if (keyState[Key.ControlLeft])
                speed = 1;

           //Add your own Mesh
            if (keyState[Key.Tab])
            {
                teller = 0; pressed = true;
                Console.WriteLine("Please enter in the Location of obj file. Example: C:\\Users\\Documents\\mesh.obj");
                string loc = Console.ReadLine();
                Console.WriteLine("Please enter the location of the texture file.Example: C:\\Users\\Documents\\mesh.jpg. if none please enter Null. This is not yet implemented");
                string tex = Console.ReadLine();
                Console.WriteLine("please enter the coordinates in the following configuration (all integers) X -coordinate Y-coordinate Z-coordinate");
                try
                {
                    string[] arr = Console.ReadLine().Split(' ');
                    int tx = int.Parse(arr[0]), ty = int.Parse(arr[1]), tz = int.Parse(arr[2]);
                    if (string.IsNullOrEmpty(tex))
                        scene.lijst.Add(new Mesh(loc, Matrix4.CreateTranslation(tx, ty, tz)));
                    else
                        scene.lijst.Add(new Mesh(loc, Matrix4.CreateTranslation(tx, ty, tz), new Texture(tex)));
                    var keyboard = OpenTK.Input.Keyboard.GetState();
                    Control(keyboard);
                }
                catch(Exception e)
                {
                    Console.WriteLine("something went wrong please try again.");
                    var keyboard = OpenTK.Input.Keyboard.GetState();
                    Control(keyboard);
                }
            }

            //Control the Car
            if (keyState[Key.P])
            { car = true;  }

            //Return Car to origin position
            if (keyState[Key.R])
            {
                scene.car.modelmatrix = scene.car.origin;
                scene.update();
                
            }
                oldKeyboardState = keyState;
            //scene.view *= Matrix4.CreateTranslation(-1 * speed, 0, 0);
        }

        void updateTire()
        {
            scene.tireRA.rotation *= Matrix4.CreateRotationX(tireR);
            scene.tireRF.rotation *= Matrix4.CreateRotationX(tireR);
            scene.tireLA.rotation  *= Matrix4.CreateRotationX(tireR);
            scene.tireLF.rotation *= Matrix4.CreateRotationX(tireR);
        }
    
        void cameraUpdate()
        {
            scene.view = scene.car.modelmatrix * Matrix4.CreateRotationY(scene.rotation);
        }
	    // tick for OpenGL rendering code
	    public void RenderGL()
	    {
            // measure frame duration
            teller++;
            if (teller > 30)
                pressed = false;
           
                scene.view = Matrix4.Identity;
                scene.view *= Matrix4.CreateTranslation(cam_pos);
                scene.view *= Matrix4.CreateRotationY(cam_x);
                scene.view *= Matrix4.CreateRotationX(cam_z + 90);
            

		    if (useRenderTarget)
		    {
			    // enable render target
			    target.Bind();

                scene.render(car);

			    // render quad
			    target.Unbind();
			    quad.Render( postproc, target.GetTextureID() );
		    }
		    else
		    {
                // render scene directly to the screen
                scene.render(car);
		    }
	    }
    }

} // namespace Template_P3