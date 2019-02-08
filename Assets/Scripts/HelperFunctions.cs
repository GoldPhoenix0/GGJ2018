using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{

	public static T InstantiateChild<T>(this Transform transform, GameObject prefab, string name = null) 
    {
        if (prefab == null)
            return default(T);

        GameObject newObject = GameObject.Instantiate(prefab, transform);
        Transform newTransform = newObject.transform;

        if (!string.IsNullOrEmpty(name))
            newObject.gameObject.name = name;
        newTransform.localPosition = Vector3.zero;
        newTransform.localScale = Vector3.one;
        newTransform.localRotation = Quaternion.identity;

        return newObject.GetComponent<T>();
    }

    public static IEnumerator PulseAlpha(this Material material, Color startColor, float targetAlpha, float duration = 1f, bool loop = true)
    {
        if (duration <= 0)
            duration = 1f;

        material.color = startColor;

        int numLoops = 0;
        float currentTime = 0f;
        float startTargetAlpha = startColor.a;
        float currentStartAlpha = startTargetAlpha;
        float currentTargetAlpha = targetAlpha;
        while (material != null)
        {
            yield return null;
            currentTime += Time.deltaTime;

            if (currentTime >= duration)
            {
                numLoops++;

                if (!loop && numLoops > 1)
                    break;

                currentTime = 0f;

                bool isApproxEqual = Mathf.Approximately(currentTargetAlpha, targetAlpha);

                currentTargetAlpha = isApproxEqual ? startTargetAlpha : targetAlpha;
                currentStartAlpha = isApproxEqual ? targetAlpha : startTargetAlpha;

            }

            Color color = material.color;
            color.a = Mathf.Lerp(currentStartAlpha, currentTargetAlpha, currentTime / duration); //Mathf.Lerp(color.a, currentTargetAlpha, Easings.Interpolate(currentTime / duration, Easings.Functions.SineEaseInOut)); //Mathf.SmoothStep(color.a, currentTargetAlpha, currentTime / duration);
            material.color = color;
        }

        material.color = startColor;
    }

    public static float GetValueBetween2Numbers(float current, float min, float max)
    {
        return (current - min) / (max - min);
    }

    public static string ConvertNumberToText(int number)
    {
        if (number == 1)
            return "One";
        else if (number == 2)
            return "Two";
        else if (number == 3)
            return "Three";
        else if (number == 4)
            return "Four";
        else if (number == 5)
            return "Five";
        else if (number == 6)
            return "Six";
        else if (number == 7)
            return "Seven";
        else if (number == 8)
            return "Eight";
        else if (number == 9)
            return "Nine";
        else if (number == 10)
            return "Ten";


        return "Thingamabob";
    }
}
