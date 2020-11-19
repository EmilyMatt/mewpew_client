using System.Threading.Tasks;
using UnityEngine;

public class HUDElement : MonoBehaviour
{
    public bool button;
    public bool input;
    public bool password;
    public Sprite[] inputTextures;
    public string passString;
    public bool focus;
    public bool active;

    private TextMesh text;

    private void Start()
    {
        text = GetComponentInChildren<TextMesh>();
    }

    private void Update()
    {
        if (input && focus && Input.anyKey)
            WriteGlobal();
    }

    private void WriteGlobal()
    {
        if (Input.inputString.Length == 0)
            return;

        char inputChar = Input.inputString.ToCharArray()[0];
        if (inputChar == '\b')
        {
            if (text.text.Length > 0)
            {
                text.text = text.text.Remove(text.text.Length - 1, 1);
                if (password)
                    passString = passString.Remove(passString.Length - 1, 1);
            }
                
            return;
        }

        if (inputChar > 127 || inputChar == '\r')
            return;

        if (password)
            WritePass(inputChar);
        else
            WriteText(inputChar);

    }

    private void WritePass(char inputChar)
    {
        if (passString.Length == 15)
            return;

        text.text += '*';
        passString += inputChar;
    }

    private void WriteText(char inputChar)
    {
        if (text.text.Length == 35)
            return;

        text.text += inputChar;
    }

    private void OnMouseEnter()
    {
        if(button && active)
            transform.Find("Shadow").GetComponent<SpriteRenderer>().enabled = true;
    }
    
    private void OnMouseExit()
    {
        if(button && active)
            transform.Find("Shadow").GetComponent<SpriteRenderer>().enabled = false;
    }

    private async void OnMouseDown()
    {
        if(button)
        {
            if (!active)
                return;

            active = false;
            GetComponent<SpriteRenderer>().color = new Color(0, 255, 255);
            await Task.Run(() => Task.Delay(25));
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            await Task.Run(() => Task.Delay(25));
            GetComponent<SpriteRenderer>().color = new Color(128, 128, 128);
        }
        else if(input)
        {
            SetInputTexture(1);
        }

        SendMessageUpwards("HUDEvent", new string[2] { transform.name, "mouseDown" });
    }

    public void SetInputTexture(int idx)
    {
        GetComponent<SpriteRenderer>().sprite = inputTextures[idx];
        if (idx == 0)
            focus = false;
        else
            focus = true;
    }
}
