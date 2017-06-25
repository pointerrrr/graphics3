#version 330

// shader input
in vec2 uv;						// interpolated texture coordinates
//in vec3 intensity;
in vec4 normal;					// interpolated normal
in float diffuse;
in float specular;

uniform sampler2D pixels;		// texture sampler

// shader output
out vec4 outputColor;

uniform vec3 lint;

uniform vec4 ambient;

// fragment shader
void main()
{
	vec4 intensity4 = vec4(lint, 1.0);
	outputColor = texture( pixels, uv ) * diffuse * intensity4 + texture( pixels, uv ) * specular * intensity4 + ambient * 0.1;
	//outputColor = intensity4;
}
