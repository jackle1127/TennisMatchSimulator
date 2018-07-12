using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Hand))]
public class MenuToggler : MonoBehaviour {
    [SerializeField] private ControlUIController controlUIController;
    
    // Update is called once per frame
    void Update () {
		if (Input.GetButtonDown("Submit"))
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
