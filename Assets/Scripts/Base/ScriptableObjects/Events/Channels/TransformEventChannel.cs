using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// TransformEvent - A simple observer pattern implementation using ScriptableObject.
/// Allows broadcasting Transform references to multiple listeners.
/// </summary>
[CreateAssetMenu(menuName = "Events/Transform Event Channel")]
public class TransformEventChannel : EventChannel<Transform>
{
	//
}