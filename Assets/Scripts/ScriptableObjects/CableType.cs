using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cable Type")]
public class CableType : ScriptableObject
{
    public float thickness;
    public CableMaterial material;
    public float velocityFactor; //Not sure how to actually use this yet
    //Contains GENERIC, REUSED DATA about the cable (material, thickness' e.c.t.)
}

public enum CableMaterial
{
    ALUMINIUM,
    COPPER
}
