using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonControlar : MonoBehaviour
{
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
    }
}
