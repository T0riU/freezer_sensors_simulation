using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGR
{
    public enum SensorType
    {
        TEMPERATURE,
        CONCENTRATION,
        CURRENTDENSITY,
        NONE
    };
    public struct Const
    {
        public const double MIN_CR_SENSOR_VAL = 4.2d;
        public const double MAX_CR_SENSOR_VAL= 7.8d;
        public const double MIN_N_SENSOR_VAL = 5.3d;
        public const double MAX_N_SENSOR_VAL = 6.7d;
        public const double DF_VAL_SENSOR = 6.0d;
        public const double MAX_SENSOR_VAL = 12.0d;
        public const double MIN_SENSOR_VAL = 0.0d;
    }

}
