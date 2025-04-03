using System;
using UnityEngine;

/// <summary>
/// Scriptable Object that defines a set of audio clips that can be played together as a logical sound "cue".
/// Supports organization of multiple audio variations with different playback sequence modes.
/// </summary>
[CreateAssetMenu(fileName = "AudioCueData", menuName = "Audio/Audio Cue Data")]
public class AudioCueData : ScriptableObject
{
	/// <summary>
	/// Determines if this audio cue should play in a loop.
	/// </summary>
	[Tooltip("If true, the audio will play continuously until stopped")]
	public bool looping = false;

	/// <summary>
	/// Collection of audio clip groups that make up this cue.
	/// Each group can contain multiple variations of a sound with different sequencing rules.
	/// </summary>
	[Tooltip("Groups of audio clips with different playback patterns")]
	[SerializeField] private AudioClipGroup audioClipGroups;


	public AudioClip GetClip()
	{
		return audioClipGroups.GetNextClip();
	}

	/// <summary>
	/// Represents a group of AudioClips that can be treated as one, and provides automatic randomisation or sequencing based on the <c>SequenceMode</c> value.
	/// </summary>
	[Serializable]
	public class AudioClipGroup
	{
		/// <summary>
		/// Defines how clips in this group will be selected for playback.
		/// </summary>
		[Tooltip("How to select the next clip: randomly, randomly with no repeats, or in sequence")]
		public SequenceMode sequenceMode = SequenceMode.RandomNoImmediateRepeat;

		/// <summary>
		/// The collection of audio clips in this group.
		/// Multiple clips allow for variation in sounds to avoid repetitiveness.
		/// </summary>
		[Tooltip("The audio clips that make up this group - variations of the same sound")]
		public AudioClip[] audioClips;

		/// <summary>
		/// Tracks the index of the next clip to be played.
		/// Initialized to -1 to indicate that no clip has been selected yet.
		/// </summary>
		private int nextClipToPlay = -1;

		/// <summary>
		/// Tracks the index of the most recently played clip.
		/// Used to prevent immediate repetition in RandomNoImmediateRepeat mode.
		/// </summary>
		private int lastClipPlayed = -1;

		/// <summary>
		/// Chooses the next clip in the sequence, either following the order or randomly.
		/// </summary>
		/// <returns>A reference to an AudioClip based on the current SequenceMode</returns>
		public AudioClip GetNextClip()
		{
			// Fast out if there is only one clip to play
			if (audioClips.Length == 1)
			{
				return audioClips[0];
			}

			if (nextClipToPlay == -1)
			{
				// Index needs to be initialised: 0 if Sequential, random if otherwise
				nextClipToPlay = (sequenceMode == SequenceMode.Sequential) ? 0 : UnityEngine.Random.Range(0, audioClips.Length);
			}
			else
			{
				// Select next clip index based on the appropriate SequenceMode
				switch (sequenceMode)
				{
					case SequenceMode.Random:
						// Completely random selection - can repeat the same clip
						nextClipToPlay = UnityEngine.Random.Range(0, audioClips.Length);
						break;

					case SequenceMode.RandomNoImmediateRepeat:
						// Random but avoids playing the same clip twice in a row
						do
						{
							nextClipToPlay = UnityEngine.Random.Range(0, audioClips.Length);
						} while (nextClipToPlay == lastClipPlayed);
						break;

					case SequenceMode.Sequential:
						// Play clips in order, looping back to the start when reaching the end
						nextClipToPlay = ++nextClipToPlay % audioClips.Length;
						break;
				}
			}

			// Store the selected clip index for comparison in the next call
			lastClipPlayed = nextClipToPlay;

			return audioClips[nextClipToPlay];
		}

		/// <summary>
		/// Defines different modes for selecting the next clip from a group.
		/// </summary>
		public enum SequenceMode
		{
			/// <summary>
			/// Select clips completely randomly, allowing for immediate repetition.
			/// </summary>
			Random,

			/// <summary>
			/// Select clips randomly, but avoid playing the same clip twice in a row.
			/// </summary>
			RandomNoImmediateRepeat,

			/// <summary>
			/// Play clips in the exact order they appear in the array, looping back to the beginning.
			/// </summary>
			Sequential,
		}
	}
}