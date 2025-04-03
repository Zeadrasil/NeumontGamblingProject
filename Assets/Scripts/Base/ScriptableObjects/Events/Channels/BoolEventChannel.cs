using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// BoolEvent - A simple observer pattern implementation using ScriptableObject.
/// Allows broadcasting boolean values to multiple listeners.
/// </summary>
[CreateAssetMenu(menuName = "Events/Bool Event Channel")]
public class BoolEventChannel : EventChannel<bool>
{
	//
}