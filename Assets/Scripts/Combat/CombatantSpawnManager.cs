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
    /// <summary>
    /// A component responsible for spawning all combatants.
    /// When a combatant is spawned, it is spawned as a child of this component's game object, making combatants easy to find in the tree.
    /// </summary>
    public class CombatantSpawnManager : MonoBehaviour
    {
        /// <summary>
        /// Object with pathfinding data about the map, used so we know where we can spawn combatants.
        /// </summary>
        private PathfindingMapController pathfindingMapController;
        /// <summary>
        /// Grid onto which the combatants will be spawned.
        /// </summary>
        private Grid grid;
        /// <summary>
        /// The configuration of the used generator is used for determining which enemies are the most dangerous.
        /// Necessary because the designer can specify spawn points for the most dangerous enemies.
        /// </summary>
        private EncounterGeneratorConfiguration generatorConfiguration;

        public void Start()
        {
            generatorConfiguration = EncounterGeneratorConfiguration.CurrentConfig;
        }

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
            // Initialize references to necessary objects if not set yet.
            if (pathfindingMapController == null || grid == null)
            {
                pathfindingMapController = FindObjectOfType<PathfindingMapController>();
                grid = FindObjectOfType<Grid>();
            }
            // This list will be filled with combatants as we spawn them.
            var toReturn = new List<GameObject>(); 
            // List of all places where an enemy could spawn.
            var spawnTileCandidates = new List<Vector2Int>(spawnRoom.RoomSquaresPositions);
            // Find explicitly defined spawn points for this room if they exist.
            var roomSpawnPoints = FindObjectsOfType<SpawnPoint>().Where(spawnPoint =>
                spawnPoint.GetComponent<RoomInfoComponent>().RoomIndex == spawnRoom.Index);
            // Spawn points have types of creatures they can support, group them by those types.
            var spawnPointGroups = roomSpawnPoints.GroupBy(spawnPoint => spawnPoint.Type).ToDictionary(group => group.Key, group => group.ToList());
            // For each spawned character, pick which spawn point should it get.
            // This might actually be a bit complex, because we need to be able to spawn the most dangerous enemy somehow.
            var spawnPointAssigments = GetSpawnPointTypesForCombatants(combatantTemplates, spawnPointGroups);

            // In this set we will have the squares where we've already spawned someone.
            HashSet<Vector2Int> occupiedSquares = new HashSet<Vector2Int>();
            foreach (var combatantTemplate in combatantTemplates)
            {
                var combatantSpawnPointType = spawnPointAssigments[combatantTemplate.GetComponent<CombatantBase>()];
                // First, try to find a spawn point for the candidate.
                var spawnPointCandidates = spawnPointGroups.ContainsKey(combatantSpawnPointType)
                    ? spawnPointGroups[combatantSpawnPointType]
                    : null;

                if (spawnPointCandidates?.Any() == true)
                {
                    // Spawn point found, great! Remove it from the list of spawn points and just spawn the monster on that point.
                    var spawnPointForMonster = spawnPointCandidates.GetRandomElementOrDefault();
                    spawnPointCandidates.Remove(spawnPointForMonster);
                    var spawnPointOnGrid = grid.WorldToCell(spawnPointForMonster.transform.position);
                    toReturn.Add(SpawnCombatant(combatantTemplate, occupiedSquares,
                        new Vector2Int(spawnPointOnGrid.x, spawnPointOnGrid.y)));
                }
                else
                { 
                    // No spawn point, just spawn the monster anywhere in the room. 
                    toReturn.Add(SpawnCombatant(combatantTemplate, spawnTileCandidates, occupiedSquares, incomingDoors, minDistanceToDoor));
                }
            }

            return toReturn;
        }
        /// <summary>
        /// Spawns a monster somewhere in the room.
        /// </summary>
        /// <param name="combatantTemplate">The monster that should be spawned.</param>
        /// <param name="tiles"> Tiles where this combatant could spawn.</param>
        /// <param name="occupiedSquares">The list of spaces where the monster cannot spawn.</param>
        /// <param name="incomingDoors">Doors through which the party came.</param>
        /// <param name="minDistanceToDoor">Minimum distance from the doors to spawn point.</param>
        /// <returns></returns>
        private GameObject SpawnCombatant(GameObject combatantTemplate, List<Vector2Int> tiles, HashSet<Vector2Int> occupiedSquares, Doors incomingDoors, float minDistanceToDoor)
        {
            //This will get the map in which positions of other combatants are also marked as impassable.
            // TODO: Make this behavior more understandable
            var pathfindingMap = pathfindingMapController.GetPassabilityMapForCombatant(null);
            // Go through all the tiles. Check if the space satisfies all conditions. If not, remove it from the list of candidates and move on.
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
            UnityEngine.Debug.Assert(false, "Could not spawn combatant");
            return null;
        }
        /// <summary>
        /// Spawn a combatant at the specified position, adding the target space to the list of occupied spaces.
        /// </summary>
        /// <param name="combatantTemplate">Combatant to spawn.</param>
        /// <param name="occupiedSquares">Set of squares already occupied.</param>
        /// <param name="spawnTile">The space where the combatant should spawn.</param>
        /// <returns></returns>
        private GameObject SpawnCombatant(GameObject combatantTemplate, HashSet<Vector2Int> occupiedSquares, Vector2Int spawnTile)
        {
            occupiedSquares.Add(spawnTile);
            var combatantInstance = Instantiate(combatantTemplate);
            combatantInstance.transform.parent = transform;
            combatantInstance.transform.localPosition = new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, -1);
            combatantInstance.SetActive(true);
            return combatantInstance;
        }
        /// <summary>
        /// For each combatant, assign to him the appropriate type of spawn point where he should spawn.
        /// </summary>
        /// <param name="combatantsGameObjects">List of all combatants to spawn.</param>
        /// <param name="spawnPoints">List of all spawn points in the room. </param>
        /// <returns>A map assigning a spawn point type to each combatant template.</returns>
        private Dictionary<CombatantBase, SpawnPointType> GetSpawnPointTypesForCombatants(List<GameObject> combatantsGameObjects,
            Dictionary<SpawnPointType, List<SpawnPoint>> spawnPoints)
        {
            Dictionary<CombatantBase, SpawnPointType> toReturn = new Dictionary<CombatantBase, SpawnPointType>();

            var heroes = combatantsGameObjects.Select(combatantObject => combatantObject.GetComponent<Hero>())
                .Where(hero => hero != null);
            // Heroes are easy, each hero has one type of spawn point.
            foreach (var hero in heroes)
            {
                toReturn[hero] = SpawnPoint.GetSpawnPointTypeForCombatant(hero);
            }
            // For monsters, we need to order them by their dangerousness.
            // We calculate how many slots for most dangerous enemies are there and we spawn those enemies in those point.
            // Those who are not a most dangerous enemy will just have a spawn point assigned by their type.
            // Brutally ineffective LINQ expression, but we should not be dealing with many objects here.
            var monsters  = combatantsGameObjects.Select(combatantObject => combatantObject.GetComponent<Monster>())
                .Where(hero => hero != null).OrderByDescending(GetSpawnPointPriority).ToList();
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
        /// <summary>
        /// Retrieves how dangerous this monster is, whether it deserves the most dangerous spawn point.
        /// </summary>
        /// <param name="monster">Monster whose dangerousness level should be evaluated.</param>
        /// <returns>How dangerous the monster is and how much does it deserve the most dangerous spawn point. </returns>
        private float GetSpawnPointPriority(Monster monster)
        {
            // As far as spawn points are concerned, leaders should always be considered the most dangerous.
            if (monster.Role == MonsterRole.Leader)
            {
                return float.PositiveInfinity;
            }

            return generatorConfiguration.MonsterRankWeights[new MonsterType(monster.Rank, monster.Role)];
        }
    }
}