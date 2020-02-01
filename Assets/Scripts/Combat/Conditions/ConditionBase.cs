using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Can be put on a character to somehow affect it, eithe positively or negatively. Has set duration.
/// </summary>
public abstract class ConditionBase : MonoBehaviour
{
    public float remainingDuration = float.PositiveInfinity;
    public event EventHandler<EventArgs> ConditionEnded;

    protected virtual void Start()
    {
        StartCondition();
    }

    protected virtual void Update()
    {
        remainingDuration -= Time.deltaTime;
        if (remainingDuration <= 0)
        {
            EndCondition();
        }
    }

    protected virtual void StartCondition() { }

    protected virtual void EndCondition()
    {
        ConditionEnded?.Invoke(this, new EventArgs()); 
    }
}