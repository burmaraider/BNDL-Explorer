using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNDL_Explorer.Viewer
{
    public class Light
    {
        private Vector3 _position;
        private Vector3 _ambientColor;
        private Vector3 _diffuseColor;
        private Vector3 _specularColor;
        
        public Light()
        {
            _position = new Vector3(20.0f, 0.0f, 20.0f);
            _ambientColor = new Vector3(0.2f);
            _diffuseColor = new Vector3(0.5f);
            _specularColor = new Vector3(0.6f);
        }

        public Light(Vector3 position, Vector3 color, Vector3 specularColor)
        {
            _position = position;
            _ambientColor = new Vector3(0.2f);
            _diffuseColor = color;
            _specularColor = specularColor;
        }


        public Vector3 Position { get { return _position; } set { _position = value; } }
        public Vector3 AmbientColor { get { return _ambientColor; } set { _ambientColor = value; } }
        public Vector3 Color { get { return _diffuseColor; } set { _diffuseColor = value; } }
        public Vector3 SpecularColor { get { return _specularColor; } set { _specularColor = value; } }

    }
}
