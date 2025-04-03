using UnityEngine;

/// <summary>
/// A ScriptableObject that stores a boolean value.
/// Can be created as an asset in the project and referenced by multiple scripts.
/// </summary>
[CreateAssetMenu(fileName = "AudioClipData", menuName = "Data/AudioClipData")]
public class AudioClipData : ScriptableObjectBase
{
	/// <summary>
	/// The boolean value stored in this ScriptableObject.
	/// </summary>
	[SerializeField] private AudioClip value;

	/// <summary>
	/// Public property to access or modify the stored boolean value.
	/// </summary>
	public AudioClip Value { get => value; set => this.value = value; }

	/// <summary>
	/// Implicit conversion operator that allows using this ScriptableObject directly as a bool.
	/// Example usage: bool isActive = myBoolData; // instead of bool isActive = myBoolData.value;
	/// Returns false if the variable is null.
	/// </summary>
	/// <param name="variable">The BoolData object to convert</param>
	public static implicit operator AudioClip(AudioClipData variable)
	{
		return variable.value;
	}
}