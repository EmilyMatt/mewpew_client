using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ClientListener : MonoBehaviour
{
    public static bool listening;

    private static NetworkStream stream;
    private static TcpClient client;
    private static UdpClient udp;
    private static IPAddress ip;
    private string lastMessage = "";

    //this section happens on connection
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        ip = IPAddress.Parse("3.139.68.151");

        listening = true;
        client = new TcpClient();
        udp = new UdpClient();

        InitConnect();
    }

    void InitConnect()
    {
        try
        {

            if(!client.ConnectAsync(ip, 2500).Wait(2500))
                RetryConnection();

            ConnectToServer();
        }
        catch(Exception e)
        {

            Debug.Log($"Unable To Connect: {e}\n retrying...");
            StartCoroutine(RetryConnection());
            return;
        }
    }

    public void ConnectToServer()
    {
        Debug.Log("Connected to server");
        try
        {
            stream = client.GetStream();
            udp.Connect(ip, 2701);
        }
        catch (Exception e)
        {
            Debug.Log($"Error while connecting: {e}");
            StartCoroutine(RetryConnection());
        }
        StartCoroutine(ListenTCP());
        StartCoroutine(ListenUDP());
    }

    public IEnumerator RetryConnection()
    {
        yield return new WaitForSeconds(5);
        client.Dispose();
        client = new TcpClient();
        InitConnect();
    }

    IEnumerator ListenTCP()
    {
        //as long as socket is active, check for data and handle it, and send data when needed
        while (listening)
        {
            if(stream  == null)
            {
                StartCoroutine(RetryConnection());
                yield break;
            }   
            
            if (stream.DataAvailable || lastMessage != "")
                GetDataTCP();

            yield return null;
        }
    }

    //method that returns the message from client
    public async void GetDataTCP()
    {
        string message = lastMessage;
        while (true)
        {
            if (message.Contains(";") && message != "")
            {
                int charidx = message.IndexOf(';');
                lastMessage = message.Substring(charidx + 1);
                message = message.Substring(0, charidx);
                Debug.Log($"Data Recieved: {message} via TCP");
                TCPRouter.Route(message);
                break;
            }
            Byte[] bytes = new Byte[512];
            try
            {
                int idx = await stream.ReadAsync(bytes, 0, bytes.Length);
                string msg = Encoding.UTF8.GetString(bytes, 0, idx);
                message += msg;
            }
            catch (Exception e)
            {
                Debug.Log($"Error on TCP: {e}");
            }

            if (message == "")
                return;

            await Task.Delay(1);
        }
    }

    //method that sends data to client
    public static async void SendDataTCP(string msg)
    {
        Byte[] bytes = new Byte[256];
        bytes = Encoding.UTF8.GetBytes(msg + ";");

        try
        {
            if (msg == string.Empty)
                return;

            await stream.WriteAsync(bytes, 0, bytes.Length);
            Debug.Log($"Data Sent: {msg} via TCP");
        }
        catch (Exception e)
        {
            listening = false;
            Debug.Log($"Error on TCP: {e}");
        }

    }

    IEnumerator ListenUDP()
    {
        //as long as socket is active, check for data and handle it, and send data when needed
        while (listening)
        {

            if (udp.Available > 0)
                GetDataUDP();

            yield return null;
        }
    }

    async void GetDataUDP()
    {
        try
        {
            UdpReceiveResult res = await udp.ReceiveAsync();
            string message = Encoding.UTF8.GetString(res.Buffer);
            Debug.Log($"Data Recieved: {message} via UDP");
            UDPRouter.Route(message);
        }
        catch (Exception e)
        {
            Debug.Log($"Error on UDP: {e}");
            listening = false;
        }
    }

    public static async void SendDataUDP(string msg)
    {
        Byte[] buffer = UTF8Encoding.UTF8.GetBytes(msg);

        try
        {
            if (msg == string.Empty)
                return;

            await udp.SendAsync(buffer, buffer.Length);
            Debug.Log($"Data Sent: {msg} via UDP");
        }
        catch(Exception e)
        {
            Debug.Log($"Error on UDP: {e}");
        }

    }
}