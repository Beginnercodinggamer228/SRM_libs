using System;
using UnityEngine;

// Token: 0x02000637 RID: 1591
public class UIDetector : SRBehaviour
{
	// Token: 0x06002164 RID: 8548 RVA: 0x0007F9EB File Offset: 0x0007DBEB
	public void Awake()
	{
		this.fpInput = base.GetComponentInChildren<vp_FPInput>();
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x0007F9F9 File Offset: 0x0007DBF9
	public void Start()
	{
		this.mainCamera = Camera.main;
		this.weaponVac = base.GetComponentInChildren<WeaponVacuum>();
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x0007FA12 File Offset: 0x0007DC12
	public void OnDisable()
	{
		Destroyer.Destroy(this.displayingGui, "UIDetector.OnDisable");
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x0007FA24 File Offset: 0x0007DC24
	private void Update()
	{
		Vector3 pos = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		RaycastHit raycastHit;
		Physics.Raycast(this.mainCamera.ScreenPointToRay(pos), out raycastHit, this.interactDistance);
		UIActivator uiactivator = null;
		SlimeGateActivator slimeGateActivator = null;
		TreasurePod treasurePod = null;
		TechActivator techActivator = null;
		GadgetInteractor gadgetInteractor = null;
		GadgetSite gadgetSite = null;
		if (raycastHit.collider != null)
		{
			GameObject gameObject = raycastHit.collider.gameObject;
			uiactivator = gameObject.GetComponent<UIActivator>();
			slimeGateActivator = gameObject.GetComponent<SlimeGateActivator>();
			treasurePod = gameObject.GetComponent<TreasurePod>();
			techActivator = gameObject.GetComponent<TechActivator>();
			gadgetInteractor = gameObject.GetComponentInParent<GadgetInteractor>();
			gadgetSite = gameObject.GetComponentInParent<GadgetSite>();
		}
		if (uiactivator != null && uiactivator.CanActivate() && this.InteractionEnabled())
		{
			if (false && uiactivator.blockInExpoPrefab != null)
			{
				this.MaybeInstantiateDisplayGui(uiactivator.blockInExpoPrefab);
				return;
			}
			this.MaybeInstantiateDisplayGui(this.activationGuiPrefab);
			if (SRInput.Actions.interact.WasReleased)
			{
				uiactivator.Activate();
				return;
			}
		}
		else if (slimeGateActivator != null && slimeGateActivator.gateDoor.CurrState == AccessDoor.State.LOCKED && this.InteractionEnabled())
		{
			bool flag = SRSingleton<SceneContext>.Instance.PlayerState.GetKeys() > 0;
			if (flag)
			{
				this.MaybeInstantiateDisplayGui(this.slimeGateActivationGuiPrefab);
			}
			else
			{
				this.MaybeInstantiateDisplayGui(this.slimeGateNoKeyActivationGuiPrefab);
			}
			if (flag && SRInput.Actions.interact.WasReleased)
			{
				slimeGateActivator.Activate();
				return;
			}
		}
		else if (treasurePod != null && treasurePod.CurrState == TreasurePod.State.LOCKED && this.InteractionEnabled())
		{
			bool flag2 = treasurePod.HasKey();
			bool flag3 = treasurePod.HasAnyKey();
			if (flag2)
			{
				this.MaybeInstantiateDisplayGui(this.treasurePodActivationGuiPrefab);
			}
			else if (flag3)
			{
				this.MaybeInstantiateDisplayGui(this.treasurePodInsufKeyActivationGuiPrefab);
			}
			else
			{
				this.MaybeInstantiateDisplayGui(this.treasurePodNoKeyActivationGuiPrefab);
			}
			if (flag2 && SRInput.Actions.interact.WasReleased)
			{
				treasurePod.Activate();
				return;
			}
		}
		else if (techActivator != null && this.InteractionEnabled())
		{
			GameObject customGuiPrefab = techActivator.GetCustomGuiPrefab();
			if (customGuiPrefab == null)
			{
				customGuiPrefab = this.activationGuiPrefab;
			}
			this.MaybeInstantiateDisplayGui(customGuiPrefab);
			if (SRInput.Actions.interact.WasReleased)
			{
				techActivator.Activate();
				Destroyer.Destroy(this.displayingGui, "UIDetector.Update");
				this.displayingGui = null;
				return;
			}
		}
		else if (gadgetSite != null && this.InteractionEnabled() && this.weaponVac.InGadgetMode())
		{
			if (this.MaybeInstantiateDisplayGui(this.gadgetModeActivationGuiPrefab))
			{
				RotationRowUI component = this.displayingGui.GetComponent<RotationRowUI>();
				if (component != null)
				{
					if (gadgetSite.HasAttached())
					{
						component.ShowRow();
					}
					else
					{
						component.HideRow();
					}
				}
			}
			if (SRInput.Actions.interact.WasReleased)
			{
				gadgetSite.Activate();
			}
			if (SRInput.Actions.vac)
			{
				gadgetSite.OnRotateCCW();
				return;
			}
			if (SRInput.Actions.attack)
			{
				gadgetSite.OnRotateCW();
				return;
			}
		}
		else if (gadgetInteractor != null && gadgetInteractor.CanInteract() && this.InteractionEnabled())
		{
			this.MaybeInstantiateDisplayGui(this.activationGuiPrefab);
			if (SRInput.Actions.interact.WasReleased)
			{
				gadgetInteractor.OnInteract();
				return;
			}
		}
		else if (this.displayingGui != null)
		{
			Destroyer.Destroy(this.displayingGui, "UIDetector.Update");
			this.displayingGui = null;
			this.displayingGuiPrefab = null;
		}
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x0007FD90 File Offset: 0x0007DF90
	private bool MaybeInstantiateDisplayGui(GameObject prefab)
	{
		if (this.displayingGui != null && this.displayingGuiPrefab != null && this.displayingGuiPrefab != prefab)
		{
			this.displayingGui.SetActive(false);
			Destroyer.Destroy(this.displayingGui, "UIDetector.InstantiateGuiPrefab");
			this.displayingGui = null;
			this.displayingGuiPrefab = null;
		}
		if (this.displayingGui == null)
		{
			this.displayingGui = UnityEngine.Object.Instantiate<GameObject>(prefab);
			this.displayingGuiPrefab = prefab;
			return true;
		}
		return false;
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x0007FE15 File Offset: 0x0007E015
	private bool InteractionEnabled()
	{
		return Time.timeScale > 0f && this.fpInput.enabled;
	}

	// Token: 0x040020B1 RID: 8369
	public GameObject activationGuiPrefab;

	// Token: 0x040020B2 RID: 8370
	public GameObject gadgetModeActivationGuiPrefab;

	// Token: 0x040020B3 RID: 8371
	public GameObject slimeGateActivationGuiPrefab;

	// Token: 0x040020B4 RID: 8372
	public GameObject slimeGateNoKeyActivationGuiPrefab;

	// Token: 0x040020B5 RID: 8373
	public GameObject puzzleGateActivationGuiPrefab;

	// Token: 0x040020B6 RID: 8374
	public GameObject puzzleGateLockedActivationGuiPrefab;

	// Token: 0x040020B7 RID: 8375
	public GameObject treasurePodActivationGuiPrefab;

	// Token: 0x040020B8 RID: 8376
	public GameObject treasurePodInsufKeyActivationGuiPrefab;

	// Token: 0x040020B9 RID: 8377
	public GameObject treasurePodNoKeyActivationGuiPrefab;

	// Token: 0x040020BA RID: 8378
	public float interactDistance = 2f;

	// Token: 0x040020BB RID: 8379
	private GameObject displayingGui;

	// Token: 0x040020BC RID: 8380
	private GameObject displayingGuiPrefab;

	// Token: 0x040020BD RID: 8381
	private vp_FPInput fpInput;

	// Token: 0x040020BE RID: 8382
	private Camera mainCamera;

	// Token: 0x040020BF RID: 8383
	private WeaponVacuum weaponVac;
}
