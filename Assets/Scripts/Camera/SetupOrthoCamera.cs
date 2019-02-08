using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupOrthoCamera : MonoBehaviour {

    private Camera myCamera;
    private float ScoringYMin;
    [SerializeField]
    private RectTransform ScoringPanel;
	[SerializeField]
	private SpriteRenderer BackgroundImage;	// So we can change the scale at runtime to better fit the aspect ratio of the screen.

	// Use this for initialization
	void Start () {
        myCamera = Camera.main;
        ScoringYMin = ScoringPanel.anchorMin.y;
        Vector3 newPos = myCamera.transform.position;
		// depending on the aspect ratio, even if there are more blocks in the y direction, may need to base the camera size on the size of blocks in the x direction
		// Adding 0.5 onto the board sizes so the board isn't hard up against any edges
		myCamera.orthographicSize = Mathf.Max( ((PersistentData.instance.BoardYSize + 0.5f) * 0.5f) / ScoringYMin, (PersistentData.instance.BoardXSize + 0.5f) / myCamera.aspect) ;
        newPos.z = ((PersistentData.instance.BoardYSize / ScoringYMin) * 0.5f) - 0.5f;

		// myCamera.size * myCamera.aspect = sizeOneBlocks that can fit on left half of screen.
		// so half of that is where the midpoint of what blocks there are should be.
		// so:  myCamera.size * myCamera.aspect * 0.5 + PersistentData.instance.BoardXSize * 0.5 is where left hand side of blocks should be
		// and just need to subtract 0.5 to take into account that the blocks are positions by their centers, not their edge
		newPos.x = (myCamera.orthographicSize * myCamera.aspect * 0.5f) + (PersistentData.instance.BoardXSize * 0.5f) - 0.5f;
        myCamera.transform.position = newPos;


		// myCaera.orthographicSize is world units from center of screen to edge vertically
		// that * pixelsPerUnit * 2 is how many pixels are needed to fill the screen vertically
		// multiply that by myCamera.aspect for the number of horizontal pixels
		// those numbers / texture.width/height should give the scale value needed to make the backgroundimage fill the screen.
		if (BackgroundImage != null) {
			Vector3 newScale = new Vector3 {
				x = myCamera.orthographicSize * myCamera.aspect * BackgroundImage.sprite.pixelsPerUnit * 2 / BackgroundImage.sprite.texture.width,
				y = myCamera.orthographicSize * BackgroundImage.sprite.pixelsPerUnit * 2 / BackgroundImage.sprite.texture.height,
				z = 1
			};

			BackgroundImage.transform.localScale = newScale;
		}
	}

}
