using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

class Projectile: MonoBehaviour
{
    public void StartProjectile(Vector2 target, float speed, Action onHit)
    {
        // Rotate already rotates the arrow correctly - if the arrow was flipped, flip it back.
        if (transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        StartCoroutine(MoveProjectileToTarget(target, speed, onHit));
    }

    private IEnumerator MoveProjectileToTarget(Vector2 target, float speed, Action onHit)
    {
        while (Vector2.Distance(target, transform.position) > 0.1)
        {
            Vector2 targetDirection = target - (Vector2)(transform.position);
            transform.right = - targetDirection;
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        onHit();
        Destroy(gameObject);
    }
}