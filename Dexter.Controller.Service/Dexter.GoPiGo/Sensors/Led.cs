namespace Dexter.GoPiGo.Sensors
{
    public interface ILed
    {
        SensorStatus CurrentState { get; }
        ILed ChangeState(SensorStatus newState);
    }

    public class Led : Sensor<ILed>, ILed
    {
        public Led(IGoPiGo device, Pin pin) : base(device, pin, PinMode.Output)
        {
        }
    }
}
