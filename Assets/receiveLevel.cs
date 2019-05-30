using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.UI;


public class receiveLevel : MonoBehaviour
{
    public Slider slider;

    bool running = false;

    /*
    async void Update()
    {

        if (!running) {
            running = true;
            try
            {
                using (ClientWebSocket ws = new ClientWebSocket())
                {
                    Debug.Log("created socket");
                    Uri serverUri = new Uri("ws://localhost:3000/");
                    await ws.ConnectAsync(serverUri, CancellationToken.None);
                    Debug.Log("Connected?");
                    while (ws.State == WebSocketState.Open)
                    {
                        /*
                        Console.Write("Input message ('exit' to exit): ");
                        string msg = Console.ReadLine();
                        if (msg == "exit")
                        {
                            break;
                        }
                        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                        await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                        */ /*
                        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                        WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                        String value = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
                        //Debug.Log(value); 
                        slider.value = float.Parse(value);
                    }
                }
            } catch (WebSocketException exp)
            {
                Debug.Log("Caught exception: " + exp.Message);
                running = false;
            }
        }
    }
    */
}
