using System;
using UnityEngine;

// Token: 0x02000854 RID: 2132
public class vp_SimpleHUD : MonoBehaviour, EventHandlerRegistrable
{
	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06002CF4 RID: 11508 RVA: 0x000A9A68 File Offset: 0x000A7C68
	protected float m_HealthWidth
	{
		get
		{
			return this.HealthStyle.CalcSize(new GUIContent(this.FormattedHealth)).x;
		}
	}

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06002CF5 RID: 11509 RVA: 0x000A9A85 File Offset: 0x000A7C85
	public GUIStyle MessageStyle
	{
		get
		{
			if (vp_SimpleHUD.m_MessageStyle == null)
			{
				vp_SimpleHUD.m_MessageStyle = new GUIStyle("Label");
				vp_SimpleHUD.m_MessageStyle.alignment = TextAnchor.MiddleCenter;
				vp_SimpleHUD.m_MessageStyle.font = this.MessageFont;
			}
			return vp_SimpleHUD.m_MessageStyle;
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06002CF6 RID: 11510 RVA: 0x000A9AC4 File Offset: 0x000A7CC4
	public GUIStyle HealthStyle
	{
		get
		{
			if (this.m_HealthStyle == null)
			{
				this.m_HealthStyle = new GUIStyle("Label");
				this.m_HealthStyle.font = this.BigFont;
				this.m_HealthStyle.alignment = TextAnchor.MiddleRight;
				this.m_HealthStyle.fontSize = 28;
				this.m_HealthStyle.wordWrap = false;
			}
			return this.m_HealthStyle;
		}
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06002CF7 RID: 11511 RVA: 0x000A9B2C File Offset: 0x000A7D2C
	public GUIStyle AmmoStyle
	{
		get
		{
			if (this.m_AmmoStyle == null)
			{
				this.m_AmmoStyle = new GUIStyle("Label");
				this.m_AmmoStyle.font = this.BigFont;
				this.m_AmmoStyle.alignment = TextAnchor.MiddleRight;
				this.m_AmmoStyle.fontSize = 28;
				this.m_AmmoStyle.wordWrap = false;
			}
			return this.m_AmmoStyle;
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x06002CF8 RID: 11512 RVA: 0x000A9B94 File Offset: 0x000A7D94
	public GUIStyle AmmoStyleSmall
	{
		get
		{
			if (this.m_AmmoStyleSmall == null)
			{
				this.m_AmmoStyleSmall = new GUIStyle("Label");
				this.m_AmmoStyleSmall.font = this.SmallFont;
				this.m_AmmoStyleSmall.alignment = TextAnchor.UpperLeft;
				this.m_AmmoStyleSmall.fontSize = 15;
				this.m_AmmoStyleSmall.wordWrap = false;
			}
			return this.m_AmmoStyleSmall;
		}
	}

	// Token: 0x06002CF9 RID: 11513 RVA: 0x000A9BFA File Offset: 0x000A7DFA
	protected virtual void Awake()
	{
		this.m_Player = base.transform.GetComponent<vp_FPPlayerEventHandler>();
		this.m_Audio = this.m_Player.transform.GetComponent<AudioSource>();
	}

	// Token: 0x06002CFA RID: 11514 RVA: 0x000A9C23 File Offset: 0x000A7E23
	protected virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.Register(this.m_Player);
		}
	}

	// Token: 0x06002CFB RID: 11515 RVA: 0x000A9C3F File Offset: 0x000A7E3F
	protected virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.Unregister(this.m_Player);
		}
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x06002CFC RID: 11516 RVA: 0x000A9C5C File Offset: 0x000A7E5C
	private string FormattedHealth
	{
		get
		{
			this.m_FormattedHealth = this.m_Player.Health.Get() * this.HealthMultiplier;
			if (this.m_FormattedHealth < 1f)
			{
				this.m_FormattedHealth = (this.m_Player.Dead.Active ? Mathf.Min(this.m_FormattedHealth, 0f) : 1f);
			}
			if (this.m_Player.Dead.Active && this.m_FormattedHealth > 0f)
			{
				this.m_FormattedHealth = 0f;
			}
			return ((int)this.m_FormattedHealth).ToString();
		}
	}

	// Token: 0x06002CFD RID: 11517 RVA: 0x000A9D00 File Offset: 0x000A7F00
	private void Update()
	{
		this.m_CurrentAmmoOffset = Mathf.SmoothStep(this.m_CurrentAmmoOffset, this.m_TargetAmmoOffset, Time.deltaTime * 10f);
		this.m_CurrentHealthOffset = Mathf.SmoothStep(this.m_CurrentHealthOffset, this.m_TargetHealthOffset, Time.deltaTime * 10f);
		if (this.m_Player.CurrentWeaponIndex.Get() == 0 || this.m_Player.CurrentWeaponType.Get() == 2)
		{
			this.m_TargetAmmoOffset = 200f;
		}
		else
		{
			this.m_TargetAmmoOffset = 10f;
		}
		if (this.m_Player.Dead.Active)
		{
			this.HealthColor = Color.black;
		}
		else if (this.m_Player.Health.Get() < this.HealthLowLevel)
		{
			this.HealthColor = Color.Lerp(Color.white, this.HealthLowColor, vp_MathUtility.Sinus(6f, 0.1f, 0f) * 5f + 0.5f);
			if (this.HealthLowSound != null && Time.time >= this.m_NextAllowedPlayHealthLowSoundTime)
			{
				this.m_NextAllowedPlayHealthLowSoundTime = Time.time + this.HealthLowSoundInterval;
				this.m_Audio.pitch = 1f;
				this.m_Audio.PlayOneShot(this.HealthLowSound);
			}
		}
		else
		{
			this.HealthColor = Color.white;
		}
		if (this.m_Player.CurrentWeaponAmmoCount.Get() < 1 && this.m_Player.CurrentWeaponType.Get() != 3)
		{
			this.AmmoColor = Color.Lerp(Color.white, this.AmmoLowColor, vp_MathUtility.Sinus(8f, 0.1f, 0f) * 5f + 0.5f);
			return;
		}
		this.AmmoColor = Color.white;
	}

	// Token: 0x06002CFE RID: 11518 RVA: 0x000A9EDD File Offset: 0x000A80DD
	protected virtual void OnGUI()
	{
		if (!this.ShowHUD)
		{
			return;
		}
		this.DrawHealth();
		this.DrawAmmo();
		this.DrawText();
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x000A9EFC File Offset: 0x000A80FC
	private void DrawHealth()
	{
		this.DrawLabel("", new Vector2(this.m_CurrentHealthOffset, (float)(Screen.height - 68)), new Vector2(80f + this.m_HealthWidth, 52f), this.AmmoStyle, Color.white, this.m_TranspBlack, null);
		if (this.HealthIcon != null)
		{
			this.DrawLabel("", new Vector2(this.m_CurrentHealthOffset + 10f, (float)(Screen.height - 58)), new Vector2(32f, 32f), this.AmmoStyle, Color.white, this.HealthColor, this.HealthIcon);
		}
		this.DrawLabel(this.FormattedHealth, new Vector2(this.m_CurrentHealthOffset - 18f - (45f - this.m_HealthWidth), (float)Screen.height - this.BigFontOffset), new Vector2(110f, 60f), this.HealthStyle, this.HealthColor, Color.clear, null);
		this.DrawLabel("%", new Vector2(this.m_CurrentHealthOffset + 50f + this.m_HealthWidth, (float)Screen.height - this.SmallFontOffset), new Vector2(110f, 60f), this.AmmoStyleSmall, this.HealthColor, Color.clear, null);
		GUI.color = Color.white;
	}

	// Token: 0x06002D00 RID: 11520 RVA: 0x000AA05C File Offset: 0x000A825C
	private void DrawAmmo()
	{
		if (this.m_Player.CurrentWeaponType.Get() == 3)
		{
			this.DrawLabel("", new Vector2(this.m_CurrentAmmoOffset + (float)Screen.width - 93f - this.AmmoStyle.CalcSize(new GUIContent(this.m_Player.CurrentWeaponAmmoCount.Get().ToString())).x, (float)(Screen.height - 68)), new Vector2(200f, 52f), this.AmmoStyle, this.AmmoColor, this.m_TranspBlack, null);
			if (this.m_Player.CurrentAmmoIcon.Get() != null)
			{
				this.DrawLabel("", new Vector2(this.m_CurrentAmmoOffset + (float)Screen.width - 83f - this.AmmoStyle.CalcSize(new GUIContent(this.m_Player.CurrentWeaponAmmoCount.Get().ToString())).x, (float)(Screen.height - 58)), new Vector2(32f, 32f), this.AmmoStyle, Color.white, this.AmmoColor, this.m_Player.CurrentAmmoIcon.Get());
			}
			this.DrawLabel((this.m_Player.CurrentWeaponAmmoCount.Get() + this.m_Player.CurrentWeaponClipCount.Get()).ToString(), new Vector2(this.m_CurrentAmmoOffset + (float)Screen.width - 145f, (float)Screen.height - this.BigFontOffset), new Vector2(110f, 60f), this.AmmoStyle, this.AmmoColor, Color.clear, null);
			return;
		}
		this.DrawLabel("", new Vector2(this.m_CurrentAmmoOffset + (float)Screen.width - 115f - this.AmmoStyle.CalcSize(new GUIContent(this.m_Player.CurrentWeaponAmmoCount.Get().ToString())).x, (float)(Screen.height - 68)), new Vector2(200f, 52f), this.AmmoStyle, this.AmmoColor, this.m_TranspBlack, null);
		if (this.m_Player.CurrentAmmoIcon.Get() != null)
		{
			this.DrawLabel("", new Vector2(this.m_CurrentAmmoOffset + (float)Screen.width - 105f - this.AmmoStyle.CalcSize(new GUIContent(this.m_Player.CurrentWeaponAmmoCount.Get().ToString())).x, (float)(Screen.height - 58)), new Vector2(32f, 32f), this.AmmoStyle, Color.white, this.AmmoColor, this.m_Player.CurrentAmmoIcon.Get());
		}
		this.DrawLabel(this.m_Player.CurrentWeaponAmmoCount.Get().ToString(), new Vector2(this.m_CurrentAmmoOffset + (float)Screen.width - 177f, (float)Screen.height - this.BigFontOffset), new Vector2(110f, 60f), this.AmmoStyle, this.AmmoColor, Color.clear, null);
		this.DrawLabel("/ " + this.m_Player.CurrentWeaponClipCount.Get().ToString(), new Vector2(this.m_CurrentAmmoOffset + (float)Screen.width - 60f, (float)Screen.height - this.SmallFontOffset), new Vector2(110f, 60f), this.AmmoStyleSmall, this.AmmoColor, Color.clear, null);
	}

	// Token: 0x06002D01 RID: 11521 RVA: 0x000AA44C File Offset: 0x000A864C
	private void DrawText()
	{
		if (this.m_PickupMessage == null)
		{
			return;
		}
		if (this.m_MessageColor.a < 0.01f)
		{
			return;
		}
		this.m_MessageColor = Color.Lerp(this.m_MessageColor, this.m_InvisibleColor, Time.deltaTime * 0.4f);
		GUI.color = this.m_MessageColor;
		GUI.Box(new Rect(200f, 150f, (float)(Screen.width - 400), (float)(Screen.height - 400)), this.m_PickupMessage, this.MessageStyle);
		GUI.color = Color.white;
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x000AA4E4 File Offset: 0x000A86E4
	protected virtual void OnMessage_HUDText(string message)
	{
		this.m_MessageColor = Color.white;
		this.m_PickupMessage = message;
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x000AA4F8 File Offset: 0x000A86F8
	private void DrawLabel(string text, Vector2 position, Vector2 scale, GUIStyle textStyle, Color textColor, Color bgColor, Texture texture)
	{
		if (texture == null)
		{
			texture = this.Background;
		}
		if (scale.x == 0f)
		{
			scale.x = textStyle.CalcSize(new GUIContent(text)).x;
		}
		if (scale.y == 0f)
		{
			scale.y = textStyle.CalcSize(new GUIContent(text)).y;
		}
		this.m_DrawLabelRect.x = (this.m_DrawPos.x = position.x);
		this.m_DrawLabelRect.y = (this.m_DrawPos.y = position.y);
		this.m_DrawLabelRect.width = (this.m_DrawSize.x = scale.x);
		this.m_DrawLabelRect.height = (this.m_DrawSize.y = scale.y);
		if (bgColor != Color.clear)
		{
			GUI.color = bgColor;
			if (texture != null)
			{
				GUI.DrawTexture(this.m_DrawLabelRect, texture);
			}
		}
		GUI.color = textColor;
		GUI.Label(this.m_DrawLabelRect, text, textStyle);
		GUI.color = Color.white;
		this.m_DrawPos.x = this.m_DrawPos.x + this.m_DrawSize.x;
		this.m_DrawPos.y = this.m_DrawPos.y + this.m_DrawSize.y;
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x000AA65E File Offset: 0x000A885E
	private void OnStart_SetWeapon()
	{
		this.m_TargetAmmoOffset = 200f;
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x000AA66B File Offset: 0x000A886B
	private void OnStop_SetWeapon()
	{
		this.m_TargetAmmoOffset = 10f;
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x000AA678 File Offset: 0x000A8878
	private void OnStop_Dead()
	{
		this.m_CurrentHealthOffset = -200f;
		this.m_TargetHealthOffset = 0f;
		this.HealthColor = Color.white;
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x000AA69C File Offset: 0x000A889C
	public void Register(vp_EventHandler eventHandler)
	{
		eventHandler.RegisterActivity("Dead", null, new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.RegisterActivity("SetWeapon", new vp_Activity.Callback(this.OnStart_SetWeapon), new vp_Activity.Callback(this.OnStop_SetWeapon), null, null, null, null);
		eventHandler.RegisterMessage<string>("HUDText", new vp_Message<string>.Sender<string>(this.OnMessage_HUDText));
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x000AA704 File Offset: 0x000A8904
	public void Unregister(vp_EventHandler eventHandler)
	{
		eventHandler.UnregisterActivity("Dead", null, new vp_Activity.Callback(this.OnStop_Dead), null, null, null, null);
		eventHandler.UnregisterActivity("SetWeapon", new vp_Activity.Callback(this.OnStart_SetWeapon), new vp_Activity.Callback(this.OnStop_SetWeapon), null, null, null, null);
		eventHandler.UnregisterMessage<string>("HUDText", new vp_Message<string>.Sender<string>(this.OnMessage_HUDText));
	}

	// Token: 0x04002AD2 RID: 10962
	public bool ShowHUD = true;

	// Token: 0x04002AD3 RID: 10963
	protected vp_FPPlayerEventHandler m_Player;

	// Token: 0x04002AD4 RID: 10964
	public Font BigFont;

	// Token: 0x04002AD5 RID: 10965
	public Font SmallFont;

	// Token: 0x04002AD6 RID: 10966
	public Font MessageFont;

	// Token: 0x04002AD7 RID: 10967
	public float BigFontOffset = 69f;

	// Token: 0x04002AD8 RID: 10968
	public float SmallFontOffset = 56f;

	// Token: 0x04002AD9 RID: 10969
	public Texture Background;

	// Token: 0x04002ADA RID: 10970
	protected Vector2 m_DrawPos = Vector2.zero;

	// Token: 0x04002ADB RID: 10971
	protected Vector2 m_DrawSize = Vector2.zero;

	// Token: 0x04002ADC RID: 10972
	protected Rect m_DrawLabelRect = new Rect(0f, 0f, 0f, 0f);

	// Token: 0x04002ADD RID: 10973
	protected Rect m_DrawShadowRect = new Rect(0f, 0f, 0f, 0f);

	// Token: 0x04002ADE RID: 10974
	protected float m_TargetHealthOffset;

	// Token: 0x04002ADF RID: 10975
	protected float m_CurrentHealthOffset;

	// Token: 0x04002AE0 RID: 10976
	protected float m_TargetAmmoOffset = 200f;

	// Token: 0x04002AE1 RID: 10977
	protected float m_CurrentAmmoOffset = 200f;

	// Token: 0x04002AE2 RID: 10978
	public Texture2D HealthIcon;

	// Token: 0x04002AE3 RID: 10979
	public float HealthMultiplier = 10f;

	// Token: 0x04002AE4 RID: 10980
	public Color HealthColor = Color.white;

	// Token: 0x04002AE5 RID: 10981
	public float HealthLowLevel = 2.5f;

	// Token: 0x04002AE6 RID: 10982
	public Color HealthLowColor = new Color(0.75f, 0f, 0f, 1f);

	// Token: 0x04002AE7 RID: 10983
	public AudioClip HealthLowSound;

	// Token: 0x04002AE8 RID: 10984
	public float HealthLowSoundInterval = 1f;

	// Token: 0x04002AE9 RID: 10985
	protected float m_FormattedHealth;

	// Token: 0x04002AEA RID: 10986
	protected float m_NextAllowedPlayHealthLowSoundTime;

	// Token: 0x04002AEB RID: 10987
	protected AudioSource m_Audio;

	// Token: 0x04002AEC RID: 10988
	public Color AmmoColor = Color.white;

	// Token: 0x04002AED RID: 10989
	public Color AmmoLowColor = new Color(0f, 0f, 0f, 1f);

	// Token: 0x04002AEE RID: 10990
	protected string m_PickupMessage = "";

	// Token: 0x04002AEF RID: 10991
	protected Color m_MessageColor = new Color(1f, 1f, 1f, 2f);

	// Token: 0x04002AF0 RID: 10992
	protected Color m_InvisibleColor = new Color(0f, 0f, 0f, 0f);

	// Token: 0x04002AF1 RID: 10993
	protected Color m_TranspBlack = new Color(0f, 0f, 0f, 0.5f);

	// Token: 0x04002AF2 RID: 10994
	protected Color m_TranspWhite = new Color(1f, 1f, 1f, 0.5f);

	// Token: 0x04002AF3 RID: 10995
	protected static GUIStyle m_MessageStyle;

	// Token: 0x04002AF4 RID: 10996
	protected GUIStyle m_HealthStyle;

	// Token: 0x04002AF5 RID: 10997
	protected GUIStyle m_AmmoStyle;

	// Token: 0x04002AF6 RID: 10998
	protected GUIStyle m_AmmoStyleSmall;
}
