using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class CFX_Demo : MonoBehaviour
{
	// Token: 0x060001C1 RID: 449 RVA: 0x0000D88C File Offset: 0x0000BA8C
	private void OnMouseDown()
	{
		RaycastHit raycastHit = default(RaycastHit);
		if (base.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 9999f))
		{
			GameObject gameObject = this.spawnParticle();
			gameObject.transform.position = raycastHit.point + gameObject.transform.position;
		}
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000D8F0 File Offset: 0x0000BAF0
	private GameObject spawnParticle()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ParticleExamples[this.exampleIndex]);
		gameObject.SetActive(true);
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			gameObject.transform.GetChild(i).gameObject.SetActive(true);
		}
		float y = 0f;
		foreach (KeyValuePair<string, float> keyValuePair in this.ParticlesYOffsetD)
		{
			if (gameObject.name.StartsWith(keyValuePair.Key))
			{
				y = keyValuePair.Value;
				break;
			}
		}
		gameObject.transform.position = new Vector3(0f, y, 0f);
		return gameObject;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(5f, 20f, (float)(Screen.width - 10), 30f));
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Effect", new GUILayoutOption[]
		{
			GUILayout.Width(50f)
		});
		if (GUILayout.Button("<", new GUILayoutOption[]
		{
			GUILayout.Width(20f)
		}))
		{
			this.prevParticle();
		}
		GUILayout.Label(this.ParticleExamples[this.exampleIndex].name, new GUILayoutOption[]
		{
			GUILayout.Width(190f)
		});
		if (GUILayout.Button(">", new GUILayoutOption[]
		{
			GUILayout.Width(20f)
		}))
		{
			this.nextParticle();
		}
		GUILayout.Label("Click on the ground to spawn selected particles", Array.Empty<GUILayoutOption>());
		if (GUILayout.Button(CFX_Demo_RotateCamera.rotating ? "Pause Camera" : "Rotate Camera", new GUILayoutOption[]
		{
			GUILayout.Width(140f)
		}))
		{
			CFX_Demo_RotateCamera.rotating = !CFX_Demo_RotateCamera.rotating;
		}
		if (GUILayout.Button(this.randomSpawns ? "Stop Random Spawns" : "Start Random Spawns", new GUILayoutOption[]
		{
			GUILayout.Width(140f)
		}))
		{
			this.randomSpawns = !this.randomSpawns;
			if (this.randomSpawns)
			{
				base.StartCoroutine("RandomSpawnsCoroutine");
			}
			else
			{
				base.StopCoroutine("RandomSpawnsCoroutine");
			}
		}
		this.randomSpawnsDelay = GUILayout.TextField(this.randomSpawnsDelay, 10, new GUILayoutOption[]
		{
			GUILayout.Width(42f)
		});
		this.randomSpawnsDelay = Regex.Replace(this.randomSpawnsDelay, "[^0-9.]", "");
		if (GUILayout.Button(base.GetComponent<Renderer>().enabled ? "Hide Ground" : "Show Ground", new GUILayoutOption[]
		{
			GUILayout.Width(90f)
		}))
		{
			base.GetComponent<Renderer>().enabled = !base.GetComponent<Renderer>().enabled;
		}
		if (GUILayout.Button(this.slowMo ? "Normal Speed" : "Slow Motion", new GUILayoutOption[]
		{
			GUILayout.Width(100f)
		}))
		{
			this.slowMo = !this.slowMo;
			if (this.slowMo)
			{
				Time.timeScale = 0.33f;
			}
			else
			{
				Time.timeScale = 1f;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000DC27 File Offset: 0x0000BE27
	private IEnumerator RandomSpawnsCoroutine()
	{
		for (;;)
		{
			GameObject gameObject = this.spawnParticle();
			if (this.orderedSpawns)
			{
				gameObject.transform.position = base.transform.position + new Vector3(this.order, gameObject.transform.position.y, 0f);
				this.order -= this.step;
				if (this.order < -this.range)
				{
					this.order = this.range;
				}
			}
			else
			{
				gameObject.transform.position = base.transform.position + new Vector3(UnityEngine.Random.Range(-this.range, this.range), 0f, UnityEngine.Random.Range(-this.range, this.range)) + new Vector3(0f, gameObject.transform.position.y, 0f);
			}
			yield return new WaitForSeconds(float.Parse(this.randomSpawnsDelay));
		}
		yield break;
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000DC36 File Offset: 0x0000BE36
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			this.prevParticle();
			return;
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			this.nextParticle();
		}
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000DC60 File Offset: 0x0000BE60
	private void prevParticle()
	{
		this.exampleIndex--;
		if (this.exampleIndex < 0)
		{
			this.exampleIndex = this.ParticleExamples.Length - 1;
		}
		if (this.ParticleExamples[this.exampleIndex].name.Contains("Splash") || this.ParticleExamples[this.exampleIndex].name == "CFX_Ripple" || this.ParticleExamples[this.exampleIndex].name == "CFX_Fountain")
		{
			base.GetComponent<Renderer>().material = this.waterMat;
			return;
		}
		base.GetComponent<Renderer>().material = this.groundMat;
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000DD14 File Offset: 0x0000BF14
	private void nextParticle()
	{
		this.exampleIndex++;
		if (this.exampleIndex >= this.ParticleExamples.Length)
		{
			this.exampleIndex = 0;
		}
		if (this.ParticleExamples[this.exampleIndex].name.Contains("Splash") || this.ParticleExamples[this.exampleIndex].name == "CFX_Ripple" || this.ParticleExamples[this.exampleIndex].name == "CFX_Fountain")
		{
			base.GetComponent<Renderer>().material = this.waterMat;
			return;
		}
		base.GetComponent<Renderer>().material = this.groundMat;
	}

	// Token: 0x0400021D RID: 541
	public bool orderedSpawns = true;

	// Token: 0x0400021E RID: 542
	public float step = 1f;

	// Token: 0x0400021F RID: 543
	public float range = 5f;

	// Token: 0x04000220 RID: 544
	private float order = -5f;

	// Token: 0x04000221 RID: 545
	public Material groundMat;

	// Token: 0x04000222 RID: 546
	public Material waterMat;

	// Token: 0x04000223 RID: 547
	public GameObject[] ParticleExamples;

	// Token: 0x04000224 RID: 548
	private Dictionary<string, float> ParticlesYOffsetD = new Dictionary<string, float>
	{
		{
			"CFX_ElectricGround",
			0.15f
		},
		{
			"CFX_ElectricityBall",
			1f
		},
		{
			"CFX_ElectricityBolt",
			1f
		},
		{
			"CFX_Explosion",
			2f
		},
		{
			"CFX_SmallExplosion",
			1.5f
		},
		{
			"CFX_SmokeExplosion",
			2.5f
		},
		{
			"CFX_Flame",
			1f
		},
		{
			"CFX_DoubleFlame",
			1f
		},
		{
			"CFX_Hit",
			1f
		},
		{
			"CFX_CircularLightWall",
			0.05f
		},
		{
			"CFX_LightWall",
			0.05f
		},
		{
			"CFX_Flash",
			2f
		},
		{
			"CFX_Poof",
			1.5f
		},
		{
			"CFX_Virus",
			1f
		},
		{
			"CFX_SmokePuffs",
			2f
		},
		{
			"CFX_Slash",
			1f
		},
		{
			"CFX_Splash",
			0.05f
		},
		{
			"CFX_Fountain",
			0.05f
		},
		{
			"CFX_Ripple",
			0.05f
		},
		{
			"CFX_Magic",
			2f
		},
		{
			"CFX_SoftStar",
			1f
		},
		{
			"CFX_SpikyAura_Sphere",
			1f
		},
		{
			"CFX_Firework",
			2.4f
		},
		{
			"CFX_GroundA",
			0.05f
		}
	};

	// Token: 0x04000225 RID: 549
	private int exampleIndex;

	// Token: 0x04000226 RID: 550
	private string randomSpawnsDelay = "0.5";

	// Token: 0x04000227 RID: 551
	private bool randomSpawns;

	// Token: 0x04000228 RID: 552
	private bool slowMo;
}
