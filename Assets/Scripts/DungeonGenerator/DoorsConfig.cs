using Assets.ProceduralLevelGenerator.Scripts.Pipeline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon generator/Encounter Generation RPG/Doors task", fileName = "DoorsTask")]
public class DoorsConfig : PipelineConfig
{
    public bool AddDoors;

    public GameObject VerticalDoorsTop;

    public GameObject VerticalDoorsBottom;

    public GameObject HorizontalDoorsLeft;

    public GameObject HorizontalDoorsRight;
}