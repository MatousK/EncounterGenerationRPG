using System;
using System.Collections;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Effects.Projectiles
{
    /// <summary>
    /// A behavior for a projectile that, when started, will move this object towards its target.
    /// Once we hit the target, a callback will be called. 
    /// This is a homing projectile - if the target moves, the projectile will follow him.
    /// This component will destroy the attached game object on hit.
    /// The game object will also be destroyed if the target game combatant is destroyed.
    /// </summary>
    class Projectile: MonoBehaviour
    {
        /// <summary>
        /// Sends the projectile towards the target.
        /// </summary>
        /// <param name="target">The target of this projectile.</param>
        /// <param name="speed">How many squares per second should the projectile move.</param>
        /// <param name="onHit">Callback that will be called once the projectile hits the target.</param>
        public void StartProjectile(CombatantBase target, float speed, Action onHit)
        {
            // This game object might have been spawned flipped if the original character was flipped.
            // However, when moving the projectile we rotate it correctly without the flip.
            // So if the original projectile was flipped, flip it back.
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            StartCoroutine(MoveProjectileToTarget(target, speed, onHit));
        }
        /// <summary>
        /// The coroutine which moves the projectile towards the target, calling a callback when it ends.
        /// </summary>
        /// <param name="target">The target of this projectile.</param>
        /// <param name="speed">How many squares per second should the projectile move.</param>
        /// <param name="onHit">Callback that will be called once the projectile hits the target.</param>
        /// <returns>The enumerator for the coroutine.</returns>
        private IEnumerator MoveProjectileToTarget(CombatantBase target, float speed, Action onHit)
        {
            Vector2 targetPosition = GetTargetPosition(target);
            while (Vector2.Distance(targetPosition, transform.position) > 0.5)
            {
                // Orient the arrow towards the target and move it towards him.
                Vector2 targetDirection = targetPosition - (Vector2)(transform.position);
                transform.right = - targetDirection;
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
                targetPosition = GetTargetPosition(target);
            }
            onHit();
            Destroy(gameObject);
        }
        /// <summary>
        /// Retrieve the current position of the target.
        /// If the target has already been destroyed, self destruct.
        /// </summary>
        /// <param name="target">The target of the projectile.</param>
        /// <returns>The current position of the target, or zero vector if the target is destroyed.</returns>
        private Vector2 GetTargetPosition(CombatantBase target)
        {
            if (target == null)
            {
                //Target has been destroyed, destroy the project, it has no target.
                Destroy(gameObject);
                return Vector2.zero;
            }
            var targetCollider = target.GetComponent<Collider2D>();
            return targetCollider != null ? targetCollider.bounds.center : target.transform.position;
        }
    }
}