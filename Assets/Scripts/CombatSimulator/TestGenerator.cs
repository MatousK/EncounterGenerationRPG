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
            }
        }

        readonly List<PartyConfigurationProvider.PartyConfigurationProvider> partyProviders = new List<PartyConfigurationProvider.PartyConfigurationProvider>();
        readonly SimulatorEncounterProvider encounterProvider = new SimulatorEncounterProvider();

        const int TotalTestCount = 20000;
        const int MonsterCountTiers = 10;
        const int MonsterTierMonsterCountIncrement = 2;
        const int PowerupTiers = 4;
        const int PowerupTierIncrement = 4;
        const int EncountersPerMonsterCount = TotalTestCount / MonsterCountTiers;

        public PartyConfigurationProvider.PartyConfigurationProvider CurrentPartyProvider
        { get; private set; }
        public PartyConfiguration CurrentPartyConfiguration { get; private set; }
        public EncounterDefinition CurrentEncounter { get; private set; }
        public int MonsterTier { get; private set; }

        public void ReadyNextTest(int testIndex)
        {
            testIndex -= 26602; // Next round of simulations, we want to start it at zero.
            var partyProviderIndex = testIndex % partyProviders.Count;
            CurrentPartyProvider = partyProviders[partyProviderIndex];
            CurrentPartyConfiguration = CurrentPartyProvider.GetPartyConfiguration();
            if (partyProviderIndex == 0 || CurrentEncounter == null)
            {
                MonsterTier = testIndex / EncountersPerMonsterCount + 1;
                CurrentEncounter = encounterProvider.GetEncounter(MonsterTier * MonsterTierMonsterCountIncrement);
            }
            UnityEngine.Debug.Log("Monster tier: " + MonsterTier.ToString());
            UnityEngine.Debug.Log("Party provider: " + CurrentPartyProvider.ToString());
        }
    }

    public struct PartyConfiguration
    {
        public PartyMemberConfiguration KnightStats;
        public PartyMemberConfiguration RangerStats;
        public PartyMemberConfiguration ClericStats;
    }

    public struct PartyMemberConfiguration
    {
        public int MaxHp;
        public float AttackModifier;
    }
}