#version 330
// light struct
struct Light
{
	vec3 position;
	vec3 intensity;
};

// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in float diffuse;
in float specular;



// shader output
out vec4 outputColor;

// uniform variables
uniform Light light;
uniform vec4 ambient;
uniform sampler2D pixels;		// texture sampler

// fragment shader
void main()
{
	vec4 intensity4 = vec4(light.intensity, 1.0);
	outputColor = texture( pixels, uv ) * diffuse * intensity4 + texture( pixels, uv ) * specular * intensity4 + ambient * 0.1;
}
