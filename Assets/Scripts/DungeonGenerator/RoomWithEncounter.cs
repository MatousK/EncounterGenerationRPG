using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using Assets.Scripts.EncounterGenerator.Configuration;

namespace Assets.Scripts.DungeonGenerator
{
    /// <summary>
    /// Extension of the Room class from Dungeon generator that includes info about encounter difficulties and treasures.
    /// </summary>
    public class RoomWithEncounter: Room
    {
        /// <summary>
        /// The encounter that should appear here.
        /// </summary>
        public EncounterConfiguration EncounterConfiguration;

        public int TreasureChestsMax;
        public int TreasureChestsMin;
    }
}
