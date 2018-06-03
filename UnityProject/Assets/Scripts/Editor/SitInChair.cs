using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SitInChair : Editor {
    [MenuItem("Tools/Sit in chair %#&C")]
    static void SitIn() {
        if (Selection.activeGameObject) {
            SnappingTeleportingPoint tpPoint = Selection.activeGameObject.GetComponent<SnappingTeleportingPoint>();
            if (tpPoint && tpPoint.snappingPoint) {
                Vector3 position = tpPoint.snappingPoint.position + Vector3.up * 1.3f;
                Vector3 pivot = position + tpPoint.snappingPoint.forward * SceneView.lastActiveSceneView.cameraDistance;
                SceneView.lastActiveSceneView.rotation = tpPoint.snappingPoint.transform.rotation;
                SceneView.lastActiveSceneView.pivot = pivot;
            }
        }
    }
}
