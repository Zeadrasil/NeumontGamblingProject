using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// GameObjectEvent - A simple observer pattern implementation using ScriptableObject.
/// Allows broadcasting GameObject references to multiple listeners.
/// </summary>
[CreateAssetMenu(menuName = "Events/GameObject Event Channel")]
public class GameObjectEventChannel : EventChannel<GameObject>
{
	//
}