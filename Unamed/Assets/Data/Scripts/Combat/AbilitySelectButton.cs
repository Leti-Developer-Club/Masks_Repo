using UnityEngine;

public class AbilitySelectButton : MonoBehaviour
{
    [SerializeField] private MonoBehaviour abilityComponent;
    private IAbility ability;
    [SerializeField] private AbilityManager abilityManager;

    private void Awake()
    {
        ability = abilityComponent as IAbility;
    }

    public void OnClick_SelectAbility()
    {
        abilityManager.SetAbility(ability);
    }
}

