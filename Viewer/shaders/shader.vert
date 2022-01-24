#version 330 compatibility
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 aTangents;

// struct Light {
    // vec3 position;
    // vec3 ambient;
    // vec3 diffuse;
    // vec3 specular;
    // float radius;
// };

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 viewPos;
//uniform Light[4] light;

out VS_OUT 
{
    vec3 Normal;
    vec3 FragPos;
    vec2 TexCoords;
    //vec3 LightDirTangent;
    //vec3 EyeDirTangent;
    mat3 TBN;
} vs_out;

void main()
{
    vec3 T = normalize(vec3(model * vec4(aTangents, 0.0)));
    vec3 N = normalize(vec3(model * vec4(aNormal, 0.0)));
    vec3 B = cross(N, T);
    T = normalize(T - dot(T, N) * N);
    mat3 TBN = mat3(T, B, N);
    
    //vec3 lightDir = normalize(light.position - vs_out.FragPos);
    //vec3 viewDir = normalize(viewPos - vs_out.FragPos);

    vs_out.FragPos = vec3(vec4(aPos, 1.0) * model);
    vs_out.Normal = aNormal * mat3(transpose(inverse(model)));
    vs_out.TBN = transpose(TBN);
    //vs_out.LightDirTangent = vs_out.TBN * lightDir;
    //vs_out.EyeDirTangent =  vs_out.TBN * viewDir;
    vs_out.TexCoords = aTexCoords;

    gl_Position = vec4(aPos, 1.0) * model * view * projection;
}