using UnityEngine;
using UnityEngine.InputSystem;

// required components
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _playerSpeed = 2.0f;
    [SerializeField] private float _jumpHeight = 1.0f;
    [SerializeField] private float _gravityValue = -9.81f;

    private CharacterController _controller;
    private PlayerInput _playerInput;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;

    // List my input actions
    private InputAction _movementAction;
    private InputAction _jumpAction;

    private void Start()
    {
        // These components are required (see top of this class) so I want to just access them directly
        // I choose to see errors rather than adding the components because I want to have ownership on 
        // the settings and make sure they're properly configured
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();

        _movementAction = _playerInput.actions["Movement"];
        _jumpAction = _playerInput.actions["Jump"];
    }

    void Update()
    {
        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0) _playerVelocity.y = 0f;

        Vector2 input = _movementAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(input.x, 0, input.y);
        _controller.Move(move * Time.deltaTime * _playerSpeed);

        if (_jumpAction.triggered && _groundedPlayer) _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);

        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }
}