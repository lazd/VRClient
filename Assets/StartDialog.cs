using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartDialog : MonoBehaviour {
	public FollowCamera followCamera;
	public GameObject wasp;
	public GameObject waspDrone;
	public GameObject ant;
	public GameObject mobileButtons;
	public GameObject dualTouchControls;
	public GameObject leftTouchControls;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startGame() {
		// Get settings from dropdowns
		var dialog = transform.Find("Dialog").gameObject;
		var character = transform.Find("Dialog/Character/Label").GetComponent<Text>().text;
		var controlStyle = transform.Find ("Dialog/Control Style/Label").GetComponent<Text>().text;

		var activeCharacter = ant;
		if (character == "Ant") {
			activeCharacter = ant;
		} else if (character == "Wasp") {
			activeCharacter = wasp;
		} else if (character == "Wasp Drone") {
			activeCharacter = waspDrone;
		}

		if (controlStyle == "Simple") {
			leftTouchControls.SetActive (true);
			activeCharacter.GetComponent<WallWalker>().controlStyle = WallWalker.ControlStyle.Mode1;
		}
		else {
			dualTouchControls.SetActive (true);
			WallWalker.ControlStyle enumStyle = (WallWalker.ControlStyle) System.Enum.Parse (typeof (WallWalker.ControlStyle), controlStyle);
			activeCharacter.GetComponent<WallWalker>().controlStyle = enumStyle;
		}

		// Follow the character and enable it
		followCamera.target = activeCharacter;
		activeCharacter.SetActive (true);

		// Hide canvas
		dialog.SetActive(false);
	}
}
