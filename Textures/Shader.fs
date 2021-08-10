#version 330 core
out vec4 FragColor;

int vec3 ourColor;
int vec2 TexCoord;

uniform simpler2D texture1;

void main()
{
	FragColor=texture(texture1,TexCoord);
}


