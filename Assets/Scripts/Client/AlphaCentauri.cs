using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaCentauri : MonoBehaviour
{

    private Rigidbody station;
    private Transform loading;
    // Start is called before the first frame update
    void Start()
    {
        station = GameObject.Find("Station").GetComponent<Rigidbody>();
        ClientListener.SendDataTCP($"maploaded?map={gameObject.name}&status=Roam");
        loading = GameObject.Find("Loading").transform;
    }

    void FixedUpdate()
    {
        station.angularVelocity = new Vector3(0, 0.1f, 0);
    }

    void UpdateShipPosition(Message[] parameters)
    {
        string id = Message.StringValueOfKey(parameters, "id");
        Vector3 pos = Message.Vector3ValueOfKey(parameters, "pos");
        Vector3 rot = Message.Vector3ValueOfKey(parameters, "rot");
        Vector3 vel = Message.Vector3ValueOfKey(parameters, "vel");
        Vector3 aVel = Message.Vector3ValueOfKey(parameters, "avel");

        Transform ship = GameObject.Find(id).transform;
        ship.position = pos;
        ship.rotation = Quaternion.Euler(rot);
        Rigidbody rigid = ship.GetComponent<Rigidbody>();
        rigid.velocity = vel;
        rigid.angularVelocity = aVel;
    }

    void SpawnNonPlayer(Message[] parameters)
    {
        string id = Message.StringValueOfKey(parameters, "id");
        string mesh = Message.StringValueOfKey(parameters, "mesh");
        Debug.Log(Message.StringValueOfKey(parameters, "pos"));
        Vector3 pos = Message.Vector3ValueOfKey(parameters, "pos");
        Vector3 rot = Message.Vector3ValueOfKey(parameters, "rot");
        Vector3 vel = Message.Vector3ValueOfKey(parameters, "vel");
        Vector3 aVel = Message.Vector3ValueOfKey(parameters, "avel");

        GameObject ship = Instantiate(Resources.Load($"NonPlayerShips/{mesh}"), pos, Quaternion.Euler(rot)) as GameObject;
        ship.name = id;
        Rigidbody shipRigid = ship.GetComponent<Rigidbody>();
        shipRigid.velocity = vel;
        shipRigid.angularVelocity = aVel;
    }

    void ReadyToSpawn()
    {
        Vector3 spawnPoint = station.position + Random.onUnitSphere * 250;
        while (Physics.OverlapSphere(spawnPoint, 25).Length > 0)
            ReadyToSpawn();

        GameObject ship = Instantiate(Resources.Load($"PlayerShips/{GameData.myShip.Mesh}"), spawnPoint, Random.rotation) as GameObject;
        ship.name = GameData.myShip.UserId;
        loading.localScale = Vector3.zero;
        Camera.main.GetComponent<PlayerCamera>().target = ship.transform;
        StartCoroutine(EnableShip(Camera.main.transform, ship.transform));
    }

    IEnumerator EnableShip(Transform cam, Transform ship)
    {
        PlayerCamera camScript = Camera.main.GetComponent<PlayerCamera>();
        while(Vector3.Distance(cam.position, ship.position) > camScript.distance + 1)
            yield return null;

        Rigidbody shipRigid = ship.GetComponent<Rigidbody>();
        ship.GetComponent<PlayerShipControls>().shipEnabled = true;
        ClientListener.SendDataTCP($"shipspawned?id={ship.name}" +
            $"&pos={Message.ParseVector3(ship.position)}" +
            $"&rotr={Message.ParseVector3(ship.eulerAngles)}" +
            $"&vel={Message.ParseVector3(shipRigid.velocity)}" +
            $"&avel={Message.ParseVector3(shipRigid.angularVelocity)}");
    }
}
