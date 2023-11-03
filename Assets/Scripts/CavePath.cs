using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CavePath ===================================================================
//
// Unity MonoBehaviour script for the Cave_Path prefab.

[ExecuteAlways]
public class CavePath : MonoBehaviour {

    // private variables ------------------------------------------------------

    [Header("Path Properties")]
    [SerializeField][Min(1)] private int _length = 1;
    [SerializeField][Min(0)] private float _height = 1f;
    [SerializeField][Min(0)] private float _frontExtension = 0f;
    [SerializeField] private AnimationCurve _pathCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));

    private Transform _pathPrefab = null;
    private LinkedList<CavePathColumn> _columns = new LinkedList<CavePathColumn>();

    // public properties ------------------------------------------------------

    public int Length {
        get { return _length; }
        set { _length = value; UpdateChildCount(); }
    }

    public float Height {
        get { return _height; }
    }

    // Unity methods ----------------------------------------------------------

    private void Start() {
        ResetPrefabReference();
        UpdateChildCount();
    }

    private void Update() {
        if (!Application.isPlaying) { // do stuff in edit mode...
            ResetPrefabReference();
            UpdateChildCount();
        }
    }

    // private methods --------------------------------------------------------

    private void ResetPrefabReference() {
        if (_pathPrefab == null) _pathPrefab = transform.Find("Prefab");
        _pathPrefab.SetAsFirstSibling();
        _pathPrefab.gameObject.SetActive(false);
    }

    private void UpdateChildCount() {
        int adjustment = (_length + 1) - transform.childCount;

        // if child objects need to be added
        if (adjustment > 0) {
            for (int i = 0; i < adjustment; i++) {
                Transform newChild = Instantiate(_pathPrefab, transform);
                newChild.gameObject.SetActive(true);
            }
        }

        // if child objects need to be removed
        if (adjustment < 0) {
            for (int i = 0; i < MathF.Abs(adjustment); i++) {
                if (Application.isPlaying) {
                    Destroy(transform.GetChild(1).gameObject);
                } else {
                    DestroyImmediate(transform.GetChild(1).gameObject);
                }
            }
        }

        // repopulate column list
        UpdateColumns();
    }

    private void UpdateColumns() {
        float minCurveValue = GetMinCurveValue();

        _columns.Clear();
        for (int i = 1; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);

            float curveTime = (_length == 1) ? 0.5f : (float)(i - 1) / (_length - 1);
            float curveValue = _pathCurve.Evaluate(curveTime);

            Vector3 colPosition = Vector3.zero;
            colPosition.x = i - 1;
            colPosition.y = minCurveValue * _height;
            // set transform's position
            child.localPosition = colPosition;

            // add this child's CavePathColumn object to the column list
            LinkedListNode<CavePathColumn> node = _columns.AddLast(child.GetComponent<CavePathColumn>());
            node.Value.Height = (curveValue + MathF.Abs(minCurveValue)) * _height;
        }

        UpdateNeighbourColumnHeights();
    }

    private float GetMinCurveValue() {
        float minCurveValue = 0f;

        for (int i = 0; i < _length; i++) {
            float curveTime = (_length == 1) ? 0f : (float)i / (_length - 1);
            float curveValue = _pathCurve.Evaluate(curveTime);

            if (curveValue < minCurveValue) { minCurveValue = curveValue; }
        }

        return minCurveValue;
    }

    private void UpdateNeighbourColumnHeights() {
        LinkedListNode<CavePathColumn> node = _columns.First;
        while (node != null) {
            node.Value.PrevHeight = (node.Previous != null) ? node.Previous.Value.Height : 0f;
            node.Value.NextHeight = (node.Next != null) ? node.Next.Value.Height : 0f;
            node.Value.FrontExtension = _frontExtension;
            node = node.Next;
        }
    }
}
