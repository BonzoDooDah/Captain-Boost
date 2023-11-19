using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BoostCavePrefab : MonoBehaviour {

    public enum GripType {
        Stalagtite = -1, Stalagmite = 1
    }

    [SerializeField] private float _height = 0.0f;
    [SerializeField] private float _prevHeight = 0.0f;
    [SerializeField] private float _nextHeight = 0.0f;
    [SerializeField] private GripType _columnType = GripType.Stalagmite;

    [SerializeField] private Transform _cap = null;
    [SerializeField] private Transform _front = null;
    [SerializeField] private Transform _left = null;
    [SerializeField] private Transform _right = null;
    [SerializeField] private Transform _collider = null;

    private bool _isDirty = true;

    public float Height {
        get { return _height; }
        set { _height = value; _isDirty = true; }
    }

    public float PrevHeight {
        get { return _prevHeight; }
        set { _prevHeight = value; _isDirty = true; }
    }

    public float NextHeight {
        get { return _nextHeight; }
        set { _nextHeight = value; _isDirty = true; }
    }

    public GripType ColumnType {
        get { return _columnType; }
        set { _columnType = value; _isDirty = true; }
    }

    private void Reset() {
        _height = 0.0f;
        _prevHeight = 0.0f;
        _nextHeight = 0.0f;
        _columnType = GripType.Stalagmite;
        ResetTransforms();
    }

    private void ResetTransforms() {
        _cap = transform.Find("Cap");
        _front = transform.Find("Front");
        _left = transform.Find("Left");
        _right = transform.Find("Right");
        _collider = transform.Find("Collider");
    }

    private void Start() {
        PositionTransforms();
    }

    private void Update() {
        if (!Application.isPlaying) {
            PositionTransforms();
        }
    }

    private bool CalculateActiveState(float h) {
        if (_columnType == GripType.Stalagmite) {
            return h > 0;
        } else {
            return h < 0;
        }
    }

    private void PositionTransforms() {
        if (!_isDirty) return;

        ResetTransforms();

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
            Transform t = (_columnType == GripType.Stalagmite) ?
                (_prevHeight > _nextHeight) ? _right : _left :
                (_prevHeight < _nextHeight) ? _right : _left;

            _collider.localPosition = t.localPosition;
            _collider.localScale = t.localScale;
            _collider.gameObject.SetActive(t.gameObject.activeSelf);
        }

        _isDirty = false;
    }

    public void SetCapVariant(System.Random rng) {
        if (!_cap) ResetTransforms();

        int capVariant = rng == null ? 0 : rng.Next(_cap.childCount);

        for (int i = 0; i < _cap.childCount; i++) {
            if (capVariant == i) {
                _cap.GetChild(i).gameObject.SetActive(true);
            } else {
                _cap.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
