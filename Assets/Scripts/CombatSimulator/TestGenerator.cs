using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Combat;
using Assets.Scripts.CombatSimulator.PartyConfigurationProvider;
using Assets.Scripts.EncounterGenerator.Model;

namespace Assets.Scripts.CombatSimulator
{
    /// <summary>
    /// Class that is responsible for generating test scenarios.
    /// For a given test index retrieves the appropriate party configuration and an encounter.
    /// Tests are done in tiers. In each tier <see cref="TestsPerTier"/> tests are executed.
    /// Tiers specify how many monsters should spawn. It is always CurrentTier*<see cref="MonsterTierMonsterCountIncrement"/>.
    /// Party provider is specified by TestIndex mod number of party providers.
    /// This class is also a bit hacky in that once we generated the matrix via this process, we found some blind spaces in the matrix.
    /// These were filled by ad hoc tests which were executed if test index exceeded <see cref="TestsPerTier"/> * <see cref="MaxMonsterTier"/>.
    /// Note that as this class was only ever used in development, not much focus was spent on making it perfect.
    /// </summary>
    public class TestGenerator
    {
        /// <summary>
        /// The constructor initializes the list of party providers supported.
        /// </summary>
        public TestGenerator()
        {
            partyProviders.Add(new NoPowerupsPartyConfigurationProvider());

            for (int tier = 1; tier <= PowerupTiers; ++tier)
            {
                partyProviders.Add(new RangerPowerupsOnlyPartConfigurationProvider { TierIncrement = PowerupTierIncrement, TierIndex = tier });
                partyProviders.Add(new ClericPowerupsOnlyPartyConfigurationProvider { TierIncrement = PowerupTierIncrement, TierIndex = tier });
                partyProviders.Add(new KnightPowerupsOnlyPartyConfigurationProvider { TierIncrement = PowerupTierIncrement, TierIndex = tier });
                partyProviders.Add(new BalancedPartyConfigurationProvider { TierIncrement = PowerupTierIncrement, TierIndex = tier });
                partyProviders.Add(new RandomPartyConfigurationProvider { TierIncrement = PowerupTierIncrement, TierIndex = tier });
            }
        }
        /// <summary>
        /// The list of all party providers registered.
        /// </summary>
        readonly List<PartyConfigurationProvider.PartyConfigurationProvider> partyProviders = new List<PartyConfigurationProvider.PartyConfigurationProvider>();
        /// <summary>
        /// The class which can create encounters for a specified number of monsters.
        /// </summary>
        readonly SimulatorEncounterProvider encounterProvider = new SimulatorEncounterProvider();
        /// <summary>
        /// How many tests are executed per tier.
        /// </summary>
        private const int TestsPerTier = 4200;
        /// <summary>
        /// How many monsters are added in each tier.
        /// </summary>
        private const int MonsterTierMonsterCountIncrement = 2;
        /// <summary>
        /// How many tiers of power ups should be simulated. These specify how many power ups should the providers the providers distribute. Its TierIndex*<see cref="PowerupTierIncrement"/>.
        /// </summary>
        private const int PowerupTiers = 4;
        /// <summary>
        /// How many power ups are added to the number of power ups to distribute in each tier.
        /// </summary>
        private const int PowerupTierIncrement = 3;
        /// <summary>
        /// Max tier of monsters.
        /// This... This is a hack. After the tier exceeds this value, we execute a block of code which was used to fill blank areas of the matrix.
        /// </summary>
        private const int MaxMonsterTier = 10;
        /// <summary>
        /// Normally party providers were selected in sequence, guaranteeing that each will be used the same amount of times.
        /// However, in the latest hacked tests we wanted to quickly fill some specific areas, so we added a flag which could turn on
        /// completely random selection of party providers.
        /// </summary>
        private const bool RandomPartyConfiguration = false;
        /// <summary>
        /// The party provider that should be used in the next text. Filled by <see cref="ReadyNextTest"/>, always call that method before accessing this.
        /// </summary>
        public PartyConfigurationProvider.PartyConfigurationProvider CurrentPartyProvider
        { get; private set; }
        /// <summary>
        /// The party that should be used in the next text. Filled by <see cref="ReadyNextTest"/>, always call that method before accessing this.
        /// </summary>
        public PartyConfiguration CurrentPartyConfiguration { get; private set; }
        /// <summary>
        /// The encounter that should be used in the next text. Filled by <see cref="ReadyNextTest"/>, always call that method before accessing this.
        /// </summary>
        public EncounterDefinition CurrentEncounter { get; private set; }
        /// <summary>
        /// The current tier of monsters that are being tested. Filled by <see cref="ReadyNextTest"/>, always call that method before accessing this.
        /// </summary>
        public int MonsterTier { get; private set; }
        /// <summary>
        /// Calculates what should the next test look like.
        /// Fills the <see cref="CurrentEncounter"/> and <see cref="CurrentPartyConfiguration"/> to be valid for the test with the specified test index.
        /// </summary>
        /// <param name="testIndex">Index of the test to be executed.</param>
        public void ReadyNextTest(int testIndex)
        {
            var partyProviderIndex = RandomPartyConfiguration ? UnityEngine.Random.Range(0, partyProviders.Count) : testIndex % partyProviders.Count;
            CurrentPartyProvider = partyProviders[partyProviderIndex];
            CurrentPartyConfiguration = CurrentPartyProvider.GetPartyConfiguration();
            MonsterTier = testIndex / TestsPerTier + 1;
            if (MonsterTier > MaxMonsterTier)
            {
                MonsterTier = UnityEngine.Random.Range(11, 16);
            }
            CurrentEncounter = encounterProvider.GetEncounter(MonsterTier * MonsterTierMonsterCountIncrement);
            UnityEngine.Debug.Log("Monster tier: " + MonsterTier.ToString());
            UnityEngine.Debug.Log("Party provider: " + CurrentPartyProvider.ToString());
        }
    }
    /// <summary>
    /// Defines attributes of all party members.
    /// </summary>
    public class PartyConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartyConfiguration"/> class.
        /// </summary>
        public PartyConfiguration() { }
        /// <summary>
        /// Creates a party configuration which represents the given list of heroes.
        /// </summary>
        /// <param name="fromHeroes"></param>
        public PartyConfiguration(Hero[] fromHeroes)
        {
            var knight = fromHeroes.FirstOrDefault(hero => hero.HeroProfession == HeroProfession.Knight);
            KnightStats = knight != null ? new PartyMemberConfiguration(knight) : default;
            var ranger = fromHeroes.FirstOrDefault(hero => hero.HeroProfession == HeroProfession.Ranger);
            RangerStats = ranger != null ? new PartyMemberConfiguration(ranger) : default;
            var cleric = fromHeroes.FirstOrDefault(hero => hero.HeroProfession == HeroProfession.Cleric);
            ClericStats = cleric != null ? new PartyMemberConfiguration(cleric) : default;
        }
        /// <summary>
        /// Attributes of the knight this class represents.
        /// </summary>
        public PartyMemberConfiguration KnightStats;
        /// <summary>
        /// Attributes of the ranger this class represents.
        /// </summary>
        public PartyMemberConfiguration RangerStats;
        /// <summary>
        /// Attributes of the cleric this class represents.
        /// </summary>
        public PartyMemberConfiguration ClericStats;
        /// <summary>
        /// Retrieve the attributes of the specified hero.
        /// </summary>
        /// <param name="profession">The profession of the hero whose stats are requested.</param>
        /// <returns>The attributes of the specified hero.</returns>
        public PartyMemberConfiguration? GetStatsFor(HeroProfession profession)
        {
            switch (profession)
            {
                case HeroProfession.Cleric:
                    return ClericStats;
                case HeroProfession.Ranger:
                    return RangerStats;
                case HeroProfession.Knight:
                    return KnightStats;
            }
            UnityEngine.Debug.Assert(false, "Requesting stats for unknown hero.");
            return null;
        }
    }
    /// <summary>
    /// The attributes of the specified hero.
    /// </summary>
    public struct PartyMemberConfiguration
    {
        /// <summary>
        /// Initializes a new instance of this class and fills the attributes by the attributes of the specified hero.
        /// </summary>
        /// <param name="fromHero">Hero whose attributes should fill this class.</param>
        public PartyMemberConfiguration(Hero fromHero)
        {
            MaxHp = (int)fromHero.TotalMaxHitpoints;
            AttackModifier = fromHero.Attributes.DealtDamageMultiplier;
        }
        /// <summary>
        /// Maximum HP of the represented character.
        /// </summary>
        public int MaxHp;
        /// <summary>
        /// The attack modifier of the represented character.
        /// </summary>
        public float AttackModifier;
    }
}