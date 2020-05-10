using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Branch))]
public class BranchEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate branch from endpoint"))
        {
            var branch = (Branch)target;
            branch.Plant.GenerateBranch(branch, branch.Endpoint, branch.BranchDepth + 1);
        }

        if (GUILayout.Button("Generate branch from random joint"))
        {
            var branch = (Branch)target;
            var joint = branch.Joints[Random.Range(0, branch.Joints.Count)];
            branch.Plant.GenerateBranch(branch, joint.Transform, branch.BranchDepth + 1, joint.IsReversed);
        }

    }
}
