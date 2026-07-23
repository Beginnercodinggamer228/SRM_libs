using System;
using System.Collections.Generic;
using Assets.Script.Util.Extensions;
using UnityEngine;

// Token: 0x0200044A RID: 1098
[ExecuteInEditMode]
public class SlimeAppearanceApplicator : MonoBehaviour
{
	// Token: 0x14000018 RID: 24
	// (add) Token: 0x060016A7 RID: 5799 RVA: 0x00057F80 File Offset: 0x00056180
	// (remove) Token: 0x060016A8 RID: 5800 RVA: 0x00057FB8 File Offset: 0x000561B8
	public event SlimeAppearanceApplicator.OnAppearanceChangedDelegate OnAppearanceChanged = delegate(SlimeAppearance <p0>)
	{
	};

	// Token: 0x060016A9 RID: 5801 RVA: 0x00057FF0 File Offset: 0x000561F0
	public void Initialize(bool force = false)
	{
		if (this._isInitialized && !force)
		{
			return;
		}
		if (this._boneLookup == null)
		{
			this._boneLookup = new Dictionary<SlimeAppearance.SlimeBone, GameObject>(SlimeAppearance.DefaultBoneComparer);
		}
		else
		{
			this._boneLookup.Clear();
		}
		foreach (SlimeAppearanceApplicator.BoneMapping boneMapping in this.Bones)
		{
			if (this._boneLookup.ContainsKey(boneMapping.Bone))
			{
				Log.Error("Duplicate bone in SlimeAppearanceApplicator: {0}", new object[]
				{
					boneMapping.Bone
				});
			}
			else
			{
				this._boneLookup.Add(boneMapping.Bone, boneMapping.BoneObject);
			}
		}
	}

	// Token: 0x060016AA RID: 5802 RVA: 0x00058098 File Offset: 0x00056298
	public void Awake()
	{
		if (this.SlimeDefinition != null && this.SlimeAppearanceDirector != null)
		{
			SlimeAppearance chosenSlimeAppearance = this.SlimeAppearanceDirector.GetChosenSlimeAppearance(this.SlimeDefinition);
			if (this.Appearance != chosenSlimeAppearance)
			{
				this.Appearance = chosenSlimeAppearance;
			}
			this.SlimeAppearanceDirector.onSlimeAppearanceChanged += this.HandleChosenAppearanceChanged;
			this.animator = this.RootAppearanceObject.GetRequiredComponent<Animator>();
			this.ApplyAppearance();
		}
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x00058118 File Offset: 0x00056318
	public void OnDestroy()
	{
		if (this.AppearanceObjectProvider != null)
		{
			foreach (SlimeAppearanceApplicator.AppearanceObjectPair appearanceObjectPair in this._currentAppearanceObjects)
			{
				if (appearanceObjectPair.AppearanceObject != null)
				{
					this.AppearanceObjectProvider.Put(appearanceObjectPair.Prefab, appearanceObjectPair.AppearanceObject);
				}
			}
		}
		if (this.SlimeAppearanceDirector != null)
		{
			this.SlimeAppearanceDirector.onSlimeAppearanceChanged -= this.HandleChosenAppearanceChanged;
		}
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x000581B8 File Offset: 0x000563B8
	public void ApplyAppearance()
	{
		this.Initialize(false);
		if (this.AppearanceObjectProvider == null)
		{
			ObjectPool appearanceObjectPool = SRSingleton<SceneContext>.Instance.appearanceObjectPool;
			this.AppearanceObjectProvider = new PooledSlimeAppearanceObjectProvider(appearanceObjectPool);
		}
		this.ClearAppearance();
		if (this.Appearance == null)
		{
			return;
		}
		if (this.animator != null)
		{
			if (this.Appearance.AnimatorOverride != null)
			{
				this.animator.runtimeAnimatorController = this.Appearance.AnimatorOverride;
			}
			else
			{
				this.animator.runtimeAnimatorController = this.SlimeAppearanceDirector.defaultAnimatorController;
			}
		}
		List<Renderer>[] array = new List<Renderer>[4];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new List<Renderer>();
		}
		foreach (SlimeAppearanceStructure appearanceStructure in this.Appearance.Structures)
		{
			this.ApplyAppearanceStructure(appearanceStructure, array);
		}
		LOD[] lods = this.LODGroup.GetLODs();
		for (int k = 0; k < lods.Length; k++)
		{
			lods[k].renderers = array[k].ToArray();
		}
		this.LODGroup.SetLODs(lods);
		SlimeAppearanceApplicator.RecalculateBoundsHelper.RecalculateBounds(this);
		this.SetExpression(SlimeFace.SlimeExpression.Happy);
		this.OnAppearanceChanged(this.Appearance);
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x000582FC File Offset: 0x000564FC
	public Transform GetFashionParent(Fashion.Slot fashionSlot)
	{
		if (fashionSlot == Fashion.Slot.TOP)
		{
			return this._boneLookup[SlimeAppearance.SlimeBone.JiggleTop].transform;
		}
		if (fashionSlot != Fashion.Slot.FRONT)
		{
			Log.Error("Unhandled fashion slot", new object[]
			{
				"slot",
				fashionSlot
			});
			return null;
		}
		return this._boneLookup[SlimeAppearance.SlimeBone.JiggleBack].transform;
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x00058359 File Offset: 0x00056559
	private void HandleChosenAppearanceChanged(SlimeDefinition definition, SlimeAppearance newAppearance)
	{
		if (this.SlimeDefinition == definition)
		{
			this.Appearance = newAppearance;
			this.ApplyAppearance();
		}
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x00058378 File Offset: 0x00056578
	private void ClearAppearance()
	{
		foreach (SlimeAppearanceApplicator.AppearanceObjectPair appearanceObjectPair in this._currentAppearanceObjects)
		{
			this.AppearanceObjectProvider.Put(appearanceObjectPair.Prefab, appearanceObjectPair.AppearanceObject);
		}
		this._currentAppearanceObjects.Clear();
		this._faceRenderers.Clear();
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x000583F4 File Offset: 0x000565F4
	private void ApplyAppearanceStructure(SlimeAppearanceStructure appearanceStructure, List<Renderer>[] lods)
	{
		for (int i = 0; i < appearanceStructure.Element.Prefabs.Length; i++)
		{
			this.ApplyAppearanceObject(appearanceStructure, appearanceStructure.Element, appearanceStructure.Element.Prefabs[i], i, lods);
		}
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x00058438 File Offset: 0x00056638
	private void ApplyAppearanceObject(SlimeAppearanceStructure structure, SlimeAppearanceElement element, SlimeAppearanceObject appearancePrefab, int objectIndex, List<Renderer>[] lods)
	{
		GameObject gameObject = this.RootAppearanceObject;
		if (appearancePrefab.ParentBone != SlimeAppearance.SlimeBone.None)
		{
			gameObject = this._boneLookup.Get(appearancePrefab.ParentBone);
			if (gameObject == null)
			{
				Log.Error("Unable to find ParentBone for element.", new object[]
				{
					"ParentBone",
					appearancePrefab.ParentBone,
					"AppearanceObject",
					appearancePrefab.name
				});
				return;
			}
		}
		SlimeAppearanceObject slimeAppearanceObject = null;
		try
		{
			slimeAppearanceObject = this.AppearanceObjectProvider.Get(appearancePrefab, gameObject);
		}
		catch (Exception ex)
		{
			Log.Error("caught exception e", new object[]
			{
				"prefab",
				appearancePrefab,
				"exception",
				ex
			});
			throw;
		}
		this._currentAppearanceObjects.Add(new SlimeAppearanceApplicator.AppearanceObjectPair(appearancePrefab, slimeAppearanceObject));
		Renderer component = slimeAppearanceObject.GetComponent<Renderer>();
		if (component != null)
		{
			int num = 0;
			if (structure.SupportsFaces)
			{
				SlimeFaceRules slimeFaceRules = structure.FaceRules[objectIndex];
				if (slimeFaceRules.ShowEyes || slimeFaceRules.ShowMouth)
				{
					this._faceRenderers.Add(new SlimeAppearanceApplicator.FaceRenderer
					{
						Renderer = component,
						ShowEyes = structure.FaceRules[objectIndex].ShowEyes,
						ShowMouth = structure.FaceRules[objectIndex].ShowMouth
					});
				}
				num += (slimeFaceRules.ShowEyes ? 1 : 0) + (slimeFaceRules.ShowMouth ? 1 : 0);
			}
			Material[] array = structure.ElementMaterials[objectIndex].OverrideDefaults ? structure.ElementMaterials[objectIndex].Materials : structure.DefaultMaterials;
			num += array.Length;
			Material[] array2 = new Material[num];
			Array.Copy(array, array2, array.Length);
			component.materials = array2;
			if (component is SkinnedMeshRenderer)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = component as SkinnedMeshRenderer;
				Transform[] array3 = new Transform[appearancePrefab.AttachedBones.Length];
				for (int i = 0; i < appearancePrefab.AttachedBones.Length; i++)
				{
					array3[i] = this._boneLookup[appearancePrefab.AttachedBones[i]].transform;
				}
				skinnedMeshRenderer.bones = array3;
				skinnedMeshRenderer.rootBone = this._boneLookup[appearancePrefab.RootBone].transform;
			}
			if (!appearancePrefab.IgnoreLODIndex)
			{
				lods[appearancePrefab.LODIndex].Add(component);
			}
		}
		if (appearancePrefab.AttachRubberBoneEffect && component is SkinnedMeshRenderer)
		{
			RubberBoneEffect component2 = this.RootAppearanceObject.GetComponent<RubberBoneEffect>();
			component2.skinRenderer = (component as SkinnedMeshRenderer);
			component2.Presets = appearancePrefab.RubberType;
		}
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x000586D4 File Offset: 0x000568D4
	public void SetExpression(SlimeFace.SlimeExpression slimeExpression)
	{
		SlimeExpressionFace expressionFace = this.Appearance.Face.GetExpressionFace(slimeExpression);
		foreach (SlimeAppearanceApplicator.FaceRenderer faceRenderer in this._faceRenderers)
		{
			Material[] sharedMaterials = faceRenderer.Renderer.sharedMaterials;
			int num = sharedMaterials.Length - 2;
			int num2 = sharedMaterials.Length - 1;
			if (faceRenderer.ShowEyes != faceRenderer.ShowMouth)
			{
				num = num2;
			}
			if (faceRenderer.ShowEyes && expressionFace.Eyes != null)
			{
				sharedMaterials[num] = expressionFace.Eyes;
			}
			if (faceRenderer.ShowMouth && expressionFace.Mouth != null)
			{
				sharedMaterials[num2] = expressionFace.Mouth;
			}
			faceRenderer.Renderer.sharedMaterials = sharedMaterials;
		}
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x000587B0 File Offset: 0x000569B0
	public SlimeAppearance.Palette GetAppearancePalette()
	{
		if (this.Appearance == null)
		{
			Log.Warning("Appearance was null when retrieving appearance palette. Returning default palette", Array.Empty<object>());
			return SlimeAppearance.Palette.Default;
		}
		return this.Appearance.ColorPalette;
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x000587E0 File Offset: 0x000569E0
	private void SetHideFlags(GameObject gameObject)
	{
		gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
		if (gameObject.transform.childCount > 0)
		{
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
			}
		}
	}

	// Token: 0x040015F2 RID: 5618
	public SlimeAppearanceDirector SlimeAppearanceDirector;

	// Token: 0x040015F3 RID: 5619
	public SlimeAppearance Appearance;

	// Token: 0x040015F4 RID: 5620
	public SlimeDefinition SlimeDefinition;

	// Token: 0x040015F5 RID: 5621
	public SlimeAppearanceApplicator.BoneMapping[] Bones;

	// Token: 0x040015F6 RID: 5622
	public SlimeAppearanceObjectProvider AppearanceObjectProvider;

	// Token: 0x040015F7 RID: 5623
	public LODGroup LODGroup;

	// Token: 0x040015F8 RID: 5624
	public GameObject RootAppearanceObject;

	// Token: 0x040015F9 RID: 5625
	private Dictionary<SlimeAppearance.SlimeBone, GameObject> _boneLookup;

	// Token: 0x040015FA RID: 5626
	private List<SlimeAppearanceApplicator.AppearanceObjectPair> _currentAppearanceObjects = new List<SlimeAppearanceApplicator.AppearanceObjectPair>();

	// Token: 0x040015FB RID: 5627
	private List<SlimeAppearanceApplicator.FaceRenderer> _faceRenderers = new List<SlimeAppearanceApplicator.FaceRenderer>();

	// Token: 0x040015FC RID: 5628
	private const int EYES_MATERIAL_INDEX_OFFSET = 2;

	// Token: 0x040015FD RID: 5629
	private const int MOUTH_MATERIAL_INDEX_OFFSET = 1;

	// Token: 0x040015FE RID: 5630
	private const int LOD_GROUP_LEVEL_COUNT = 4;

	// Token: 0x040015FF RID: 5631
	private bool _isInitialized;

	// Token: 0x04001600 RID: 5632
	public SlimeFace.SlimeExpression SlimeExpression = SlimeFace.SlimeExpression.Happy;

	// Token: 0x04001601 RID: 5633
	private Animator animator;

	// Token: 0x04001602 RID: 5634
	private SlimeAppearanceApplicator.RecalculateBoundsHelper recalculateBoundsHelper;

	// Token: 0x04001603 RID: 5635
	private SlimeAnimatorStateIdle animatorState;

	// Token: 0x0200044B RID: 1099
	// (Invoke) Token: 0x060016B7 RID: 5815
	public delegate void OnAppearanceChangedDelegate(SlimeAppearance newAppearance);

	// Token: 0x0200044C RID: 1100
	[Serializable]
	public struct AppearanceObjectPair
	{
		// Token: 0x060016BA RID: 5818 RVA: 0x0005887E File Offset: 0x00056A7E
		public AppearanceObjectPair(SlimeAppearanceObject prefab, SlimeAppearanceObject appearanceObject)
		{
			this.Prefab = prefab;
			this.AppearanceObject = appearanceObject;
		}

		// Token: 0x04001604 RID: 5636
		public SlimeAppearanceObject Prefab;

		// Token: 0x04001605 RID: 5637
		public SlimeAppearanceObject AppearanceObject;
	}

	// Token: 0x0200044D RID: 1101
	[Serializable]
	public struct BoneMapping
	{
		// Token: 0x04001606 RID: 5638
		public SlimeAppearance.SlimeBone Bone;

		// Token: 0x04001607 RID: 5639
		public GameObject BoneObject;
	}

	// Token: 0x0200044E RID: 1102
	public struct FaceRenderer
	{
		// Token: 0x04001608 RID: 5640
		public Renderer Renderer;

		// Token: 0x04001609 RID: 5641
		public bool ShowEyes;

		// Token: 0x0400160A RID: 5642
		public bool ShowMouth;
	}

	// Token: 0x0200044F RID: 1103
	private class RecalculateBoundsHelper : MonoBehaviour
	{
		// Token: 0x060016BB RID: 5819 RVA: 0x00058890 File Offset: 0x00056A90
		public static void RecalculateBounds(SlimeAppearanceApplicator parent)
		{
			if (parent.recalculateBoundsHelper != null)
			{
				return;
			}
			if (!Application.isPlaying || parent.animator == null)
			{
				parent.LODGroup.RecalculateBounds();
				return;
			}
			if (!SlimeAppearanceApplicator.RecalculateBoundsHelper.TryRecalculateBounds(parent))
			{
				parent.recalculateBoundsHelper = parent.gameObject.AddComponent<SlimeAppearanceApplicator.RecalculateBoundsHelper>();
				parent.recalculateBoundsHelper.parent = parent;
			}
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x000588F4 File Offset: 0x00056AF4
		private static bool TryRecalculateBounds(SlimeAppearanceApplicator parent)
		{
			if (!parent.gameObject.activeInHierarchy)
			{
				return false;
			}
			if (parent.animatorState == null)
			{
				parent.animatorState = parent.animator.GetBehaviour<SlimeAnimatorStateIdle>();
			}
			if (!parent.animatorState.IsInitialized)
			{
				parent.LODGroup.RecalculateBounds();
				return true;
			}
			if (parent.animatorState.IsCurrentState)
			{
				parent.LODGroup.RecalculateBounds();
				return true;
			}
			return false;
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x00058964 File Offset: 0x00056B64
		public void Update()
		{
			if (SlimeAppearanceApplicator.RecalculateBoundsHelper.TryRecalculateBounds(this.parent))
			{
				Destroyer.Destroy(this, "RecalculateBoundsHelper.Update");
			}
		}

		// Token: 0x0400160B RID: 5643
		private SlimeAppearanceApplicator parent;
	}
}
