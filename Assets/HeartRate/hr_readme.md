# Hear Rate Info

# Usage
1. If needed specify <port>
2. Choose if you want data to end written in file or stored in List by toggling writeToFileInsteadOfStore
2. Data is accessible in public member HrReadings, each time the data is accessed, container is cleared
3. To use with smartwatch: Devices must be on the same wifi network, wifi must have enabled option to detect devices in same network
4. Smartwatch must have dedicated app installed: Should be in assets folder
5. Enter, in Smartwatch app, IP and PORT, they are visible in Debug Log in Console after starting Unity project.
6. Smartwatch HR measurement can be started by pressing "Connect" button, or stopped by clicking it again.

1. Specify Port (if needed)
2. Choose Data Storage Option - Toggle writeToFileInsteadOfStore to decide whether you want the data to be saved to a file or stored in a list (public class member).
3. Accessing Data - Data is available via the public member HrReadings. Note that each time the data is accessed, the container is cleared.
4. Using with Smartwatch - Ensure that both the devices are on the same Wi-Fi network. The Wi-Fi network should have the option enabled to detect devices within the same network.
5. Install Smartwatch App - The smartwatch must have a dedicated app installed, which can be found in the assets folder.
6. Configure Smartwatch App - In the smartwatch app, enable all pervimsions that app asks for, enter the IP and PORT. These values can be found in the Debug Log in the Console after starting the Unity project. Configure how often measurements are to be sent (1s, 2s, 5s, 10s, 30s, 60s)
7. Start/Stop HR Measurement - Press the "Start" button to start the smartwatch HR measurement. Once started, app will try to send data to the server, you can run and stop Unity project as many times as you want, Wear OS app should still work continously. Press the "Stop" button to stop the measurement.
8. Warnigs - on smartwach app dialog with warnigs may appear - act reasonable according to commutinates, dialog can be closed or vanishes after 3 seconds, dialog does not stop the app.

When server is started, you can test its functionality by (having a watch duh..) or writing in cmd:
curl -X POST -d "bpm=666" http://<YOUR LOCAL IP>:<PORT>/
Results will be written to a file (line by line in csv fashion) located in Assets/<YOUR PATH AND FILE NAME>
