using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template_P3 {

// mesh and loader based on work by JTalton; http://www.opentk.com/node/642

    public class Mesh
    {
        // data members
        const float PI = 3.1415926535f;

        public Mesh parent;                     // parent mesh, for relative positioning
        public Matrix4 modelmatrix, transform, MV, origin, location; // matrices, used in the shader and scenegraph
        public float spec = 100;                // specularity
		public float diffPerc = 0.5f;           // diffuse percentage
	    public ObjVertex[] vertices;			// vertex positions, model space
	    public ObjTriangle[] triangles;			// triangles (3 vertex indices)
	    public ObjQuad[] quads;					// quads (4 vertex indices)

	    private int vertexBufferId;				// vertex buffer
	    private int triangleBufferId;			// triangle buffer
	    private int quadBufferId;			    // quad buffer
        private Texture texture;                // texure for this mesh

        // constructor for mesh without parent
        public Mesh( string fileName,Matrix4 model, Texture text = null, float specularity = 0, float diffusePerc = 1)
		{
			MeshLoader loader = new MeshLoader();
			loader.Load( this, fileName );
		    if (text == null)
		        texture = new Texture("../../assets/white.png");
            else
			    texture = text;
			modelmatrix = model;
			spec = specularity;
			diffPerc = diffusePerc;
		} // Mesh

        // constructor for mesh with parent
		public Mesh(string fileName, Matrix4 model, Mesh parent, Texture text = null, float specularity = 0, float diffusePerc = 1)
		{
			MeshLoader loader = new MeshLoader();
			loader.Load(this, fileName);
		    if (text == null)
		        texture = new Texture("../../assets/white.png");
		    else
		        texture = text;
            this.parent = parent;
			spec = specularity;
			diffPerc = diffusePerc;
			origin = model;
            // multiply by parent matrix for relative position
			modelmatrix =  model * parent.modelmatrix;
		} // Mesh

        // update the matrix relative to its original, to make sure everything stays in order
        public void update()
        {
            if (parent != null)
            {
                modelmatrix = origin * parent.modelmatrix;
            }
        } // update

	    // initialization; called during first render
	    public void Prepare( Shader shader )
	    {
            if (vertexBufferId != 0) return; // already taken care of

            // generate interleaved vertex data (uv/normal/position (total 8 floats) per vertex)
		    GL.GenBuffers( 1, out vertexBufferId );
		    GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBufferId );
		    GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf( typeof( ObjVertex ) )), vertices, BufferUsageHint.StaticDraw );

		    // generate triangle index array
		    GL.GenBuffers( 1, out triangleBufferId );
		    GL.BindBuffer( BufferTarget.ElementArrayBuffer, triangleBufferId );
		    GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf( typeof( ObjTriangle ) )), triangles, BufferUsageHint.StaticDraw );

		    // generate quad index array
		    GL.GenBuffers( 1, out quadBufferId );
		    GL.BindBuffer( BufferTarget.ElementArrayBuffer, quadBufferId );
		    GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf( typeof( ObjQuad ) )), quads, BufferUsageHint.StaticDraw );
	    } // Prepare

	    // render the mesh using the supplied shader and matrix
	    public void Render( Shader shader, Matrix4 view )
	    {
            // on first run, prepare buffers
            Prepare( shader );

            // enable texture
            if (texture != null)
            {
                int texLoc = GL.GetUniformLocation(shader.programID, "pixels");
                GL.Uniform1(texLoc, 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texture.id);
            }

		    // enable shader
		    GL.UseProgram( shader.programID );
	        List<Light> lights = SceneGraph.lights;
            // set light variables
	        Vector4 ambient = new Vector4(0.1f, 0.1f, 0.1f, 0);
            Matrix4 position1 = lights[0].position * lights[0].parentmatrix * view;
	        Matrix4 position2 = lights[1].position * lights[1].parentmatrix * view;
	        Matrix4 position3 = lights[2].position * lights[2].parentmatrix * view;
	        Matrix4 position4 = lights[3].position * lights[3].parentmatrix * view;
	        Matrix4 position5 = lights[4].position * lights[4].parentmatrix * view;
	        Matrix4 position6 = lights[5].position * lights[5].parentmatrix * view;

            Vector3 intensity1 = lights[0].intensity;
	        Vector3 intensity2 = lights[1].intensity;
	        Vector3 intensity3 = lights[2].intensity;
	        Vector3 intensity4 = lights[3].intensity;
	        Vector3 intensity5 = lights[4].intensity;
	        Vector3 intensity6 = lights[5].intensity;
            // pass them to the shader
            GL.Uniform3(shader.uniform_lightintensity1, ref intensity1);
	        GL.Uniform3(shader.uniform_lightintensity2, ref intensity2);
	        GL.Uniform3(shader.uniform_lightintensity3, ref intensity3);
	        GL.Uniform3(shader.uniform_lightintensity4, ref intensity4);
	        GL.Uniform3(shader.uniform_lightintensity5, ref intensity5);
	        GL.Uniform3(shader.uniform_lightintensity6, ref intensity6);
            GL.Uniform4(shader.uniform_ambient, ref ambient);
            GL.UniformMatrix4(shader.uniform_lightposition1, false, ref position1);
	        GL.UniformMatrix4(shader.uniform_lightposition2, false, ref position2);
	        GL.UniformMatrix4(shader.uniform_lightposition3, false, ref position3);
	        GL.UniformMatrix4(shader.uniform_lightposition4, false, ref position4);
	        GL.UniformMatrix4(shader.uniform_lightposition5, false, ref position5);
	        GL.UniformMatrix4(shader.uniform_lightposition6, false, ref position6);

	        // pass transform to vertex shader
            GL.UniformMatrix4( shader.uniform_mview, false, ref transform );
            GL.UniformMatrix4( shader.uniform_mv, false, ref MV);

		    // bind interleaved vertex data
		    GL.EnableClientState( ArrayCap.VertexArray );
		    GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBufferId );
		    GL.InterleavedArrays( InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf( typeof( ObjVertex ) ), IntPtr.Zero );

		    // link vertex attributes to shader parameters 
		    GL.VertexAttribPointer( shader.attribute_vuvs, 2, VertexAttribPointerType.Float, false, 32, 0 );
		    GL.VertexAttribPointer( shader.attribute_vnrm, 3, VertexAttribPointerType.Float, true, 32, 2 * 4 );
		    GL.VertexAttribPointer( shader.attribute_vpos, 3, VertexAttribPointerType.Float, false, 32, 5 * 4 );
		    GL.VertexAttrib1(shader.attribute_spec, spec);
		    GL.VertexAttrib1(shader.attribute_diffPerc, diffPerc);

		    // enable position, normal and uv attributes
		    GL.EnableVertexAttribArray( shader.attribute_vpos );
            GL.EnableVertexAttribArray( shader.attribute_vnrm );
            GL.EnableVertexAttribArray( shader.attribute_vuvs );

		    // bind triangle index data and render
		    GL.BindBuffer( BufferTarget.ElementArrayBuffer, triangleBufferId );
		    GL.DrawArrays( PrimitiveType.Triangles, 0, triangles.Length * 3 );

		    // bind quad index data and render
		    if (quads.Length > 0)
		    {
			    GL.BindBuffer( BufferTarget.ElementArrayBuffer, quadBufferId );
			    GL.DrawArrays( PrimitiveType.Quads, 0, quads.Length * 4 );
		    }

		    // restore previous OpenGL state
		    GL.UseProgram( 0 );
	    } // Render

       

        // layout of a single vertex
        [StructLayout(LayoutKind.Sequential)] public struct ObjVertex
	    {
		    public Vector2 TexCoord;
		    public Vector3 Normal;
		    public Vector3 Vertex;
	    }

	    // layout of a single triangle
	    [StructLayout(LayoutKind.Sequential)] public struct ObjTriangle
	    {
		    public int Index0, Index1, Index2;
	    }

	    // layout of a single quad
	    [StructLayout(LayoutKind.Sequential)] public struct ObjQuad
	    {
	        public int Index0, Index1, Index2, Index3;
	    }

    }

    // light source struct
    public class Light
    {
        public Matrix4 position;

        // return the matrix of the parent
        public Matrix4 parentmatrix
        {
            get
            {
                if(parent!=null)
                    return parent.modelmatrix;
                return Matrix4.Identity;
            }
        }

        public Vector3 intensity;
        public Mesh parent;

        public Light(Matrix4 position, Vector3 intensity, Mesh parent = null)
        {
            this.position = position;
            this.intensity = intensity;
            this.parent = parent;
            if (parent != null)
                this.position *= parent.modelmatrix;
        }
    }
} // namespace Template_P3