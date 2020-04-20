using Assets.Scripts.Combat;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class PowerUp : MonoBehaviour
    {
        public float AttackModifierAddition;
        public float AttackModifierMultiplier = 1;
        public float DefenseModifierSubtraction;
        public float DefenseModifierMultiplier = 1;
        public float TotalMaxHpAddition;
        public float TotalMaxHpMultiplier = 1;
        public float HealedMaxHpPercentage;

        private GameStateManager gameStateManager;

        void Start()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
            gameStateManager.GameOver += GameStateManager_GameOver;
        }

        private void GameStateManager_GameOver(object sender, System.EventArgs e)
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            gameStateManager.GameOver -= GameStateManager_GameOver;
        }

        public void ApplyPowerup(Hero forHero)
        {
            forHero.Attributes.DealtDamageMultiplier += AttackModifierAddition;
            forHero.Attributes.DealtDamageMultiplier *= AttackModifierMultiplier;

            forHero.Attributes.ReceivedDamageMultiplier -= DefenseModifierSubtraction;
            forHero.Attributes.ReceivedDamageMultiplier *= DefenseModifierMultiplier;

            var oldTotalMaxHp = forHero.TotalMaxHitpoints;
            forHero.TotalMaxHitpoints += TotalMaxHpAddition;
            forHero.TotalMaxHitpoints *= TotalMaxHpMultiplier;

            var toAddMaxHp = forHero.TotalMaxHitpoints - oldTotalMaxHp + forHero.TotalMaxHitpoints * HealedMaxHpPercentage;
            forHero.MaxHitpoints += toAddMaxHp;
            if (toAddMaxHp != 0)
            {
                if (forHero.MaxHitpoints > forHero.TotalMaxHitpoints)
                {
                    forHero.MaxHitpoints = forHero.TotalMaxHitpoints;
                }
                forHero.HealDamage((int)toAddMaxHp, null, true);
            }
        }
    }
}