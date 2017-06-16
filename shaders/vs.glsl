#version 330
//based off of http://www.learnopengles.com/android-lesson-two-ambient-and-diffuse-lighting/
// shader input
in vec2 vUV;				// vertex uv coordinate
in vec3 vNormal;			// untransformed vertex normal
in vec3 vPosition;			// untransformed vertex position

// shader output
out vec4 normal;			// transformed vertex normal
out vec2 uv;		
out vec3 Pos;		
out float diffuse;
out float specular;

uniform mat4 transform;
uniform mat4 MV;
 
// vertex shader
void main()
{
	vec3 LightPos = vec3(0,-1,-7);
	vec3 modelViewVertex = vec3(MV * vec4( vPosition, 1.0 )); 
	vec3 modelViewNormal = vec3(MV * vec4( vNormal, 0.0));
	float distance = length(LightPos - modelViewVertex);
	vec3 lightVector = normalize(LightPos - modelViewVertex);
	diffuse = max(dot(modelViewNormal, lightVector), 0.1);
	vec3 V = normalize(-modelViewVertex);
	vec3 H = normalize(V + lightVector);
	float specDot = dot(modelViewNormal, H);	
	float attenuation = 1 / (distance * distance);	
	specular = pow(max(0, specDot),10) * attenuation;
	diffuse = diffuse * (1.0 / (1.0 + (0.25 * distance * distance)));		
	
	// transform vertex using supplied matrix
	gl_Position = transform * vec4(vPosition, 1.0);
	// forward normal and uv coordinate; will be interpolated over triangle
	normal = transform * vec4( vNormal, 0.0f );
	uv = vUV;
}