// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html
using System;
using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
  // Called once per frame
  void Update () {
  	transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
  }
}
