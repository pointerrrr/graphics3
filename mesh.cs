using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template_P3 {

// mesh and loader based on work by JTalton; http://www.opentk.com/node/642

    public class Mesh
    {
        // data members
        public Mesh parent;
        public Matrix4 modelmatrix, transform, MV, origin; 
        Texture texture;
		public float spec = 100;
		public float diffPerc = 0.5f;
        const float PI = 3.1415926535f;
        public Matrix4 location;
	    public ObjVertex[] vertices;			// vertex positions, model space
	    public ObjTriangle[] triangles;			// triangles (3 vertex indices)
	    public ObjQuad[] quads;					// quads (4 vertex indices)
	    int vertexBufferId;						// vertex buffer
	    int triangleBufferId;					// triangle buffer
	    int quadBufferId;						// quad buffer

		// constructor
		public Mesh( string fileName,Matrix4 model, Texture text = null, float specularity = 0, float diffusePerc = 1)
		{
			MeshLoader loader = new MeshLoader();
			loader.Load( this, fileName );
			texture = text;
			modelmatrix = model;
			spec = specularity;
			diffPerc = diffusePerc;
		}

		public Mesh(string fileName, Matrix4 model, Mesh parent, Texture text = null, float specularity = 0, float diffusePerc = 1)
		{
			MeshLoader loader = new MeshLoader();
			loader.Load(this, fileName);
				texture = text;
			this.parent = parent;
			spec = specularity;
			diffPerc = diffusePerc;
			origin = model;
			modelmatrix =  model * parent.modelmatrix;
		}

        public void update()
        {
            if (this.parent != null)
            {
                modelmatrix = origin * parent.modelmatrix;
                
            }
           
        }

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
	    }

	// render the mesh using the supplied shader and matrix
	public void Render( Shader shader )
	{
            //Matrix4 loc = transform * Matrix4.CreateTranslation(x, y, z);
            //if (parent != null)
            //    loc *= Matrix4.CreateTranslation(parent.x, parent.y, parent.z);
            //Matrix4 MV = loc;
            //loc *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            //meshTree[i].transform = meshTree[i].modelMatrix * viewMatrix * projectionMatrix;

            

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

            Vector3 position = new Vector3(0, 10, 0);
            Vector3 intensity = new Vector3(1000, 1000, 1000);
            Vector4 ambient = new Vector4(1,1,1,1);

	        GL.Uniform3(shader.uniform_lightintensity, ref intensity);
	        GL.Uniform3(shader.uniform_lightposition, ref position);
            GL.Uniform4(shader.uniform_ambient, ref ambient);

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
	    }

       

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

    

} // namespace Template_P3