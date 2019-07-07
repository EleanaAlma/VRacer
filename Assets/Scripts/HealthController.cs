using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthController : EntityController
{
	public AudioClip powerUpClip;

	/// <summary>
	/// When player hits me, heal player by 20, 
	/// play sound effect and destroy myself
	/// </summary>
	public override void Hit()
	{
		AudioSource.PlayClipAtPoint(powerUpClip, transform.position);
		characterController.ModifyCarLife(20);
		Destroy(gameObject);
	}
}
