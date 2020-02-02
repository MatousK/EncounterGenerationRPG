using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Represents an action that an AI might take against some target.
/// </summary>
public abstract class AIActionSelectionMethodBase: MonoBehaviour
{
    protected CombatantBase representedCombatant;
    public bool CanTargetFriendlies = false;
    public bool CanTargetHostiles = true;

    protected virtual void Awake()
    {
        representedCombatant = transform.parent.gameObject.GetComponent<CombatantBase>();
    }
    /// <summary>
    /// The skill that should be used against the target if the condition represented by this class is satisfied.
    /// </summary>
    public Skill ActionSkill;
    /// <summary>
    /// If true, this skill should be executed at the specified target.
    /// </summary>
    /// <param name="target"> The potential target of this skill.</param>
    /// <returns> True if the skill should be executed, otherwise false.</returns>
    public abstract bool ShouldSelectAction(CombatantBase target);
}