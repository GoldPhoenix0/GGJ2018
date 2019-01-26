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

        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        newObject.transform.localRotation = Quaternion.identity;

        return newObject.GetComponent<T>();
    }
}
