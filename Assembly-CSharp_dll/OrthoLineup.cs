using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x020004CA RID: 1226
public class OrthoLineup : MonoBehaviour
{
	// Token: 0x060019A7 RID: 6567 RVA: 0x00064090 File Offset: 0x00062290
	public void Start()
	{
		Time.timeScale = 0f;
		SRQualitySettings.CurrentLevel = SRQualitySettings.Level.VERY_HIGH;
		this.ShowLineup();
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x000640A8 File Offset: 0x000622A8
	public void Update()
	{
		float axisRaw = Input.GetAxisRaw("Horizontal");
		float axisRaw2 = Input.GetAxisRaw("Vertical");
		Vector3 b = (float)(Input.GetKey(KeyCode.LeftShift) ? 3 : 1) * this.cameraSpeed * Time.unscaledDeltaTime * new Vector3(axisRaw, axisRaw2, 0f).normalized;
		this.cam.transform.position += b;
		if (Input.GetKeyDown(KeyCode.Space))
		{
			string text = string.Format("{0}orthoslimes-{1}.png", Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + Path.DirectorySeparatorChar.ToString(), DateTime.Now.ToFileTime());
			ScreenCapture.CaptureScreenshot(text);
			Log.Debug("Screenshot saved as " + text, Array.Empty<object>());
		}
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x00064180 File Offset: 0x00062380
	public void ShowLineup()
	{
		List<SlimeDefinition> list = (from slime in this.definitions.Slimes
		where this.includeLargos || !slime.IsLargo
		select slime).ToList<SlimeDefinition>();
		AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(this.animatorController);
		animatorOverrideController.ApplyOverrides(new List<KeyValuePair<AnimationClip, AnimationClip>>(new KeyValuePair<AnimationClip, AnimationClip>[]
		{
			new KeyValuePair<AnimationClip, AnimationClip>(this.idle, this.idleOverride)
		}));
		for (int i = 0; i < list.Count; i++)
		{
			SlimeDefinition slimeDefinition = list[i];
			Vector3 position = new Vector3(0f, (float)i * -this.viewSpacing.y + (this.showLabels ? ((float)i * -this.extraLabelSpacing) : 0f));
			GameObject gameObject = new GameObject(slimeDefinition.Name);
			gameObject.transform.position = position;
			gameObject.transform.parent = base.transform;
			for (int j = 0; j < slimeDefinition.Appearances.Count<SlimeAppearance>(); j++)
			{
				SlimeAppearance slimeAppearance = slimeDefinition.Appearances.ElementAt(j);
				Vector3 localPosition = new Vector3((float)j * (this.viewSpacing.x * (float)this.views.Length + this.extraAppearanceSpacing), 0f, 0f);
				string text = SRSingleton<GameContext>.Instance.MessageDirector.GetBundle("actor").Xlate(slimeAppearance.NameXlateKey);
				if (string.IsNullOrEmpty(text))
				{
					text = "Classic";
				}
				GameObject gameObject2 = new GameObject(text);
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localPosition = localPosition;
				if (this.showLabels)
				{
					string text2 = string.Format("{0} ({1})", slimeDefinition.Name, text);
					TextMesh textMesh = UnityEngine.Object.Instantiate<TextMesh>(this.labelPrefab, gameObject2.transform);
					textMesh.transform.localPosition = Vector3.zero;
					textMesh.transform.name = "Label";
					textMesh.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
					textMesh.text = text2;
				}
				for (int k = 0; k < this.views.Length; k++)
				{
					SlimeAppearanceApplicator slimeAppearanceApplicator = LineupUtils.GenerateAppearancePreview(this.appearancePrefab, slimeDefinition, slimeAppearance);
					slimeAppearanceApplicator.GetComponentInChildren<Animator>().runtimeAnimatorController = animatorOverrideController;
					foreach (Type t in this.blacklistedObjectTypes)
					{
						Component[] componentsInChildren = slimeAppearanceApplicator.GetComponentsInChildren(t);
						for (int m = 0; m < componentsInChildren.Length; m++)
						{
							componentsInChildren[m].gameObject.SetActive(false);
						}
					}
					Vector3 localPosition2 = new Vector3((float)k * this.viewSpacing.x, this.showLabels ? (-this.extraLabelSpacing) : 0f, 0f);
					slimeAppearanceApplicator.transform.parent = gameObject2.transform;
					slimeAppearanceApplicator.transform.localPosition = localPosition2;
					slimeAppearanceApplicator.transform.rotation = this.views[k];
				}
			}
		}
	}

	// Token: 0x04001943 RID: 6467
	public SlimeAppearanceApplicator appearancePrefab;

	// Token: 0x04001944 RID: 6468
	public SlimeDefinitions definitions;

	// Token: 0x04001945 RID: 6469
	public Vector2 viewSpacing = new Vector2(1.5f, 1.25f);

	// Token: 0x04001946 RID: 6470
	public float extraLabelSpacing = 1f;

	// Token: 0x04001947 RID: 6471
	public float extraAppearanceSpacing = 1f;

	// Token: 0x04001948 RID: 6472
	public Camera cam;

	// Token: 0x04001949 RID: 6473
	public float cameraSpeed = 2f;

	// Token: 0x0400194A RID: 6474
	public bool showLabels = true;

	// Token: 0x0400194B RID: 6475
	public TextMesh labelPrefab;

	// Token: 0x0400194C RID: 6476
	public RuntimeAnimatorController animatorController;

	// Token: 0x0400194D RID: 6477
	public AnimationClip idle;

	// Token: 0x0400194E RID: 6478
	public AnimationClip idleOverride;

	// Token: 0x0400194F RID: 6479
	public bool includeLargos;

	// Token: 0x04001950 RID: 6480
	public Quaternion[] views = new Quaternion[]
	{
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.identity
	};

	// Token: 0x04001951 RID: 6481
	private readonly Type[] blacklistedObjectTypes = new Type[]
	{
		typeof(RadExpandMarker),
		typeof(TrailRenderer)
	};
}
