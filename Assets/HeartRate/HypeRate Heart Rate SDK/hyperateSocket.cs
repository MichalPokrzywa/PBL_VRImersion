using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json.Linq;
using NativeWebSocket;

using System.IO;

//MODIFIED: This script was initially created by Hyperate.io,
// and modified to fit the needs of the project, the original script can be found at asset store
//SOLUTION: Until the response from the service owner, this script wont work, as the websocket token is not provided
//Hyperate app must be installed on smart watch

public class hyperateSocket : MonoBehaviour
{
    [SerializeField]
    float heartRateRefreshRate = 25f;
    [SerializeField]
    string logFileName = "HeartRate/hr_log_file.csv";
    string logFilePath;
    
	// Put your websocket Token ID here
    // API Request was made to the service owner, no response as for 20.12.2024
    // Requested on 18.12.2024, for response check temp trash email:
    // Mail: grid5115@tutanota.com
    // Pass: sileGrid8464@#R%
    public string websocketToken = "<Request your Websocket Token>"; //You don't have one, get it here https://www.hyperate.io/api
    public string hyperateID = "internal-testing";
	// Websocket for connection with Hyperate
    WebSocket websocket;
    async void Start()
    {        
        logFilePath = Path.Combine(Application.dataPath, logFileName);

        websocket = new WebSocket("wss://app.hyperate.io/socket/websocket?token=" + websocketToken);
        Debug.Log("Connect!");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            SendWebSocketMessage();
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
        // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            var msg = JObject.Parse(message);

            if (msg["event"].ToString() == "hr_update")
            {
                // get the heart rate from the message and log it
                string rate = (string) msg["payload"]["hr"];
                Debug.Log("Current heart rate: " + rate + " at " + Time.time.ToString() + " from program start, (" + DateTime.Now.ToString() + ")");
                WriteToFile(rate);
            }
        };

        // Send heartbeat message every 25seconds in order to not suspended the connection
        InvokeRepeating("SendHeartbeat", 1.0f, heartRateRefreshRate);

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Log into the "internal-testing" channel
            await websocket.SendText("{\"topic\": \"hr:"+hyperateID+"\", \"event\": \"phx_join\", \"payload\": {}, \"ref\": 0}");
        }
    }
    async void SendHeartbeat()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Send heartbeat message in order to not be suspended from the connection
            await websocket.SendText("{\"topic\": \"phoenix\",\"event\": \"heartbeat\",\"payload\": {},\"ref\": 0}");

        }
    }

    private async void OnApplicationQuit()
    {
        if(websocket != null)
            await websocket.Close();
    }

    private void WriteToFile(string rate)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true)) 
            {
                string content = "HR"+","+ rate +","+ Time.time.ToString() +","+ DateTime.Now.ToString();
                writer.WriteLine(content);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("An error occurred while writing to file: " + e.Message);
        }
    }

}

public class HyperateResponse
{
    public string Event { get; set; }
    public string Payload { get; set; }
    public string Ref { get; set; }
    public string Topic { get; set; }
}
