using Dexter.GoPiGo;
using Dexter.GoPiGo.Sensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.IoT.ServerService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Dexter.Controller.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IGoPiGo _goPiGo;
        private IUltrasonicRangerSensor _devices;

        public MainPage()
        {
            this.InitializeComponent();

            //_goPiGo = DeviceFactory.Build.BuildGoPiGo();

            //_goPiGo.MotorController().EnableServo();

            //_devices = DeviceFactory.Build.BuildUltraSonicSensor(Pin.Trigger);

            System.Diagnostics.Debug.WriteLine("Start Listener");
            ServerSocketConnection.StartListener();
            ServerSocketConnection.NewMessageReady += SendCommand;

            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            //var camera = new Camera();
            //var mediaFrameFormats = await camera.GetMediaFrameFormatsAsync();
            //ConfigurationFile.SetSupportedVideoFrameFormats(mediaFrameFormats);
            //var videoSetting = await ConfigurationFile.Read(mediaFrameFormats);

            //await camera.Initialize(videoSetting);
            //camera.Start();

            //var httpServer = new HttpServer(camera);
            //httpServer.Start();
        }

        private void SendCommand(object sender, MessageSentEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Robot Message Received " + e.Message);
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

        private async void ParseCommand(GoPiGoCommand command, int value)
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
                case GoPiGoCommand.UltrasonicMeasure:
                    Scan();
                    break;
            }
        }

        public void Scan()
        {
            var delay = .02;
            var debug = 0;
            var num_of_readings = 45;
            var incr = 180 / num_of_readings;
            var ang_l = new double[(num_of_readings + 1)];
            var dist_l = new double[(num_of_readings + 1)];
            var x = new double[(num_of_readings + 1)];
            var y = new double[(num_of_readings + 1)];

            var buf = new int[40];
            var ang = 0;
            var lim = 250;
            var index = 0;
            var sample = 2;

            for (int i = 0; i < sample; i++)
            {
                var dist = us_dist(15);
                if (dist < lim && dist >= 0)
                    buf[i] = dist;
                else
                    buf[i] = lim;
            }

            var max = buf[1];
            var rm = max;

            if (rm == -1)
                rm = lim;

            System.Diagnostics.Debug.WriteLine("Index: " + index + " Ang: " + ang + " Dist: " + rm);

            ang_l[index] = ang;
            dist_l[index] = rm;

            index++;

        }

        int us_dist(int pin)
        {
            byte[] buffer = new byte[4];
            buffer[0] = 117;
            buffer[0] = (byte)pin;
            buffer[0] = 0;
            buffer[0] = 0;
            _goPiGo.RunCommand(Commands.UltraSonic, 15, 0, 0);

            System.Threading.Tasks.Task.Delay(200);

            try
            {
                var b1 = _goPiGo.DigitalRead(Pin.Trigger); // read 0
                var b2 = _goPiGo.DigitalRead(Pin.Trigger); // read 1

                if ((int)b1 != -1 && (int)b2 != -1)
                    return ((int)b1 * 255) + (int)b2;
                else
                    return -1;
            }
            catch
            {
                return -1;
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
        SetServoAngle,
        UltrasonicMeasure
    }
}
