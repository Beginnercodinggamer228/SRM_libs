using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000419 RID: 1049
public class PlayWithToys : FindConsumable
{
	// Token: 0x060015E5 RID: 5605 RVA: 0x00054E8C File Offset: 0x0005308C
	public override void Awake()
	{
		base.Awake();
		this.toysDict = new Dictionary<Identifiable.Id, DriveCalculator>(Identifiable.idComparer);
		DriveCalculator value = new PlayWithToys.ToyDriveCalculator(this.slimeDefinition.FavoriteToys);
		foreach (Identifiable.Id key in Identifiable.TOY_CLASS)
		{
			this.toysDict[key] = value;
		}
		DestroyOnTouching component = base.GetComponent<DestroyOnTouching>();
		if (component != null && !component.touchingWaterOkay && !component.touchingAshOkay)
		{
			this.neverPlayWithToys = true;
		}
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x00054F34 File Offset: 0x00053134
	protected override Dictionary<Identifiable.Id, DriveCalculator> GetSearchIds()
	{
		return this.toysDict;
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x00054F3C File Offset: 0x0005313C
	public override float Relevancy(bool isGrounded)
	{
		if (Time.time < this.nextPlayTime)
		{
			return 0f;
		}
		if (this.neverPlayWithToys)
		{
			return 0f;
		}
		PlayWithToys.localStaticToyEntries.Clear();
		CellDirector.GetToysNearMember(this.member, PlayWithToys.localStaticToyEntries);
		this.target = base.FindNearestConsumable(PlayWithToys.localStaticToyEntries, out this.currDrive);
		if (this.target == null)
		{
			return 0f;
		}
		this.currDrive = Randoms.SHARED.GetFloat(0.8f);
		if (!(this.target == null) && (!this.onlyFloatingToys || DragFloatReactor.IsFloating(this.target)))
		{
			return this.currDrive * this.currDrive;
		}
		return 0f;
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x00054FFC File Offset: 0x000531FC
	public override void Action()
	{
		if (this.target == null)
		{
			return;
		}
		if (this.onlyFloatingToys && !DragFloatReactor.IsFloating(this.target))
		{
			return;
		}
		if (base.IsGrounded())
		{
			Vector3 vector = SlimeSubbehaviour.GetGotoPos(this.target) - base.transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			Vector3 normalized = vector.normalized;
			base.RotateTowards(normalized);
			float num = 1.2f;
			float d = Mathf.Sqrt(Mathf.Sqrt(sqrMagnitude) * Physics.gravity.magnitude) * num;
			base.GetComponent<Rigidbody>().AddForce((normalized + Vector3.up).normalized * d, ForceMode.VelocityChange);
			this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
			this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
			this.target = null;
			this.nextPlayTime = Time.fixedTime + 5f;
		}
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x00003296 File Offset: 0x00001496
	public override void Selected()
	{
	}

	// Token: 0x040014CE RID: 5326
	[Tooltip("We use the SlimeDefinition to find the slime's favorite toys.")]
	public SlimeDefinition slimeDefinition;

	// Token: 0x040014CF RID: 5327
	[Tooltip("Should we only play with toys that are floating")]
	public bool onlyFloatingToys;

	// Token: 0x040014D0 RID: 5328
	private GameObject target;

	// Token: 0x040014D1 RID: 5329
	private float currDrive;

	// Token: 0x040014D2 RID: 5330
	private float nextPlayTime;

	// Token: 0x040014D3 RID: 5331
	private Dictionary<Identifiable.Id, DriveCalculator> toysDict;

	// Token: 0x040014D4 RID: 5332
	private const float POUNCE_DIST = 8f;

	// Token: 0x040014D5 RID: 5333
	private const float POUNCE_DIST_SQR = 64f;

	// Token: 0x040014D6 RID: 5334
	private const float PLAY_RESET_TIME = 5f;

	// Token: 0x040014D7 RID: 5335
	private bool neverPlayWithToys;

	// Token: 0x040014D8 RID: 5336
	private static List<GameObjectActorModelIdentifiableIndex.Entry> localStaticToyEntries = new List<GameObjectActorModelIdentifiableIndex.Entry>();

	// Token: 0x0200041A RID: 1050
	private class ToyDriveCalculator : DriveCalculator
	{
		// Token: 0x060015EC RID: 5612 RVA: 0x0005510D File Offset: 0x0005330D
		public ToyDriveCalculator(Identifiable.Id[] favoriteToys) : base(SlimeEmotions.Emotion.NONE, 0f, 0f)
		{
			this.favoriteToys = favoriteToys;
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x00055127 File Offset: 0x00053327
		public override float Drive(SlimeEmotions emotions, Identifiable.Id id)
		{
			return base.Drive(emotions, id) * (this.favoriteToys.Contains(id) ? 1f : 0.5f);
		}

		// Token: 0x040014D9 RID: 5337
		private Identifiable.Id[] favoriteToys;
	}
}
