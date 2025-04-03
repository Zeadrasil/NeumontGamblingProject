using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// FloatEvent - A simple observer pattern implementation using ScriptableObject.
/// Allows broadcasting float values to multiple listeners.
/// </summary>
[CreateAssetMenu(menuName = "Events/Float Event Channel")]
public class FloatEventChannel : EventChannel<float>
{
	//
}