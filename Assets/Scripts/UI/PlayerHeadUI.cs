using UnityEngine;

public class PlayerHeadUI : MonoBehaviour
{
    private Canvas _worldCanvas;

    private void Awake()
    {
        _worldCanvas = GetComponent<Canvas>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            _worldCanvas.transform.forward = Camera.main.transform.forward;
        }
    }

}
