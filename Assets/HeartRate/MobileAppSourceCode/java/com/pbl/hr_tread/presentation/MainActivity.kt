package com.pbl.hr_tread.presentation

import android.Manifest
import android.content.pm.PackageManager
import android.os.Build
import android.os.Bundle
import android.util.Log
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.result.contract.ActivityResultContracts
import androidx.annotation.RequiresApi
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.AlertDialog
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.core.content.ContextCompat
import androidx.core.splashscreen.SplashScreen.Companion.installSplashScreen
import androidx.wear.compose.material.MaterialTheme
import androidx.wear.compose.material.Text
import androidx.wear.compose.material.TimeText
import com.pbl.hr_tread.presentation.theme.HR_TreadTheme
import okhttp3.Call
import okhttp3.Callback
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody
import okhttp3.RequestBody.Companion.toRequestBody
import okhttp3.Response
import org.json.JSONObject
import java.io.IOException
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.TextFieldDefaults
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.wear.compose.material.Button
import kotlinx.coroutines.delay

var connection_error_counter = 0
var showDialog = mutableStateOf(false)
var no_pulse_error_counter = 0

class MainActivity : ComponentActivity() {

    private val requestPermissionLauncher = registerForActivityResult(
        ActivityResultContracts.RequestPermission()
    ) { isGranted: Boolean ->
        if (!isGranted) {
            // Permission denied
            finish()
        }
    }

    @RequiresApi(Build.VERSION_CODES.P)
    override fun onCreate(savedInstanceState: Bundle?) {
        installSplashScreen()
        super.onCreate(savedInstanceState)

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.BODY_SENSORS)
            != PackageManager.PERMISSION_GRANTED) {
            requestPermissionLauncher.launch(Manifest.permission.BODY_SENSORS)
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS)
            != PackageManager.PERMISSION_GRANTED) {
            requestPermissionLauncher.launch(Manifest.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS)
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.WAKE_LOCK)
            != PackageManager.PERMISSION_GRANTED) {
            requestPermissionLauncher.launch(Manifest.permission.WAKE_LOCK)
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.FOREGROUND_SERVICE)
            != PackageManager.PERMISSION_GRANTED) {
            requestPermissionLauncher.launch(Manifest.permission.FOREGROUND_SERVICE)
        }

        setTheme(android.R.style.Theme_DeviceDefault)
        setContent {
            WearApp()
        }
    }
}

@Composable
fun WearApp() {
    HR_TreadTheme {
        Box(
            modifier = Modifier
                .fillMaxSize()
                .background(MaterialTheme.colors.background),
            contentAlignment = Alignment.Center
        ) {
            TimeText()
            Log.d("D_TAG", "App start")
            WearOSInputScreen()
        }
    }
}

@Composable
fun WearOSInputScreen() {
    val context = LocalContext.current
    val heartRateManager = remember { HeartRateManager(context) }
    var isMonitoring by remember { mutableStateOf(false) }

    var freqIndex = 2
    val freqList = arrayOf(1,2,5,10,30,60)
    var freq by remember { mutableIntStateOf(freqList[freqIndex]) }

    var ipInput by remember { mutableStateOf("192.168.0.0") }
    var portInput by remember { mutableStateOf("8080") }

    val showDialogC by showDialog

    ShowAutoClosingAlert(showDialogC, onDismiss = { showDialog.value = false })
    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(horizontal = 25.dp, vertical = 5.dp),
        verticalArrangement = Arrangement.Center
    ) {
        Spacer(modifier = Modifier.height(8.dp))
        OutlinedTextField(
            value = ipInput,
            textStyle = TextStyle(fontSize = 10.sp, color = Color.White),
            singleLine = true,
            colors =TextFieldDefaults.colors(
                focusedContainerColor = Color(0xFF000000),
                unfocusedContainerColor = Color(0xFF202020),
                focusedIndicatorColor = Color.Magenta,
                unfocusedIndicatorColor = Color.White,
                cursorColor = Color.White,
                focusedTextColor = Color.White,
                unfocusedTextColor = Color.White
            ),
            onValueChange = { ipInput = it },
            label = { Text("Target IP", color = Color.White, fontSize = 9.sp) },
            keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
            modifier = Modifier.fillMaxWidth(),
            enabled = !isMonitoring
        )
        Spacer(modifier = Modifier.height(2.dp))
        OutlinedTextField(
            value = portInput,
            colors =TextFieldDefaults.colors(
                focusedContainerColor = Color(0xFF000000),
                unfocusedContainerColor = Color(0xFF202020),
                focusedIndicatorColor = Color.Magenta,
                unfocusedIndicatorColor = Color.White,
                cursorColor = Color.White,
                focusedTextColor = Color.White,
                unfocusedTextColor = Color.White
            ),
            textStyle = TextStyle(fontSize = 10.sp, color = Color.White),
            onValueChange = { portInput = it },
            keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
            singleLine = true,
            label = { Text("Target Port", color = Color.White, fontSize = 9.sp) },
            modifier = Modifier.fillMaxWidth(),
            enabled = !isMonitoring
        )
        Spacer(modifier = Modifier.height(2.dp))
        Box(
            modifier = Modifier.fillMaxWidth(),
            contentAlignment = Alignment.Center
        ) {
            Row()
            {
                Button(onClick = {
                    Log.d(
                        "D_TAG",
                        " Button Clicked, HR is starting: $isMonitoring, Address: $ipInput : $portInput"
                    )
                    if (isMonitoring) {
                        heartRateManager.stop()
                    } else {
                        heartRateManager.start(ipInput, portInput, freq)
                    }
                    isMonitoring = !isMonitoring
                })
                {
                    Text(text = if (!isMonitoring) "Start" else "Stop", fontSize = 8.sp)
                }
                Button(enabled = !isMonitoring, onClick = {
                    Log.d(
                        "D_TAG",
                        " Freq Button Clicked, Frequency setting is now: $freq"
                    )
                    if (!isMonitoring) {
                        freqIndex += 1
                        if(freqIndex == freqList.size) {freqIndex = 0}
                        freq = freqList[freqIndex]
                    }
                })
                {
                    Text(text = "f= $freq s", fontSize = 8.sp)
                }
            }
        }
        Spacer(modifier = Modifier.height(4.dp))
    }
}



fun sendHeartRate(ip: String = "localhost", port: Int = 6547, heartRate: Float) {
    if (heartRate == 0.0f)
    {
        no_pulse_error_counter += 1
        if (no_pulse_error_counter == 4)
        {
            showDialog.value = true
        }
        Log.d("D_TAG", "Heart Rate is $heartRate, skipping sending operation")
        return
    }
    no_pulse_error_counter = 0
    val url = "http://$ip:$port/"

    val json = JSONObject()
    json.put("heartRate", heartRate)

    val requestBody: RequestBody = json.toString().toRequestBody("application/json".toMediaType())

    val client = OkHttpClient()
    val request = Request.Builder()
        .url(url)
        .post(requestBody)
        .build()

    client.newCall(request).enqueue(object : Callback {
        override fun onFailure(call: Call, e: IOException) {
            e.printStackTrace()
            connection_error_counter += 1
            if (connection_error_counter == 3)
            {
                showDialog.value = true
            }
        }

        override fun onResponse(call: Call, response: Response) {
            Log.d("D_TAG","Response: ${response.body?.string()}")
            connection_error_counter = 0
        }
    })
    Log.d("D_TAG", "Heart Rate $heartRate sent to $url")
}

@Composable
fun ShowAutoClosingAlert(showDialog: Boolean,onDismiss: () -> Unit) {
    if (showDialog) {
        LaunchedEffect(Unit) {
            delay(3000)
            onDismiss()
        }
        val title = if (connection_error_counter == 3) "Connection Error" else "Heart Rate Error"
        val message = if (connection_error_counter == 3) "Server (probably) not reachable" else "No pulse detected (bpm=0.0)"
        if (connection_error_counter >= 3) connection_error_counter = 0 else if(no_pulse_error_counter >= 4) no_pulse_error_counter = 0

        AlertDialog(
            onDismissRequest = { onDismiss() },
            title = { Text(title, color = Color.White, fontSize = 10.sp) },
            text = { Text(message, color = Color.White, fontSize = 9.sp) },
            confirmButton = {
                Button(onClick = { onDismiss() }) {
                    Text("OK", color = Color.White)
                }
            },
            containerColor = Color.Black
        )
    }
}
