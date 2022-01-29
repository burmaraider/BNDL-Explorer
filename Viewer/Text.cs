using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;


namespace BNDL_Explorer.Viewer
{

    public class TextObject
    {
        private static readonly string CharSheet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-=_+[]{}\\|;:'\".,<>/?`~ ";

        private readonly List<float[]> alphaQuad = new List<float[]>();
        private readonly Shader _shader = new Shader("Viewer/Shaders/screen.vert", "Viewer/Shaders/screen.frag");
        private readonly Texture _texture;
        private readonly NativeWindow _window;

        public float charWidth;
        public float cWidth;

        private readonly int vao;
        private readonly int vbo;
        private readonly int ebo;

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
                cWidth = h2;

                float x1 = (float)(8f / 766f) * i;
                float x2 = (float)(8f / 766f) * (i + 1);

                alphaQuad.Add(new float[]
                {
                    //Position        UV's
                     h2/2,  w2/2, 0.0f, x2, 1.0f, // top right
                     h2/2, -w2/2, 0.0f, x2, 0.0f, // bottom right
                    -h2/2, -w2/2, 0.0f, x1, 0.0f, // bottom left
                    -h2/2,  w2/2, 0.0f, x1, 1.0f  // top left
                });
            } 
        }

        public void DrawString(string inputString, float x, float y, float scale = 1.0f)
        {
            List<float> vaoArray = new List<float>();
            int i = 0;

            // Convert to pixel coords
            x = x / (_window.Size.Width / 2f) - 1f;
            y = (_window.Size.Height - y - charWidth * 2 * scale) / (_window.Size.Height / 2f) - 1f;
            Vector3 Position = new Vector3(x, y, 0);

            foreach (char c in inputString)
            {
                var index = CharSheet.IndexOf(c);

                vaoArray.Add(alphaQuad[index][0] + (cWidth * i));
                vaoArray.Add(alphaQuad[index][1]);
                vaoArray.Add(alphaQuad[index][2]);
                vaoArray.Add(alphaQuad[index][3]);
                vaoArray.Add(alphaQuad[index][4]);

                vaoArray.Add(alphaQuad[index][5] + (cWidth * i));
                vaoArray.Add(alphaQuad[index][6]);
                vaoArray.Add(alphaQuad[index][7]);
                vaoArray.Add(alphaQuad[index][8]);
                vaoArray.Add(alphaQuad[index][9]);

                vaoArray.Add(alphaQuad[index][10] + (cWidth * i));
                vaoArray.Add(alphaQuad[index][11]);
                vaoArray.Add(alphaQuad[index][12]);
                vaoArray.Add(alphaQuad[index][13]);
                vaoArray.Add(alphaQuad[index][14]);

                vaoArray.Add(alphaQuad[index][15] + (cWidth * i));
                vaoArray.Add(alphaQuad[index][16]);
                vaoArray.Add(alphaQuad[index][17]);
                vaoArray.Add(alphaQuad[index][18]);
                vaoArray.Add(alphaQuad[index][19]);
                i++;
            }


            //Calculate our indices
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
            Matrix4 modelMatrix = ortho * Matrix4.CreateScale(scale) * Matrix4.CreateTranslation(Position);

            //bind our vao and buffer data
            var data = vaoArray.ToArray();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.DynamicDraw);

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

            
            GL.Disable(EnableCap.DepthTest);//disable depth testing so it render above everything else
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.Enable(EnableCap.DepthTest);//re enable depth testing
        }

        public void Reset()
        {
            for (int i = 0; i < CharSheet.Length; i++)
            {
                float h = 16f / (_window.Size.Height / 2f);
                float h2 = ((_window.Size.Height / 2f) / 16f) / 4f;
                float w = 8f / (_window.Size.Width / 2f);
                float w2 = ((_window.Size.Width / 2f) / 8f) / 4f;

                charWidth = w2;
                cWidth = h2;

                float x1 = (float)(8f / 766f) * i;
                float x2 = (float)(8f / 766f) * (i + 1);

                alphaQuad.Add(new float[]
                {
                    //Position        UV's
                     h2/2,  w2/2, 0.0f, x2, 1.0f, // top right
                     h2/2, -w2/2, 0.0f, x2, 0.0f, // bottom right
                    -h2/2, -w2/2, 0.0f, x1, 0.0f, // bottom left
                    -h2/2,  w2/2, 0.0f, x1, 1.0f  // top left
                });
            }
        }
    }
}
