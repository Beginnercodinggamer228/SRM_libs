using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004AD RID: 1197
public class TentacleGrapple : FindConsumable
{
	// Token: 0x060018FE RID: 6398 RVA: 0x0006145F File Offset: 0x0005F65F
	public override void Awake()
	{
		base.Awake();
		this.nextHookTime = Time.time + this.cooldown;
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x00061479 File Offset: 0x0005F679
	public override void Start()
	{
		base.Start();
		this.playerObj = SRSingleton<SceneContext>.Instance.Player;
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x00061494 File Offset: 0x0005F694
	protected override Dictionary<Identifiable.Id, DriveCalculator> GetSearchIds()
	{
		Dictionary<Identifiable.Id, DriveCalculator> searchIds = base.GetSearchIds();
		if (this.addSlimesToSearchList)
		{
			foreach (Identifiable.Id key in Identifiable.SLIME_CLASS)
			{
				searchIds[key] = new DriveCalculator(SlimeEmotions.Emotion.NONE, -0.8f, 0f);
			}
			foreach (Identifiable.Id key2 in Identifiable.LARGO_CLASS)
			{
				searchIds[key2] = new DriveCalculator(SlimeEmotions.Emotion.NONE, -0.8f, 0f);
			}
		}
		return searchIds;
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x0006155C File Offset: 0x0005F75C
	public override float Relevancy(bool isGrounded)
	{
		if (Time.time < this.nextHookTime || base.IsCaptive())
		{
			return 0f;
		}
		if (this.groundedOnly && !isGrounded)
		{
			return 0f;
		}
		float num;
		this.target = base.FindNearestConsumable(out num);
		if (this.ignoreGrapplers)
		{
			TentacleGrapple component = this.target.GetComponent<TentacleGrapple>();
			if (component != null && component.activeTentacle != null)
			{
				return 0f;
			}
		}
		if (this.target == this.playerObj)
		{
			return 0f;
		}
		if (!(this.target == null))
		{
			return num * num * 0.95f;
		}
		return 0f;
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x0006160C File Offset: 0x0005F80C
	public override void Action()
	{
		if (this.activeTentacle == null)
		{
			this.grappling = false;
			this.plexer.ForceRethink();
			return;
		}
		if (Time.time >= this.hookTimeout)
		{
			Destroyer.Destroy(this.activeTentacle, "TentacleGrapple.Action");
			this.grappling = false;
			return;
		}
		if (this.target != null && base.IsGrounded())
		{
			Vector3 normalized = (SlimeSubbehaviour.GetGotoPos(this.target) - base.transform.position).normalized;
			base.RotateTowards(normalized);
		}
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x000616A0 File Offset: 0x0005F8A0
	public override void Selected()
	{
		if (this.target != null && this.MaybeGrapple(this.target))
		{
			this.grappling = true;
		}
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x000616C5 File Offset: 0x0005F8C5
	public override void Deselected()
	{
		base.Deselected();
		this.nextHookTime = Time.time + this.cooldown;
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x000616DF File Offset: 0x0005F8DF
	public override bool CanRethink()
	{
		return !this.grappling;
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x000616EA File Offset: 0x0005F8EA
	public bool IsGrappling(GameObject target)
	{
		return this.grappling && target == this.target;
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x00061704 File Offset: 0x0005F904
	private bool MaybeGrapple(GameObject target)
	{
		RaycastHit raycastHit = default(RaycastHit);
		float intermediateHeight = 0f;
		float[] array = this.heightsAboveToGrapple;
		if (array == null || array.Length == 0)
		{
			array = new float[1];
		}
		foreach (float num in array)
		{
			Vector3 vector = base.transform.position + Vector3.up * num;
			Vector3 direction = SlimeSubbehaviour.GetGotoPos(target) - vector;
			Physics.Raycast(vector, direction, out raycastHit, direction.magnitude);
			if (raycastHit.collider != null && raycastHit.collider.gameObject == target)
			{
				intermediateHeight = num;
				break;
			}
		}
		if (raycastHit.collider == null || raycastHit.collider.gameObject != target)
		{
			return false;
		}
		if (TentacleHook.IsAlreadyHooked(raycastHit.collider.gameObject))
		{
			return false;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.tentaclePrefab);
		Attachment component = gameObject.GetComponent<Attachment>();
		gameObject.transform.SetParent(base.transform, false);
		component.Init(base.gameObject, target, raycastHit.point, this.causeFear, intermediateHeight);
		this.activeTentacle = gameObject;
		this.hookTimeout = Time.time + 10f;
		return true;
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x0006184A File Offset: 0x0005FA4A
	public void Release()
	{
		if (this.activeTentacle != null)
		{
			Destroyer.Destroy(this.activeTentacle, "TentacleGrapple.Release");
		}
	}

	// Token: 0x040018B5 RID: 6325
	[Tooltip("The prefab to form the grappling tentacle.")]
	public GameObject tentaclePrefab;

	// Token: 0x040018B6 RID: 6326
	[Tooltip("Time between tentacles, in seconds.")]
	public float cooldown = 2f;

	// Token: 0x040018B7 RID: 6327
	[Tooltip("Whether being grappled should cause fear.")]
	public bool causeFear = true;

	// Token: 0x040018B8 RID: 6328
	[Tooltip("Heights above the slime to grapple from. If empty, will grapple from slime center")]
	public float[] heightsAboveToGrapple;

	// Token: 0x040018B9 RID: 6329
	[Tooltip("Whether to add slimes to our search list, with a constant drive")]
	public bool addSlimesToSearchList;

	// Token: 0x040018BA RID: 6330
	[Tooltip("Whether we should ignore those currently grappling something else.")]
	public bool ignoreGrapplers;

	// Token: 0x040018BB RID: 6331
	[Tooltip("Should we only do this behavior when grounded?")]
	public bool groundedOnly;

	// Token: 0x040018BC RID: 6332
	private GameObject target;

	// Token: 0x040018BD RID: 6333
	private bool grappling;

	// Token: 0x040018BE RID: 6334
	private GameObject activeTentacle;

	// Token: 0x040018BF RID: 6335
	private float hookTimeout;

	// Token: 0x040018C0 RID: 6336
	private const float HOOK_TIMEOUT = 10f;

	// Token: 0x040018C1 RID: 6337
	private float nextHookTime;

	// Token: 0x040018C2 RID: 6338
	private GameObject playerObj;

	// Token: 0x040018C3 RID: 6339
	private const float EXTRA_SLIME_SEARCH_DRIVE = 0.2f;
}
