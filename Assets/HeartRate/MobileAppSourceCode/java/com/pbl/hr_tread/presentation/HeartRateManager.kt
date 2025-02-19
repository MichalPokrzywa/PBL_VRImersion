package com.pbl.hr_tread.presentation

import android.content.Context
import android.content.Intent
import android.provider.Settings
import android.util.Log

class HeartRateManager(private val context: Context) {

    private var isRunning = false

    fun start(serverIp: String, port: String, frequency: Int = 5) {
        if (isRunning) return
        isRunning = true

        val intent1 = Intent(Settings.ACTION_IGNORE_BATTERY_OPTIMIZATION_SETTINGS)
        context.startActivity(intent1)

        val intent = Intent(context, HeartRateService::class.java).apply {
            putExtra("IP_ADDRESS", serverIp)
            putExtra("PORT", port.toInt())
            putExtra("FREQ", frequency)
        }
        context.startForegroundService(intent)
    }

    fun stop() {
        if (!isRunning) return
        isRunning = false
        Log.d("D_TAG", "Heart rate monitoring stopped")
        val intent = Intent(context, HeartRateService::class.java)
        context.stopService(intent)
    }
}
