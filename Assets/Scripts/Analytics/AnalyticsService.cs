using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.EncounterGenerator.Model;
using Assets.Scripts.GameFlow;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Analytics
{
    public class AnalyticsService : MonoBehaviour
    {
        /// <summary>
        /// Keep a count of how many times did we try to revoke the privacy agreement.
        /// </summary>
        public int RevokeAttemptIndex = 0;
        public int MaxRevokeAttempts = 100;
        public bool RevokedAgreement
        {
            get => UserGuid == Guid.Empty;
        }
        public bool DidFailToSendRevokeAgreement
        {
            get => RevokeAttemptIndex >= MaxRevokeAttempts;
        }
        public Guid UserGuid = Guid.NewGuid();
        public bool IsPendingKill;
        public GameObject RevokeActivityIndicatorTemplate;
        private List<MonsterType> orderedMonsterTypes = new List<MonsterType>
        {
            new MonsterType(MonsterRank.Minion, MonsterRole.Minion),
            new MonsterType(MonsterRank.Regular, MonsterRole.Brute),
            new MonsterType(MonsterRank.Elite, MonsterRole.Brute),
            new MonsterType(MonsterRank.Boss, MonsterRole.Brute),
            new MonsterType(MonsterRank.Regular, MonsterRole.Leader),
            new MonsterType(MonsterRank.Elite, MonsterRole.Leader),
            new MonsterType(MonsterRank.Boss, MonsterRole.Leader),
            new MonsterType(MonsterRank.Regular, MonsterRole.Lurker),
            new MonsterType(MonsterRank.Elite, MonsterRole.Lurker),
            new MonsterType(MonsterRank.Boss, MonsterRole.Lurker),
            new MonsterType(MonsterRank.Regular, MonsterRole.Sniper),
            new MonsterType(MonsterRank.Elite, MonsterRole.Sniper),
            new MonsterType(MonsterRank.Boss, MonsterRole.Sniper),
        };

        void Awake()
        {
            if (FindObjectsOfType<AnalyticsService>().Length > 1)
            {
                IsPendingKill = true;
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        /// <summary>
        /// Used to reset guid once the player finishes the game.
        /// </summary>
        public void ResetGuid()
        {
            UserGuid = Guid.NewGuid();
        }

        public void LogCombat(Dictionary<HeroProfession, float> partyStartHp, Dictionary<HeroProfession, float> partyEndHp, Dictionary<HeroProfession, float> partyAttack,
            EncounterDefinition encounter, float expectedDifficulty, float realDifficulty, bool wasGameOver, bool wasStatic, bool wasLogged)
        {
            List<string> lineCells = new List<string> {
                "Combat",
                UserGuid.ToString(),
                DateTime.Now.ToFileTimeUtc().ToString(),
            };
            lineCells.Add(partyStartHp[HeroProfession.Knight].ToString(CultureInfo.InvariantCulture));
            lineCells.Add(partyStartHp[HeroProfession.Ranger].ToString(CultureInfo.InvariantCulture));
            lineCells.Add(partyStartHp[HeroProfession.Cleric].ToString(CultureInfo.InvariantCulture));
            lineCells.Add(partyEndHp[HeroProfession.Knight].ToString(CultureInfo.InvariantCulture));
            lineCells.Add(partyEndHp[HeroProfession.Ranger].ToString(CultureInfo.InvariantCulture));
            lineCells.Add(partyEndHp[HeroProfession.Cleric].ToString(CultureInfo.InvariantCulture));
            lineCells.Add(partyAttack[HeroProfession.Knight].ToString(CultureInfo.InvariantCulture));
            lineCells.Add(partyAttack[HeroProfession.Ranger].ToString(CultureInfo.InvariantCulture));
            lineCells.Add(partyAttack[HeroProfession.Cleric].ToString(CultureInfo.InvariantCulture));
            foreach (var monsterType in orderedMonsterTypes)
            {
                var monsterCount =
                    encounter.AllEncounterGroups.FirstOrDefault(group => group.MonsterType == monsterType)?.MonsterCount ?? 0;
                lineCells.Add(monsterCount.ToString(CultureInfo.InvariantCulture));
            }
            lineCells.Add(expectedDifficulty.ToString(CultureInfo.InvariantCulture));
            lineCells.Add(realDifficulty.ToString(CultureInfo.InvariantCulture));
            lineCells.Add(wasGameOver ? "1" : "0");
            lineCells.Add(wasStatic ? "1" : "0");
            lineCells.Add(wasLogged ? "1" : "0");

            StartCoroutine(LogCsvLine(lineCells));
        }

        public void LogRevokeAndExit()
        {
            Instantiate(RevokeActivityIndicatorTemplate, null);
            List<string> lineCells = new List<string> {
                "RevokeAgreement",
                UserGuid.ToString(),
                DateTime.Now.ToFileTimeUtc().ToString(),
            };
            StartCoroutine(LogCsvLine(lineCells, true));
        }

        private IEnumerator LogCsvLine(IEnumerable<string> cells, bool isRevoke = false)
        {
            var line = string.Join(";", cells);
            var payload = $"{{\"payload\": \"{line}\" }}";
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            var www = UnityEngine.Networking.UnityWebRequest.Put("http://mattka.tcf2.cz/storeData.php", payloadBytes);
            www.SetRequestHeader("Accept", "application/json");
            yield return www.SendWebRequest();

            UnityEngine.Debug.Log(www.responseCode);
            AnalyticsResponse response;
            try
            {
                response = JsonUtility.FromJson<AnalyticsResponse>(www.downloadHandler.text);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
                response = null;
            }
            UnityEngine.Debug.Log(www.downloadHandler.text);

            if (isRevoke)
            {
                // TODO: Actually parse the json. We depend on only a single field.
                if (www.responseCode == 200 && response?.success == true)
                {
                    UserGuid = Guid.Empty;
                }
                else if (RevokeAttemptIndex++ < MaxRevokeAttempts)
                {
                    // Nothing to do but try again.
                    StartCoroutine(LogCsvLine(cells, isRevoke));
                }
            }
        }
    }
    [Serializable]
    class AnalyticsResponse
    {
        // Purposufelly lowercase to match server response.
        public bool success;
    }
}