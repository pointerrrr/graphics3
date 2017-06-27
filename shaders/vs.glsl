#version 330
//based off of http://www.learnopengles.com/android-lesson-two-ambient-and-diffuse-lighting/
// light struct
struct Light
{
	mat4 position;
	vec3 intensity;
};

// shader input
in vec2 vUV;				// vertex uv coordinate
in vec3 vNormal;			// untransformed vertex normal
in vec3 vPosition;			// untransformed vertex position
in float spec;				// specularity of mesh
in float diffPerc;			// percentage of diffuse lighting

// shader output
out vec4 normal;			// transformed vertex normal
out vec2 uv;
out vec3 diffuse;
out vec3 specular;

// uniform variables
uniform mat4 transform;
uniform mat4 MV;
uniform Light light1;
uniform Light light2;
uniform Light light3;
uniform Light light4;
uniform Light light5;
uniform Light light6;
 
// vertex shader
void main()
{
	// vertex positions after transform
	vec3 modelViewVertex = vec3(MV * vec4( vPosition, 1.0 )); 
	vec3 modelViewNormal = vec3(MV * vec4( vNormal, 0.0));
	// the lights
	Light lights[6] = Light[6](light1, light2, light3, light4, light5, light6);
	diffuse = vec3(0,0,0);
	specular = vec3(0,0,0);
	// calculate the total light value and pass them to the fragment shader
	for(int i = 0; i < 6;i++)
	{
		vec3 LightPos = vec3(lights[i].position * vec4(0,0,0,1));
		float distance = length(LightPos - modelViewVertex);
		vec3 lightVector = normalize(LightPos - modelViewVertex);
		float diffusetemp = max(dot(modelViewNormal, lightVector), 0.1);
		vec3 V = normalize(-modelViewVertex);
		vec3 H = normalize(V + lightVector);
		float specDot = dot(modelViewNormal, H);	
		float attenuation = 1 / (distance * distance);	
		specular = specular + pow(max(0, specDot),spec + 1) * attenuation * (1 - diffPerc) * lights[i].intensity;
		diffuse = diffuse + diffusetemp * attenuation * diffPerc * lights[i].intensity;		
	}
	// transform vertex using supplied matrix
	gl_Position = transform * vec4(vPosition, 1.0);
	// forward normal and uv coordinate; will be interpolated over triangle
	uv = vUV;
}

