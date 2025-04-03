using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// EventChannel<T> - A generic implementation of the observer pattern using ScriptableObject.
/// Allows broadcasting events with parameters of type T to multiple listeners.
/// Use this class to create strongly-typed event channels for your game systems.
/// </summary>
[CreateAssetMenu(menuName = "Events/Event Channel")]
public abstract class EventChannel<T> : ScriptableObjectBase
{
	/// <summary>
	/// Unity Action that holds references to all subscribed methods.
	/// Allows dynamically calling multiple functions with a parameter of type T when the event is raised.
	/// </summary>
	public UnityAction<T> OnEventRaised;

	/// <summary>
	/// Raises the event with a parameter of type T.
	/// </summary>
	/// <param name="value">The value to pass to all listeners.</param>
	public void RaiseEvent(T value)
	{
		OnEventRaised?.Invoke(value);
	}

	/// <summary>
	/// Subscribes a listener to the event.
	/// </summary>
	/// <param name="listener">The method that will be called when the event is raised.</param>
	public void Subscribe(UnityAction<T> listener)
	{
		if (listener != null) OnEventRaised += listener;
	}

	/// <summary>
	/// Unsubscribes a listener from the event.
	/// </summary>
	/// <param name="listener">The method that should no longer be called when the event is raised.</param>
	public void Unsubscribe(UnityAction<T> listener)
	{
		if (listener != null) OnEventRaised -= listener;
	}
}

/// <summary>
/// Empty struct used for parameter-less events to maintain generic type safety.
/// </summary>
public readonly struct Empty { }

/// <summary>
/// EventChannel - A specialized version of EventChannel<T> for parameter-less events.
/// Uses the Empty struct as a placeholder type to maintain the generic structure.
/// Create instances of this class when you need events that don't pass any data.
/// </summary>
[CreateAssetMenu(menuName = "Events/EventChannel")]
public class EventChannel : EventChannel<Empty>
{
	/// <summary>
	/// Raises the event without requiring an Empty parameter.
	/// A convenience method that creates an Empty instance internally.
	/// </summary>
	public void RaiseEvent()
	{
		base.RaiseEvent(new Empty());
	}
}