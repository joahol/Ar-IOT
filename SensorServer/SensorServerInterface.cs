using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorServer
{
    interface SensorServerInterface
    {
        String getSensorReading(SensorQuery sensorQuery);
    }

    public struct SensorQuery
    {
        public String sensorID;
        public String sensorType;
    }

    public struct SensorReading
    {
        String sensorID;
        String sensorValue;
    }
}
