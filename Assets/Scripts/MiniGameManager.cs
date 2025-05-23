using System;
using UnityEngine;

// TODO: ���� �Ŵ������� ��� ���۰� ���� ó���϶�� �������� �̴� ���� ���¸� �����ϰ� �ִ� �̴� ���� �Ŵ����� �ʿ��Ұ� ����
// �̴� ���� �Ŵ����� ���� �Ŵ����� �ϴ��� ó�� ready���¿� end ������ �� �Է��� ���� ������ ������� ��

public class MiniGameManager : MonoBehaviour
{
    private float _finishLineX;
    private float _baseSpeed;
    private float _maxSpeed;
    private float _acceleration;
    private float _deceleration;

    [SerializeField] private float _currentSpeed;

    [SerializeField] private Transform _miniGameStartPos;
    private Vector3 _originalPos;

    [SerializeField] private KeyCode _inputKey = KeyCode.A;

    public bool isMiniFinished = false;

    private Animator _animator;
    private float _defaultAnimSpeed;

    private void Awake()
    {
        _finishLineX = 30.0f;
        _baseSpeed = 1.0f;
        _maxSpeed = 10.0f;
        _acceleration = 3.0f;
        _deceleration = 5.0f;
        _currentSpeed = 0.0f;

        _animator = GetComponent<Animator>();
        _defaultAnimSpeed = _animator.speed;

        _originalPos = transform.position;

        if (_miniGameStartPos != null )
        {
            transform.position = _miniGameStartPos.position;
            transform.rotation = _miniGameStartPos.rotation;
        }        
    }
    private void Start()
    {
        _animator.SetBool("isMoving", true);
    }

    private void Update()
    {
        if (isMiniFinished)
        {
            return;
        }
        PressedButton();
        Deceleration();
        Move();
        CheckFinish();
    } 

    private void PressedButton()
    {        
        if (Input.GetKeyDown(_inputKey))
        {
            Accelerate();
        }
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
        // �÷��̾��� �������� �κ�
        transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime);
        UpdateAnimatorSpeed();
    }

    private void UpdateAnimatorSpeed()
    {
        float speedRatio = _currentSpeed / _baseSpeed;
        _animator.speed = Mathf.Clamp(speedRatio, 0.5f, 2.0f);
    }

    private void CheckFinish()
    {
        if (transform.position.x >= _finishLineX)
        {
            isMiniFinished = true;
            _currentSpeed = 0.0f;
            Debug.Log(gameObject.name + "��¼� ����");
            // �ִϸ��̼� ����
            _animator.SetBool("isMoving", false);
            _animator.speed = _defaultAnimSpeed;
            // �̴ϰ��� �Ŵ����� ���� �����ٴ°� �˸�
            ReturnToOriginalPos();
        }
    }

    private void ReturnToOriginalPos()
    {
        transform.position = _originalPos;
        transform.rotation = Quaternion.identity;
        Debug.Log("���� ��ġ ����");
    }
}
