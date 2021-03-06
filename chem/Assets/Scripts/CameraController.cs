﻿using UnityEngine;
using System.Collections;
using Leap;

public class CameraController : MonoBehaviour {

	private GameObject cam;
	private GameObject target;
	private float camDistance;
	private Frame currentFrame { get; set; }
	private Frame previousFrame { get; set; }
	private float Xmin, Xmax, Ymin, Ymax, Zmin, Zmax;
	private Controller c;

	public CameraController (float[] moleculeDimension) {
		c = new Controller ();
		cam = GameObject.Find ("/Core/Main Camera");
		Xmin = moleculeDimension [0];
		Xmax = moleculeDimension [1];
		Ymin = moleculeDimension [2];
		Ymax = moleculeDimension [3];
		Zmin = moleculeDimension [4];
		Zmax = moleculeDimension [5];

		Vector3 centerCoords = new Vector3( Util.average(Xmin,Xmax), Util.average(Ymin,Ymax), Util.average(Zmin,Zmax) );
		target = new GameObject ("Target");
		target.transform.position = centerCoords;
		cam.transform.parent = target.transform;
	}


	public void adjustCameraPosition() {
		Vector3 newPosition = new Vector3(0, 0, -10);
		cam.transform.position = newPosition;
	}

	// track frame should
	public void trackFrame() {
		currentFrame = c.Frame ();
		previousFrame = c.Frame (1);
	}

	// Orbits (rotates) camera depending on hand movement
	public void cameraOrbit() {
		HandList hands = currentFrame.Hands;
		Rigidbody camRigidbody = cam.GetComponent<Rigidbody>();
		Vector3 firstHandDelta = findHandDelta (hands [0]);

//		if (true) {
		if (!hands.IsEmpty && firstHandDelta.magnitude>1) {
			cam.transform.RotateAround (target.transform.position, new Vector3(0,firstHandDelta.x,0), firstHandDelta.magnitude);
//			cam.transform.RotateAround (target.transform.position, new Vector3(firstHandDelta.y,0,0), firstHandDelta.magnitude);
		}
		else if (hands.IsEmpty) {
			cam.transform.RotateAround (target.transform.position, new Vector3(0,1,0) , 1);
		}
		cam.transform.LookAt (target.transform);
	}

	Vector3 findHandPosition (Hand h) {
		Vector leapVec = h.PalmPosition;
		return new Vector3 (leapVec.x, leapVec.y, leapVec.z);
	}

	Vector3 findHandDelta (Hand h) {
		Vector leapVec = h.Translation (previousFrame);
		return new Vector3 (leapVec.x, leapVec.y, leapVec.z);
	}

	void debugPrintDimensions() {
		Debug.Log("Debug Print Dimensions X: "+Xmin+" "+Xmax+" Y: "+Ymin+" "+Ymax+" Z: "+Zmin+" "+Zmax);
	}


}
