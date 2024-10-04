using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[RequireComponent(typeof(ActorController), typeof(PlayerDamage))]

public class Player : Actor
{
 
    [SerializeField] float _moveSpeed = 16.5f;                  
    [SerializeField] float _groundMoveAcceleration = 9.865f;   
    [SerializeField] float _groundMoveDeceleration = 19f;    
    [SerializeField] float _airMoveAcceleration = 5.5f;    
    [SerializeField] float _airMoveDeceleration = 8f;   
    [SerializeField] float _runDustEffectDelay = 0.2f;  

  
    [Space(10)]
    [SerializeField] float _jumpForce = 24f;           
    [SerializeField] float _jumpHeight = 4.5f;         
    [SerializeField] float _doubleJumpHeight = 2.0f;  
    [SerializeField] float _maxCoyoteTime = 0.06f;      
    [SerializeField] float _maxJumpBuffer = 0.25f;  

   
    [Space(10)]
    [SerializeField] float _slidingForce = 24f;         
    [SerializeField] float _dodgeDuration = 0.25f;       
    [SerializeField] float _dodgeInvinsibleTime = 0.15f; 
    [SerializeField] float _dodgeCooldown = 0.15f;      
    [SerializeField] float _dodgeDustEffectDelay = 0.1f; 

  
    [Space(10)]
    [SerializeField] float _wallSlidingSpeed = 8.5f; 
    [SerializeField] float _wallJumpHeight = 1.5f;  
    [SerializeField] float _wallJumpXForce = 8f;    
    [SerializeField] float _wallSlideDustEffectDelay = 0.15f;

  
    [Space(10)]
    [SerializeField] int _power = 3;                           
    [SerializeField] float _basicAttackKnockBackForce = 8.0f;  
    [SerializeField] float _fireBallKnockBackForce = 7.5f;     

   
    [Space(10)]
    [SerializeField] Image _healingEffect;  

    
    [Space(10)]
    [SerializeField] AudioClip _swingSound;        
    [SerializeField] AudioClip _fireBallSound;     
    [SerializeField] AudioClip _attackHitSound;    
    [SerializeField] AudioClip _stepSound;          
    [SerializeField] AudioClip _jumpSound;          

    float _coyoteTime;     
    float _maxJumpHeight;

    float _moveX;   

    float _nextWallSlideDustEffectTime = 0;
    float _nextRunDustEffectTime = 0;      

    bool _canMove = true;       
    bool _canWallSliding = true; 
    bool _hasDoubleJumped;       
    bool _canDodge = true;      

  

  
    int _xAxis;            
    bool _leftMoveInput;    
    bool _rightMoveInput;   
    float _timeHeldBackInput;      

   
    bool _jumpInput;       
    bool _jumpDownInput;   
    float _jumpBuffer;     
   
    bool _attackInput;     
    bool _specialInput;   
    bool _dodgeInput;       
    bool _isJumped;           
    bool _isFalling;           
    bool _isAttacking;       
    bool _isDodging;          
    bool _isWallSliding;      
    bool _isResting;           
    bool isBeingKnockedBack;   

    KnockBack _basicAttackKnockBack = new KnockBack();
    KnockBack _fireBallKnockBack = new KnockBack();

    Transform _backCliffChecked;           
    Coroutine _knockedBackCoroutine = null; 
    PlayerAttack _attack;                  
    PlayerDrivingForce _drivingForce;       
    PlayerDamage _damage;                 

    #region MonoBehaviour

    protected override void Awake()
    {
        base.Awake();

       
        _backCliffChecked = transform.Find("BackCliffChecked").GetComponent<Transform>();
        _damage = GetComponent<PlayerDamage>();
        _drivingForce = GetComponent<PlayerDrivingForce>();
        _attack = GetComponent<PlayerAttack>();

      
        _basicAttackKnockBack.force = _basicAttackKnockBackForce;
        _fireBallKnockBack.force = _fireBallKnockBackForce;

  
        _damage.KnockBack += OnKnockedBack;
        _damage.Died += OnDied;
    }

    void Start()
    {
        if (!GameManager.instance.IsStarted())
        {
            
            actorTransform.position = GameManager.instance.playerStartPos;
            actorTransform.localScale = new Vector3(GameManager.instance.playerStartlocalScaleX, 1, 1);

            _damage.CurrentHealth = GameManager.instance.playerCurrentHealth;
            _drivingForce.CurrentDrivingForce = GameManager.instance.playerCurrentDrivingForce;

            GameManager.instance.GameSave();
        }
        else
        {
         
            GameManager.instance.playerCurrentHealth = _damage.CurrentHealth;
            GameManager.instance.playerCurrentDrivingForce = _drivingForce.CurrentDrivingForce;

            if (GameManager.instance.firstStart)
            {
             
                GameManager.instance.playerResurrectionPos = actorTransform.position;
                GameManager.instance.resurrectionScene = SceneManager.GetActiveScene().name;
                GameManager.instance.firstStart = false;
            }
            else
            {
                if (GameManager.instance.resurrectionScene == SceneManager.GetActiveScene().name)
                {
                    actorTransform.position = GameManager.instance.playerStartPos;
                }
            }
        }
    }

    void Update()
    {
       
        if (isDead || _isResting) return;

      
        deltaTime = Time.deltaTime;

       
        HandleInput();

   
        HandleMove();
        HandleJump();
        HandleWallSlide();
        HandleFallingAndLanding();
        HandleAttack();
        HandleDodge();

       
        AnimationUpdate();
    }

    #endregion


    #region Input

    
    void HandleInput()
    {
      
        if (GameManager.instance.currentGameState != GameManager.GameState.Play) return;

       
        _leftMoveInput = GameInputManager.PlayerInput(GameInputManager.PlayerActions.MoveLeft);
        _rightMoveInput = GameInputManager.PlayerInput(GameInputManager.PlayerActions.MoveRight);
        _xAxis = _leftMoveInput ? -1 : _rightMoveInput ? 1 : 0;

       
        _jumpInput = GameInputManager.PlayerInput(GameInputManager.PlayerActions.Jump);
        _jumpDownInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Jump);

       
        _attackInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Attack);
        _specialInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.SpecialAttack);
        _dodgeInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Dodge);
    }

    #endregion

    #region Move

    
    void HandleMove()
    {
      
        if (!_canMove) return;

        if(_xAxis != 0)
        {
          
            _moveX += (controller.IsGrounded ? _groundMoveAcceleration : _airMoveAcceleration) * deltaTime;

            if (actorTransform.localScale.x != _xAxis)
            {
                if(!_isAttacking)
                {
                    Flip();
                    _moveX = 0;
                    if(controller.IsGrounded)
                    {
                        _nextRunDustEffectTime = 0;
                    }
                }
            }

         
            if(controller.IsGrounded)
            {
               _nextRunDustEffectTime -= deltaTime;
               if(_nextRunDustEffectTime <= 0)
               {
                 
                    ObjectPoolManager.instance.GetPoolObject("RunDust", actorTransform.position, actorTransform.localScale.x);
                    _nextRunDustEffectTime = _runDustEffectDelay;   
                   
                    SoundManager.instance.SoundEffectPlay(_stepSound);
                    GamepadVibrationManager.instance.GamepadRumbleStart(0.02f, 0.017f);
               }
            }
        }
        else
        {
           
            _moveX -= (controller.IsGrounded ? _groundMoveDeceleration : _airMoveDeceleration) * deltaTime;
            _nextRunDustEffectTime = 0;
        }
        _moveX = Mathf.Clamp(_moveX, 0f, 1f);  
        controller.VelocityX = _xAxis * _moveSpeed * _moveX;   
    }

    #endregion

    #region Jump

    
    void HandleJump()
    {
       
        if(controller.IsGrounded || controller.IsWalled)
        {
            _hasDoubleJumped = false;
        }

       
        if (!_isJumped)
        {
         
            if(JumpInputBuffer() && CoyoteTime())
            {
             
                if (!_isAttacking && !_isDodging && !isBeingKnockedBack)
                {
                    StartJump();
                }
            }
          
            else if(PlayerLearnedSkills.hasLearnedDoubleJump)
            {
             
                if(!_hasDoubleJumped && !controller.IsGrounded && !_isWallSliding)
                {
                    if(_jumpDownInput)
                    {
                        StartDoubleJump();
                    }
                }
            }
        }
        else
        {
            ContinueJumping();
        }
    }

   
    void StartJump()
    {
        _isJumped = true;  

       
        _coyoteTime = _maxCoyoteTime;
        _jumpBuffer = _maxJumpBuffer;

     
        _maxJumpHeight = actorTransform.position.y + _jumpHeight;

       
        ObjectPoolManager.instance.GetPoolObject("JumpDust", actorTransform.position);
        GamepadVibrationManager.instance.GamepadRumbleStart(0.5f, 0.05f);
        SoundManager.instance.SoundEffectPlay(_stepSound);
        SoundManager.instance.SoundEffectPlay(_jumpSound);
    }

    
    void StartDoubleJump()
    {
       
        _isJumped = true;
        _hasDoubleJumped = true;
       
        _maxJumpHeight = actorTransform.position.y + _doubleJumpHeight;
       
        animator.SetTrigger(GetAnimationHash("DoubleJump"));
       
        ObjectPoolManager.instance.GetPoolObject("JumpDust", actorTransform.position);
        GamepadVibrationManager.instance.GamepadRumbleStart(0.5f, 0.05f);
        SoundManager.instance.SoundEffectPlay(_jumpSound);
    }

    
    void ContinueJumping()
    {
        controller.VelocityY = _jumpForce;
     
        if (_maxJumpHeight <= actorTransform.position.y)
        {
            _isJumped = false;
            controller.VelocityY = _jumpForce * 0.75f;
        }
      
        else if (!_jumpInput || controller.IsRoofed)
        {
            _isJumped = false;
            controller.VelocityY = _jumpForce * 0.5f;
        }
    }

   
    bool JumpInputBuffer()
    {
        if (_jumpInput)
        {
            
            if (_jumpBuffer < _maxJumpBuffer)
            {
                _jumpBuffer += deltaTime;
                return true;
            }
        }
        else
        {
            
            _jumpBuffer = 0;
        }
        return false;
    }

  
    bool CoyoteTime()
    {
        if(controller.IsGrounded)
        {
          
            _coyoteTime = 0;
            return true;
        }
        else if(_coyoteTime < _maxCoyoteTime)
        {
           
            _coyoteTime += deltaTime;
            return true;
        }
        
        return false;
    }

  
    void HandleFallingAndLanding()
    {
        if (controller.VelocityY < -5f)
        {
           
            _isFalling = true;
        }
        else if (controller.IsGrounded && _isFalling)
        {
           
            _isFalling = false;
          
            SoundManager.instance.SoundEffectPlay(_stepSound);
            ObjectPoolManager.instance.GetPoolObject("JumpDust", actorTransform.position);
            GamepadVibrationManager.instance.GamepadRumbleStart(0.25f, 0.05f);
        }
    }

    #endregion

    #region ClimbingWall

    /// <summary>
    /// 플레이어 캐릭터가 벽에 붙어는 상태일 때 벽에서 미끄러지는 기능을 처리하기 위한 메소드입니다.
    /// </summary>
    void HandleWallSlide()
    {
        // 벽차기를 배우지 않은 상태, 벽에서 미끄러지기가 가능하지 않은 상태이거나 공격 중인 상태이면 실행하지 않음
        if(!PlayerLearnedSkills.hasLearnedClimbingWall || !_canWallSliding || _isAttacking) return;

        if(!_isWallSliding)
        {
            // 아직 벽에서 미끄러지고 있는 상태가 아닐 경우를 처리
            // 벽에 접촉한 상태에서 땅에 닿지 않고 점프하고 있지 않을 경우 
            if(controller.IsWalled && !controller.IsGrounded && controller.VelocityY < 0)
            {
                // 벽에 접촉한 방향과 입력하고 있는 X축 방향이 일치하면 벽에서 미끄러지기를 실행합니다.
                int wallDirection = controller.IsLeftWalled ? -1 : 1;
                if (_xAxis == wallDirection)
                {
                    StartWallSliding();
                }
            }
        }
        else
        {
            // 벽에서 미끄러지고 있는 상태일 때를 처리함
            ContinueWallSliding();
        }
    }

    /// <summary>
    /// 플레이어 캐릭터가 벽에서 미끄러지기를 실행하는 메소드입니다.
    /// </summary>
    void StartWallSliding()
    {
        _isWallSliding = true;
        _isDodging = _canMove = false;
        controller.SlideCancle();
    }

    /// <summary>
    /// 플레이어 캐릭터가 벽에서 미끄러지고 있는 상태일때를 처리하는 메소드입니다.
    /// </summary>
    void ContinueWallSliding()
    {
        int wallDirection = controller.IsLeftWalled ? -1 : 1;
        float dustEffectXPos = actorTransform.position.x + (wallDirection == 1 ? 0.625f : -0.625f);
        Vector2 dustEffectPos = new Vector2(dustEffectXPos, actorTransform.position.y + 1.25f);

        // 벽에서 서서히 미끄러져 내려오기 위해 Y 속도를 변경함
        controller.VelocityY = Mathf.Clamp(controller.VelocityY, -_wallSlidingSpeed, float.MaxValue);

        // 벽 방향의 X축 반대 방향을 0.2초 이상 입력하면 플레이어 캐릭터가 벽에서 떨어져 나옴
        bool backInput = (wallDirection == 1 && _leftMoveInput) || 
                         (wallDirection == -1 && _rightMoveInput);
        if (backInput)
        {
            _timeHeldBackInput += deltaTime;
            if(_timeHeldBackInput >= 0.2f)
            {
                WallSlidingCancle();
            }
        }
        else
        {
            // 입력 취소시 초기화
            _timeHeldBackInput = 0;
        }

        // 점프를 입력하면 플레이어가 벽 점프 실행
        if(_jumpDownInput)
        {
            // 점프 상태로 설정
            _isJumped =true;
            _coyoteTime = _maxCoyoteTime;
            _maxJumpHeight = actorTransform.position.y + _wallJumpHeight;

            // 벽 점프 시 바라보는 방향을 반대로 뒤집고 전방으로 미끄러지며 움직임
            Flip();
            controller.SlideMove(_wallJumpXForce, -wallDirection, 30f);
            // _moveX값을 즉시 1로 설정
            _moveX = 1.0f; 

            // 벽 점프 먼지 이펙트 활성화, 게임 패드 진동, 사운드 재생
            ObjectPoolManager.instance.GetPoolObject("WallJumpDust", dustEffectPos, -actorTransform.localScale.x);
            GamepadVibrationManager.instance.GamepadRumbleStart(0.5f, 0.05f);
            SoundManager.instance.SoundEffectPlay(_stepSound);

            // 벽에서 미끄러지는 상태 취소
            WallSlidingCancle();
        }

        // 벽에 닿지 않은 상태, 땅에 닿은 상태, 넉백 중인 상태가 하나라도 해당되면 벽에서 미끄러지기를 취소함
        if(!controller.IsWalled || controller.IsGrounded || isBeingKnockedBack)
        {
            WallSlidingCancle();
        }

        // 벽에서 미끄러지는 동안 지속적으로 이펙트 실행
        _nextWallSlideDustEffectTime -= deltaTime;
        if (_nextWallSlideDustEffectTime <= 0)
        {
            _nextWallSlideDustEffectTime = _wallSlideDustEffectDelay;
            // 먼지 이펙트 생성, 게임 패드 진동, 사운드 재생
            ObjectPoolManager.instance.GetPoolObject("WallSlideDust", dustEffectPos,actorTransform.localScale.x);
            GamepadVibrationManager.instance.GamepadRumbleStart(0.1f, 0.033f);
            SoundManager.instance.SoundEffectPlay(_swingSound);
        }
    }

    /// <summary>
    /// 플레이어 캐릭터가 벽에서 미끄러지는 상태를 취소하는 기능을 하는 메소드입니다.
    /// </summary>
    void WallSlidingCancle()
    {
        _isWallSliding = false;
        _canMove = true;
        _timeHeldBackInput = 0;
        _nextWallSlideDustEffectTime = _wallSlideDustEffectDelay;
    }

    #endregion

    #region Attack

    /// <summary>
    /// 플레이어 캐릭터의 공격을 처리하는 메소드입니다.
    /// </summary>
    void HandleAttack()
    {
        // 다음 상태이면 실행하지 않음(공격 중, 벽에서 미끄러지는 중, 회피 중, 넉백 중)
        if(_isAttacking || _isWallSliding || _isDodging || isBeingKnockedBack) return;

        if(_attackInput)
        {
            // 공격 버튼 입력시 기본 공격 실행
            StartCoroutine(BasicAttack());
        }
        else if(_specialInput)
        {
            // 스페셜 버튼 입력시 스페셜 실행
            // 현재 원동력이 1 이상일 경우 원동력을 1 소모하고, 파이어볼을 실행
            if (_drivingForce.TryConsumeDrivingForce(1))
            {
                StartCoroutine(FireBall());
            }
        }
    }

    /// <summary>
    /// 플레이어 캐릭터의 기본 공격을 처리하기 위한 코루틴입니다.
    /// </summary>
    IEnumerator BasicAttack()
    {
        // 공격 중인 상태로 설정
        _isAttacking = true;

        bool isHit = false;
        bool isNextAttacked = false;

        // 기본 공격 애니메이션 실행, 버그 방지를 위해 애니메이션 종료 트리거를 리셋
        animator.SetTrigger(GetAnimationHash("BasicAttack"));
        animator.ResetTrigger(GetAnimationHash("AnimationEnd"));

        // 기본 공격의 넉백 방향을 바라보는 방향으로 설정
        _basicAttackKnockBack.direction = actorTransform.localScale.x;

        // 땅에 있을 경우 현재 이동속도 만큼 전방으로 미끄러짐
        if (controller.IsGrounded)
        {
            controller.SlideMove(_moveSpeed, actorTransform.localScale.x, 65f);
        }

        // 1프레임 이후 실행
        yield return null;
        
        // 무기를 휘두르는 사운드 재생
        SoundManager.instance.SoundEffectPlay(_swingSound);

        // 공격 애니메이션이 종료될 때 까지 실행
        while(!IsAnimationEnded())
        {
            // 넉백 될 경우 공격 취소
            if(isBeingKnockedBack) break;

            if(!isHit)
            {
                // 공격이 적중하지 않았을 때 지속적으로 공격을 실행하고 적중했는지 체크
                if (_attack.IsAttacking(_power, _basicAttackKnockBack))
                {
                    // 공격이 적중했을 경우

                    // 공격이 적중한 수 만큼 원동력 회복
                    for (int i = 0; i < _attack.HitCount; i++)
                    {
                        _drivingForce.IncreaseDrivingForce();
                    }
                    // 공격 적중 사운드 재생 및 게임 패드 진동
                    SoundManager.instance.SoundEffectPlay(_attackHitSound);
                    GamepadVibrationManager.instance.GamepadRumbleStart(0.5f, 0.05f);

                    // 플레이어가 바라보는 반대 방향으로 미끄러짐
                    controller.SlideMove(11.5f, -actorTransform.localScale.x);

                    // 플레이어가 공중에 있을 경우 위쪽으로 띄워짐
                    if (!controller.IsGrounded)
                    {
                        controller.VelocityY = 15;
                    }

                    // 공격이 적중한 상태로 설정
                    isHit = true;
                }
            }

            // 미끄러지는 도중 뒤쪽으로 절벽이 감지됐을 경우 미끄러짐을 멈춤
            if(!Physics2D.Raycast(_backCliffChecked.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground")))
            {
                controller.SlideCancle();
            }
            
            // 땅에 닿지 않은 상태일 경우 움직일 수 있는 상태로 설정
            _canMove = !controller.IsGrounded;

            // 현재 애니메이션 정규화 시간이 0.4에서 0.87 사이이고 공격 버튼을 누르면 isNextAttacked가 true로 설정됩니다.
            // isNextAttacked가 true이고 현재 애니메이션 정규화 시간이 0.87 이상이면 다음 공격을 실행한다
            if (IsAnimatorNormalizedTimeInBetween(0.4f, 0.87f))
            {
                if (_attackInput)
                {
                    isNextAttacked = true;
                }
            }
            else if(IsAnimatorNormalizedTimeInBetween(0.87f, 1.0f))
            {
                if(isNextAttacked)
                {
                    // x축 입력 방향과 바라보는 방향이 같다면 전방으로 미끄러짐
                    if(_xAxis == actorTransform.localScale.x)
                    {
                        controller.SlideMove(_moveSpeed, actorTransform.localScale.x, 65f);
                    }
                    // 다음 공격 애니메이션 실행
                    animator.SetTrigger(GetAnimationHash("NextAttack"));
                    // 사운드 재생
                    SoundManager.instance.SoundEffectPlay(_swingSound);

                    // 공격 적중 상태와 다음 공격 실행을 초기화
                    isHit = isNextAttacked = false;
                }
            }

            yield return null;
        }

        // 공격 종료 시 AttackEnd 메소드 실행
        AttackEnd();
    }

    /// <summary>
    /// 플레이어 캐릭터의 파이어볼 스페셜을 처리하기 위한 코루틴입니다.
    /// </summary>
    IEnumerator FireBall()
    {
        // 공격 중인 상태로 설정
        _isAttacking = true;

        // 파이어볼이 생성된 상태인지 체크
        bool hasSpawnedFireBall = false;

        // 파이어볼 애니메이션 실행, 버그 방지를 위해 애니메이션 종료 트리거를 리셋
        animator.SetTrigger(GetAnimationHash("FireBall"));
        animator.ResetTrigger(GetAnimationHash("AnimationEnd"));
        _fireBallKnockBack.direction = actorTransform.localScale.x;

        // 1프레임 이후 실행
        yield return null;

        // 파이어볼 사운드 재생
        SoundManager.instance.SoundEffectPlay(_fireBallSound);

        // 애니메이션이 종료될 때 까지 기능 실행
        while(!IsAnimationEnded())
        {
            // 넉백 중인 상태일 경우 중단
            if(isBeingKnockedBack) break;

            // 땅에 있을 경우 이동할 수 없게 설정
            if(controller.IsGrounded)
            {
                _moveX = 0;
                _canMove = false;
            }

            // 애니메이션 정규화 시간이 0.28 ~ 0.4 사이일 때 파이어볼 생성
            if(IsAnimatorNormalizedTimeInBetween(0.28f, 0.4f))
            {
                // 파이어볼이 아직 생성되지 않았을 경우에만 파이어볼 생성
                if(!hasSpawnedFireBall)
                {
                    float scaleX = actorTransform.localScale.x;
                    float addPosX = actorTransform.position.x + (0.66f * scaleX);
                    float addPosY = actorTransform.position.y + 1.12f;
                    Vector2 addPos = new Vector2(addPosX, addPosY);
                    float angle = scaleX == 1 ? 180 : 0;

                    ObjectPoolManager.instance.GetPoolObject("FireBall", addPos, scaleX, angle);

                    hasSpawnedFireBall = true;  // 파이어 볼이 생성된 상태로 설정
                }
            }

            yield return null;
        }

        // 공격 종료 시 AttackEnd 메소드 실행
        AttackEnd();
    }

    /// <summary>
    /// 공격이 종료됐을 때 처리해야 할 기능들을 실행하는 메소드입니다.
    /// </summary>
    void AttackEnd()
    {
        animator.SetTrigger(GetAnimationHash("AnimationEnd"));
        _isAttacking = false;
        _canMove = true;
    }

    #endregion

    #region Dodge

    /// <summary>
    /// 플레이어 캐릭터의 회피를 처리하는 메소드입니다.
    /// </summary>
    void HandleDodge()
    {
        // 다음 상태이면 실행하지 않음(공격 중, 벽에서 미끄러지는 중, 회피 중, 넉백 중)
        if (_isAttacking || _isWallSliding || _isDodging || isBeingKnockedBack) return;

        // 회피를 입력했을 때 회피가 가능한 상태이고 땅에 접촉한 상태이면 슬라이딩 실행
        if(_dodgeInput && _canDodge && controller.IsGrounded)
        {
            StartCoroutine(Dodging());
        }
    }

    /// <summary>
    /// 플레이어 캐릭터의 지상 회피 기능을 처리하는 코루틴입니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator Dodging()
    {
        // 회피 무적 시간 및 이펙트 생성 시간 설정
        float nextDodgeDustEffectTime = 0;
        float dodgeInvinsibleTime = _dodgeInvinsibleTime;
        float dodgeDuration = _dodgeDuration;

        // 회피 중인 상태로 설정
        _damage.IsDodged = _isDodging = true;
        // 회피와 이동이 불가능한 상태로 설정
        _canDodge = _canMove = false;

        // 회피 애니메이션 실행, 버그 방지를 위해 애니메이션 종료 트리거를 리셋
        animator.SetTrigger(GetAnimationHash("Sliding"));
        animator.ResetTrigger(GetAnimationHash("AnimationEnd"));

        // 1프레임 대기
        yield return null;
        
        // 미끄러지는 움직임 실행
        controller.SlideMove(_slidingForce, actorTransform.localScale.x);

        // 회피 지속 시간이 끝날 때 까지 회피 실행
        while(dodgeDuration > 0)
        {
            // 넉백 될 경우 회피 취소
            if(isBeingKnockedBack) break;

            // 회피 중 지속적으로 먼지 이펙트 생성 및 사운드 재생
            if(nextDodgeDustEffectTime <= 0)
            {
                ObjectPoolManager.instance.GetPoolObject("RunDust",
                                                        actorTransform.position,
                                                        actorTransform.localScale.x);
                nextDodgeDustEffectTime = _dodgeDustEffectDelay;
                SoundManager.instance.SoundEffectPlay(_jumpSound);
            }
            else
            {
                nextDodgeDustEffectTime -= deltaTime;
            }

            // 회피 중 무적 시간이 끝날 경우 무적 상태 취소
            if(dodgeInvinsibleTime <= 0)
            {
                _damage.IsDodged = false;
            }
            else
            {
                dodgeInvinsibleTime -= deltaTime;
            }

            // 실시간으로 회피 시간 감소
            dodgeDuration -= deltaTime;
            yield return null;
        }

        // 회피 종료 메소드 실행
        DodgeEnd();
    }

    /// <summary>
    /// 회피가 종료될 때 필요한 기능을 실행하는 메소드입니다.
    /// </summary>
    void DodgeEnd()
    {
        // 미끄러지는 움직임 중단
        controller.SlideCancle();
        // 애니메이션 종료
        animator.SetTrigger(GetAnimationHash("AnimationEnd"));

        // 회피 중인 상태를 종료하고 이동이 가능한 상태로 설정
        _isDodging = false;
        _canMove = true;

        // 회피 쿨다운 코루틴 실행
        StartCoroutine(DodgeCooldown());
    }

    /// <summary>
    /// 회피를 사용한 이후 일정 시간 뒤에 회피를 사용가능하는 코루틴입니다.
    /// </summary>
    IEnumerator DodgeCooldown()
    {
        // 회피 쿨다운 이후 다시 회피가 가능한 상태로 변경
        yield return YieldInstructionCache.WaitForSeconds(_dodgeCooldown);
        _canDodge = true;
    }

    #endregion

    #region Rest

    /// <summary>
    /// 체크 포인트에서 휴식을 실행하는 메소드 입니다. 플레이어 캐릭터의 체력을 회복하고 게임을 저장합니다. 플레이어가 사망하거나 게임을 이어서 할 경우 해당 위치에서 게임을 시작합니다.
    /// </summary>
    /// <param name="checkPointPos">체크 포인트의 좌표</param>
    public void RestAtCheckPoint(Vector2 checkPointPos)
    {
        // 플레이어가 공중에 있거나 이미 휴식중일 경우 휴식을 실행하지 않음
        if (!controller.IsGrounded || _isResting) return;

        // 플레이어의 부활 장소를 체크 포인트의 좌표와 현재 씬으로 설정
        GameManager.instance.playerResurrectionPos = checkPointPos;
        GameManager.instance.resurrectionScene = SceneManager.GetActiveScene().name;

        // 처치했던 적들 부활
        DeadEnemyManager.ClearDeadEnemies();

        // 게임 저장
        GameManager.instance.GameSave();

        // 휴식 코루틴 실행
        StartCoroutine("Resting");
    }

    /// <summary>
    /// 휴식을 처리하는 코루틴입니다.
    /// </summary>
    IEnumerator Resting()
    {
        float red = _healingEffect.color.r;
        float green = _healingEffect.color.g;
        float blue = _healingEffect.color.b;
        float alpha = 0.5f;

        // 휴식 실행
        _isResting = true;
        animator.ResetTrigger(GetAnimationHash("AnimationEnd"));
        animator.SetTrigger(GetAnimationHash("Rest"));
        yield return YieldInstructionCache.WaitForSecondsRealtime(0.2f);

        // 화면이 번쩍이는 이펙트를 실행한 후 체력을 전부 회복
        _healingEffect.enabled = AccessibilitySettingsManager.screenFlashes;
        _healingEffect.color = new Color(red, green, blue, alpha);
        _damage.HealthRecovery(_damage.maxHealth);
        yield return YieldInstructionCache.WaitForSecondsRealtime(0.1f);

        // 화면이 번쩍이는 이펙트를 천천히 제거
        while (alpha > 0f)
        {
            alpha -= 0.025f;
            _healingEffect.color = new Color(red, green, blue, alpha);

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }
        yield return YieldInstructionCache.WaitForSecondsRealtime(0.1f);

        // 휴식 종료
        animator.SetTrigger(GetAnimationHash("AnimationEnd"));
        _isResting = false;
    }

    #endregion

    #region Damage

    /// <summary>
    /// 플레이어 캐릭터가 적에게 공격을 받았을 때 넉백을 실행하기 위한 콜백 메소드입니다.
    /// </summary>
    /// <param name="knockBack">넉백 구조체</param>
    void OnKnockedBack(KnockBack knockBack)
    {
        // 중력을 사용하는 상태로 변경
        controller.UseGravity = true;

        // 플레이어 캐릭터의 좌표를 살짝 위로 올림(버그 방지)
        actorTransform.Translate(new Vector2(0, 0.1f));

        // 넉백량과 방향으로 미끄러지는 움직임을 실행하고 움직일 수 없는 상태로 설정
        controller.SlideMove(knockBack.force, knockBack.direction);
        _canMove = false;

        // 휴식 상태 코루틴을 중단하고 휴식 상태를 false로 설정
        StopCoroutine("Resting");
        _isResting = false;

        // 다음 상태를 모두 false로 설정(공격 상태, 회피 상태, 벽에서 미끄러지는 상태, 점프 상태)
        _isAttacking = _isDodging = _isWallSliding = _isJumped = false;
        // 넉백 중인 상태로 설정하고 게임 패드 진동 실행
        isBeingKnockedBack = true;
        GamepadVibrationManager.instance.GamepadRumbleStart(0.8f, 0.068f);

        if(!isDead)
        {
            // 플레이어 캐릭터가 죽지 않은 상태일 경우 위로 띄워지고 넉백 코루틴 실행
            controller.VelocityY = 24f;
            if(_knockedBackCoroutine != null)
            {
                // 이미 넉백 중인 상태이면 기존 넉백 중단
                StopCoroutine(_knockedBackCoroutine);
                _knockedBackCoroutine = null;
            }
            _knockedBackCoroutine = StartCoroutine(KnockedBackCoroutine());
        }
    }

    /// <summary>
    /// 플레이어 캐릭터가 넉백 됐을 때 애니메이션 및 넉백 종료 기능을 처리하기 위한 코루틴입니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator KnockedBackCoroutine()
    {
        // 넉백 애니메이션 실행
        animator.SetTrigger(GetAnimationHash("KnockBack"));

        // 미끄러짐이 멈출때까지 대기
        while(controller.IsSliding)
        {
            yield return null;
        }

        // 이동 가능한 상태로 설정하고 넉백 상태를 중단
        _canMove = true;
        isBeingKnockedBack = false;

        // 넉백 애니메이션 종료
        animator.SetTrigger(GetAnimationHash("KnockBackEnd"));

        _knockedBackCoroutine = null;
    }

    /// <summary>
    /// 플레이어 캐릭터가 사망했을 경우 실행되는 콜백 코루틴입니다.
    /// </summary>
    IEnumerator OnDied()
    {
        // 죽은 상태로 설정
        isDead = true;
        // 게임 매니저에서 플레이어가 죽었다는 메시지 전달
        GameManager.instance.HandlePlayerDeath();
        // 플레이어 캐릭터 사망 애니메이션 실행
        animator.SetTrigger(GetAnimationHash("Die"));
        // 불릿 타임을 1초 동안 실행
        ScreenEffect.instance.BulletTimeStart(0.3f, 1.0f);
        // 2.5초를 대기한 후 체력과 원동력을 초기화하고 부활 장소로 이동
        yield return YieldInstructionCache.WaitForSecondsRealtime(2.5f);
        GameManager.instance.playerCurrentHealth = _damage.maxHealth;
        GameManager.instance.playerCurrentDrivingForce = 0;
        SceneTransition.instance.LoadScene(GameManager.instance.resurrectionScene);
    }

    #endregion

    #region Animator

    /// <summary>
    /// 플레이어 캐릭터의 애니메이션을 업데이트하는 메소드입니다.
    /// </summary>
    void AnimationUpdate()
    {
        animator.SetFloat(GetAnimationHash("MoveX"), _moveX);
        animator.SetFloat(GetAnimationHash("FallSpeed"), controller.VelocityY);

        animator.SetBool(GetAnimationHash("IsGrounded"), controller.IsGrounded);
        animator.SetBool(GetAnimationHash("IsWallSliding"), _isWallSliding);
    }

    #endregion
}