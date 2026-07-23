using System;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using UnityEngine;

// Token: 0x020003C5 RID: 965
[RequireComponent(typeof(GordoFaceAnimator))]
public class EventGordoEat : GordoEat
{
	// Token: 0x06001422 RID: 5154 RVA: 0x0004DEC8 File Offset: 0x0004C0C8
	public void OnEnable()
	{
		if (Application.isPlaying)
		{
			if (this.gordoModel == null || this.gordoModel.gordoEatenCount == -1 || !SRSingleton<SceneContext>.Instance.GameModel.GetHolidayModel().eventGordos.Any((HolidayModel.EventGordo e) => e.objectId == base.id))
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.onActiveCueInstance.Stop(false);
			this.onActiveCueInstance = SECTR_AudioSystem.Play(this.onActiveCue, base.transform.position, true);
		}
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x0004DF4F File Offset: 0x0004C14F
	public void OnDisable()
	{
		if (Application.isPlaying)
		{
			this.onActiveCueInstance.Stop(false);
		}
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0004DF64 File Offset: 0x0004C164
	public override void SetModel(GordoModel model)
	{
		this.gordoModel = model;
		if (this.gordoModel.gordoEatenCount != -1)
		{
			base.SetModel(model);
		}
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0004DF82 File Offset: 0x0004C182
	protected override PediaDirector.Id GetPediaId()
	{
		return PediaDirector.Id.PARTY_GORDO_SLIME;
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0004DF89 File Offset: 0x0004C189
	protected override void DidCompleteBurst()
	{
		base.DidCompleteBurst();
		new GameObject("EventGordoMusic")
		{
			transform = 
			{
				position = base.transform.position
			}
		}.AddComponent<EventGordoEat.EventGordoMusic>();
	}

	// Token: 0x040012D7 RID: 4823
	[Tooltip("SFX played when the EventGordo is enabled.")]
	public SECTR_AudioCue onActiveCue;

	// Token: 0x040012D8 RID: 4824
	private SECTR_AudioCueInstance onActiveCueInstance;

	// Token: 0x020003C6 RID: 966
	private class EventGordoMusic : MonoBehaviour
	{
		// Token: 0x06001429 RID: 5161 RVA: 0x0004DFC0 File Offset: 0x0004C1C0
		public void Awake()
		{
			this.player = SRSingleton<SceneContext>.Instance.Player;
			this.music = SRSingleton<GameContext>.Instance.MusicDirector;
			this.music.SetEventGordoMode(true);
			this.time = Time.unscaledTime + this.music.eventGordoMusic.MinClipLength() - this.music.eventGordoMusic.FadeOutTime;
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x0004E028 File Offset: 0x0004C228
		public void Update()
		{
			if (Time.unscaledTime >= this.time || (base.transform.position - this.player.transform.position).sqrMagnitude >= 900f)
			{
				Destroyer.Destroy(base.gameObject, "EventGordoMusic.Update");
			}
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x0004E081 File Offset: 0x0004C281
		public void OnDestroy()
		{
			this.music.SetEventGordoMode(false);
		}

		// Token: 0x040012D9 RID: 4825
		private const float MAX_DISTANCE = 30f;

		// Token: 0x040012DA RID: 4826
		private const float MAX_DISTANCE_SQR = 900f;

		// Token: 0x040012DB RID: 4827
		private GameObject player;

		// Token: 0x040012DC RID: 4828
		private MusicDirector music;

		// Token: 0x040012DD RID: 4829
		private float time;
	}
}
