using UnityEngine;

/// <summary>
/// A ScriptableObject that stores a string value.
/// Can be created as an asset in the project and referenced by multiple scripts.
/// </summary>
[CreateAssetMenu(fileName = "StringData", menuName = "Data/StringData")]
public class StringData : ScriptableObjectBase
{
	/// <summary>
	/// The string value stored in this ScriptableObject.
	/// </summary>
	[SerializeField] private string value;

	/// <summary>
	/// Public property to access or modify the stored string value.
	/// </summary>
	public string Value { get => value; set => this.value = value; }

	/// <summary>
	/// Implicit conversion operator that allows using this ScriptableObject directly as a string.
	/// Example usage: string name = myStringData; // instead of string name = myStringData.value;
	/// Returns an empty string if the variable is null.
	/// </summary>
	/// <param name="variable">The StringData object to convert</param>
	public static implicit operator string(StringData variable)
	{
		return variable?.value ?? "";
	}
}