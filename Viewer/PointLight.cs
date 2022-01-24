using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNDL_Explorer.Viewer
{
    public class PointLight : ILight
    {
        
        public PointLight()
        {
            Position = new Vector3(2.0f, 0.0f, 2.0f);
            AmbientColor = new Vector3(0.2f);
            Color = new Vector3(0.5f);
            SpecularColor = new Vector3(0.6f);
            Radius = 9.0f;
            Intensity = 1.0f;
        }

        public PointLight(Vector3 position, Vector3 color, Vector3 specularColor, float radius)
        {
            Position = position;
            AmbientColor = new Vector3(0.2f);
            Color = color;
            SpecularColor = specularColor;
            Radius = radius;
            Intensity = 1.0f;
        }


        public Vector3 Position { get; set; }
        public Vector3 AmbientColor { get; set; }
        public Vector3 Color { get; set; }
        public Vector3 SpecularColor { get; set; }
        public float Radius { get; set; }
        public float Intensity { get; set; }

    }
}
