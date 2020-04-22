using Assets.Scripts.Combat;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class PowerUp : MonoBehaviour
    {
        public bool IsHealthPowerup;
        public bool IsAttackPowerup;
        public float HealedMaxHpPercentage;

        private GameStateManager gameStateManager;

        private void Start()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
            gameStateManager.GameOver += GameStateManager_GameOver;
        }

        private void GameStateManager_GameOver(object sender, System.EventArgs e)
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            gameStateManager.GameOver -= GameStateManager_GameOver;
        }

        public void ApplyPowerup(Hero forHero)
        {
            var attackModifierAddition = IsAttackPowerup ? forHero.AttackPowerupIncrement : 0;
            var totalMaxHpAddition = IsHealthPowerup ? forHero.HealthPowerupIncrement : 0;

            forHero.Attributes.DealtDamageMultiplier += attackModifierAddition;

            var oldTotalMaxHp = forHero.TotalMaxHitpoints;
            forHero.TotalMaxHitpoints += totalMaxHpAddition;

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