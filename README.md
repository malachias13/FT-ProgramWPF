# FT ProgramWPF

FT Program is a Windows desktop application that can send and receive data/files through a network using the Remote Procedure Call (gRPC) framework.

## How it works
-  A user can choose to host an instance of the server on their machine, and another user can choose to join that server as a client.
- The host machine can choose to select a folder with all the files they want to share with all clients that connect to the server.



## Setup and Instillation
1. Download and install the program.
2. If you are running/using this program on a local network and don't have a SLL certificate run the following command in your terminal:
```Command
dotnet dev-certs https --trust
```

   
   If you have a SLL certificate locate your appsettings.json file in the app data folder. Copy the following code and configure your certificate info.
  ```JSON
   "Certificate": {
          "Path": "certs/server.pfx",
          "Password": "your_password"
        }
```        
Example:
```JSON
  "Kestrel": {
    "Endpoints": {
      "Http2": {
        "Url": "https://0.0.0.0:7134",
        "Protocols": "Http2",
        "Certificate": {
          "Path": "certs/server.pfx",
          "Password": "your_password"
        }
      }
    }
  }
```
### Note: you can use a custom https URL.

3. run your appliaction.
