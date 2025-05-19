using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Start()
    {
        // 메인 카메라 캐싱
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // 카메라를 향하도록 회전
        transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward,
                         _cameraTransform.rotation * Vector3.up);
    }
}
