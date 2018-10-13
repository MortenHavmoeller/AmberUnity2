using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using UnityEngine;

namespace AmberInput
{
	public interface IInputSubscriber
	{
		void OnInput(InputEvent evt);
	}

	public class ExactInput : MonoBehaviour
	{
		public enum TimeContext
		{
			Immediate,
			Update,
			FixedUpdate
		}
		public enum InputContext
		{
			Default,
			FreeCamera // etc.
		}

		private static ExactInput _singleton;

		private TimeContext _timeContext;
		private InputContext _inputContext;

		private Keyboard _keyboard;
		private ContextualizedSubscribers _subscribers;

		private float _trueUpdateTime;
		private float _trueFixedUpdateTime;

		// may be null
		public static ExactInput GetSingleton()
		{
			return _singleton;
		}

		public static void Subscribe(InputContext context, TimeContext timeContext, IInputSubscriber subscriber)
		{
			_singleton._subscribers.AddSubscriber(context, timeContext, subscriber);
		}

		public static void SetContext(InputContext newContext)
		{
			_singleton._inputContext = newContext;
		}

		private void Awake()
		{
			if (_singleton != null)
			{
				Debug.LogError("more than one instance of ExactInput");
				return;
			}
			_singleton = this;

			_inputContext = InputContext.Default;
			_timeContext = TimeContext.Update;

			_keyboard = new Keyboard();
			_subscribers = new ContextualizedSubscribers();

			// start input updater thread
			keepThreadAlive = true;
			threadReport = new List<string>(5000);
			threadTimeReport = new List<int>(5000);
			Thread t = new Thread(new ThreadStart(MyThreadMethod));
			t.Start();
		}

		private void OnDestroy()
		{
			keepThreadAlive = false;
			string log = "";
			for (int i = 0; i < threadReport.Count; i++)
			{
				log += threadReport[i] + "\n";
			}
			Debug.Log("THREAD REPORT\n" + log);

			log = "";
			for (int i = 1; i < threadTimeReport.Count; i++)
			{
				if (threadTimeReport[i] > threadTimeReport[i - 1] && threadTimeReport[i] - threadTimeReport[i - 1] > 6)
				{

					log += "iteration" + (i + 1) + "; " + (threadTimeReport[i] - threadTimeReport[i - 1]) + "\n";
				}
			}
			Debug.Log("THREAD TIME REPORT\n" + log);
		}

		bool keepThreadAlive;
		int threadIteration = 0;
		List<string> threadReport;
		List<int> threadTimeReport;
		private void MyThreadMethod()
		{
			while (keepThreadAlive)
			{
				threadIteration++;
				ThreadReport(threadIteration);
				Thread.Sleep(5);
			}
		}

		private void ThreadReport(int iteration)
		{
			threadReport.Add("thread iteration " + iteration + ", system time " + DateTime.Now.Millisecond);
			threadTimeReport.Add(DateTime.Now.Millisecond);
		}
		

		private void HandleKeyboardEvent(Event e, float exactTime)
		{
			throw new NotImplementedException();
		}

		private void Update()
		{
			_timeContext = TimeContext.Update;
			TransmitInputToSubscribers(Time.unscaledTime, Time.unscaledDeltaTime);
		}

		private void FixedUpdate()
		{
			_timeContext = TimeContext.FixedUpdate;
			TransmitInputToSubscribers(Time.fixedUnscaledTime, Time.fixedUnscaledDeltaTime);

			_keyboard.CleanKeyboardRecord();
		}

		private void TransmitInputToSubscribers(float frameStartTime, float deltaTime)
		{
			throw new NotImplementedException();
		}

		#region internal_classes
		private class ContextualizedSubscribers
		{
			private Dictionary<InputContext, List<IInputSubscriber>[]> _subscribers;

			public ContextualizedSubscribers()
			{
				_subscribers = new Dictionary<InputContext, List<IInputSubscriber>[]>();
			}

			public void AddSubscriber(InputContext context, TimeContext timeContext, IInputSubscriber subscriber)
			{
				List<IInputSubscriber>[] subscriberArray;
				if (_subscribers.TryGetValue(context, out subscriberArray))
				{
					subscriberArray[(int)timeContext].Add(subscriber);
					return;
				}

				// if it doesn't exist already, initialize and add the array
				subscriberArray = new List<IInputSubscriber>[Enum.GetValues(typeof(TimeContext)).Length];
				for (int i = 0; i < subscriberArray.Length; i++)
				{
					subscriberArray[i] = new List<IInputSubscriber>(1);
				}
				subscriberArray[(int)timeContext].Add(subscriber);
				_subscribers.Add(context, subscriberArray);
			}

			public void RemoveSubscriber(IInputSubscriber subscriber)
			{
				throw new NotImplementedException();
			}

			public bool HasContext(InputContext context)
			{
				return _subscribers.ContainsKey(context);
			}

			public bool HasContext(InputContext context, TimeContext timeContext)
			{
				List<IInputSubscriber>[] subscriberArray;
				if (_subscribers.TryGetValue(context, out subscriberArray))
				{
					return subscriberArray[(int)timeContext].Count > 0;
				}
				return false;
			}

			public bool TryGetSubscribers(InputContext context, TimeContext timeContext, out List<IInputSubscriber> subscribers)
			{
				List<IInputSubscriber>[] subscriberArray;
				if (_subscribers.TryGetValue(context, out subscriberArray))
				{
					subscribers = subscriberArray[(int)timeContext];
					if (subscribers.Count > 0)
					{
						return true;
					}
				}

				subscribers = null;
				return false;
			}
		}

		private class Keyboard
		{
			private const float KEY_TIMEOUT = 1f;
			private List<Keypress> _keyRecord;

			public Keyboard()
			{
				_keyRecord = new List<Keypress>(10);
			}

			public void SetKeyPressed(KeyCode key, float time)
			{
				Keypress keypress;
				// is the key already pressed?
				if (TryFindKeypress(key, out keypress))
				{
					return;
				}

				_keyRecord.Add(new Keypress(key, time));
				Debug.LogWarning("Pressing key " + key + " at time " + time + " (key record count " + _keyRecord.Count + ")");
			}

			public void SetKeyReleased(KeyCode key, float time)
			{
				Keypress keypress;
				if (TryFindKeypress(key, out keypress))
				{
					keypress.Release(time);
					return;
				}

				Debug.LogError("trying to release a key that has not been pressed");
			}

			public bool GetKeyPressed(KeyCode key)
			{
				Keypress keypress;
				return TryFindKeypress(key, out keypress);
			}

			public ReadOnlyCollection<Keypress> GetKeyboardRecord()
			{
				return _keyRecord.AsReadOnly();
			}

			public void TryConsumeKeypress(Keypress keypress)
			{
				if (keypress.IsReleased)
				{
					_keyRecord.Remove(keypress);
				}
			}

			public void CleanKeyboardRecord()
			{
				for (int i = 0; i < _keyRecord.Count;)
				{
					if (_keyRecord[i].IsReleased)
					{
						// remove any key that's been released but not consumed, after KEY_TIMEOUT has passed
						if (_keyRecord[i].ReleaseTime + KEY_TIMEOUT < Time.realtimeSinceStartup)
						{
							_keyRecord.RemoveAt(i);
							continue;
						}
					}
					i++;
				}
			}

			private bool TryFindKeypress(KeyCode key, out Keypress result)
			{
				for (int i = 0; i < _keyRecord.Count; i++)
				{
					if (_keyRecord[i].IsReleased)
						continue;

					if (_keyRecord[i].Key == key)
					{
						result = _keyRecord[i];
						return true;
					}
				}
				result = null;
				return false;
			}

			public class Keypress
			{
				private KeyCode _keyCode;
				public KeyCode Key { get { return _keyCode; } }
				private float _time;
				public float PressTime { get { return _time; } }
				private float _releaseTime;
				public float ReleaseTime { get { return _releaseTime; } }
				public bool IsReleased { get { return _releaseTime >= 0f; } }

				public Keypress(KeyCode keyCode, float time)
				{
					_keyCode = keyCode;
					_time = time;
					_releaseTime = -1;
				}

				public void Release(float time)
				{
					_releaseTime = time;
				}
			}
		}

		private class Mouse
		{
			bool mouse0Pressed, mouse1Pressed, mouse2Pressed;
			Vector2 delta;

			// update mouse buttons. returns true if the button state changed
			public bool CheckButton(int btnNumber, bool isCurrentlyPressed)
			{
				switch (btnNumber)
				{
					case 0:
						if (mouse0Pressed != isCurrentlyPressed)
						{
							mouse0Pressed = isCurrentlyPressed;
							return true;
						}
						return false;
					case 1:
						if (mouse1Pressed != isCurrentlyPressed)
						{
							mouse1Pressed = isCurrentlyPressed;
							return true;
						}
						return false;
					case 2:
						if (mouse2Pressed != isCurrentlyPressed)
						{
							mouse2Pressed = isCurrentlyPressed;
							return true;
						}
						return false;
					default:
						Debug.Log("pressing a mouse button that is not implemented");
						return false;
				}
			}
		}
		#endregion
	}

	public class InputEvent
	{
		private readonly float _activationTime;
		public float ActivationTime { get { return _activationTime; } }

		public float deltaTime;

		public KeyCode keyCode;
		public EventType type;

		private bool _used;
		public bool Used { get { return _used; } }

		public bool HasSetDeltaTime { get { return deltaTime >= 0f; } }

		public InputEvent(float startTime, float endTime)
		{
			_activationTime = startTime;
			deltaTime = endTime - _activationTime;
		}

		public InputEvent(float startTime)
		{
			_activationTime = startTime;
			deltaTime = -1;
		}

		public void Use()
		{
			_used = true;
		}
	}
}
