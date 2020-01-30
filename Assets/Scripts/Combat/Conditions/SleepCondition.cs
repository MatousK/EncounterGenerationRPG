using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// When added to a creature, will put it to sleep for a specified duration, then waking it up.
/// </summary>
class SleepCondition: MonoBehaviour
{
    public float remainingDuration = float.PositiveInfinity;
    private float originalHitpoints;
    private void Start()
    {
        foreach (var skill in GetComponents<Skill>())
        {
            skill.TryStopSkill();
        }
        GetComponent<AggressiveBehavior>().enabled = false;
        GetComponent<AutoAttacking>().enabled = false;
        GetComponent<Animator>().SetBool("Asleep", true);
        originalHitpoints = GetComponent<CombatantBase>().HitPoints;
    }

    private void Update()
    {
        remainingDuration -= Time.deltaTime;
        if (originalHitpoints != GetComponent<CombatantBase>().HitPoints)
        {
            // Took damage, wake up.
            StopSleeping();
        }
        else if (remainingDuration <= 0)
        {
            StopSleeping();
        }
    }

    private void StopSleeping()
    {
        GetComponent<AggressiveBehavior>().enabled = true;
        GetComponent<AutoAttacking>().enabled = true;
        GetComponent<Animator>().SetBool("Asleep", false);
        Destroy(this);
    }
}