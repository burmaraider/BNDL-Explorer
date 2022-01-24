using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;


namespace BNDL_Explorer.Viewer
{

    public class TextObject
    {
        private static string CharSheet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-=_+[]{}\\|;:'\".,<>/?`~ ";

        private float[] _quad;
        private readonly uint[] _indices =
        {
            3, 1, 0,
            3, 2, 1
        };
        private List<float[]> alphaQuad = new List<float[]>();
        private List<int> _vertexBufferObject = new List<int>();
        private List<int> _vertexArrayObject = new List<int>();
        private List<int> _elementBufferObject = new List<int>();
        private Shader _shader = new Shader("Viewer/Shaders/screen.vert", "Viewer/Shaders/screen.frag");
        private Texture _texture;
        private NativeWindow _window;

        private float charWidth;
        private float charHeight;

        int vao;
        int vbo;
        int ebo;

        public TextObject(NativeWindow window)
        {
            _window = window;
            _texture = Texture.LoadFromFile("Viewer/Textures/char.png");

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            for (int i = 0; i < CharSheet.Length; i++)
            {
                float h = 16f / (_window.Size.Height / 2f);
                float h2 = ((_window.Size.Height / 2f) / 16f) / 4f;
                float w = 8f / (_window.Size.Width / 2f);
                float w2 = ((_window.Size.Width / 2f) / 8f) /4f;

                charWidth = w2;
                charHeight = h2;

                float x1 = (float)(8f / 766f) * i;
                float x2 = (float)(8f / 766f) * (i + 1);

                _quad = new float[]
                {
                    //Position        UV's
                     h2/2,  w2/2, 0.0f, x2, 1.0f, // top right
                     h2/2, -w2/2, 0.0f, x2, 0.0f, // bottom right
                    -h2/2, -w2/2, 0.0f, x1, 0.0f, // bottom left
                    -h2/2,  w2/2, 0.0f, x1, 1.0f  // top left
                };

                alphaQuad.Add(_quad);

                //_vertexArrayObject.Add(GL.GenVertexArray());
                //GL.BindVertexArray(_vertexArrayObject[i]);
                //
                //_vertexBufferObject.Add(GL.GenBuffer());
                //GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject[i]);
                //GL.BufferData(BufferTarget.ArrayBuffer, alphaQuad[i].Length * sizeof(float), alphaQuad[i], BufferUsageHint.DynamicDraw);
                //
                //_elementBufferObject.Add(GL.GenBuffer());
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject[i]);
                //GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.DynamicDraw);
                //
                //_shader.Use();
                //
                //var vertexLocation = _shader.GetAttribLocation("aPosition");
                //GL.EnableVertexAttribArray(vertexLocation);
                //GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
                //
                //var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
                //GL.EnableVertexAttribArray(texCoordLocation);
                //GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            }

            
        }

        public void Reset()
        {

            for (int i = 0; i < CharSheet.Length; i++)
            {
                GL.DeleteVertexArray(_vertexArrayObject[i]);
                GL.DeleteBuffer(_vertexBufferObject[i]);
                GL.DeleteBuffer(_elementBufferObject[i]);
            }

            alphaQuad = null;
            _vertexBufferObject = null;
            _vertexArrayObject = null;
            _elementBufferObject = null;
            GC.Collect();

            alphaQuad = new List<float[]>();
            _vertexBufferObject = new List<int>();
            _vertexArrayObject = new List<int>();
            _elementBufferObject = new List<int>();

            for (int i = 0; i < CharSheet.Length; i++)
            {
                float h2 = ((_window.Size.Height / 2f) / 16f) / 4f;
                float w2 = ((_window.Size.Width / 2f) / 8f) / 4f;

                charWidth = w2;
                charHeight = h2;

                float x1 = (float)(8f / 766f) * i;
                float x2 = (float)(8f / 766f) * (i + 1);

                _quad = new float[]
                {
                    //Position        UV's
                     h2/2,  w2/2, 0.0f, x2, 1.0f, // top right
                     h2/2, -w2/2, 0.0f, x2, 0.0f, // bottom right
                    -h2/2, -w2/2, 0.0f, x1, 0.0f, // bottom left
                    -h2/2,  w2/2, 0.0f, x1, 1.0f  // top left
                };

                alphaQuad.Add(_quad);

                _vertexArrayObject.Add(GL.GenVertexArray());
                GL.BindVertexArray(_vertexArrayObject[i]);

                _vertexBufferObject.Add(GL.GenBuffer());
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject[i]);
                GL.BufferData(BufferTarget.ArrayBuffer, alphaQuad[i].Length * sizeof(float), alphaQuad[i], BufferUsageHint.DynamicDraw);

                _elementBufferObject.Add(GL.GenBuffer());
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject[i]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.DynamicDraw);

                _shader.Use();

                var vertexLocation = _shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            }

        }

        public void DrawString2(string str, float x, float y)
        {
            int index = 0;
            foreach (char c in str)
            {
                DrawCharacter(c, (float)(charHeight * index) + x, (charHeight * 2f) + y);
                index++;
            }
        }
        public void DrawCharacter(char c, float x, float y)
        {
            int i = CharSheet.IndexOf(c);

            // Convert to pixel coords
            x = x / (_window.Size.Width / 2f) - 1f;
            y = (_window.Size.Height - y - charHeight * 6f) / (_window.Size.Height / 2f) - 1f;
            Vector3 Position = new Vector3(x, y, 0);

            // Update shader with quad position
            Matrix4 ortho = Matrix4.CreateOrthographic(_window.Size.Width, _window.Size.Height, -1f, 1f);
            Matrix4 modelMatrix = ortho * Matrix4.CreateScale(1.0f) * Matrix4.CreateTranslation(Position);

            GL.BindVertexArray(_vertexArrayObject[i]);

            _texture.Use(TextureUnit.Texture0);
            _shader.Use();
            _shader.SetMatrix4("projection", modelMatrix);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void DrawString(string inputString, float x, float y)
        {
            List<float> vaoArray = new List<float>();
            int i = 0;

            // Convert to pixel coords
            x = x / (_window.Size.Width / 2f) - 1f;
            y = (_window.Size.Height - y - charHeight * 6f) / (_window.Size.Height / 2f) - 1f;
            Vector3 Position = new Vector3(x, y, 0);

            foreach (char c in inputString)
            {
                var index = CharSheet.IndexOf(c);

                vaoArray.Add(alphaQuad[index][0] + (charHeight * i));
                vaoArray.Add(alphaQuad[index][1]);
                vaoArray.Add(alphaQuad[index][2]);
                vaoArray.Add(alphaQuad[index][3]);
                vaoArray.Add(alphaQuad[index][4]);

                vaoArray.Add(alphaQuad[index][5] + (charHeight * i));
                vaoArray.Add(alphaQuad[index][6]);
                vaoArray.Add(alphaQuad[index][7]);
                vaoArray.Add(alphaQuad[index][8]);
                vaoArray.Add(alphaQuad[index][9]);

                vaoArray.Add(alphaQuad[index][10] + (charHeight * i));
                vaoArray.Add(alphaQuad[index][11]);
                vaoArray.Add(alphaQuad[index][12]);
                vaoArray.Add(alphaQuad[index][13]);
                vaoArray.Add(alphaQuad[index][14]);

                vaoArray.Add(alphaQuad[index][15] + (charHeight * i));
                vaoArray.Add(alphaQuad[index][16]);
                vaoArray.Add(alphaQuad[index][17]);
                vaoArray.Add(alphaQuad[index][18]);
                vaoArray.Add(alphaQuad[index][19]);
                i++;
            }

            int[] indices = new int[inputString.Length * 6];

            for (int t = 0; t < inputString.Length; t++)
            {
                indices[t * 6] = (t * 4) + 3;
                indices[(t * 6) + 1] = (t * 4) + 1;
                indices[(t * 6) + 2] = (t * 4);
                
                indices[(t * 6) + 3] = (t * 4) + 3;
                indices[(t * 6) + 4] = (t * 4) + 2;
                indices[(t * 6) + 5] = (t * 4) + 1;
            }

            // Update shader with quad position
            Matrix4 ortho = Matrix4.CreateOrthographic(_window.Size.Width, _window.Size.Height, -1f, 1f);
            Matrix4 modelMatrix = ortho * Matrix4.CreateScale(1.0f) * Matrix4.CreateTranslation(Position);


            var data = vaoArray.ToArray();

            //var vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            //var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.DynamicDraw);

            //var ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

            _shader.Use();
            _texture.Use(TextureUnit.Texture0);
            _shader.SetMatrix4("projection", modelMatrix);

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));


            GL.Disable(EnableCap.DepthTest);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
