﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    public static class Input
    {
        private static KeyboardState KeyState;
        private static KeyboardState PreviousKeyState;
        private static MouseState MouseState;
        private static MouseState PreviousMouseState;
        private static bool IsMouseWithinViewport;
        public static void Update(BackBuffer backBuffer)
        {
            UpdateKeyboardState();
            UpdateMouseState(backBuffer);
        }

        public static Vector2 MousePosition { get; private set; }

        public static bool IsKeyDown(Keys key) 
        {
            return KeyState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return KeyState.IsKeyUp(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return PreviousKeyState.IsKeyUp(key) && KeyState.IsKeyDown(key);
        }


        public static bool IsMouseLeftDown()
        {
            return MouseState.LeftButton == ButtonState.Pressed && IsMouseWithinViewport;
        }

        public static bool IsMouseLeftUp()
        {
            return MouseState.LeftButton == ButtonState.Released;
        }

        public static bool IsMouseLeftPressed()
        {
            return PreviousMouseState.LeftButton == ButtonState.Released && MouseState.LeftButton == ButtonState.Pressed && IsMouseWithinViewport;
        }

        public static bool IsMouseRightDown()
        {
            return MouseState.RightButton == ButtonState.Pressed && IsMouseWithinViewport;
        }

        public static bool IsMouseRightUp()
        {
            return MouseState.RightButton == ButtonState.Released;
        }

        public static bool IsMouseRightPressed()
        {
            return PreviousMouseState.RightButton == ButtonState.Released && MouseState.RightButton == ButtonState.Pressed && IsMouseWithinViewport;
        }

        public static float GetAxis(Keys negative, Keys positive)
        {
            var result = 0f;

            if (IsKeyDown(negative)) 
            {
                result -= 1f;
            }

            if (IsKeyDown(positive))
            {
                result += 1f;
            }

            return result;
        }

        public static Vector2 GetVector(Keys left, Keys right, Keys up, Keys down)
        {
            return new Vector2(GetAxis(left, right), GetAxis(up, down));
        }

        private static void UpdateKeyboardState() 
        {
            PreviousKeyState = KeyState;
            KeyState = Keyboard.GetState();
        }

        private static void UpdateMouseState(BackBuffer backBuffer) 
        {
            PreviousMouseState = MouseState;
            MouseState = Mouse.GetState();
            MousePosition = backBuffer.Unproject(MouseState.Position.ToVector2());
            IsMouseWithinViewport = backBuffer.LetterboxViewport.Bounds.Contains(MouseState.Position);
        }
    }
}
