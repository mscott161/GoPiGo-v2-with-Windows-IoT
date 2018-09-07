using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.UI.Xaml;

namespace Dexter.GoPiGo.Sensors
{
    public interface IUltrasonicRangerSensor
    {
        Task<int> MeasureInCentimeters();
    }

    internal class UltrasonicRangerSensor : IUltrasonicRangerSensor
    {
        private const byte CommandAddress = 117;
        private readonly GoPiGo _device;
        private readonly Pin _pin;

        internal UltrasonicRangerSensor(GoPiGo device, Pin pin)
        {
            _device = device;
            _pin = pin;
        }

        public async Task<int> MeasureInCentimeters()
        {
            var buffer = new[] { CommandAddress, (byte)_pin, Constants.Unused, Constants.Unused };
            _device.DirectAccess.Write(buffer);
            await Task.Delay(5);
            _device.DirectAccess.Read(buffer);

            System.Diagnostics.Debug.WriteLine("Byte 0: " + buffer[0]);
            System.Diagnostics.Debug.WriteLine("Byte 1: " + buffer[1]);
            System.Diagnostics.Debug.WriteLine("Byte 2: " + buffer[2]);
            System.Diagnostics.Debug.WriteLine("Byte 3: " + buffer[3]);

            return buffer[1] * 256 + buffer[2];
        }
    }

}
