using UnityEngine;

public class TCPRouter
{
    public static void Route(string msg)
    {
        string[] msgParts = msg.Split('?');

        Message[] parameters = new Message[0];
        if (msgParts.Length > 1 && msgParts[1] != "")
            parameters = Message.Parse(msgParts[1]);

        string route = msgParts[0];
        GameObject tmpGameObject;
        string tmpString;

        switch (route)
        {
            case "loginsuccess":
                if (GameData.location != "Login")
                    return;

                tmpGameObject = GameObject.Find("Login");
                if(tmpGameObject)
                    tmpGameObject.GetComponent<Login>().LoginSuccess(parameters);
                return;
            case "loginfailed":
                if (GameData.location != "Login")
                    return;

                tmpGameObject = GameObject.Find("Login");
                if (tmpGameObject)
                    tmpGameObject.GetComponent<Login>().LoginFailed(parameters);
                return;
            case "sendmyshipdata":
                GameObject.Find("Hangar").GetComponent<Hangar>().RecieveShipData(parameters);
                return;
            case "launch":
                tmpString = Message.StringValueOfKey(parameters, "map");
                MonoBehaviour script = (MonoBehaviour)GameObject.Find(GameData.location).GetComponent(GameData.location);
                script.StartCoroutine("ChangeMap", parameters);
                return;
            case "readytospawn":
                tmpString = Message.StringValueOfKey(parameters, "map");
                GameObject.Find(tmpString).SendMessage("ReadyToSpawn");
                return;
            case "spawnnonplayer":
                GameObject.Find(GameData.location).SendMessage("SpawnNonPlayer", parameters);
                return;
            case "poll":
                ClientListener.SendDataTCP("returnpoll?");
                return;
            default:
                return;
        }
    }
}
