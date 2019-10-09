using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public List<Character> PlayableCharacters;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerable<Character> GetSelectedCharacters()
    {
        return PlayableCharacters.Where(character => character.GetComponent<SelectableObject>().IsSelected);
    }
}
