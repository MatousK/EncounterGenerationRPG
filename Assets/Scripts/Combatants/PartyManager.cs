using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public List<Character> PartyMembers;

    public IEnumerable<Character> SelectedPartyMembers
    {
        get
        {
            return PartyMembers.Where(character => character.GetComponent<SelectableObject>().IsSelected);
        }
    }
    public IEnumerable<Character> AlivePartyMembers
    {
        get
        {
            return PartyMembers.Where(character => !character.IsDown);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
