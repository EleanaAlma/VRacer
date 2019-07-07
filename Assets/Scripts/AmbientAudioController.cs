using UnityEngine;

/// <summary>
/// Class reponsible for modifying audio source's properties
/// on Awake.
/// </summary>
//Make sure an audio source exists on gameobject
[RequireComponent(typeof(AudioSource))]
public class AmbientAudioController : MonoBehaviour
{
	public bool playOnAwake;
	public bool randomizeTime;
	public bool randomizePitch;

	private AudioSource audioSource;

	private void Awake()
	{
		//Cache the audio source
		audioSource = GetComponent<AudioSource>();
		//If no clip is added to audio source, abort
		if (audioSource.clip == null)
			return;
		//Check if we want the clip's pitch to be random
		if(randomizePitch)
		{
			audioSource.pitch = Random.Range(0.2f, 0.8f);
		}
		//Check if we want the clip's starting to be random
		if(randomizeTime)
		{
			audioSource.time = Random.Range(0f, audioSource.clip.length / 2f);
		}
		//Check if we want the clip to start playing as soon as the gameobject is loaded
		if (playOnAwake && !audioSource.playOnAwake)
		{
			audioSource.Play();
		}
	}	
}
