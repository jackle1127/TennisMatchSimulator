using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaCaster : MonoBehaviour
{
    public float stepSize = 1;
    public float strength = 15;
    public float acceptableUpAngle = 20;
    public float backOffWallDistance = .1f;
    public int maxSteps = 100;
    public GameObject visualizerAssembly;
    public Transform visualizerParentBone;
    public Transform destinationArrow;
    public TeleportAudioController teleportAudio;

    private List<Transform> bones = new List<Transform>();
    private Transform castingObjectTransform;
    private SnappingTeleportingPoint prevTeleportPoint = null;

    private void Start()
    {
        Transform current = visualizerParentBone;
        while (current.childCount > 0)
        {
            bones.Add(current);
            current = current.GetChild(0);
        }
    }

    public Matrix4x4 Cast(Transform sourceTransform, Vector2 inputDirection)
    {
        if (this.castingObjectTransform == null) this.castingObjectTransform = sourceTransform;
        else if (this.castingObjectTransform != sourceTransform) return Matrix4x4.zero;
        visualizerAssembly.SetActive(true);
        transform.position = sourceTransform.position;
        Parabola parabola = Parabola.Cast(sourceTransform.position, sourceTransform.forward * strength,
            stepSize, maxSteps);
        for (int i = 0; i < bones.Count; i++)
        {
            Utility.ApplyMatrixToTransform(bones[i], parabola.GetNormalizedPathMatrix((float)i / (bones.Count - 1)));
            //bones[i].rotation = Quaternion.AngleAxis(90, bones[i].right) * Quaternion.AngleAxis(90, bones[i].up) * bones[i].rotation;
            bones[i].rotation = Quaternion.AngleAxis(90, bones[i].up) * Quaternion.AngleAxis(-90, bones[i].right) * bones[i].rotation;
        }

        if (parabola.hasDestination)
        {
            SnappingTeleportingPoint teleportPoint = parabola.hitObject.GetComponent<SnappingTeleportingPoint>();
            if (teleportPoint)
            {
                if (prevTeleportPoint != teleportPoint)
                {
                    UnhighlightTeleportPoint();
                    prevTeleportPoint = teleportPoint;
                    teleportPoint.Highlight();
                }
                destinationArrow.gameObject.SetActive(false);
                return teleportPoint.snappingPoint.localToWorldMatrix;
            }
            else
            {
                UnhighlightTeleportPoint();

                if (Vector3.Angle(parabola.normal, Vector3.up) <= acceptableUpAngle)
                {
                    destinationArrow.gameObject.SetActive(true);
                    destinationArrow.position = parabola.destination;
                }
                else
                {
                    // Cast a ray straight down to find ground.
                    RaycastHit hit;
                    if (Physics.Raycast(parabola.destination + Vector3.up * .001f, -Vector3.up, out hit))
                    {
                        if (Vector3.Angle(hit.normal, Vector3.up) <= acceptableUpAngle)
                        {
                            destinationArrow.gameObject.SetActive(true);
                            Vector3 backOffWallVector = parabola.normal;
                            backOffWallVector.y = 0;
                            backOffWallVector = backOffWallVector.normalized * backOffWallDistance;
                            destinationArrow.position = hit.point + backOffWallVector;
                        }
                    }
                }

                if (destinationArrow.gameObject.activeSelf)
                {
                    Vector3 right = Vector3.Cross(Vector3.up, sourceTransform.forward).normalized;
                    Vector3 forward = Vector3.Cross(right, Vector3.up);
                    Vector3 direction = right * inputDirection.x + forward * inputDirection.y;
                    destinationArrow.rotation = Quaternion.LookRotation(direction);
                    return destinationArrow.localToWorldMatrix;
                }
            }
        }
        destinationArrow.gameObject.SetActive(false);
        return Matrix4x4.zero;
    }

    public void Hide(Transform castingObjectTransform)
    {
        // Object that is not casting the parabola has no effect.
        if (this.castingObjectTransform != castingObjectTransform) return;
        visualizerAssembly.SetActive(false);
        destinationArrow.gameObject.SetActive(false);
        this.castingObjectTransform = null;
        UnhighlightTeleportPoint();
    }

    public bool beingUsed
    {
        get { return visualizerAssembly.activeSelf; }
    }

    public bool IsCastingFrom(Transform t)
    {
        return t == castingObjectTransform;
    }

    private void UnhighlightTeleportPoint()
    {
        if (prevTeleportPoint)
        {
            prevTeleportPoint.Unhighlight();
            prevTeleportPoint = null;
        }
    }
}
