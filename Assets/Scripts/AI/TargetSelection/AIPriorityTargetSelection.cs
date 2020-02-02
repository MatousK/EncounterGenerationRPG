using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Targets the enemy with the greatest targeting priority, i.e. he who will be the most dangerous according to this AI.
/// </summary>
public class AIPriorityTargetSelection : AITargetSelectionMethodBase
{
    private CombatantsManager combatantsManager;
    protected override void Awake()
    {
        base.Awake();
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }

    public override bool TryGetTarget(ref CombatantBase target)
    {
        int currentMaxPriority = int.MinValue;
        foreach (var hero in combatantsManager.GetPlayerCharacters(onlyAlive:true))
        {
            if (hero.AITargetPriority > currentMaxPriority)
            {
                currentMaxPriority = hero.AITargetPriority;
                target = hero;
            }
        }
        return currentMaxPriority != int.MinValue;
    }
}