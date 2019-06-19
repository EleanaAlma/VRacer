using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthController : EntityController
{
	public AudioClip powerUpClip;

	public override void Hit()
	{
		AudioSource.PlayClipAtPoint(powerUpClip, transform.position);
		characterController.ModifyCarLife(20);
		Destroy(gameObject);
	}
}
