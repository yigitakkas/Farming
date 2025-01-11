using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float SprintSpeed = 8f;
    public float RotationSpeed = 10f;
    public float JumpForce = 8f;
    public float Gravity = -9.81f;
    
    [Header("Ground Check")]
    public Transform GroundCheck;
    public float GroundDistance = 0.2f;
    public LayerMask GroundMask;
    
    [Header("References")]
    public Animator Animator;
    public Transform ModelTransform;
    
    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _currentSpeed;
    
    // Animator parameter hashes for better performance
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _currentSpeed = MoveSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        HandleSprint();
        UpdateAnimations();
    }
    
    // Checks if player is touching ground
    private void CheckGround()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        
        if (wasGrounded != _isGrounded)
        {
            Animator.SetBool(IsGrounded, _isGrounded);
        }
        
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;  // Small downward force when grounded
        }
    }
    
    // Handles all movement-related updates
    private void HandleMovement()
    {
        Vector3 move = GetMovementInput();
        
        if (move.magnitude > 0.1f)
        {
            MoveCharacter(move);
            RotateModel(move);
        }
        
        ApplyGravity();
    }
    
    // Gets raw input and converts to world space movement
    private Vector3 GetMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return transform.right * horizontal + transform.forward * vertical;
    }
    
    // Applies movement to character controller
    private void MoveCharacter(Vector3 move)
    {
        _controller.Move(move.normalized * _currentSpeed * Time.deltaTime);
    }
    
    // Rotates character model to face movement direction
    private void RotateModel(Vector3 move)
    {
        if (ModelTransform != null)
        {
            Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
            ModelTransform.rotation = Quaternion.Lerp(ModelTransform.rotation, toRotation, RotationSpeed * Time.deltaTime);
        }
    }
    
    // Applies gravity to character
    private void ApplyGravity()
    {
        _velocity.y += Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
    
    // Handles jump input and physics
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpForce * -2f * Gravity);
            Animator.SetTrigger(Jump);
            ResetMovementAnimations();
        }
    }
    
    // Toggles between walk and sprint speeds
    private void HandleSprint()
    {
        _currentSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : MoveSpeed;
    }
    
    // Updates animation states based on movement
    private void UpdateAnimations()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMoving = input.magnitude > 0.1f;
        
        if (isMoving)
        {
            Animator.SetBool(IsWalking, true);
            Animator.SetBool(IsRunning, _currentSpeed == SprintSpeed);
        }
        else
        {
            ResetMovementAnimations();
        }
    }
    
    // Resets all movement-related animations
    private void ResetMovementAnimations()
    {
        Animator.SetBool(IsWalking, false);
        Animator.SetBool(IsRunning, false);
    }
} 