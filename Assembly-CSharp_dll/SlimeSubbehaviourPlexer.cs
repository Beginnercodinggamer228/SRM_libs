using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000495 RID: 1173
public class SlimeSubbehaviourPlexer : RegisteredActorBehaviour, FloatingReactor, RegistryFixedUpdateable
{
	// Token: 0x170001DA RID: 474
	// (get) Token: 0x06001869 RID: 6249 RVA: 0x0005E5F2 File Offset: 0x0005C7F2
	// (set) Token: 0x0600186A RID: 6250 RVA: 0x0005E5FA File Offset: 0x0005C7FA
	public float? activationDelayOverride { get; set; }

	// Token: 0x0600186B RID: 6251 RVA: 0x0005E604 File Offset: 0x0005C804
	public override void Start()
	{
		base.Start();
		this.CollectSubbehaviours();
		this.body = base.GetComponent<Rigidbody>();
		this.vacuumable = base.GetComponent<Vacuumable>();
		foreach (Collider collider in base.GetComponents<Collider>())
		{
			if (!collider.isTrigger)
			{
				this.ownCollider = collider;
				break;
			}
		}
		this.totemLinker = base.GetComponentInChildren<TotemLinker>();
		this.activationTime = Time.fixedTime + ((this.activationDelayOverride != null) ? this.activationDelayOverride.Value : 3f);
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x0005E69C File Offset: 0x0005C89C
	public void RegisterBehaviorBlocker()
	{
		this.behaviorBlockers++;
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x0005E6AC File Offset: 0x0005C8AC
	public void UnregisterBehaviorBlocker()
	{
		this.behaviorBlockers--;
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x0005E6BC File Offset: 0x0005C8BC
	public void CollectSubbehaviours()
	{
		SlimeSubbehaviour[] components = base.GetComponents<SlimeSubbehaviour>();
		List<SlimeSubbehaviour> list = new List<SlimeSubbehaviour>(components);
		foreach (SlimeSubbehaviour slimeSubbehaviour in components)
		{
			foreach (SlimeSubbehaviour slimeSubbehaviour2 in components)
			{
				if (slimeSubbehaviour.Forbids(slimeSubbehaviour2))
				{
					Destroyer.Destroy(slimeSubbehaviour2, "SlimeSubbehaviourPlexer.CollectSubbehaviours");
					list.Remove(slimeSubbehaviour2);
				}
			}
		}
		this.subbehaviors = list.ToArray();
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x0005E738 File Offset: 0x0005C938
	public void RegistryFixedUpdate()
	{
		if (this.ownCollider != null)
		{
			this.distToGround = this.ownCollider.bounds.extents.y * 1.3f;
		}
		if (Time.fixedTime >= this.nextGroundCheckTime)
		{
			this.nextGroundCheckTime = Time.fixedTime + 0.25f;
			if (this.distToGround > 0f)
			{
				RaycastCommand command = new RaycastCommand(this.body.position, Vector3.down, this.distToGround, -5, 1);
				SRSingleton<GameContext>.Instance.RaycastBatcher.QueueRaycast(command, new Action<RaycastHit>(this.OnGroundedRaycastResultReceived));
			}
			else
			{
				this.wasGrounded = false;
			}
		}
		if (Time.fixedTime < this.activationTime)
		{
			return;
		}
		if (this.IsCaptive() || this.behaviorBlockers > 0)
		{
			if (this.currBehavior != null)
			{
				if (this.currBehavior.CanRethink())
				{
					this.currBehavior.Deselected();
					this.currBehavior = null;
					return;
				}
				this.currBehavior.Action();
				return;
			}
		}
		else if (Time.fixedTime >= this.nextRethinkTime && (this.currBehavior == null || this.currBehavior.CanRethink()))
		{
			this.nextRethinkTime = Time.fixedTime + 1f;
			SlimeSubbehaviour bestBehaviour = this.GetBestBehaviour();
			if (bestBehaviour != null)
			{
				if (bestBehaviour != this.currBehavior)
				{
					if (this.currBehavior != null)
					{
						this.currBehavior.Deselected();
					}
					this.currBehavior = bestBehaviour;
					this.currBehavior.Selected();
				}
				bestBehaviour.Action();
				return;
			}
			if (this.currBehavior != null)
			{
				this.currBehavior.Deselected();
			}
			this.currBehavior = null;
			return;
		}
		else if (this.currBehavior != null)
		{
			this.currBehavior.Action();
		}
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x0005E90C File Offset: 0x0005CB0C
	private void OnGroundedRaycastResultReceived(RaycastHit result)
	{
		this.groundHit = result;
		this.wasGrounded = (result.collider != null);
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x0005E928 File Offset: 0x0005CB28
	private SlimeSubbehaviour GetBestBehaviour()
	{
		bool isGrounded = this.IsGrounded();
		float num = 0.0001f;
		SlimeSubbehaviour result = null;
		if (this.subbehaviors != null)
		{
			foreach (SlimeSubbehaviour slimeSubbehaviour in this.subbehaviors)
			{
				if (slimeSubbehaviour.enabled)
				{
					float num2 = slimeSubbehaviour.Relevancy(isGrounded);
					if (num2 < 0f || num2 > 1f)
					{
						Log.Error("Behavior relevancy outside of correct range.", new object[]
						{
							"relevancy",
							num2,
							"behavior",
							slimeSubbehaviour.name
						});
					}
					if (num2 > num)
					{
						num = num2;
						result = slimeSubbehaviour;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x0005E9D3 File Offset: 0x0005CBD3
	public bool IsCaptive()
	{
		return this.vacuumable != null && (this.vacuumable.isCaptive() || this.vacuumable.isHeld());
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x0005E9FF File Offset: 0x0005CBFF
	public void ForceRethink()
	{
		this.nextRethinkTime = float.NegativeInfinity;
		if (this.currBehavior != null)
		{
			this.currBehavior.Deselected();
			this.currBehavior = null;
		}
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x0005EA2C File Offset: 0x0005CC2C
	public bool IsFloating()
	{
		return this.isFloating;
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x0005EA34 File Offset: 0x0005CC34
	public void SetIsFloating(bool isFloating)
	{
		this.isFloating = isFloating;
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x0005EA3D File Offset: 0x0005CC3D
	public bool IsGrounded()
	{
		if (this.IsFloating())
		{
			this.groundHit.normal = Vector3.up;
			return true;
		}
		return this.wasGrounded;
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x0005EA5F File Offset: 0x0005CC5F
	public RaycastHit GroundHit()
	{
		return this.groundHit;
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x0005EA67 File Offset: 0x0005CC67
	public bool IsTotemed()
	{
		return this.totemLinker != null && this.totemLinker.IsLinkedFrom();
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x0005EA84 File Offset: 0x0005CC84
	public bool IsNearGrounded(float dist)
	{
		return this.body != null && Physics.Raycast(this.body.position, Vector3.down, this.distToGround + dist);
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x0005EAB4 File Offset: 0x0005CCB4
	public bool IsBlocked(GameObject obj, int layersToIgnore, bool forceCheckFullDist)
	{
		Vector3 direction = (obj == null) ? base.transform.forward : (SlimeSubbehaviour.GetGotoPos(obj) - base.transform.position);
		return this.IsBlocked(obj, direction, layersToIgnore, forceCheckFullDist);
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x0005EAF8 File Offset: 0x0005CCF8
	public bool IsBlocked(GameObject obj, Vector3 direction, int layersToIgnore, bool forceCheckFullDist)
	{
		if (forceCheckFullDist || ((Time.time > this.nextBlockCheckTime || this.lastBlockedTarget != obj) && this.distToGround > 0f))
		{
			direction.y = 0f;
			float radius = this.distToGround * 0.05f;
			float num;
			if (obj != null)
			{
				num = Vector3.Distance(base.transform.position, obj.transform.position);
				if (!forceCheckFullDist)
				{
					num = Mathf.Min(this.distToGround * 5f, num);
				}
			}
			else
			{
				num = this.distToGround * 5f;
			}
			RaycastHit raycastHit;
			Physics.SphereCast(this.body.position, radius, direction, out raycastHit, num, ~layersToIgnore);
			this.lastBlocked = (raycastHit.collider != null && (obj == null || raycastHit.collider.gameObject != obj));
			this.lastBlockedTarget = obj;
			this.nextBlockCheckTime = Time.time + 1f;
		}
		return this.lastBlocked;
	}

	// Token: 0x040017F5 RID: 6133
	private const float RETHINK_PERIOD = 1f;

	// Token: 0x040017F6 RID: 6134
	private SlimeSubbehaviour[] subbehaviors;

	// Token: 0x040017F7 RID: 6135
	private SlimeSubbehaviour currBehavior;

	// Token: 0x040017F8 RID: 6136
	private float nextRethinkTime;

	// Token: 0x040017F9 RID: 6137
	private Vacuumable vacuumable;

	// Token: 0x040017FA RID: 6138
	private bool isFloating;

	// Token: 0x040017FB RID: 6139
	private float activationTime;

	// Token: 0x040017FC RID: 6140
	private float distToGround;

	// Token: 0x040017FD RID: 6141
	private Collider ownCollider;

	// Token: 0x040017FE RID: 6142
	private bool lastBlocked;

	// Token: 0x040017FF RID: 6143
	private GameObject lastBlockedTarget;

	// Token: 0x04001800 RID: 6144
	private float nextBlockCheckTime;

	// Token: 0x04001801 RID: 6145
	private float nextGroundCheckTime;

	// Token: 0x04001802 RID: 6146
	private bool wasGrounded;

	// Token: 0x04001803 RID: 6147
	private RaycastHit groundHit;

	// Token: 0x04001804 RID: 6148
	private Rigidbody body;

	// Token: 0x04001805 RID: 6149
	private TotemLinker totemLinker;

	// Token: 0x04001806 RID: 6150
	private int behaviorBlockers;

	// Token: 0x04001807 RID: 6151
	private const float BLOCKED_CHECK_INTERVAL = 1f;

	// Token: 0x04001808 RID: 6152
	private const float GROUND_CHECK_INTERVAL = 0.25f;

	// Token: 0x04001809 RID: 6153
	private const float DEFAULT_ACTIVATION_DELAY = 3f;

	// Token: 0x0400180B RID: 6155
	private const float GROUNDING_FACTOR = 1.3f;

	// Token: 0x0400180C RID: 6156
	private const float BLOCKED_FACTOR = 5f;
}
