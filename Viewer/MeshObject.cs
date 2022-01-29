using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Graphics;
using OpenTK;

namespace BNDL_Explorer.Viewer
{
    public class MeshObject
    {
        private Camera _camera;

        public string _name;

        public Vector3 _position;

        private byte[] _vboData;
        private int _vao, _vbo;

        //private Texture _tex0; //tDiffuseMap
        //private Texture _tex1; //tSpecularMap
        //private Texture _tex2; //tNormalMap
        //
        //public Shader _lightingShader;

        private Material _material;

        public Material Material { get { return _material; } }


        public MeshObject(byte[] vboData, string texture0, string texture1, string texture2, ref Camera cam)
        {
            _camera = cam;

            _vboData = vboData;

            //_tex0 = Texture.LoadFromFileDDS(texture0); //tDiffuseMap
            //_tex1 = Texture.LoadFromFileDDS(texture1); //tSpecularMap
            //_tex2 = Texture.LoadFromFileDDS(texture2); //tNormalMap

            _material = new Material("shader", "lighting", texture0, texture1, texture2, Material.ShadingType.blinn, 16f);

            SetupShaderAndObject();

        }

        public MeshObject(byte[] vboData, ref Camera cam)
        {
            _camera = cam;

            _vboData = vboData;

            //_tex0 = Texture.LoadFromFileDDS(texture0); //tDiffuseMap
            //_tex1 = Texture.LoadFromFileDDS(texture1); //tSpecularMap
            //_tex2 = Texture.LoadFromFileDDS(texture2); //tNormalMap

            _material = new Material();

            SetupShaderAndObject();

        }


        public void SetupShaderAndObject()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vboData.Length * sizeof(byte), _vboData, BufferUsageHint.StaticDraw);

            _material.Shader.Use();

            var position = _material.Shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(position);
            GL.VertexAttribPointer(position, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 0);

            var normals = _material.Shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normals);
            GL.VertexAttribPointer(normals, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 3 * sizeof(float));

            var texCoords = _material.Shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoords);
            GL.VertexAttribPointer(texCoords, 2, VertexAttribPointerType.Float, false, 11 * sizeof(float), 6 * sizeof(float));

            var tangents = _material.Shader.GetAttribLocation("aTangents");
            GL.EnableVertexAttribArray(tangents);
            GL.VertexAttribPointer(tangents, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 8 * sizeof(float));

            GL.BindVertexArray(_vao);
        }



        public void RenderObject(Matrix4 newPos, ref List<PointLight> light)
        {
            GL.BindVertexArray(_vao);

            _material.TDiffuseMap.Use(TextureUnit.Texture0);
            _material.TSpecularMap.Use(TextureUnit.Texture1);
            _material.TNormalMap.Use(TextureUnit.Texture2);
            _material.Shader.Use();

            _material.Shader.SetMatrix4("model", Matrix4.Identity * newPos);
            _material.Shader.SetMatrix4("view", _camera.GetViewMatrix());
            _material.Shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _material.Shader.SetVector3("viewPos", _camera.Position);

            _material.Shader.SetInt("material.tDiffuseMap", 0);
            _material.Shader.SetInt("material.tSpecularMap", 1);
            _material.Shader.SetInt("material.tNormalMap", 2);
            _material.Shader.SetFloat("material.fMaxtSpecularMapPower", _material.SpecularPower);
            _material.Shader.SetInt("material.blinn", (int)_material.Shading);


            for (int i = 0; i < light.Count; i++)
            {
                _material.Shader.SetVector3($"light[{i}].position", light[i].Position);
                _material.Shader.SetVector3($"light[{i}].ambient", light[i].AmbientColor);
                _material.Shader.SetVector3($"light[{i}].diffuse", light[i].Color);
                _material.Shader.SetVector3($"light[{i}].specular", light[i].SpecularColor);
                _material.Shader.SetFloat($"light[{i}].radius", light[i].Radius);
            }

            GL.DrawArrays(PrimitiveType.Triangles, 0, _vboData.Length / 3);
        }

        public double? IntersectsRay(Vector3 rayDirection, Vector3 rayOrigin)
        {
            var radius = 1.5f;
            var difference = _position - rayDirection;
            var differenceLengthSquared = difference.LengthSquared;
            var sphereRadiusSquared = radius * radius;
            if (differenceLengthSquared < sphereRadiusSquared)
            {
                return 0d;
            }
            var distanceAlongRay = Vector3.Dot(rayDirection, difference);
            if (distanceAlongRay < 0)
            {
                return null;
            }
            var dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;
            var result = (dist < 0) ? null : distanceAlongRay - (double?)Math.Sqrt(dist);
            return result;
        }
    }
}