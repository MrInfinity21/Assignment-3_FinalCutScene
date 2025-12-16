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
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private bool _isRunning = false;
    private bool _isAttacking = false;
    private bool _isJumping = false;

    private float _verticalVelocity;
    private bool _isGrounded;

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

        // Attack input
        _inputActions.Player.Attack.performed += ctx => _isAttacking = true;
        _inputActions.Player.Attack.canceled += ctx => _isAttacking = false;

        // Jump input
        _inputActions.Player.Jump.performed += ctx =>
        {
            if (_isGrounded && !_isJumping)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
                _isJumping = true;
            }
        };
    }

    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    private void Update()
    {
        GroundCheck();
        ApplyGravity();
        MoveCharacter();
        UpdateAnimation();
    }

    private void GroundCheck()
    {
        // Calculate the bottom of the capsule
        float capsuleBottom = _characterController.bounds.min.y;
        Vector3 checkPosition = new Vector3(transform.position.x, capsuleBottom + 0.01f, transform.position.z);

        // Sphere check for ground
        _isGrounded = Physics.CheckSphere(checkPosition, groundDistance, groundMask);

        // Reset jump when grounded
        if (_isGrounded && _verticalVelocity < 0)
        {
            _isJumping = false;
            _verticalVelocity = 0f;
        }
    }

    private void ApplyGravity()
    {
        _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        _characterController.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
    }

    private void MoveCharacter()
    {
        float currentSpeed = (_isRunning && _moveInput.sqrMagnitude > 0.01f) ? runSpeed : walkSpeed;
        Vector3 moveDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _characterController.Move(moveDir * currentSpeed * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("Velocity X", _moveInput.x);
        animator.SetFloat("Velocity Z", _moveInput.y);

        animator.SetBool("isWalking", _moveInput.sqrMagnitude > 0.01f && !_isRunning);
        animator.SetBool("isRunning", _isRunning && _moveInput.sqrMagnitude > 0.01f);
        animator.SetBool("isAttacking", _isAttacking);
        animator.SetBool("isJumping", _isJumping);
    }

    private void OnDrawGizmosSelected()
    {
        if (_characterController == null) return;

        float capsuleBottom = _characterController.bounds.min.y;
        Vector3 checkPosition = new Vector3(transform.position.x, capsuleBottom + 0.01f, transform.position.z);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPosition, groundDistance);
    }
}
