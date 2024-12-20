#Hear Rate Info
For now, the best solution is using hrServer script located in /C#Websocket and present in HeartRate prefab. It works on separate thread, so idk how this will affect main program in the long run. 
When server is started, you can test its functionality by (having a watch duh..) or writing in cmd:
curl -X POST -d "bpm=666" http://<YOUR LOCAL IP>>:6547/
Results will be written to a file (line by line in csv fashion) located in Assets/<YOUR PATH AND FILE NAME>

Alternatives in other folders are:
- python web socket - does similar thins, maybe can be fired from within unity?
- hyperateSocket - elaborate service but we need an API key form owner of this service 

Apps for smart watch:
[C# or python websocket] - https://galaxystore.samsung.com/geardetail/tUhSWQRbmv 

Sources:
https://github.com/loic2665/HeartRateToWeb/
https://www.hyperate.io/