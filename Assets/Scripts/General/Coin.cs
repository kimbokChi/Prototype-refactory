using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int Value;

    [Header("PopAnimation")]
    [SerializeField] private float _PopAnimationTime;
    [Range(0f, 1f)]
    [SerializeField] private float _PopRange;

    [Header("Animator")]
    [SerializeField] private Animator _Animator;
    
    private int _AnimationHash;
    private Coroutine _PopRoutine;

    private void OnEnable()
    {
        if (_PopRoutine == null)
        {
            _PopRoutine = new Coroutine(this);
        }
        _PopRoutine.StartRoutine(PopAnimation());
        _AnimationHash = _Animator.GetParameter(0).nameHash;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_Animator.GetBool(_AnimationHash))
        {
            _PopRoutine.StopRoutine();

            MoneyManager.Instance.AddMoney(Value);
            _Animator.SetBool(_AnimationHash, true);
        }
    }
    private void Disable()
    {
        gameObject.SetActive(false);

        transform.localScale = Vector3.one;
        _Animator.SetBool(_AnimationHash, false);
    }
    private IEnumerator PopAnimation()
    {
        Vector2 start = transform.position;

        Vector2 target = start + Vector2.right * Random.Range(-_PopRange, _PopRange);
                target.x.Range(-4, 4);

        for (float i = 0; i < _PopAnimationTime; i += Time.deltaTime * Time.timeScale)
        {
            transform.position = Vector2.Lerp(start, target, i / _PopAnimationTime);
            yield return null;
        }
        _PopRoutine.Finish();
    }
}
