using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(CavePathColumn))]
public class CavePathColumn_Inspector : Editor {
    public VisualTreeAsset _visualTree;

    public override VisualElement CreateInspectorGUI() {
        VisualElement root = new VisualElement();

        _visualTree.CloneTree(root);

        return root;
        //return base.CreateInspectorGUI();
    }
}
