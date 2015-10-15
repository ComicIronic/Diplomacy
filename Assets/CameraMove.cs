using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	public float xBound = 100f;
	public float yBound = 100f;
	
	float zoomSpeed = 0.3f;

	float moveMod = 2f;
	
	// Use this for initialization
	void Start () {
		Camera.main.orthographicSize = 50;
	}
	
	// Update is called once per frame
	void Update () {
		CanvasCreator canvas = gameObject.GetComponent<CanvasCreator> ();
		if (canvas.currentState == EditorState.Creation) {
			return;
		}
		
		float vertLimit = yBound - Camera.main.orthographicSize;
		float horizLimit = xBound - (Camera.main.orthographicSize * (Screen.width / Screen.height));
		float vertMove = Mathf.Clamp (transform.position.y + Input.GetAxis ("Vertical") * moveMod, -vertLimit, vertLimit);
		float horizMove = Mathf.Clamp (transform.position.x + Input.GetAxis ("Horizontal") * moveMod, -horizLimit, horizLimit);
		
		float zoomMove = 0;
		if (Input.GetKey ("e")) {
			zoomMove = zoomSpeed;
		} else if (Input.GetKey ("q")) {
			zoomMove = -zoomSpeed;
		}
		transform.Translate (horizMove - transform.position.x, vertMove - transform.position.y, 0);
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize + zoomMove, 0, xBound/2);
	}
}
