#version 330 compatibility

struct Material {
    sampler2D tDiffuseMap;
    sampler2D tSpecularMap;
    sampler2D tNormalMap;
    float     fMaxtSpecularMapPower;
    int blinn;
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
    mat3 TBN;
} fs_in;

out vec4 FragColor;

vec3 tDiffuseMap = texture(material.tDiffuseMap, fs_in.TexCoords).rgb;
vec4 tSpecularMap = texture(material.tSpecularMap, fs_in.TexCoords).rgba;
vec3 tNormalMap = texture(material.tNormalMap, fs_in.TexCoords).rgb;

vec3 CalcPointLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
    // Ambient
    vec3 ambient = light[0].ambient * tDiffuseMap;
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
    vec3 diffuse = light.diffuse * att * diff * tDiffuseMap;
    //Normal map
    vec3 normal = tNormalMap * 2.0 - 1.0;   

    vec3 specular = vec3(0);
    if(material.blinn == 1)
    {
        // Specular - Blinn-Phong
        vec3 reflectDir = reflect(-lightDir, normal);
        vec3 halfwayDir = normalize(viewDir + lightDir);  
        float spec = pow(max(dot(normal, halfwayDir), 0.0), material.fMaxtSpecularMapPower);
        specular = (light.specular * 0.5) * (att * tSpecularMap.a ) * spec * tSpecularMap.rgb;
    }
    else
    {
        // Specular
        vec3 reflectDir = reflect(-lightDir, normal);
        vec3 halfwayDir = normalize(viewDir + reflectDir);  
        float spec = pow(max(dot(halfwayDir, viewDir), 0.0), material.fMaxtSpecularMapPower);
        specular = light.specular * (att * tSpecularMap.a * 1.5 )* spec * tSpecularMap.rgb;
    }
    //Final output
    vec3 result = diffuse + specular;

    return result;
}