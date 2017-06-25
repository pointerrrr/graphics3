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
            if (keyState[Key.Left])
                a += 0.001f * frameDuration;
            if(keyState[Key.Right])
                a -= 0.001f * frameDuration;
            if (keyState[Key.Up])
                b += 0.001f * frameDuration;
            if (keyState[Key.Down])
                b -= 0.001f * frameDuration;
            if (keyState[Key.Z])
                c += 0.001f * frameDuration;
            if (keyState[Key.X])
                c -= 0.001f * frameDuration;

            if (keyState[Key.W])
                z += 0.001f * frameDuration;
            if (keyState[Key.A])
                x += 0.001f * frameDuration;
            if (keyState[Key.S])
                z -= 0.001f * frameDuration;
            if (keyState[Key.D])
                x -= 0.001f * frameDuration;
            if (keyState[Key.ControlLeft] && keyState[Key.ShiftLeft])
            { a = PI / 2f; b = 0; c = 0; x = 0; y = -4; z = -15; }
          
            oldKeyboardState = keyState;
        }
    

	    // tick for OpenGL rendering code
	    public void RenderGL()
	    {
            // measure frame duration
            

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