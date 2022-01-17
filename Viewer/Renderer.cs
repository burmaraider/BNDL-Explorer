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
        //public Vector3 _lightPos = new Vector3(20.2f, 1.0f, 20.0f);
        public Light _light = new Light();
        private Camera _camera;
        private Input _input;
        private TextObject _textRenderer;
        public List<MeshObject> _meshList = new List<MeshObject>();
        public float _xAngle;
        public float _yAngle;
        public bool _angleallowed;

        public string selectedObject = String.Empty;

        public Renderer(int width, int height, GraphicsMode mode, string title) : base(width, height, mode, title) { }
        public Camera GetCamera { get { return _camera; } }


        private void OnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (mouseButtonEventArgs.Button != MouseButton.Left)
                return;
            PickObjectOnScreen(mouseButtonEventArgs.X, mouseButtonEventArgs.Y);
        }

        private void PickObjectOnScreen(int mouseX, int mouseY)
        {
            // heavily influenced by: http://antongerdelan.net/opengl/raycasting.html
            // viewport coordinate system
            // normalized device coordinates
            var x = (2f * mouseX) / Width - 1f;
            var y = 1f - (2f * mouseY) / Height;
            var z = 1f;
            var rayNormalizedDeviceCoordinates = new Vector3(x, y, z);

            // 4D homogeneous clip coordinates
            var rayClip = new Vector4(rayNormalizedDeviceCoordinates.X, rayNormalizedDeviceCoordinates.Y, -1f, 1f);

            // 4D eye (camera) coordinates
            var rayEye = _camera.GetProjectionMatrix().Inverted() * rayClip;
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1f, 0f);

            // 4D world coordinates
            var rayWorldCoordinates = (_camera.LookAtMatrix.Inverted() * rayEye).Xyz;
            rayWorldCoordinates.Normalize();
            FindClosestObject(rayWorldCoordinates);
        }

        private void FindClosestObject(Vector3 rayWorldCoordinates)
        {
            double? bestDistance = null;
            MeshObject bestCandidate = null;

            foreach (MeshObject meshObject in _meshList)
            {

                var candidateDistance = meshObject.IntersectsRay(rayWorldCoordinates, _camera.Position);

                if (!candidateDistance.HasValue)
                    continue;
                if (!bestDistance.HasValue)
                {
                    bestDistance = candidateDistance;
                    bestCandidate = meshObject;
                    continue;
                }
                if (candidateDistance < bestDistance)
                {
                    bestDistance = candidateDistance;
                    bestCandidate = meshObject;
                }

            }
            if (bestCandidate != null)
            {
                selectedObject = String.Format("Object Selected is: {0}", bestCandidate._name);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            _textRenderer = new TextObject(this);
            _input = new Input(this);
            _camera = new Camera(Vector3.UnitZ * 3, (float)Size.Width / (float)Size.Width);
            CursorGrabbed = true;

            //Setup Event Handlers, as many as we'd like
            MouseUp += OnMouseUp;

            List<float> vbo = new List<float>();

            //load our meshes from disk
            var result = FileFormatObj.Load("gun.obj", false);
            PrepareVBO(out vbo, result);

            //Add our models so that we can render them
            _meshList.Add(new MeshObject(vbo.ToArray(), "Viewer/Textures/gund.png", "Viewer/Textures/guns.png", "Viewer/Textures/gunn.png", ref _camera));
            _meshList[_meshList.Count - 1]._name = "GUN";

            //We can access our light here and change all the settings
            _light.Color = new Vector3(0.2f, 0.2f, 1.0f);
            _light.SpecularColor = new Vector3(0.3f, 0.3f, 0.5f);

            GLInit();
            base.OnLoad(e);
        }

        private static void GLInit()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);
        }

        /// <summary>
        /// Use our OBJ data and convert it into the format our VBO is expecting
        /// </summary>
        /// <param name="vbo">The list containing all the VBO float values</param>
        /// <param name="result">OBJ data to parse</param>
        private static void PrepareVBO(out List<float> vbo, FileLoadResult<FileFormatWavefront.Model.Scene> result)
        {
            vbo = new List<float>();
            foreach (var face in result.Model.UngroupedFaces)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 v0 = new Vector3(
                                            result.Model.Vertices[face.Indices[0].vertex].x,
                                            result.Model.Vertices[face.Indices[0].vertex].y,
                                            result.Model.Vertices[face.Indices[0].vertex].z);
                    Vector3 v1 = new Vector3(
                                            result.Model.Vertices[face.Indices[1].vertex].x,
                                            result.Model.Vertices[face.Indices[1].vertex].y,
                                            result.Model.Vertices[face.Indices[1].vertex].z);
                    Vector3 v2 = new Vector3(
                                            result.Model.Vertices[face.Indices[2].vertex].x,
                                            result.Model.Vertices[face.Indices[2].vertex].y,
                                            result.Model.Vertices[face.Indices[2].vertex].z);

                    Vector2 uv0 = new Vector2(
                                            result.Model.Uvs[(int)face.Indices[0].uv].u,
                                            result.Model.Uvs[(int)face.Indices[0].uv].v);

                    Vector2 uv1 = new Vector2(
                                            result.Model.Uvs[(int)face.Indices[1].uv].u,
                                            result.Model.Uvs[(int)face.Indices[1].uv].v);
                    Vector2 uv2 = new Vector2(
                                            result.Model.Uvs[(int)face.Indices[2].uv].u,
                                            result.Model.Uvs[(int)face.Indices[2].uv].v);

                    Vector3 deltaPos1 = v1 - v0;
                    Vector3 deltaPos2 = v2 - v0;

                    Vector2 deltaUV1 = uv1 - uv0;
                    Vector2 deltaUV2 = uv2 - uv0;

                    float r = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                    Vector3 tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                    Vector3 bitangent = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;

                    vbo.Add(result.Model.Vertices[face.Indices[i].vertex].x);
                    vbo.Add(result.Model.Vertices[face.Indices[i].vertex].y);
                    vbo.Add(result.Model.Vertices[face.Indices[i].vertex].z);

                    vbo.Add(result.Model.Normals[(int)face.Indices[i].normal].x);
                    vbo.Add(result.Model.Normals[(int)face.Indices[i].normal].y);
                    vbo.Add(result.Model.Normals[(int)face.Indices[i].normal].z);

                    vbo.Add(result.Model.Uvs[(int)face.Indices[i].uv].u);
                    vbo.Add(result.Model.Uvs[(int)face.Indices[i].uv].v);
                    vbo.Add(tangent.X); vbo.Add(tangent.Y); vbo.Add(tangent.Z);
                    vbo.Add(bitangent.X); vbo.Add(bitangent.Y); vbo.Add(bitangent.Z);
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderMeshes();
            DebugInfo();

            SwapBuffers();
        }

        /// <summary>
        /// Render all meshes in the model list, this is very basic
        /// </summary>
        private void RenderMeshes()
        {
            int i = 0;
            foreach (MeshObject mesh in _meshList)
            {
                var transform = Matrix4.Identity;
                transform *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_xAngle)); //rotate
                transform *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_yAngle)); //rotate
                transform = transform * Matrix4.CreateScale(1.0f); // scale

                transform *= Matrix4.CreateTranslation(i * 2, 0.0f, 0.0f); //move each model to the right just a bit
                mesh._position = transform.ExtractTranslation();//set our objects position
                mesh.RenderObject(transform, ref _light);
                i++;
            }
        }

        /// <summary>
        /// Display some debug info, use this to show text on the screen.
        /// </summary>
        private void DebugInfo()
        {
            _textRenderer.DrawString(String.Format("Position: {0:0.000} , {1:0.000} , {2:0.000}", _camera.Position.X, _camera.Position.Y, _camera.Position.Z), 2f, 0f);
            _textRenderer.DrawString(String.Format("Camera FOV: {0:0}", _camera.Fov), 2f, 32f);
            _textRenderer.DrawString(String.Format("Camera Aspect Ratio: {0:0.00}", _camera.AspectRatio), 2f, 64f);
            _textRenderer.DrawString(selectedObject, 2f, 90f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _input.ProcessControls(ref e);

            if(_angleallowed)
                _xAngle += 0.2f;
            //todo: depth sorting
            
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