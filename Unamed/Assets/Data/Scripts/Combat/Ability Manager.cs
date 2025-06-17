using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private IAbility currentAbility;

    public void SetAbility(IAbility newAbility)
    {
        currentAbility = newAbility;
    }

    public void UseCurrentAbility()
    {
        currentAbility?.Use();
    }
}

