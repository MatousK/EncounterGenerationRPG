using EncounterGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CombatSimulator: MonoBehaviour
{
    public GameObject KnightSpawnPoint;
    public GameObject RangerSpawnPoint;
    public GameObject ClericSpawnPoint;
    public List<GameObject> EnemySpawnPoints;
    public GameObject KnightTemplate;
    public GameObject RangerTemplate;
    public GameObject ClericTemplate;
    GameObject Knight;
    GameObject Ranger;
    GameObject Cleric;
    CombatantsManager CombatantsManager;
    Grid MapGrid;
    SimulatorMonstersGenerator MonstersGenerator;
    EncounterDefinition CurrentTestEncounter;
    TestGenerator TestGenerator = new TestGenerator();
    private void Start()
    {
        MapGrid = FindObjectOfType<Grid>();
        MonstersGenerator = GetComponent<SimulatorMonstersGenerator>();
        CombatantsManager = FindObjectOfType<CombatantsManager>();
        Knight = Instantiate(KnightTemplate, MapGrid.transform);
        Ranger = Instantiate(RangerTemplate, MapGrid.transform);
        Cleric = Instantiate(ClericTemplate, MapGrid.transform);
        StartNewCombat();
    }

    private void Update()
    {
        if (!CombatantsManager.IsCombatActive || !CombatantsManager.GetPlayerCharacters(onlyAlive: true).Any())
        {
            StartNewCombat();
        }
    }

    private void StartNewCombat()
    {
        TestGenerator.ReadyNextTest();
        // Remove old monsters
        var oldMonsters = CombatantsManager.GetEnemies().ToList();
        foreach (var monster in oldMonsters)
        {
            CombatantsManager.Enemies.Remove(monster);
            Destroy(monster.gameObject);
        }
        RespawnHeroes();
        // Spawn new monsters
        CurrentTestEncounter = TestGenerator.GetCurrentTestEncounter();
        SpawnMonsters(MonstersGenerator.GenerateMonsters(CurrentTestEncounter));
    }

    private void RespawnHeroes()
    {
        var partyStats = TestGenerator.GetCurrentPartyConfiguration();
        CombatantsManager.PlayerCharacters.Remove(Knight.GetComponent<Hero>());
        Destroy(Knight);
        CombatantsManager.PlayerCharacters.Remove(Ranger.GetComponent<Hero>());
        Destroy(Ranger);
        CombatantsManager.PlayerCharacters.Remove(Cleric.GetComponent<Hero>());
        Destroy(Cleric);
        Knight = SpawnHero(KnightTemplate, KnightSpawnPoint, partyStats.KnightStats);
        Knight.GetComponent<AutoAttacking>().enabled = false;
        Ranger = SpawnHero(RangerTemplate, RangerSpawnPoint, partyStats.RangerStats);
        Ranger.GetComponent<AutoAttacking>().enabled = false;
        Cleric = SpawnHero(ClericTemplate, ClericSpawnPoint, partyStats.ClericStats);
        Cleric.GetComponent<AutoAttacking>().enabled = false;
        FillAIHeroReferences(Knight.AddComponent<KnightAI>());
        FillAIHeroReferences(Ranger.AddComponent<RangerAI>());
        FillAIHeroReferences(Cleric.AddComponent<ClericAI>());
    }

    private void FillAIHeroReferences(HeroAIBase aiToFill)
    {
        aiToFill.Knight = Knight.GetComponent<Hero>();
        aiToFill.Ranger = Ranger.GetComponent<Hero>();
        aiToFill.Cleric = Cleric.GetComponent<Hero>();
    }

    private GameObject SpawnHero(GameObject heroTemplate, GameObject spawnPoint, PartyMemberConfiguration stats)
    {
        var hero = Instantiate(heroTemplate, MapGrid.transform);
        hero.transform.localPosition = spawnPoint.transform.localPosition;
        var heroCombatant = hero.GetComponent<CombatantBase>();
        heroCombatant.TotalMaxHitpoints = stats.MaxHP;
        heroCombatant.MaxHitpoints = stats.MaxHP;
        heroCombatant.HealDamage(stats.MaxHP * 2, heroCombatant, false);
        heroCombatant.Attributes.DealtDamageMultiplier = stats.AttackModifier;
        return hero;
    }

    private void SpawnMonsters(List<GameObject> monstersToSpawn)
    {
        var freeSpawnPoints = new List<GameObject>(EnemySpawnPoints);
        for (int i = 0; i < monstersToSpawn.Count; ++i)
        {
            if (!freeSpawnPoints.Any())
            {
                Debug.LogWarning("Ran out of monster spawn points");
                break;
            }
            var spawnPointIndex = UnityEngine.Random.Range(0, freeSpawnPoints.Count);
            var monsterSpawnPoint = freeSpawnPoints[spawnPointIndex];
            freeSpawnPoints.RemoveAt(spawnPointIndex);
            // Got the monster spawn point and the monster, just spawn it there.
            var newMonster = Instantiate(monstersToSpawn[i], MapGrid.transform);
            newMonster.transform.localPosition = monsterSpawnPoint.transform.localPosition;
        }
    }
}