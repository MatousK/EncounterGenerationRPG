using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameFlow
{
    public class GameOverHintManager : MonoBehaviour
    {
        public List<GameOverHint> AllHints;
        private int currentHintIndex = 0;

        private void Start()
        {
            if (FindObjectsOfType<GameOverHintManager>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public GameOverHint GetNextHint()
        {
            var toReturn = AllHints[currentHintIndex++];
            currentHintIndex = currentHintIndex % AllHints.Count;
            return toReturn;
        }
    }

    [Serializable]
    public class GameOverHint
    {
        /// <summary>
        /// The actual hint to be shown.
        /// </summary>
        [TextArea(3, 6)]
        public string Text;
    }

}
