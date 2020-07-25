using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameFlow
{
    /// <summary>
    /// A scene that could be loaded by the <see cref="LevelLoader"/>
    /// </summary>
    public enum SceneType
    {
        /// <summary>
        /// The generated level with the dungeon in which the game takes place.
        /// </summary>
        DungeonLevel,
        /// <summary>
        /// End credits.
        /// </summary>
        Credits, 
        /// <summary>
        /// The main menu.
        /// </summary>
        MainMenu
    }
}
