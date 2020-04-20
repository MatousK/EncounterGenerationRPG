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
            statusTextfield = GetComponent<Text>();
        }

        private void Start()
        {
            combatSimulator = FindObjectOfType<CombatSimulator>();
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