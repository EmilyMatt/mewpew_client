using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    private void Start()
    {
        GameData.location = "Login";

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && transform.Find("Button_Login").GetComponent<HUDElement>().active)
            LoginFunc();

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            HUDElement element = transform.Find("InputMail").GetComponent<HUDElement>();
            if(element.active)
                SetFocus("InputPassword");
        }
    }

    string MD5Generator(string str)
    {
        Byte[] valueBytes = Encoding.UTF8.GetBytes(str);
        byte[] hashBytes = MD5.Create().ComputeHash(valueBytes);
        return String.Join("", hashBytes.Select(c => c.ToString("x2")));
    }

    void HUDEvent(string[] info)
    {
        Transform eventTransform = GameObject.Find(info[0]).transform;
        string myEvent = info[1];

        if(myEvent == "mouseDown")
        {
            SetFocus(eventTransform.name);

            if(eventTransform.name == "Button_Login")
                LoginFunc();
        }
    }

    void SetFocus(string name)
    {
        HUDElement[] inputs = GetComponentsInChildren<HUDElement>();
        foreach (HUDElement element in inputs)
        {
            if (!element.input)
                continue;

            if (element.name == name)
            {
                element.SetInputTexture(1);
                continue;
            }       

            element.SetInputTexture(0);
        }
    }

    public void LoginFailed(Message[] parameters)
    {
        string err = Message.StringValueOfKey(parameters, "err");
        TextMesh errorText = GameObject.Find("Error").GetComponent<TextMesh>();
        if(err == "data")
            errorText.text = "Data send corrupted";
        else if(err == "auth")
            errorText.text = "Invalid username or password";
        else if(err == "active")
            errorText.text = "You are already signed in";
    }
    public async void LoginSuccess(Message[] parameters)
    {
        GameObject loading = GameObject.Find("Loading");
        loading.transform.localScale = Vector3.one;
        AsyncOperation loadScene = SceneManager.LoadSceneAsync("Hangar", LoadSceneMode.Additive);
        while (!loadScene.isDone)
            await Task.Delay(1);

        Hangar hangar = GameObject.Find("Hangar").GetComponent<Hangar>();
        hangar.InitHangar(Message.BoolValueOfKey(parameters, "firsttime"));

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Hangar"));

        AsyncOperation unloadScene = SceneManager.UnloadSceneAsync("Login");
        while (!unloadScene.isDone)
            await Task.Delay(1);
    }

    void LoginFunc()
    {
        TextMesh errorText = GameObject.Find("Error").GetComponent<TextMesh>();
        errorText.text = "";
        string mailString = transform.Find("InputMail/Text").GetComponent<TextMesh>().text;
        string passString = transform.Find("InputPassword").GetComponent<HUDElement>().passString;

        if (!mailString.Contains("@") || !mailString.Contains("."))
        {
            errorText.text = "Invalid Mail Address";
            return;
        }
         
        if(passString.Length < 4)
        {
            errorText.text = "Password Too Short";
            return;
        }

        passString = MD5Generator(passString);

        ClientListener.SendDataTCP($"login?mail={mailString}&pass={passString}");
    }
}
