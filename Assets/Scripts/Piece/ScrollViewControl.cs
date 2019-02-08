using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    public void ChangeSelectedFurniture(int FurnitureID)
    {
        BoardManager.instance.UpdatePiece(FurnitureID);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
