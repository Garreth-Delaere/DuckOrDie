using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour
{
    [Header("Flap")]
    [SerializeField] private float _flapForce = 6f;

    [Header("Horizontal Bounds")]
    [SerializeField] private float _horizontalSpeed = 4f;
    [SerializeField] private float _horizontalLimit = 4f;

    [Header("Vertical Bounds")]
    [SerializeField] private float _verticalMin = -3f;
    [SerializeField] private float _verticalMax = 4f;

    [Header("Tilt")]
    [SerializeField] private float _maxTiltAngle = 25f;
    [SerializeField] private float _tiltSpeed = 8f;
    
    [Header("Death")]
    [SerializeField] private float _deathZThreshold = -2.5f;

    [Header("Grounded")]
    [SerializeField] private float _groundRecoverySpeed = 5f;
    [SerializeField] private float _groundedY = 0f;
    [SerializeField] private float _groundedMoveTowardsZ = 2f;

    [SerializeField] private Transform _visual;

    private Rigidbody _rb;
    private BirdInputActions _inputActions;
    private float _horizontalInput;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputActions = new BirdInputActions();
    }

    void OnEnable()
    {
        _inputActions.Bird.Enable();
        _inputActions.Bird.Flap.performed += OnFlap;
        _inputActions.Bird.Move.performed += OnMove;
        _inputActions.Bird.Move.canceled += OnMoveCanceled;
    }

    void OnDisable()
    {
        _inputActions.Bird.Flap.performed -= OnFlap;
        _inputActions.Bird.Move.performed -= OnMove;
        _inputActions.Bird.Move.canceled -= OnMoveCanceled;
        _inputActions.Bird.Disable();
    }

    private void OnFlap(InputAction.CallbackContext ctx)
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, 0f);
        _rb.AddForce(Vector3.up * _flapForce, ForceMode.Impulse);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _horizontalInput = ctx.ReadValue<float>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        _horizontalInput = 0f;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver) return;

        float newX = _rb.position.x + _horizontalInput * _horizontalSpeed * Time.fixedDeltaTime;
        newX = Mathf.Clamp(newX, -_horizontalLimit, _horizontalLimit);
        float newY = Mathf.Clamp(_rb.position.y, _verticalMin, _verticalMax);

        float newZ = _rb.position.z;

        bool isGrounded = _rb.position.y <= _groundedY;

        if (isGrounded)
        {
            newZ = Mathf.MoveTowards(
                _rb.position.z,
                _groundedMoveTowardsZ,
                _groundRecoverySpeed * Time.fixedDeltaTime
            );
        }

        _rb.MovePosition(new Vector3(newX, newY, newZ));
    }

    void ApplyTilt()
    {
        // Bank left/right based on horizontal input
        float targetRoll = -_horizontalInput * _maxTiltAngle;
        float smoothRoll = Mathf.LerpAngle(
            _visual.localEulerAngles.z,
            targetRoll,
            _tiltSpeed * Time.deltaTime
        );

        // Pitch up/down based on vertical velocity
        float targetPitch = Mathf.Lerp(90f, -90f,
            Mathf.InverseLerp(-10f, 10f, _rb.linearVelocity.y));
        float smoothPitch = Mathf.LerpAngle(
            _visual.localEulerAngles.x,
            targetPitch,
            _tiltSpeed * Time.deltaTime
        );

        _visual.localRotation = Quaternion.Euler(smoothPitch, 0f, smoothRoll);
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver)
        {
            _horizontalInput = 0f;
            return;
        }

        if (transform.position.z < _deathZThreshold)
        {
            GameManager.Instance.TriggerGameOver();
            return;
        }

        ApplyTilt();
    }
}