using UnityEngine;

public class ManagersController : MonoBehaviour
{
    [SerializeField] private ManagerRefs managerRefs;

    private IInitializableManager[] managers;

    private void Awake()
    {
        managers = GetComponentsInChildren<IInitializableManager>();

        foreach (IInitializableManager manager in managers)
            manager.Register(managerRefs);

        foreach (IInitializableManager manager in managers)
            manager.Init(managerRefs);
    }
}