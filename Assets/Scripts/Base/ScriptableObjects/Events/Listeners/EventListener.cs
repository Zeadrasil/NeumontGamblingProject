using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// Generic EventListener that connects EventChannels to UnityEvents.
/// This component listens to an EventChannel of type T and forwards events to a UnityEvent.
/// </summary>
/// <typeparam name="T">The type of data this listener will handle</typeparam>
public abstract class EventListener<T> : MonoBehaviour
{
	[Tooltip("The event channel this listener will subscribe to")]
	[SerializeField] private EventChannel<T> eventChannel;

	[Tooltip("The UnityEvent that will be invoked when the event channel raises an event")]
	[SerializeField] private UnityEvent<T> unityEvent;

	protected virtual void OnEnable()
	{
		if (eventChannel != null)
		{
			// Subscribe to the event channel
			eventChannel.Subscribe(OnEventRaised);
		}
	}

	protected virtual void OnDisable()
	{
		if (eventChannel != null)
		{
			// Unsubscribe from the event channel
			eventChannel.Unsubscribe(OnEventRaised);
		}
	}

	/// <summary>
	/// Called when the event channel raises an event. Forwards the value to the UnityEvent.
	/// </summary>
	/// <param name="value">The value passed by the event channel</param>
	private void OnEventRaised(T value)
	{
		Raise(value);
	}

	/// <summary>
	/// Raises the UnityEvent with the provided value.
	/// This can also be called directly to trigger the event manually.
	/// </summary>
	/// <param name="value">The value to pass to the UnityEvent</param>
	public void Raise(T value)
	{
		unityEvent?.Invoke(value);
	}
}

/// <summary>
/// Concrete implementation of EventListener for parameterless events.
/// This simplifies the use of EventListener for cases where no data needs to be passed.
/// </summary>
public class EventListener : EventListener<Empty>
{
	[Tooltip("Simple UnityEvent with no parameters")]
	[SerializeField] private UnityEvent simpleEvent;

	/// <summary>
	/// Overridden to allow for calling a parameterless UnityEvent
	/// </summary>
	/// <param name="value">The Empty value (ignored)</param>
	public new void Raise(Empty value)
	{
		base.Raise(value);
		simpleEvent?.Invoke();
	}

	/// <summary>
	/// Convenience method to raise the event without needing to provide an Empty parameter
	/// </summary>
	public void Raise()
	{
		Raise(new Empty());
	}
}