using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;
using System.Text;

namespace SensorServer
{
    class RaspberrySensorServer :SensorServerInterface
    {
        private SerialDevice arduino;
        private DataReader dataReader;
        private CancellationTokenSource readCancelToken;
        const int RECEIEVE_BYTE_BUFFER = 11;
        private bool abort = true;
        private string TemperatureCurrentValue = "";

        public async Task initArduino(uint baudrate)
        {
            String devAQS = SerialDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(devAQS);
            arduino = await SerialDevice.FromIdAsync(dis[0].Id);
            arduino.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            arduino.BaudRate = baudrate;
            arduino.Parity = SerialParity.None;
            arduino.StopBits = SerialStopBitCount.One;
            arduino.DataBits = 8;
            dataReader = new DataReader(arduino.InputStream);
            dataReader.InputStreamOptions = InputStreamOptions.Partial;
            abort = false;
            startListening();

        }
      
        public async void startListening()
        {

            readCancelToken = new CancellationTokenSource();
  //           while (true)
 //            {
      //           if(readCancelToken.Token.IsCancellationRequested || arduino == null)
             //    {
    //                 break;
      //           }
                 Task SensorListeningTask = sensorListener();

                 await sensorListener();
            
             //}
        }

        private async Task sensorListener()
        {
            byte[] buffer;
            UInt32 bytesRecieved;
            try
            {
                if (arduino != null)
                {
                    while (true)
                    {
                        bytesRecieved = await dataReader.LoadAsync(RECEIEVE_BYTE_BUFFER).AsTask();
                        if (readCancelToken.Token.IsCancellationRequested || arduino == null) { abort = true; break; }
                        if (bytesRecieved > 0)
                        {
                            buffer = new byte[RECEIEVE_BYTE_BUFFER];
                            dataReader.ReadBytes(buffer);
                            TemperatureCurrentValue = System.Text.Encoding.UTF8.GetString(buffer);
                        
                        }
                        await Task.Delay(2500);
                    }
                }
            }
            catch (Exception e)
            {
                if (readCancelToken != null) { readCancelToken.Cancel(); System.Diagnostics.Debug.WriteLine(e.Message); }
            }
        }

        public string getSensorReading(SensorQuery sensorQuery)
        {
            return TemperatureCurrentValue;
        }
    }
}
