using UnityEngine;

public class UDPRouter
{
    public static void Route(string msg)
    {
        string[] msgParts = msg.Split('?');

        Message[] parameters = new Message[0];
        if (msgParts.Length > 1 && msgParts[1] != "")
            parameters = Message.Parse(msgParts[1]);

        string route = msgParts[0];
        GameObject tmpGameObject;
        switch (route)
        {
            case "usp":
                tmpGameObject = GameObject.Find(GameData.location);
                if(tmpGameObject)
                    tmpGameObject.SendMessage("UpdateShipPosition", parameters);
                return;
            default:
                return;
        }
    }
}
