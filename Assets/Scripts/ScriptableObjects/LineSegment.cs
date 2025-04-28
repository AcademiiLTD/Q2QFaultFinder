using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LineSegment
{
    public CableType cableType;
    public float length;
}

public enum CableType
{
    WAVECON_185,
    CONSAC_185
}
