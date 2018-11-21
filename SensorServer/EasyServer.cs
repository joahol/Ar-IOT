using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using System.Diagnostics;

using Windows.Devices;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace SensorServer
{


    class EasyServer
    {
        private const uint BUFFER_SIZE = 4096;
        private const string SERVER_PORT = "9091";
        // private int numhits = 0;
        public const bool iverbose = true;
       private static  SensorServerInterface ssiHandle;
        public EasyServer(SensorServerInterface sensorServeriInterfaceHandle)
        {
            ssiHandle = sensorServeriInterfaceHandle;
        }
        public async void start()
        {//listen for innbound connections
            var socketListener = new StreamSocketListener();
            await socketListener.BindServiceNameAsync(SERVER_PORT);
            socketListener.ConnectionReceived += async (sender, args) =>
            {
                var request = new StringBuilder();
                using (var input = args.Socket.InputStream)
                {
                    var data = new byte[BUFFER_SIZE];
                    IBuffer readBuffer = data.AsBuffer();
                    var dataread = BUFFER_SIZE;
                    while (dataread == BUFFER_SIZE)
                    {
                        await input.ReadAsync(readBuffer, BUFFER_SIZE, InputStreamOptions.Partial);
                        request.Append(System.Text.Encoding.UTF8.GetString(data, 0, data.Length));
                        dataread = readBuffer.Length;
                    }
                }
                
                string query = echoQuery(request);
                //
                using (var output = args.Socket.OutputStream)
                {
                    using (var response = output.AsStreamForWrite())
                    {
                        var html = Encoding.UTF8.GetBytes(
                            $"<html><head><title>Background Message</title></head><body>testpage {query}</body></html>");
                        using (var bodyStream = new MemoryStream(html))
                        {
                            var header = $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\n\r\n";
                            var headerArray = Encoding.UTF8.GetBytes(header);
                            await response.WriteAsync(headerArray, 0, headerArray.Length);
                            await bodyStream.CopyToAsync(response);
                            await response.FlushAsync();
                        }
                    }
                }
            };
        }
        //Parse SensorType and SensorId from query
        private static SensorQuery parseQuery(String qString)
        {
            //String qString = query.ToString();
            int indexfirst = qString.IndexOf('?');
            int indexsecond = qString.IndexOf('=');

            SensorQuery sq = new SensorQuery();
            sq.sensorID = qString.Substring(indexfirst + 1, (indexsecond - indexfirst) - 1);
            sq.sensorType = qString.Substring(indexsecond + 1, ((qString.Length) - indexsecond) - 1);
            if (iverbose)
            {
                Debug.WriteLine("parseRequest sensorId: " + sq.sensorID + " sensorType:" + sq.sensorType);
            }
            return sq;
        }

        //Trim request to querry parts only remove header values, if query not empty then parse using parsequery
        private static string echoQuery(StringBuilder req)
        {
            var reqString = req.ToString().Split(' ');
            var url = reqString.Length > 1 ? reqString[1] : "";
            var uri = new Uri("http://localhost" + url);
            var query = uri.Query;
            if (query != "")
            {
                SensorQuery sq = parseQuery(query);
                query ="Sensor reading:" +ssiHandle.getSensorReading(sq);
                
            }
          
            if (iverbose)
            {
                Debug.WriteLine("getQuery:" + req.ToString());
            }
            return query;
        }
    }
}
 
