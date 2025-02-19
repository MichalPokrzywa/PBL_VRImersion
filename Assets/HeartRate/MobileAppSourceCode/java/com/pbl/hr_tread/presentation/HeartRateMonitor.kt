package com.pbl.hr_tread.presentation

import android.content.Context
import android.hardware.Sensor
import android.hardware.SensorEvent
import android.hardware.SensorEventListener
import android.hardware.SensorManager
import android.util.Log

class HeartRateMonitor(context: Context, private val listener: (Float) -> Unit) :
    SensorEventListener {

    private val sensorManager = context.getSystemService(Context.SENSOR_SERVICE) as SensorManager
    private val heartRateSensor: Sensor? = sensorManager.getDefaultSensor(Sensor.TYPE_HEART_RATE)
    private var startTime: Long = 0
    private var measurementDelay: Long = 5000

    fun start(delay: Long = 5000) {
        measurementDelay = delay
        Log.d("D_TAG", "HR Monitor start")
        startTime = System.currentTimeMillis()
        heartRateSensor?.let {
            Log.d("D_TAG", "HR Monitor registered Listener")
            sensorManager.registerListener(this, it, SensorManager.SENSOR_DELAY_UI)
        }
    }

    fun stop() {
        sensorManager.unregisterListener(this)
        startTime = System.currentTimeMillis()
    }

    override fun onSensorChanged(event: SensorEvent?) {
        if (System.currentTimeMillis() - startTime < measurementDelay)
        {
            return
        }
        startTime = System.currentTimeMillis()
        Log.d("D_TAG", "HR Monitor reading")
        event?.values?.firstOrNull()?.let { listener(it) }
    }

    override fun onAccuracyChanged(sensor: Sensor?, accuracy: Int) {}
}
