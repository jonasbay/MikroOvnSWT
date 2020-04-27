using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Tests.Integration
{

    //Behøver kun at teste forbindelsen ikke om der modtages korrekt - det er gjort i unit test
    //Denne måde er den korrekte måde vi tester userInterface

    [TestFixture]
    public class IT1_Button_Door
    {
        private ICookController _cookController;
        private ILight _light;
        private IDisplay _display;
        private UserInterface _userInterface;
        private Door _door;
        private Button _timeButton;
        private Button _powerButton;
        private Button _startCancelButton;

        [SetUp]
        public void SetUp()
        {
            _cookController = Substitute.For<ICookController>();
            _light = Substitute.For<ILight>();
            _display = Substitute.For<IDisplay>();
            _door = new Door();
            _timeButton = new Button();
            _powerButton = new Button();
            _startCancelButton = new Button();
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
        }


        [TestCase(1, 50)] //Test READY state
        [TestCase(2, 100)] //Test SETPOWER state
        public void powerBtn_Pressed_DisplayReceivedCall(int presses, int powerResult)
        {
            for (int i = 0; i < presses; i++)
            {
                _powerButton.Press();
            }
            
            _display.Received().ShowPower(powerResult);
        }

        [TestCase(1, 1)] //Test SETPOWER state
        [TestCase(2, 2)] //Test SETTIME state
        public void timeBtn_Pressed_DisplayReceivedCall(int presses, int timeResult)
        {
            _powerButton.Press();
            for (int i = 0; i < presses; i++)
            {
                _timeButton.Press();
            }

            _display.Received().ShowTime(timeResult, 0);
        }

        [Test] //SETPOWER state
        public void startCancelBtn_Pressed_LightReceivedCall()
        {
            _powerButton.Press();
            _startCancelButton.Press();
            _light.Received().TurnOff();
        }

        [Test] //SetTimeState
        public void startCancelBtn_Pressed_CookControllerReceivedCall()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _cookController.Received().StartCooking(50, 1 * 60);
        }

        [Test] //CookingState
        public void startCancelBtn_Pressed_CookingState_LightReceivedCall()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();
            _light.Received().TurnOff();
        }

        // DOOR TEST // 
        #region DOORTESTS
        [Test] 
        public void onDoorOpened_LightReceivedCall_TurnOn_ReadyState()
        {
            _door.Open();
            _light.Received().TurnOn();
        }

        [Test]
        public void onDoorOpened_DisplayReceivedCall_Clear_SetPower()
        {
            _powerButton.Press();
            _door.Open();
            _display.Received().Clear();
        }

        [Test]
        public void onDoorOpened_MyCookerReceivedCall_Stop_Cooking()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();
            _cookController.Received().Stop();
        }
        #endregion
    }
}
