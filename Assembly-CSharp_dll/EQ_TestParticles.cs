using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000064 RID: 100
public class EQ_TestParticles : MonoBehaviour
{
	// Token: 0x060001B9 RID: 441 RVA: 0x0000D12B File Offset: 0x0000B32B
	private void Start()
	{
		if (this.m_CategoryList.Length != 0)
		{
			this.m_CurrentCategoryIndex = 0;
			this.m_CurrentCategoryIndexOld = -1;
			this.m_CurrentParticleIndex = 0;
			this.m_CurrentParticleIndexOld = -1;
			this.ShowParticle();
		}
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000D158 File Offset: 0x0000B358
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			this.m_CurrentCategoryIndexOld = this.m_CurrentCategoryIndex;
			this.m_CurrentCategoryIndex++;
			this.m_CurrentParticleIndex = 0;
			this.ShowParticle();
			return;
		}
		if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			this.m_CurrentCategoryIndexOld = this.m_CurrentCategoryIndex;
			this.m_CurrentCategoryIndex--;
			this.m_CurrentParticleIndex = 0;
			this.ShowParticle();
			return;
		}
		if (Input.GetKeyUp(KeyCode.LeftArrow))
		{
			this.m_CurrentParticleIndexOld = this.m_CurrentParticleIndex;
			this.m_CurrentParticleIndex--;
			this.ShowParticle();
			return;
		}
		if (Input.GetKeyUp(KeyCode.RightArrow))
		{
			this.m_CurrentParticleIndexOld = this.m_CurrentParticleIndex;
			this.m_CurrentParticleIndex++;
			this.ShowParticle();
		}
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000D228 File Offset: 0x0000B428
	private void OnGUI()
	{
		GUI.Window(1, new Rect((float)(Screen.width - 260), 5f, 250f, 105f), new GUI.WindowFunction(this.AppNameWindow), "FX Quest 0.3.0");
		GUI.Window(2, new Rect((float)(Screen.width - 300), (float)(Screen.height - 150), 290f, 60f), new GUI.WindowFunction(this.SceneWindow), "Scenes");
		GUI.Window(3, new Rect((float)(Screen.width - 410), (float)(Screen.height - 85), 400f, 80f), new GUI.WindowFunction(this.ParticleInformationWindow), "Information");
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000D2E8 File Offset: 0x0000B4E8
	private void ShowParticle()
	{
		if (this.m_CurrentCategoryIndex >= this.m_CategoryList.Length)
		{
			this.m_CurrentCategoryIndex = 0;
		}
		else if (this.m_CurrentCategoryIndex < 0)
		{
			this.m_CurrentCategoryIndex = this.m_CategoryList.Length - 1;
		}
		if (this.m_CurrentCategoryIndex != this.m_CurrentCategoryIndexOld)
		{
			if (this.m_CurrentCategoryIndexOld >= 0)
			{
				int num = 0;
				foreach (object obj in this.m_CategoryList[this.m_CurrentCategoryIndexOld])
				{
					Transform transform = (Transform)obj;
					this.m_CurrentParticle = transform.gameObject.GetComponent<ParticleSystem>();
					if (this.m_CurrentParticle != null)
					{
						this.m_CurrentParticle.Stop();
						this.m_CurrentParticle.gameObject.SetActive(false);
					}
					num++;
				}
			}
			if (this.m_CurrentCategoryIndex >= 0)
			{
				int num = 0;
				foreach (object obj2 in this.m_CategoryList[this.m_CurrentCategoryIndex])
				{
					Transform transform2 = (Transform)obj2;
					this.m_CurrentParticle = transform2.gameObject.GetComponent<ParticleSystem>();
					if (this.m_CurrentParticle != null)
					{
						this.m_CurrentParticle.Stop();
						this.m_CurrentParticle.gameObject.SetActive(false);
					}
					num++;
				}
			}
			if (this.m_CurrentCategoryIndexOld >= 0)
			{
				this.m_CategoryList[this.m_CurrentCategoryIndexOld].gameObject.SetActive(false);
			}
			if (this.m_CurrentCategoryIndex >= 0)
			{
				this.m_CategoryList[this.m_CurrentCategoryIndex].gameObject.SetActive(true);
			}
			this.m_CurrentCategoryName = this.m_CategoryList[this.m_CurrentCategoryIndex].name;
			this.m_CurrentCategoryChildCount = this.m_CategoryList[this.m_CurrentCategoryIndex].childCount;
		}
		if (this.m_CurrentParticleIndex >= this.m_CurrentCategoryChildCount)
		{
			this.m_CurrentParticleIndex = 0;
		}
		else if (this.m_CurrentParticleIndex < 0)
		{
			this.m_CurrentParticleIndex = this.m_CurrentCategoryChildCount - 1;
		}
		if (this.m_CurrentParticleIndex != this.m_CurrentParticleIndexOld || this.m_CurrentCategoryIndex != this.m_CurrentCategoryIndexOld)
		{
			if (this.m_CurrentParticle != null)
			{
				this.m_CurrentParticle.Stop();
				this.m_CurrentParticle.gameObject.SetActive(false);
			}
			int num = 0;
			foreach (object obj3 in this.m_CategoryList[this.m_CurrentCategoryIndex])
			{
				Transform transform3 = (Transform)obj3;
				if (num == this.m_CurrentParticleIndex)
				{
					this.m_CurrentParticle = transform3.gameObject.GetComponent<ParticleSystem>();
					if (this.m_CurrentParticle != null)
					{
						this.m_CurrentParticle.gameObject.SetActive(true);
						this.m_CurrentParticle.Play();
						this.m_CurrentParticleName = this.m_CurrentParticle.name;
						break;
					}
					break;
				}
				else
				{
					num++;
				}
			}
		}
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000D5F4 File Offset: 0x0000B7F4
	private void AppNameWindow(int id)
	{
		if (GUI.Button(new Rect(15f, 25f, 220f, 20f), "www.ge-team.com"))
		{
			Application.OpenURL("http://ge-team.com/pages/unity-3d/");
		}
		if (GUI.Button(new Rect(15f, 50f, 220f, 20f), "geteamdev@gmail.com"))
		{
			Application.OpenURL("mailto:geteamdev@gmail.com");
		}
		if (GUI.Button(new Rect(15f, 75f, 220f, 20f), "Tutorial"))
		{
			Application.OpenURL("http://youtu.be/TWpKPCGYEyI");
		}
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000D690 File Offset: 0x0000B890
	private void SceneWindow(int id)
	{
		if (this.m_CurrentParticleIndex >= 0)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (SceneManager.GetActiveScene().name == "2D_Demo")
			{
				GUI.enabled = false;
			}
			else
			{
				GUI.enabled = true;
			}
			if (GUI.Button(new Rect(12f, 25f, 125f, 25f), "2D Demo scene"))
			{
				SceneManager.LoadScene("2D_Demo");
			}
			if (SceneManager.GetActiveScene().name == "3D_Demo")
			{
				GUI.enabled = false;
			}
			else
			{
				GUI.enabled = true;
			}
			if (GUI.Button(new Rect(155f, 25f, 125f, 25f), "3D Demo scene"))
			{
				SceneManager.LoadScene("3D_Demo");
			}
			GUILayout.EndHorizontal();
		}
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000D764 File Offset: 0x0000B964
	private void ParticleInformationWindow(int id)
	{
		if (this.m_CurrentParticleIndex >= 0)
		{
			GUI.Label(new Rect(12f, 25f, 400f, 20f), string.Concat(new object[]
			{
				"Up/Down: Change Type (",
				this.m_CurrentCategoryIndex + 1,
				" of ",
				this.m_CategoryList.Length,
				" ",
				this.m_CurrentCategoryName,
				")"
			}));
			GUI.Label(new Rect(12f, 50f, 400f, 20f), string.Concat(new object[]
			{
				"Left/Right: Change Particle (",
				this.m_CurrentParticleIndex + 1,
				" of ",
				this.m_CurrentCategoryChildCount,
				" ",
				this.m_CurrentParticleName,
				")"
			}));
		}
	}

	// Token: 0x04000214 RID: 532
	public Transform[] m_CategoryList;

	// Token: 0x04000215 RID: 533
	private int m_CurrentCategoryIndex;

	// Token: 0x04000216 RID: 534
	private int m_CurrentCategoryIndexOld = -1;

	// Token: 0x04000217 RID: 535
	private int m_CurrentCategoryChildCount;

	// Token: 0x04000218 RID: 536
	private int m_CurrentParticleIndex;

	// Token: 0x04000219 RID: 537
	private int m_CurrentParticleIndexOld = -1;

	// Token: 0x0400021A RID: 538
	private ParticleSystem m_CurrentParticle;

	// Token: 0x0400021B RID: 539
	private string m_CurrentCategoryName = "";

	// Token: 0x0400021C RID: 540
	private string m_CurrentParticleName = "";
}
