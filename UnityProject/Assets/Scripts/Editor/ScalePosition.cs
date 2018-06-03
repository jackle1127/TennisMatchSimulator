using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScalePosition : EditorWindow {
    public static float scale = 1;

    [MenuItem("Tools/Scale Local Positions")]
    public static void ScaleLocalPostions() {
        ScalePosition scaler = EditorWindow.GetWindow<ScalePosition>();
        scaler.Show();
    }

    private void OnGUI() {
        scale = EditorGUILayout.FloatField("Scale", scale);
        if (GUILayout.Button("Scale local positions of selected objects")) {
            List<Transform> transformList = new List<Transform>();
            foreach (Transform transform in Selection.transforms) {
                Traverse(transform, transformList);
            }
            foreach (Transform transform in transformList) {
                transform.localPosition *= scale;
            }
        }
    }

    private void Traverse(Transform transform, List<Transform> transformList) {
        transformList.Add(transform);
        for (int i = 0; i < transform.childCount; i++) {
            Traverse(transform.GetChild(i), transformList);
        }
    }

}
