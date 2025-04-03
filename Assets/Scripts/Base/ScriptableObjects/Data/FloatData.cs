// Required for ScriptableObject, SerializeField, and other Unity functionality
using UnityEngine;

// Attribute that allows creating instances of this ScriptableObject through the Unity menu
// Creates a menu item at "Assets > Create > Data > IntData" with default filename "IntData"
[CreateAssetMenu(fileName = "FloatData", menuName = "Data/FloatData")]
public class FloatData : ScriptableObjectBase
{
	// The main integer value stored in this ScriptableObject
	// SerializeField ensures it's editable in the Inspector despite being private
	[SerializeField] float value;

	// Public property to get/set the value
	public float Value { get => value; set => this.value = value; }

	// Implicit conversion operator allows using this ScriptableObject directly as an int
	// Example usage: int x = myIntData; // instead of int x = myIntData.value;
	// The null-coalescing operator (??) returns 0 if variable is null
	public static implicit operator float(FloatData variable)
	{
		return variable?.value ?? 0;
	}
}