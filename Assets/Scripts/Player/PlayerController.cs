using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float MoveSpeed = 5f;
    public float SprintSpeed = 8f;
    public float RotationSpeed = 10f;
    public float JumpForce = 8f;
    public float Gravity = -9.81f;
    
    [Header("Ground Check")]
    public Transform GroundCheck;
    public float GroundDistance = 0.2f;
    public LayerMask GroundMask;
    
    [Header("Animation")]
    public Animator Animator;
    
    private CharacterController _controller;
    private Camera _mainCamera;
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _currentSpeed;
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int Jump = Animator.StringToHash("Jump");
    
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        _currentSpeed = MoveSpeed;
        
        if (Animator == null)
        {
            Animator = GetComponentInChildren<Animator>();
        }
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleSprint();
        UpdateAnimations();
    }
    
    private void HandleMovement()
    {
        // Ground check
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Calculate movement direction
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        
        // Store movement magnitude for animations
        float moveAmount = move.magnitude;
        
        // Apply movement
        _controller.Move(move.normalized * _currentSpeed * Time.deltaTime);
        
        // Apply gravity
        _velocity.y += Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
    
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpForce * -2f * Gravity);
            Animator.SetTrigger(Jump);
        }
    }
    
    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = SprintSpeed;
        }
        else
        {
            _currentSpeed = MoveSpeed;
        }
    }
    
    private void UpdateAnimations()
    {
        // Get the actual movement magnitude
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMoving = input.magnitude > 0.1f;
        
        // Set animation parameters
        Animator.SetBool(IsWalking, isMoving && _currentSpeed == MoveSpeed);
        Animator.SetBool(IsRunning, isMoving && _currentSpeed == SprintSpeed);
    }
} 