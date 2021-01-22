using UnityEngine;

public class WindowObject : MonoBehaviour
{
    public enum UsingCheck
    { Using, UnUsing }

    [SerializeField] private UsingCheck _IntractableLayer;
    [SerializeField] private BlockingLayer _BlockingLayer;

    private void OnEnable()
    {
        switch (_IntractableLayer)
        {
            case UsingCheck.Using:
                _BlockingLayer.SetActive(true, gameObject);
                break;

            case UsingCheck.UnUsing:
                _BlockingLayer.gameObject.SetActive(true);
                break;
        }
    }
    private void OnDisable()
    {
        switch (_IntractableLayer)
        {
            case UsingCheck.Using:
                _BlockingLayer.SetActive(false, gameObject);
                break;

            case UsingCheck.UnUsing:
                _BlockingLayer.gameObject.SetActive(false);
                break;
        }
    }
}
