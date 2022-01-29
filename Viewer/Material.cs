using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNDL_Explorer.Viewer
{
    public class Material
    {
        public enum ShadingType
        {
            phong, blinn
        };

        public Shader Shader { get; set; }
        public Texture TDiffuseMap { get; set; }
        public Texture TSpecularMap { get; set; }
        public Texture TNormalMap { get; set; }
        public ShadingType Shading { get; set; }
        public float SpecularPower { get; set; }

        public Material()
        {
            Shader = new Shader("Viewer/shaders/shader.vert", "Viewer/shaders/lighting.frag");
            SpecularPower = 16f;
            Shading = ShadingType.phong;
            TDiffuseMap = Texture.LoadFromFileDDS("Viewer/Textures/Grid_D.dds");
            TSpecularMap = Texture.LoadFromFileDDS("Viewer/Textures/flat_gray.dds");
            TNormalMap = Texture.LoadFromFileDDS("Viewer/Textures/flat_normal_map.dds");
        }

        public Material(string shaderVert, string shaderFrag, string diffuse, string specular, string normal, ShadingType shading = ShadingType.phong, float specularPower = 12f)
        {
            Shader = new Shader($"Viewer/shaders/{shaderVert}.vert", $"Viewer/shaders/{shaderFrag}.frag");
            SpecularPower = specularPower;
            Shading = shading;
            TDiffuseMap = Texture.LoadFromFileDDS($"Viewer/Textures/{diffuse}.dds");
            TSpecularMap = Texture.LoadFromFileDDS($"Viewer/Textures/{specular}.dds");
            TNormalMap = Texture.LoadFromFileDDS($"Viewer/Textures/{normal}.dds");
        }
    }
}
