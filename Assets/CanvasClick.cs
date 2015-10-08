using UnityEngine;
using System.Collections;

public class CanvasClick : MonoBehaviour {

	public CanvasCreator parentCC;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnMouseDown() {
		CNode newNode = new GameObject ().AddComponent<CNode> ();
		newNode.Initialise(Camera.main.ScreenToWorldPoint (Input.mousePosition) + new Vector3(0,0,10), parentCC);
	}
}
