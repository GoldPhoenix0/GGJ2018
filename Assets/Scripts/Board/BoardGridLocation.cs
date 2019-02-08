using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGridLocation : MonoBehaviour
{
    private int _x;
    public int X { get { return _x; } set { _x = value; } }
    private int _y;
    public int Y { get { return _y; } set { _y = value; } }

    public MeshRenderer ThisRenderer;
    public BoxCollider Collider;
    public Material[] PossibleMaterials;

    public void SetMaterial(int materialType)
    {
        ThisRenderer.material = PossibleMaterials[materialType % PossibleMaterials.Length];
    }

    public bool InUse { get; set; }

    public float Width { get { return Collider.bounds.size.x; } }
}
