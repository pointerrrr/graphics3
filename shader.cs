using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Template_P3 {

    public class Shader
    {
	    // data members
	    public int programID, vsID, fsID;
	    public int attribute_vpos;
	    public int attribute_vnrm;
	    public int attribute_vuvs;
		public int attribute_spec;
		public int attribute_diffPerc;
	    public int uniform_mview;
        public int uniform_mv;
        public int uniform_lightposition1, uniform_lightposition2, uniform_lightposition3, uniform_lightposition4, uniform_lightposition5, uniform_lightposition6;
        public int uniform_lightintensity1, uniform_lightintensity2, uniform_lightintensity3, uniform_lightintensity4, uniform_lightintensity5, uniform_lightintensity6;
        public int uniform_ambient;
        
        // constructor
        public Shader( String vertexShader, String fragmentShader )
	    {
		    // compile shaders
		    programID = GL.CreateProgram();
		    Load( vertexShader, ShaderType.VertexShader, programID, out vsID );
		    Load( fragmentShader, ShaderType.FragmentShader, programID, out fsID );
		    GL.LinkProgram( programID );
		    Console.WriteLine( GL.GetProgramInfoLog( programID ) );
            // get locations of shader parameters
		    attribute_vpos = GL.GetAttribLocation( programID, "vPosition" );
		    attribute_vnrm = GL.GetAttribLocation( programID, "vNormal" );
		    attribute_vuvs = GL.GetAttribLocation( programID, "vUV" );
			attribute_spec = GL.GetAttribLocation( programID, "spec");
			attribute_diffPerc = GL.GetAttribLocation( programID, "diffPerc");
		    uniform_mview = GL.GetUniformLocation( programID, "transform" );
	        uniform_mv = GL.GetUniformLocation(programID, "MV");
	        uniform_ambient = GL.GetUniformLocation(programID, "ambient");
            uniform_lightposition1 = GL.GetUniformLocation(programID, "light1.position");
	        uniform_lightintensity1 = GL.GetUniformLocation(programID, "light1.intensity");
	        uniform_lightposition2 = GL.GetUniformLocation(programID, "light2.position");
	        uniform_lightintensity2 = GL.GetUniformLocation(programID, "light2.intensity");
	        uniform_lightposition3 = GL.GetUniformLocation(programID, "light3.position");
	        uniform_lightintensity3 = GL.GetUniformLocation(programID, "light3.intensity");
	        uniform_lightposition4 = GL.GetUniformLocation(programID, "light4.position");
	        uniform_lightintensity4 = GL.GetUniformLocation(programID, "light4.intensity");
	        uniform_lightposition5 = GL.GetUniformLocation(programID, "light5.position");
	        uniform_lightintensity5 = GL.GetUniformLocation(programID, "light5.intensity");
	        uniform_lightposition6 = GL.GetUniformLocation(programID, "light6.position");
	        uniform_lightintensity6 = GL.GetUniformLocation(programID, "light6.intensity");
        } // Shader

	    // loading shaders
	    void Load( String filename, ShaderType type, int program, out int ID )
	    {
		    // source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
		    ID = GL.CreateShader( type );
		    using (StreamReader sr = new StreamReader( filename )) GL.ShaderSource( ID, sr.ReadToEnd() );
		    GL.CompileShader( ID );
		    GL.AttachShader( program, ID );
		    Console.WriteLine( GL.GetShaderInfoLog( ID ) );
	    } // Load
    }

} // namespace Template_P3
