using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x020006B3 RID: 1715
public class Vacuumable : CollidableActorBehaviour, Collidable
{
	// Token: 0x14000020 RID: 32
	// (add) Token: 0x060023B3 RID: 9139 RVA: 0x0008A3B8 File Offset: 0x000885B8
	// (remove) Token: 0x060023B4 RID: 9140 RVA: 0x0008A3F0 File Offset: 0x000885F0
	public event Vacuumable.OnSetHeld onSetHeld;

	// Token: 0x14000021 RID: 33
	// (add) Token: 0x060023B5 RID: 9141 RVA: 0x0008A428 File Offset: 0x00088628
	// (remove) Token: 0x060023B6 RID: 9142 RVA: 0x0008A460 File Offset: 0x00088660
	public event Vacuumable.OnSetLaunched onSetLaunched;

	// Token: 0x14000022 RID: 34
	// (add) Token: 0x060023B7 RID: 9143 RVA: 0x0008A498 File Offset: 0x00088698
	// (remove) Token: 0x060023B8 RID: 9144 RVA: 0x0008A4D0 File Offset: 0x000886D0
	public event Vacuumable.Consume consume;

	// Token: 0x060023B9 RID: 9145 RVA: 0x0008A508 File Offset: 0x00088708
	public override void Awake()
	{
		base.Awake();
		this.defaultLayer = base.gameObject.layer;
		this.sfAnimator = base.GetComponent<SlimeFaceAnimator>();
		this.body = base.GetComponent<Rigidbody>();
		this.identifiable = base.GetComponent<Identifiable>();
		this.slimeAppearanceApplicator = base.GetComponent<SlimeAppearanceApplicator>();
		if (this.slimeAppearanceApplicator != null)
		{
			this.slimeAppearanceApplicator.OnAppearanceChanged += delegate(SlimeAppearance appearance)
			{
				this.ForceUpdateLayer();
			};
		}
		this.ignoresGravity = !this.body.useGravity;
		VacDelaunchTrigger[] componentsInChildren = base.GetComponentsInChildren<VacDelaunchTrigger>(true);
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			this.delaunchTrigger = componentsInChildren[0];
		}
		this.heldShaderPropertyId = Shader.PropertyToID("_HeldInVac");
		this.consumeStartScale = base.transform.localScale.x;
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x0008A5D4 File Offset: 0x000887D4
	public bool TryConsume()
	{
		if (this.consume != null)
		{
			this.consume();
			return true;
		}
		if (SRSingleton<SceneContext>.Instance.PlayerState.Ammo.MaybeAddToSlot(this.identifiable.id, this.identifiable))
		{
			Destroyer.DestroyActor(base.transform.gameObject, "Vacuumable.consume", false);
			return true;
		}
		return false;
	}

	// Token: 0x060023BB RID: 9147 RVA: 0x0008A636 File Offset: 0x00088836
	public void PreventCaptureFor(float seconds)
	{
		this.nextVacTime = Time.time + seconds;
	}

	// Token: 0x060023BC RID: 9148 RVA: 0x0008A645 File Offset: 0x00088845
	public bool canCapture()
	{
		return Time.time >= this.nextVacTime;
	}

	// Token: 0x060023BD RID: 9149 RVA: 0x0008A658 File Offset: 0x00088858
	public void capture(Joint toJoint)
	{
		if (this.captiveToJoint == null && this.sfAnimator != null)
		{
			this.sfAnimator.SetTrigger("triggerAwe");
		}
		if (this.body != null)
		{
			this.body.isKinematic = false;
		}
		this.SetCaptive(toJoint);
	}

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x060023BE RID: 9150 RVA: 0x0008A6B4 File Offset: 0x000888B4
	// (set) Token: 0x060023BF RID: 9151 RVA: 0x0008A701 File Offset: 0x00088901
	public bool Pending
	{
		get
		{
			if (double.IsNaN(this.lastPending))
			{
				return false;
			}
			if ((double)Time.time <= this.lastPending + (double)Time.deltaTime + 0.0010000000474974513)
			{
				return true;
			}
			this.lastPending = double.NaN;
			return false;
		}
		set
		{
			if (value)
			{
				this.lastPending = (double)Time.time;
				return;
			}
			this.lastPending = double.NaN;
		}
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x0008A722 File Offset: 0x00088922
	public void release()
	{
		if (this.isCaptive() || this.isHeld())
		{
			this.PreventCaptureFor(1f);
		}
		this.SetCaptive(null);
		this.SetHeld(false);
		this.SetTornadoed(false);
		this.Pending = false;
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x0008A75B File Offset: 0x0008895B
	public void hold()
	{
		if (this.isCaptive())
		{
			this.SetCaptive(null);
		}
		this.SetHeld(true);
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x0008A773 File Offset: 0x00088973
	public bool isCaptive()
	{
		return this.captiveToJoint != null;
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x0008A781 File Offset: 0x00088981
	public void SetTornadoed(bool tornadoed)
	{
		this.isTornadoed = tornadoed;
		this.UpdateLayer(false);
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x0008A791 File Offset: 0x00088991
	public bool IsTornadoed()
	{
		return this.isTornadoed;
	}

	// Token: 0x060023C5 RID: 9157 RVA: 0x0008A799 File Offset: 0x00088999
	public bool isHeld()
	{
		return this.held;
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x0008A7A1 File Offset: 0x000889A1
	public bool isLaunched()
	{
		return this.launched;
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x0008A7A9 File Offset: 0x000889A9
	public void Launch(Vacuumable.LaunchSource source)
	{
		if (source == Vacuumable.LaunchSource.PLAYER)
		{
			SECTR_AudioSystem.Play(this.onLaunchCue, base.transform.position, false);
		}
		this.SetLaunched(true);
	}

	// Token: 0x060023C8 RID: 9160 RVA: 0x0008A7CD File Offset: 0x000889CD
	public bool delaunch()
	{
		if (!this.launched)
		{
			return false;
		}
		this.SetLaunched(false);
		return true;
	}

	// Token: 0x060023C9 RID: 9161 RVA: 0x0008A7E1 File Offset: 0x000889E1
	public void SetDestroyOnVac(bool destroy)
	{
		this.destroyOnVac = destroy;
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x0008A7EA File Offset: 0x000889EA
	public bool GetDestroyOnVac()
	{
		return this.destroyOnVac;
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x0008A7F4 File Offset: 0x000889F4
	protected virtual void SetCaptive(Joint toJoint)
	{
		if (this.captiveToJoint != null)
		{
			Destroyer.Destroy(this.captiveToJoint.gameObject, "Vacuumable.SetCaptive");
		}
		this.captiveToJoint = toJoint;
		this.body.useGravity = (this.captiveToJoint == null && !this.ignoresGravity);
		if (this.captiveToJoint != null)
		{
			this.captiveToJoint.connectedBody = this.body;
		}
		this.UpdateLayer(false);
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x0008A878 File Offset: 0x00088A78
	private void SetHeld(bool held)
	{
		if (this.held != held)
		{
			this.held = held;
			if (this.held)
			{
				this.delaunch();
			}
			SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<WeaponVacuum>().cameraHeld.SetActive(this.held);
			this.UpdateLayer(false);
			this.UpdateMaterialsHeldState();
			if (this.onSetHeld != null)
			{
				this.onSetHeld(this.held);
			}
		}
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x0008A8E9 File Offset: 0x00088AE9
	public void ProcessCollisionEnter(Collision col)
	{
		if (this.launched && !col.collider.isTrigger && !(col.collider is CharacterController))
		{
			this.SetLaunched(false);
		}
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x0008A914 File Offset: 0x00088B14
	private void UpdateMaterialsHeldState()
	{
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
		{
			for (int j = 0; j < renderer.sharedMaterials.Length; j++)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				renderer.GetPropertyBlock(materialPropertyBlock, j);
				materialPropertyBlock.SetInt(this.heldShaderPropertyId, this.held ? 1 : 0);
				renderer.SetPropertyBlock(materialPropertyBlock, j);
			}
		}
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x0008A980 File Offset: 0x00088B80
	private void UpdateLayer(bool isForced = false)
	{
		if (this.launched && this.delaunchTrigger != null)
		{
			this.SetLayerRecursively(LayerMask.NameToLayer("Launched"), isForced);
			return;
		}
		if (this.isHeld())
		{
			this.SetLayerRecursively(LayerMask.NameToLayer("Held"), isForced);
			return;
		}
		if (this.IsTornadoed())
		{
			this.SetLayerRecursively(LayerMask.NameToLayer("ActorEchoes"), isForced);
			return;
		}
		if (this.isCaptive())
		{
			this.SetLayerRecursively(LayerMask.NameToLayer("ActorIgnorePlayer"), isForced);
			return;
		}
		this.SetLayerRecursively(this.defaultLayer, isForced);
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x0008AA10 File Offset: 0x00088C10
	public void ForceUpdateLayer()
	{
		this.UpdateLayer(true);
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x0008AA1C File Offset: 0x00088C1C
	private void SetLayerRecursively(int layerNumber, bool isForced)
	{
		if (isForced || base.gameObject.layer != layerNumber)
		{
			Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = layerNumber;
			}
		}
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x0008AA64 File Offset: 0x00088C64
	private void SetLaunched(bool launched)
	{
		if (launched != this.launched)
		{
			this.launched = launched;
			this.UpdateLayer(false);
			if (this.delaunchTrigger != null)
			{
				this.delaunchTrigger.SetTriggerEnabled(launched);
			}
			if (launched)
			{
				base.gameObject.AddComponent<DontGoThroughThings>();
			}
			else
			{
				DontGoThroughThings component = base.gameObject.GetComponent<DontGoThroughThings>();
				if (component != null)
				{
					component.AllowDestroy();
				}
			}
			if (this.onSetLaunched != null)
			{
				this.onSetLaunched(this.launched);
			}
		}
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x0008AAE7 File Offset: 0x00088CE7
	public Joint CaptiveToJoint()
	{
		return this.captiveToJoint;
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x0008AAF0 File Offset: 0x00088CF0
	public void StartConsumeScale()
	{
		if (this.consumeScaleTween == null || !this.consumeScaleTween.IsActive())
		{
			this.consumeScaleTween = base.gameObject.transform.DOScale(this.consumeStartScale * 0.1f, 0.2f).SetEase(Ease.Linear);
		}
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x0008AB40 File Offset: 0x00088D40
	public void ReverseConsumeScale()
	{
		if (this.consumeScaleTween != null && this.consumeScaleTween.IsPlaying())
		{
			this.consumeScaleTween.Flip();
		}
		else
		{
			base.gameObject.transform.DOScale(this.consumeStartScale, 0.2f).SetEase(Ease.Linear);
		}
		this.consumeScaleTween = null;
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x0008AB98 File Offset: 0x00088D98
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.captiveToJoint != null)
		{
			Destroyer.Destroy(this.captiveToJoint.gameObject, "Vacuumable.OnDestroy");
		}
		if (this.held && SRSingleton<SceneContext>.Instance != null && SRSingleton<SceneContext>.Instance.Player != null)
		{
			SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<WeaponVacuum>().cameraHeld.SetActive(false);
		}
	}

	// Token: 0x040022DA RID: 8922
	private const float CONSUME_SCALE_DOWN_TIME = 0.2f;

	// Token: 0x040022DB RID: 8923
	private const float CONSUME_SCALE_FACTOR = 0.1f;

	// Token: 0x040022DF RID: 8927
	public Vacuumable.Size size;

	// Token: 0x040022E0 RID: 8928
	[Tooltip("Audio played when this object is shot out of the vacuum.")]
	public SECTR_AudioCue onLaunchCue;

	// Token: 0x040022E1 RID: 8929
	private bool ignoresGravity;

	// Token: 0x040022E2 RID: 8930
	private Joint captiveToJoint;

	// Token: 0x040022E3 RID: 8931
	private bool held;

	// Token: 0x040022E4 RID: 8932
	private bool launched;

	// Token: 0x040022E5 RID: 8933
	private float nextVacTime;

	// Token: 0x040022E6 RID: 8934
	private bool destroyOnVac;

	// Token: 0x040022E7 RID: 8935
	private const float VAC_RELEASE_DELAY = 1f;

	// Token: 0x040022E8 RID: 8936
	private const float HELD_OPACITY = 0.75f;

	// Token: 0x040022E9 RID: 8937
	private const string LAUNCHED_LAYER = "Launched";

	// Token: 0x040022EA RID: 8938
	private const string CAPTIVE_LAYER = "ActorIgnorePlayer";

	// Token: 0x040022EB RID: 8939
	private const string TORNADOED_LAYER = "ActorEchoes";

	// Token: 0x040022EC RID: 8940
	private const string HELD_LAYER = "Held";

	// Token: 0x040022ED RID: 8941
	private int defaultLayer;

	// Token: 0x040022EE RID: 8942
	private SlimeFaceAnimator sfAnimator;

	// Token: 0x040022EF RID: 8943
	protected Rigidbody body;

	// Token: 0x040022F0 RID: 8944
	private VacDelaunchTrigger delaunchTrigger;

	// Token: 0x040022F1 RID: 8945
	private Identifiable identifiable;

	// Token: 0x040022F2 RID: 8946
	private SlimeAppearanceApplicator slimeAppearanceApplicator;

	// Token: 0x040022F3 RID: 8947
	private bool isTornadoed;

	// Token: 0x040022F4 RID: 8948
	private double lastPending = double.NaN;

	// Token: 0x040022F5 RID: 8949
	private int heldShaderPropertyId;

	// Token: 0x040022F6 RID: 8950
	private TweenerCore<Vector3, Vector3, VectorOptions> consumeScaleTween;

	// Token: 0x040022F7 RID: 8951
	private float consumeStartScale;

	// Token: 0x020006B4 RID: 1716
	public enum Size
	{
		// Token: 0x040022F9 RID: 8953
		NORMAL,
		// Token: 0x040022FA RID: 8954
		LARGE,
		// Token: 0x040022FB RID: 8955
		GIANT
	}

	// Token: 0x020006B5 RID: 1717
	public enum LaunchSource
	{
		// Token: 0x040022FD RID: 8957
		PLAYER,
		// Token: 0x040022FE RID: 8958
		CHICKEN_CLONER
	}

	// Token: 0x020006B6 RID: 1718
	// (Invoke) Token: 0x060023DB RID: 9179
	public delegate void OnSetHeld(bool held);

	// Token: 0x020006B7 RID: 1719
	// (Invoke) Token: 0x060023DF RID: 9183
	public delegate void OnSetLaunched(bool launched);

	// Token: 0x020006B8 RID: 1720
	// (Invoke) Token: 0x060023E3 RID: 9187
	public delegate void Consume();
}
