using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Display under a character who is using a skill on someone to indicate that who is his target, as our sprite can only be oriented left or right.
    /// Will rotate itself towards the target.
    /// One instance should be on a combatant which will clone itself whenever a new skill should be indicated.
    /// </summary>
    public class AttackDirectionIndicator: MonoBehaviour
    {
        /// <summary>
        /// How quickly should the indicator rotate when target moves.
        /// </summary>
        public float RotationSpeed = 10f;
        /// <summary>
        /// The current target this indicator is pointing to.
        /// </summary>
        private CombatantBase currentTarget;
        /// <summary>
        /// Called every frame, to update the rotation we are pointing to.
        /// </summary>
        void Update()
        {
            UpdateRotation(true);
        }
        /// <summary>
        /// Clones this indicator and points it towards some target. It will play its animation of quickly appearing and disappearing.
        /// When it disappears, destroy the object.
        /// </summary>
        /// <param name="target">The target of the attack.</param>
        public void IndicateAttackOnTarget(CombatantBase target)
        {
            var clone = Instantiate(gameObject, transform.parent);
            var cloneIndicator = clone.GetComponent<AttackDirectionIndicator>();
            cloneIndicator.currentTarget = target;
            cloneIndicator.GetComponent<SpriteRenderer>().enabled = true;
            cloneIndicator.GetComponent<Animation>().Play();
            cloneIndicator.UpdateRotation(false);
        }
        /// <summary>
        /// Update the rotation of this attack so it points toward the target.
        /// </summary>
        /// <param name="withAnimation">If true, we should animate this rotation.</param>
        private void UpdateRotation(bool withAnimation)
        {
            if (currentTarget == null)
            {
                return;
            }
            Vector3 vectorToTarget = currentTarget.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
            // This is the target rotation.
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            if (withAnimation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * RotationSpeed);
            }
            else
            {
                transform.rotation = q;
            }
        }
        /// <summary>
        /// Called when the animation the fade animation of this indicator is completed. Destroys self.
        /// </summary>
        public void OnAnimationCompleted()
        {
            Destroy(gameObject);
        }
    }
}
