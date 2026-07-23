using System;
using System.Collections.Generic;
using DG.Tweening;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x02000771 RID: 1905
public class ResourceCycle : RegisteredActorBehaviour, RegistryUpdateable, Ignitable, ActorModel.Participant
{
	// Token: 0x060027DD RID: 10205 RVA: 0x00096E04 File Offset: 0x00095004
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.vacuumable = base.GetComponent<Vacuumable>();
		this.hasVacuumable = (this.vacuumable != null);
		this.mainRenderer = base.GetComponentInChildren<Renderer>();
		this.defaultScale = base.transform.localScale;
		this.vibrateAmplitude = 0.033f / this.defaultScale.x;
		this.body = base.GetComponent<Rigidbody>();
		this.toShakeDefaultPos = this.toShake.localPosition;
		this.ident = base.GetComponent<Identifiable>();
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x00096E9C File Offset: 0x0009509C
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.onDetachment = null;
		this.ident = null;
		if (this.rotInProgressMat != null)
		{
			Destroyer.Destroy(this.rotInProgressMat, "ResourceCycle.OnDestroy");
		}
		if (this.preservative != null)
		{
			this.preservative.RemoveResourceCycle(this);
		}
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x00096EF5 File Offset: 0x000950F5
	public void InitModel(ActorModel model)
	{
		((ProduceModel)model).progressTime = this.timeDir.HoursFromNowOrStart(ResourceCycle.Vary(this.edibleGameHours));
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x00096F18 File Offset: 0x00095118
	public void SetModel(ActorModel model)
	{
		this.model = (ProduceModel)model;
		this.SetInitState(this.model.state, this.model.progressTime);
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x00096F44 File Offset: 0x00095144
	public void Attach(Joint joint, ResourceCycle.AdditionalRipeness additionalRipenessDelegate = null, ResourceCycle.DetachmentEvent detachmentDelegate = null)
	{
		this.model.state = ResourceCycle.State.UNRIPE;
		this.DetachFromJoint();
		this.joint = SafeJointReference.AttachSafely(this.body.gameObject, joint, false);
		joint.anchor = Vector3.zero;
		joint.connectedAnchor = Vector3.zero;
		this.additionalRipenessDelegate = additionalRipenessDelegate;
		this.onDetachment = detachmentDelegate;
		this.body.isKinematic = true;
		base.transform.localScale = this.defaultScale * 0.33f;
		this.model.progressTime = this.timeDir.HoursFromNowOrStart(ResourceCycle.Vary(this.unripeGameHours));
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x00096FE7 File Offset: 0x000951E7
	public void Reattach(Joint joint)
	{
		this.DetachFromJoint();
		this.joint = SafeJointReference.AttachSafely(this.body.gameObject, joint, false);
		joint.anchor = Vector3.zero;
		joint.connectedAnchor = Vector3.zero;
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x0009701D File Offset: 0x0009521D
	public void Detach(ResourceCycle.AdditionalRipeness additionalRipenessDelegate)
	{
		this.additionalRipenessDelegate = (ResourceCycle.AdditionalRipeness)Delegate.Remove(this.additionalRipenessDelegate, additionalRipenessDelegate);
		if (this.model.state == ResourceCycle.State.RIPE)
		{
			this.MakeEdible();
		}
		this.DetachFromJoint();
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x00097050 File Offset: 0x00095250
	private void DetachFromJoint()
	{
		if (this.joint != null)
		{
			if (this.joint.joint != null)
			{
				this.joint.joint.connectedBody = null;
			}
			Destroyer.Destroy(this.joint, "ResourceCycle.DetachFromJoint");
			this.joint = null;
			if (this.onDetachment != null)
			{
				this.onDetachment();
				this.onDetachment = null;
			}
		}
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x000970C0 File Offset: 0x000952C0
	public void AttachPreservative(MiracleMix preservative)
	{
		this.preservative = preservative;
		if (this.model.state == ResourceCycle.State.EDIBLE)
		{
			this.additionalRipenessDelegate = new ResourceCycle.AdditionalRipeness(preservative.PreservativeRipenessModifier);
		}
	}

	// Token: 0x060027E6 RID: 10214 RVA: 0x000970E9 File Offset: 0x000952E9
	public void DetachPreservative(MiracleMix preservative)
	{
		if (this.model.state == ResourceCycle.State.EDIBLE)
		{
			this.additionalRipenessDelegate = null;
		}
		this.preservative = null;
	}

	// Token: 0x060027E7 RID: 10215 RVA: 0x00097108 File Offset: 0x00095308
	public void Ignite(GameObject igniter)
	{
		if (this.model.state == ResourceCycle.State.EDIBLE || this.model.state == ResourceCycle.State.ROTTEN)
		{
			if (this.igniteFX != null)
			{
				SRBehaviour.SpawnAndPlayFX(this.igniteFX);
			}
			if (this.model.state == ResourceCycle.State.EDIBLE)
			{
				Destroyer.DestroyActor(base.gameObject, "ResourceCycle.Ignite", false);
				return;
			}
			Destroyer.Destroy(base.gameObject, "ResourceCycle.Ignite");
		}
	}

	// Token: 0x060027E8 RID: 10216 RVA: 0x0009717B File Offset: 0x0009537B
	public void ImmediatelyRipen(float bonusRipenessHours)
	{
		if (this.model.state != ResourceCycle.State.UNRIPE)
		{
			Debug.Log("Trying to ripen already-ripe resource?");
			return;
		}
		this.model.progressTime = this.timeDir.HoursFromNowOrStart(-bonusRipenessHours);
	}

	// Token: 0x060027E9 RID: 10217 RVA: 0x000971AD File Offset: 0x000953AD
	public void ImmediatelyRot()
	{
		if (this.model != null && this.model.state == ResourceCycle.State.EDIBLE)
		{
			this.Rot();
			this.SetRotten(true);
		}
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x000971D2 File Offset: 0x000953D2
	public static float Vary(float val)
	{
		return Randoms.SHARED.GetInRange(0.9f, 1.1f) * val;
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x000971EC File Offset: 0x000953EC
	public void RegistryUpdate()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (this.additionalRipenessDelegate != null)
		{
			this.model.progressTime -= (double)this.additionalRipenessDelegate() * this.timeDir.DeltaWorldTime();
		}
		Rigidbody rigidbody = this.body;
		if (this.model.state == ResourceCycle.State.UNRIPE && this.joint == null)
		{
			Destroyer.DestroyActor(base.gameObject, "ResourceCycle.RegistryUpdate#1", false);
		}
		else if (this.joint == null && rigidbody.isKinematic)
		{
			rigidbody.isKinematic = false;
		}
		if (this.hasVacuumable && this.vacuumableWhenRipe && this.model.state == ResourceCycle.State.RIPE && this.vacuumable.Pending && !this.preparingToRelease)
		{
			this.preparingToRelease = true;
			this.releaseAt = Time.time + this.releasePrepTime;
		}
		if (this.preparingToRelease)
		{
			if (this.hasVacuumable && this.vacuumable.Pending)
			{
				this.toShake.localPosition = this.toShakeDefaultPos + UnityEngine.Random.insideUnitSphere * this.vibrateAmplitude;
			}
			else
			{
				this.preparingToRelease = false;
				this.releaseAt = 0f;
				this.toShake.localPosition = this.toShakeDefaultPos;
			}
		}
		this.ProgressResource(this.model.progressTime);
		if (this.rotInProgressMat != null)
		{
			float num = Mathf.Min(1f, this.rotInProgressMat.GetFloat("_Rot") + 0.5f * Time.deltaTime);
			if (num < 1f)
			{
				this.rotInProgressMat.SetFloat("_Rot", num);
				return;
			}
			this.mainRenderer.sharedMaterial = this.rottenMat;
			Destroyer.Destroy(this.rotInProgressMat, "ResourceCycle.RegistryUpdate#2");
			this.rotInProgressMat = null;
		}
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000973C6 File Offset: 0x000955C6
	public void UpdateToNow()
	{
		this.ProgressResource(this.model.progressTime);
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x000973DC File Offset: 0x000955DC
	public bool WouldProgressToRotten(double spawnTime, double worldTime)
	{
		double targetWorldTime = spawnTime + (double)((this.unripeGameHours + this.ripeGameHours + this.edibleGameHours) * 3600f);
		return TimeUtil.HasReached(worldTime, targetWorldTime);
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x00097410 File Offset: 0x00095610
	public void ProgressResource(double nextProgressionTime)
	{
		Rigidbody rigidbody = this.body;
		bool flag = this.timeDir.HasReached(this.model.progressTime + 3600.0);
		this.model.progressTime = nextProgressionTime;
		while (this.timeDir.HasReached(this.model.progressTime) || (this.preparingToRelease && Time.time >= this.releaseAt))
		{
			if (this.model.state == ResourceCycle.State.UNRIPE && this.timeDir.HasReached(this.model.progressTime))
			{
				this.Ripen();
				if (this.vacuumableWhenRipe)
				{
					this.vacuumable.enabled = true;
				}
				if (base.gameObject.transform.localScale.x < this.defaultScale.x * 0.33f)
				{
					base.gameObject.transform.localScale = this.defaultScale * 0.33f;
				}
				TweenUtil.ScaleTo(base.gameObject, this.defaultScale, 4f, Ease.InOutQuad);
			}
			else if (this.model.state == ResourceCycle.State.RIPE && ((this.preparingToRelease && Time.time >= this.releaseAt) || this.timeDir.HasReached(this.model.progressTime)))
			{
				this.MakeEdible();
				this.additionalRipenessDelegate = null;
				rigidbody.isKinematic = false;
				if (this.preparingToRelease)
				{
					this.preparingToRelease = false;
					this.releaseAt = 0f;
					this.toShake.localPosition = this.toShakeDefaultPos;
					if (this.releaseCue != null)
					{
						SECTR_PointSource component = base.GetComponent<SECTR_PointSource>();
						component.Cue = this.releaseCue;
						component.Play();
					}
				}
				rigidbody.WakeUp();
				this.Eject(rigidbody);
				this.DetachFromJoint();
				if (this.hasVacuumable)
				{
					this.vacuumable.Pending = false;
				}
			}
			else if (this.model.state == ResourceCycle.State.EDIBLE && this.timeDir.HasReached(this.model.progressTime))
			{
				this.Rot();
				this.SetRotten(false);
			}
			else if (this.model.state == ResourceCycle.State.ROTTEN && this.timeDir.HasReached(this.model.progressTime))
			{
				if (this.destroyFX != null && !flag)
				{
					SRBehaviour.SpawnAndPlayFX(this.destroyFX, base.transform.position, base.transform.rotation);
				}
				Destroyer.Destroy(base.gameObject, 0f, "ResourceCycle.ProgressResource", true, false);
				this.model.progressTime = double.MaxValue;
			}
		}
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x000976C0 File Offset: 0x000958C0
	private void Ripen()
	{
		this.model.state = ResourceCycle.State.RIPE;
		this.AdvanceProgressTime(this.ripeGameHours);
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x000976DA File Offset: 0x000958DA
	private void MakeEdible()
	{
		this.model.state = ResourceCycle.State.EDIBLE;
		if (this.preservative != null)
		{
			this.additionalRipenessDelegate = new ResourceCycle.AdditionalRipeness(this.preservative.PreservativeRipenessModifier);
		}
		this.AdvanceProgressTime(this.edibleGameHours);
	}

	// Token: 0x060027F1 RID: 10225 RVA: 0x00097719 File Offset: 0x00095919
	private void Rot()
	{
		if (this.preservative != null)
		{
			this.preservative.RemoveResourceCycle(this);
		}
		this.model.state = ResourceCycle.State.ROTTEN;
		this.AdvanceProgressTime(this.rottenGameHours);
	}

	// Token: 0x060027F2 RID: 10226 RVA: 0x0009774D File Offset: 0x0009594D
	internal ResourceCycle.State GetState()
	{
		return this.model.state;
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x0009775A File Offset: 0x0009595A
	private void AdvanceProgressTime(float progressBaseAmount)
	{
		this.model.progressTime = Math.Min(this.timeDir.WorldTime(), this.model.progressTime) + (double)(ResourceCycle.Vary(progressBaseAmount) * 3600f);
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x00097790 File Offset: 0x00095990
	public void Eject(Rigidbody rigidbody)
	{
		rigidbody.MoveRotation(Quaternion.Euler(5f, Randoms.SHARED.GetFloat(360f), 0f));
		rigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * 5f);
		if (this.joint != null && this.joint.joint != null && this.addEjectionForce)
		{
			rigidbody.AddForce(this.joint.joint.transform.up * 80f);
		}
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x00097824 File Offset: 0x00095A24
	private void SetInitState(ResourceCycle.State state, double progressTime)
	{
		if ((state == ResourceCycle.State.UNRIPE || state == ResourceCycle.State.RIPE) && !this.AttachToNearest())
		{
			state = ResourceCycle.State.EDIBLE;
			progressTime = (double)((float)this.timeDir.HoursFromNow(ResourceCycle.Vary(this.edibleGameHours)));
			Log.Debug("Could not find joint within patch", new object[]
			{
				"resource",
				base.gameObject
			});
		}
		this.model.progressTime = progressTime;
		this.model.state = state;
		if (this.hasVacuumable && this.vacuumableWhenRipe && state != ResourceCycle.State.UNRIPE)
		{
			this.vacuumable.enabled = true;
		}
		base.transform.localScale = this.defaultScale * ((state == ResourceCycle.State.UNRIPE) ? 0.33f : 1f);
		if (state == ResourceCycle.State.ROTTEN)
		{
			this.SetRotten(true);
		}
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000978E8 File Offset: 0x00095AE8
	private void SetRotten(bool immediate)
	{
		if (this.hasVacuumable)
		{
			this.vacuumable.SetDestroyOnVac(true);
		}
		SRSingleton<SceneContext>.Instance.GameModel.DestroyActorModel(base.gameObject);
		if (immediate)
		{
			this.mainRenderer.sharedMaterial = this.rottenMat;
			return;
		}
		this.mainRenderer.material = this.rottenMat;
		this.rotInProgressMat = this.mainRenderer.material;
		this.rotInProgressMat.SetFloat("_Rot", 0f);
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x0009796C File Offset: 0x00095B6C
	private bool AttachToNearest()
	{
		ZoneDirector zoneDirector;
		if (this.ident != null && this.ident.id == Identifiable.Id.GINGER_VEGGIE && ZoneDirector.zones.TryGetValue(ZoneDirector.Zone.DESERT, out zoneDirector))
		{
			List<GingerPatchNode> currentGingerPatches = zoneDirector.GetCurrentGingerPatches();
			GingerPatchNode gingerPatchNode;
			if (this.GetNearestIdHandler<GingerPatchNode>(currentGingerPatches, 10f, out gingerPatchNode))
			{
				gingerPatchNode.Grow(base.gameObject);
				return true;
			}
		}
		KookadobaNodeModel kookadobaNodeModel;
		if (this.ident != null && this.ident.id == Identifiable.Id.KOOKADOBA_FRUIT && this.GetNearest<KookadobaNodeModel>(SRSingleton<SceneContext>.Instance.GameModel.AllKookadobaNodes(), 10f, out kookadobaNodeModel))
		{
			kookadobaNodeModel.Grow(base.gameObject);
			return true;
		}
		SpawnResourceModel spawnResourceModel;
		if (this.GetNearest<SpawnResourceModel>(SRSingleton<SceneContext>.Instance.GameModel.AllResourceSpawners(), 10f, out spawnResourceModel))
		{
			Joint x = spawnResourceModel.NearestJoint(base.transform.position, 0.1f);
			if (x != null)
			{
				this.Attach(x, null, null);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x00097A68 File Offset: 0x00095C68
	private bool GetNearest<T>(IEnumerable<T> items, float distance, out T picked) where T : PositionalModel
	{
		picked = default(T);
		float num = distance * distance;
		foreach (T t in items)
		{
			float sqrMagnitude = (t.pos - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				picked = t;
			}
		}
		return picked != null;
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x00097AF4 File Offset: 0x00095CF4
	private bool GetNearestIdHandler<T>(IEnumerable<T> items, float distance, out T picked) where T : IdHandler
	{
		picked = default(T);
		float num = distance * distance;
		foreach (T t in items)
		{
			float sqrMagnitude = (t.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				picked = t;
			}
		}
		return picked != null;
	}

	// Token: 0x0400276F RID: 10095
	public float unripeGameHours = 6f;

	// Token: 0x04002770 RID: 10096
	public float ripeGameHours = 6f;

	// Token: 0x04002771 RID: 10097
	public float edibleGameHours = 12f;

	// Token: 0x04002772 RID: 10098
	public float rottenGameHours = 6f;

	// Token: 0x04002773 RID: 10099
	public Material rottenMat;

	// Token: 0x04002774 RID: 10100
	public GameObject destroyFX;

	// Token: 0x04002775 RID: 10101
	public SECTR_AudioCue releaseCue;

	// Token: 0x04002776 RID: 10102
	public Transform toShake;

	// Token: 0x04002777 RID: 10103
	public bool vacuumableWhenRipe = true;

	// Token: 0x04002778 RID: 10104
	private ResourceCycle.DetachmentEvent onDetachment;

	// Token: 0x04002779 RID: 10105
	public bool addEjectionForce = true;

	// Token: 0x0400277A RID: 10106
	public float releasePrepTime = 1f;

	// Token: 0x0400277B RID: 10107
	public GameObject igniteFX;

	// Token: 0x0400277C RID: 10108
	private Vacuumable vacuumable;

	// Token: 0x0400277D RID: 10109
	private Renderer mainRenderer;

	// Token: 0x0400277E RID: 10110
	private TimeDirector timeDir;

	// Token: 0x0400277F RID: 10111
	private SafeJointReference joint;

	// Token: 0x04002780 RID: 10112
	private Vector3 defaultScale;

	// Token: 0x04002781 RID: 10113
	private ResourceCycle.AdditionalRipeness additionalRipenessDelegate;

	// Token: 0x04002782 RID: 10114
	private bool preparingToRelease;

	// Token: 0x04002783 RID: 10115
	private float releaseAt;

	// Token: 0x04002784 RID: 10116
	private Material rotInProgressMat;

	// Token: 0x04002785 RID: 10117
	private const float EJECT_TORQUE = 5f;

	// Token: 0x04002786 RID: 10118
	private const float EJECT_FORCE = 80f;

	// Token: 0x04002787 RID: 10119
	private const float UNRIPE_SCALE = 0.33f;

	// Token: 0x04002788 RID: 10120
	private const float RIPEN_SCALE_TIME = 4f;

	// Token: 0x04002789 RID: 10121
	private const float VIBRATE_AMPLITUDE = 0.033f;

	// Token: 0x0400278A RID: 10122
	private const float ROT_PROGRESS_PER_SEC = 0.5f;

	// Token: 0x0400278B RID: 10123
	private const string ROT_SHADER_PARAM = "_Rot";

	// Token: 0x0400278C RID: 10124
	private bool hasVacuumable;

	// Token: 0x0400278D RID: 10125
	private Rigidbody body;

	// Token: 0x0400278E RID: 10126
	private Vector3 toShakeDefaultPos;

	// Token: 0x0400278F RID: 10127
	private float vibrateAmplitude;

	// Token: 0x04002790 RID: 10128
	private Identifiable ident;

	// Token: 0x04002791 RID: 10129
	private ProduceModel model;

	// Token: 0x04002792 RID: 10130
	private MiracleMix preservative;

	// Token: 0x02000772 RID: 1906
	[Serializable]
	public enum State
	{
		// Token: 0x04002794 RID: 10132
		UNRIPE,
		// Token: 0x04002795 RID: 10133
		RIPE,
		// Token: 0x04002796 RID: 10134
		EDIBLE,
		// Token: 0x04002797 RID: 10135
		ROTTEN
	}

	// Token: 0x02000773 RID: 1907
	[Serializable]
	public class CycleData
	{
		// Token: 0x060027FB RID: 10235 RVA: 0x00097BE0 File Offset: 0x00095DE0
		public CycleData(ResourceCycle.State state, float progressTime)
		{
			this.state = state;
			this.progressTime = progressTime;
		}

		// Token: 0x060027FC RID: 10236 RVA: 0x00097BF8 File Offset: 0x00095DF8
		public override bool Equals(object o)
		{
			if (!(o is ResourceCycle.CycleData))
			{
				return false;
			}
			ResourceCycle.CycleData cycleData = (ResourceCycle.CycleData)o;
			return this.state == cycleData.state && this.progressTime == cycleData.progressTime;
		}

		// Token: 0x060027FD RID: 10237 RVA: 0x00097C34 File Offset: 0x00095E34
		public override int GetHashCode()
		{
			return this.state.GetHashCode() ^ this.progressTime.GetHashCode();
		}

		// Token: 0x060027FE RID: 10238 RVA: 0x00097C53 File Offset: 0x00095E53
		public override string ToString()
		{
			return this.state + ":" + this.progressTime;
		}

		// Token: 0x04002798 RID: 10136
		public ResourceCycle.State state;

		// Token: 0x04002799 RID: 10137
		public float progressTime;
	}

	// Token: 0x02000774 RID: 1908
	// (Invoke) Token: 0x06002800 RID: 10240
	public delegate float AdditionalRipeness();

	// Token: 0x02000775 RID: 1909
	// (Invoke) Token: 0x06002804 RID: 10244
	public delegate void DetachmentEvent();
}
