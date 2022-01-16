﻿using System;
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

        private float[] _vboData;
        private int _vaoModel;
        int _vertexBufferObject;

        private Texture _tex0; //tDiffuseMap
        private Texture _tex1; //tSpecularMap
        private Texture _tex2; //tNormalMap

        Shader _lightingShader;

        public MeshObject(float[] vboData, string texture0, string texture1, string texture2, ref Camera cam)
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
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vboData.Length * sizeof(float), _vboData, BufferUsageHint.StaticDraw);

            _lightingShader = new Shader("Viewer/Shaders/shader.vert", "Viewer/Shaders/lighting.frag");

            {
                _vaoModel = GL.GenVertexArray();
                GL.BindVertexArray(_vaoModel);

                var position = _lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(position);
                GL.VertexAttribPointer(position, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                var normals = _lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normals);
                GL.VertexAttribPointer(normals, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                var texCoords = _lightingShader.GetAttribLocation("aTexCoords");
                GL.EnableVertexAttribArray(texCoords);
                GL.VertexAttribPointer(texCoords, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            }
        }

        public void RenderObject(Matrix4 newPos, ref Vector3 lp)
        {
            GL.BindVertexArray(_vaoModel);

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
            _lightingShader.SetVector3("material.tSpecularMap", new Vector3(0.5f, 0.5f, 0.5f));
            _lightingShader.SetFloat("material.fMaxtSpecularMapPower", 5.0f);

            _lightingShader.SetVector3("light.position", lp);
            _lightingShader.SetVector3("light.ambient", new Vector3(0.2f));
            _lightingShader.SetVector3("light.diffuse", new Vector3(0.5f));
            _lightingShader.SetVector3("light.specular", new Vector3(1.0f));

            GL.DrawArrays(PrimitiveType.Triangles, 0, _vboData.Length / 3);
        }
    }
}