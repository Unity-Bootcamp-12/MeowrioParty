using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    private Transform _cameraTransform;

    void Start()
    {
        // ���� ī�޶� ĳ��
        _cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // ī�޶� ���ϵ��� ȸ��
        transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward,
                         _cameraTransform.rotation * Vector3.up);
    }
}
