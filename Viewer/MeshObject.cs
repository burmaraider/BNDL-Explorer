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

        private Texture _tex0; //tDiffuseMap
        private Texture _tex1; //tSpecularMap
        private Texture _tex2; //tNormalMap

        public Shader _lightingShader;


        public MeshObject(byte[] vboData, string texture0, string texture1, string texture2, ref Camera cam)
        {
            _camera = cam;

            _vboData = vboData;

            _tex0 = Texture.LoadFromFile(texture0); //tDiffuseMap
            _tex1 = Texture.LoadFromFile(texture1); //tSpecularMap
            _tex2 = Texture.LoadFromFile(texture2); //tNormalMap

            SetupShaderAndObject();

        }

        public void SetupShaderAndObject()
        {

            _lightingShader = new Shader("Viewer/Shaders/shader.vert", "Viewer/Shaders/lighting.frag");


            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();


            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vboData.Length * sizeof(byte), _vboData, BufferUsageHint.StaticDraw);


            _lightingShader.Use();

            var position = _lightingShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(position);
            GL.VertexAttribPointer(position, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 0);

            var normals = _lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normals);
            GL.VertexAttribPointer(normals, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 3 * sizeof(float));

            var texCoords = _lightingShader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoords);
            GL.VertexAttribPointer(texCoords, 2, VertexAttribPointerType.Float, false, 11 * sizeof(float), 6 * sizeof(float));

            var tangents = _lightingShader.GetAttribLocation("aTangents");
            GL.EnableVertexAttribArray(tangents);
            GL.VertexAttribPointer(tangents, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 8 * sizeof(float));

            GL.BindVertexArray(_vao);
        }



        public void RenderObject(Matrix4 newPos, ref List<PointLight> light)
        {
            GL.BindVertexArray(_vao);

            _tex0.Use(TextureUnit.Texture0); //tDiffuseMap
            _tex1.Use(TextureUnit.Texture1); //tSpecularMap
            _tex2.Use(TextureUnit.Texture2); //tNormalMap
            _lightingShader.Use();

            _lightingShader.SetMatrix4("model", Matrix4.Identity * newPos);
            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingShader.SetVector3("viewPos", _camera.Position);

            _lightingShader.SetInt("material.tDiffuseMap", 0);
            _lightingShader.SetInt("material.tSpecularMap", 1);
            _lightingShader.SetInt("material.tNormalMap", 2);
            _lightingShader.SetVector3("material.tSpecularMap", new Vector3(0.5f,0.5f,0.5f));
            _lightingShader.SetFloat("material.fMaxtSpecularMapPower", 15f);


            for (int i = 0; i < light.Count; i++)
            {
                _lightingShader.SetVector3($"light[{i}].position", light[i].Position);
                _lightingShader.SetVector3($"light[{i}].ambient", light[i].AmbientColor);
                _lightingShader.SetVector3($"light[{i}].diffuse", light[i].Color);
                _lightingShader.SetVector3($"light[{i}].specular", light[i].SpecularColor);
                _lightingShader.SetFloat($"light[{i}].radius", light[i].Radius);
            }


            GL.DrawArrays(PrimitiveType.Triangles, 0, _vboData.Length / 3);
        }

        public void RenderObject(ref Shader shader, ref List<PointLight> light)
        {
            GL.BindVertexArray(_vao);

            _tex0.Use(TextureUnit.Texture0); //tDiffuseMap
            _tex1.Use(TextureUnit.Texture1); //tSpecularMap
            _tex2.Use(TextureUnit.Texture2); //tNormalMap
            _lightingShader.Use();

            _lightingShader.SetMatrix4("model", Matrix4.Identity);
            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingShader.SetVector3("viewPos", _camera.Position);

            _lightingShader.SetInt("material.tDiffuseMap", 0);
            _lightingShader.SetInt("material.tSpecularMap", 1);
            _lightingShader.SetInt("material.tNormalMap", 2);
            _lightingShader.SetVector3("material.tSpecularMap", new Vector3(0.5f, 0.5f, 0.5f));
            _lightingShader.SetFloat("material.fMaxtSpecularMapPower", 32f);
            _lightingShader.SetInt("totalLights", light.Count);
            for (int i = 0; i < light.Count; i++)
            {
                _lightingShader.SetVector3("light.position[" + i + "]", light[i].Position);
                _lightingShader.SetVector3("light.ambient[" + i + "]", light[i].AmbientColor);
                _lightingShader.SetVector3("light.diffuse[" + i + "]", light[i].Color);
                _lightingShader.SetVector3("light.specular[" + i + "]", light[i].SpecularColor);
                _lightingShader.SetFloat("light.radius[" + i + "]", light[i].Radius);
                _lightingShader.SetFloat("light.intensity[" + i + "]", light[i].Intensity);
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