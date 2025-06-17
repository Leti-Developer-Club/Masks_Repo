using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerControls;
    [SerializeField] private ShootingAbility shootAbility;

    private InputAction movementAction;
    private InputAction shootAction;
    private InputAction meleeAction;

    public Vector2 MovementInput { get; private set; }

    #region Enable Input Action Map
    private void OnEnable()
    {
        playerControls.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        playerControls.FindActionMap("Player").Disable();
    }
    #endregion

    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap("Player");

        meleeAction = mapReference.FindAction("Melee Attack");
        movementAction = mapReference.FindAction("Move");
        shootAction = mapReference.FindAction("Fire");

        Subscribe_ActionValues_ToInputEvents();
    }

    private void Subscribe_ActionValues_ToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        meleeAction.started += ctx => MeleeAttack();

        shootAction.started += ctx => CanShoot_True();
        shootAction.canceled += ctx => CanShoot_False();
    }

    private void CanShoot_True()
    {
        shootAbility.canShoot = true;
    }
    private void CanShoot_False()
    {
        shootAbility.canShoot = false;
    }

    private void MeleeAttack()
    {
        var meleeAttack = GetComponent<AutoLockOnShooter>();
        meleeAttack.Melee_Attack();
    }
}
