using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectMeshesOnly : Editor {
    [MenuItem("Tools/Select Meshes Only")]
    static void SelectMeshes() {
        List<GameObject> selections = new List<GameObject>();
        Traverse(Selection.activeTransform, selections);
        Selection.objects = selections.ToArray();
    }

    private static void Traverse(Transform transform, List<GameObject> selections) {
        if (transform.gameObject.GetComponent<MeshRenderer>() || transform.gameObject.GetComponent<LODGroup>()) {
            selections.Add(transform.gameObject);
            return;
        } else {
            for (int i = 0; i < transform.childCount; i++) {
                Traverse(transform.GetChild(i), selections);
            }
        }
    }

    [MenuItem("Tools/Select Empty Only")]
    static void Bob() {
        List<GameObject> selections = new List<GameObject>();
        TraverseEmpty(Selection.activeTransform, selections);
        Selection.objects = selections.ToArray();
    }

    private static void TraverseEmpty(Transform transform, List<GameObject> selections) {
        Debug.Log(transform.gameObject + ": " + transform.gameObject.GetComponents<Component>().Length);
        if (transform.gameObject.GetComponents<Component>().Length == 1) {
            selections.Add(transform.gameObject);
        }
        for (int i = 0; i < transform.childCount; i++) {
            TraverseEmpty(transform.GetChild(i), selections);
        }
    }
}
