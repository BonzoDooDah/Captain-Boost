using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CavePathColumn =============================================================
//
// Unity MonoBehaviour script for the Cave_Path_Column prefab.

[ExecuteAlways]
public class CavePathColumn : MonoBehaviour {

    // private variables ------------------------------------------------------

    [Header("Column Properties")]
    [SerializeField][Min(0)] private float _height = 1f;
    [SerializeField][Min(0)] private float _prevHeight = 0f;
    [SerializeField][Min(0)] private float _nextHeight = 0f;
    [SerializeField][Min(0)] private float _frontExtension = 0f;

    private Transform _cap = null;
    private Transform _base = null;

    // public properties ------------------------------------------------------

    public float Height {
        get { return _height; }
        set { _height = (value < 0f) ? 0f : value; UpdateLocalPositions(); }
    }

    public float PrevHeight {
        get { return _prevHeight; }
        set { _prevHeight = (value < 0f) ? 0f : value; UpdateLocalPositions(); }
    }

    public float NextHeight {
        get { return _nextHeight; }
        set { _nextHeight = (value < 0f) ? 0f : value; UpdateLocalPositions(); }
    }

    public float FrontExtension {
        get { return _frontExtension; }
        set { _frontExtension = (value < 0f) ? 0f : value; UpdateLocalPositions(); }
    }

    // Unity methods ----------------------------------------------------------

    private void Start() {
        UpdateChildReferences();
    }

    private void Update() {
        if (!Application.isPlaying) { // do stuff in edit mode...
            UpdateChildReferences();
            UpdateLocalPositions();
        }
    }

    // private methods --------------------------------------------------------

    private void UpdateChildReferences() {
        _cap = transform.Find("Cap");
        _base = transform.Find("Base");
    }


    private void UpdateLocalPositions() {
        // position the column cap
        if (_cap != null) _cap.localPosition = Vector3.up * _height;

        // position the column base
        if (_base != null) _base.localPosition = Vector3.up * _height;

        UpdateBaseChildren();
    }

    private void UpdateBaseChildren() {
        if (_base == null) return;

        Vector3 baseScale = Vector3.one;

        float prevHeightDiff = _height - _prevHeight;
        float nextHeightDiff = _height - _nextHeight;
        float colliderHeight = Mathf.Max(Mathf.Max(prevHeightDiff, nextHeightDiff), 0f);

        for (int i = 0; i < _base.childCount; i++) {
            Transform child = _base.GetChild(i);

            switch (child.name) {
                case "Front":
                    baseScale.y = _height + _frontExtension; break;
                case "Left":
                    baseScale.y = (prevHeightDiff < 0f) ? 0f : prevHeightDiff; break;
                case "Right":
                    baseScale.y = (nextHeightDiff < 0f) ? 0f : nextHeightDiff; break;
                case "Collider":
                    baseScale.y = colliderHeight; break;
                default:
                    baseScale.y = _height; break;
            }

            child.localScale = baseScale;
        }
    }
}
