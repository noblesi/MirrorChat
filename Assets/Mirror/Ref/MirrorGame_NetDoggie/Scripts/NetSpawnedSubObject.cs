using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetSpawnedSubObject : NetworkBehaviour
{
    public float _destroyAfter = 2.0f;
    public float _force = 1000;

    public Rigidbody RigidBody_SubObj;

}
