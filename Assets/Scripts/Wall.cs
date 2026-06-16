using UnityEngine;

public class Wall : MonoBehaviour
{
    private static float _currentSpeed = 10f;

    private Rigidbody _rb;

    private float _speed;
    private float _speedIncreasePerWall = 1f;
    private float _destroyAtZ = -2.75f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
        }

        _rb.isKinematic = true;
        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    void Start()
    {
        _speed = _currentSpeed;
        _currentSpeed += _speedIncreasePerWall;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver) return;

        Vector3 nextPosition = _rb.position + transform.forward * _speed * Time.fixedDeltaTime;
        _rb.MovePosition(nextPosition);

        if (nextPosition.z <= _destroyAtZ)
        {
            ScoreManager.Instance.AddWallSurvived();
            Debug.Log("Current wall speed: " + _currentSpeed);
            Destroy(gameObject);
        }
    }

    public static void ResetSpeed()
    {
        _currentSpeed = 10f;
    }
}
