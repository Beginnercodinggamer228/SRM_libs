using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000780 RID: 1920
public class SlimeHoop : Attractor
{
	// Token: 0x06002826 RID: 10278 RVA: 0x000981B8 File Offset: 0x000963B8
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.musicDir = SRSingleton<GameContext>.Instance.MusicDirector;
		this.achieveDir = SRSingleton<SceneContext>.Instance.AchievementsDirector;
		this.defaultVert = this.hoopBone.transform.localPosition.x;
		this.defaultRot = this.hoopBone.transform.localRotation.eulerAngles.x;
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x00098234 File Offset: 0x00096434
	public void FixedUpdate()
	{
		double num = this.timeDir.WorldTime();
		if (this.mode == SlimeHoop.Mode.ACTIVE)
		{
			this.hoopBone.transform.localPosition = new Vector3(this.CurrVert(num - this.startTime), this.hoopBone.transform.localPosition.y, this.hoopBone.transform.localPosition.z);
			this.hoopBone.transform.localRotation = Quaternion.Euler(this.CurrRot(num - this.startTime), 0f, 90f);
			if (num >= this.endTime)
			{
				this.mode = SlimeHoop.Mode.RESETTING;
				base.SetAweFactor(0f);
				if (this.endFX != null)
				{
					SRBehaviour.SpawnAndPlayFX(this.endFX, this.scoreFxTransform.position, this.scoreFxTransform.rotation);
				}
				if (this.endCue != null)
				{
					SECTR_AudioSystem.Play(this.endCue, this.scoreFxTransform.position, false);
				}
			}
		}
		else if (this.mode == SlimeHoop.Mode.RESETTING)
		{
			bool flag = false;
			Vector3 localPosition = this.hoopBone.transform.localPosition;
			if (localPosition.x - this.defaultVert > 0.01f)
			{
				localPosition.x = Mathf.Max(this.defaultVert, localPosition.x - Time.fixedDeltaTime * 1f);
				flag = true;
			}
			else if (localPosition.x - this.defaultVert < -0.01f)
			{
				localPosition.x = Mathf.Min(this.defaultVert, localPosition.x + Time.fixedDeltaTime * 1f);
				flag = true;
			}
			Vector3 eulerAngles = this.hoopBone.transform.localRotation.eulerAngles;
			if (eulerAngles.x - this.defaultRot > 0.1f)
			{
				eulerAngles.x = Mathf.Max(this.defaultRot, eulerAngles.x - Time.fixedDeltaTime * 90f);
				flag = true;
			}
			else if (eulerAngles.x - this.defaultRot < -0.1f)
			{
				eulerAngles.x = Mathf.Min(this.defaultRot, eulerAngles.x + Time.fixedDeltaTime * 90f);
				flag = true;
			}
			if (!flag)
			{
				localPosition.x = this.defaultVert;
				eulerAngles.x = this.defaultRot;
				this.mode = SlimeHoop.Mode.IDLE;
			}
			this.hoopBone.transform.localPosition = localPosition;
			this.hoopBone.transform.localRotation = Quaternion.Euler(eulerAngles);
		}
		if (this.mode == SlimeHoop.Mode.ACTIVE)
		{
			int num2 = (int)Math.Floor(this.endTime - num) / 60;
			if (num2 != this.timeLeftToDisplay)
			{
				this.timeLeftToDisplay = num2;
				this.timeText.text = this.timeLeftToDisplay.ToString();
			}
		}
		else
		{
			this.timeText.text = "--:--";
		}
		if (this.scoreToDisplay != this.currScore)
		{
			this.scoreToDisplay = this.currScore;
			this.scoreText.text = this.scoreToDisplay.ToString();
		}
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x00098548 File Offset: 0x00096748
	public void AddScore()
	{
		if (this.mode == SlimeHoop.Mode.IDLE)
		{
			this.mode = SlimeHoop.Mode.ACTIVE;
			this.startTime = this.timeDir.WorldTime();
			this.endTime = this.startTime + 3600.0;
			this.currScore = 0;
			this.musicDir.EnableSlimeHoopMode(this.endTime);
		}
		if (this.mode != SlimeHoop.Mode.RESETTING)
		{
			SRBehaviour.SpawnAndPlayFX(this.scoreFX, this.scoreFxTransform.position, this.scoreFxTransform.rotation);
			if (this.scoreCue != null)
			{
				SECTR_AudioSystem.Play(this.scoreCue, this.scoreFxTransform.position, false);
			}
			this.currScore++;
			base.SetAweFactor(Mathf.Min(1f, (float)this.currScore / 10f));
			this.achieveDir.MaybeUpdateMaxStat(AchievementsDirector.IntStat.SLIMEBALL_SCORE, this.currScore);
		}
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x00098635 File Offset: 0x00096835
	private float CurrVert(double time)
	{
		return (float)(Math.Sin(time * 0.009519978426396847) * 1.0) + this.defaultVert;
	}

	// Token: 0x0600282A RID: 10282 RVA: 0x00098659 File Offset: 0x00096859
	private float CurrRot(double time)
	{
		return (float)(Math.Sin(time * 0.008055365644395351) * 45.0) + this.defaultRot;
	}

	// Token: 0x040027AA RID: 10154
	public Transform hoopBone;

	// Token: 0x040027AB RID: 10155
	public GameObject scoreFX;

	// Token: 0x040027AC RID: 10156
	public Transform scoreFxTransform;

	// Token: 0x040027AD RID: 10157
	public GameObject endFX;

	// Token: 0x040027AE RID: 10158
	public SECTR_AudioCue scoreCue;

	// Token: 0x040027AF RID: 10159
	public SECTR_AudioCue endCue;

	// Token: 0x040027B0 RID: 10160
	public Text scoreText;

	// Token: 0x040027B1 RID: 10161
	public Text timeText;

	// Token: 0x040027B2 RID: 10162
	private SlimeHoop.Mode mode;

	// Token: 0x040027B3 RID: 10163
	private TimeDirector timeDir;

	// Token: 0x040027B4 RID: 10164
	private MusicDirector musicDir;

	// Token: 0x040027B5 RID: 10165
	private AchievementsDirector achieveDir;

	// Token: 0x040027B6 RID: 10166
	private float defaultVert;

	// Token: 0x040027B7 RID: 10167
	private float defaultRot;

	// Token: 0x040027B8 RID: 10168
	private double startTime;

	// Token: 0x040027B9 RID: 10169
	private double endTime;

	// Token: 0x040027BA RID: 10170
	private int currScore;

	// Token: 0x040027BB RID: 10171
	private int scoreToDisplay = 999;

	// Token: 0x040027BC RID: 10172
	private int timeLeftToDisplay;

	// Token: 0x040027BD RID: 10173
	private const float VERT_PERIOD = 660f;

	// Token: 0x040027BE RID: 10174
	private const float VERT_FACTOR = 0.009519978f;

	// Token: 0x040027BF RID: 10175
	private const float VERT_RANGE = 1f;

	// Token: 0x040027C0 RID: 10176
	private const float VERT_RESET_SPD = 1f;

	// Token: 0x040027C1 RID: 10177
	private const float ROT_PERIOD = 780f;

	// Token: 0x040027C2 RID: 10178
	private const float ROT_FACTOR = 0.008055366f;

	// Token: 0x040027C3 RID: 10179
	private const float ROT_RANGE = 45f;

	// Token: 0x040027C4 RID: 10180
	private const float ROT_RESET_SPD = 90f;

	// Token: 0x040027C5 RID: 10181
	private const float DURATION = 3600f;

	// Token: 0x040027C6 RID: 10182
	private const float DOWN_FORCE = 5f;

	// Token: 0x040027C7 RID: 10183
	private const float MAX_AWE_SCORE = 10f;

	// Token: 0x02000781 RID: 1921
	private enum Mode
	{
		// Token: 0x040027C9 RID: 10185
		IDLE,
		// Token: 0x040027CA RID: 10186
		ACTIVE,
		// Token: 0x040027CB RID: 10187
		RESETTING
	}
}
