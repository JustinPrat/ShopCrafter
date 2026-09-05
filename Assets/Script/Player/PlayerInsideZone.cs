using Alchemy.Inspector;
using UnityEngine;

public class PlayerInsideZone : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [ReadOnly, SerializeField]
    private bool isInside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBrain>())
        {
            isInside = true;
            managerRefs.GameEventsManager.playerEvents.PlayerZoneTrain(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerBrain>())
        {
            isInside = false;
            managerRefs.GameEventsManager.playerEvents.PlayerZoneTrain(false);
        }
    }
}
