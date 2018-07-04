using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Hand))]
public class VRUIMenuToggler : MonoBehaviour {
    [SerializeField] private VRControlUIController controlUIController;
    private Hand steamVRHand;
    
	// Use this for initialization
	void Start () {
        steamVRHand = GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update () {
		if (steamVRHand.controller.GetPressDown(EVRButtonId.k_EButton_A))
        {
            if (!controlUIController.isVisible)
            {
                controlUIController.Show();
            }
            else
            {
                controlUIController.Hide();
            }
        }
	}
}
