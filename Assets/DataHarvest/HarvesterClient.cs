using System.Net.Sockets;
using System.IO;
using System.Text;
using System;

public sealed class HarvesterClient {
    
    const string SERVER_ADDRESS = "127.0.0.1";
    const int SERVER_PORT = 5558;
    const float CONN_TIMEOUT = 5f;

    // Tries to send `data` to the server (synchronously). Returns success or failure status.
    public bool SendData(string data) {
        LogHelper.Info(this, "sending data to " + SERVER_ADDRESS + ":" + SERVER_PORT + "...");
        
        using (var client = new UdpClient(SERVER_ADDRESS, SERVER_PORT)) {
            byte[] sent = Encoding.UTF8.GetBytes(data);
            try {
                client.Send(sent, sent.Length);
                // TODO: receive server ack?
            } catch (Exception ex) {
                LogHelper.Error(this, "while sending data: " + ex.StackTrace);
                return false;
            } 
        }
        return false;
        //return true;
    }

       // TCP, maybe in future
       /* try {
            // Connect to the server
            LogHelper.Info(this, "starting connection to " + SERVER_ADDRESS + ":" + SERVER_PORT + "...");
            
            var result = client.BeginConnect(SERVER_ADDRESS, SERVER_PORT, null, null);
            //client.Connect(SERVER_ADDRESS, SERVER_PORT);

            if (result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(CONN_TIMEOUT))) {
            //if (connected) {
                LogHelper.Ok(this, "connection succeeded.");
            } else {
                LogHelper.Error(this, "connection failed.");
                return false;
            }

            client.EndConnect(result); 
            
            NetworkStream stream = client.GetStream();
            if (!stream.CanWrite) {
                LogHelper.Error(this, "cannot write to socket!");
                return false;
            }

            // Write data to the socket
            using (var writer = new StreamWriter(stream, new UTF8Encoding())) {
                writer.WriteLine(data);
            }

            return true;
        } finally {
            client.Close();
        }
    }*/
}
