using UnityEngine;

/// <summary>
/// Base class for all interractible Entities of the scene
/// </summary>
public abstract class EntityController : MonoBehaviour
{
	protected LevelController levelController;
	protected CharacterController characterController;

	/// <summary>
	/// Called by Level controller to gain references 
	/// for LevelController and CharacterController
	/// </summary>
	public void Setup(LevelController levelController, CharacterController characterController)
	{
		this.levelController = levelController;
		this.characterController = characterController;
	}

	/// <summary>
	/// Implemented by extending classes
	/// </summary>
	public abstract void Hit();
}
