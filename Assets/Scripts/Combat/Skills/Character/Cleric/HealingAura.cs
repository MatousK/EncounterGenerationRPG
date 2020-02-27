using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class HealingAura : PersonalSkill
{
    public HealingAura()
    {
        SkillAnimationName = "";
    }
    /// <summary>
    /// How far must the character be for the aura to work.
    /// </summary>
    public float AuraRange;
    /// <summary>
    ///  How many times per second should the heal pulse fire.
    /// </summary>
    public float HealPulseFrequency;
    /// <summary>
    /// How much healing should the aura do per pulse.
    /// </summary>
    public float HealPulsePercentage = 10;
    /// <summary>
    /// The aura object which should be activated while this skill is active.
    /// </summary>
    public GameObject AuraInstance;
    /// <summary>
    /// Returns how much time is between heal pulses.
    /// </summary>
    private float PulseTime
    {
        get
        {
            return 1 / HealPulseFrequency;
        }
    }
    private float timeToNextPulse = float.PositiveInfinity;
    private CombatantsManager combatantsManager;
    // Start is called before the first frame update
    protected override void Awake()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        timeToNextPulse -= Time.deltaTime;
        if (timeToNextPulse <= 0)
        {
            HealPulse();
        }
        base.Update();
    }

    private void HealPulse()
    {
        timeToNextPulse = PulseTime;
        foreach (var ally in combatantsManager.PlayerCharacters)
        {
            if (Vector2.Distance(ally.transform.position, transform.position) < AuraRange)
            {
                float healPulseAmount = ally.TotalMaxHitpoints * HealPulsePercentage;
                ally.HealDamage(healPulseAmount, selfCombatant);
            }
        }
    }

    protected override void OnPersonalSkillStarted()
    {
        AuraInstance.SetActive(true);
        HealPulse();
    }

    protected override void OnPersonalSkillStopped()
    {
        AuraInstance.SetActive(false);
        timeToNextPulse = float.PositiveInfinity;
    }
}