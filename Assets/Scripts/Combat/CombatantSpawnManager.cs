using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CombatantSpawnManager: MonoBehaviour
{
    // Used so we know where can we spawn monsters.
    PathfindingMapController PathfindingMapController;

    private void Start()
    {
        PathfindingMapController = FindObjectOfType<PathfindingMapController>();
    }
    public bool SpawnCombatant(GameObject combatantTemplate, RoomInfo spawnRoom)
    {
        var random = new System.Random();
        //This will get the map in which positions of other combatants are also marked as impassable.
        // TODO: Make this behavior more understandable
        var pathfindingMap = PathfindingMapController.GetPassabilityMapForCombatant(null);
        var spawnTileCandidates = new List<Vector2Int>(spawnRoom.RoomSquaresPositions);
        while (spawnTileCandidates.Any())
        {
            var spawnTileCandidateIndex = random.Next(spawnTileCandidates.Count);
            var spawnTileCandidate = spawnTileCandidates[spawnTileCandidateIndex];
            if (!pathfindingMap.GetSquareIsPassable(spawnTileCandidate))
            {
                spawnTileCandidates.RemoveAt(spawnTileCandidateIndex);
                continue;
            }
            var combatantInstance = Instantiate(combatantTemplate);
            combatantInstance.transform.parent = transform;
            combatantInstance.transform.localPosition = new Vector3Int(spawnTileCandidate.x, spawnTileCandidate.y, -1);
            combatantInstance.SetActive(true);
            return true;
        }
        return false;
    }
}