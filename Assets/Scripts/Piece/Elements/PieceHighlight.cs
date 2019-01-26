using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceHighlight : MonoBehaviour
{
    protected Material thisMaterial;

    //public Color ValidColor;
    public Color InvalidColor;

    private int _x;
    public int X { get { return _x; } set { _x = value; } }
    private int _y;
    public int Y { get { return _y; } set { _y = value; } }

    public BoardGridLocation FoundGridPosition { get; protected set; }

    protected Coroutine IsAnimatingColorCoroutine;

    protected void Awake()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        thisMaterial = renderer.material;

        //EnableColor(Random.value > 0.5f, true);
    }

    public void EnableColor(bool isValid, bool pulseColor = false)
    {
        Color currentPlayerColor = BoardManager.instance.CurrentPlayerColor;
        Color chosenColor = isValid ? currentPlayerColor : InvalidColor;
        EnableColor(chosenColor, pulseColor);
    }

    public void EnableColor(Color chosenColor, bool pulseColor = false)
    {
        thisMaterial.color = chosenColor;

        if (pulseColor)
        {
            ClearPulseAnimation();
            IsAnimatingColorCoroutine = StartCoroutine(thisMaterial.PulseAlpha(chosenColor, 0.5f, 1f, loop: pulseColor));
        }
    }

    public void ClearPulseAnimation()
    {
        if(IsAnimatingColorCoroutine != null)
        {
            StopCoroutine(IsAnimatingColorCoroutine);
            IsAnimatingColorCoroutine = null;
        }
    }
    
    public void HideHighlight()
    {
        ClearPulseAnimation();
        thisMaterial.color = new Color32(0, 0, 0, 255);
    }

    public bool IsGridPositionValid()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down * 5);
        RaycastHit[] allHits = Physics.RaycastAll(ray);

        FoundGridPosition = null;

        for (int i = 0; i < allHits.Length; i++)
        {
            RaycastHit checkHit = allHits[i];

            BoardGridLocation gridLocation = checkHit.transform.GetComponent<BoardGridLocation>();

            if (gridLocation == null)
                continue;

            FoundGridPosition = gridLocation;

            return !gridLocation.InUse;
        }

        return false;
    }

    private void Update()
    {
    #if UNITY_EDITOR
        Debug.DrawRay(transform.position + Vector3.up, Vector3.down * 5);
    #endif
    }
}
