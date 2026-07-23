using System;
using UnityEngine;

// Token: 0x0200084F RID: 2127
public class vp_MuzzleFlash : MonoBehaviour
{
	// Token: 0x170002CD RID: 717
	// (get) Token: 0x06002CCD RID: 11469 RVA: 0x000A90F4 File Offset: 0x000A72F4
	// (set) Token: 0x06002CCE RID: 11470 RVA: 0x000A90FC File Offset: 0x000A72FC
	public float FadeSpeed
	{
		get
		{
			return this.m_FadeSpeed;
		}
		set
		{
			this.m_FadeSpeed = value;
		}
	}

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06002CCF RID: 11471 RVA: 0x000A9105 File Offset: 0x000A7305
	// (set) Token: 0x06002CD0 RID: 11472 RVA: 0x000A910D File Offset: 0x000A730D
	public bool ForceShow
	{
		get
		{
			return this.m_ForceShow;
		}
		set
		{
			this.m_ForceShow = value;
		}
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x000A9118 File Offset: 0x000A7318
	private void Awake()
	{
		this.m_Transform = base.transform;
		this.m_ForceShow = false;
		this.m_Light = base.GetComponent<Light>();
		if (this.m_Light != null)
		{
			this.m_LightIntensity = this.m_Light.intensity;
			this.m_Light.intensity = 0f;
		}
		this.m_Renderer = base.GetComponent<Renderer>();
		if (this.m_Renderer != null)
		{
			this.m_Material = base.GetComponent<Renderer>().material;
			if (this.m_Material != null)
			{
				this.m_Color = this.m_Material.GetColor("_TintColor");
				this.m_Color.a = 0f;
			}
		}
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x000A91D4 File Offset: 0x000A73D4
	private void Start()
	{
		GameObject gameObject = GameObject.Find("WeaponCamera");
		if (gameObject != null && gameObject.transform.parent == this.m_Transform.parent)
		{
			base.gameObject.layer = 31;
		}
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x000A9220 File Offset: 0x000A7420
	private void Update()
	{
		if (this.m_ForceShow)
		{
			this.Show();
		}
		else if (this.m_Color.a > 0f)
		{
			this.m_Color.a = this.m_Color.a - this.m_FadeSpeed * (Time.deltaTime * 60f);
			if (this.m_Light != null)
			{
				this.m_Light.intensity = this.m_LightIntensity * (this.m_Color.a * 2f);
			}
		}
		if (this.m_Material != null)
		{
			this.m_Material.SetColor("_TintColor", this.m_Color);
		}
		if (this.m_Color.a < 0.01f)
		{
			this.m_Renderer.enabled = false;
			if (this.m_Light != null)
			{
				this.m_Light.enabled = false;
			}
		}
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x000A9300 File Offset: 0x000A7500
	public void Show()
	{
		this.m_Renderer.enabled = true;
		if (this.m_Light != null)
		{
			this.m_Light.enabled = true;
			this.m_Light.intensity = this.m_LightIntensity;
		}
		this.m_Color.a = 0.5f;
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x000A9354 File Offset: 0x000A7554
	public void Shoot()
	{
		this.ShootInternal(true);
	}

	// Token: 0x06002CD6 RID: 11478 RVA: 0x000A935D File Offset: 0x000A755D
	public void ShootLightOnly()
	{
		this.ShootInternal(false);
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x000A9368 File Offset: 0x000A7568
	public void ShootInternal(bool showMesh)
	{
		this.m_Color.a = 0.5f;
		if (showMesh)
		{
			this.m_Transform.Rotate(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
			this.m_Renderer.enabled = true;
		}
		if (this.m_Light != null)
		{
			this.m_Light.enabled = true;
			this.m_Light.intensity = this.m_LightIntensity;
		}
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x000A93E0 File Offset: 0x000A75E0
	public void SetFadeSpeed(float fadeSpeed)
	{
		this.FadeSpeed = fadeSpeed;
	}

	// Token: 0x04002AB9 RID: 10937
	protected float m_FadeSpeed = 0.075f;

	// Token: 0x04002ABA RID: 10938
	protected bool m_ForceShow;

	// Token: 0x04002ABB RID: 10939
	protected Color m_Color = new Color(1f, 1f, 1f, 0f);

	// Token: 0x04002ABC RID: 10940
	protected Transform m_Transform;

	// Token: 0x04002ABD RID: 10941
	protected Light m_Light;

	// Token: 0x04002ABE RID: 10942
	protected float m_LightIntensity;

	// Token: 0x04002ABF RID: 10943
	protected Renderer m_Renderer;

	// Token: 0x04002AC0 RID: 10944
	protected Material m_Material;
}
