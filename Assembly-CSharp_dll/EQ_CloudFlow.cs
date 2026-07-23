using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class EQ_CloudFlow : MonoBehaviour
{
	// Token: 0x0600019F RID: 415 RVA: 0x0000BEF0 File Offset: 0x0000A0F0
	private void Start()
	{
		this.m_CloudList = new Cloud[base.transform.childCount];
		int num = UnityEngine.Random.Range(0, 2);
		int num2 = 0;
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.m_CloudList[num2] = new Cloud();
			this.m_CloudList[num2].m_MoveSpeed = UnityEngine.Random.Range(this.m_MinSpeed, this.m_MaxSpeed);
			if (num == 0)
			{
				this.m_CloudList[num2].m_MoveSpeed *= -1f;
				if (this.m_Behavior == eCloudFlowBehavior.SwitchLeftRight)
				{
					num = 1;
				}
			}
			else if (this.m_Behavior == eCloudFlowBehavior.SwitchLeftRight)
			{
				num = 0;
			}
			this.m_CloudList[num2].m_Cloud = transform.gameObject;
			if (this.m_EnableLargeCloudLoop)
			{
				this.m_CloudList[num2].m_CloudFollower = UnityEngine.Object.Instantiate<GameObject>(transform.gameObject);
			}
			this.m_CloudList[num2].m_OriginalLocalPos = this.m_CloudList[num2].m_Cloud.transform.localPosition;
			num2++;
		}
		if (this.m_EnableLargeCloudLoop)
		{
			Cloud[] cloudList = this.m_CloudList;
			for (int i = 0; i < cloudList.Length; i++)
			{
				cloudList[i].m_CloudFollower.transform.parent = base.transform;
			}
		}
		this.FindTheOrthographicCamera();
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000C068 File Offset: 0x0000A268
	private void Update()
	{
		if (this.m_Camera == null)
		{
			this.FindTheOrthographicCamera();
		}
		if (this.m_Camera == null)
		{
			Debug.LogWarning("There is no Orthographic camera in the scene.");
			return;
		}
		int num = 0;
		Cloud[] cloudList = this.m_CloudList;
		for (int i = 0; i < cloudList.Length; i++)
		{
			if (cloudList[i].m_Cloud.activeSelf)
			{
				this.m_CloudList[num].m_Cloud.transform.localPosition = new Vector3(this.m_CloudList[num].m_Cloud.transform.localPosition.x + this.m_CloudList[num].m_MoveSpeed * Time.deltaTime, this.m_CloudList[num].m_Cloud.transform.localPosition.y, this.m_CloudList[num].m_Cloud.transform.localPosition.z);
				if (this.m_CloudList[num].m_MoveSpeed > 0f)
				{
					if (this.m_CloudList[num].m_CloudFollower != null)
					{
						this.m_CloudList[num].m_CloudFollower.transform.localPosition = new Vector3(this.m_CloudList[num].m_Cloud.transform.localPosition.x - this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.x, this.m_CloudList[num].m_Cloud.transform.localPosition.y, this.m_CloudList[num].m_Cloud.transform.localPosition.z);
					}
					if (this.m_CloudList[num].m_Cloud.transform.localPosition.x > this.RightMostOfScreen.x + this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.x / 2f)
					{
						if (this.m_EnableLargeCloudLoop)
						{
							GameObject cloud = this.m_CloudList[num].m_Cloud;
							this.m_CloudList[num].m_Cloud = this.m_CloudList[num].m_CloudFollower;
							this.m_CloudList[num].m_CloudFollower = cloud;
						}
						else
						{
							this.m_CloudList[num].m_MoveSpeed = UnityEngine.Random.Range(this.m_MinSpeed, this.m_MaxSpeed);
							this.m_CloudList[num].m_Cloud.transform.localPosition = new Vector3(this.LeftMostOfScreen.x - this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.x, UnityEngine.Random.Range(-this.m_Camera.orthographicSize / 2f, this.m_Camera.orthographicSize / 2f), this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.z);
						}
					}
				}
				else
				{
					if (this.m_CloudList[num].m_CloudFollower != null)
					{
						this.m_CloudList[num].m_CloudFollower.transform.localPosition = new Vector3(this.m_CloudList[num].m_Cloud.transform.localPosition.x + this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.x, this.m_CloudList[num].m_Cloud.transform.localPosition.y, this.m_CloudList[num].m_Cloud.transform.localPosition.z);
					}
					if (this.m_CloudList[num].m_Cloud.transform.localPosition.x < this.LeftMostOfScreen.x - this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.x / 2f)
					{
						if (this.m_EnableLargeCloudLoop)
						{
							GameObject cloud2 = this.m_CloudList[num].m_Cloud;
							this.m_CloudList[num].m_Cloud = this.m_CloudList[num].m_CloudFollower;
							this.m_CloudList[num].m_CloudFollower = cloud2;
						}
						else
						{
							this.m_CloudList[num].m_MoveSpeed = -UnityEngine.Random.Range(this.m_MinSpeed, this.m_MaxSpeed);
							this.m_CloudList[num].m_Cloud.transform.localPosition = new Vector3(this.RightMostOfScreen.x + this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.x, UnityEngine.Random.Range(this.m_CloudList[num].m_OriginalLocalPos.y - this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.y, this.m_CloudList[num].m_OriginalLocalPos.y + this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.y), this.m_CloudList[num].m_Cloud.GetComponent<Renderer>().bounds.size.z);
						}
					}
				}
			}
			num++;
		}
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000C5C0 File Offset: 0x0000A7C0
	private void FindTheOrthographicCamera()
	{
		if (this.m_Camera == null)
		{
			foreach (Camera camera in UnityEngine.Object.FindObjectsOfType<Camera>())
			{
				if (camera.orthographic)
				{
					this.m_Camera = camera;
					break;
				}
			}
		}
		if (this.m_Camera != null)
		{
			this.LeftMostOfScreen = this.m_Camera.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
			this.RightMostOfScreen = this.m_Camera.ScreenToWorldPoint(new Vector3((float)Screen.width, 0f, 0f));
		}
	}

	// Token: 0x040001E7 RID: 487
	[HideInInspector]
	public Cloud[] m_CloudList;

	// Token: 0x040001E8 RID: 488
	public bool m_EnableLargeCloudLoop;

	// Token: 0x040001E9 RID: 489
	public eCloudFlowBehavior m_Behavior = eCloudFlowBehavior.FlowTheSameWay;

	// Token: 0x040001EA RID: 490
	public float m_MinSpeed = 0.05f;

	// Token: 0x040001EB RID: 491
	public float m_MaxSpeed = 0.3f;

	// Token: 0x040001EC RID: 492
	public Camera m_Camera;

	// Token: 0x040001ED RID: 493
	private Vector3 LeftMostOfScreen;

	// Token: 0x040001EE RID: 494
	private Vector3 RightMostOfScreen;
}
