using System.Collections.Generic;
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
[CreateAssetMenu(menuName = "Events/Event Channel")]
public class EventChannel : EventChannel<Empty>
{
	// Dictionary to keep track of the wrapped actions
	private Dictionary<UnityAction, UnityAction<Empty>> wrappedActions = new Dictionary<UnityAction, UnityAction<Empty>>();

	public void RaiseEvent()
	{
		base.RaiseEvent(new Empty());
	}

	// Subscribe with a parameterless action
	public void Subscribe(UnityAction listener)
	{
		if (listener != null)
		{
			// Create a wrapper that takes Empty but calls the parameterless action
			UnityAction<Empty> wrapper = (_) => listener();

			// Store the mapping
			wrappedActions[listener] = wrapper;

			// Subscribe the wrapper
			base.Subscribe(wrapper);
		}
	}

	// Unsubscribe the parameterless action
	public void Unsubscribe(UnityAction listener)
	{
		if (listener != null && wrappedActions.TryGetValue(listener, out var wrapper))
		{
			// Unsubscribe the wrapper
			base.Unsubscribe(wrapper);

			// Remove from dictionary
			wrappedActions.Remove(listener);
		}
	}
}