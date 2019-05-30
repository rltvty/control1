using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using UnityEngine;
using CodeMonkey.Utils;

public class StreamedEvent
{
    public string event_name;
    public string mac_address;
    public int level;
}

public class GameHandler : MonoBehaviour
{
    [SerializeField] public LevelMeter leftFrontMain;
    [SerializeField] public LevelMeter leftFrontSub;
    [SerializeField] public LevelMeter centerFront;
    [SerializeField] public LevelMeter rightFrontMain;
    [SerializeField] public LevelMeter rightFrontSub;
    [SerializeField] public LevelMeter leftRear;
    [SerializeField] public LevelMeter rightRear;

    /*
    // Start is called before the first frame update
    private void Start()
    {
        float level = 1f;
        float change = -0.01f;
        FunctionPeriodic.Create(() =>
        {

            level += change;
            if (level <= 0f)
            {
                level = 0f;
                change = change * -1;
            }
            if (level >= 1f)
            {
                level = 1f;
                change = change * -1;
            }
            levelMeter.SetLevel(level);
        }, .03f);
    }
    */


    bool running = false;

    async void Update()
    {
        if (!running)
        {
            running = true;
            try
            {
                using (ClientWebSocket ws = new ClientWebSocket())
                {
                    //Debug.Log("created socket");
                    Uri serverUri = new Uri("ws://localhost:3000/");
                    await ws.ConnectAsync(serverUri, CancellationToken.None);
                    //Debug.Log("Connected?");
                    while (ws.State == WebSocketState.Open)
                    {
                        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                        WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                        String value = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
                        try
                        {
                            StreamedEvent obj = JsonUtility.FromJson<StreamedEvent>(value);

                            if (obj.event_name == "input_level")
                            {
                                float level = obj.level / 32767f;
                                switch (obj.mac_address)
                                {
                                    case "00:0A:92:D6:66:BB": //SL328AI SPK
                                        leftFrontMain.SetLevel(level);
                                        break;
                                    case "00:0A:92:C8:0B:EF": //SL18sAI SPK
                                        leftFrontSub.SetLevel(level);
                                        break;
                                    case "00:0A:92:A9:19:0C": //SL18sAI SPK
                                        rightFrontSub.SetLevel(level);
                                        break;
                                    case "00:0A:92:D7:04:10": //SL328AI SPK
                                        rightFrontMain.SetLevel(level);
                                        break;
                                    case "00:0A:92:C8:33:09": //SL315AI SPK
                                        leftRear.SetLevel(level);
                                        break;
                                    case "00:0A:92:D6:66:EE": //SL328AI SPK
                                        centerFront.SetLevel(level);
                                        break;
                                    case "00:0A:92:C8:33:87": //SL315AI SPK
                                        rightRear.SetLevel(level);
                                        break;
                                    default:
                                        Debug.Log(obj.mac_address);
                                        break;
                                }
                            }
                        }
                        catch (ArgumentException exp)
                        {

                        }
                    }
                }
            }
            catch (WebSocketException exp)
            {
                Debug.Log("Caught exception: " + exp.Message);
                running = false;
            }
        }
    }
}
