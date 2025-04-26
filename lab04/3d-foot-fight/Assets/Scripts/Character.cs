using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float lookSpeed = 10;
    private Camera _camera;

    private Vector2 _lookDirection = Vector2.zero;
    private Vector2 _moveDirection = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var moveMultiplier = Time.fixedDeltaTime * moveSpeed;

        transform.Translate(_moveDirection.x * moveMultiplier, 0,
            _moveDirection.y * moveMultiplier);

        var lookMultiplier = Time.fixedDeltaTime * lookSpeed;

        transform.Rotate(0, _lookDirection.x * lookMultiplier, 0);

        _camera.transform.Rotate(-_lookDirection.y * lookMultiplier, 0, 0);
    }

    private void OnLook(InputValue value)
    {
        _lookDirection = value.Get<Vector2>();
    }

    public void OnMove(InputValue value)
    {
        _moveDirection = value.Get<Vector2>();
    }
}