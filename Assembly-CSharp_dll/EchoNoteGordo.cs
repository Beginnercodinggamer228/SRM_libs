using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public class EchoNoteGordo : IdHandler, EchoNoteGordoModel.Participant
{
	// Token: 0x060009E5 RID: 2533 RVA: 0x0002BB50 File Offset: 0x00029D50
	public void Awake()
	{
		this.animator = base.GetComponentInChildren<Animator>();
		this.teleporter = base.GetComponentInChildren<TeleportSource>();
		SRSingleton<SceneContext>.Instance.GameModel.RegisterEchoNoteGordo(base.id, base.gameObject);
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x00003296 File Offset: 0x00001496
	public void Start()
	{
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x0002BB88 File Offset: 0x00029D88
	public void OnEnable()
	{
		if (this.model == null || !SRSingleton<SceneContext>.Instance.GameModel.GetHolidayModel().eventEchoNoteGordos.Any((HolidayModel.EventEchoNoteGordo e) => e.objectId == base.id))
		{
			this.teleporter.waitForExternalActivation = true;
			base.gameObject.SetActive(false);
			return;
		}
		if (this.model.state == EchoNoteGordoModel.State.NOT_POPPED)
		{
			this.onActiveCueInstance.Stop(true);
			this.onActiveCueInstance = SECTR_AudioSystem.Play(this.onActiveCue, this.gordo.transform.position, true);
		}
		this.onPoppingCueInstance.Pause(false);
		this.teleporter.waitForExternalActivation = (this.model.state != EchoNoteGordoModel.State.POPPED);
		this.gordo.SetActive(this.model.state != EchoNoteGordoModel.State.POPPED);
		this.ring.SetActive(this.model.state == EchoNoteGordoModel.State.POPPED);
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0002BC75 File Offset: 0x00029E75
	public void OnDisable()
	{
		this.onActiveCueInstance.Stop(true);
		this.onPoppingCueInstance.Pause(true);
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0002BC8F File Offset: 0x00029E8F
	public void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.UnregisterEchoNoteGordo(base.id);
		}
		this.onPoppingCueInstance.Stop(true);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x0002BCC0 File Offset: 0x00029EC0
	public void Update()
	{
		if (this.model.state == EchoNoteGordoModel.State.NOT_POPPED && (SRSingleton<SceneContext>.Instance.Player.transform.position - this.gordo.transform.position).sqrMagnitude <= 64f)
		{
			SRSingleton<SceneContext>.Instance.PediaDirector.MaybeShowPopup(PediaDirector.Id.ECHO_NOTE_GORDO_SLIME);
			this.model.state = EchoNoteGordoModel.State.POPPING_1;
			this.animator.SetBool("ACTIVATED", true);
		}
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0002BD43 File Offset: 0x00029F43
	protected override string IdPrefix()
	{
		return "gordoEchoNote";
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0002BD4A File Offset: 0x00029F4A
	public void InitModel(EchoNoteGordoModel model)
	{
		model.state = EchoNoteGordoModel.State.NOT_POPPED;
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0002BD53 File Offset: 0x00029F53
	public void SetModel(EchoNoteGordoModel model)
	{
		this.model = model;
		if (this.model.state < EchoNoteGordoModel.State.POPPED)
		{
			this.model.state = EchoNoteGordoModel.State.NOT_POPPED;
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0002BD78 File Offset: 0x00029F78
	public void OnAnimationEvent_StateEnter(EchoNoteGordoAnimatorState.Id id)
	{
		if (id == EchoNoteGordoAnimatorState.Id.ACTIVATION && this.model.state == EchoNoteGordoModel.State.POPPING_1)
		{
			this.model.state = EchoNoteGordoModel.State.POPPING_2;
			this.onActiveCueInstance.Stop(false);
			this.onPoppingCueInstance.Stop(true);
			this.onPoppingCueInstance = SECTR_AudioSystem.Play(this.onPoppingCue, this.gordo.transform.position, false);
		}
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x0002BDDD File Offset: 0x00029FDD
	public void OnAnimationEvent_StateExit(EchoNoteGordoAnimatorState.Id id)
	{
		if (id == EchoNoteGordoAnimatorState.Id.ACTIVATION && this.model.state == EchoNoteGordoModel.State.POPPED)
		{
			this.teleporter.waitForExternalActivation = false;
			this.gordo.SetActive(false);
			this.ring.SetActive(true);
		}
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x0002BE18 File Offset: 0x0002A018
	public void OnAnimationEvent_Popped()
	{
		if (this.model.state == EchoNoteGordoModel.State.POPPING_2)
		{
			this.model.state = EchoNoteGordoModel.State.POPPED;
			AnalyticsUtil.CustomEvent("TwinkleSlimeBurst", new Dictionary<string, object>
			{
				{
					"type",
					base.name
				},
				{
					"twinkleId",
					base.id
				}
			}, true);
			RegionRegistry.RegionSetId setId = base.GetComponentInParent<Region>().setId;
			SRSingleton<SceneContext>.Instance.InstrumentDirector.UnlockNextInstrument();
			foreach (InstrumentModel.Instrument instrument in this.bonusInstruments)
			{
				SRSingleton<SceneContext>.Instance.InstrumentDirector.UnlockInstrument(instrument);
			}
			EchoNoteMetadata[] componentsInChildren = base.GetComponentsInChildren<EchoNoteMetadata>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				EchoNoteMetadata echoNoteMetadata = componentsInChildren[i];
				GameObject gameObject = SRBehaviour.InstantiateActor(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(echoNoteMetadata.id), setId, echoNoteMetadata.transform.position, echoNoteMetadata.transform.rotation, false);
				float inRange = Randoms.SHARED.GetInRange(0f, 1f);
				Renderer chimeRenderer = gameObject.GetComponentInChildren<Renderer>();
				chimeRenderer.gameObject.AddComponent<PauseTweenOnDisable>().tween = DOTween.To(() => this.GetMaterialFade(chimeRenderer), delegate(float fade)
				{
					this.SetMaterialFade(chimeRenderer, fade);
				}, 1f, 3f).From(0f, true).SetDelay(inRange);
			}
			this.teleporter.waitForExternalActivation = false;
			this.ring.SetActive(true);
		}
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0002BFB0 File Offset: 0x0002A1B0
	private float GetMaterialFade(Renderer targetRenderer)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		targetRenderer.GetPropertyBlock(materialPropertyBlock);
		return materialPropertyBlock.GetFloat(EchoNoteGordo.PROPERTY_FADE);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0002BFD8 File Offset: 0x0002A1D8
	private void SetMaterialFade(Renderer targetRenderer, float fade)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		targetRenderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(EchoNoteGordo.PROPERTY_FADE, fade);
		targetRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x0002C008 File Offset: 0x0002A208
	public void PrepareWorldGenerated()
	{
		GameObject gameObject = new GameObject("cluster_notes_metadata");
		gameObject.transform.SetParent(base.transform, false);
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			EchoNote[] componentsInChildren = base.transform.GetChild(i).GetComponentsInChildren<EchoNote>(true);
			if (componentsInChildren.Length != 0)
			{
				foreach (EchoNote echoNote in componentsInChildren)
				{
					GameObject gameObject2 = new GameObject(string.Format("echoNote{0}", echoNote.clip.ToString("D2")));
					gameObject2.transform.SetParent(gameObject.transform, false);
					gameObject2.transform.position = echoNote.transform.position;
					gameObject2.AddComponent<EchoNoteMetadata>().id = Identifiable.Id.ECHO_NOTE_01 + echoNote.clip - 1;
				}
				if (Application.isPlaying)
				{
					Destroyer.Destroy(base.transform.GetChild(i).gameObject, 0f, "EchoNoteGordo.Start", true, false);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
				}
			}
		}
		bool isPlaying = Application.isPlaying;
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x0002C12D File Offset: 0x0002A32D
	public void OnDrawGizmosSelected()
	{
		if (this.gordo != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(this.gordo.transform.position, 8f);
		}
	}

	// Token: 0x04000834 RID: 2100
	[Tooltip("Parent GameObject containing the gordo model.")]
	public GameObject gordo;

	// Token: 0x04000835 RID: 2101
	[Tooltip("SFX played when the EchoNoteGordo is active.")]
	public SECTR_AudioCue onActiveCue;

	// Token: 0x04000836 RID: 2102
	private SECTR_AudioCueInstance onActiveCueInstance;

	// Token: 0x04000837 RID: 2103
	[Tooltip("SFX played when the EchoNoteGordo is popping.")]
	public SECTR_AudioCue onPoppingCue;

	// Token: 0x04000838 RID: 2104
	private SECTR_AudioCueInstance onPoppingCueInstance;

	// Token: 0x04000839 RID: 2105
	[Tooltip("Parent GameObject containing the portal ring.")]
	public GameObject ring;

	// Token: 0x0400083A RID: 2106
	[Tooltip("Instruments unlocked in addition to the next instrument in the unlock sequence.")]
	public InstrumentModel.Instrument[] bonusInstruments;

	// Token: 0x0400083B RID: 2107
	private const float POPPING_DISTANCE = 8f;

	// Token: 0x0400083C RID: 2108
	private const float POPPING_DISTANCE_SQR = 64f;

	// Token: 0x0400083D RID: 2109
	private static readonly int PROPERTY_FADE = Shader.PropertyToID("_Fade");

	// Token: 0x0400083E RID: 2110
	private EchoNoteGordoModel model;

	// Token: 0x0400083F RID: 2111
	private Animator animator;

	// Token: 0x04000840 RID: 2112
	private TeleportSource teleporter;
}
