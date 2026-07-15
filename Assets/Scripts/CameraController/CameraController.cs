using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private PlayerController _playerController;
    private Vector2 _moveInput;

    private void Awake()
    {
        _playerController = new PlayerController();
    }

    private void Update()
    {
        Movement();
    }

    private void OnEnable()
    {
        _playerController.Camera.Move.performed += OnMove;
        _playerController.Camera.Move.canceled += OnMove;
        _playerController.Enable();
    }

    private void OnDisable()
    {
        _playerController.Camera.Move.performed -= OnMove;
        _playerController.Camera.Move.canceled += OnMove;
        _playerController.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Movement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 direction = (forward * _moveInput.y + right * _moveInput.x).normalized;
        transform.position += direction * (_moveSpeed * Time.deltaTime);
    }
}
