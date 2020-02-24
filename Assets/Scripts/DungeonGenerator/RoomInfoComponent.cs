using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Component for game objects which need to know in which room do they exist.
/// </summary>
public class RoomInfoComponent: MonoBehaviour
{
    /// <summary>
    /// Index of room where this game object can be found.
    /// </summary>
    public int RoomIndex;
}