using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PathCap : MonoBehaviour {
    public enum CapVariantType {
        Default = 0, Type1, Type2, Type3
    }

    [SerializeField] private CapVariantType _currentVariant = CapVariantType.Default;
    [SerializeField] private int _variantCount = 0;

    public CapVariantType CurrentVariant {
        get { return _currentVariant; }
        set { _currentVariant = value; }
    }

    private void Start() {
        UpdateCapVariant();
    }

    private void Update() {
        if (!Application.isPlaying) {
            UpdateCapVariant();
        }
    }

    private void UpdateCapVariant() {
        _variantCount = transform.childCount;

        if (_variantCount != sizeof(CapVariantType)) return;

        for (int i = 0; i < _variantCount; i++) {
            if (i == (int)_currentVariant) {
                transform.GetChild(i).gameObject.SetActive(true);
            } else {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
