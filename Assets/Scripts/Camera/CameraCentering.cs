using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// This behavior centers the camera on the player characters. Will just find their center, if they're scattered, too bad.
/// </summary>
public class CameraCentering : MonoBehaviour
{
    CombatantsManager combatantsManager;
    private void Awake()
    {
        combatantsManager = FindObjectOfType<CombatantsManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Center();
    }

    public void Center()
    {
        var alivePlayerCharacters = combatantsManager.GetPlayerCharacters(onlyAlive: true).ToList();
        var playerPositions = alivePlayerCharacters.Select(character => character.transform.position);
        var playerCenter = playerPositions.Aggregate((position1, position2) => position1 + position2) / alivePlayerCharacters.Count;
        transform.position = new Vector3(playerCenter.x, playerCenter.y, transform.position.z);
    }
}
