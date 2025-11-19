using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private CharacterController _characterController;

    private Vector2 _moveInput;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f; // Speed for Running

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private bool _isRunning = false;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputActions = new InputSystem_Actions();

        // Movement input
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        // Sprint input
        _inputActions.Player.Sprint.performed += ctx => _isRunning = true;
        _inputActions.Player.Sprint.canceled += ctx => _isRunning = false;
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
        float currentSpeed = (_isRunning && _moveInput.sqrMagnitude > 0.01f) ? runSpeed : walkSpeed;
        Vector3 moveDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _characterController.Move(moveDir * currentSpeed * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        // Update walking Blend Tree velocities
        animator.SetFloat("Velocity X", _moveInput.x);
        animator.SetFloat("Velocity Z", _moveInput.y);

        // Update booleans
        animator.SetBool("isWalking", _moveInput.sqrMagnitude > 0.01f && !_isRunning);
        animator.SetBool("isRunning", _isRunning && _moveInput.sqrMagnitude > 0.01f);
    }
}
