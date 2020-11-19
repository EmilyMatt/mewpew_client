using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hangar : MonoBehaviour
{
    public Mesh[] shipMeshes;
    public int ship;

    private Transform loading;

    private bool shipDataRecieved = false;
    public void RecieveShipData(Message[] parameters)
    {
        GameData.myShip = JsonConvert.DeserializeObject<PlayerShip>(Message.StringValueOfKey(parameters, "data"));
        shipDataRecieved = true;
    }

    public void InitHangar(bool firstTime)
    {
        loading = GameObject.Find("Loading").transform;
        GameData.location = "Hangar";
        transform.Find("General").gameObject.SetActive(false);
        transform.Find("ShipSelect").gameObject.SetActive(false);
        if (firstTime)
        {
            transform.Find("ShipSelect").gameObject.SetActive(true);
            loading.localScale = Vector3.zero;
        } else
        {
            ClientListener.SendDataTCP("reqmyshipdata?");
            StartCoroutine(AwaitShipData());
        }
    }

    IEnumerator AwaitShipData()
    {
        int timeout = 100;
        while (timeout > 0)
        {
            if(shipDataRecieved)
            {
                transform.Find("General").gameObject.SetActive(true);
                transform.Find("ShipSelect").gameObject.SetActive(false);
                loading.localScale = Vector3.zero;

                int meshIdx = 0;
                GameObject.Find("Ship").GetComponent<MeshFilter>().mesh = shipMeshes[meshIdx];
                yield break;
            }

            timeout--;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void HUDEvent(string[] info)
    {
        Transform element = GameObject.Find(info[0]).transform;
        string myEvent = info[1];

        if(myEvent == "mouseDown")
        {
            if (info[0] == "Button_Launch")
                ClientListener.SendDataTCP("launch?");

            if (info[0] == "Button_Select")
                SelectShip();
        }
    }

    void SelectShip()
    {
        loading.transform.localScale = Vector3.one;
        ClientListener.SendDataTCP($"selectship?idx={ship}");
        ClientListener.SendDataTCP("reqmyshipdata?");
        StartCoroutine(AwaitShipData());
    }

    IEnumerator ChangeMap(Message[] parameters)
    {
        loading.transform.localScale = Vector3.one;
        string mapName = Message.StringValueOfKey(parameters, "map");
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        while (!loadScene.isDone)
            yield return null;

        GameData.location = mapName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mapName));
    }
}
