using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AI.HeroAI;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.CombatSimulator
{
    /// <summary>
    /// Class for generating the matrix used for generating encounters.
    /// Tries to generate all possible combat scenarios in the game and log their results.
    /// It will try to find an existing log file and continue generating tests from the last test index which was logged.
    /// If no log file was found, this is a new test, so we start from zero.
    /// Has a deterministic system provided by <see cref="TestGenerator"/> which assigns to each test positive test index a test definition.
    /// This definition specifies how many monsters should spawn, how many power ups did the party pick up at this point and how should they distribute them.
    /// From this definition the class creates the actual list of monsters that should spawn and a stats for the attributes the hero controls.
    /// It will then spawn these creatures, lets them fight, stores the result and moves to the next combat.
    /// Note that this class attaches AI scripts to the individual heroes so the fight on their own.
    /// </summary>
    public class CombatSimulator: MonoBehaviour
    {
        /// <summary>
        /// Position where the knight should spawn.
        /// </summary>
        public GameObject KnightSpawnPoint;
        /// <summary>
        /// Position where the ranger should spawn.
        /// </summary>
        public GameObject RangerSpawnPoint;
        /// <summary>
        /// Position where the cleric should spawn.
        /// </summary>
        public GameObject ClericSpawnPoint;
        /// <summary>
        /// Positions where the enemies should spawn.
        /// </summary>
        public List<GameObject> EnemySpawnPoints;
        /// <summary>
        /// Template for the knight game object.
        /// </summary>
        public GameObject KnightTemplate;
        /// <summary>
        /// Template for the ranger game object.
        /// </summary>
        public GameObject RangerTemplate;
        /// <summary>
        /// Template for the cleric game object.
        /// </summary>
        public GameObject ClericTemplate;
        /// <summary>
        /// Class for retrieving test definitions this combat simulator should run.
        /// </summary>
        public TestGenerator TestGenerator = new TestGenerator();
        /// <summary>
        /// Index of the currently executed test.
        /// </summary>
        public int CurrentTestIndex => testLogger.CurrentTestIndex;
        /// <summary>
        /// The knight that is currently spawned and fighting.
        /// </summary>
        GameObject knight;
        /// <summary>
        /// The ranger that is currently spawned and fighting.
        /// </summary>
        GameObject ranger;
        /// <summary>
        /// The cleric that is currently spawned and fighting.
        /// </summary>
        GameObject cleric;
        /// <summary>
        /// Class which knows about all combatants existing in the game.
        /// </summary>
        CombatantsManager combatantsManager;
        /// <summary>
        /// The grid onto which the characters and monsters will be spawned.
        /// </summary>
        Grid mapGrid;
        /// <summary>
        /// Class for generating monsters that will fight in an encounter based on a test definition.
        /// </summary>
        SimulatorMonstersGenerator monstersGenerator;
        /// <summary>
        /// The encounter that is currently being simulated.
        /// </summary>
        EncounterDefinition currentTestEncounter;
        /// <summary>
        /// The logger which logs the results of individual tests.
        /// </summary>
        readonly TestResultLogger testLogger = new TestResultLogger();
        /// <summary>
        /// Called before <see cref="Update"/> is executed for the first time.
        /// Initializes references to dependencies, instantiates player characters and starts the first combat.
        /// </summary>
        private void Start()
        {
            // Simulator should be silent.
            AudioListener.volume = 0f;

            mapGrid = FindObjectOfType<Grid>();
            monstersGenerator = GetComponent<SimulatorMonstersGenerator>();
            combatantsManager = FindObjectOfType<CombatantsManager>();
            knight = Instantiate(KnightTemplate, mapGrid.transform);
            ranger = Instantiate(RangerTemplate, mapGrid.transform);
            cleric = Instantiate(ClericTemplate, mapGrid.transform);
            StartNewCombat();
        }
        /// <summary>
        /// Called in every frame. If the combat is over, which means that either the monsters or the players are dead, logs the results of the test and starts the next combat.
        /// </summary>
        private void Update()
        {
            if (!combatantsManager.IsCombatActive || !combatantsManager.GetPlayerCharacters(onlyAlive: true).Any())
            {
                LogCombatResults();
                StartNewCombat();
            }
        }
        /// <summary>
        /// Gathers the result of this combat and asks the <see cref="testLogger"/> to save it.
        /// </summary>
        private void LogCombatResults()
        {
            testLogger.LogResult(new TestResult
            {
                Cleric = cleric.GetComponent<Hero>(),
                Knight = knight.GetComponent<Hero>(),
                Ranger = ranger.GetComponent<Hero>(),
                MonsterTier = TestGenerator.MonsterTier,
                PartyConfiguration = TestGenerator.CurrentPartyConfiguration,
                PartyProvider = TestGenerator.CurrentPartyProvider,
                TestEncounter = TestGenerator.CurrentEncounter,
                TestIndex = CurrentTestIndex
            });
        }
        /// <summary>
        /// Generates the next encounter definition, cleans up the old monsters, heals and respawns the heroes and spawns the monsters.
        /// </summary>
        private void StartNewCombat()
        {
            TestGenerator.ReadyNextTest(CurrentTestIndex);
            // Remove old monsters
            var oldMonsters = combatantsManager.GetEnemies().ToList();
            foreach (var monster in oldMonsters)
            {
                combatantsManager.Enemies.Remove(monster);
                Destroy(monster.gameObject);
            }
            RespawnHeroes();
            // Spawn new monsters
            currentTestEncounter = TestGenerator.CurrentEncounter;
            SpawnMonsters(monstersGenerator.GenerateMonsters(currentTestEncounter));
        }
        /// <summary>
        /// Destroys all heroes and creates them once more with the stats saved in the <see cref="TestGenerator"/>.
        /// Also adds AI components to the heroes so they attack on their own.
        /// </summary>
        private void RespawnHeroes()
        {
            var partyStats = TestGenerator.CurrentPartyConfiguration;
            combatantsManager.PlayerCharacters.Remove(knight.GetComponent<Hero>());
            Destroy(knight);
            combatantsManager.PlayerCharacters.Remove(ranger.GetComponent<Hero>());
            Destroy(ranger);
            combatantsManager.PlayerCharacters.Remove(cleric.GetComponent<Hero>());
            Destroy(cleric);
            knight = SpawnHero(KnightTemplate, KnightSpawnPoint, partyStats.KnightStats);
            knight.GetComponent<AutoAttacking>().enabled = false;
            ranger = SpawnHero(RangerTemplate, RangerSpawnPoint, partyStats.RangerStats);
            ranger.GetComponent<AutoAttacking>().enabled = false;
            cleric = SpawnHero(ClericTemplate, ClericSpawnPoint, partyStats.ClericStats);
            cleric.GetComponent<AutoAttacking>().enabled = false;
            FillAiHeroReferences(knight.AddComponent<SimpleHeroAi>());
            FillAiHeroReferences(ranger.AddComponent<SimpleHeroAi>());
            FillAiHeroReferences(cleric.AddComponent<SimpleHeroAi>());
        }
        /// <summary>
        /// AI classes needs to know about all other heroes.
        /// This method fills these references.
        /// </summary>
        /// <param name="aiToFill">AI which needs references to individual heroes.</param>
        private void FillAiHeroReferences(HeroAiBase aiToFill)
        {
            aiToFill.Knight = knight.GetComponent<Hero>();
            aiToFill.Ranger = ranger.GetComponent<Hero>();
            aiToFill.Cleric = cleric.GetComponent<Hero>();
        }
        /// <summary>
        /// Spawns the specified hero at the specified location with specific stats.
        /// </summary>
        /// <param name="heroTemplate"> The object representing the hero to be spawned.</param>
        /// <param name="spawnPoint">The spawn point where the hero should spawn.</param>
        /// <param name="stats">Attributes of the character that should be spawned.</param>
        /// <returns></returns>
        private GameObject SpawnHero(GameObject heroTemplate, GameObject spawnPoint, PartyMemberConfiguration stats)
        {
            var hero = Instantiate(heroTemplate, mapGrid.transform);
            hero.transform.localPosition = spawnPoint.transform.localPosition;
            var heroCombatant = hero.GetComponent<CombatantBase>();
            heroCombatant.TotalMaxHitpoints = stats.MaxHp;
            heroCombatant.MaxHitpoints = stats.MaxHp;
            heroCombatant.HealDamage(stats.MaxHp * 2, heroCombatant, false);
            heroCombatant.Attributes.DealtDamageMultiplier = stats.AttackModifier;
            return hero;
        }
        /// <summary>
        /// Spawns the list of specific monsters.
        /// </summary>
        /// <param name="monstersToSpawn">Monsters which should be spawned.</param>
        private void SpawnMonsters(List<GameObject> monstersToSpawn)
        {
            var freeSpawnPoints = new List<GameObject>(EnemySpawnPoints);
            foreach (var monster in monstersToSpawn)
            {
                if (!freeSpawnPoints.Any())
                {
                    UnityEngine.Debug.LogWarning("Ran out of monster spawn points");
                    break;
                }
                var spawnPointIndex = UnityEngine.Random.Range(0, freeSpawnPoints.Count);
                var monsterSpawnPoint = freeSpawnPoints[spawnPointIndex];
                freeSpawnPoints.RemoveAt(spawnPointIndex);
                // Got the monster spawn point and the monster, just spawn it there.
                var newMonster = Instantiate(monster, mapGrid.transform);
                newMonster.transform.localPosition = monsterSpawnPoint.transform.localPosition;
            }
        }
    }
}