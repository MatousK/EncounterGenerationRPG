using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class FloatingText: MonoBehaviour
{
    void Update()
    {
        if (transform.parent.localScale.x < 0 != transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void OnDisappearAnimationFinished()
    {
        // Text animation has finished, we should destroy this.
        Destroy(gameObject);
    }
}
