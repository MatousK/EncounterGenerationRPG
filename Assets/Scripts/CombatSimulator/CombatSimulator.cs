using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AI.HeroAI;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using UnityEngine;

namespace Assets.Scripts.CombatSimulator
{
    public class CombatSimulator: MonoBehaviour
    {
        public GameObject KnightSpawnPoint;
        public GameObject RangerSpawnPoint;
        public GameObject ClericSpawnPoint;
        public List<GameObject> EnemySpawnPoints;
        public GameObject KnightTemplate;
        public GameObject RangerTemplate;
        public GameObject ClericTemplate;
        public TestGenerator TestGenerator = new TestGenerator();
        public int CurrentTestIndex => testLogger.CurrentTestIndex;
        GameObject knight;
        GameObject ranger;
        GameObject cleric;
        CombatantsManager combatantsManager;
        Grid mapGrid;
        SimulatorMonstersGenerator monstersGenerator;
        EncounterDefinition currentTestEncounter;
        readonly TestResultLogger testLogger = new TestResultLogger();
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

        private void Update()
        {
            if (!combatantsManager.IsCombatActive || !combatantsManager.GetPlayerCharacters(onlyAlive: true).Any())
            {
                LogCombatResults();
                StartNewCombat();
            }
        }

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

        private void FillAiHeroReferences(HeroAiBase aiToFill)
        {
            aiToFill.Knight = knight.GetComponent<Hero>();
            aiToFill.Ranger = ranger.GetComponent<Hero>();
            aiToFill.Cleric = cleric.GetComponent<Hero>();
        }

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