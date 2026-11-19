using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PickBox : BaseWorldInteract
{
    [SerializeField]
    private List<GameObject> spawnObjectList;

    [SerializeField]
    private CinemachineImpulseSource cinemachineImpulseSource;

    public override void StartInteract(PlayerBrain playerBrain)
    {
        cinemachineImpulseSource.GenerateImpulse(0.2f);
        base.StartInteract(playerBrain);
        Destroy(gameObject);
    }
}
