using Assets.ProceduralLevelGenerator.Scripts.Data.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Extension of the Room class from Dungeon generator that includes info about encounter difficulties.
/// </summary>
public class RoomWithEncounter: Room
{
    /// <summary>
    /// The encounter that should appear here.
    /// </summary>
    public EncounterConfiguration EncounterConfiguration;
}
