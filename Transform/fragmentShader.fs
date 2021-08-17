# version 330 core
out vec4 FragColor;
in vec2 TexCoord;

//texture samplers
uniform samplers2D texture1;
uniform samplers2D texture2;

void main()
{
	//linearly interpolate between both textures
	FragColor=mix(texture(texture1,TexCoord),texture(texture2,TexCoord),0.2);
}