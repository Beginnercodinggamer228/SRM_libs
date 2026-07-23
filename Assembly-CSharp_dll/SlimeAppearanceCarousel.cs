using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x0200061A RID: 1562
public class SlimeAppearanceCarousel : SRBehaviour
{
	// Token: 0x1400001F RID: 31
	// (add) Token: 0x060020C2 RID: 8386 RVA: 0x0007D238 File Offset: 0x0007B438
	// (remove) Token: 0x060020C3 RID: 8387 RVA: 0x0007D270 File Offset: 0x0007B470
	public event SlimeAppearanceCarousel.OnSlimeAppearanceSelectedDelegate onSlimeAppearanceConfirmed = delegate(SlimeDefinition <p0>, SlimeAppearance <p1>)
	{
	};

	// Token: 0x060020C4 RID: 8388 RVA: 0x0007D2A5 File Offset: 0x0007B4A5
	private void Awake()
	{
		this.unscaledTimePropertyId = Shader.PropertyToID("_UnscaledTime");
	}

	// Token: 0x060020C5 RID: 8389 RVA: 0x0007D2B7 File Offset: 0x0007B4B7
	private IEnumerator ResetExpressionAfterTime(SlimeAppearanceApplicator appearanceApplicator)
	{
		yield return new WaitForSecondsRealtime(0.5f);
		if (appearanceApplicator != null && appearanceApplicator.gameObject != null && appearanceApplicator.gameObject.activeInHierarchy)
		{
			appearanceApplicator.SetExpression(SlimeFace.SlimeExpression.Happy);
		}
		yield break;
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x0007D2C8 File Offset: 0x0007B4C8
	public void ShowSlime(SlimeDefinition slime)
	{
		this.currentSlime = slime;
		foreach (GameObject obj in this.currentSlimeAppearancePreviews)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.currentSlimeAppearancePreviews.Clear();
		this.currentAppearances = slime.Appearances.ToArray<SlimeAppearance>();
		SlimeAppearance chosenSlimeAppearance = this.slimeAppearanceDirector.GetChosenSlimeAppearance(slime);
		for (int i = 0; i < Mathf.Min(this.maxAppearancesToShow, this.currentAppearances.Length); i++)
		{
			SlimeAppearance slimeAppearance = this.currentAppearances[i];
			GameObject gameObject = this.CreateAppearancePreview(slime, slimeAppearance, slimeAppearance == chosenSlimeAppearance);
			gameObject.transform.localPosition = new Vector3((float)i * this.spacing - this.spacing / 2f, 0f, 0f);
			gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			if (slimeAppearance != chosenSlimeAppearance)
			{
				gameObject.transform.localPosition = this.GetUnfocusedPosition(gameObject.transform.position);
			}
			else
			{
				this.spotlight.localPosition = new Vector3((float)i * this.spacing - this.spacing / 2f, this.spotlight.localPosition.y, this.spotlight.localPosition.z);
			}
		}
	}

	// Token: 0x060020C7 RID: 8391 RVA: 0x0007D448 File Offset: 0x0007B648
	private Vector3 GetUnfocusedPosition(Vector3 focusedPosition)
	{
		return new Vector3(focusedPosition.x * this.unselectedXScaling, 0f, this.unselectedMoveBack);
	}

	// Token: 0x060020C8 RID: 8392 RVA: 0x0007D468 File Offset: 0x0007B668
	public void ConfirmSlimeAppearance(int index)
	{
		if (this.slimeAppearanceDirector.GetChosenSlimeAppearance(this.currentSlime) == this.currentAppearances[index])
		{
			return;
		}
		this.onSlimeAppearanceConfirmed(this.currentSlime, this.currentAppearances[index]);
		GameObject gameObject = this.currentSlimeAppearancePreviews[index];
		SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
		SlimeAppearanceApplicator component = gameObject.GetComponent<SlimeAppearanceApplicator>();
		if (component != null)
		{
			component.SetExpression(SlimeFace.SlimeExpression.Elated);
			base.StartCoroutine(this.ResetExpressionAfterTime(component));
		}
		else if (componentInChildren != null)
		{
			componentInChildren.color = Color.white;
		}
		if (this.selectFx != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.selectFx, gameObject);
		}
		for (int i = 0; i < this.currentSlimeAppearancePreviews.Count; i++)
		{
			if (i != index)
			{
				GameObject gameObject2 = this.currentSlimeAppearancePreviews[i];
				SlimeAppearanceApplicator component2 = gameObject2.GetComponent<SlimeAppearanceApplicator>();
				SpriteRenderer componentInChildren2 = gameObject2.GetComponentInChildren<SpriteRenderer>();
				if (component2 == null && componentInChildren2 != null)
				{
					componentInChildren2.color = Color.grey;
				}
				gameObject2.transform.DOLocalJump(this.GetUnfocusedPosition(new Vector3((float)i * this.spacing - this.spacing / 2f, 0f, 0f)), this.jumpAmount, 1, this.transitionTime, false).SetUpdate(true);
			}
		}
		this.currentSlimeAppearancePreviews[index].transform.DOLocalJump(new Vector3((float)index * this.spacing - this.spacing / 2f, 0f, 0f), this.jumpAmount, 1, this.transitionTime, false).SetUpdate(true);
		this.spotlight.transform.DOLocalMove(new Vector3((float)index * this.spacing - this.spacing / 2f, this.spotlight.localPosition.y, this.spotlight.localPosition.z), this.transitionTime, false).SetUpdate(true);
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x0007D66C File Offset: 0x0007B86C
	private bool UseSpriteForSlime(SlimeDefinition slime)
	{
		return slime.IdentifiableId == Identifiable.Id.SABER_SLIME;
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x0007D67C File Offset: 0x0007B87C
	private GameObject CreateAppearancePreview(SlimeDefinition slime, SlimeAppearance appearance, bool isSelected)
	{
		GameObject gameObject;
		if (this.UseSpriteForSlime(slime))
		{
			gameObject = UnityEngine.Object.Instantiate<GameObject>(this.appearanceSpritePrefab, this.root);
			SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
			this.SetLayerInChildren(gameObject);
			this.currentSlimeAppearancePreviews.Add(gameObject);
			componentInChildren.sprite = (appearance.Icon ?? this.slimeAppearanceDirector.missingIcon);
			componentInChildren.color = (isSelected ? Color.white : Color.grey);
		}
		else
		{
			SlimeAppearanceApplicator slimeAppearanceApplicator = UnityEngine.Object.Instantiate<SlimeAppearanceApplicator>(this.appearancePrefab, this.root);
			slimeAppearanceApplicator.Appearance = appearance;
			slimeAppearanceApplicator.ApplyAppearance();
			this.currentSlimeAppearancePreviews.Add(slimeAppearanceApplicator.gameObject);
			this.SetLayerInChildren(slimeAppearanceApplicator.gameObject);
			foreach (Type t in this.blacklistedObjectTypes)
			{
				Component[] componentsInChildren = slimeAppearanceApplicator.GetComponentsInChildren(t);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].gameObject.SetActive(false);
				}
			}
			foreach (EnableBasedOnGrounded enableBasedOnGrounded in slimeAppearanceApplicator.GetComponentsInChildren<EnableBasedOnGrounded>())
			{
				if (enableBasedOnGrounded.enableOnGrounded)
				{
					enableBasedOnGrounded.gameObject.SetActive(false);
				}
			}
			DeactivateOnHeld[] componentsInChildren3 = slimeAppearanceApplicator.GetComponentsInChildren<DeactivateOnHeld>();
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				componentsInChildren3[i].enabled = false;
			}
			Animator[] componentsInChildren4 = slimeAppearanceApplicator.GetComponentsInChildren<Animator>();
			for (int i = 0; i < componentsInChildren4.Length; i++)
			{
				componentsInChildren4[i].updateMode = AnimatorUpdateMode.UnscaledTime;
			}
			ParticleSystem[] componentsInChildren5 = slimeAppearanceApplicator.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren5.Length; i++)
			{
				componentsInChildren5[i].main.useUnscaledTime = true;
			}
			Renderer[] componentsInChildren6 = slimeAppearanceApplicator.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren6.Length; i++)
			{
				foreach (Material material in componentsInChildren6[i].materials)
				{
					material.SetInt(this.unscaledTimePropertyId, 1);
					material.EnableKeyword("_UNSCALEDTIME_ON");
				}
			}
			slimeAppearanceApplicator.GetComponentInChildren<RubberBoneEffect>().unscaledTime = true;
			gameObject = slimeAppearanceApplicator.gameObject;
		}
		UnityEngine.Object.Instantiate<GameObject>(this.shadowPrefab, gameObject.transform);
		return gameObject;
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x0007D88C File Offset: 0x0007BA8C
	private void SetLayerInChildren(GameObject gameObject)
	{
		Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = base.gameObject.layer;
		}
	}

	// Token: 0x04002013 RID: 8211
	public Camera cam;

	// Token: 0x04002014 RID: 8212
	public Transform root;

	// Token: 0x04002015 RID: 8213
	public SlimeAppearanceApplicator appearancePrefab;

	// Token: 0x04002016 RID: 8214
	public GameObject appearanceSpritePrefab;

	// Token: 0x04002017 RID: 8215
	public float spacing = 1.5f;

	// Token: 0x04002018 RID: 8216
	public float transitionTime = 0.25f;

	// Token: 0x04002019 RID: 8217
	public SlimeAppearanceDirector slimeAppearanceDirector;

	// Token: 0x0400201A RID: 8218
	public GameObject shadowPrefab;

	// Token: 0x0400201B RID: 8219
	public Transform spotlight;

	// Token: 0x0400201C RID: 8220
	public GameObject selectFx;

	// Token: 0x0400201D RID: 8221
	public float unselectedMoveBack = 1f;

	// Token: 0x0400201E RID: 8222
	public float unselectedXScaling = 1.2f;

	// Token: 0x0400201F RID: 8223
	public float jumpAmount = 0.25f;

	// Token: 0x04002020 RID: 8224
	public int maxAppearancesToShow = 2;

	// Token: 0x04002021 RID: 8225
	private const string unscaledTimeKeyword = "_UNSCALEDTIME_ON";

	// Token: 0x04002022 RID: 8226
	private readonly Type[] blacklistedObjectTypes = new Type[]
	{
		typeof(RadExpandMarker),
		typeof(TrailRenderer)
	};

	// Token: 0x04002023 RID: 8227
	private List<GameObject> currentSlimeAppearancePreviews = new List<GameObject>();

	// Token: 0x04002024 RID: 8228
	private SlimeDefinition currentSlime;

	// Token: 0x04002025 RID: 8229
	private SlimeAppearance[] currentAppearances;

	// Token: 0x04002026 RID: 8230
	private int unscaledTimePropertyId;

	// Token: 0x0200061B RID: 1563
	// (Invoke) Token: 0x060020CE RID: 8398
	public delegate void OnSlimeAppearanceSelectedDelegate(SlimeDefinition slime, SlimeAppearance appearance);
}
