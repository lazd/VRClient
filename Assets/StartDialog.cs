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

  private GameObject dialog;

	// Use this for initialization
	void Start () {
    dialog = transform.Find("Dialog").gameObject;
		
    // Show canvas
    dialog.SetActive(true);
	}
	
	public void startGame() {
		// Get settings from dropdowns
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
			mobileButtons.SetActive (true);
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

    activeCharacter.transform.position = new Vector3(84.81f, 0.05f, -74.2f);
    activeCharacter.transform.rotation = Quaternion.Euler(0, -36f, 0);

		// Hide canvas
		dialog.SetActive(false);
	}
}
