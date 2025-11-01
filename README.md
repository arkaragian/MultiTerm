# MultiTerm

A Small Command Line Serial and TCP Terminal

## Serial Configuration

```json
{
    "Terminals" : [
        {
            "ConnectionType": "Serial",
            "SerialPortSettings": {
                "PortName": "COM20",
                "BaudRate": 115200,
                "Parity": "None",
                "DataBits": 8,
                "StopBits": "One",
                "LogSettings": {
                    "EnableLogging": false,
                    "LogTransmit": true,
                    "TransmitLogType" : "byte",
                    "LogReceive": true,
                    "ReceiveLogType" : "byte",
                    "LogLevel": "Information"
                }
            }
        }
    ]
}
```


## Network Configuration
```json
{
    "Terminals" : [
        {
            "ConnectionType": "Network",
            "NetworkConnectionSettings": {
                "NetworkType" : "InterNetwork",
                "TransmissionProtocol" :"TCP",
                "RemoteAddress": "192.168.0.90",
                "RemotePort" : 3000,
                "LogSettings": {
                    "EnableLogging": false,
                    "LogTransmit": true,
                    "TransmitLogType" : "byte",
                    "LogReceive": true,
                    "ReceiveLogType" : "byte",
                    "LogLevel": "Information"
                }
            }
        }
    ]
}
```
