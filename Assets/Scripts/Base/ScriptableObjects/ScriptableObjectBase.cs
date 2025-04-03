// Required Unity namespace for ScriptableObject functionality
using UnityEngine;

// Base class that other ScriptableObjects can inherit from
// Provides common functionality like description field
public class ScriptableObjectBase : ScriptableObject
{
	// Description field that appears as a multi-line text area in the Inspector
	// Can be used to document the purpose of the ScriptableObject instance
	[SerializeField, TextArea] string description;
}