using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DungeonGenerator;
using Assets.Scripts.EncounterGenerator.Configuration;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.Environment;
using Assets.Scripts.Extension;
using Assets.Scripts.Movement.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    public class CombatantSpawnManager : MonoBehaviour
    {

        // Used so we know where can we spawn monsters.
        PathfindingMapController pathfindingMapController;
        private Grid grid;
        /// <summary>
        /// Spawns a monster on the map.
        /// </summary>
        /// <param name="combatantTemplates">The monsters that should be spawned.</param>
        /// <param name="spawnRoom">The room in which the monster should be spawned.</param>
        /// <param name="incomingDoors">Doors through which the party came.</param>
        /// <param name="minDistanceToDoor">Minimum distance from the doors to spawn</param>
        /// <returns></returns>
        public List<GameObject> SpawnCombatants(List<GameObject> combatantTemplates, RoomInfo spawnRoom, Doors incomingDoors = null, float minDistanceToDoor = 0)
        {
            if (pathfindingMapController == null || grid == null)
            {
                pathfindingMapController = FindObjectOfType<PathfindingMapController>();
                grid = FindObjectOfType<Grid>();
            }
            var toReturn = new List<GameObject>();
            var spawnTileCandidates = new List<Vector2Int>(spawnRoom.RoomSquaresPositions);
            var roomSpawnPoints = FindObjectsOfType<SpawnPoint>().Where(spawnPoint =>
                spawnPoint.GetComponent<RoomInfoComponent>().RoomIndex == spawnRoom.Index);
            var spawnPointGroups = roomSpawnPoints.GroupBy(spawnPoint => spawnPoint.Type).ToDictionary(group => group.Key, group => group.ToList());

            var spawnPointAssigments = GetSpawnPointTypesForCombatants(combatantTemplates, spawnPointGroups);

            // In this set we will have the squares where we've already spawned someone.
            HashSet<Vector2Int> occupiedSquares = new HashSet<Vector2Int>();
            foreach (var combatantTemplate in combatantTemplates)
            {
                var combatantSpawnPointType = spawnPointAssigments[combatantTemplate.GetComponent<CombatantBase>()];
                var spawnPointCandidates = spawnPointGroups.ContainsKey(combatantSpawnPointType)
                    ? spawnPointGroups[combatantSpawnPointType]
                    : null;

                if (spawnPointCandidates?.Any() == true)
                {
                    var spawnPointForMonster = spawnPointCandidates.GetRandomElementOrDefault();
                    spawnPointCandidates.Remove(spawnPointForMonster);
                    var spawnPointOnGrid = grid.WorldToCell(spawnPointForMonster.transform.position);
                    toReturn.Add(SpawnCombatant(combatantTemplate, occupiedSquares,
                        new Vector2Int(spawnPointOnGrid.x, spawnPointOnGrid.y)));
                }
                else
                { 
                    // Fallback, just pick one.
                    toReturn.Add(SpawnCombatant(combatantTemplate, spawnTileCandidates, occupiedSquares, incomingDoors, minDistanceToDoor));
                }
            }

            return toReturn;
        }
        /// <summary>
        /// Spawns a monster on the map.
        /// </summary>
        /// <param name="combatantTemplate">The monster that should be spawned.</param>
        /// <param name="tiles"> Tiles where this combatant could spawn.</param>
        /// <param name="incomingDoors">Doors through which the party came.</param>
        /// <param name="minDistanceToDoor">Minimum distance from the doors to spawn</param>
        /// <returns></returns>
        private GameObject SpawnCombatant(GameObject combatantTemplate, List<Vector2Int> tiles, HashSet<Vector2Int> occupiedSquares, Doors incomingDoors, float minDistanceToDoor)
        {
            //This will get the map in which positions of other combatants are also marked as impassable.
            // TODO: Make this behavior more understandable
            var pathfindingMap = pathfindingMapController.GetPassabilityMapForCombatant(null);
            while (tiles.Any())
            {
                var spawnTileCandidateIndex = UnityEngine.Random.Range(0, tiles.Count);
                var spawnTileCandidate = tiles[spawnTileCandidateIndex];
                var distanceToDoor = incomingDoors == null ? float.PositiveInfinity : Vector2.Distance(incomingDoors.transform.localPosition, spawnTileCandidate);
                bool isTooClose = distanceToDoor < minDistanceToDoor;
                if (!pathfindingMap.GetSquareIsPassable(spawnTileCandidate) || isTooClose || occupiedSquares.Contains(spawnTileCandidate))
                {
                    tiles.RemoveAt(spawnTileCandidateIndex);
                    continue;
                }

                return SpawnCombatant(combatantTemplate, occupiedSquares, spawnTileCandidate);
            }
            return null;
        }

        private GameObject SpawnCombatant(GameObject combatantTemplate, HashSet<Vector2Int> occupiedSquares, Vector2Int spawnTileCandidate)
        {
            occupiedSquares.Add(spawnTileCandidate);
            var combatantInstance = Instantiate(combatantTemplate);
            combatantInstance.transform.parent = transform;
            combatantInstance.transform.localPosition = new Vector3(spawnTileCandidate.x + 0.5f, spawnTileCandidate.y + 0.5f, -1);
            combatantInstance.SetActive(true);
            return combatantInstance;
        }

        private Dictionary<CombatantBase, SpawnPointType> GetSpawnPointTypesForCombatants(List<GameObject> combatantsGameObjects,
            Dictionary<SpawnPointType, List<SpawnPoint>> spawnPoints)
        {
            // TODO: Get this from somewhere.
            EncounterGeneratorConfiguration generatorConfig = new EncounterGeneratorConfiguration();
            Dictionary<CombatantBase, SpawnPointType> toReturn = new Dictionary<CombatantBase, SpawnPointType>();

            var heroes = combatantsGameObjects.Select(combatantObject => combatantObject.GetComponent<Hero>())
                .Where(hero => hero != null);

            foreach (var hero in heroes)
            {
                toReturn[hero] = SpawnPoint.GetSpawnPointTypeForCombatant(hero);
            }
            // Brutally ineffective LINQ expression, but we should not be dealing with many objects here.
            var monsters  = combatantsGameObjects.Select(combatantObject => combatantObject.GetComponent<Monster>())
                .Where(hero => hero != null).OrderByDescending(monster => generatorConfig.MonsterRankWeights[new MonsterType(monster.Rank, monster.Role)]).ToList();
            var mostDangerousSlots = spawnPoints.ContainsKey(SpawnPointType.MostPowerfulEnemy) ? spawnPoints[SpawnPointType.MostPowerfulEnemy].Count : 0;
            foreach (var monster in monsters)
            {
                if (mostDangerousSlots-- > 0)
                {
                    toReturn[monster] = SpawnPointType.MostPowerfulEnemy;
                }
                else
                {
                    toReturn[monster] = SpawnPoint.GetSpawnPointTypeForCombatant(monster);
                }
            }

            return toReturn;
        }
    }
}