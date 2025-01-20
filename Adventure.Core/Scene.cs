using System.Collections.Generic;
using System.Diagnostics;

namespace Adventure.Core;

public class Scene
{
    private readonly List<Actor> actors = [];
    private readonly List<Actor> actorsToDespawn = [];

    public virtual void Start() { }
    public virtual void Stop() { }

    public virtual void Update()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].Update();
        }

        if (actorsToDespawn.Count > 0) 
        {
            for (int i = 0; i < actorsToDespawn.Count; i++)
            {
                actors[i].Destroy();
            }

            actorsToDespawn.Clear();
        }
    }

    public virtual void Draw()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].Draw();
        }
    }

    public void Spawn(Actor actor)
    {
        Debug.Assert(!actors.Contains(actor), "A spawned actor should not be spawned.");

        actors.Add(actor);
    }

    public void Despawn(Actor actor)
    {
        Debug.Assert(!actorsToDespawn.Contains(actor), "A despawned actor should not be despawned.");

        actorsToDespawn.Add(actor);
    }
}
