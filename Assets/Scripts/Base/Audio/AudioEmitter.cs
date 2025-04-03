using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles audio playback from a single source in the game.
/// Works with AudioConfiguration scriptable objects to apply consistent audio settings.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioEmitter : MonoBehaviour
{
	/// <summary>
	/// The actual Unity AudioSource component that this class wraps and manages.
	/// </summary>
	private AudioSource audioSource;

	/// <summary>
	/// Event that notifies listeners (typically AudioManager) when a non-looping audio clip has finished playing.
	/// </summary>
	public event UnityAction<AudioEmitter> onAudioFinishedPlaying;

	/// <summary>
	/// Initialize the AudioEmitter by getting the AudioSource component and ensuring it doesn't play automatically.
	/// </summary>
	private void Awake()
	{
		audioSource = this.GetComponent<AudioSource>();
		audioSource.playOnAwake = false; // We want to control playback manually
	}

	/// <summary>
	/// Plays an audio clip with the specified settings.
	/// </summary>
	/// <param name="clip">The audio clip to play</param>
	/// <param name="settings">The configuration settings to apply to the audio source</param>
	/// <param name="loop">Whether the clip should loop continuously</param>
	/// <param name="position">World position where the sound should play from (default: current transform position)</param>
	public void PlayAudioClip(AudioClip clip, AudioConfigurationData settings, bool loop, Vector3 position = default)
	{
		// Configure the audio source
		audioSource.clip = clip;
		settings.ApplyTo(audioSource); // Apply all settings from the configuration
		audioSource.transform.position = position;
		audioSource.loop = loop;
		audioSource.time = 0f; // Start from the beginning of the clip
		audioSource.Play();

		// If not looping, set up a coroutine to detect when the clip finishes
		if (!loop)
		{
			StartCoroutine(FinishedPlaying(clip.length));
		}
	}

	/// <summary>
	/// Used to check which music track is being played.
	/// </summary>
	/// <returns>The currently assigned AudioClip, or null if nothing is assigned</returns>
	public AudioClip GetClip()
	{
		return audioSource.clip;
	}

	/// <summary>
	/// Used when the game is unpaused, to pick up SFX from where they left.
	/// </summary>
	public void Resume()
	{
		audioSource.Play();
	}

	/// <summary>
	/// Used when the game is paused.
	/// </summary>
	public void Pause()
	{
		audioSource.Pause();
	}

	/// <summary>
	/// Immediately stops the audio playback.
	/// </summary>
	public void Stop()
	{
		audioSource.Stop();
	}

	/// <summary>
	/// Allows a looping sound to finish its current playthrough and then stop.
	/// Will trigger the onAudioFinishedPlaying event when complete.
	/// </summary>
	public void Finish()
	{
		if (audioSource.loop)
		{
			audioSource.loop = false; // Disable looping
			float timeRemaining = audioSource.clip.length - audioSource.time; // Calculate remaining time
			StartCoroutine(FinishedPlaying(timeRemaining));
		}
	}

	/// <summary>
	/// Checks if the audio source is currently playing.
	/// </summary>
	/// <returns>True if audio is playing, false otherwise</returns>
	public bool IsPlaying()
	{
		return audioSource.isPlaying;
	}

	/// <summary>
	/// Checks if the audio source is set to loop playback.
	/// </summary>
	/// <returns>True if audio is set to loop, false otherwise</returns>
	public bool IsLooping()
	{
		return audioSource.loop;
	}

	/// <summary>
	/// Coroutine that waits for the specified clip length and then fires the completion event.
	/// </summary>
	/// <param name="clipLength">Duration to wait before considering the clip finished</param>
	/// <returns>IEnumerator for Unity's coroutine system</returns>
	IEnumerator FinishedPlaying(float clipLength)
	{
		yield return new WaitForSeconds(clipLength);

		NotifyBeingDone();
	}

	/// <summary>
	/// Invokes the onAudioFinishedPlaying event to notify listeners (typically the AudioManager)
	/// that this emitter has finished playing its assigned clip.
	/// </summary>
	private void NotifyBeingDone()
	{
		onAudioFinishedPlaying?.Invoke(this); // The AudioManager will pick this up
	}
}