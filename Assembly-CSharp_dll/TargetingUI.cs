using System;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x0200062A RID: 1578
public class TargetingUI : SRSingleton<TargetingUI>
{
	// Token: 0x0600211D RID: 8477 RVA: 0x0007E830 File Offset: 0x0007CA30
	public void Start()
	{
		SRSingleton<GameContext>.Instance.MessageDirector.RegisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		this.player = SRSingleton<SceneContext>.Instance.PlayerState;
		this.pediaDir = SRSingleton<SceneContext>.Instance.PediaDirector;
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x0007E86D File Offset: 0x0007CA6D
	public void OnBundlesAvailable(MessageDirector msgDir)
	{
		this.uiBundle = msgDir.GetBundle("ui");
		this.pediaBundle = msgDir.GetBundle("pedia");
		this.currentTarget = null;
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x0007E898 File Offset: 0x0007CA98
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SRSingleton<GameContext>.Instance != null)
		{
			SRSingleton<GameContext>.Instance.MessageDirector.UnregisterBundlesListener(new MessageDirector.BundlesListener(this.OnBundlesAvailable));
		}
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x0007E8C8 File Offset: 0x0007CAC8
	public void Update()
	{
		GameObject targeting = this.player.Targeting;
		if (targeting != null && this.currentTarget == targeting)
		{
			this.holdInfoUntil = Time.time + 1f;
			return;
		}
		this.currentTarget = null;
		if (targeting != null && (this.GetIdentifiableTarget(targeting) || this.GetGordoIdentifiableTarget(targeting) || this.GetDroneTarget(targeting)))
		{
			this.holdInfoUntil = Time.time + 1f;
			this.currentTarget = targeting;
		}
		bool enabled = Time.time <= this.holdInfoUntil;
		this.nameText.enabled = enabled;
		this.infoText.enabled = enabled;
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x0007E978 File Offset: 0x0007CB78
	private bool GetIdentifiableTarget(GameObject gameObject)
	{
		Identifiable.Id id = Identifiable.GetId(gameObject);
		if (id != Identifiable.Id.NONE)
		{
			if (Identifiable.IsPlort(id))
			{
				this.nameText.text = Identifiable.GetName(id, true);
				this.infoText.text = this.uiBundle.Get("m.hudinfo_plort");
				return true;
			}
			if (Identifiable.IsEcho(id))
			{
				this.nameText.text = Identifiable.GetName(id, true);
				this.infoText.text = this.uiBundle.Get("m.hudinfo_echo");
				return true;
			}
			if (Identifiable.IsEchoNote(id))
			{
				this.nameText.text = Identifiable.GetName(id, true);
				this.infoText.text = this.uiBundle.Get("m.hudinfo_echo_note");
				return true;
			}
			if (Identifiable.IsOrnament(id))
			{
				this.nameText.text = Identifiable.GetName(id, true);
				this.infoText.text = this.uiBundle.Get("m.hudinfo_ornament");
				return true;
			}
			if (Identifiable.IsToy(id))
			{
				this.nameText.text = Identifiable.GetName(id, true);
				if (id == Identifiable.Id.KOOKADOBA_BALL)
				{
					this.infoText.text = this.uiBundle.Get("m.hudinfo_fruitball");
				}
				else
				{
					this.infoText.text = this.uiBundle.Get("m.hudinfo_toy");
				}
				return true;
			}
			if (this.pediaDir.GetPediaId(id) != null)
			{
				this.nameText.text = Identifiable.GetName(id, true);
				this.infoText.text = this.GetIdentifiableInfoText(id);
				return true;
			}
			if (Identifiable.IsTarr(id))
			{
				this.nameText.text = Identifiable.GetName(Identifiable.Id.TARR_SLIME, true);
				this.infoText.text = this.GetIdentifiableInfoText(Identifiable.Id.TARR_SLIME);
				return true;
			}
			if (Identifiable.IsSlime(id))
			{
				this.nameText.text = Identifiable.GetName(id, true);
				this.infoText.text = this.GetIdentifiableInfoText(id);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x0007EB68 File Offset: 0x0007CD68
	private string GetIdentifiableInfoText(Identifiable.Id identId)
	{
		SlimeEat component = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(identId).GetComponent<SlimeEat>();
		if (Identifiable.IsTarr(identId))
		{
			return this.uiBundle.Xlate(MessageUtil.Compose("m.hudinfo_diet", new string[]
			{
				"m.foodgroup.tarr"
			}));
		}
		if (identId == Identifiable.Id.PUDDLE_SLIME)
		{
			return this.uiBundle.Xlate(MessageUtil.Compose("m.hudinfo_diet", new string[]
			{
				"m.foodgroup.water"
			}));
		}
		if (identId == Identifiable.Id.FIRE_SLIME)
		{
			return this.uiBundle.Xlate(MessageUtil.Compose("m.hudinfo_diet", new string[]
			{
				"m.foodgroup.ash"
			}));
		}
		if (component != null)
		{
			return this.uiBundle.Xlate(MessageUtil.Compose("m.hudinfo_diet", new string[]
			{
				component.slimeDefinition.Diet.GetModulesFoodGroupsMsg()
			}));
		}
		return this.uiBundle.Xlate(MessageUtil.Compose("m.hudinfo_type", new string[]
		{
			SlimeDiet.GetFoodCategoryMsg(identId)
		}));
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x0007EC68 File Offset: 0x0007CE68
	private bool GetGordoIdentifiableTarget(GameObject gameObject)
	{
		GordoIdentifiable component = gameObject.GetComponent<GordoIdentifiable>();
		GordoEat component2 = gameObject.GetComponent<GordoEat>();
		if (component != null && component2 != null && Identifiable.IsGordo(component.id))
		{
			this.nameText.text = Identifiable.GetName(component.id, true);
			this.infoText.text = this.uiBundle.Xlate(MessageUtil.Compose("m.hudinfo_diet", new string[]
			{
				component2.GetDirectFoodGroupsMsg()
			}));
			return true;
		}
		return false;
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x0007ECEC File Offset: 0x0007CEEC
	private bool GetDroneTarget(GameObject gameObject)
	{
		Drone component = gameObject.GetComponent<Drone>();
		if (component != null)
		{
			this.nameText.text = this.pediaBundle.Get("m.gadget.name.drone");
			this.infoText.text = string.Join(", ", (from p in component.gadget.programs
			where p.IsComplete()
			select p.target.GetName()).ToArray<string>());
			return true;
		}
		return false;
	}

	// Token: 0x04002077 RID: 8311
	public TMP_Text nameText;

	// Token: 0x04002078 RID: 8312
	public TMP_Text infoText;

	// Token: 0x04002079 RID: 8313
	private PediaDirector pediaDir;

	// Token: 0x0400207A RID: 8314
	private MessageBundle uiBundle;

	// Token: 0x0400207B RID: 8315
	private MessageBundle pediaBundle;

	// Token: 0x0400207C RID: 8316
	private PlayerState player;

	// Token: 0x0400207D RID: 8317
	private float holdInfoUntil;

	// Token: 0x0400207E RID: 8318
	private const float HOLD_DURATION = 1f;

	// Token: 0x0400207F RID: 8319
	private GameObject currentTarget;
}
