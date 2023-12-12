using UnityEngine;
using UnityEngine.InputSystem;

// required components
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _playerSpeed = 2.0f;
    [SerializeField] private float _jumpHeight = 1.0f;
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _animationPlayTransitionTime = 0.13f;


    private CharacterController _controller;
    private PlayerInput _playerInput;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;

    // List my input actions
    private InputAction _movementAction;
    private InputAction _jumpAction;

    private Animator _animator;
    private int _movementXAnimParamId;
    private int _movementZAnimParamId;
    private int _jumpStateId;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();

        _movementAction = _playerInput.actions["Movement"];
        _jumpAction = _playerInput.actions["Jump"];

        // retrieving the correct IDs
        _movementXAnimParamId = Animator.StringToHash("MovementX");
        _movementZAnimParamId = Animator.StringToHash("MovementZ");
        _jumpStateId = Animator.StringToHash("Jump");

        _movementAction = _playerInput.actions["Movement"];
        _jumpAction = _playerInput.actions["Jump"];
    }

    void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.MainMenu) return;

        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0) _playerVelocity.y = 0f;

        Vector2 input = _movementAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(input.x, 0, input.y);
        _controller.Move(move * Time.deltaTime * _playerSpeed);

        _animator.SetFloat(_movementXAnimParamId, input.x);
        _animator.SetFloat(_movementZAnimParamId, input.y);

        if (_jumpAction.triggered && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
            //_animator.SetTrigger(_jumpAnimParamId);
            _animator.CrossFade(_jumpStateId, _animationPlayTransitionTime);

        }
        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }
}