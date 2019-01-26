using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceHighlight : MonoBehaviour
{
    protected Material thisMaterial;

    public Color ValidColor;
    public Color InvalidColor;

    private int _x;
    public int X { get { return _x; } set { _x = value; } }
    private int _y;
    public int Y { get { return _y; } set { _y = value; } }

    protected Coroutine IsAnimatingColorCoroutine;

    protected void Awake()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        thisMaterial = renderer.material;

        EnableColor(Random.value > 0.5f, true);
    }

    public void EnableColor(bool isValid, bool pulseColor = false)
    {
        Color chosenColor = isValid ? ValidColor : InvalidColor;

        thisMaterial.color = chosenColor;

        if (pulseColor)
        {
            ClearPulseAnimation();
            IsAnimatingColorCoroutine = StartCoroutine(thisMaterial.PulseAlpha(chosenColor, 0.5f, loop:pulseColor));
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
        thisMaterial.color = new Color32(255, 255, 255, 0);
    }
}
