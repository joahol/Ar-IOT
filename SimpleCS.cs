// Reference: I have used this page for basis of this server/client:
//https://windowsinstructed.com/windows-iot-raspberry-communication-server-client/
//Smal and simple server implementation for win RT solutions on Raspberry using windows 10 IoT 
//JH - 29.10.18


using Windows.Storage.Streams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace SensorServer
{


    class SimpleCS
    {
     
        StreamSocket ConnectionSocket;
        static int SERVER_PORT = 9091;
        static String SERVER_ADDRES = "192.168.0.118";
          Windows.Storage.Streams.UnicodeEncoding eUTF8 = Windows.Storage.Streams.UnicodeEncoding.Utf8;

        public void startServer() {
            StreamSocketListener DataListener = new StreamSocketListener();
            DataListener.ConnectionReceived += ConnectionRecieved;
            DataListener.BindServiceNameAsync(SERVER_PORT.ToString()).AsTask().Wait();
            
        }
            public async void SendResponse(HostName adress, string message)
        {
            try
            {
                ConnectionSocket = new StreamSocket();
            await ConnectionSocket.ConnectAsync(adress, "9091");
            DataWriter responseWriter = new DataWriter(ConnectionSocket.OutputStream);
            byte[] encodedMessage = Encoding.UTF8.GetBytes(message);
            responseWriter.WriteBytes(encodedMessage);
            await responseWriter.StoreAsync();
            responseWriter.Dispose();
            ConnectionSocket.Dispose();
        
        }catch(Exception e){
        Debug.WriteLine("Something fishy with thte connection"+ e.Message);
            ConnectionSocket = null;
}   

        }
        private async void ConnectionRecieved( StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args) {
            StringBuilder sBuilder;
            string recived;
            DataReader dReader;

            using (dReader = new DataReader(args.Socket.InputStream)) {
                sBuilder = new StringBuilder();
                dReader.UnicodeEncoding = eUTF8;
                dReader.InputStreamOptions = InputStreamOptions.Partial;
                dReader.ByteOrder = ByteOrder.LittleEndian;
                await dReader.LoadAsync(256);
                while(dReader.UnconsumedBufferLength > 0) {
                    sBuilder.Append(dReader.ReadString(dReader.UnconsumedBufferLength));
                    await dReader.LoadAsync(256);
                }dReader.DetachStream();
                recived = sBuilder.ToString();
                if (recived != null) {
                    SendResponse(args.Socket.Information.RemoteAddress, "hi");
                    Debug.WriteLine("Message in:" + recived)
;
                }
            }
        }
    }
}
