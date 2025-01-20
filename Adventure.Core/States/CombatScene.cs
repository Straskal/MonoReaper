using Engine;
using Engine.Extensions;
using System.Collections;
using System.Collections.Generic;

namespace Adventure.Core.States;

public enum CombatActionType 
{
    Attack,
    Defend,
    Skill,
    Item
}

public class CombatAction 
{
    /// <summary>
    /// The actor that took the action.
    /// </summary>
    public Actor Current { get; set; }

    /// <summary>
    /// The type of action.
    /// </summary>
    public CombatActionType Type { get; set; }

    /// <summary>
    /// The target of the action, if there is one.
    /// </summary>
    public Actor Target { get; set; }

    /// <summary>
    /// The ID of the choice item. (Item, skill, etc...)
    /// </summary>
    public int ChoiceId { get; set; }
}

public sealed class CombatScene : Scene
{
    public const int MaxEnemies = 6;

    private readonly List<Actor> party = new(Session.MaxPlayers);
    private readonly List<Actor> enemies = new(MaxEnemies);
    private readonly CoroutineRunner coroutineRunner = new();
    private readonly Queue<CombatAction> queue = [];

    private float turnTimer = 0f;

    public void Initialize(IEnumerable<Actor> party, IEnumerable<Actor> enemies)
    {
        this.party.AddRange(party);
        this.enemies.AddRange(enemies);
    }

    public override void Start()
    {
    }

    public override void Stop()
    {
    }

    public override void Update()
    {
        coroutineRunner.Update();
    }

    private bool CheckCombatOverConditions() 
    {
        return false;
    }

    private IEnumerator RunPartyState()
    {
        yield return PartyStateEnter();
        yield return PartyStatePlan();
        yield return PartyStateExecute();

        coroutineRunner.Start(CheckCombatOverConditions() ? RunSummary() : RunEnemyState());
    }

    private IEnumerator RunEnemyState()
    {
        yield return EnemyStateEnter();
        yield return EnemyStatePlan();
        yield return EnemyStateExecute();

        coroutineRunner.Start(CheckCombatOverConditions() ? RunSummary() : RunPartyState());
    }

    private IEnumerator RunSummary() 
    {
        // Party wins if all enemies are dead or fleed.
        // - Give XP
        // - Give rewards
        // - Give downed party members negative effects

        // Enemies win if all party members are down / dead / fleed.
        // - Give party negative effects
        // - Give downed party members negative effects

        // GOTO: Previous scene.

        return null;
    }

    private IEnumerator PartyStateEnter()
    {
        // Reset turn timer.
        turnTimer = 0f;

        return null;
    }

    private IEnumerator PartyStatePlan()
    {
        // Increment turn timer.
        turnTimer += Adventure.Time.GetDeltaTime();

        // Wait for party members to take turns or for the timer to run out.
        if (queue.Count < party.Count && turnTimer < 5f) 
        {
            yield return null;
        }
    }

    private IEnumerator PartyStateExecute()
    {
        while (queue.TryDequeue(out var action)) 
        {
            yield return ExecuteCombatAction(action);
        }
    }

    private IEnumerator EnemyStateEnter()
    {
        return null;
    }

    private IEnumerator EnemyStatePlan()
    {
        return null;
    }

    private IEnumerator EnemyStateExecute()
    {
        while (queue.TryDequeue(out var action))
        {
            yield return ExecuteCombatAction(action);
        }
    }

    private IEnumerator ExecuteCombatAction(CombatAction action) 
    {
        switch (action.Type) 
        {
            case CombatActionType.Attack:
                return ExecuteAttackAction(action);
            case CombatActionType.Defend:
                return ExecuteDefendAction(action);
            case CombatActionType.Skill:
                return ExecuteSkillAction(action);
            case CombatActionType.Item:
                return ExecuteItemAction(action);
            default:
                return null;
        }
    }

    private IEnumerator ExecuteAttackAction(CombatAction action) 
    {
        yield return PlayAttackAnimation(action);
    }

    private IEnumerator PlayAttackAnimation(CombatAction action) 
    {
        return null;
    }

    private IEnumerator ExecuteDefendAction(CombatAction action)
    {
        return null;
    }

    private IEnumerator ExecuteSkillAction(CombatAction action)
    {
        return null;
    }

    private IEnumerator ExecuteItemAction(CombatAction action)
    {
        return null;
    }
}
