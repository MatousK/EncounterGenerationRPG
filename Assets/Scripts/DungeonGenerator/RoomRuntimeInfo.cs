using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class RoomInfo
{
    public EncounterConfiguration RoomEncounter;
    public List<Vector2Int> RoomSquaresPositions;
    public bool IsStartingRoom;
    public event EventHandler<bool> IsExploredChanged;
    [SerializeField]
    private bool _Explored;
    public bool IsExplored {
        get
        {
            return _Explored;
        }
        set
        {
            if (_Explored == value)
            {
                return;
            }
            _Explored = value;
            IsExploredChanged?.Invoke(this, _Explored);
        }
    }
}
