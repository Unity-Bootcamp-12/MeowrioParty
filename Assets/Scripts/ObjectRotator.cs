using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Start()
    {
        // ���� ī�޶� ĳ��
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // ī�޶� ���ϵ��� ȸ��
        transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward,
                         _cameraTransform.rotation * Vector3.up);
    }
}
