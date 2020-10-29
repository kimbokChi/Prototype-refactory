using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Vector3 Offset;
    [SerializeField] private Image FillHealthBarImage;

    [Header("Master Info")]
    [SerializeField] private Transform MasterTransform;
    [SerializeField] private AbilityTable AbilityTable;

    private void LateUpdate()
    {
        transform.position = MasterTransform.position + Offset;

        FillHealthBarImage.fillAmount = 
            AbilityTable.Table[Ability.CurHealth] 
          / AbilityTable.Table[Ability.MaxHealth];
    }
}
