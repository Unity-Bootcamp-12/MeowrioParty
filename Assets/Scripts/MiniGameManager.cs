using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MiniGameManager : NetworkBehaviour
{
    public InputManagerSO inputManager;
    private PlayerController controller;


    private float _finishLineX;
    private float _baseSpeed;
    private float _maxSpeed;
    private float _acceleration;
    private float _deceleration;

    [SerializeField] private float _currentSpeed;

    [SerializeField] private Transform _miniGameStartPos;
    private Vector3 _originalPos;


    public bool isMiniFinished = false;

    private Animator _animator;
    private float _defaultAnimSpeed;

    private bool isReady = false;

    private void Awake()
    {
        _finishLineX = 30.0f;
        _baseSpeed = 1.0f;
        _maxSpeed = 10.0f;
        _acceleration = 3.0f;
        _deceleration = 5.0f;
        _currentSpeed = 0.0f;

    }
    public override void OnNetworkSpawn()
    {
        Debug.Log(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject());
        NetworkObject playerObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        controller = playerObj.GetComponent<PlayerController>();
        _animator = controller.gameObject.GetComponent<Animator>();
        _defaultAnimSpeed = _animator.speed;
        _animator.SetBool("isMoving", true);

        _originalPos = controller.transform.position;
        if (IsServer)
        {
            controller.transform.position = _miniGameStartPos.position;
            controller.transform.rotation = _miniGameStartPos.rotation;           
        }

        isReady = true;
        inputManager.OnConfirmButtonPerformed += GetInput;
        //StartCoroutine(WaitAndInitialize());

    }

    
    private IEnumerator WaitAndInitialize()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject() != null);
        controller = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
            ?.GetComponent<PlayerController>();

        _animator = controller.gameObject.GetComponent<Animator>();
        _defaultAnimSpeed = _animator.speed;
        _animator.SetBool("isMoving", true);

        _originalPos = controller.transform.position;
        controller.transform.position = _miniGameStartPos.position;
        controller.transform.rotation = _miniGameStartPos.rotation;
        isReady = true;
        inputManager.OnConfirmButtonPerformed += GetInput;
    }

    private void Update()
    {
        if (!isReady || isMiniFinished) return;
        Deceleration();
        if(IsServer)
            Move();
    }

    private void Accelerate()
    {

        _currentSpeed += _acceleration;
        _currentSpeed = Mathf.Clamp(_currentSpeed, _baseSpeed, _maxSpeed);

    }

    private void Deceleration()
    {
        _currentSpeed -= _deceleration * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, _baseSpeed, _maxSpeed);
    }

    private void Move()
    {
        // 플레이어의 보여지는 부분
        controller.transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime);
        UpdateAnimatorSpeed();
    }

    private void UpdateAnimatorSpeed()
    {
        float speedRatio = _currentSpeed / _baseSpeed;
        _animator.speed = Mathf.Clamp(speedRatio, 0.5f, 2.0f);
    }

    // private void CheckFinish()
    // {
    //     if (transform.position.x >= _finishLineX)
    //     {
    //         isMiniFinished = true;
    //         _currentSpeed = 0.0f;
    //         Debug.Log(gameObject.name + "결승선 도착");
    //         // 애니메이션 연출
    //         _animator.SetBool("isMoving", false);
    //         _animator.speed = _defaultAnimSpeed;
    //         // 미니게임 매니저로 게임 끝났다는걸 알림
    //         ReturnToOriginalPos();
    //     }
    // }

    // private void ReturnToOriginalPos()
    // {
    //     transform.position = _originalPos;
    //     transform.rotation = Quaternion.identity;
    //     Debug.Log("원래 위치 복귀");
    // }
    
    private void GetInput(object sender, bool isPressed)
    {
        if (!isPressed || !IsOwner) return;

        Accelerate();
    }
}
