using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using DG.Tweening;

public class Loot : MonoBehaviour
{
	// You can chose to use Scriptable Object or Serialized.
	// One settings for SplashSetting is already created
	// [SerializeField] SplashSettings settings;	// Scriptable Object
	[SerializeField] LootSettings settings; // Serialized

	private int bounces = 0;
	private bool isGrounded = true;
	private Vector2 groundVelocity;
	private float verticalVelocity, afterVelocity;

	public CanvasGroup canvasgroup;

	public RectTransform t_parent; // Main
	public RectTransform t_body; // Body
	public RectTransform t_shadow; // Shadow
	public Image bodysprite;
	public Image shadowsprite;
	void OnEnable()
	{
		// if (settings.destroyTime > 0)
		// 	Destroy(this.gameObject, settings.destroyTime);
		canvasgroup.alpha = 1;
		bounces = 0;
		
		SimulateDrop();
	}

	void Update()
	{
		UpdatePosition();
	}

	public void SetSprite(Sprite sp)
	{
		bodysprite.sprite = sp;
		shadowsprite.sprite = sp;
	}

	void Initialize(Vector2 groundvelocity)
	{
		isGrounded = false;
		// Slow down the height of bounce
		afterVelocity /= settings.YReducer;
		this.groundVelocity = groundvelocity;
		this.verticalVelocity = afterVelocity;
		bounces++;
	}


	// Call this method to simulate bounce effect
	// On Default it's in the Start()
	public void SimulateDrop()
	{
		StartCoroutine(Simulate());
	}

	private IEnumerator Simulate()
	{
		groundVelocity = new Vector2(Random.Range(-settings.horizontalForce, settings.horizontalForce),
			Random.Range(-settings.horizontalForce, settings.horizontalForce));
		verticalVelocity = Random.Range(settings.velocity - 1, settings.velocity);
		afterVelocity = verticalVelocity;
		Initialize(groundVelocity);

		yield return null;
	}

	private void UpdatePosition()
	{
		if (!isGrounded)
		{
			verticalVelocity += settings.gravity * Time.deltaTime;
			t_body.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
			t_parent.position += (Vector3)groundVelocity * Time.deltaTime;
			CheckGroundHit();
		}
	}

	/// <summary>
	/// If number of bounces is less than current bounces, it will add force to the item
	/// Force is each bounce reduced by XReducer and YReducer
	/// </summary>
	private void CheckGroundHit()
	{
		if (t_body.position.y < t_shadow.position.y)
		{
			t_body.position = t_shadow.position;

			if (bounces < settings.numberOfBounces)
			{
				Initialize(new Vector2(groundVelocity.x / settings.XReducer, groundVelocity.y / settings.XReducer));
			}
			else
			{

				// Give item shadow after last bounce
				if (settings.shadow)
					t_shadow.position = new Vector3(t_shadow.position.x, t_shadow.position.y - 0.05f,
						t_shadow.position.z);

				// Prevent item moving
				isGrounded = true;

				canvasgroup.DOFade(0f, 2.5f)
					.SetEase(Ease.InBack).OnComplete(() =>
					{
						this.gameObject.SetActive(false);
					});
			}

		}
	}
}

[System.Serializable]
	public class LootSettings
	{
		[Tooltip("XReducer will slow down horizontal axis ( left right top bottom movement )")] [Range(1f, 2.5f)]
		public float YReducer = 1.5f;

		[Tooltip("YReducer will slow down vertical axis ( height of the bounce )")] [Range(1f, 2.5f)]
		public float XReducer = 1.5f;

		public int numberOfBounces = 3;

		[Tooltip("Amount of vertical force")] public float velocity = 10;

		[Tooltip("Amount of horizontal force")]
		public float horizontalForce = 2;

		public float gravity = -30;

		[Tooltip("It will create small shadow after last bounce")]
		public bool shadow = true;

		public float destroyTime = 0f;
	}
