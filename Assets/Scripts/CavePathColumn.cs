using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum CavePathColumnType {
    Stalagtite = -1, Stalagmite = 1
}

[ExecuteAlways]
public class CavePathColumn : MonoBehaviour {
    [SerializeField] private float _height = 0.0f;
    [SerializeField] private float _prevHeight = 0.0f;
    [SerializeField] private float _nextHeight = 0.0f;
    [SerializeField] private CavePathColumnType _columnType = CavePathColumnType.Stalagmite;

    [SerializeField] private Transform _cap = null;
    [SerializeField] private Transform _front = null;
    [SerializeField] private Transform _left = null;
    [SerializeField] private Transform _right = null;
    [SerializeField] private Transform _collider = null;

    public float Height {
        get { return _height; }
        set { _height = value; PositionTransforms(); }
    }

    public float PrevHeight {
        get { return _prevHeight; }
        set { _prevHeight = value; PositionTransforms(); }
    }

    public float NextHeight {
        get { return _nextHeight; }
        set { _nextHeight = value; PositionTransforms(); }
    }

    public CavePathColumnType ColumnType {
        get { return _columnType; }
        set { _columnType = value; PositionTransforms(); }
    }

    void Start() {
        RefreshTransformReferences();
        PositionTransforms();
    }

    void Update() {
        if (!Application.isPlaying) {
            RefreshTransformReferences();
            PositionTransforms();
        }
    }

    private void RefreshTransformReferences() {
        if (_cap == null) _cap = transform.Find("Cap");
        if (_front == null) _front = transform.Find("Front");
        if (_left == null) _left = transform.Find("Left");
        if (_right == null) _right = transform.Find("Right");
        if (_collider == null) _collider = transform.Find("Collider");
    }

    private bool CalculateActiveState(float h) {
        if (_columnType == CavePathColumnType.Stalagmite) {
            return h > 0;
        } else {
            return h < 0;
        }
    }

    private void PositionTransforms() {
        if (_cap != null) {
            _cap.localPosition = Vector3.up * _height;
            _cap.localScale = new Vector3(1f, (float)_columnType, 1f);
        }

        Vector3 newScale = Vector3.one;

        if (_front != null) {
            _front.localPosition = Vector3.zero;
            newScale.y = _height;
            _front.localScale = newScale;
            _front.gameObject.SetActive(CalculateActiveState(newScale.y));
        }

        if (_left != null) {
            _left.localPosition = Vector3.up * _prevHeight;
            newScale.y = _height - _prevHeight;
            _left.localScale = newScale;
            _left.gameObject.SetActive(CalculateActiveState(newScale.y));
        }

        if (_right != null) {
            _right.localPosition = Vector3.up * _nextHeight;
            newScale.y = _height - _nextHeight;
            _right.localScale = newScale;
            _right.gameObject.SetActive(CalculateActiveState(newScale.y));
        }

        if (_collider != null) {
            Transform t = (_columnType == CavePathColumnType.Stalagmite) ?
                (_prevHeight > _nextHeight) ? _right : _left :
                (_prevHeight < _nextHeight) ? _right : _left;

            _collider.localPosition = t.localPosition;
            _collider.localScale = t.localScale;
            _collider.gameObject.SetActive(t.gameObject.activeSelf);
        }
    }
}
