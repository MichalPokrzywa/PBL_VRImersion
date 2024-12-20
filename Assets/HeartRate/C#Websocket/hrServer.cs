using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;

public class HeartRateServer : MonoBehaviour
{
    private HttpListener httpListener;
    private Thread serverThread;
    private ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();
   
    [SerializeField]
    string logFileName = "HeartRate/hr_log_file.csv";
    string logFilePath;
    // private const string logFilePath = "./hr.txt";

    void Start()
    {
        logFilePath = Path.Combine(Application.dataPath, logFileName);
        string localIP = GetLocalIPAddress();
        int port = 6547; // Default port
        Debug.Log($"This is your IP: {localIP}");
        Debug.Log($"Port is {port}");

        // Background thread for the server, communication with main thread is done via actions (mainThreadActions:ConcurrentQueue)
        serverThread = new Thread(() => StartServer(port));
        serverThread.Start();
    }

    void Update()
    {
        while (mainThreadActions.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }

    void OnApplicationQuit()
    {
        StopServer();
    }

    private void StartServer(int port)
    {
        httpListener = new HttpListener();
        httpListener.Prefixes.Add($"http://*:{port}/");
        httpListener.Start();
        Debug.Log("Server started");

        while (httpListener.IsListening)
        {
            try
            {
                var context = httpListener.GetContext();
                HandleRequest(context);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error handling request: {e}");
            }
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        string responseString = string.Empty;
        int statusCode = 200;

        try
        {
            if (context.Request.HttpMethod == "GET")
            {
                Debug.Log("GET");

                // responseString = ReadHeartRate();
                // WriteToFile(responseString);
            }
            else if (context.Request.HttpMethod == "POST")
            {
                Debug.Log("POST");
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    string postData = reader.ReadToEnd();
                    var dataParts = postData.Split('=');

                    if (dataParts.Length > 1)
                    {
                        string heartRate = dataParts[1];
                        Debug.Log($"Received BPM = {heartRate}");
                        WriteToFile(heartRate);
                        responseString = "OK";
                    }
                    else
                    {
                        responseString = "Invalid POST data";
                        statusCode = 400;
                    }
                }
            }
            else
            {
                responseString = "Method Not Allowed";
                statusCode = 405;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error processing request: {ex}");
            responseString = "Internal Server Error";
            statusCode = 500;
        }

        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        context.Response.StatusCode = statusCode;
        context.Response.ContentLength64 = buffer.Length;
        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.OutputStream.Close();
    }

    private void StopServer()
    {
        if (httpListener != null)
        {
            httpListener.Stop();
            httpListener.Close();
            httpListener = null;
            Debug.Log("Server stopped.");
        }

        if (serverThread != null)
        {
            serverThread.Abort();
            serverThread = null;
            Debug.Log("Thread stopped.");
        }
    }

    private string GetLocalIPAddress()
    {
        string localIP = "127.0.0.1";
        try
        {
            using (var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
            {
                socket.Connect("10.255.255.255", 1); // Non-routable address
                localIP = ((IPEndPoint)socket.LocalEndPoint).Address.ToString();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting local IP: {ex}");
        }
        return localIP;
    }

    private void WriteToFile(string rate)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true)) 
            {
            string content = "HR"+","+ rate +","+ DateTime.Now.ToString();
            writer.WriteLine(content);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("An error occurred while writing to file: " + e.Message);
        }
    }
}
