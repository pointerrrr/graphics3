#version 330
// light struct
struct Light
{
	mat4 position;
	vec3 intensity;
};

// shader input
in vec2 uv;						// interpolated texture coordinates
in vec3 diffuse;
in vec3 specular;

// shader output
out vec4 outputColor;

// uniform variables
uniform vec4 ambient;
uniform sampler2D pixels;		// texture sampler

// fragment shader
void main()
{
	vec4 diffuse2 = vec4(diffuse, 1.0);
	vec4 specular2 = vec4(specular, 1.0);
	outputColor = texture( pixels, uv ) * diffuse2  + texture( pixels, uv ) * specular2  + ambient * texture(pixels, uv);	
}
