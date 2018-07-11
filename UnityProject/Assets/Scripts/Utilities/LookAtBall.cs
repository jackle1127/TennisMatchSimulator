using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtBall : MonoBehaviour {
    [SerializeField] Transform eyeLocation;
    [SerializeField] Transform ball;
    [SerializeField] float minDistance = .15f;
    
	// Update is called once per frame
	void Update () {
        Vector3 lookAt = ball.position - eyeLocation.position;
        if (lookAt.magnitude > minDistance)
        {
            transform.rotation = Quaternion.LookRotation(lookAt, Vector3.Cross(Vector3.up, lookAt));
        }
    }
}
