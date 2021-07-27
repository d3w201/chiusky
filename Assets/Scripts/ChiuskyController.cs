using Doublsb.Dialog;
using InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
[RequireComponent(typeof(PlayerInput))]
#endif
public class ChiuskyController : MonoBehaviour
{
    [Header("Chiusky")] 
    public bool isMove;
    public bool move;
    
    [Tooltip("speed in m/s")]
    public float moveSpeed = 2.0f;
    public float sprintSpeed = 5.335f;
    public float aimSpeed = 0.1f;
    
    [Tooltip("Acceleration and deceleration")] 
    public float speedChangeRate = 10.0f;
    
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;
    
    private InputsHandler _inputHandler;
    private CharacterController _controller;
    private GameObject _mainCamera;
    private Animator _animator;
    
    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _animationBlend;
    private GameObject _focusItem;
    private GameObject _diaologAsset;
    private DialogManager _diaolog;
    private GameObject _gameManager;
    private GameController _gameController;

    private int _animIDSpeed;
    private int _animIDAim;
    private int _animIDAttack;
    private bool _hold;

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("speed");
        _animIDAim = Animator.StringToHash("aim");
        _animIDAttack = Animator.StringToHash("attack");
    }
    private void Awake()
    {
        if (!_mainCamera)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("CC00");
        }
        if (!_diaologAsset)
        {
            _diaologAsset = GameObject.FindGameObjectWithTag("DialogAsset");
        }
        if (!_gameManager)
        {
            _gameManager = GameObject.FindGameObjectWithTag("GameManager");
        }
    }
    void Start()
    {
        
        _inputHandler = GetComponent<InputsHandler>();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _diaolog = _diaologAsset.GetComponent<DialogManager>();
        _gameController = _gameManager.GetComponent<GameController>();
        
        AssignAnimationIDs();
    }
    void Update()
    {
        if (GameStatus.play.Equals(_gameController.GetStatus()))
        {
            HandleGravity();
            if (!_hold)
            {
                HandleMovement();
            }
        }
    }
    
    public void HandleAim(InputValue inputValue)
    {
        if (GameStatus.play.Equals(_gameController.GetStatus()))
            _animator.SetBool(_animIDAim,inputValue.isPressed);
    }

    public void HandleHit()
    {
        Debug.Log("HIT");
    }

    public void HandleInteract(InputValue value)
    {
        if (GameStatus.play.Equals(_gameController.GetStatus()))
        {
            if (_animator.GetBool(_animIDAim))
            {
                HandleCombat();
            }
            else if (GetFocusItem())
            {
                _gameController.SetStatus(GameStatus.interact);
                HandleStop();
                
                _diaolog.Show(new DialogData(GetFocusItem().tag));
            }
        }
        else if(GameStatus.interact.Equals(_gameController.GetStatus()))
        {
            _diaolog.Click_Window();
            if (State.Deactivate.Equals(_diaolog.state))
            {
                _gameController.SetStatus(GameStatus.play);
                HandleStart();
            }
        }
    }

    private void HandleCombat()
    {
        _animator.SetTrigger(_animIDAttack);
    }

    private void HandleGravity()
    {
        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        // ... ??? whatever it works for going up and down stairs / climbs
        if (_verticalVelocity < 53.0f)
        {
            _verticalVelocity += -15.0f * Time.deltaTime;
        }
    }
    private void HandleMovement()
    {
        move = !Vector2.zero.Equals(_inputHandler.move);
        isMove = _inputHandler.isMove;
        
        //SET INITIAL SPEED
        var targetSpeed = _animator.GetBool(_animIDAim) ? aimSpeed : _inputHandler.sprint ? sprintSpeed : moveSpeed ;
        if (_inputHandler.move == Vector2.zero) targetSpeed = 0.0f;
        _speed = targetSpeed;
        
        //player absolute direction
        var inputDirection = new Vector3(_inputHandler.move.x, 0.0f, _inputHandler.move.y).normalized;
        
        //acceleration (optional)
        var velocity = _controller.velocity;
        var currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;
        //Debug.Log($"currentHorizontalSpeed :: {currentHorizontalSpeed}");
        
        const float speedOffset = 0.1f;
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * 1f, Time.deltaTime * speedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }
        //end acceleration snippet

        //Rotation
        if (_inputHandler.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        else
        {
            _speed = 0.0f;
        }

        //MOVE
        var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        
        //AnimationBlend
        _animationBlend = Mathf.Lerp(_animationBlend, _speed, Time.deltaTime * speedChangeRate);
        _animator.SetFloat(_animIDSpeed, _animationBlend);
    }
    
    private void HandleStop()
    {
        _hold = true;
        _speed = 0f;
        _controller.SimpleMove(Vector3.zero);
        _animator.SetFloat(_animIDSpeed, 0f);
    }

    private void HandleStart()
    {
        _hold = false;
    }
    
    //Getter & Setter
    public GameObject GetFocusItem()
    {
        return this._focusItem;
    }
    
    public void SetFocusItem(GameObject item)
    {
        this._focusItem = item;
    }

    public void SetCamera(GameObject cam)
    {
        HandleStop();
        _mainCamera = cam;
        HandleStart();
    }

    public GameObject GetCamera()
    {
        return _mainCamera;
    }
}