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
    public class TestGenerator
    {
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

        readonly List<PartyConfigurationProvider.PartyConfigurationProvider> partyProviders = new List<PartyConfigurationProvider.PartyConfigurationProvider>();
        readonly SimulatorEncounterProvider encounterProvider = new SimulatorEncounterProvider();

        private const int TestsPerTier = 2100;
        private const int MonsterTierMonsterCountIncrement = 2;
        private const int PowerupTiers = 4;
        private const int PowerupTierIncrement = 3;

        public PartyConfigurationProvider.PartyConfigurationProvider CurrentPartyProvider
        { get; private set; }
        public PartyConfiguration CurrentPartyConfiguration { get; private set; }
        public EncounterDefinition CurrentEncounter { get; private set; }
        public int MonsterTier { get; private set; }

        public void ReadyNextTest(int testIndex)
        {
            var partyProviderIndex = testIndex % partyProviders.Count;
            CurrentPartyProvider = partyProviders[partyProviderIndex];
            CurrentPartyConfiguration = CurrentPartyProvider.GetPartyConfiguration();
            MonsterTier = testIndex / TestsPerTier + 1;
            CurrentEncounter = encounterProvider.GetEncounter(MonsterTier * MonsterTierMonsterCountIncrement);
            UnityEngine.Debug.Log("Monster tier: " + MonsterTier.ToString());
            UnityEngine.Debug.Log("Party provider: " + CurrentPartyProvider.ToString());
        }
    }

    public class PartyConfiguration
    {
        public PartyConfiguration() { }

        public PartyConfiguration(Hero[] fromHeroes)
        {
            var knight = fromHeroes.FirstOrDefault(hero => hero.HeroProfession == HeroProfession.Knight);
            KnightStats = knight != null ? new PartyMemberConfiguration(knight) : default;
            var ranger = fromHeroes.FirstOrDefault(hero => hero.HeroProfession == HeroProfession.Ranger);
            RangerStats = ranger != null ? new PartyMemberConfiguration(ranger) : default;
            var cleric = fromHeroes.FirstOrDefault(hero => hero.HeroProfession == HeroProfession.Cleric);
            ClericStats = cleric != null ? new PartyMemberConfiguration(cleric) : default;
        }

        public PartyMemberConfiguration KnightStats;
        public PartyMemberConfiguration RangerStats;
        public PartyMemberConfiguration ClericStats;

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

    public struct PartyMemberConfiguration
    {
        public PartyMemberConfiguration(Hero fromHero)
        {
            MaxHp = (int)fromHero.TotalMaxHitpoints;
            AttackModifier = fromHero.Attributes.DealtDamageMultiplier;
        }
        public int MaxHp;
        public float AttackModifier;
    }
}