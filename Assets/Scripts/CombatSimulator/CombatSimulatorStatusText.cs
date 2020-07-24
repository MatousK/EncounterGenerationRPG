using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CombatSimulator
{
    /// <summary>
    /// Component which shows the current status of the combat simulator in the game.
    /// </summary>
    public class CombatSimulatorStatusText: MonoBehaviour
    {
        /// <summary>
        /// The field in which we will actually display the status text.
        /// </summary>
        private Text statusTextfield;
        /// <summary>
        /// The simulator whose state we want to represent.
        /// </summary>
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