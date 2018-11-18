using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SensorServer
{
    internal class SmallServer
    {
        private static uint BUFFER_SIZE = 2048;
        private const string EMPTY_RESPONSE = "no value ready";
        private static string SERVICE_PORT = "9198";
        SmallServerMessageInterface ssmi;

        public SmallServer() { }


        public async Task StartServer()
        {
            StreamSocketListener listen = new StreamSocketListener();
            string response = "";
            await listen.BindServiceNameAsync(SERVICE_PORT);
            listen.ConnectionReceived += async (sender, args) =>
            {
                StringBuilder req = new StringBuilder();

                using (var flow = args.Socket.InputStream)
                {

                    byte[] recieved = new byte[BUFFER_SIZE];
                    IBuffer buff = recieved.AsBuffer();
                    uint reci = BUFFER_SIZE;
                    while (reci == BUFFER_SIZE)
                    {
                        await flow.ReadAsync(buff, BUFFER_SIZE, InputStreamOptions.Partial);
                        req.Append(Encoding.UTF8.GetString(recieved, 0, recieved.Length));
                        reci = buff.Length;
                    }
                }
                response = ssmi.RequestRecieved(req.ToString());
                if (response == "") { response = EMPTY_RESPONSE; }
                using (var output = args.Socket.OutputStream)
                {
                    using (var respond = output.AsStreamForWrite())
                    {
                        byte[] message = Encoding.UTF8.GetBytes(response);
                        MemoryStream ms = new MemoryStream(message);
                        using (ms)
                        {
                            await respond.WriteAsync(message, 0, message.Length);
                            await ms.CopyToAsync(respond);
                            await respond.FlushAsync();
                        }
                    }
                }
            };
        }
    }


}
interface SmallServerMessageInterface
{
    string RequestRecieved(String request);
    void Response(String response);
}

