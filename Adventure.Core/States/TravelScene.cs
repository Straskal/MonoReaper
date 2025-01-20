using System.Collections.Generic;

namespace Adventure.Core.States;

public class TravelScene : Scene
{
    private readonly List<Actor> actors = [];

    public override void Update()
    {
        for (int i = 0; i < actors.Count; i++) 
        {
            actors[i].Update();
        }
    }

    public override void Draw()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].Draw();
        }
    }

    private void SpawnActor(Actor actor) 
    {
        actors.Add(actor);
    }
}
