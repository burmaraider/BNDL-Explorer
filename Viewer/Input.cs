using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNDL_Explorer.Viewer
{
    public class Input
    {
        private readonly Renderer r;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        public Input(Renderer gameWindow)
        {
            r = gameWindow;
        }

        public void ProcessControls(ref FrameEventArgs e)
        {
            if (!r.Focused) // Check to see if the window is focused
            {
                return;
            }


            float cameraSpeed = 1.5f;
            const float _mouseSensitivity = 0.2f;

            if (Mouse.GetState().IsButtonDown(MouseButton.Right)) //Enable Viewport Interaction
            {
                r.CursorGrabbed = true;
                r.CursorVisible = false;

                if (Keyboard.GetState().IsKeyDown(Key.ShiftLeft)) //Go faster
                    cameraSpeed *= 2.0f;
                else
                    cameraSpeed = 1.5f;

                if (Keyboard.GetState().IsKeyDown(Key.W))
                    r.GetCamera.Position += r.GetCamera.Front * cameraSpeed * (float)e.Time; // Forward

                if (Keyboard.GetState().IsKeyDown(Key.S))
                    r.GetCamera.Position -= r.GetCamera.Front * cameraSpeed * (float)e.Time; // Backwards

                if (Keyboard.GetState().IsKeyDown(Key.A))
                    r.GetCamera.Position -= r.GetCamera.Right * cameraSpeed * (float)e.Time; // Left

                if (Keyboard.GetState().IsKeyDown(Key.D))
                    r.GetCamera.Position += r.GetCamera.Right * cameraSpeed * (float)e.Time; // Right

                if (Keyboard.GetState().IsKeyDown(Key.Space))
                    r.GetCamera.Position += r.GetCamera.Up * cameraSpeed * (float)e.Time; // Up

                if (Keyboard.GetState().IsKeyDown(Key.C))
                    r.GetCamera.Position -= r.GetCamera.Up * cameraSpeed * (float)e.Time; // Down

                if (Keyboard.GetState().IsKeyDown(Key.E)) //Up World Space
                    r.GetCamera.Move(0f, 0f, 0.01f * (float)e.Time);

                if (Keyboard.GetState().IsKeyDown(Key.Q)) //Down World Space
                    r.GetCamera.Move(0f, 0f, -0.01f * (float)e.Time);

                if (Keyboard.GetState().IsKeyDown(Key.KeypadPlus))
                    r.GetCamera.Fov += 0.1f * (float)e.Time;

                if (Keyboard.GetState().IsKeyDown(Key.KeypadMinus))
                    r.GetCamera.Fov -= 0.1f * (float)e.Time;

                if (_firstMove) // This bool variable is initially set to true.
                {
                    _lastPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    _firstMove = false;
                }
                else
                {
                    var deltaX = Mouse.GetState().X - _lastPos.X;
                    var deltaY = Mouse.GetState().Y - _lastPos.Y;
                    _lastPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                    r.GetCamera.Yaw += deltaX * _mouseSensitivity;
                    r.GetCamera.Pitch -= deltaY * _mouseSensitivity;
                }
            }
            else
            {
                r.CursorGrabbed = false;
                r.CursorVisible = true;
            }
        }
    }
}
