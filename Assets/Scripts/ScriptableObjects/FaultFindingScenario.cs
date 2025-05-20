using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fault Finding Scenario")]
public class FaultFindingScenario : ScriptableObject
{
    public List<LineSegment> _lineSegments;
    public Month month;
    public Sprite mapImage;
}

public enum Month
{
    JAN,
    FEB,
    MAR,
    APR,
    MAY,
    JUN,
    JUL,
    AUG,
    SEP,
    OCT,
    NOV,
    DEC
}

[Serializable]
public class LineSegment
{
    public CableType cableType;
    public float length;
}

public enum CableType
{
    CNE,
    PILC_cu,
    CONSAC,
    PILC_al
}
