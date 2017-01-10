using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace FlocKing.Helpers
{
	public enum ControllerEvents
	{
		NoEvent = -1, Connected, Disconnected
	}

	public enum AnalogStickDeadZone
	{
		None = 0, IndependantAxes, Circular
	}

	public enum ContolPadButtons
	{
		A = 0, B, Back, LeftShoulder, LeftStick, RightShoulder, RightStick, Start, X, Y, LeftTrigger,
		RightTrigger, DPadDown, DPadLeft, DPadRight, DPadUp, LeftThumbStickX, LeftThumbStickY,
		RightThumbStickX, RightThumbStickY, NumControlPadButtons
	}

	public enum MouseButtons
	{
		LeftButton = 0, MiddleButton, RightButton, XButton1, XButton2, NumMouseButtons
	}

	// Controller event arguments to be passed into the InputEventHandler
	public class ControllerEventArgs : EventArgs
	{
		public ControllerEvents ControllerEvent { get; set; }
		public int ControllerID { get; set; }
	}

	// Input event arguments to be passed into the InputEventHandler
	public class InputEventArgs : EventArgs
	{
		public ContolPadButtons ButtonID { get; set; }
		public PlayerIndex ControllerID { get; set; }
	}

	public class KeyboardEventArgs : EventArgs
	{
		public Keys KeyID { get; set; }
	}

	public class MouseEventArgs : EventArgs
	{
		public MouseButtons MouseButtonID { get; set; }
	}

	// The InputEventHandler that passes along all input events to whichever systems registered for the events
    public delegate void ControllerEventHandler(InputManager sender, ControllerEventArgs e);
	public delegate void InputEventHandler(InputManager sender ,InputEventArgs e);
    public delegate void KeyboardEventHandler(InputManager sender, KeyboardEventArgs e);
    public delegate void MouseEventHandler(InputManager sender, MouseEventArgs e);

	// Internal Controller class uses to keep track of all aspects of a controller and its states
	class GamePadDevice
	{
		// Member variables
		public GamePadCapabilities Capabilities { get; internal set; }
		public GamePadType Type { get; internal set; }
		public bool IsConnected { get; internal set; }
		public bool _prevIsConnected;
		public Dictionary<ContolPadButtons, float> PrevButtonStateList { get; set; }
		public Dictionary<ContolPadButtons, float> CurrButtonStateList { get; set; }


		// Member functions
		public GamePadDevice()
		{
			Capabilities = new GamePadCapabilities();
			_prevIsConnected = false;
			PrevButtonStateList = new Dictionary<ContolPadButtons, float>();
			CurrButtonStateList = new Dictionary<ContolPadButtons, float>();

			foreach (ContolPadButtons b in Enum.GetValues(typeof(ContolPadButtons)))
			{
				PrevButtonStateList.Add(b, 0);
				CurrButtonStateList.Add(b, 0);
			}
		}

		/// <summary>
		/// Copies the information from the passed in GamePad state
		/// </summary>
		public void CopyGamePadInfo(PlayerIndex index, GamePadDeadZone deadZone)
		{
			GamePadState currState = GamePad.GetState(index, deadZone);

			foreach (ContolPadButtons b in Enum.GetValues(typeof(ContolPadButtons)))
			{
				PrevButtonStateList[b] = CurrButtonStateList[b];
			}

			Capabilities = GamePad.GetCapabilities(index);
			_prevIsConnected = IsConnected;

			CurrButtonStateList[ContolPadButtons.A] = (float)currState.Buttons.A;
			CurrButtonStateList[ContolPadButtons.B] = (float)currState.Buttons.B;
			CurrButtonStateList[ContolPadButtons.Back] = (float)currState.Buttons.Back;
			CurrButtonStateList[ContolPadButtons.LeftShoulder] = (float)currState.Buttons.LeftShoulder;
			CurrButtonStateList[ContolPadButtons.LeftStick] = (float)currState.Buttons.LeftStick;
			CurrButtonStateList[ContolPadButtons.RightShoulder] = (float)currState.Buttons.RightShoulder;
			CurrButtonStateList[ContolPadButtons.RightStick] = (float)currState.Buttons.RightStick;
			CurrButtonStateList[ContolPadButtons.Start] = (float)currState.Buttons.Start;
			CurrButtonStateList[ContolPadButtons.X] = (float)currState.Buttons.X;
			CurrButtonStateList[ContolPadButtons.Y] = (float)currState.Buttons.Y;

			CurrButtonStateList[ContolPadButtons.DPadDown] = (float)currState.DPad.Down;
			CurrButtonStateList[ContolPadButtons.DPadLeft] = (float)currState.DPad.Left;
			CurrButtonStateList[ContolPadButtons.DPadRight] = (float)currState.DPad.Right;
			CurrButtonStateList[ContolPadButtons.DPadUp] = (float)currState.DPad.Up;

			CurrButtonStateList[ContolPadButtons.LeftThumbStickX] = (float)currState.ThumbSticks.Left.X;
			CurrButtonStateList[ContolPadButtons.LeftThumbStickY] = (float)currState.ThumbSticks.Left.Y;
			CurrButtonStateList[ContolPadButtons.RightThumbStickX] = (float)currState.ThumbSticks.Right.X;
			CurrButtonStateList[ContolPadButtons.RightThumbStickY] = (float)currState.ThumbSticks.Right.Y;

			CurrButtonStateList[ContolPadButtons.LeftTrigger] = (float)currState.Triggers.Left;
			CurrButtonStateList[ContolPadButtons.RightTrigger] = (float)currState.Triggers.Right;

			IsConnected = currState.IsConnected;
		}
	}

	// Internal Keyboard class uses to keep track of all aspects of a keyboard and its states
	class KeyboardDevice
	{
		// Member variables
		public KeyboardState PrevState { get; internal set; }
		public KeyboardState CurrState { get; internal set; }

		// Member functions
		public KeyboardDevice()
		{
			PrevState = new KeyboardState();
			CurrState = new KeyboardState();
		}
	}

	// Internal Mouse class uses to keep track of all aspects of a mouse and its states
	public class MouseDevice
	{
		public Point CurrPos { get; internal set; }
		public Point PrevPos { get; internal set; }
		public Point Delta { get; internal set; }

		public int CurrWheel { get; internal set; }
		public int PrevWheel { get; internal set; }
		public Dictionary<MouseButtons, ButtonState> PrevButtonStateList { get; internal set; }
		public Dictionary<MouseButtons, ButtonState> CurrButtonStateList { get; internal set; }

		internal MouseDevice()
		{
			PrevButtonStateList = new Dictionary<MouseButtons, ButtonState>();
			CurrButtonStateList = new Dictionary<MouseButtons, ButtonState>();

			MouseState currState = Mouse.GetState();
			PrevPos = CurrPos = new Point(currState.X, currState.Y);

			PrevWheel = CurrWheel = currState.ScrollWheelValue;

			foreach (MouseButtons b in Enum.GetValues(typeof(MouseButtons)))
			{
				PrevButtonStateList.Add(b, ButtonState.Released);
				CurrButtonStateList.Add(b, ButtonState.Released);
			}
		}

		internal void CopyMouseInfo()
		{
			MouseState currState = Mouse.GetState();

			foreach (MouseButtons b in Enum.GetValues(typeof(MouseButtons)))
			{
				PrevButtonStateList[b] = CurrButtonStateList[b];
			}

			CurrButtonStateList[MouseButtons.LeftButton] = currState.LeftButton;
			CurrButtonStateList[MouseButtons.MiddleButton] = currState.MiddleButton;
			CurrButtonStateList[MouseButtons.RightButton] = currState.RightButton;
			CurrButtonStateList[MouseButtons.XButton1] = currState.XButton1;
			CurrButtonStateList[MouseButtons.XButton2] = currState.XButton2;

			PrevPos = CurrPos;
			CurrPos = new Point(currState.X, currState.Y);

			PrevWheel = CurrWheel;
			CurrWheel = currState.ScrollWheelValue;
		}

		public void SetCursorPos(int x, int y)
		{
			Mouse.SetPosition(x, y);
			CopyMouseInfo();
		}
	}

	/// <summary>
	/// Handles the updating and processing of all input devices connected to the system
	/// </summary>
	public class InputManager
	{
		/* Member variables */
		GamePadDevice[] _gamePadList = new GamePadDevice[4];
		KeyboardDevice _keyboard = new KeyboardDevice();
		//MouseDevice _mouse = new MouseDevice();
		public GamePadDeadZone DeadZone { get; set; }

		/* Event handlers */
		public event ControllerEventHandler ControllerConnected;
		public event ControllerEventHandler ControllerDisconnected;
		public event InputEventHandler ButtonTriggered;
		public event InputEventHandler ButtonReleased;
		public event KeyboardEventHandler KeyDown;
		public event KeyboardEventHandler KeyReleased;
		public event MouseEventHandler MouseButtonTriggered;
		public event MouseEventHandler MouseButtonReleased;

		/* Properties */
		public MouseDevice Mouse { get; set; }

		/* Member functions */
		public InputManager()
		{
			_gamePadList[0] = new GamePadDevice();
			_gamePadList[1] = new GamePadDevice();
			_gamePadList[2] = new GamePadDevice();
			_gamePadList[3] = new GamePadDevice();
			Mouse = new MouseDevice();

			DeadZone = GamePadDeadZone.IndependentAxes;
		}

		public void GetLeftThumbStickX(int index, ref float currValue, ref float delta)
		{
			//if (currValue != null)
			currValue = _gamePadList[index].CurrButtonStateList[ContolPadButtons.LeftThumbStickX];

			//if (delta != null)
			delta = _gamePadList[index].CurrButtonStateList[ContolPadButtons.LeftThumbStickX] -
					_gamePadList[index].PrevButtonStateList[ContolPadButtons.LeftThumbStickX];
		}

		public void GetLeftThumbStickY(int index, ref float currValue, ref float delta)
		{
			//if (currValue != null)
			currValue = _gamePadList[index].CurrButtonStateList[ContolPadButtons.LeftThumbStickY];

			//if (delta != null)
			delta = _gamePadList[index].CurrButtonStateList[ContolPadButtons.LeftThumbStickY] -
					_gamePadList[index].PrevButtonStateList[ContolPadButtons.LeftThumbStickY];
		}

		public void GetLeftTrigger(int index, ref float currValue, ref float delta)
		{
			//if (currValue != null)
			currValue = _gamePadList[index].CurrButtonStateList[ContolPadButtons.LeftTrigger];

			//if (delta != null)
			delta = _gamePadList[index].CurrButtonStateList[ContolPadButtons.LeftTrigger] -
					_gamePadList[index].PrevButtonStateList[ContolPadButtons.LeftTrigger];
		}

		public void GetRightThumbStickX(int index, ref float currValue, ref float delta)
		{
			//if (currValue != null)
			currValue = _gamePadList[index].CurrButtonStateList[ContolPadButtons.RightThumbStickX];

			//if (delta != null)
			delta = _gamePadList[index].CurrButtonStateList[ContolPadButtons.RightThumbStickX] -
					_gamePadList[index].PrevButtonStateList[ContolPadButtons.RightThumbStickX];
		}

		public void GetRightThumbStickY(int index, ref float currValue, ref float delta)
		{
			//if (currValue != null)
			currValue = _gamePadList[index].CurrButtonStateList[ContolPadButtons.RightThumbStickY];

			//if (delta != null)
			delta = _gamePadList[index].CurrButtonStateList[ContolPadButtons.RightThumbStickY] -
					_gamePadList[index].PrevButtonStateList[ContolPadButtons.RightThumbStickY];
		}

		public void GetRightTrigger(int index, ref float currValue, ref float delta)
		{
			//if (currValue != null)
			currValue = _gamePadList[index].CurrButtonStateList[ContolPadButtons.RightTrigger];

			//if (delta != null)
			delta = _gamePadList[index].CurrButtonStateList[ContolPadButtons.RightTrigger] -
					_gamePadList[index].PrevButtonStateList[ContolPadButtons.RightTrigger];
		}

		public void SetGamePadDeadZoneType(AnalogStickDeadZone flag)
		{
			if (flag == AnalogStickDeadZone.None)
				DeadZone = GamePadDeadZone.None;
			else if (flag == AnalogStickDeadZone.Circular)
				DeadZone = GamePadDeadZone.Circular;
			else
				DeadZone = GamePadDeadZone.IndependentAxes;
		}

		/// <summary>
		/// Updates the previous and current state of all input devices connected to the system
		/// </summary>
		void UpdateInputDeviceStates()
		{

			_gamePadList[0].CopyGamePadInfo(PlayerIndex.One, DeadZone);
			_gamePadList[1].CopyGamePadInfo(PlayerIndex.Two, DeadZone);
			_gamePadList[2].CopyGamePadInfo(PlayerIndex.Three, DeadZone);
			_gamePadList[3].CopyGamePadInfo(PlayerIndex.Four, DeadZone);

			_keyboard.PrevState = _keyboard.CurrState;
			_keyboard.CurrState = Keyboard.GetState();

			Mouse.CopyMouseInfo();
		}

		/// <summary>
		/// Checks all input devices that were connected to the system to make sure they are still connected
		/// </summary>
		void UpdateInputDeviceConnections()
		{
			ControllerEventArgs args = new ControllerEventArgs();

			if (_gamePadList[0]._prevIsConnected == true && _gamePadList[0].IsConnected == false)
			{
				// Send the disconnect message for all who will here it!
				if (ControllerDisconnected != null)
				{
					args.ControllerEvent = ControllerEvents.Disconnected;
					args.ControllerID = 0;
					ControllerDisconnected(this, args);
				}
			}

			if (_gamePadList[0]._prevIsConnected == false && _gamePadList[0].IsConnected == true)
			{
				// Send the disconnect message for all who will here it!
				if (ControllerConnected != null)
				{
					args.ControllerEvent = ControllerEvents.Connected;
					args.ControllerID = 0;
                    ControllerConnected(this, args);
				}
			}

			if (_gamePadList[1]._prevIsConnected == true && _gamePadList[1].IsConnected == false)
			{
				// Send the disconnect message for all who will here it!
				if (ControllerDisconnected != null)
				{
					args.ControllerEvent = ControllerEvents.Disconnected;
					args.ControllerID = 1;
                    ControllerDisconnected(this, args);
				}
			}

			if (_gamePadList[1]._prevIsConnected == false && _gamePadList[1].IsConnected == true)
			{
				// Send the disconnect message for all who will here it!
				if (ControllerConnected != null)
				{
					args.ControllerEvent = ControllerEvents.Connected;
					args.ControllerID = 1;
                    ControllerConnected(this, args);
				}
			}

			if (_gamePadList[2]._prevIsConnected == true && _gamePadList[2].IsConnected == false)
			{
				// Send the disconnect message for all who will here it!
				if (ControllerDisconnected != null)
				{
					args.ControllerEvent = ControllerEvents.Disconnected;
					args.ControllerID = 2;
                    ControllerDisconnected(this, args);
				}
			}

			if (_gamePadList[2]._prevIsConnected == false && _gamePadList[2].IsConnected == true)
			{
				// Send the disconnect message for all who will here it!
				if (ControllerConnected != null)
				{
					args.ControllerEvent = ControllerEvents.Connected;
					args.ControllerID = 2;
                    ControllerConnected(this, args);
				}
			}

			if (_gamePadList[3]._prevIsConnected == true && _gamePadList[3].IsConnected == false)
			{
				// Send the disconnect message for all who will here it!
				if (ControllerDisconnected != null)
				{
					args.ControllerEvent = ControllerEvents.Disconnected;
					args.ControllerID = 3;
                    ControllerDisconnected(this, args);
				}
			}

			if (_gamePadList[3]._prevIsConnected == false && _gamePadList[3].IsConnected == true)
			{
				// Send the disconnect message for all who will here it!
				if (ControllerConnected != null)
				{
					args.ControllerEvent = ControllerEvents.Connected;
					args.ControllerID = 3;
                    ControllerConnected(this, args);
				}
			}


		}

		/// <summary>
		/// Updates the status of all buttons for each input device connected to the system
		/// </summary>
		void ProcessInputDeviceButtonStates()
		{
			InputEventArgs args = new InputEventArgs();

			for (int i = 0; i < 4; i++)
			{
				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.A] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.A] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.A;
						args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.A] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.A] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.A;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.B] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.B] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.B;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.B] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.B] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.B;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.Back] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.Back] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.Back;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.Back] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.Back] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.Back;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.DPadDown] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.DPadDown] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.DPadDown;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.DPadDown] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.DPadDown] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.DPadDown;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.DPadLeft] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.DPadLeft] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.DPadLeft;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.DPadLeft] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.DPadLeft] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.DPadLeft;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.DPadRight] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.DPadRight] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.DPadRight;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.DPadRight] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.DPadRight] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.DPadRight;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.DPadUp] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.DPadUp] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.DPadUp;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.DPadUp] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.DPadUp] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.DPadUp;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.LeftShoulder] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.LeftShoulder] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.LeftShoulder;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.LeftShoulder] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.LeftShoulder] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.LeftShoulder;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.LeftStick] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.LeftStick] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.LeftStick;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.LeftStick] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.LeftStick] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.LeftStick;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.RightShoulder] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.RightShoulder] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.RightShoulder;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.RightShoulder] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.RightShoulder] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.RightShoulder;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.RightStick] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.RightStick] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.RightStick;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.RightStick] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.RightStick] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.RightStick;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.Start] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.Start] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.Start;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.Start] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.Start] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.Start;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.X] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.X] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.X;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.X] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.X] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.X;
                        args.ControllerID = (PlayerIndex)i;

						ButtonReleased(this, args);
					}
				}

				// Detect triggered
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.Y] == 0 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.Y] == 1)
				{
					if (ButtonTriggered != null)
					{
						args.ButtonID = ContolPadButtons.Y;
                        args.ControllerID = (PlayerIndex)i;

						ButtonTriggered(this, args);
					}
				}

				// Detect released
				if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.Y] == 1 &&
					_gamePadList[i].CurrButtonStateList[ContolPadButtons.Y] == 0)
				{
					if (ButtonReleased != null)
					{
						args.ButtonID = ContolPadButtons.Y;
                        args.ControllerID = (PlayerIndex)i;

						ButtonReleased(this, args);
					}
				}

                // Detect triggered
                if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.LeftTrigger] < 0.1f &&
                    _gamePadList[i].CurrButtonStateList[ContolPadButtons.LeftTrigger] > 0.1f)
                {
                    if (ButtonTriggered != null)
                    {
                        args.ButtonID = ContolPadButtons.LeftTrigger;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
                    }
                }

                // Detect released
                if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.LeftTrigger] > 0.1f &&
                    _gamePadList[i].CurrButtonStateList[ContolPadButtons.LeftTrigger] < 0.1f)
                {
                    if (ButtonReleased != null)
                    {
                        args.ButtonID = ContolPadButtons.LeftTrigger;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
                    }
                }
                // Detect triggered
                if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.RightTrigger] < 0.1f &&
                    _gamePadList[i].CurrButtonStateList[ContolPadButtons.RightTrigger] > 0.1f)
                {
                    if (ButtonTriggered != null)
                    {
                        args.ButtonID = ContolPadButtons.RightTrigger;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonTriggered(this, args);
                    }
                }

                // Detect released
                if (_gamePadList[i].PrevButtonStateList[ContolPadButtons.RightTrigger] > 0.1f &&
                    _gamePadList[i].CurrButtonStateList[ContolPadButtons.RightTrigger] < 0.1f)
                {
                    if (ButtonReleased != null)
                    {
                        args.ButtonID = ContolPadButtons.RightTrigger;
                        args.ControllerID = (PlayerIndex)i;

                        ButtonReleased(this, args);
                    }
                }
			}

			KeyboardEventArgs keyArgs = new KeyboardEventArgs();

			foreach (Keys key in Enum.GetValues(typeof(Keys)))
			{
				if (_keyboard.PrevState.IsKeyUp(key) && _keyboard.CurrState.IsKeyDown(key))
				{
					if (KeyDown != null)
					{
						keyArgs.KeyID = key;
						KeyDown(this, keyArgs);
					}
				}

				if (_keyboard.PrevState.IsKeyDown(key) && _keyboard.CurrState.IsKeyUp(key))
				{
					if (KeyReleased != null)
					{
						keyArgs.KeyID = key;
						KeyReleased(this, keyArgs);
					}
				}
			}

			MouseEventArgs mouseArgs = new MouseEventArgs();

			// Detect triggered
			if (Mouse.PrevButtonStateList[MouseButtons.LeftButton] == ButtonState.Released &&
				Mouse.CurrButtonStateList[MouseButtons.LeftButton] == ButtonState.Pressed)
			{
				if (MouseButtonTriggered != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.LeftButton;
					MouseButtonTriggered(this, mouseArgs);
				}
			}

			// Detect released
			if (Mouse.PrevButtonStateList[MouseButtons.LeftButton] == ButtonState.Pressed &&
				Mouse.CurrButtonStateList[MouseButtons.LeftButton] == ButtonState.Released)
			{
				if (MouseButtonReleased != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.LeftButton;
					MouseButtonReleased(this, mouseArgs);
				}
			}

			// Detect triggered
			if (Mouse.PrevButtonStateList[MouseButtons.MiddleButton] == ButtonState.Released &&
				Mouse.CurrButtonStateList[MouseButtons.MiddleButton] == ButtonState.Pressed)
			{
				if (MouseButtonTriggered != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.MiddleButton;
					MouseButtonTriggered(this, mouseArgs);
				}
			}

			// Detect released
			if (Mouse.PrevButtonStateList[MouseButtons.MiddleButton] == ButtonState.Pressed &&
				Mouse.CurrButtonStateList[MouseButtons.MiddleButton] == ButtonState.Released)
			{
				if (MouseButtonReleased != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.MiddleButton;
					MouseButtonReleased(this, mouseArgs);
				}
			}

			// Detect triggered
			if (Mouse.PrevButtonStateList[MouseButtons.RightButton] == ButtonState.Released &&
				Mouse.CurrButtonStateList[MouseButtons.RightButton] == ButtonState.Pressed)
			{
				if (MouseButtonTriggered != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.RightButton;
					MouseButtonTriggered(this, mouseArgs);
				}
			}

			// Detect released
			if (Mouse.PrevButtonStateList[MouseButtons.RightButton] == ButtonState.Pressed &&
				Mouse.CurrButtonStateList[MouseButtons.RightButton] == ButtonState.Released)
			{
				if (MouseButtonReleased != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.RightButton;
					MouseButtonReleased(this, mouseArgs);
				}
			}

			// Detect triggered
			if (Mouse.PrevButtonStateList[MouseButtons.XButton1] == ButtonState.Released &&
				Mouse.CurrButtonStateList[MouseButtons.XButton1] == ButtonState.Pressed)
			{
				if (MouseButtonTriggered != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.XButton1;
					MouseButtonTriggered(this, mouseArgs);
				}
			}

			// Detect released
			if (Mouse.PrevButtonStateList[MouseButtons.XButton1] == ButtonState.Pressed &&
				Mouse.CurrButtonStateList[MouseButtons.XButton1] == ButtonState.Released)
			{
				if (MouseButtonReleased != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.XButton1;
					MouseButtonReleased(this, mouseArgs);
				}
			}

			// Detect triggered
			if (Mouse.PrevButtonStateList[MouseButtons.XButton2] == ButtonState.Released &&
				Mouse.CurrButtonStateList[MouseButtons.XButton2] == ButtonState.Pressed)
			{
				if (MouseButtonTriggered != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.XButton2;
					MouseButtonTriggered(this, mouseArgs);
				}
			}

			// Detect released
			if (Mouse.PrevButtonStateList[MouseButtons.XButton2] == ButtonState.Pressed &&
				Mouse.CurrButtonStateList[MouseButtons.XButton2] == ButtonState.Released)
			{
				if (MouseButtonReleased != null)
				{
					mouseArgs.MouseButtonID = MouseButtons.XButton2;
					MouseButtonReleased(this, mouseArgs);
				}
			}
		}

		/// <summary>
		/// Updates connection status and button states for all input devices connected to the system
		/// </summary>
		public void Update()
		{
			// Check for new input devices and disconnects
			UpdateInputDeviceConnections();
			UpdateInputDeviceStates();

			// Keep button states up-to-date
			ProcessInputDeviceButtonStates();
		}
	}
}