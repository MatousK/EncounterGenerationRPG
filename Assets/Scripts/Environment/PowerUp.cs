using Assets.Scripts.Combat;
using Assets.Scripts.GameFlow;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    /// <summary>
    /// This component can increase a character's stats or heal him when picked up.
    /// This object does not have any interaction logic, the <see cref="TreasureChest"/> is responsible for calling the <see cref="ApplyPowerup(Hero)"/>
    /// </summary>
    public class PowerUp : MonoBehaviour
    {
        /// <summary>
        /// If true, this component represents should increase total max HP when applied. The amount is determined by the character who picks it up.
        /// </summary>
        public bool IsHealthPowerup;
        /// <summary>
        /// If true, this component represents should increase the attack when applied. The amount is determined by the character who picks it up.
        /// </summary>
        public bool IsAttackPowerup;
        /// <summary>
        /// When applied, restore up to this many percent of lost max HP.
        /// </summary>
        public float HealedMaxHpPercentage;
        /// <summary>
        /// Can detect game over and game reload. On game over this object self destructs.
        /// </summary>
        private GameStateManager gameStateManager;
        /// <summary>
        /// Called before the first frame update. Initializes references and subcribes to events.
        /// </summary>
        private void Start()
        {
            gameStateManager = FindObjectOfType<GameStateManager>();
            gameStateManager.GameOver += GameStateManager_GameOver;
        }
        /// <summary>
        /// On game over we destroy this object, as the powerups should not survive level reload.
        /// </summary>
        /// <param name="sender">Sender of this event.</param>
        /// <param name="e">Arguments of the event.</param>
        private void GameStateManager_GameOver(object sender, System.EventArgs e)
        {
            Destroy(gameObject);
        }
        /// <summary>
        /// Unsubscribe from event on destroy.
        /// </summary>
        private void OnDestroy()
        {
            gameStateManager.GameOver -= GameStateManager_GameOver;
        }
        /// <summary>
        /// Give the benefits provided by this powerup to the specified hero.
        /// </summary>
        /// <param name="forHero">Th hero who should receive the benefits from the power up.</param>
        public void ApplyPowerup(Hero forHero)
        {
            // Determine how much should the powerup increase attributes and increase them.
            var attackModifierAddition = IsAttackPowerup ? forHero.AttackPowerupIncrement : 0;
            var totalMaxHpAddition = IsHealthPowerup ? forHero.HealthPowerupIncrement : 0;

            forHero.Attributes.DealtDamageMultiplier += attackModifierAddition;

            // When increasing total max HP, we also have to increase normal max HP, otherwise the character would look injured, which would be wierd.
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
                // We also heal regular HP appropriately.
                forHero.HealDamage((int)toAddMaxHp, null, true);
            }
        }
    }
}