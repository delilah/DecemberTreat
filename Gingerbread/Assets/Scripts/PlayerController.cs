using UnityEngine;
using UnityEngine.InputSystem;
using CardboardCore.DI;
using CardboardCore.Utilities;

// required components
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]


public class PlayerController : CardboardCoreBehaviour
{
    [SerializeField] private float _playerSpeed = 2.0f;
    [SerializeField] private float _jumpHeight = 1.0f;
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _animationPlayTransitionTime = 0.13f;
    [SerializeField] private float _smoothAnimTime = 0.1f;
    [SerializeField] private float _rotationSpeed = 4f;

    private CharacterController _controller;
    private PlayerInput _playerInput;
    private Transform _cameraTransform;

    // List my input actions
    private InputAction _movementAction;
    private InputAction _jumpAction;

    private Animator _animator;

    private Vector3 _playerVelocity;
    private Vector2 _currentAnimBlendVector;
    private Vector2 _animVelocity;

    private int _movementXAnimParamId;
    private int _movementZAnimParamId;
    private int _jumpStateId;

    private bool _groundedPlayer;

    private bool _isGameState;


    protected override InjectTiming MyInjectTiming => InjectTiming.OnEnable;
    [Inject] private GameManager _gameManager;

    protected override void OnInjected()
    {
        Log.Write(_gameManager.InjectedGM);
    }

    protected override void OnReleased()
    {
        Log.Write(_gameManager.ReleasedGM);
    }

    protected override void Start()
    {
        base.Start();
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();

        // Getting the camera transform for later use
        _cameraTransform = Camera.main.transform;

        _movementAction = _playerInput.actions["Movement"];
        _jumpAction = _playerInput.actions["Jump"];

        // retrieving the correct IDs
        _movementXAnimParamId = Animator.StringToHash("MovementX");
        _movementZAnimParamId = Animator.StringToHash("MovementZ");
        _jumpStateId = Animator.StringToHash("Jump");

        _movementAction = _playerInput.actions["Movement"];
        _jumpAction = _playerInput.actions["Jump"];
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.OnStateChanged += HandleStateChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameManager.GameState state)
    {
        _isGameState = (state == GameManager.GameState.Game);
    }

    void Update()
    {
        if (!_isGameState) return;


        _groundedPlayer = _controller.isGrounded;

        if (_groundedPlayer && _playerVelocity.y < 0) _playerVelocity.y = 0f;

        Vector2 input = _movementAction.ReadValue<Vector2>();

        // Interpolate the movement so it "feels" better
        _currentAnimBlendVector = Vector2.SmoothDamp(_currentAnimBlendVector, input, ref _animVelocity, _smoothAnimTime);

        Vector3 move = new Vector3(_currentAnimBlendVector.x, 0, _currentAnimBlendVector.y);

        // We factor in the camera transform so our user moves in relation to the camera.
        // Forward is forward based on the camera itself, normalized is so we get the returned vector with
        // a magnitude of 1.
        move = move.x * _cameraTransform.right.normalized + move.z * _cameraTransform.forward.normalized;

        // Sanity: we want to make sure the y stays 0 even after the move calculations
        move.y = 0f;

        _controller.Move(move * Time.deltaTime * _playerSpeed);

        _animator.SetFloat(_movementXAnimParamId, input.x);
        _animator.SetFloat(_movementZAnimParamId, input.y);

        if (_jumpAction.triggered && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
            _animator.CrossFade(_jumpStateId, _animationPlayTransitionTime);

        }
        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);

        // For camera to rotate with the player movement
        float angle = _cameraTransform.eulerAngles.y;

        // We rotate the y, Lerp to smoothen it
        Quaternion finalRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, _rotationSpeed * Time.deltaTime);
    }
}