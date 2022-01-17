#version 330 core

struct Material {
    sampler2D tDiffuseMap;
    sampler2D tSpecularMap;
    sampler2D tNormalMap;
    float     fMaxtSpecularMapPower;
};
struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;
uniform Material material;
uniform vec3 viewPos;

in VS_OUT 
{
    vec3 Normal;
    vec3 FragPos;
    vec2 TexCoords;
    vec3 LightDirTangent;
    vec3 EyeDirTangent;
    mat3 TBN;
} fs_in;

out vec4 FragColor;

void main()
{
    // Ambient
    vec3 ambient = light.ambient * vec3(texture(material.tDiffuseMap, fs_in.TexCoords));

    // Diffuse 
    vec3 norm = normalize(fs_in.Normal);
    vec3 lightDir = fs_in.TBN * normalize(light.position - fs_in.FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.tDiffuseMap, fs_in.TexCoords));

    //Normal map
    vec3 normal = texture(material.tNormalMap, fs_in.TexCoords).rgb;
    normal = normal * 2.0 - 1.0;   

    // Specular
    vec3 viewDir = fs_in.TBN * normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    vec3 halfwayDir = normalize(viewDir + reflectDir);  
    float spec = pow(max(dot(halfwayDir, viewDir), 0.0), material.fMaxtSpecularMapPower);
    vec3 specular = light.specular * spec * vec3(texture(material.tSpecularMap, fs_in.TexCoords));

    //Final output
    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
}