using System.Collections;
using UnityEngine;

/// <summary>
/// MonoBehaviour that connects the data-focused AudioCueData with the runtime event system.
/// Acts as a trigger point for playing audio within the scene.
/// </summary>
public class AudioCue : MonoBehaviour
{
	/// <summary>
	/// The scriptable object containing audio clip groups and looping information.
	/// Defines what this cue will sound like.
	/// </summary>
	[Header("Sound definition")]
	[Tooltip("The audio data asset defining what clips will be played")]
	[SerializeField] private AudioCueData audioCueData = default;

	/// <summary>
	/// Whether the audio cue should play automatically when this object starts.
	/// </summary>
	[Tooltip("If true, this audio will play automatically shortly after scene load")]
	[SerializeField] private bool playOnStart = false;

	/// <summary>
	/// Event system connection that allows this AudioCue to communicate with the AudioManager.
	/// </summary>
	[Header("Configuration")]
	[Tooltip("Event channel to communicate with the AudioManager")]
	[SerializeField] private AudioCueEvent audioCueEvent = default;

	/// <summary>
	/// Configuration data that defines how this audio should be played (volume, pitch, spatial settings).
	/// </summary>
	[Tooltip("Settings for how this audio cue should be played")]
	[SerializeField] private AudioConfigurationData audioConfigurationData = default;

	/// <summary>
	/// Called when the object is activated. Initiates playback if playOnStart is true.
	/// </summary>
	private void Start()
	{
		if (playOnStart)
		{
			PlayAudioCue();
		}
	}

	/// <summary>
	/// Called when the object is deactivated. Ensures any playing audio is stopped.
	/// </summary>
	private void OnDisable()
	{
		playOnStart = false; // Prevent delayed play from triggering if object is re-enabled
		StopAudioCue();
	}

	/// <summary>
	/// Coroutine that adds a short delay before playing the audio cue.
	/// This allows the AudioManager to initialize fully before receiving play requests.
	/// </summary>
	/// <returns>IEnumerator for Unity's coroutine system</returns>
	private IEnumerator PlayDelayed()
	{
		//The wait allows the AudioManager to be ready for play requests
		yield return new WaitForSeconds(1);

		//This additional check prevents the AudioCue from playing if the object is disabled or the scene unloaded
		//This prevents playing a looping AudioCue which then would be never stopped
		if (playOnStart)
		{
			PlayAudioCue();
		}
	}

	/// <summary>
	/// Plays the audio cue at the current transform position.
	/// Sends the play request through the AudioCueEvent.
	/// </summary>
	public void PlayAudioCue()
	{
		bool success = audioCueEvent.OnPlayEvent(audioCueData, audioConfigurationData, transform.position);
		// Note: the success variable could be used for error handling or debug logging
	}

	/// <summary>
	/// Immediately stops the audio cue from playing.
	/// Currently not implemented.
	/// </summary>
	public void StopAudioCue()
	{
		// TODO: Implement stop functionality by calling appropriate event on audioCueEvent
	}

	/// <summary>
	/// Allows a looping audio cue to finish its current loop and then stop.
	/// Currently not implemented.
	/// </summary>
	public void FinishAudioCue()
	{
		// TODO: Implement finish functionality by calling appropriate event on audioCueEvent
	}
}