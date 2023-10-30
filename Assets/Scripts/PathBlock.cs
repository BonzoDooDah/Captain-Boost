using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathBlock : MonoBehaviour {
    [Header("Mesh Objects")]
    [SerializeField] private Transform _pathTop = null;
    [SerializeField] private Transform _wallFront = null;
    [SerializeField] private Transform _wallLeft = null;
    [SerializeField] private Transform _wallRight = null;

    [Header("Positional Data")]
    [SerializeField] private float _height = 0f;
    [SerializeField] private float _frontOffset = 0f;
    [SerializeField] private float _leftOffset = 0f;
    [SerializeField] private float _rightOffset = 0f;

    public float Height { get { return _height; } }

    private void CalculateBlock() {
        Vector3 heightOffset = Vector3.up * _height;
        Vector3 wallScale = new Vector3(1f, _height, 1f);

        if (_pathTop != null) {
            _pathTop.position = transform.position + heightOffset;
        }

        if (_wallFront != null) {
            _wallFront.position = transform.position + heightOffset;
            _wallFront.localScale = wallScale + (Vector3.up * _frontOffset);
        }

        if (_wallLeft != null) {
            _wallLeft.position = transform.position + heightOffset;
            _wallLeft.localScale = wallScale - (Vector3.up * _leftOffset);
        }

        if (_wallRight != null) {
            _wallRight.position = transform.position + heightOffset;
            _wallRight.localScale = wallScale - (Vector3.up * _rightOffset);
        }
    }

    private void Awake() {
        //if (Application.isPlaying) { Debug.Log("Play mode - Awake"); } else { Debug.Log("Edit mode - Awake"); }
    }

    private void Start() {
        // calculate object positions.
        CalculateBlock();
    }

    private void Update() {
        // if in edit mode, calculate object positions.
        if (!Application.isPlaying) CalculateBlock();
    }

    public void SetHeights(float height, float leftOffset = 0f, float rightOffset = 0f, float frontOffset = 0f) {
        _height = height;
        _leftOffset = leftOffset;
        _rightOffset = rightOffset;
        _frontOffset = frontOffset;
        CalculateBlock();
    }
}
