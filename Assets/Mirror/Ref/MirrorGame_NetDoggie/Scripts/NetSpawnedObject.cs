using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class NetSpawnedObject : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent NavAgent_Player;
    public Animator Animator_Player;
    public TextMesh TextMesh_HealthBar;
    public Transform Transform_Player;
    public GameObject GObj_RpcCheck;

    [Header("Movement")]
    public float _rotationSpeed = 100.0f;

    [Header("Attack")]
    public KeyCode _atkKey = KeyCode.Space;
    public GameObject Prefab_AtkObject;
    public Transform Tranfrom_AtkSpawnPos;

    [Header("Stats Server")]
    [SyncVar] public int _health = 4;

    private void Awake()
    {
        GObj_RpcCheck.SetActive(false);
    }

    private void Update()
    {
        SetHealthBarOnUpdate(_health);
        TestOnUpdateAll();

        if (CheckIsFocusedOnUpdate() == false)
        {
            return;
        }

        CheckIsLocalPlayerOnUpdate();
        OnlyServerOnUpdate();
    }

    private void SetHealthBarOnUpdate(int health)
    {
        TextMesh_HealthBar.text = new string('-', health);
    }

    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isFocused;
    }

    private void CheckIsLocalPlayerOnUpdate()
    {
        if(isLocalPlayer == false)
            return;

        // 회전
        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontal * _rotationSpeed * Time.deltaTime, 0);

        // 이동
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        NavAgent_Player.velocity = forward * Mathf.Max(vertical, 0) * NavAgent_Player.speed;
        Animator_Player.SetBool("Moving", NavAgent_Player.velocity != Vector3.zero);

        // 공격
        if (Input.GetKeyDown(_atkKey))
        {
            CommandAtk();
        }
        
        RotateLocalPlayer();
    }

    void TestOnUpdateAll()
    {
        // 아무 클라에서나 눌러도 netId에만 영향을 주는 케이스 - 아무 클라에서 누르면 2번 클라만 반응

        if (this.netId == 1)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                CommandFromServer();
            }
        }
    }

    [Command(requiresAuthority = false)]
    void CommandFromServer()
    {
        OnlyFromCommand();
    }

    [ClientRpc]
    void OnlyFromCommand()
    {
        StartCoroutine(CoStartRpcEffect());
    }


    [Server]
    void OnlyServerOnUpdate()
    {
        // 서버에서만 브로드 캐스팅 하는 경우 - 모든 클라 불림
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnlyFromServer();
        }
    }

    [ClientRpc]
    void OnlyFromServer()
    {
        StartCoroutine(CoStartRpcEffect());
    }

    // 서버 사이드
    [Command]
    void CommandAtk()
    {
        GameObject atkObjectForSpawn = Instantiate(Prefab_AtkObject, Tranfrom_AtkSpawnPos.transform.position, Tranfrom_AtkSpawnPos.transform.rotation);
        NetworkServer.Spawn(atkObjectForSpawn);

        RpcOnAtk();
    }

    [ClientRpc]
    void RpcOnAtk()
    {
        Animator_Player.SetTrigger("Atk");
        StartCoroutine(CoStartRpcEffect());
    }

    IEnumerator CoStartRpcEffect()
    {
        GObj_RpcCheck.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        GObj_RpcCheck.SetActive(false);
    }

    // 클라에서 다음 함수가 실행되지 않도록 ServerCallback을 달아줌
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var atkGenObject = other.GetComponent<NetSpawnedSubObject>();
        if (atkGenObject != null)
        {
            _health--;

            if (_health == 0)
            {
                NetworkServer.Destroy(this.gameObject);
            }
        }
    }

    void RotateLocalPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.DrawLine(ray.origin, hit.point);
            Vector3 lookRotate = new Vector3(hit.point.x, Transform_Player.position.y, hit.point.z);
            Transform_Player.LookAt(lookRotate);
        }
    }
}
