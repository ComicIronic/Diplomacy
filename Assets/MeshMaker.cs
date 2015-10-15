using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshMaker
{

	//Given a 2d mesh game object, will spit out a black outline for it
	//Thickness is based on the outlineThickness variable
	//You don't need to know how it works, only that it does
	/* Extends a point in each direction through the angular bisector of the two neighbours of a vertex
	 * These two points are then used to form quads with the other pairs of points of other verteces
	 * Outer points are correctly matched to other outer points, and inner points to inner points
	 * Each quad is then divided by shortest-distance diagonals, and the triangles placed into a new mesh
	 */
	public static void GenerateOutline(GameObject toOutline) {
		Mesh meshToWork = toOutline.GetComponent<MeshFilter>().mesh;
		Mesh outMesh = new Mesh ();

		Debug.Log ("Generating new outline");
		List<List<Vector3>> tempTriangles = new List<List<Vector3>> ();
		Dictionary<Vector3, List<Vector3>> junctions = new Dictionary<Vector3, List<Vector3>> ();
	
		for (int i = 0; i < meshToWork.vertexCount; i++) {
		
			int nextIn = i + 1;
			int prevIn = i - 1;
			if (nextIn == meshToWork.vertexCount) {
				nextIn = 0;
			}
			if (prevIn == -1) {
				prevIn = meshToWork.vertexCount - 1;
			}
		
			float angle = GetCWAngle (meshToWork.vertices [i], meshToWork.vertices [prevIn], meshToWork.vertices [nextIn]);
			float distance = 1f / (Mathf.Sin (angle / 2));
		
			float prevAngle = Mathf.Atan2 (meshToWork.vertices [i].y - meshToWork.vertices [prevIn].y, meshToWork.vertices [i].x - meshToWork.vertices [prevIn].x);
		
			float angleChange = prevAngle - angle / 2; //We subtract the halfangle because CW is negative
		
			Vector3 lineChange = new Vector3 (Mathf.Cos (angleChange) * distance, Mathf.Sin (angleChange) * distance, 0f);
		
			Vector3 newOuter = meshToWork.vertices [i] + lineChange;
			Vector3 newInner = meshToWork.vertices [i] - lineChange;
		
			//Debug.Log("Generated outer point of " + newOuter + " from" + meshToWork.vertices[i]);
			//Debug.Log("Generated inner point of " + newInner + " from" + meshToWork.vertices[i]);
		
			junctions.Add (meshToWork.vertices [i], new List<Vector3> ());
			junctions [meshToWork.vertices [i]].Add (newOuter);
			junctions [meshToWork.vertices [i]].Add (newInner);

			//Connect up to our previous point
			if (junctions.ContainsKey (meshToWork.vertices [prevIn])) {
				Vector3 lastOuter = junctions [meshToWork.vertices [prevIn]] [0];
				Vector3 lastInner = junctions [meshToWork.vertices [prevIn]] [1];
			
				if (GetDistance (newOuter, lastInner) < GetDistance (lastOuter, newInner)) {
					List<Vector3> newTriangle = new List<Vector3> ();
					newTriangle.Add (lastOuter);
					newTriangle.Add (newOuter);
					newTriangle.Add (lastInner);
					tempTriangles.Add (newTriangle);
					newTriangle = new List<Vector3> ();
					newTriangle.Add (lastInner);
					newTriangle.Add (newOuter);
					newTriangle.Add (newInner);
					tempTriangles.Add (newTriangle);
				} else {
					List<Vector3> newTriangle = new List<Vector3> ();
					newTriangle.Add (lastInner);
					newTriangle.Add (lastOuter);
					newTriangle.Add (newInner);
					tempTriangles.Add (newTriangle);
					newTriangle = new List<Vector3> ();
					newTriangle.Add (lastOuter);
					newTriangle.Add (newOuter);
					newTriangle.Add (newInner);
					tempTriangles.Add (newTriangle);
				}
			}

			//Connect up to the next point
			if (junctions.ContainsKey (meshToWork.vertices [nextIn])) {
				Vector3 lastOuter = junctions [meshToWork.vertices [nextIn]] [0];
				Vector3 lastInner = junctions [meshToWork.vertices [nextIn]] [1];
			
				if (GetDistance (newOuter, lastInner) < GetDistance (lastOuter, newInner)) {
					List<Vector3> newTriangle = new List<Vector3> ();
					newTriangle.Add (lastOuter);
					newTriangle.Add (newOuter);
					newTriangle.Add (lastInner);
					tempTriangles.Add (newTriangle);
					newTriangle = new List<Vector3> ();
					newTriangle.Add (lastInner);
					newTriangle.Add (newOuter);
					newTriangle.Add (newInner);
					tempTriangles.Add (newTriangle);
				} else {
					List<Vector3> newTriangle = new List<Vector3> ();
					newTriangle.Add (lastInner);
					newTriangle.Add (lastOuter);
					newTriangle.Add (newInner);
					tempTriangles.Add (newTriangle);
					newTriangle = new List<Vector3> ();
					newTriangle.Add (lastOuter);
					newTriangle.Add (newOuter);
					newTriangle.Add (newInner);
					tempTriangles.Add (newTriangle);
				}
			}
		}
	
		Vector3[] vertices = new Vector3[2 * junctions.Count];
	
		List<Vector3> tempVertices = new List<Vector3> ();

		//We place the points in the [] so we can reference them in the triangle construction
		for (int i = 0; i < junctions.Count; i++) {
			Vector3 node = junctions.Keys.ElementAt (i);
			List<Vector3> nodes = junctions [node];
			tempVertices.Add (nodes [0]);
			tempVertices.Add (nodes [1]);
			vertices [2 * i] = nodes [0];
			vertices [2 * i + 1] = nodes [1];
		}
	
		int pointCount = 0;
	
		int[] triangles = new int[tempTriangles.Count * 3];
	
		//Debug.Log("Generated " + tempTriangles.Count + " triangles");

		//Actual triangle generation using the points index system
		foreach (List<Vector3> triangle in tempTriangles) {
			foreach (Vector3 point in triangle) {
				triangles [pointCount] = tempVertices.FindIndex (0, newPoint => newPoint == point);
				pointCount++;
			}
		}

		outMesh.vertices = vertices;
		outMesh.triangles = triangles;
		outMesh.RecalculateNormals ();
		outMesh.RecalculateBounds ();

		GameObject outline = new GameObject ();
		outline.transform.parent = toOutline.transform;
		outline.transform.position = toOutline.transform.position;
		outline.name = toOutline.name + "-Outline";
		MeshFilter meshFilter = outline.AddComponent<MeshFilter> ();
		meshFilter.mesh = outMesh;
		MeshRenderer renderer = outline.AddComponent<MeshRenderer> ();
		renderer.materials [0].color = Color.black;
		renderer.materials [0].shader = toOutline.GetComponent<MeshRenderer> ().material.shader;
	}

	//Returns the clockwise angle magnitude between the two lines - we go clockwise for mesh building
	public static float GetCWAngle(Vector3 centre, Vector3 point1, Vector3 point2) { 
		float angle1 = Mathf.Atan2 (point1.y - centre.y, point1.x - centre.x);
		float angle2 = Mathf.Atan2 (point2.y - centre.y, point2.x - centre.x);
		
		if (angle1 == angle2) {
			return 0f;
		}
		
		if (angle1 > angle2) {
			return Mathf.Abs (angle2 - angle1);
		} else {
			return (2 * Mathf.PI - Mathf.Abs (angle1 - angle2));
		}
	}

	//Returns the distance magnitude between two points - used for shortest-diagonal quad splitting
	static float GetDistance(Vector3 point1, Vector3 point2) {
		return Mathf.Sqrt (Mathf.Pow ((point1.x - point2.x), 2) + Mathf.Pow ((point1.y - point2.y), 2));
	}

	public static string ColorToHex(Color32 color) {
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}
	
	public static Color HexToColor(string hex) {
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}
}

