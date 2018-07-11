using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtBall : MonoBehaviour {
    [SerializeField] Transform eyeLocation;
    [SerializeField] Transform ball;
    [SerializeField] float minDistance = .15f;
    [SerializeField] float angleLimit = 72;
    
	// Update is called once per frame
	void LateUpdate () {
        Vector3 lookAt = ball.position - eyeLocation.position;
        if (lookAt.magnitude > minDistance)
        {
            float angle = Vector3.Angle(lookAt, transform.parent.forward);
            if (angle > angleLimit)
            {
                //Debug.Log(angle + ", " + angleLimit + " ," + (angle / angleLimit));
                lookAt = Vector3.Slerp(transform.parent.forward, lookAt, angleLimit / angle);
                //lookAt = transform.parent.forward;
            }
            transform.rotation = Quaternion.LookRotation(lookAt, Vector3.Cross(Vector3.up, lookAt));
        }
    }
}
