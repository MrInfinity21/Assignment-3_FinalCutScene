using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private CharacterController _characterControl;

    private Vector2 _moveInput;

    [Header("Movement Values")]
    [SerializeField] private float walkSpeed = 3f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        _characterControl = GetComponent<CharacterController>();
        _inputActions = new InputSystem_Actions();

        // Listen for movement input
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
    }

    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    private void Update()
    {
        MoveCharacter();
        UpdateAnimation();
    }

    private void MoveCharacter()
    {
        Vector3 moveDirection =
            transform.right * _moveInput.x +
            transform.forward * _moveInput.y;

        _characterControl.Move(moveDirection * walkSpeed * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        // Update directional movement
        animator.SetFloat("Horizontal", _moveInput.x);
        animator.SetFloat("Vertical", _moveInput.y);

        // Calculate speed magnitude (0 = idle, 1 = full movement)
        float speed = _moveInput.magnitude;

        // Update speed for transitions
        animator.SetFloat("Speed", speed);
    }
}
