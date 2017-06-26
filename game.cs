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

	    const float PI = 3.1415926535f;			// PI
        private float a = PI/2f, b = 0, c = 0, x =0, y =0, z =0;		// teapot rotation angle
	    Stopwatch timer;						// timer for measuring frame duration
							// shader to use for rendering
	    Shader postproc;						// shader to use for post processing
        SceneGraph scene;

        RenderTarget target;					// intermediate render target
	    ScreenQuad quad;						// screen filling quad for post processing
	    bool useRenderTarget = true;
        private Vector3 cam_pos;
        private float cam_x , cam_z = -90;
        private  KeyboardState oldKeyboardState = OpenTK.Input.Keyboard.GetState();

        // initialize
        public void Init()
	    {
            Console.WriteLine("Press Left-Shift and Left-Control simultaneously to reset view");
            scene = new SceneGraph();
		    // initialize stopwatch
		    timer = new Stopwatch();
		    timer.Reset();
		    timer.Start();
		    // create shaders
		    
		    postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );
		    // load a texture

		    // create the render target
		    target = new RenderTarget( screen.width, screen.height );
		    quad = new ScreenQuad();
   	    }

	    // tick for background surface
	    public void Tick()
	    {
		    screen.Clear( 0 );
		    screen.Print( "hello world", 2, 2, 0xffff00 );
	    }

        public void Control(KeyboardState keyState)
        {
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
            Vector3 direction = new Vector3((float)Math.Sin(cam_x   ) * (float)Math.Sin(cam_z  ), (float)Math.Cos(cam_z  ), (float)Math.Cos(cam_x ) * (float)Math.Sin(cam_z ));

            if (keyState[Key.Left])
                cam_x -= 0.1f;
            if (keyState[Key.Right])
                cam_x += 0.1f;
            if (keyState[Key.Up])
                cam_z -= 0.1f;
            if (keyState[Key.Down])
                cam_z += 0.1f;
            /*if (keyState[Key.Z])
                cam_dir.Z -= 0.1f;
            if (keyState[Key.X])
                cam_dir.Z += 0.1f;*/
            if (keyState[Key.W])
                cam_pos -= new Vector3(-direction.X, 0, direction.Z);
            if (keyState[Key.S])
                cam_pos += new Vector3(-direction.X, 0, direction.Z);
            if (keyState[Key.A])
                cam_pos -= new Vector3(direction.Z, 0, direction.X);
            if (keyState[Key.D])
                cam_pos += new Vector3(direction.Z, 0, direction.X);
            if (keyState[Key.Q])
                cam_pos -= new Vector3(0, 1, 0);
            if (keyState[Key.E])
                cam_pos += new Vector3(0, 1, 0);

            if (keyState[Key.ControlLeft] && keyState[Key.ShiftLeft])
            {
                cam_x = 0;
                cam_z = -90;
                cam_pos = new Vector3(0,0,0);
            }
          
            oldKeyboardState = keyState;
        }
    

	    // tick for OpenGL rendering code
	    public void RenderGL()
	    {
	        // measure frame duration
            scene.view = Matrix4.Identity;
	        scene.view *= Matrix4.CreateTranslation(cam_pos);
            scene.view *= Matrix4.CreateRotationY(cam_x);
	        scene.view *= Matrix4.CreateRotationX(cam_z+90);



            // prepare matrix for vertex shader
            // Matrix4 transform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a); 
            // transform *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), b);
            // transform *= Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), c);
            Matrix4 transform =Matrix4.CreateRotationY(a);
            transform *= Matrix4.CreateRotationZ(c);
            transform *= Matrix4.CreateRotationX(b);
            transform *= Matrix4.CreateTranslation(x, y, z);

	        
            // update rotation
            
            if (a > 2 * PI) a -= 2 * PI;

		    if (useRenderTarget)
		    {
			    // enable render target
			    target.Bind();

                scene.render(transform);

			    // render quad
			    target.Unbind();
			    quad.Render( postproc, target.GetTextureID() );
		    }
		    else
		    {
                // render scene directly to the screen
                scene.render(transform);
		    }
	    }
    }

} // namespace Template_P3