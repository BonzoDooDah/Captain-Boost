using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[ExecuteAlways]
public class BoostCave : MonoBehaviour {
    [Header("Dimensions")]
    [SerializeField] private uint _length = 0;
    [SerializeField] private float _height = 0.0f;
    [Header("Ceiling")]
    [SerializeField] private AnimationCurve _curveCeiling = null; // new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 0.0f));
    [SerializeField] private Transform _ceiling = null;
    [Header("Floor")]
    [SerializeField] private AnimationCurve _curveFloor = null; // new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 0.0f));
    [SerializeField] private Transform _floor = null;
    [Header("Prefab")]
    [SerializeField] private Transform _prefab = null;
    [SerializeField] private int _seed = 0;

    private bool _isDirty = true;
    private System.Random _rng = null;

    private void Reset() {
        _length = 10;
        _height = 20.0f;
        _curveCeiling = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.1f, 5.0f), new Keyframe(0.9f, 5.0f), new Keyframe(1.0f, 0.0f));
        _ceiling = transform.Find("Ceiling");
        _curveFloor = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.1f, -5.0f), new Keyframe(0.9f, -5.0f), new Keyframe(1.0f, 0.0f));
        _floor = transform.Find("Floor");
        _prefab = transform.Find("Boost_Cave_Prefab");
        _seed = 88888;
        _isDirty = true;
        _rng = new System.Random(_seed);
    }

    private void OnValidate() {
        _isDirty = true;
    }

    private void Start() {
        CalculateCave();
    }

    private void Update() {
        if (!Application.isPlaying) {
            CalculateCave();
        }
    }

    private void DestroyAllChildren(Transform parent) {
        //Debug.Log($"DESTROY - {parent.name} : child count = {parent.childCount}");

        for (int i = parent.childCount; i > 0; i--) {
            if (Application.isPlaying) {
                Destroy(parent.GetChild(i - 1).gameObject);
            } else {
                DestroyImmediate(parent.GetChild(i - 1).gameObject);
            }
        }

        //Debug.Log($"DESTROYED - {parent.name} : child count = {parent.childCount}");
    }

    private void CreateAllChildren(Transform parent, BoostCavePrefab.GripType gripType) {
        //Debug.Log($"CREATE - {parent.name} : child count = {parent.childCount}");

        _rng = new System.Random(_seed);

        for (int i = 0; i < _length; i++) {
            GameObject gObj = Instantiate(_prefab.gameObject, parent);
            CalculateBoostCavePrefabObject(i, gObj, gripType);
        }

        //Debug.Log($"CREATED - {parent.name} : child count = {parent.childCount}");
    }

    private void CalculateCave() {
        if (_isDirty) {
            DestroyAllChildren(_ceiling);
            DestroyAllChildren(_floor);

            CreateAllChildren(_ceiling, BoostCavePrefab.GripType.Stalagtite);
            CreateAllChildren(_floor, BoostCavePrefab.GripType.Stalagmite);

            _isDirty = false;
        }
    }

    private void CalculateBoostCavePrefabObject(int i, GameObject gObj, BoostCavePrefab.GripType gripType) {
        float offset = _height / 2 * (int)gripType;
        AnimationCurve _curve = gripType == BoostCavePrefab.GripType.Stalagtite ? _curveCeiling : _curveFloor;

        gObj.transform.localPosition = (Vector3.right * i) + (Vector3.down * offset);

        BoostCavePrefab bcp = gObj.GetComponent<BoostCavePrefab>();
        if (bcp != null) {
            bcp.ColumnType = gripType;
            bcp.Height = (_curve.Evaluate((_length <= 1) ? 0.0f : (float)i / (_length - 1))) + offset;
            bcp.PrevHeight = (i == 0 ? -offset : _curve.Evaluate((_length <= 1) ? 0.0f : (float)(i - 1) / (_length - 1))) + offset;
            bcp.NextHeight = (i == _length - 1 ? -offset : _curve.Evaluate((_length <= 1) ? 0.0f : (float)(i + 1) / (_length - 1))) + offset;

            // randomise cap
            if ((i == 0) || (i == _length - 1)) {
                bcp.SetCapVariant(null);
            } else {
                bcp.SetCapVariant(_rng);
            }
        }

        gObj.SetActive(true);
    }
}
