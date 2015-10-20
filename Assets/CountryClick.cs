using System;
using UnityEngine;

public class CountryClick : ClickBehaviour {

	CountryNode lastNode;

	public CountryClick () {
	}
	
	public override void ClickUpdate() {
		if (Input.GetMouseButtonUp (0)) {
			Ray castRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			//Debug.Log ("Raycasting from " + castRay.origin.ToString());
			RaycastHit hit;
			if (Physics.Raycast (castRay, out hit) && hit.collider != null) {
				GameObject hitObject = hit.collider.gameObject;
				if(hitObject.GetComponent<CountryNode>() != null) {
					CountryNode node = hitObject.GetComponent<CountryNode>();
					if(node != lastNode) {
						lastNode.EstablishLink(node);
					} else {
						lastNode.menuOpen = !lastNode.menuOpen;
					}

					lastNode = null;
				}
			}
		}

		if ((Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown(1)) && GUIUtility.hotControl==0) { //We check that we haven't clicked on an active GUI element
			Ray castRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			//Debug.Log ("Raycasting from " + castRay.origin.ToString());
			RaycastHit hit;
			if (Physics.Raycast (castRay, out hit) && hit.collider != null) {
				GameObject hitObject = hit.collider.gameObject;

				if(hitObject.GetComponent<CountryObject>() != null) {
					CountryObject country = hitObject.GetComponent<CountryObject>();
					if(Input.GetMouseButtonDown (0)) {
						country.parentMenuOpen = !country.parentMenuOpen;
						country.menuOpen = false;
					} else if(Input.GetMouseButtonDown(1)) {
						country.menuOpen = !country.menuOpen;
						country.parentMenuOpen = false;
					}
				}

				if(hitObject.GetComponent<CountryNode>() != null) {
					CountryNode node = hitObject.GetComponent<CountryNode>();
					if(Input.GetMouseButtonDown (0)) {
						lastNode = node;
						node.StartCoroutine("LinkDraw");
					} else if(Input.GetMouseButtonDown (1)) {
						node.StartCoroutine ("NodeMove");
					}
				}
			}
		}
	}
}
