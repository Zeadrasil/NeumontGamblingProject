using UnityEngine;

/// <summary>
/// A ScriptableObject that stores a Transform reference.
/// Can be created as an asset in the project and referenced by multiple scripts.
/// </summary>
[CreateAssetMenu(fileName = "TransformData", menuName = "Data/TransformData")]
public class TransformData : ScriptableObjectBase
{
	/// <summary>
	/// The Transform reference stored in this ScriptableObject.
	/// Note: Since Transform components are scene-specific, this will typically
	/// be assigned at runtime rather than in the asset itself.
	/// </summary>
	[SerializeField] private Transform value;

	/// <summary>
	/// Public property to access or modify the stored Transform reference.
	/// </summary>
	public Transform Value { get => value; set => this.value = value; }

	/// <summary>
	/// Implicit conversion operator that allows using this ScriptableObject directly as a Transform.
	/// Example usage: Transform target = myTransformData; // instead of Transform target = myTransformData.value;
	/// Returns null if the variable is null or its value is null.
	/// </summary>
	/// <param name="variable">The TransformData object to convert</param>
	public static implicit operator Transform(TransformData variable)
	{
		return variable?.value ?? null;
	}
}
