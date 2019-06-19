using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
	protected LevelController levelController;
	protected CharacterController characterController;

	public void Setup(LevelController levelController, CharacterController characterController)
	{
		this.levelController = levelController;
		this.characterController = characterController;
	}

	public abstract void Hit();
}
