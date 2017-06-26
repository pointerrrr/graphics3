﻿using System.Diagnostics;
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
        float speed = 1;
        public bool pressed;
	    const float PI = 3.1415926535f;			// PI
        private float a = PI/2f, b = 0, c = 0, x =0, y =0, z =0;		// teapot rotation angle
	    Stopwatch timer;                        // timer for measuring frame duration
        int teller = 0;		// shader to use for rendering
	    Shader postproc;						// shader to use for post processing
        SceneGraph scene;

        RenderTarget target;					// intermediate render target
	    ScreenQuad quad;						// screen filling quad for post processing
	    bool useRenderTarget = true;
        private  KeyboardState oldKeyboardState = OpenTK.Input.Keyboard.GetState();

        // initialize
        public void Init()
	    {
            Console.WriteLine("Press Left-Shift to reset view");
            Console.WriteLine("Press right shift to reset speed");
            Console.WriteLine("Press Escape to add an model.");
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
            if (keyState[Key.Left])
                // a += 0.001f * frameDuration;
                scene.view *= Matrix4.CreateRotationY(-.1f * speed);
            if(keyState[Key.Right])
                // a -= 0.001f * frameDuration;
                scene.view *= Matrix4.CreateRotationY(.1f * speed);
            if (keyState[Key.Up])
                // b += 0.001f * frameDuration;
                scene.view *= Matrix4.CreateRotationX(-.1f * speed);
            if (keyState[Key.Down])
                // b -= 0.001f * frameDuration;
                scene.view *= Matrix4.CreateRotationX(.1f * speed);
            if (keyState[Key.Z])
                // c += 0.001f * frameDuration;
                scene.view *= Matrix4.CreateRotationZ(-.1f * speed);
            if (keyState[Key.X])
                // c -= 0.001f * frameDuration;
                scene.view *= Matrix4.CreateRotationZ(.1f * speed);

            if (keyState[Key.W])
                // z += 0.001f * frameDuration;
                scene.view *= Matrix4.CreateTranslation(0,0,1 * speed);
            if (keyState[Key.A])
                //x += 0.001f * frameDuration;
                scene.view *= Matrix4.CreateTranslation(1 * speed, 0, 0);
            if (keyState[Key.S])
                // z -= 0.001f * frameDuration;
                scene.view *= Matrix4.CreateTranslation(0, 0, -1 * speed);
            if (keyState[Key.D])
                //x -= 0.001f * frameDuration;
                scene.view *= Matrix4.CreateTranslation(-1 * speed, 0, 0);
            if (keyState[Key.Q] && !pressed)
            { speed /= 2; teller = 0; pressed = true; }
            if (keyState[Key.E] && !pressed)
            { speed *= 2; teller = 0; pressed = true; }
            if (speed < 0)
                speed = 0;
            if (keyState[Key.ShiftLeft])
                scene.view = Matrix4.Identity;
            if (keyState[Key.ControlLeft])
                speed = 1;
            if (keyState[Key.Escape])
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
            oldKeyboardState = keyState;
        }
    

	    // tick for OpenGL rendering code
	    public void RenderGL()
	    {
            // measure frame duration
            teller++;
            if (teller > 30)
                pressed = false;

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