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

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;

in vec2 TexCoords;

void main()
{
    // ambient
    vec3 ambient = light.ambient * vec3(texture(material.tDiffuseMap, TexCoords));

    // Diffuse 
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.tDiffuseMap, TexCoords));

    // Specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.fMaxtSpecularMapPower);
    vec3 tSpecularMap = light.specular * spec * vec3(texture(material.tSpecularMap, TexCoords));

    //Normal - todo
    vec3 normalMap = vec3(texture(material.tNormalMap, TexCoords));

    normalMap *= 0.001;


    vec3 result = ambient + diffuse + tSpecularMap + normalMap;
    FragColor = vec4(result, 1.0);
}