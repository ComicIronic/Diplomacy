using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	
	public float x_bound = 100f;
	public float y_bound = 100f;
	
	float zoomSpeed = 0.3f;
	
	float moveMod = 2f;
	
	// Use this for initialization
	void Start () {
		Camera.main.orthographicSize = 50;
	}
	
	// Update is called once per frame
	void Update () {
		GameObject canvas = gameObject.GetComponent<CanvasCreator> ().canvas;
		if (canvas == null) {
			return;
		}

		CanvasClick canvasC = canvas.GetComponent<CanvasClick> ();
		x_bound = canvasC.xBound;
		y_bound = canvasC.yBound;
		
		float vertLimit = y_bound - Camera.main.orthographicSize;
		float horizLimit = x_bound - (Camera.main.orthographicSize * (Screen.width / Screen.height));
		float vertMove = Mathf.Clamp (transform.position.y + Input.GetAxis ("Vertical") * moveMod, -vertLimit, vertLimit);
		float horizMove = Mathf.Clamp (transform.position.x + Input.GetAxis ("Horizontal") * moveMod, -horizLimit, horizLimit);
		
		float zoomMove = 0;
		if (Input.GetKey ("e")) {
			zoomMove = zoomSpeed;
		} else if (Input.GetKey ("q")) {
			zoomMove = -zoomSpeed;
		}
		transform.Translate (horizMove - transform.position.x, vertMove - transform.position.y, 0);
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize + zoomMove, 0, x_bound/2);
		
	}
}