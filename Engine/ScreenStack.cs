using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Engine.Graphics;

namespace Engine
{
    /// <summary>
    /// This class represents a stack of game states.
    /// </summary>
    /// <remarks>
    /// States are updated top down, and are drawn bottom up. The root state is always updated and drawn.
    /// </remarks>
    public sealed class ScreenStack
    {
        private readonly List<Screen> _stack = new();
        private readonly Queue<Action> _stackOperations = new();

        /// <summary>
        /// Gets the game state at the top of the stack
        /// </summary>
        public Screen Top => _stack.LastOrDefault();

        /// <summary>
        /// Returns the root state
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetRoot<T>() where T : Screen 
        {
            return _stack[0] as T;
        }

        /// <summary>
        /// Pushes the given screen onto the top of the stack
        /// </summary>
        /// <param name="screen"></param>
        public void Push(Screen screen) 
        {
            _stackOperations.Enqueue(() => 
            {
                _stack.Add(screen);
                screen.Start();
            });
        }

        /// <summary>
        /// Pops the given screen off of the stack
        /// </summary>
        /// <param name="screen"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Pop(Screen screen) 
        {
            _stackOperations.Enqueue(() =>
            {
                if (_stack.Count == 1) 
                {
                    throw new InvalidOperationException("The root state cannot be popped from the stack");
                }

                screen.Stop();
                _stack.Remove(screen);
            });
        }

        /// <summary>
        /// Pops all screens, except for the root screen, from the stack
        /// </summary>
        public void PopAll()
        {
            _stackOperations.Enqueue(() =>
            {
                while (_stack.Count > 1) 
                {
                    _stack.Last().Stop();
                    _stack.Remove(_stack.Last());
                }
            });
        }

        /// <summary>
        /// Pops all screens from the stack and pushes the given screen to the top
        /// </summary>
        /// <param name="screen"></param>
        public void SetTop(Screen screen) 
        {
            _stackOperations.Enqueue(() =>
            {
                while (_stack.Count > 1)
                {
                    _stack.Last().Stop();
                    _stack.Remove(_stack.Last());
                }

                _stack.Add(screen);
                screen.Start();
            });
        }

        internal void Update(GameTime gameTime)
        {
            RunStackOperations();
            
            for (int i = 0; i < _stack.Count; i++)
            {
                // Always update the root state and the top state.
                if (i == 0 || i == _stack.Count - 1 || _stack[i + 1].ShouldUpdateBelow)
                {
                    _stack[i].Update(gameTime);
                }
            }
        }

        internal void Draw(Renderer renderer, GameTime gameTime) 
        {
            for (int i = 0; i < _stack.Count; i++) 
            {
                // Always draw the root state and the top state.
                if (i == 0 || i == _stack.Count - 1 || _stack[i + 1].ShouldDrawBelow) 
                {
                    _stack[i].Draw(renderer, gameTime);
                }
            }
        }

        private void RunStackOperations() 
        {
            while (_stackOperations.TryDequeue(out var operation))
            {
                operation();
            }
        }
    }
}
