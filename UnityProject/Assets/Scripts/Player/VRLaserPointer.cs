using UnityEngine;
using System.Collections;

public class VRLaserPointer : MonoBehaviour
{
    public bool active = true;
    public Color color;
    public float thickness = 0.002f;
    public float tipSize = .03f;
    bool isActive = false;
    public Transform reference;
    private GameObject holder;
    private GameObject pointer;
    private GameObject tipSphere;

    // Use this for initialization
    void Start ()
    {
        if (!reference) reference = transform;

        holder = new GameObject("Laser Holder");
        holder.transform.parent = reference.transform;
        holder.transform.localPosition = Vector3.zero;
		holder.transform.localRotation = Quaternion.identity;
        Vector3 lossyScale = reference.transform.lossyScale;
        lossyScale.x = 1 / lossyScale.x;
        lossyScale.y = 1 / lossyScale.y;
        lossyScale.z = 1 / lossyScale.z;
        holder.transform.localScale = lossyScale;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pointer.name = "Laser Pointer";
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, 1, thickness);
        pointer.transform.localPosition = new Vector3(0f, 1f, 0f);
		pointer.transform.localRotation = Quaternion.identity;
        Destroy(pointer.GetComponent<Collider>());

        tipSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tipSphere.name = "Laser Tip";
        tipSphere.transform.localScale = Vector3.one * tipSize;
        Destroy(tipSphere.GetComponent<Collider>());

        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;
        tipSphere.GetComponent<MeshRenderer>().material = newMaterial;

    }

    private void Update()
    {
        if (!isActive && active)
        {
            holder.SetActive(true);
            tipSphere.SetActive(true);
        }
        else
        if (isActive && !active)
        {
            holder.SetActive(false);
            tipSphere.SetActive(false);
        }
        isActive = active;
    }

    public void SetDistance(float dist)
    {
        holder.transform.localScale = new Vector3(1, dist / 2, 1);
        tipSphere.transform.position = holder.transform.position + holder.transform.up * dist;
    }
}
