using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RevertAll : Editor {
    [MenuItem("Tools/Revert selected to Prefab %#&r")]
    static void Revert() {
        GameObject[] selections = Selection.gameObjects;

        if (selections.Length > 0) {
            foreach (GameObject obj in selections) {
                Debug.Log(obj + "?");
                Debug.Log(PrefabUtility.RevertPrefabInstance(obj) ? "woo" : "ahhhhh");
            }
        } else {
            Debug.Log("Cannot revert to prefab - nothing selected");
        }
    }
}
