using Dexter.GoPiGo;
using Dexter.GoPiGo.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.IoT.ServerService;

namespace Dexter.Controller.Service
{
    public sealed class Main : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private IGoPiGo _goPiGo;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            _goPiGo = DeviceFactory.Build.BuildGoPiGo();

            _goPiGo.MotorController().EnableServo();

            SocketConnection.StartListener();
            SocketConnection.NewMessageReady += SendCommand;
        }

        private void SendCommand(object sender, MessageSentEventArgs e)
        {
            ParseCommand(e.Message);
        }

        public void ParseCommand(string message)
        {   
            try
            {
                var parsedCommand = message.Split('|');
                var commandInt = Convert.ToInt32(parsedCommand[0]);
                var command = (GoPiGoCommand)commandInt;
                var value = Convert.ToInt32(parsedCommand[1]);
                ParseCommand(command, value);
            }
            catch (Exception e)
            {
                //ToDo: error catching
            }
        }

        private void ParseCommand(GoPiGoCommand command, int value)
        {
            var motorController = _goPiGo.MotorController();
            switch (command)
            {
                case GoPiGoCommand.Stop:
                    motorController.Stop();
                    break;
                case GoPiGoCommand.Backward:
                    motorController.MoveBackward();
                    break;
                case GoPiGoCommand.Forward:
                    motorController.MoveForward();
                    break;
                case GoPiGoCommand.Left:
                    motorController.MoveLeft();
                    break;
                case GoPiGoCommand.Right:
                    motorController.MoveRight();
                    break;
                case GoPiGoCommand.RotateLeft:
                    motorController.RotateLeft();
                    break;
                case GoPiGoCommand.RotateRight:
                    motorController.RotateRight();
                    break;
                case GoPiGoCommand.SetLeftMotorSpeed:
                    motorController.SetLeftMotorSpeed(value);
                    break;
                case GoPiGoCommand.SetRightMotorSpeed:
                    motorController.SetRightMotorSpeed(value);
                    break;
                case GoPiGoCommand.SetServoAngle:
                    motorController.RotateServo(value);
                    break;
            }
        }
    }

    public enum GoPiGoCommand
    {
        Stop = 0,
        Forward,
        Backward,
        Left,
        Right,
        RotateLeft,
        RotateRight,
        SetLeftMotorSpeed,
        SetRightMotorSpeed,
        SwitchLeftLed,
        SwitchRightled,
        SetServoAngle
    }
}
