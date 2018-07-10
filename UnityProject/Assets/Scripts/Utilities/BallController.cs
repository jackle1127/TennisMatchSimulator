using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [SerializeField] private Transform ballTransform;
    public Vector3 spinningVector = Vector3.forward;
    public float spinningSpeed = 2600;

    private bool free = false;
    private Rigidbody rb;
    private Vector3 prevLocation = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        prevLocation = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!free)
        {
            ballTransform.rotation = Quaternion.AngleAxis(spinningSpeed * Time.deltaTime, spinningVector) * ballTransform.rotation;
        }
        velocity = (transform.position - prevLocation) / Time.deltaTime;
        prevLocation = transform.position;
    }

    public void SetControlled(bool controlled)
    {
        free = !controlled;
        rb.isKinematic = controlled;

        if (!controlled)
        {
            rb.velocity = velocity;
        }
    }
}
