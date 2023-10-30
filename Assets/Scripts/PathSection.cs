using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathSection : MonoBehaviour {
    [SerializeField] private GameObject _pathPrefab = null;
    [SerializeField] private AnimationCurve _pathCurve = new AnimationCurve(new Keyframe(0f, 0.5f), new Keyframe(1f, 0.5f));
    [Min(1)]
    [SerializeField] private int _pathCount = 1;
    [SerializeField] private float _blockWidth = 2f;
    [SerializeField] private float _height = 1f;
    [SerializeField] private float _heightOffset = 0f;

    private void AdjustChildCount() {
        int adjustment = _pathCount - transform.childCount;

        // if child objects need to be added
        if (adjustment > 0) {
            for (int i = 0; i < adjustment; i++) { Instantiate(_pathPrefab, transform); }
        }

        // if child objects need to be removed
        if (adjustment < 0) {
            for (int i = 0; i < MathF.Abs(adjustment); i++) {
                if (Application.isPlaying) {
                    Destroy(transform.GetChild(0).gameObject);
                } else {
                    DestroyImmediate(transform.GetChild(0).gameObject);
                }
            }
        }
    }

    private void CalculatePath() {
        for (int i = 0; i < transform.childCount; i++) {
            // set child object horizontal position
            transform.GetChild(i).position = transform.position + (_blockWidth * i * Vector3.right);

            float t = (transform.childCount == 1) ? 0f : (float)i / (transform.childCount - 1);

            float blockHeight = _pathCurve.Evaluate(t) * _height;

            float leftBlockHeight = (i == 0) ? 0f : transform.GetChild(i - 1).GetComponent<PathBlock>().Height;
            float leftOffset = (leftBlockHeight < blockHeight) ? leftBlockHeight : blockHeight;

            float rightBlockHeight = (i == (transform.childCount - 1)) ? 0f : transform.GetChild(i + 1).GetComponent<PathBlock>().Height;
            float rightOffset = (rightBlockHeight < blockHeight) ? rightBlockHeight : blockHeight;

            PathBlock pb = transform.GetChild(i).GetComponent<PathBlock>();
            pb.SetHeights(blockHeight, leftOffset, rightOffset, _heightOffset);
        }
    }

    private void Awake() {
        //if (Application.isPlaying) { Debug.Log("Play mode - Awake"); } else { Debug.Log("Edit mode - Awake"); }
    }

    private void Start() {
        //if (Application.isPlaying) { Debug.Log("Play mode - Start"); } else { Debug.Log("Edit mode - Start"); }

        AdjustChildCount();
        CalculatePath();
    }

    private void Update() {
        //if (Application.isPlaying) { Debug.Log("Play mode - Update"); } else { Debug.Log("Edit mode - Update"); }
        if (!Application.isPlaying) {
            AdjustChildCount();
            CalculatePath();
        }
    }
}
