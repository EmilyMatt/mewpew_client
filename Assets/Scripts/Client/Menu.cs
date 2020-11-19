using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private bool pressable = true;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape) && pressable)
        {
            pressable = false;
            if (transform.localScale == Vector3.zero)
                transform.localScale = new Vector3(145, 45, 1);
            else
                transform.localScale = Vector3.zero;

            StartCoroutine(ReloadEsc());
        }
    }

    IEnumerator ReloadEsc()
    {
        yield return new WaitForSeconds(0.1f);
        pressable = true;
    }

    void HUDEvent(string[] info)
    {
        Transform element = GameObject.Find(info[0]).transform;
        string myEvent = info[1];

        if (element.name == "Button_Quit" && myEvent == "mouseDown")
        {
            ClientListener.SendDataTCP("disconnect?");
            Application.Quit();
        }
    }

}
