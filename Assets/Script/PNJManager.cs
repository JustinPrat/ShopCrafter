using System.Collections.Generic;
using UnityEngine;

public class PNJManager : MonoBehaviour
{
    [SerializeField]
    private ManagerRefs managerRefs;

    [SerializeField]
    private GameObject PnjPrefab;

    [SerializeField]
    private int targetNumberPnj;

    private void Awake()
    {
        managerRefs.PNJManager = this;
    }
}
