using UnityEngine;

public class WindowObject : MonoBehaviour
{
    [SerializeField] private BlockingLayer _BlockingLayer;

    private void OnEnable()
    {
        _BlockingLayer.SetActive(true, gameObject);
    }
    private void OnDisable()
    {
        _BlockingLayer.SetActive(false, gameObject);
    }
}
