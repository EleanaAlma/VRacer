using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientAudioController : MonoBehaviour
{
	public bool playOnAwake;
	public bool randomizeTime;
	public bool randomizePitch;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource.clip == null)
			return;
		if(randomizePitch)
		{
			audioSource.pitch = Random.Range(0.2f, 0.8f);
		}
		if(randomizeTime)
		{
			audioSource.time = Random.Range(0f, audioSource.clip.length / 2f);
		}
		if (playOnAwake && !audioSource.playOnAwake)
		{
			audioSource.Play();
		}
	}	
}
