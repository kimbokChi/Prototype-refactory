using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonControlar : MonoBehaviour
{
    private readonly Vector2 TopPosition = new Vector2(0, 440);
    private readonly Vector2 BotPosition = new Vector2(0, 0);

    [SerializeField] private AttackButton _LAttackButton;
    [SerializeField] private AttackButton _RAttackButton;

    private Player _Player;

    private void Awake()
    {
        _Player = FindObjectOfType(typeof(Player)) as Player;

        _LAttackButton.IntractEvent += () => 
        {
            _Player.SetLookAtLeft(true);

            _Player.AttackOrder(); 
        };
        _RAttackButton.IntractEvent += () => 
        {
            _Player.SetLookAtLeft(false);

            _Player.AttackOrder(); 
        };

        //_Player.MovingEvent += OnMoving;
    }
    private void OnMoving(LPOSITION3 movePosition, float ratio)
    {
        switch (movePosition)
        {
            case LPOSITION3.TOP:
                {
                    transform.localPosition = Vector2.Lerp(transform.localPosition, TopPosition, ratio);
                }
                break;

            case LPOSITION3.MID:
            case LPOSITION3.BOT:
                {
                    transform.localPosition = Vector2.Lerp(transform.localPosition, BotPosition, ratio);
                }
                break;
        }
    }
}
