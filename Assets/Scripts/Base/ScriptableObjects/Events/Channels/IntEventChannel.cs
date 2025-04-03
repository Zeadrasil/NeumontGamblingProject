using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// IntEvent - A simple observer pattern implementation using ScriptableObject.
/// Allows broadcasting integer values to multiple listeners.
/// </summary>
[CreateAssetMenu(menuName = "Events/Int Event Channel")]
public class IntEventChannel : EventChannel<int>
{
	//
}