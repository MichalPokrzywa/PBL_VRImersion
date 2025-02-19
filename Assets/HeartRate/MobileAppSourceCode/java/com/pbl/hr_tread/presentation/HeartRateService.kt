package com.pbl.hr_tread.presentation

import android.app.Notification
import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.Service
import android.content.Context
import android.content.Intent
import android.os.IBinder
import android.os.PowerManager
import android.util.Log
import androidx.core.app.NotificationCompat

class HeartRateService : Service() {
    private lateinit var wakeLock: PowerManager.WakeLock

    private var heartRateMonitor: HeartRateMonitor? = null

    override fun onCreate() {
        super.onCreate()
        val powerManager = getSystemService(Context.POWER_SERVICE) as PowerManager
        wakeLock = powerManager.newWakeLock(PowerManager.SCREEN_DIM_WAKE_LOCK, "MyApp:Wakelock")
        wakeLock.acquire(120*60*1000L /*120 minutes*/)
//        heartRateMonitor = HeartRateMonitor(this) { heartRate ->
//            sendHeartRate(ip = "192.168.0.74", port = 6547, heartRate = heartRate, applicationContext)
//        }
//        heartRateMonitor?.start((5*1000).toLong())
    }

    override fun onDestroy() {
        wakeLock.release()
        heartRateMonitor?.stop()
        super.onDestroy()
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        heartRateMonitor = HeartRateMonitor(this) { heartRate ->
            sendHeartRate(
                ip = intent?.getStringExtra("IP_ADDRESS") ?: "192.168.0.74",
                port = intent?.getIntExtra("PORT", 6547) ?: 6547,
                heartRate = heartRate)
        }
        Log.d("D_TAG", "Heart rate monitoring started")
        heartRateMonitor?.start(((intent?.getIntExtra("FREQ", 5) ?: 5)*1000).toLong())

        startForeground(1, createNotification())

        return START_STICKY
    }

    override fun onBind(intent: Intent?): IBinder? = null

    private fun createNotification(): Notification {
        val channelId = "heart_rate_service"
        val channel = NotificationChannel(channelId, "Heart Rate", NotificationManager.IMPORTANCE_LOW)
        getSystemService(NotificationManager::class.java).createNotificationChannel(channel)

        return NotificationCompat.Builder(this, channelId)
            .setContentTitle("Heart Rate Monitor")
            .setContentText("Monitoring heart rate...")
            .setSmallIcon(1)
            .build()
    }
}
