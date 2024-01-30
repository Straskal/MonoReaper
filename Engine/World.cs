using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Engine
{
    public sealed class World : IEnumerable<Entity>
    {
        private readonly List<Entity> entities;
        private readonly List<Entity> entitiesToRemove;
        private readonly List<CollisionComponent> colliders;
        private bool sort;

        public World()
        {
            entities = new List<Entity>();
            entitiesToRemove = new List<Entity>();
            colliders = new List<CollisionComponent>();
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Spawn(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.World = this;
                entity.IsAlive = true;
                this.entities.Add(entity);
                entity.Spawn();
            }
            sort = true;
        }

        public void Spawn(Entity entity)
        {
            Spawn(entity, entity.Position);
        }

        public void Spawn(Entity entity, Vector2 position)
        {
            if (entity.World == null)
            {
                entity.World = this;
                entity.IsAlive = true;
                entity.Position = position;
                entities.Add(entity);
                entity.Spawn();
                sort = true;
            }
        }

        public void Destroy(Entity entity)
        {
            if (entity.IsAlive)
            {
                entity.IsAlive = false;
                entitiesToRemove.Add(entity);
            }
        }

        public void Clear()
        {
            entities.Clear();
            entitiesToRemove.Clear();
            colliders.Clear();
        }

        public T Find<T>() where T : Entity
        {
            foreach (var entity in entities)
            {
                if (entity is T t)
                {
                    return t;
                }
            }
            return null;
        }

        public T FindWithTag<T>(string tag) where T : Entity
        {
            foreach (var entity in entities)
            {
                if (entity is T t && entity.Tags.Contains(tag))
                {
                    return t;
                }
            }
            return null;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Update(gameTime);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].PostUpdate(gameTime);
            }

            for (int i = 0; i < entitiesToRemove.Count; i++)
            {
                entitiesToRemove[i].Destroy();
                entitiesToRemove[i].Collider?.Disable();
                entitiesToRemove[i].World = null;
                entities.Remove(entitiesToRemove[i]);
            }

            entitiesToRemove.Clear();
        }

        public void Draw(Renderer renderer, GameTime gameTime)
        {
            if (sort)
            {
                entities.Sort(SortEntities);
                sort = false;
            }

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Draw(renderer, gameTime);
            }
        }

        public void DebugDraw(Renderer renderer)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].DebugDraw(renderer);
            }

            foreach (var collider in colliders)
            {
                collider.Draw(renderer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SortEntities(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.DrawOrder, b.DrawOrder);
        }

        #region Collision

        public void EnableCollider(CollisionComponent collider)
        {
            colliders.Add(collider);
        }

        public void DisableCollider(CollisionComponent collider)
        {
            colliders.Remove(collider);
        }

        public List<CollisionComponent> OverlapColliders(Vector2 point)
        {
            var result = new List<CollisionComponent>();

            foreach (var collider in colliders)
            {
                if (collider.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<CollisionComponent> OverlapColliders(Vector2 point, uint layerMask)
        {
            var result = new List<CollisionComponent>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<CollisionComponent> OverlapColliders(Vector2 point, uint layerMask, CollisionComponent ignore)
        {
            var result = new List<CollisionComponent>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<CollisionComponent> OverlapColliders(CircleF circle, uint layerMask)
        {
            var result = new List<CollisionComponent>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.OverlapCircle(circle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<CollisionComponent> OverlapColliders(CircleF circle, uint layerMask, CollisionComponent ignore)
        {
            var result = new List<CollisionComponent>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.OverlapCircle(circle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<CollisionComponent> OverlapColliders(RectangleF rectangle, uint layerMask)
        {
            var result = new List<CollisionComponent>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.OverlapRectangle(rectangle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<CollisionComponent> OverlapColliders(RectangleF rectangle, uint layerMask, CollisionComponent ignore)
        {
            var result = new List<CollisionComponent>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.OverlapRectangle(rectangle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<CollisionComponent> OverlapColliders(CollisionComponent collider, uint layerMask)
        {
            var result = new List<CollisionComponent>();

            foreach (var other in colliders)
            {
                if (CanOverlapCollider(other, layerMask, collider) && other.Overlaps(collider))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(CircleF circle, uint layerMask, CollisionComponent ignore) where T : Entity
        {
            var result = new List<T>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Entity is T t && collider.OverlapCircle(circle))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(RectangleF rectangle, uint layerMask, CollisionComponent ignore) where T : Entity
        {
            var result = new List<T>();

            foreach (var collider in colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Entity is T t && collider.OverlapRectangle(rectangle))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(CollisionComponent collider, uint layerMask) where T : Entity
        {
            var result = new List<T>();

            foreach (var other in colliders)
            {
                if (CanOverlapCollider(other, layerMask, collider) && other.Entity is T t && other.Overlaps(collider))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public CollisionComponent Cast(CollisionComponent collider, Vector2 direction, uint layerMask, out Collision collision)
        {
            collision = Collision.Empty;
            CollisionComponent result = null;

            var path = new Segment(collider.Bounds.Center, direction);

            foreach (var other in OverlapColliders(collider.Bounds.Union(direction), layerMask, collider))
            {
                if (collider.Intersects(other, path, out var intersection) && intersection.Time < collision.Intersection.Time)
                {
                    result = other;
                    collision = new Collision(direction, intersection);
                }
            }

            return result;
        }

        public CollisionComponent Cast(Vector2 position, Vector2 direction, uint layerMask, CollisionComponent ignore)
        {
            var collision = Collision.Empty;
            CollisionComponent result = null;

            var segment = new Segment(position, direction);
            var broadphaseRectangle = new RectangleF(
                MathF.Min(position.X, position.X + direction.X),
                MathF.Min(position.Y, position.Y + direction.Y),
                MathF.Max(position.X, position.X + direction.X),
                MathF.Max(position.Y, position.Y + direction.Y));

            foreach (var collider in OverlapColliders(broadphaseRectangle, layerMask, ignore))
            {
                if (collider.IntersectSegment(segment, out var intersection) && intersection.Time < collision.Intersection.Time)
                {
                    result = collider;
                    collision = new Collision(direction, intersection);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanOverlapCollider(CollisionComponent collider, uint layerMask)
        {
            return collider.CheckLayer(layerMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanOverlapCollider(CollisionComponent collider, uint layerMask, CollisionComponent ignore)
        {
            return collider.CheckLayer(layerMask) && collider != ignore;
        }

        #endregion Collision
    }
}
