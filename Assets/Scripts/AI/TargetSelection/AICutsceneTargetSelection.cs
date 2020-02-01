using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Checks if there is some target forced by the current cutscene.
/// Which right now means only that an active cutscene forces no target at all.
/// </summary>
class AICutsceneTargetSelection : AITargetSelectionMethodBase
{
    CutsceneManager cutsceneManager;

    protected override void Awake()
    {
        base.Awake();
        cutsceneManager = FindObjectOfType<CutsceneManager>();
    }

    public override bool TryGetTarget(ref CombatantBase target)
    {
        if (cutsceneManager.IsCutsceneActive)
        {
            target = null;
            return true;
        }
        return false;
    }
}