using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CavePath : MonoBehaviour {
    [SerializeField] private GameObject _prefab = null;
    [SerializeField] private float _height = 10f;
    [SerializeField] private uint _length = 0;
    [SerializeField] private CavePathColumnType _pathType = CavePathColumnType.Stalagmite;

    [SerializeField] private AnimationCurve _curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));

    public float Height {
        get { return _height; }
        set { _height = value; }
    }

    public uint Length {
        get { return _length; }
        set { _length = value; }
    }

    private void Start() {
        UpdateLength(_length);
        UpdateCurve();
    }

    private void Update() {
        if (!Application.isPlaying) {
            UpdateLength(_length);
            UpdateCurve();
        }
    }

    public void UpdateLength(uint length) {
        if (length == transform.childCount) return;

        if (length < transform.childCount) {
            for (int i = (transform.childCount - 1); i > (length - 1); i--) {
                if (Application.isPlaying) {
                    Destroy(transform.GetChild(i).gameObject);
                } else {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }

        if (length > transform.childCount) {
            for (int i = transform.childCount; i < length; i++) {
                GameObject gameObj = Instantiate(_prefab, transform);
                gameObj.transform.localPosition = Vector3.right * i;
            }
        }

        _length = length;
    }

    public void UpdateCurve() {
        float offset = _height / 2 * (int)_pathType;

        for (int i = 0; i < _length; i++) {

            transform.GetChild(i).localPosition = (Vector3.right * i) + (Vector3.down * offset);

            Vector3 scale = Vector3.one;

            CavePathColumn block = transform.GetChild(i).GetComponent<CavePathColumn>();
            if (block != null) {
                block.ColumnType = _pathType;
                block.Height = (_curve.Evaluate((_length <= 1) ? 0.0f : (float)i / (_length - 1))) + offset;
                block.PrevHeight = (i == 0 ? -offset : _curve.Evaluate((_length <= 1) ? 0.0f : (float)(i - 1) / (_length - 1))) + offset;
                block.NextHeight = (i == _length - 1 ? -offset : _curve.Evaluate((_length <= 1) ? 0.0f : (float)(i + 1) / (_length - 1))) + offset;
            }
        }

    }
}
