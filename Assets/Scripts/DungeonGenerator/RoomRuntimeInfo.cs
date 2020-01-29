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
    private bool _IsExplored;
    public bool IsExplored {
        get
        {
            return _IsExplored;
        }
        set
        {
            if (_IsExplored == value)
            {
                return;
            }
            _IsExplored = value;
            IsExploredChanged?.Invoke(this, _IsExplored);
        }
    }
}
