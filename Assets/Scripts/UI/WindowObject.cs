using UnityEngine;

public class WindowObject : MonoBehaviour
{
    [SerializeField] private GameObject _BlockingLayer;

    private void OnEnable()
    {
        _BlockingLayer.SetActive(true);
    }
    private void OnDisable()
    {
        _BlockingLayer.SetActive(false);
    }
}
