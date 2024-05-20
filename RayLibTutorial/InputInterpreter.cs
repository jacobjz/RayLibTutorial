using Raylib_cs;
using System.Numerics;

namespace HelloWorld
{
    public class InputInterpreter
    {
        public void InterpretKeys(Game game)
        {
            if (Raylib.IsKeyDown(KeyboardKey.LeftControl))
            {
                if (Raylib.IsKeyPressed(KeyboardKey.U))
                {
                    game.CurrentState.Options.ShowUi = !game.CurrentState.Options.ShowUi;
                }
            }
        }

        public Camera2D InterpretMouse(Camera2D camera)
        {
            // Translate based on mouse right click
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 delta = Raylib.GetMouseDelta();
                delta = Vector2.Multiply(delta, -1.0f / camera.Zoom);
                camera.Target = Vector2.Add(camera.Target, delta);
            }

            float wheel = Raylib.GetMouseWheelMove();
            if (wheel != 0)
            {
                // Get the world point that is under the mouse
                Vector2 mouseWorldPos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);

                // Set the offset to where the mouse is
                camera.Offset = Raylib.GetMousePosition();

                // Set the target to match, so that the camera maps the world space point 
                // under the cursor to the screen space point under the cursor at any zoom
                camera.Target = mouseWorldPos;

                // Zoom increment
                float scaleFactor = 1.0f + (0.125f * Math.Abs(wheel));
                if (wheel < 0) scaleFactor = 1.0f / scaleFactor;
                camera.Zoom = Math.Clamp(camera.Zoom * scaleFactor, 0, 64.0f);
            }
            return camera;
        }
    }
}