#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in float diffuse;
in float specular;

uniform sampler2D pixels;		// texture sampler

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	// light intensity of 10
	outputColor = texture( pixels, uv ) * diffuse * 100 * 0.5 + texture( pixels, uv ) * specular * 100 * 0.5 + vec4(1,1,1,1) * 0.1;	
}
