using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000563 RID: 1379
public class CrosshairUI : MonoBehaviour
{
	// Token: 0x06001CC5 RID: 7365 RVA: 0x0006D910 File Offset: 0x0006BB10
	private void Start()
	{
		this.player = SRSingleton<SceneContext>.Instance.PlayerState;
		this.vacuum = SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<WeaponVacuum>();
		this.hudCrosshairScale = this.normScale;
		this.hudCrosshairScaleGoal = this.normScale;
		this.img = base.GetComponent<Image>();
	}

	// Token: 0x06001CC6 RID: 7366 RVA: 0x0006D968 File Offset: 0x0006BB68
	public void Update()
	{
		if (Time.timeScale <= 0f)
		{
			return;
		}
		bool flag = this.vacuum.InGadgetMode();
		if (flag && this.img.sprite != this.gadgetSprite)
		{
			this.img.sprite = this.gadgetSprite;
		}
		else if (!flag && this.img.sprite != this.normalSprite)
		{
			this.img.sprite = this.normalSprite;
		}
		if (flag)
		{
			this.img.color = this.gadgetTarget;
		}
		else if (this.player.PointedAtVaccable)
		{
			this.img.color = this.hasTarget;
		}
		else
		{
			this.img.color = this.noTarget;
		}
		if (flag)
		{
			this.hudCrosshairScaleGoal = this.gadgetScale;
		}
		else if (this.vacuum.InVacMode())
		{
			this.hudCrosshairScaleGoal = this.highScale;
		}
		else
		{
			this.hudCrosshairScaleGoal = this.normScale;
		}
		this.hudCrosshairScale += (this.hudCrosshairScaleGoal - this.hudCrosshairScale) * 0.95f * Time.deltaTime * 4f;
		if (this.vacuum.InVacMode() && this.hudCrosshairScaleGoal >= this.highScale)
		{
			this.hudCrosshairScale = this.highScale + Randoms.SHARED.GetInRange(0f, this.vibrateAmount);
		}
		this.img.transform.localScale = new Vector3(this.hudCrosshairScale, this.hudCrosshairScale, this.hudCrosshairScale);
	}

	// Token: 0x04001BD4 RID: 7124
	public float normScale = 1f;

	// Token: 0x04001BD5 RID: 7125
	public float highScale = 1.5f;

	// Token: 0x04001BD6 RID: 7126
	public float gadgetScale = 2f;

	// Token: 0x04001BD7 RID: 7127
	public float vibrateAmount = 0.4f;

	// Token: 0x04001BD8 RID: 7128
	public Color hasTarget = Color.green;

	// Token: 0x04001BD9 RID: 7129
	public Color noTarget = Color.white;

	// Token: 0x04001BDA RID: 7130
	public Color gadgetTarget = Color.white;

	// Token: 0x04001BDB RID: 7131
	public Sprite normalSprite;

	// Token: 0x04001BDC RID: 7132
	public Sprite gadgetSprite;

	// Token: 0x04001BDD RID: 7133
	private PlayerState player;

	// Token: 0x04001BDE RID: 7134
	private float hudCrosshairScale;

	// Token: 0x04001BDF RID: 7135
	private float hudCrosshairScaleGoal;

	// Token: 0x04001BE0 RID: 7136
	private Image img;

	// Token: 0x04001BE1 RID: 7137
	private WeaponVacuum vacuum;
}
