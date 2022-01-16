using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using System.Collections.Generic;
using FileFormatWavefront;
using OpenTK.Graphics;
using System.Timers;

namespace BNDL_Explorer.Viewer
{
    public class Renderer : GameWindow
    {
        private Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);
        private Camera _camera;
        private Input _input;
        private TextObject _textRenderer;
        public List<MeshObject> _meshList = new List<MeshObject>();
        private float _angle;

        public Renderer(int width, int height, GraphicsMode mode, string title) : base(width, height, mode, title) { }
        public Camera GetCamera { get { return _camera; } }

        protected override void OnLoad(EventArgs e)
        {
            _textRenderer = new TextObject(this);
            _input = new Input(this);

            //this.WindowBorder = WindowBorder.Fixed;

            List<float> vbo = new List<float>();
            List<float> box = new List<float>();
            var result = FileFormatObj.Load("docks.obj", false);
            foreach (var face in result.Model.UngroupedFaces)
            {
                for (int i = 0; i < 3; i++)
                {
                    vbo.Add(result.Model.Vertices[face.Indices[i].vertex].x);
                    vbo.Add(result.Model.Vertices[face.Indices[i].vertex].y);
                    vbo.Add(result.Model.Vertices[face.Indices[i].vertex].z);

                    vbo.Add(result.Model.Normals[(int)face.Indices[i].normal].x);
                    vbo.Add(result.Model.Normals[(int)face.Indices[i].normal].y);
                    vbo.Add(result.Model.Normals[(int)face.Indices[i].normal].z);

                    vbo.Add(result.Model.Uvs[(int)face.Indices[i].uv].u);
                    vbo.Add(result.Model.Uvs[(int)face.Indices[i].uv].v);
                }
            }

            result = FileFormatObj.Load("box.obj", false);
            foreach (var face in result.Model.UngroupedFaces)
            {
                for (int i = 0; i < 3; i++)
                {
                    box.Add(result.Model.Vertices[face.Indices[i].vertex].x);
                    box.Add(result.Model.Vertices[face.Indices[i].vertex].y);
                    box.Add(result.Model.Vertices[face.Indices[i].vertex].z);
                    
                    box.Add(result.Model.Normals[(int)face.Indices[i].normal].x);
                    box.Add(result.Model.Normals[(int)face.Indices[i].normal].y);
                    box.Add(result.Model.Normals[(int)face.Indices[i].normal].z);
                    
                    box.Add(result.Model.Uvs[(int)face.Indices[i].uv].u);
                    box.Add(result.Model.Uvs[(int)face.Indices[i].uv].v);
                }
            }


            _camera = new Camera(Vector3.UnitZ * 3, (float)Size.Width / (float)Size.Width);

            _meshList.Add(new MeshObject(vbo.ToArray(), "Viewer/Textures/Grid_D.png", "Viewer/Textures/Grid_S.png", "Viewer/Textures/FloorStone01_N.png", ref _camera));
            _meshList.Add(new MeshObject(box.ToArray(), "Viewer/Textures/container2.png", "Viewer/Textures/container2_specular.png", "Viewer/Textures/FloorStone01_N.png", ref _camera));
            _meshList.Add(new MeshObject(box.ToArray(), "Viewer/Textures/FloorStone01_D2.png", "Viewer/Textures/FloorStone01_S2.png", "Viewer/Textures/FloorStone01_N.png", ref _camera));
            _meshList.Add(new MeshObject(box.ToArray(), "Viewer/Textures/FloorStone01_D2.png", "Viewer/Textures/FloorStone01_S2.png", "Viewer/Textures/FloorStone01_N.png", ref _camera));


            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);

            CursorGrabbed = true;
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderMeshes();
            DebugInfo();

            SwapBuffers();
        }

        private void RenderMeshes()
        {
            int i = 0;
            foreach (MeshObject mesh in _meshList)
            {
                var transform = Matrix4.Identity;
                transform *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_angle)); //rotate
                //transform = transform * Matrix4.CreateScale((float)i); // scale

                transform *= Matrix4.CreateTranslation(i * 2, 0.0f, 0.0f); //move to the right just a bit
                mesh.RenderObject(transform, ref _lightPos);
                i++;
            }
        }

        private void DebugInfo()
        {
            _textRenderer.DrawString(String.Format("Position: {0:0.000} , {1:0.000} , {2:0.000}", _camera.Position.X, _camera.Position.Y, _camera.Position.Z), 2f, 0f);
            _textRenderer.DrawString(String.Format("Camera FOV: {0:0}", _camera.Fov), 2f, 32f);
            _textRenderer.DrawString(String.Format("Camera Aspect Ratio: {0:0.00}", _camera.AspectRatio), 2f, 64f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _input.ProcessControls(ref e);
            
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _camera.Fov += e.Y;
            base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.Width, Size.Height);

            _camera.AspectRatio = (Size.Width / (float)Size.Height);
            _textRenderer.Reset();
        }
    }
}