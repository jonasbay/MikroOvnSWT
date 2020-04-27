using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Tests.Integration
{
    [TestFixture]
    public class IT2A_Cookcontroller
    {
        private CookController _cookController;
        private ILight _light;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private UserInterface _userInterface;
        private Door _door;
        private Button _timeButton;
        private Button _powerButton;
        private Button _startCancelButton;

        [SetUp]
        public void SetUp()
        {
            _light = Substitute.For<ILight>();
            _display = Substitute.For<IDisplay>();
            _timer = Substitute.For<ITimer>();
            _powerTube = Substitute.For<IPowerTube>();
            _door = new Door();
            _timeButton = new Button();
            _powerButton = new Button();
            _startCancelButton = new Button();
            _cookController = new CookController(_timer, _display, _powerTube);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);

            _cookController.UI = _userInterface;
        }

        [Test] 
        public void StartCooking_MyTimer_Received_Start()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _timer.Received().Start(60);
        }

        [Test]
        public void Stop_MyPowerTube_Received_TurnOff()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();
            _powerTube.Received().TurnOff();
        }

        [Test]
        public void OnTimerTick_MyDisplay_Received_ShowTime()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _display.Received().ShowTime(1, 0);
        }

        [Test] // Indsæt flere parameter her. Fx time er større end 1 og power er højere.
        public void OnTimerExpired_UI_Received_CookingIsDone()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            _light.Received().TurnOff();
        }
    }
}
