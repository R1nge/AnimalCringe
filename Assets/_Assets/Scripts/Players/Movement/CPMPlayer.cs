using System.Collections.Generic;
using _Assets.Scripts;
using _Assets.Scripts.Players;
using _Assets.Scripts.Players.Movement;
using Unity.Netcode;
using UnityEngine;

public class CPMPlayer : NetworkBehaviour
{
    [SerializeField] private float gravity = 20.0f;
    [SerializeField] private float friction = 6;
    [SerializeField] private float moveSpeed = 7.0f; // Ground move speed
    [SerializeField] private float runAcceleration = 14.0f; // Ground accel
    [SerializeField] private float runDeacceleration = 10.0f; // Deacceleration that occurs when running on the ground
    [SerializeField] private float airAcceleration = 2.0f; // Air accel
    [SerializeField] private float airDecceleration = 2.0f; // Deacceleration experienced when opposite strafing
    [SerializeField] private float airControl = 0.3f; // How precise air control is
    [SerializeField] private float sideStrafeAcceleration = 50.0f; // How fast acceleration occurs to get up to sideStrafeSpeed when side strafing
    [SerializeField] private float sideStrafeSpeed = 1.0f; // What the max speed to generate when side strafing
    [SerializeField] private float jumpSpeed = 8.0f; // The speed at which the character's up axis gains when hitting jump
    [SerializeField] private bool holdJumpToBhop; // When enabled allows player to just hold jump button to keep on bhopping perfectly. Beware: smells like casual.
    private CharacterController _characterController;
    private PlayerInput _playerInput;
    private ClientNetworkTransform _clientNetworkTransform;

    private Vector3 _velocity = Vector3.zero;

    // Q3: players can queue the next jump just before he hits the ground
    private readonly Queue<PlayerInputCommand> _playerInputQueue = new();
    private PlayerInputCommand _lastInput;

    public Vector3 Velocity => _velocity;

    public void RollbackSetVelocity(Vector3 velocity)
    {
        _velocity = velocity;
        _characterController.Move(_velocity * Time.deltaTime);
    }


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
        _clientNetworkTransform = GetComponent<ClientNetworkTransform>();
    }

    private void OnTick()
    {
        if (IsServer) return;
        if (!IsOwner) return;
        if (!_playerInput.Enabled) return;
        MoveServerRpc();
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!_playerInput.Enabled) return;

        GetInput();
        QueueJump();
        SendInputServerRpc(_lastInput);

        if (_characterController.isGrounded)
        {
            GroundMove(_lastInput);
        }
        else
        {
            AirMove(_lastInput);
        }

        _characterController.Move(_velocity * Time.deltaTime);
    }

    public void AddForce(Vector3 force)
    {
        ApplyFriction(0);
        _velocity += force;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void QueueJump()
    {
        if (holdJumpToBhop)
        {
            _lastInput.WishJump = Input.GetButton("Jump");
            return;
        }

        if (Input.GetButtonDown("Jump") && !_lastInput.WishJump)
            _lastInput.WishJump = true;
        if (Input.GetButtonUp("Jump"))
            _lastInput.WishJump = false;
    }

    private void GetInput()
    {
        _lastInput = new PlayerInputCommand
        {
            ForwardMove = Input.GetAxisRaw("Vertical"),
            RightMove = Input.GetAxisRaw("Horizontal")
        };
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void SendInputServerRpc(PlayerInputCommand playerInputCommand) => _playerInputQueue.Enqueue(playerInputCommand);

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void MoveServerRpc()
    {
        if (_playerInputQueue.Count <= 0) return;
        PlayerInputCommand inputData = _playerInputQueue.Dequeue();
        if (_characterController.isGrounded)
        {
            GroundMove(inputData);
        }
        else
        {
            AirMove(inputData);
        }

        _characterController.Move(_velocity);
    }

    private void GroundMove(PlayerInputCommand input)
    {
        if (!_lastInput.WishJump)
            ApplyFriction(1.0f);
        else
            ApplyFriction(0);

        var wishDir = new Vector3(input.RightMove, 0, input.ForwardMove);
        wishDir = transform.TransformDirection(wishDir);
        wishDir.Normalize();

        float wishSpeed = wishDir.magnitude;
        wishSpeed *= moveSpeed;

        Accelerate(wishDir, wishSpeed, runAcceleration);

        _velocity.y = -gravity * Time.deltaTime;

        if (_lastInput.WishJump)
        {
            _velocity.y = jumpSpeed;
            _lastInput.WishJump = false;
            _clientNetworkTransform.Interpolate = false;
        }
        else
        {
            _clientNetworkTransform.Interpolate = true;
        }
    }

    private void AirMove(PlayerInputCommand input)
    {
        var wishDir = new Vector3(input.RightMove, 0, input.ForwardMove);
        wishDir = transform.TransformDirection(wishDir);

        float wishSpeed = wishDir.magnitude;
        wishSpeed *= moveSpeed;

        wishDir.Normalize();

        // CPM: Aircontrol
        float wishSpeed2 = wishSpeed;
        float accel = Vector3.Dot(_velocity, wishDir) < 0 ? airDecceleration : airAcceleration;
        // If the player is ONLY strafing left or right
        if (input.ForwardMove == 0 && input.RightMove != 0)
        {
            if (wishSpeed > sideStrafeSpeed)
                wishSpeed = sideStrafeSpeed;
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishDir, wishSpeed, accel);
        if (airControl > 0)
            AirControl(wishDir, wishSpeed2, input);
        // !CPM: Aircontrol

        _velocity.y -= gravity * Time.deltaTime;
    }

    private void AirControl(Vector3 wishDir, float wishSpeed, PlayerInputCommand input)
    {
        // Can't control movement if not moving forward or backward
        if (Mathf.Abs(input.ForwardMove) < 0.001 || Mathf.Abs(wishSpeed) < 0.001)
            return;
        float speedY = _velocity.y;
        _velocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        float speed = _velocity.magnitude;
        _velocity.Normalize();

        float dot = Vector3.Dot(_velocity, wishDir);
        float k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0)
        {
            _velocity.x = _velocity.x * speed + wishDir.x * k;
            _velocity.y = _velocity.y * speed + wishDir.y * k;
            _velocity.z = _velocity.z * speed + wishDir.z * k;

            _velocity.Normalize();
        }

        _velocity.x *= speed;
        _velocity.y = speedY; // Note this line
        _velocity.z *= speed;
    }

    private void ApplyFriction(float frictionMultiplier)
    {
        Vector3 vec = _velocity; // Equivalent to: VectorCopy();

        vec.y = 0.0f;
        float speed = vec.magnitude;
        var drop = 0.0f;

        /* Only if the player is on the ground then apply friction */
        if (_characterController.isGrounded)
        {
            float control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * Time.deltaTime * frictionMultiplier;
        }

        float newSpeed = speed - drop;

        if (newSpeed < 0)
            newSpeed = 0;
        if (speed > 0)
            newSpeed /= speed;

        _velocity.x *= newSpeed;
        _velocity.z *= newSpeed;
    }

    private void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
    {
        float currentSpeed = Vector3.Dot(_velocity, wishDir);
        float deltaSpeed = wishSpeed - currentSpeed;
        if (deltaSpeed <= 0)
            return;
        float acceleratedSpeed = accel * Time.deltaTime * wishSpeed;
        if (acceleratedSpeed > deltaSpeed)
            acceleratedSpeed = deltaSpeed;

        _velocity.x += acceleratedSpeed * wishDir.x;
        _velocity.z += acceleratedSpeed * wishDir.z;
    }
}