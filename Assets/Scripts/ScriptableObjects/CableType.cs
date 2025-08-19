using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cable Type")]
public class CableType : ScriptableObject
{
    public float thickness;
    public CableMaterial material;
    public float velocityFactor;
}

public enum CableMaterial
{
    ALUMINIUM,
    COPPER
}
