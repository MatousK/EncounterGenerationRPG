using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterPortraitsManager: MonoBehaviour
{
    public List<CharacterPortrait> AvailablePortraitWidgets;
    private CombatantsManager combatantsManager;

    void Awake() 
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }
    void Start()
    {
        UpdatePortraits();
    }

    void Update()
    {
        UpdatePortraits();
    }

    void UpdatePortraits()
    {
        var currentPartyMembers = combatantsManager.PlayerCharacters;
        for (int i = 0; i < AvailablePortraitWidgets.Count; ++i)
        {
            if (i < currentPartyMembers.Count)
            {
                AvailablePortraitWidgets[i].gameObject.SetActive(true);
                AvailablePortraitWidgets[i].RepresentedHero = currentPartyMembers[i];
            }
            else
            {
                AvailablePortraitWidgets[i].gameObject.SetActive(false);
            }
        }
    }
}