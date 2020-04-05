﻿using System;
using System.Collections;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.Effects.Projectiles
{
    class Projectile: MonoBehaviour
    {
        public void StartProjectile(CombatantBase target, float speed, Action onHit)
        {
        
            // Rotate already rotates the arrow correctly - if the arrow was flipped, flip it back.
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
            StartCoroutine(MoveProjectileToTarget(target, speed, onHit));
        }

        private IEnumerator MoveProjectileToTarget(CombatantBase target, float speed, Action onHit)
        {
            Vector2 targetPosition = GetTargetPosition(target);
            while (Vector2.Distance(targetPosition, transform.position) > 0.5)
            {
                Vector2 targetDirection = targetPosition - (Vector2)(transform.position);
                transform.right = - targetDirection;
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
                targetPosition = GetTargetPosition(target);
            }
            onHit();
            Destroy(gameObject);
        }

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