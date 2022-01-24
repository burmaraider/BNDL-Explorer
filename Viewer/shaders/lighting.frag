#version 330 compatibility

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
    float radius;
    float intensity;
};


uniform Material material;
uniform vec3 viewPos;
uniform int totalLights;
uniform Light light[32];

in VS_OUT 
{
    vec3 Normal;
    vec3 FragPos;
    vec2 TexCoords;
    //vec3 LightDirTangent;
    //vec3 EyeDirTangent;
    mat3 TBN;
} fs_in;

out vec4 FragColor;

vec3 CalcPointLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
// Ambient
    vec3 ambient = light[0].ambient * vec3(texture(material.tDiffuseMap, fs_in.TexCoords));

    vec3 norm = normalize(fs_in.Normal);
    vec3 viewDir = fs_in.TBN * normalize(viewPos - fs_in.FragPos);
    vec3 result = vec3(0);
    for(int i = 0; i < 32; i++)
        result += CalcPointLight(light[i], norm, fs_in.FragPos, viewDir);

    FragColor = vec4(ambient + result, 1.0);
}

vec3 CalcPointLight(Light light, vec3 normalIn, vec3 fragPos, vec3 viewDir)
{
    // Diffuse 
    vec3 lightDir = fs_in.TBN * normalize(light.position - fs_in.FragPos);
    vec3 lightDir2 = normalize(light.position - fs_in.FragPos);

    //Calculate falloff and attenuation
    float diff = max(dot(normalIn, lightDir2), 0.0);
    float dist = distance(light.position, fs_in.FragPos);
    float att = clamp(1.0 - dist*dist/(light.radius*light.radius), 0.0, 1.0);
    vec3 diffuse = light.diffuse * att * diff * vec3(texture(material.tDiffuseMap, fs_in.TexCoords));

    //Normal map
    vec3 normal = texture(material.tNormalMap, fs_in.TexCoords).rgb;
    normal = normal * 2.0 - 1.0;   

    // Specular
    vec3 reflectDir = reflect(-lightDir, normal);
    vec3 halfwayDir = normalize(viewDir + reflectDir);  
    float spec = pow(max(dot(halfwayDir, viewDir), 0.0), material.fMaxtSpecularMapPower);
    vec3 specular = light.specular * (att * 0.5 )* spec * vec3(texture(material.tSpecularMap, fs_in.TexCoords));

    //Final output
    vec3 result = diffuse + specular;

    return result;
}