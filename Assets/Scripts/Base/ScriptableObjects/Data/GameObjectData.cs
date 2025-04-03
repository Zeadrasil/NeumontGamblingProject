using UnityEngine;

/// <summary>
/// A ScriptableObject that stores a GameObject reference.
/// Can be created as an asset in the project and referenced by multiple scripts.
/// </summary>
[CreateAssetMenu(fileName = "GameObjectData", menuName = "Data/GameObjectData")]
public class GameObjectData : ScriptableObjectBase
{
	/// <summary>
	/// The GameObject reference stored in this ScriptableObject.
	/// Note: Since GameObject instances are scene-specific, this will typically
	/// be assigned at runtime rather than in the asset itself.
	/// </summary>
	[SerializeField] private GameObject value;

	/// <summary>
	/// Public property to access or modify the stored GameObject reference.
	/// </summary>
	public GameObject Value { get => value; set => this.value = value; }

	/// <summary>
	/// Implicit conversion operator that allows using this ScriptableObject directly as a GameObject.
	/// Example usage: GameObject obj = myGameObjectData; // instead of GameObject obj = myGameObjectData.value;
	/// Returns null if the variable is null or its value is null.
	/// </summary>
	/// <param name="variable">The GameObjectData object to convert</param>
	public static implicit operator GameObject(GameObjectData variable)
	{
		return variable?.value ?? null;
	}
}
