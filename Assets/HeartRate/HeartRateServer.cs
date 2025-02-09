using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace HR
{
/*
* Usage:
* 1. If needed specify <port>
* 2. Choose if you want data to end written in file or stored in List by toggling writeToFileInsteadOfMemoryStore
* 2. Data is accessible in public member HrReadings, each time the data is accessed, container is cleared
* 3. To use with smartwatch: Devices must be on the same wifi network
* 4. Smartwatch must have dedicated app installed:
* 5. Enter, in Smartwatch app, IP and PORT, they are visible in Debug Log in Console after starting Unity project.
* 6. Smartwatch HR measurement can be started by pressing "Connect" button, or stopped by clicking it again.
*/
public class HeartRateServer : MonoBehaviour
{
    private HttpListener httpListener;
    private Thread serverThread;
    private ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    public string currentBPM { get; private set; } = "0.0";
    public string localIP { get; private set; } = "192.168.0.0";
    
    private object _lock = new object();
    public static HeartRateServer instance;

    [SerializeField]
    public int port { get; private set; } = 6547;

    [SerializeField]
    string logFileName = "HeartRate/hr_log_file.csv";
    private string logFilePath;

    private List<(string, string)> hrReadings = new List<(string, string)>();
    public List<(string, string)> HrReadings
    {
        get
        {
            lock (_lock)
            {
            List<(string, string)> result = new List<(string, string)>(hrReadings);
            hrReadings.Clear();
            return result;
            }
        }
        private set { hrReadings = value; }
    }

    [SerializeField]
    bool writeToFileInsteadOfMemoryStore = false;

    void OnDestroy()
    {
        StopServer();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        logFilePath = Path.Combine(Application.dataPath, logFileName);
        localIP = GetLocalIPAddress();

        // Background thread for the server, communication with main thread can be done via actions (mainThreadActions:ConcurrentQueue)
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
        Debug.Log("Heart Rate Service Server started!");
        Debug.Log($"Heart Rate Address for WearOS app: IP: {localIP} | PORT: {port}");

        while (httpListener.IsListening)
        {
            try
            {
                var context = httpListener.GetContext();
                HandleRequest(context);
            }
            catch (Exception e)
            {
                Debug.LogError($"HeartRateServer: Error handling request: {e}");
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
                Debug.Log("HeartRateServer: GET, This should not happened...");
            }
            else if (context.Request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    string postData = reader.ReadToEnd();

                    var dataParts = postData.Split(':');

                    if (dataParts.Length > 1)
                    {
                        string heartRate = dataParts[1].Substring(0, dataParts[1].Length - 1);
                        Debug.Log($"HeartRateServer: Received BPM = {heartRate}");
                        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                        string currentTime = DateTime.Now.ToString("HH:mm:ss");
                        if(writeToFileInsteadOfMemoryStore)
                            WriteToFile(heartRate, currentDate, currentTime);
                        lock (_lock)
                        {
                            hrReadings.Add((heartRate, currentTime));
                        }
                        responseString = "OK";
                        currentBPM = heartRate;
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
            Debug.LogError($"HeartRateServer: Error processing request: {ex}");
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
            Debug.Log("HeartRateServer: Server stopped.");
        }

        if (serverThread != null)
        {
            serverThread.Abort();
            serverThread = null;
            Debug.Log("HeartRateServer: Thread stopped.");
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
            Debug.LogError($"HeartRateServer: Error getting local IP: {ex}");
        }
        return localIP;
    }

    private void WriteToFile(string rate, string currentDate, string currentTime)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true)) 
            {
            string content = $"HR,{rate},{currentDate},{currentTime}";
            writer.WriteLine(content);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("HeartRateServer: An error occurred while writing to file: " + e.Message);
        }
    }
}
}
