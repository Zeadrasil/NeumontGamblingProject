using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Scriptable Object that handles audio configuration settings for different types of sounds in the game.
/// Allows for consistent audio settings that can be reused across multiple audio sources.
/// </summary>
[CreateAssetMenu(fileName = "AudioConfigurationData", menuName = "Audio/Audio Configuration")]
public class AudioConfigurationData : ScriptableObject
{
	/// <summary>
	/// Defines the category of audio this configuration applies to.
	/// </summary>
	public enum Type
	{
		Sfx,    // Sound effects
		Music   // Background music
	}

	/// <summary>
	/// Defines the playback priority of the audio.
	/// Lower values indicate higher priority.
	/// Unity's AudioSource priority ranges from 0 (highest) to 256 (lowest).
	/// </summary>
	public enum Priority
	{
		Highest = 0,    // Will almost never be culled
		High = 64,      // Very unlikely to be culled
		Standard = 128, // Default priority
		Low = 194,      // More likely to be culled when many sounds play
		VeryLow = 256,  // Will be culled first when many sounds play
	}

	/// <summary>
	/// The category this audio belongs to.
	/// </summary>
	[Tooltip("Category of sound - affects how it's processed in the audio system")]
	public Type type = Type.Sfx;

	/// <summary>
	/// The playback priority of this audio.
	/// </summary>
	[Tooltip("Determines which sounds get culled first when too many are playing")]
	public Priority priority = Priority.Standard;

	/// <summary>
	/// The mixer group this audio should be routed through.
	/// Useful for applying effects or controlling volume of groups of sounds.
	/// </summary>
	[Tooltip("Audio Mixer Group to route this sound through - controls effects and volume grouping")]
	public AudioMixerGroup audioMixerGroup = null;

	/// <summary>
	/// Base volume of the audio. Range 0 (silent) to 1 (full volume).
	/// </summary>
	[Tooltip("Base volume level from 0 (silent) to 1 (full volume)")]
	[Range(0, 1)] public float volume = 1;

	/// <summary>
	/// Random volume variation to add or subtract from the base volume.
	/// Creates natural variation between sound instances.
	/// </summary>
	[Tooltip("Random volume variation (±) applied to each sound instance for natural variety")]
	[Range(0, 0.2f)] public float volumeRandom = 0;

	/// <summary>
	/// Base pitch adjustment in semitones. Range -24 (two octaves down) to 24 (two octaves up).
	/// 0 represents the original pitch.
	/// </summary>
	[Tooltip("Pitch adjustment in semitones: 0=normal, negative=lower, positive=higher")]
	[Range(-24, 24)] public float pitch = 0;

	/// <summary>
	/// Random pitch variation in semitones.
	/// Creates natural variation between sound instances.
	/// </summary>
	[Tooltip("Random pitch variation (±) in semitones for more natural sound repetition")]
	[Range(0, 12)] public float pitchRandom = 0;

	/// <summary>
	/// Controls how much the audio is affected by 3D positioning.
	/// 0 = 2D (no positioning), 1 = fully 3D positioned.
	/// </summary>
	[Tooltip("How 3D the sound is: 0=2D (no positioning), 1=fully 3D positioned")]
	[Range(0, 1)] public float spatialBlend = 1;

	/// <summary>
	/// Defines how the volume attenuates with distance from the listener.
	/// </summary>
	[Tooltip("How volume decreases with distance (Logarithmic is more realistic)")]
	public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

	/// <summary>
	/// The distance at which the sound starts to attenuate.
	/// </summary>
	[Tooltip("Distance (in units) at which the sound begins to fade")]
	[Range(0.01f, 100)] public float minDistance = 0.1f;

	/// <summary>
	/// The distance at which the sound stops attenuating.
	/// </summary>
	[Tooltip("Maximum distance (in units) at which the sound can be heard")]
	[Range(5, 100)] public float maxDistance = 50;

	/// <summary>
	/// Applies all configuration settings to the given AudioSource component.
	/// </summary>
	/// <param name="audioSource">The AudioSource to configure</param>
	public void ApplyTo(AudioSource audioSource)
	{
		// Apply mixer group
		audioSource.outputAudioMixerGroup = this.audioMixerGroup;

		// Apply volume and pitch settings with randomization
		audioSource.priority = (int)this.priority;
		audioSource.volume = this.volume + (Random.Range(-this.volumeRandom, this.volumeRandom));
		audioSource.pitch = AudioManager.SemitonesToPitch(this.pitch + (Random.Range(-this.pitchRandom, this.pitchRandom)));

		//// Apply spatial settings
		audioSource.spatialBlend = this.spatialBlend;
		audioSource.rolloffMode = this.rolloffMode;
		audioSource.minDistance = this.minDistance;
		audioSource.maxDistance = this.maxDistance;
	}
}