using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public bool IsSelected;
    Circle selectionIndicator;

    // Start is called before the first frame update
    void Start()
    {
        selectionIndicator = GetComponent<Circle>();
        selectionIndicator.IsVisible = IsSelected;
        var combatant = GetComponent<CombatantBase>();
        if (combatant != null)
        {
            combatant.CombatantDied += Combatant_CombatantDied;
        }
    }

    // Update is called once per frame
    void Update()
    {
        selectionIndicator.IsVisible = IsSelected;
    }

    private void Combatant_CombatantDied(object sender, System.EventArgs e)
    {
        IsSelected = false;
    }
}
