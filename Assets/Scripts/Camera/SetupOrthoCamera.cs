using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupOrthoCamera : MonoBehaviour {

    private Camera myCamera;
    private float ScoringYMin;
    [SerializeField]
    private RectTransform ScoringPanel;

	// Use this for initialization
	void Start () {
        myCamera = Camera.main;
        int maxBoardSize = Mathf.Max(PersistentData.instance.BoardXSize, PersistentData.instance.BoardYSize);
        ScoringYMin = ScoringPanel.anchorMin.y;
        Vector3 newPos = myCamera.transform.position;
        myCamera.orthographicSize = (maxBoardSize * 0.5f) / ScoringYMin;
        newPos.z = ((PersistentData.instance.BoardYSize / ScoringYMin) * 0.5f) - 0.5f;
        newPos.x = (PersistentData.instance.BoardXSize < PersistentData.instance.BoardYSize) ? (PersistentData.instance.BoardXSize + (maxBoardSize - PersistentData.instance.BoardXSize)*0.5f) : maxBoardSize ;
        myCamera.transform.position = newPos;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
