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
    public sealed class GameStateStack
    {
        private readonly App _application;
        private readonly List<GameState> _stack = new();
        private readonly Queue<Action> _stackOperations = new();

        public GameStateStack(App application) 
        {
            _application = application;
        }

        /// <summary>
        /// Gets the game state at the top of the stack
        /// </summary>
        public GameState Top => _stack.LastOrDefault();

        /// <summary>
        /// Pushes a state onto the top of the stack
        /// </summary>
        /// <param name="gameState"></param>
        public void Push(GameState gameState) 
        {
            _stackOperations.Enqueue(() => 
            {
                _stack.Add(gameState);
                gameState.Start();
            });
        }

        /// <summary>
        /// Pops the given state off of the stack
        /// </summary>
        /// <param name="gameState"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Pop(GameState gameState) 
        {
            _stackOperations.Enqueue(() =>
            {
                if (_stack.Count == 1) 
                {
                    throw new InvalidOperationException("The root state cannot be popped from the stack");
                }

                gameState.Stop();
                _stack.Remove(gameState);
            });
        }

        /// <summary>
        /// Pops all states, except for the root state, from the stack
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
