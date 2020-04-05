using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatSimulator
{
    public class CombatSimulatorStatusText: MonoBehaviour
    {
        private Text statusTextfield;
        private CombatSimulator combatSimulator;

        private void Awake()
        {
            combatSimulator = FindObjectOfType<global::Assets.Scripts.CombatSimulator.CombatSimulator>();
            statusTextfield = GetComponent<Text>();
        }

        private void Update()
        {
            var text = "Test Index: " + combatSimulator.CurrentTestIndex + "\n";
            text += "Party Provider: " + combatSimulator.TestGenerator.CurrentPartyProvider + "\n";
            text += "Monster Tier: " + combatSimulator.TestGenerator.MonsterTier;
            statusTextfield.text = text;

        }
    }
}