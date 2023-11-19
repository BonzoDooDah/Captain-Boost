using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketMovement : MonoBehaviour, CaptainBoostControls.IGameplayActions {

    [SerializeField] private Rigidbody _rigidbody = null;
    [SerializeField] private float _thrust = 1000f;
    [SerializeField] private float _rotation = 180f;

    private CaptainBoostControls controls;

    private float _thrustSpeed = 0f;
    private float _rotationDeg = 0f;

    private void OnEnable() {
        if (controls == null) {
            controls = new CaptainBoostControls();
            controls.gameplay.SetCallbacks(this);
        }
        controls.gameplay.Enable();
    }

    private void OnDisable() {
        controls.gameplay.Disable();
    }

    private void Start() {
        // freeze physics rotation
        _rigidbody.freezeRotation = true;
    }

    private void FixedUpdate() {
        // handle thrust
        _rigidbody.AddRelativeForce(Vector3.up * _thrustSpeed * Time.deltaTime);

        // handle rotation
        Quaternion deltaRotation = Quaternion.Euler(0f, 0f, _rotationDeg * Time.deltaTime);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }

    private void Reset() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnThrust(InputAction.CallbackContext context) {
        _thrustSpeed = context.ReadValueAsButton() ? _thrust : 0f;
    }

    public void OnRotate(InputAction.CallbackContext context) {
        _rotationDeg = -_rotation * context.ReadValue<float>();
    }
}
