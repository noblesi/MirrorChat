using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetSpawnedSubObject : NetworkBehaviour
{
    public float _destroyAfter = 2.0f;
    public float _force = 1000;

    public Rigidbody RigidBody_SubObj;

    public override void OnStartServer()
    {
    }

    private void Start()
    {
    }

    [Server]
    private void DestroySelf()
    {
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
    }
}
