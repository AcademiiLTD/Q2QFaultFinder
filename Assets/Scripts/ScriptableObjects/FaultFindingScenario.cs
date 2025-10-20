using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fault Finding Scenario")]
public class FaultFindingScenario : ScriptableObject
{
    [TextArea]
    public string description;
    public string date;
    public List<LineSegment> _lineSegments;
    public Month month;
    public Sprite mapImage;
    public float faultDistance;
    public float mapMetersPerPixel;
    public Vector2 faultPosition;
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
    public CableType cable;
    public float length;
    public float thickness;
}
