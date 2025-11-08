using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    private CharacterController _characterControl;

    private Vector2 _moveInput;

    [Header("Movement Values")]
    [SerializeField] private float _walkSpeed = 3f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Idle Threshold")]
    [SerializeField, Tooltip("Minimum velocity to trigger movement in Blend Tree")]
    private float movementThreshold = 0.01f;

    [Header("Debug")]
    [SerializeField] private bool debugParameters = false;

    private void Awake()
    {
        _characterControl = GetComponent<CharacterController>();
        _inputActions = new InputSystem_Actions();

        // Movement input
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
    }

    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    private void Update()
    {
        MoveCharacter();
        UpdateAnimatorParameters();
    }

    private void MoveCharacter()
    {
        // Movement relative to player
        Vector3 moveDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        Vector3 velocity = moveDirection * _walkSpeed;

        // Move character
        _characterControl.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimatorParameters()
    {
        // Convert velocity to local space for Blend Tree
        Vector3 localVelocity = transform.InverseTransformDirection(
            new Vector3(_moveInput.x * _walkSpeed, 0, _moveInput.y * _walkSpeed)
        );

        // Apply threshold to prevent Idle jitter
        float horizontal = Mathf.Abs(localVelocity.x) < movementThreshold ? 0f : localVelocity.x;
        float vertical = Mathf.Abs(localVelocity.z) < movementThreshold ? 0f : localVelocity.z;

        // Update Animator
        animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);

        if (debugParameters)
            Debug.Log($"H: {horizontal:F2}, V: {vertical:F2}");
    }
}
