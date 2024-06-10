using Adventure.Networking;
using Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Adventure
{
    public sealed class World : IEnumerable<Entity>
    {
        // Keep ID count within 256
        public const int MaxEntities = 256;

        private readonly HashSet<int> _processedSnapshotEntities = new HashSet<int>();
        private readonly List<Entity> _entities;
        private readonly Dictionary<int, Entity> _entitiesById;
        private readonly List<Entity> _entitiesToRemove;
        private readonly List<Collider> _colliders;
        private readonly Queue<int> _availableIds;
        private readonly Queue<int> _reservedIds;
        private bool _shouldSortEntities;

        public World()
        {
            _entities = new List<Entity>();
            _entitiesById = new Dictionary<int, Entity>();
            _entitiesToRemove = new List<Entity>();
            _colliders = new List<Collider>();
            _availableIds = new Queue<int>();
            _reservedIds = new Queue<int>();

            for (int i = 0; i < MaxEntities; i++)
            {
                _availableIds.Enqueue(i);
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T FindLocalEntity<T>() where T : Entity
        {
            return _entities.FirstOrDefault(entity => entity.IsLocal && entity is T) as T;
        }

        public void Spawn(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                Spawn(entity);
            }
        }

        public void Spawn(Entity entity)
        {
            if (entity.World == null)
            {
                // TODO: Log when there is no available id
                entity.Id = _availableIds.Dequeue();
                entity.World = this;
                entity.IsActive = true;
                _entities.Add(entity);
                _entitiesById.Add(entity.Id, entity);
                entity.Spawn();
                _shouldSortEntities = true;
            }
        }

        public void Destroy(Entity entity)
        {
            if (entity.IsActive)
            {
                entity.IsActive = false;
                _entitiesToRemove.Add(entity);
            }
        }

        public void Clear()
        {
            _entities.Clear();
            _entitiesById.Clear();
            _entitiesToRemove.Clear();
            _colliders.Clear();
            _availableIds.Clear();
            _reservedIds.Clear();

            for (int i = 0; i < MaxEntities; i++)
            {
                _availableIds.Enqueue(i);
            }
        }

        public T FindFirst<T>() where T : Entity
        {
            T result = null;
            foreach (var entity in _entities)
            {
                if (entity is T t)
                {
                    result = t;
                    break;
                }
            }
            return result;
        }

        public T FindFirstWithTag<T>(string tag) where T : Entity
        {
            T result = null;
            foreach (var entity in _entities)
            {
                if (entity is T t && entity.Tags.Contains(tag))
                {
                    result = t;
                    break;
                }
            }
            return result;
        }

        public T FindFirst<T>(Func<T, bool> predicate) where T : Entity
        {
            T result = null;
            foreach (var entity in _entities)
            {
                if (entity is T t && predicate(t))
                {
                    result = t;
                    break;
                }
            }
            return result;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Update(gameTime);
            }

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].PostUpdate(gameTime);
            }

            if (Session.Instance.IsClient) 
            {
                for (int i = 0; i < _entities.Count; i++)
                {
                    if (!_processedSnapshotEntities.Contains(_entities[i].Id)) 
                    {
                        Destroy(_entities[i]);
                    }
                }
            }

            for (int i = 0; i < _entitiesToRemove.Count; i++)
            {
                _entitiesToRemove[i].Destroy();
                _entitiesToRemove[i].Collider?.Disable();
                _entitiesToRemove[i].World = null;
                _entities.Remove(_entitiesToRemove[i]);
            }

            _entitiesToRemove.Clear();
        }

        public void Draw(Renderer renderer, GameTime gameTime)
        {
            if (_shouldSortEntities)
            {
                _entities.Sort(SortEntities);
                _shouldSortEntities = false;
            }

            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Draw(renderer, gameTime);
            }
        }

        public void DebugDraw(Renderer renderer)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].DebugDraw(renderer);
            }

            //foreach (var collider in colliders)
            //{
            //    collider.Draw(renderer);
            //}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SortEntities(Entity a, Entity b)
        {
            return Comparer<int>.Default.Compare(a.DrawOrder, b.DrawOrder);
        }

        #region Collision

        public void EnableCollider(Collider collider)
        {
            _colliders.Add(collider);
        }

        public void DisableCollider(Collider collider)
        {
            _colliders.Remove(collider);
        }

        public List<Collider> OverlapColliders(Vector2 point)
        {
            var result = new List<Collider>();

            foreach (var collider in _colliders)
            {
                if (collider.Shape.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(Vector2 point, uint layerMask)
        {
            var result = new List<Collider>();

            foreach (var collider in _colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.Shape.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(Vector2 point, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in _colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Shape.OverlapPoint(point))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(CircleF circle, uint layerMask)
        {
            var result = new List<Collider>();

            foreach (var collider in _colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.Shape.OverlapCircle(circle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(CircleF circle, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in _colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Shape.OverlapCircle(circle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(RectangleF rectangle, uint layerMask)
        {
            var result = new List<Collider>();

            foreach (var collider in _colliders)
            {
                if (CanOverlapCollider(collider, layerMask) && collider.Shape.OverlapRectangle(rectangle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(RectangleF rectangle, uint layerMask, Collider ignore)
        {
            var result = new List<Collider>();

            foreach (var collider in _colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Shape.OverlapRectangle(rectangle))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<Collider> OverlapColliders(Collider collider, uint layerMask)
        {
            var result = new List<Collider>();

            foreach (var other in _colliders)
            {
                if (CanOverlapCollider(other, layerMask, collider) && other.Shape.Overlaps(collider.Shape))
                {
                    result.Add(collider);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(CircleF circle, uint layerMask, Collider ignore) where T : Entity
        {
            var result = new List<T>();

            foreach (var collider in _colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Entity is T t && collider.Shape.OverlapCircle(circle))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(RectangleF rectangle, uint layerMask, Collider ignore) where T : Entity
        {
            var result = new List<T>();

            foreach (var collider in _colliders)
            {
                if (CanOverlapCollider(collider, layerMask, ignore) && collider.Entity is T t && collider.Shape.OverlapRectangle(rectangle))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public List<T> OverlapEntities<T>(Collider collider, uint layerMask) where T : Entity
        {
            var result = new List<T>();

            foreach (var other in _colliders)
            {
                if (CanOverlapCollider(other, layerMask, collider) && other.Entity is T t && other.Shape.Overlaps(collider.Shape))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public Collider Cast(Collider collider, Vector2 direction, uint layerMask, out Collision collision)
        {
            collision = Collision.Empty;
            Collider result = null;

            var path = new Segment(collider.Shape.Bounds.Center, direction);

            foreach (var other in OverlapColliders(collider.Shape.Bounds.Union(direction), layerMask, collider))
            {
                if (collider.Shape.Intersects(other.Shape, path, out var intersection) && intersection.Time < collision.Intersection.Time)
                {
                    result = other;
                    collision = new Collision(direction, intersection);
                }
            }

            return result;
        }

        public Collider Cast(Vector2 position, Vector2 direction, uint layerMask, Collider ignore)
        {
            var collision = Collision.Empty;
            Collider result = null;

            var segment = new Segment(position, direction);
            var broadphaseRectangle = new RectangleF(
                MathF.Min(position.X, position.X + direction.X),
                MathF.Min(position.Y, position.Y + direction.Y),
                MathF.Max(position.X, position.X + direction.X),
                MathF.Max(position.Y, position.Y + direction.Y));

            foreach (var collider in OverlapColliders(broadphaseRectangle, layerMask, ignore))
            {
                if (collider.Shape.IntersectSegment(segment, out var intersection) && intersection.Time < collision.Intersection.Time)
                {
                    result = collider;
                    collision = new Collision(direction, intersection);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanOverlapCollider(Collider collider, uint layerMask)
        {
            return collider.CheckMask(layerMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanOverlapCollider(Collider collider, uint layerMask, Collider ignore)
        {
            return collider.CheckMask(layerMask) && collider != ignore;
        }

        #endregion Collision

        #region Snapshot

        public void ServerWriteToSnapshot(Message message)
        {
            message.Write(_entities.Count);

            foreach (var entity in _entities)
            {
                message.Write(entity.Id);
                message.Write(entity.OwnerNumber);
                message.Write((int)entity.Type);
                entity.ServerWriteToSnapshot(message);
            }
        }

        public void ClientReadFromSnapshot(Message message)
        {
            _processedSnapshotEntities.Clear();

            var entityCount = message.ReadInt();

            for (var i = 0; i < entityCount; i++)
            {
                var id = message.ReadInt();
                var ownerNumber = message.ReadInt();
                var type = (EntityType)message.ReadInt();

                _processedSnapshotEntities.Add(id);

                if (!_entitiesById.TryGetValue(id, out var entity))
                {
                    entity = EntityFactory.CreateEntityFromType(type);
                    if (entity != null)
                    {
                        entity.Id = id;
                        entity.OwnerNumber = ownerNumber;
                        Spawn(entity);
                    }
                }
                entity.ClientReadFromSnapshot(message);
            }
        }

        public void ReadFromEntityMessage(int entityId, Message buffer)
        {
            if (_entitiesById.TryGetValue(entityId, out var entity))
            {
                entity.ReadFromEntityMessage(buffer);
            }
        }

        #endregion
    }
}
