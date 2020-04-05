using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class AttackDirectionIndicator: MonoBehaviour
    {
        public float RotationSpeed = 10f;
        private CombatantBase currentTarget;
        void Update()
        {
            UpdateRotation(true);
        }
        public void IndicateAttackOnTarget(CombatantBase target)
        {
            var clone = Instantiate(gameObject, transform.parent);
            var cloneIndicator = clone.GetComponent<AttackDirectionIndicator>();
            cloneIndicator.currentTarget = target;
            cloneIndicator.GetComponent<SpriteRenderer>().enabled = true;
            cloneIndicator.GetComponent<Animation>().Play();
            cloneIndicator.UpdateRotation(false);
        }

        private void UpdateRotation(bool withAnimation)
        {
            if (currentTarget == null)
            {
                return;
            }
            Vector3 vectorToTarget = currentTarget.transform.position - transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
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

        public void OnAnimationCompleted()
        {
            Destroy(gameObject);
        }
    }
}
