using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Token: 0x020000BA RID: 186
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("SECTR/Vis/SECTR Culling Camera")]
public class SECTR_CullingCamera : MonoBehaviour
{
	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000441 RID: 1089 RVA: 0x00019E9D File Offset: 0x0001809D
	public static List<SECTR_CullingCamera> All
	{
		get
		{
			return SECTR_CullingCamera.allCullingCameras;
		}
	}

	// Token: 0x170000AB RID: 171
	// (set) Token: 0x06000442 RID: 1090 RVA: 0x00019EA4 File Offset: 0x000180A4
	public Camera CullingCamera
	{
		set
		{
			this.cullingProxy = value;
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000443 RID: 1091 RVA: 0x00019EAD File Offset: 0x000180AD
	public int RenderersCulled
	{
		get
		{
			return this.renderersCulled;
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000444 RID: 1092 RVA: 0x00019EB5 File Offset: 0x000180B5
	public int LightsCulled
	{
		get
		{
			return this.lightsCulled;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000445 RID: 1093 RVA: 0x00019EBD File Offset: 0x000180BD
	public int TerrainsCulled
	{
		get
		{
			return this.terrainsCulled;
		}
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00019EC5 File Offset: 0x000180C5
	public void ResetStats()
	{
		this.renderersCulled = 0;
		this.lightsCulled = 0;
		this.terrainsCulled = 0;
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00019EDC File Offset: 0x000180DC
	private void OnEnable()
	{
		SECTR_CullingCamera.allCullingCameras.Add(this);
		int num = Mathf.Min(this.NumWorkerThreads, SystemInfo.processorCount);
		for (int i = 0; i < num; i++)
		{
			Thread thread = new Thread(new ThreadStart(this._CullingWorker));
			thread.IsBackground = true;
			thread.Priority = System.Threading.ThreadPriority.Highest;
			thread.Start();
			this.workerThreads.Add(thread);
		}
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00019F44 File Offset: 0x00018144
	private void OnDisable()
	{
		SECTR_CullingCamera.allCullingCameras.Remove(this);
		int count = this.workerThreads.Count;
		for (int i = 0; i < count; i++)
		{
			this.workerThreads[i].Abort();
		}
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00003296 File Offset: 0x00001496
	private void OnDestroy()
	{
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00019F88 File Offset: 0x00018188
	private void OnPreCull()
	{
		Camera camera = (this.cullingProxy != null) ? this.cullingProxy : base.GetComponent<Camera>();
		Vector3 position = camera.transform.position;
		float num = Mathf.Cos(Mathf.Max(camera.fieldOfView, camera.fieldOfView * camera.aspect) * 0.5f * 0.017453292f);
		float num2 = camera.nearClipPlane / num * 1.001f;
		int invisibleLayer = this.InvisibleLayer;
		bool simpleCulling = this.SimpleCulling;
		if (this.cullingProxy)
		{
			SECTR_CullingCamera component = this.cullingProxy.GetComponent<SECTR_CullingCamera>();
			if (component)
			{
				invisibleLayer = component.InvisibleLayer;
				simpleCulling = component.SimpleCulling;
			}
		}
		int count = SECTR_LOD.All.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_LOD.All[i].SelectLOD(camera);
		}
		SECTR_Member component2 = base.GetComponent<SECTR_Member>();
		if (simpleCulling)
		{
			this.initialSectors.Clear();
			this.initialSectors.AddRange(SECTR_Sector.All);
		}
		else if (component2 && component2.enabled)
		{
			this.initialSectors.Clear();
			this.initialSectors.AddRange(component2.Sectors);
		}
		else
		{
			SECTR_Sector.GetContaining(ref this.initialSectors, new Bounds(position, new Vector3(num2, num2, num2)), false);
		}
		int count2 = this.initialSectors.Count;
		if (base.enabled && camera.enabled && count2 > 0)
		{
			this.didCull = true;
			int count3 = this.workerThreads.Count;
			float shadowDistance = QualitySettings.shadowDistance;
			int count4 = SECTR_Member.All.Count;
			for (int j = 0; j < count4; j++)
			{
				SECTR_Member sectr_Member = SECTR_Member.All[j];
				if (sectr_Member.ShadowLight)
				{
					int count5 = sectr_Member.ShadowLights.Count;
					for (int k = 0; k < count5; k++)
					{
						SECTR_Member.Child child = sectr_Member.ShadowLights[k];
						if (child.light)
						{
							child.shadowLightPosition = child.light.transform.position;
							child.shadowLightRange = child.light.range;
						}
						sectr_Member.ShadowLights[k] = child;
					}
				}
			}
			this.nodeStack.Clear();
			this.shadowLights.Clear();
			this.visibleRenderers.Clear();
			this.visibleLights.Clear();
			this.visibleTerrains.Clear();
			Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
			for (int l = 0; l < count2; l++)
			{
				SECTR_Sector sector = this.initialSectors[l];
				this.nodeStack.Push(new SECTR_CullingCamera.VisibilityNode(this, sector, null, array, true));
			}
			while (this.nodeStack.Count > 0)
			{
				SECTR_CullingCamera.VisibilityNode visibilityNode = this.nodeStack.Pop();
				if (visibilityNode.frustumPlanes != null)
				{
					this.cullingPlanes.Clear();
					this.cullingPlanes.AddRange(visibilityNode.frustumPlanes);
					int count6 = this.cullingPlanes.Count;
					for (int m = 0; m < count6; m++)
					{
						Plane plane = this.cullingPlanes[m];
						Plane plane2 = this.cullingPlanes[(m + 1) % this.cullingPlanes.Count];
						float num3 = Vector3.Dot(plane.normal, plane2.normal);
						if (num3 < -0.9f && num3 > -0.99f)
						{
							Vector3 vector = plane.normal + plane2.normal;
							Vector3 vector2 = Vector3.Cross(plane.normal, plane2.normal);
							Vector3 inNormal = vector - Vector3.Dot(vector, vector2) * vector2;
							inNormal.Normalize();
							Matrix4x4 matrix4x = default(Matrix4x4);
							matrix4x.SetRow(0, new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, 0f));
							matrix4x.SetRow(1, new Vector4(plane2.normal.x, plane2.normal.y, plane2.normal.z, 0f));
							matrix4x.SetRow(2, new Vector4(vector2.x, vector2.y, vector2.z, 0f));
							matrix4x.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
							Vector3 inPoint = matrix4x.inverse.MultiplyPoint3x4(new Vector3(-plane.distance, -plane2.distance, 0f));
							this.cullingPlanes.Insert(++m, new Plane(inNormal, inPoint));
						}
					}
					count6 = this.cullingPlanes.Count;
					int num4 = 0;
					for (int n = 0; n < count6; n++)
					{
						num4 |= 1 << n;
					}
					SECTR_Sector sector2 = visibilityNode.sector;
					if (SECTR_Occluder.All.Count > 0)
					{
						List<SECTR_Occluder> occludersInSector = SECTR_Occluder.GetOccludersInSector(sector2);
						if (occludersInSector != null)
						{
							int count7 = occludersInSector.Count;
							for (int num5 = 0; num5 < count7; num5++)
							{
								SECTR_Occluder sectr_Occluder = occludersInSector[num5];
								if (sectr_Occluder.HullMesh && !this.activeOccluders.ContainsKey(sectr_Occluder))
								{
									Matrix4x4 cullingMatrix = sectr_Occluder.GetCullingMatrix(position);
									Vector3[] vertsCW = sectr_Occluder.VertsCW;
									Vector3 normalized = cullingMatrix.MultiplyVector(-sectr_Occluder.MeshNormal).normalized;
									if (vertsCW != null && !SECTR_Geometry.IsPointInFrontOfPlane(position, sectr_Occluder.Center, normalized))
									{
										int num6 = vertsCW.Length;
										this.occluderVerts.Clear();
										Bounds bounds = new Bounds(sectr_Occluder.transform.position, Vector3.zero);
										for (int num7 = 0; num7 < num6; num7++)
										{
											Vector3 vector3 = cullingMatrix.MultiplyPoint3x4(vertsCW[num7]);
											bounds.Encapsulate(vector3);
											this.occluderVerts.Add(new SECTR_CullingCamera.ClipVertex(new Vector4(vector3.x, vector3.y, vector3.z, 1f), 0f));
										}
										int num8;
										if (SECTR_Geometry.FrustumIntersectsBounds(sectr_Occluder.BoundingBox, this.cullingPlanes, num4, out num8))
										{
											List<Plane> list;
											if (this.frustumPool.Count > 0)
											{
												list = this.frustumPool.Pop();
												list.Clear();
											}
											else
											{
												list = new List<Plane>(num6 + 1);
											}
											this._BuildFrustumFromHull(camera, true, this.occluderVerts, ref list);
											list.Add(new Plane(normalized, sectr_Occluder.Center));
											this.occluderFrustums.Add(list);
											this.activeOccluders[sectr_Occluder] = sectr_Occluder;
										}
									}
								}
							}
						}
					}
					if (count3 > 0)
					{
						Queue<SECTR_CullingCamera.ThreadCullData> obj = this.cullingWorkQueue;
						lock (obj)
						{
							this.cullingWorkQueue.Enqueue(new SECTR_CullingCamera.ThreadCullData(sector2, this, position, this.cullingPlanes, this.occluderFrustums, num4, shadowDistance, simpleCulling));
							Monitor.Pulse(this.cullingWorkQueue);
						}
						Interlocked.Increment(ref this.remainingThreadWork);
					}
					else
					{
						SECTR_CullingCamera._FrustumCullSector(sector2, position, this.cullingPlanes, this.occluderFrustums, num4, shadowDistance, simpleCulling, ref this.visibleRenderers, ref this.visibleLights, ref this.visibleTerrains, ref this.shadowLights);
					}
					int num9 = simpleCulling ? 0 : visibilityNode.sector.Portals.Count;
					for (int num10 = 0; num10 < num9; num10++)
					{
						SECTR_Portal sectr_Portal = visibilityNode.sector.Portals[num10];
						bool flag2 = (sectr_Portal.Flags & SECTR_Portal.PortalFlags.PassThrough) > (SECTR_Portal.PortalFlags)0;
						if ((sectr_Portal.HullMesh || flag2) && (sectr_Portal.Flags & SECTR_Portal.PortalFlags.Closed) == (SECTR_Portal.PortalFlags)0)
						{
							bool flag3 = visibilityNode.sector == sectr_Portal.FrontSector;
							SECTR_Sector sectr_Sector = flag3 ? sectr_Portal.BackSector : sectr_Portal.FrontSector;
							bool flag4 = !sectr_Sector;
							if (!flag4)
							{
								flag4 = (SECTR_Geometry.IsPointInFrontOfPlane(position, sectr_Portal.Center, sectr_Portal.Normal) != flag3);
							}
							if (!flag4 && visibilityNode.portal)
							{
								Vector3 normalized2 = (sectr_Portal.Center - visibilityNode.portal.Center).normalized;
								Vector3 rhs = visibilityNode.forwardTraversal ? visibilityNode.portal.ReverseNormal : visibilityNode.portal.Normal;
								flag4 = (Vector3.Dot(normalized2, rhs) < 0f);
							}
							if (!flag4 && !flag2)
							{
								int count8 = this.occluderFrustums.Count;
								for (int num11 = 0; num11 < count8; num11++)
								{
									if (SECTR_Geometry.FrustumContainsBounds(sectr_Portal.BoundingBox, this.occluderFrustums[num11]))
									{
										flag4 = true;
										break;
									}
								}
							}
							if (!flag4)
							{
								if (!flag2)
								{
									this.portalVertices.Clear();
									Matrix4x4 localToWorldMatrix = sectr_Portal.transform.localToWorldMatrix;
									Vector3[] array2 = flag3 ? sectr_Portal.VertsCCW : sectr_Portal.VertsCW;
									if (array2 != null)
									{
										int num12 = array2.Length;
										for (int num13 = 0; num13 < num12; num13++)
										{
											Vector3 vector4 = localToWorldMatrix.MultiplyPoint3x4(array2[num13]);
											this.portalVertices.Add(new SECTR_CullingCamera.ClipVertex(new Vector4(vector4.x, vector4.y, vector4.z, 1f), 0f));
										}
									}
								}
								this.newFrustum.Clear();
								if (!flag2 && !sectr_Portal.IsPointInHull(position, num2))
								{
									int count9 = visibilityNode.frustumPlanes.Count;
									for (int num14 = 0; num14 < count9; num14++)
									{
										Plane plane3 = visibilityNode.frustumPlanes[num14];
										Vector4 a = new Vector4(plane3.normal.x, plane3.normal.y, plane3.normal.z, plane3.distance);
										bool flag5 = true;
										bool flag6 = true;
										for (int num15 = 0; num15 < this.portalVertices.Count; num15++)
										{
											Vector4 vertex = this.portalVertices[num15].vertex;
											float num16 = Vector4.Dot(a, vertex);
											this.portalVertices[num15] = new SECTR_CullingCamera.ClipVertex(vertex, num16);
											flag5 = (flag5 && num16 > 0f);
											flag6 = (flag6 && num16 <= -0.001f);
										}
										if (flag6)
										{
											this.portalVertices.Clear();
											break;
										}
										if (!flag5)
										{
											int num17 = this.portalVertices.Count;
											for (int num18 = 0; num18 < num17; num18++)
											{
												int index = (num18 + 1) % this.portalVertices.Count;
												float side = this.portalVertices[num18].side;
												float side2 = this.portalVertices[index].side;
												if ((side > 0f && side2 <= -0.001f) || (side2 > 0f && side <= -0.001f))
												{
													Vector4 vertex2 = this.portalVertices[num18].vertex;
													Vector4 vertex3 = this.portalVertices[index].vertex;
													float d = side / Vector4.Dot(a, vertex2 - vertex3);
													Vector4 vertex4 = vertex2 + d * (vertex3 - vertex2);
													vertex4.w = 1f;
													this.portalVertices.Insert(num18 + 1, new SECTR_CullingCamera.ClipVertex(vertex4, 0f));
													num17++;
												}
											}
											int num19 = 0;
											while (num19 < num17)
											{
												if (this.portalVertices[num19].side < -0.001f)
												{
													this.portalVertices.RemoveAt(num19);
													num17--;
												}
												else
												{
													num19++;
												}
											}
										}
									}
									this._BuildFrustumFromHull(camera, flag3, this.portalVertices, ref this.newFrustum);
								}
								else
								{
									this.newFrustum.AddRange(array);
								}
								if (this.newFrustum.Count > 2)
								{
									this.nodeStack.Push(new SECTR_CullingCamera.VisibilityNode(this, sectr_Sector, sectr_Portal, this.newFrustum, flag3));
								}
							}
						}
					}
				}
				if (visibilityNode.frustumPlanes != null)
				{
					visibilityNode.frustumPlanes.Clear();
					this.frustumPool.Push(visibilityNode.frustumPlanes);
				}
			}
			if (count3 > 0)
			{
				while (this.remainingThreadWork > 0)
				{
					while (this.cullingWorkQueue.Count > 0)
					{
						SECTR_CullingCamera.ThreadCullData threadCullData = default(SECTR_CullingCamera.ThreadCullData);
						Queue<SECTR_CullingCamera.ThreadCullData> obj = this.cullingWorkQueue;
						lock (obj)
						{
							if (this.cullingWorkQueue.Count > 0)
							{
								threadCullData = this.cullingWorkQueue.Dequeue();
							}
						}
						if (threadCullData.cullingMode == SECTR_CullingCamera.ThreadCullData.CullingModes.Graph)
						{
							this._FrustumCullSectorThread(threadCullData);
							Interlocked.Decrement(ref this.remainingThreadWork);
						}
					}
				}
				this.remainingThreadWork = 0;
			}
			if (this.shadowLights.Count > 0 && this.CullShadows)
			{
				this.shadowSectorTable.Clear();
				foreach (KeyValuePair<SECTR_Member.Child, int> keyValuePair in this.shadowLights)
				{
					SECTR_Member.Child key = keyValuePair.Key;
					List<SECTR_Sector> sectors;
					if (key.member && key.member.IsSector)
					{
						this.shadowSectors.Clear();
						this.shadowSectors.Add((SECTR_Sector)key.member);
						sectors = this.shadowSectors;
					}
					else if (key.member && key.member.Sectors.Count > 0)
					{
						sectors = key.member.Sectors;
					}
					else
					{
						SECTR_Sector.GetContaining(ref this.shadowSectors, key.lightBounds, false);
						sectors = this.shadowSectors;
					}
					int count10 = sectors.Count;
					for (int num20 = 0; num20 < count10; num20++)
					{
						SECTR_Sector key2 = sectors[num20];
						List<SECTR_Member.Child> list2;
						if (!this.shadowSectorTable.TryGetValue(key2, out list2))
						{
							list2 = ((this.shadowLightPool.Count > 0) ? this.shadowLightPool.Pop() : new List<SECTR_Member.Child>(16));
							this.shadowSectorTable[key2] = list2;
						}
						list2.Add(key);
					}
				}
				foreach (KeyValuePair<SECTR_Sector, List<SECTR_Member.Child>> keyValuePair2 in this.shadowSectorTable)
				{
					SECTR_Sector key3 = keyValuePair2.Key;
					Dictionary<SECTR_Sector, List<SECTR_Member.Child>>.Enumerator enumerator2;
					keyValuePair2 = enumerator2.Current;
					List<SECTR_Member.Child> value = keyValuePair2.Value;
					if (count3 > 0)
					{
						Queue<SECTR_CullingCamera.ThreadCullData> obj = this.cullingWorkQueue;
						lock (obj)
						{
							this.cullingWorkQueue.Enqueue(new SECTR_CullingCamera.ThreadCullData(key3, position, value));
							Monitor.Pulse(this.cullingWorkQueue);
						}
						Interlocked.Increment(ref this.remainingThreadWork);
					}
					else
					{
						SECTR_CullingCamera._ShadowCullSector(key3, value, ref this.visibleRenderers, ref this.visibleTerrains);
					}
				}
				if (count3 > 0)
				{
					while (this.remainingThreadWork > 0)
					{
						while (this.cullingWorkQueue.Count > 0)
						{
							SECTR_CullingCamera.ThreadCullData threadCullData2 = default(SECTR_CullingCamera.ThreadCullData);
							Queue<SECTR_CullingCamera.ThreadCullData> obj = this.cullingWorkQueue;
							lock (obj)
							{
								if (this.cullingWorkQueue.Count > 0)
								{
									threadCullData2 = this.cullingWorkQueue.Dequeue();
								}
							}
							if (threadCullData2.cullingMode == SECTR_CullingCamera.ThreadCullData.CullingModes.Shadow)
							{
								this._ShadowCullSectorThread(threadCullData2);
								Interlocked.Decrement(ref this.remainingThreadWork);
							}
						}
					}
					this.remainingThreadWork = 0;
				}
				foreach (KeyValuePair<SECTR_Sector, List<SECTR_Member.Child>> keyValuePair2 in this.shadowSectorTable)
				{
					List<SECTR_Member.Child> value2 = keyValuePair2.Value;
					value2.Clear();
					this.shadowLightPool.Push(value2);
				}
			}
			this._ApplyCulling(invisibleLayer);
			int count11 = this.occluderFrustums.Count;
			for (int num21 = 0; num21 < count11; num21++)
			{
				this.occluderFrustums[num21].Clear();
				this.frustumPool.Push(this.occluderFrustums[num21]);
			}
			this.occluderFrustums.Clear();
			this.activeOccluders.Clear();
		}
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0001AFE8 File Offset: 0x000191E8
	private void OnPostRender()
	{
		if (this.didCull)
		{
			this.UndoCulling();
			this.didCull = false;
		}
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x0001B000 File Offset: 0x00019200
	private void _CullingWorker()
	{
		for (;;)
		{
			SECTR_CullingCamera.ThreadCullData threadCullData = default(SECTR_CullingCamera.ThreadCullData);
			Queue<SECTR_CullingCamera.ThreadCullData> obj = this.cullingWorkQueue;
			lock (obj)
			{
				while (this.cullingWorkQueue.Count == 0)
				{
					Monitor.Wait(this.cullingWorkQueue);
				}
				threadCullData = this.cullingWorkQueue.Dequeue();
			}
			switch (threadCullData.cullingMode)
			{
			case SECTR_CullingCamera.ThreadCullData.CullingModes.Graph:
				this._FrustumCullSectorThread(threadCullData);
				break;
			case SECTR_CullingCamera.ThreadCullData.CullingModes.Shadow:
				this._ShadowCullSectorThread(threadCullData);
				break;
			}
			if (threadCullData.cullingMode == SECTR_CullingCamera.ThreadCullData.CullingModes.Graph || threadCullData.cullingMode == SECTR_CullingCamera.ThreadCullData.CullingModes.Shadow)
			{
				Interlocked.Decrement(ref this.remainingThreadWork);
			}
		}
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x0001B0BC File Offset: 0x000192BC
	private void _FrustumCullSectorThread(SECTR_CullingCamera.ThreadCullData cullData)
	{
		Dictionary<int, int> dictionary = null;
		Dictionary<int, int> dictionary2 = null;
		Dictionary<int, int> dictionary3 = null;
		Dictionary<SECTR_Member.Child, int> dictionary4 = null;
		Stack<Dictionary<int, int>> obj = this.threadVisibleListPool;
		lock (obj)
		{
			dictionary = ((this.threadVisibleListPool.Count > 0) ? this.threadVisibleListPool.Pop() : new Dictionary<int, int>(32));
			dictionary2 = ((this.threadVisibleListPool.Count > 0) ? this.threadVisibleListPool.Pop() : new Dictionary<int, int>(32));
			dictionary3 = ((this.threadVisibleListPool.Count > 0) ? this.threadVisibleListPool.Pop() : new Dictionary<int, int>(32));
		}
		Stack<Dictionary<SECTR_Member.Child, int>> obj2 = this.threadShadowLightPool;
		lock (obj2)
		{
			dictionary4 = ((this.threadShadowLightPool.Count > 0) ? this.threadShadowLightPool.Pop() : new Dictionary<SECTR_Member.Child, int>(32));
		}
		SECTR_CullingCamera._FrustumCullSector(cullData.sector, cullData.cameraPos, cullData.cullingPlanes, cullData.occluderFrustums, cullData.baseMask, cullData.shadowDistance, cullData.cullingSimpleCulling, ref dictionary, ref dictionary2, ref dictionary3, ref dictionary4);
		Dictionary<int, int> obj3 = this.visibleRenderers;
		lock (obj3)
		{
			foreach (KeyValuePair<int, int> keyValuePair in dictionary)
			{
				int key = keyValuePair.Key;
				this.visibleRenderers[key] = key;
			}
		}
		obj3 = this.visibleLights;
		lock (obj3)
		{
			foreach (KeyValuePair<int, int> keyValuePair in dictionary2)
			{
				int key2 = keyValuePair.Key;
				this.visibleLights[key2] = key2;
			}
		}
		obj3 = this.visibleTerrains;
		lock (obj3)
		{
			foreach (KeyValuePair<int, int> keyValuePair in dictionary3)
			{
				int key3 = keyValuePair.Key;
				this.visibleTerrains[key3] = key3;
			}
		}
		Dictionary<SECTR_Member.Child, int> obj4 = this.shadowLights;
		lock (obj4)
		{
			Dictionary<SECTR_Member.Child, int>.Enumerator enumerator2 = dictionary4.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Dictionary<SECTR_Member.Child, int> dictionary5 = this.shadowLights;
				KeyValuePair<SECTR_Member.Child, int> keyValuePair2 = enumerator2.Current;
				dictionary5[keyValuePair2.Key] = 0;
			}
		}
		obj = this.threadVisibleListPool;
		lock (obj)
		{
			dictionary.Clear();
			dictionary2.Clear();
			dictionary3.Clear();
			this.threadVisibleListPool.Push(dictionary);
			this.threadVisibleListPool.Push(dictionary2);
			this.threadVisibleListPool.Push(dictionary3);
		}
		obj2 = this.threadShadowLightPool;
		lock (obj2)
		{
			dictionary4.Clear();
			this.threadShadowLightPool.Push(dictionary4);
		}
		Stack<List<Plane>> obj5 = this.threadFrustumPool;
		lock (obj5)
		{
			cullData.cullingPlanes.Clear();
			this.threadFrustumPool.Push(cullData.cullingPlanes);
			int count = cullData.occluderFrustums.Count;
			for (int i = 0; i < count; i++)
			{
				cullData.occluderFrustums[i].Clear();
				this.threadFrustumPool.Push(cullData.occluderFrustums[i]);
			}
		}
		Stack<List<List<Plane>>> obj6 = this.threadOccluderPool;
		lock (obj6)
		{
			cullData.occluderFrustums.Clear();
			this.threadOccluderPool.Push(cullData.occluderFrustums);
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0001B4F8 File Offset: 0x000196F8
	private void _ShadowCullSectorThread(SECTR_CullingCamera.ThreadCullData cullData)
	{
		Dictionary<int, int> dictionary = null;
		Dictionary<int, int> dictionary2 = null;
		Stack<Dictionary<int, int>> obj = this.threadVisibleListPool;
		lock (obj)
		{
			dictionary = ((this.threadVisibleListPool.Count > 0) ? this.threadVisibleListPool.Pop() : new Dictionary<int, int>(32));
			dictionary2 = ((this.threadVisibleListPool.Count > 0) ? this.threadVisibleListPool.Pop() : new Dictionary<int, int>(32));
		}
		SECTR_CullingCamera._ShadowCullSector(cullData.sector, cullData.sectorShadowLights, ref dictionary, ref dictionary2);
		Dictionary<int, int> obj2 = this.visibleRenderers;
		lock (obj2)
		{
			foreach (KeyValuePair<int, int> keyValuePair in dictionary)
			{
				int key = keyValuePair.Key;
				this.visibleRenderers[key] = key;
			}
		}
		obj2 = this.visibleTerrains;
		lock (obj2)
		{
			foreach (KeyValuePair<int, int> keyValuePair in dictionary2)
			{
				int key2 = keyValuePair.Key;
				this.visibleTerrains[key2] = key2;
			}
		}
		obj = this.threadVisibleListPool;
		lock (obj)
		{
			dictionary.Clear();
			dictionary2.Clear();
			this.threadVisibleListPool.Push(dictionary);
			this.threadVisibleListPool.Push(dictionary2);
		}
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x0001B6A4 File Offset: 0x000198A4
	private static void _FrustumCullSector(SECTR_Sector sector, Vector3 cameraPos, List<Plane> cullingPlanes, List<List<Plane>> occluderFrustums, int baseMask, float shadowDistance, bool forceGroupCull, ref Dictionary<int, int> visibleRenderers, ref Dictionary<int, int> visibleLights, ref Dictionary<int, int> visibleTerrains, ref Dictionary<SECTR_Member.Child, int> shadowLights)
	{
		SECTR_CullingCamera._FrustumCull(sector, cameraPos, cullingPlanes, occluderFrustums, baseMask, shadowDistance, forceGroupCull, ref visibleRenderers, ref visibleLights, ref visibleTerrains, ref shadowLights);
		int count = sector.Members.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Member sectr_Member = sector.Members[i];
			if (sectr_Member.HasRenderBounds || sectr_Member.HasLightBounds)
			{
				SECTR_CullingCamera._FrustumCull(sectr_Member, cameraPos, cullingPlanes, occluderFrustums, baseMask, shadowDistance, forceGroupCull, ref visibleRenderers, ref visibleLights, ref visibleTerrains, ref shadowLights);
			}
		}
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x0001B714 File Offset: 0x00019914
	private static void _FrustumCull(SECTR_Member member, Vector3 cameraPos, List<Plane> frustumPlanes, List<List<Plane>> occluders, int baseMask, float shadowDistance, bool forceGroupCull, ref Dictionary<int, int> visibleRenderers, ref Dictionary<int, int> visibleLights, ref Dictionary<int, int> visibleTerrains, ref Dictionary<SECTR_Member.Child, int> shadowLights)
	{
		int parentMask = 0;
		int num = 0;
		bool flag = member.CullEachChild && !forceGroupCull;
		bool flag2 = member.HasRenderBounds && SECTR_Geometry.FrustumIntersectsBounds(member.RenderBounds, frustumPlanes, baseMask, out parentMask);
		bool flag3 = member.HasLightBounds && SECTR_Geometry.FrustumIntersectsBounds(member.LightBounds, frustumPlanes, baseMask, out num);
		int count = occluders.Count;
		int num2 = 0;
		while (num2 < count && (flag2 || flag3))
		{
			List<Plane> frustum = occluders[num2];
			if (flag2)
			{
				flag2 = !SECTR_Geometry.FrustumContainsBounds(member.RenderBounds, frustum);
			}
			if (flag3)
			{
				flag3 = !SECTR_Geometry.FrustumContainsBounds(member.LightBounds, frustum);
			}
			num2++;
		}
		if (flag2)
		{
			int count2 = member.Renderers.Count;
			for (int i = 0; i < count2; i++)
			{
				SECTR_Member.Child child = member.Renderers[i];
				if (child.renderHash != 0 && !visibleRenderers.ContainsKey(child.renderHash) && (!flag || SECTR_CullingCamera._IsVisible(child.rendererBounds, frustumPlanes, parentMask, occluders)))
				{
					visibleRenderers.Add(child.renderHash, child.renderHash);
				}
			}
			int count3 = member.Terrains.Count;
			for (int j = 0; j < count3; j++)
			{
				SECTR_Member.Child child2 = member.Terrains[j];
				if (child2.terrainHash != 0 && !visibleTerrains.ContainsKey(child2.terrainHash) && (!flag || SECTR_CullingCamera._IsVisible(child2.terrainBounds, frustumPlanes, parentMask, occluders)))
				{
					visibleTerrains.Add(child2.terrainHash, child2.terrainHash);
				}
			}
		}
		if (flag3)
		{
			int count4 = member.Lights.Count;
			for (int k = 0; k < count4; k++)
			{
				SECTR_Member.Child child3 = member.Lights[k];
				if (child3.lightHash != 0 && !visibleLights.ContainsKey(child3.lightHash) && (!flag || SECTR_CullingCamera._IsVisible(child3.lightBounds, frustumPlanes, parentMask, occluders)))
				{
					visibleLights.Add(child3.lightHash, child3.lightHash);
					if (child3.shadowLight && !shadowLights.ContainsKey(child3) && Vector3.Distance(cameraPos, child3.shadowLightPosition) - child3.shadowLightRange <= shadowDistance)
					{
						shadowLights.Add(child3, 0);
					}
				}
			}
		}
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x0001B970 File Offset: 0x00019B70
	private static void _ShadowCullSector(SECTR_Sector sector, List<SECTR_Member.Child> sectorShadowLights, ref Dictionary<int, int> visibleRenderers, ref Dictionary<int, int> visibleTerrains)
	{
		if (sector.ShadowCaster)
		{
			SECTR_CullingCamera._ShadowCull(sector, sectorShadowLights, ref visibleRenderers, ref visibleTerrains);
		}
		int count = sector.Members.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Member sectr_Member = sector.Members[i];
			if (sectr_Member.ShadowCaster)
			{
				SECTR_CullingCamera._ShadowCull(sectr_Member, sectorShadowLights, ref visibleRenderers, ref visibleTerrains);
			}
		}
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x0001B9C4 File Offset: 0x00019BC4
	private static void _ShadowCull(SECTR_Member member, List<SECTR_Member.Child> shadowLights, ref Dictionary<int, int> visibleRenderers, ref Dictionary<int, int> visibleTerrains)
	{
		int count = shadowLights.Count;
		int count2 = member.ShadowCasters.Count;
		if (member.CullEachChild)
		{
			for (int i = 0; i < count2; i++)
			{
				SECTR_Member.Child child = member.ShadowCasters[i];
				if (child.renderHash != 0 && !visibleRenderers.ContainsKey(child.renderHash))
				{
					for (int j = 0; j < count; j++)
					{
						SECTR_Member.Child child2 = shadowLights[j];
						if ((child2.shadowCullingMask & 1 << child.layer) != 0 && ((child2.shadowLightType == LightType.Spot && child.rendererBounds.Intersects(child2.lightBounds)) || (child2.shadowLightType == LightType.Point && SECTR_Geometry.BoundsIntersectsSphere(child.rendererBounds, child2.shadowLightPosition, child2.shadowLightRange))))
						{
							visibleRenderers.Add(child.renderHash, child.renderHash);
							break;
						}
					}
				}
				if (child.terrainHash != 0 && !visibleTerrains.ContainsKey(child.terrainHash))
				{
					for (int k = 0; k < count; k++)
					{
						SECTR_Member.Child child3 = shadowLights[k];
						if ((child3.shadowCullingMask & 1 << child.layer) != 0 && ((child3.shadowLightType == LightType.Spot && child.terrainBounds.Intersects(child3.lightBounds)) || (child3.shadowLightType == LightType.Point && SECTR_Geometry.BoundsIntersectsSphere(child.terrainBounds, child3.shadowLightPosition, child3.shadowLightRange))))
						{
							visibleTerrains.Add(child.terrainHash, child.terrainHash);
							break;
						}
					}
				}
			}
			return;
		}
		for (int l = 0; l < count; l++)
		{
			SECTR_Member.Child child4 = shadowLights[l];
			if ((child4.shadowLightType == LightType.Spot) ? member.RenderBounds.Intersects(child4.lightBounds) : SECTR_Geometry.BoundsIntersectsSphere(member.RenderBounds, child4.shadowLightPosition, child4.shadowLightRange))
			{
				int shadowCullingMask = child4.shadowCullingMask;
				for (int m = 0; m < count2; m++)
				{
					SECTR_Member.Child child5 = member.ShadowCasters[m];
					if (child5.renderHash != 0 && child5.terrainHash != 0)
					{
						if ((shadowCullingMask & 1 << child5.layer) != 0)
						{
							if (!visibleRenderers.ContainsKey(child5.renderHash))
							{
								visibleRenderers.Add(child5.renderHash, child5.renderHash);
							}
							if (!visibleTerrains.ContainsKey(child5.terrainHash))
							{
								visibleTerrains.Add(child5.terrainHash, child5.terrainHash);
							}
						}
					}
					else if (child5.renderHash != 0 && !visibleRenderers.ContainsKey(child5.renderHash) && (shadowCullingMask & 1 << child5.layer) != 0)
					{
						visibleRenderers.Add(child5.renderHash, child5.renderHash);
					}
					else if (child5.terrainHash != 0 && !visibleTerrains.ContainsKey(child5.terrainHash) && (shadowCullingMask & 1 << child5.layer) != 0)
					{
						visibleTerrains.Add(child5.terrainHash, child5.terrainHash);
					}
				}
			}
		}
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x0001BD00 File Offset: 0x00019F00
	private static bool _IsVisible(Bounds childBounds, List<Plane> frustumPlanes, int parentMask, List<List<Plane>> occluders)
	{
		int num;
		if (SECTR_Geometry.FrustumIntersectsBounds(childBounds, frustumPlanes, parentMask, out num))
		{
			int count = occluders.Count;
			for (int i = 0; i < count; i++)
			{
				if (SECTR_Geometry.FrustumContainsBounds(childBounds, occluders[i]))
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x0001BD40 File Offset: 0x00019F40
	private void _ApplyCulling(int cullingInvisibleLayer)
	{
		int count = SECTR_Member.All.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Member sectr_Member = SECTR_Member.All[i];
			int count2 = sectr_Member.Children.Count;
			for (int j = 0; j < count2; j++)
			{
				SECTR_Member.Child child = sectr_Member.Children[j];
				Renderer renderer = child.renderer;
				if (renderer && !this.visibleRenderers.ContainsKey(child.renderHash) && renderer.enabled)
				{
					this.hiddenRenderers.Add(renderer);
					renderer.enabled = false;
				}
				Light light = child.light;
				if (light && !this.visibleLights.ContainsKey(child.lightHash) && light.enabled)
				{
					this.hiddenLights.Add(light);
					light.enabled = false;
				}
				Terrain terrain = child.terrain;
				if (terrain && !this.visibleTerrains.ContainsKey(child.terrainHash) && terrain.enabled)
				{
					this.hiddenTerrains.Add(terrain);
					terrain.enabled = false;
				}
			}
		}
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x0001BE74 File Offset: 0x0001A074
	private void UndoCulling()
	{
		int count = this.hiddenRenderers.Count;
		for (int i = 0; i < count; i++)
		{
			this.hiddenRenderers[i].enabled = true;
		}
		this.hiddenRenderers.Clear();
		int count2 = this.hiddenLights.Count;
		for (int j = 0; j < count2; j++)
		{
			this.hiddenLights[j].enabled = true;
		}
		this.hiddenLights.Clear();
		int count3 = this.hiddenTerrains.Count;
		for (int k = 0; k < count3; k++)
		{
			this.hiddenTerrains[k].enabled = true;
		}
		this.hiddenTerrains.Clear();
		this.renderersCulled = count;
		this.lightsCulled = count2;
		this.terrainsCulled = count3;
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x0001BF40 File Offset: 0x0001A140
	private void _BuildFrustumFromHull(Camera cullingCamera, bool forwardTraversal, List<SECTR_CullingCamera.ClipVertex> portalVertices, ref List<Plane> newFrustum)
	{
		int count = portalVertices.Count;
		if (count > 2)
		{
			for (int i = 0; i < count; i++)
			{
				Vector3 vector = portalVertices[i].vertex;
				Vector3 vector2 = portalVertices[(i + 1) % count].vertex - vector;
				if (Vector3.SqrMagnitude(vector2) > 0.001f)
				{
					Vector3 vector3 = vector - cullingCamera.transform.position;
					Vector3 inNormal = forwardTraversal ? Vector3.Cross(vector2, vector3) : Vector3.Cross(vector3, vector2);
					inNormal.Normalize();
					newFrustum.Add(new Plane(inNormal, vector));
				}
			}
		}
	}

	// Token: 0x0400042F RID: 1071
	private List<Renderer> hiddenRenderers = new List<Renderer>(16);

	// Token: 0x04000430 RID: 1072
	private List<Light> hiddenLights = new List<Light>(16);

	// Token: 0x04000431 RID: 1073
	private List<Terrain> hiddenTerrains = new List<Terrain>(2);

	// Token: 0x04000432 RID: 1074
	private int renderersCulled;

	// Token: 0x04000433 RID: 1075
	private int lightsCulled;

	// Token: 0x04000434 RID: 1076
	private int terrainsCulled;

	// Token: 0x04000435 RID: 1077
	private bool didCull;

	// Token: 0x04000436 RID: 1078
	private List<SECTR_Sector> initialSectors = new List<SECTR_Sector>(4);

	// Token: 0x04000437 RID: 1079
	private Stack<SECTR_CullingCamera.VisibilityNode> nodeStack = new Stack<SECTR_CullingCamera.VisibilityNode>(10);

	// Token: 0x04000438 RID: 1080
	private List<SECTR_CullingCamera.ClipVertex> portalVertices = new List<SECTR_CullingCamera.ClipVertex>(16);

	// Token: 0x04000439 RID: 1081
	private List<Plane> newFrustum = new List<Plane>(16);

	// Token: 0x0400043A RID: 1082
	private List<Plane> cullingPlanes = new List<Plane>(16);

	// Token: 0x0400043B RID: 1083
	private List<List<Plane>> occluderFrustums = new List<List<Plane>>(10);

	// Token: 0x0400043C RID: 1084
	private Dictionary<SECTR_Occluder, SECTR_Occluder> activeOccluders = new Dictionary<SECTR_Occluder, SECTR_Occluder>(10);

	// Token: 0x0400043D RID: 1085
	private List<SECTR_CullingCamera.ClipVertex> occluderVerts = new List<SECTR_CullingCamera.ClipVertex>(10);

	// Token: 0x0400043E RID: 1086
	private Dictionary<SECTR_Member.Child, int> shadowLights = new Dictionary<SECTR_Member.Child, int>(10);

	// Token: 0x0400043F RID: 1087
	private List<SECTR_Sector> shadowSectors = new List<SECTR_Sector>(4);

	// Token: 0x04000440 RID: 1088
	private Dictionary<SECTR_Sector, List<SECTR_Member.Child>> shadowSectorTable = new Dictionary<SECTR_Sector, List<SECTR_Member.Child>>(4);

	// Token: 0x04000441 RID: 1089
	private Dictionary<int, int> visibleRenderers = new Dictionary<int, int>(1024);

	// Token: 0x04000442 RID: 1090
	private Dictionary<int, int> visibleLights = new Dictionary<int, int>(256);

	// Token: 0x04000443 RID: 1091
	private Dictionary<int, int> visibleTerrains = new Dictionary<int, int>(32);

	// Token: 0x04000444 RID: 1092
	private Stack<List<Plane>> frustumPool = new Stack<List<Plane>>(32);

	// Token: 0x04000445 RID: 1093
	private Stack<List<SECTR_Member.Child>> shadowLightPool = new Stack<List<SECTR_Member.Child>>(32);

	// Token: 0x04000446 RID: 1094
	private Stack<Dictionary<int, int>> threadVisibleListPool = new Stack<Dictionary<int, int>>(32);

	// Token: 0x04000447 RID: 1095
	private Stack<Dictionary<SECTR_Member.Child, int>> threadShadowLightPool = new Stack<Dictionary<SECTR_Member.Child, int>>(32);

	// Token: 0x04000448 RID: 1096
	private Stack<List<Plane>> threadFrustumPool = new Stack<List<Plane>>(32);

	// Token: 0x04000449 RID: 1097
	private Stack<List<List<Plane>>> threadOccluderPool = new Stack<List<List<Plane>>>(32);

	// Token: 0x0400044A RID: 1098
	private List<Thread> workerThreads = new List<Thread>();

	// Token: 0x0400044B RID: 1099
	private Queue<SECTR_CullingCamera.ThreadCullData> cullingWorkQueue = new Queue<SECTR_CullingCamera.ThreadCullData>(32);

	// Token: 0x0400044C RID: 1100
	private int remainingThreadWork;

	// Token: 0x0400044D RID: 1101
	private static List<SECTR_CullingCamera> allCullingCameras = new List<SECTR_CullingCamera>(4);

	// Token: 0x0400044E RID: 1102
	[SECTR_ToolTip("Forces culling into a mode designed for 2D and iso games where the camera is always outside the scene.")]
	public bool SimpleCulling;

	// Token: 0x0400044F RID: 1103
	[SECTR_ToolTip("The layer that culled objects should be assigned to.", false, true)]
	[HideInInspector]
	public int InvisibleLayer;

	// Token: 0x04000450 RID: 1104
	[SECTR_ToolTip("Distance to draw clipped frustums.", 0f, 100f)]
	public float GizmoDistance = 10f;

	// Token: 0x04000451 RID: 1105
	[SECTR_ToolTip("Material to use to render the debug frustum mesh.")]
	public Material GizmoMaterial;

	// Token: 0x04000452 RID: 1106
	[SECTR_ToolTip("Makes the Editor camera display the Game view's culling while playing in editor.")]
	public bool CullInEditor;

	// Token: 0x04000453 RID: 1107
	[SECTR_ToolTip("Set to false to disable shadow culling post pass.", true)]
	public bool CullShadows = true;

	// Token: 0x04000454 RID: 1108
	[SECTR_ToolTip("Use another camera for culling properties.", true)]
	public Camera cullingProxy;

	// Token: 0x04000455 RID: 1109
	[SECTR_ToolTip("Number of worker threads for culling. Do not set this too high or you may see hitching.", 0f, -1f)]
	public int NumWorkerThreads;

	// Token: 0x020000BB RID: 187
	private struct VisibilityNode
	{
		// Token: 0x06000459 RID: 1113 RVA: 0x0001C164 File Offset: 0x0001A364
		public VisibilityNode(SECTR_CullingCamera cullingCamera, SECTR_Sector sector, SECTR_Portal portal, Plane[] frustumPlanes, bool forwardTraversal)
		{
			this.sector = sector;
			this.portal = portal;
			if (frustumPlanes == null)
			{
				this.frustumPlanes = null;
			}
			else if (cullingCamera.frustumPool.Count > 0)
			{
				this.frustumPlanes = cullingCamera.frustumPool.Pop();
				this.frustumPlanes.AddRange(frustumPlanes);
			}
			else
			{
				this.frustumPlanes = new List<Plane>(frustumPlanes);
			}
			this.forwardTraversal = forwardTraversal;
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0001C1D0 File Offset: 0x0001A3D0
		public VisibilityNode(SECTR_CullingCamera cullingCamera, SECTR_Sector sector, SECTR_Portal portal, List<Plane> frustumPlanes, bool forwardTraversal)
		{
			this.sector = sector;
			this.portal = portal;
			if (frustumPlanes == null)
			{
				this.frustumPlanes = null;
			}
			else if (cullingCamera.frustumPool.Count > 0)
			{
				this.frustumPlanes = cullingCamera.frustumPool.Pop();
				this.frustumPlanes.AddRange(frustumPlanes);
			}
			else
			{
				this.frustumPlanes = new List<Plane>(frustumPlanes);
			}
			this.forwardTraversal = forwardTraversal;
		}

		// Token: 0x04000456 RID: 1110
		public SECTR_Sector sector;

		// Token: 0x04000457 RID: 1111
		public SECTR_Portal portal;

		// Token: 0x04000458 RID: 1112
		public List<Plane> frustumPlanes;

		// Token: 0x04000459 RID: 1113
		public bool forwardTraversal;
	}

	// Token: 0x020000BC RID: 188
	private struct ClipVertex
	{
		// Token: 0x0600045B RID: 1115 RVA: 0x0001C23B File Offset: 0x0001A43B
		public ClipVertex(Vector4 vertex, float side)
		{
			this.vertex = vertex;
			this.side = side;
		}

		// Token: 0x0400045A RID: 1114
		public Vector4 vertex;

		// Token: 0x0400045B RID: 1115
		public float side;
	}

	// Token: 0x020000BD RID: 189
	private struct ThreadCullData
	{
		// Token: 0x0600045C RID: 1116 RVA: 0x0001C24C File Offset: 0x0001A44C
		public ThreadCullData(SECTR_Sector sector, SECTR_CullingCamera cullingCamera, Vector3 cameraPos, List<Plane> cullingPlanes, List<List<Plane>> occluderFrustums, int baseMask, float shadowDistance, bool cullingSimpleCulling)
		{
			this.sector = sector;
			this.cameraPos = cameraPos;
			this.baseMask = baseMask;
			this.shadowDistance = shadowDistance;
			this.cullingSimpleCulling = cullingSimpleCulling;
			this.sectorShadowLights = null;
			Stack<List<List<Plane>>> threadOccluderPool = cullingCamera.threadOccluderPool;
			lock (threadOccluderPool)
			{
				this.occluderFrustums = ((cullingCamera.threadOccluderPool.Count > 0) ? cullingCamera.threadOccluderPool.Pop() : new List<List<Plane>>(occluderFrustums.Count));
			}
			Stack<List<Plane>> threadFrustumPool = cullingCamera.threadFrustumPool;
			lock (threadFrustumPool)
			{
				if (cullingCamera.threadFrustumPool.Count > 0)
				{
					this.cullingPlanes = cullingCamera.threadFrustumPool.Pop();
					this.cullingPlanes.AddRange(cullingPlanes);
				}
				else
				{
					this.cullingPlanes = new List<Plane>(cullingPlanes);
				}
				int count = occluderFrustums.Count;
				for (int i = 0; i < count; i++)
				{
					List<Plane> list;
					if (cullingCamera.threadFrustumPool.Count > 0)
					{
						list = cullingCamera.threadFrustumPool.Pop();
						list.AddRange(occluderFrustums[i]);
					}
					else
					{
						list = new List<Plane>(occluderFrustums[i]);
					}
					this.occluderFrustums.Add(list);
				}
			}
			this.cullingMode = SECTR_CullingCamera.ThreadCullData.CullingModes.Graph;
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x0001C3B0 File Offset: 0x0001A5B0
		public ThreadCullData(SECTR_Sector sector, Vector3 cameraPos, List<SECTR_Member.Child> sectorShadowLights)
		{
			this.sector = sector;
			this.cameraPos = cameraPos;
			this.cullingPlanes = null;
			this.occluderFrustums = null;
			this.baseMask = 0;
			this.shadowDistance = 0f;
			this.cullingSimpleCulling = false;
			this.sectorShadowLights = sectorShadowLights;
			this.cullingMode = SECTR_CullingCamera.ThreadCullData.CullingModes.Shadow;
		}

		// Token: 0x0400045C RID: 1116
		public SECTR_Sector sector;

		// Token: 0x0400045D RID: 1117
		public Vector3 cameraPos;

		// Token: 0x0400045E RID: 1118
		public List<Plane> cullingPlanes;

		// Token: 0x0400045F RID: 1119
		public List<List<Plane>> occluderFrustums;

		// Token: 0x04000460 RID: 1120
		public int baseMask;

		// Token: 0x04000461 RID: 1121
		public float shadowDistance;

		// Token: 0x04000462 RID: 1122
		public bool cullingSimpleCulling;

		// Token: 0x04000463 RID: 1123
		public List<SECTR_Member.Child> sectorShadowLights;

		// Token: 0x04000464 RID: 1124
		public SECTR_CullingCamera.ThreadCullData.CullingModes cullingMode;

		// Token: 0x020000BE RID: 190
		public enum CullingModes
		{
			// Token: 0x04000466 RID: 1126
			None,
			// Token: 0x04000467 RID: 1127
			Graph,
			// Token: 0x04000468 RID: 1128
			Shadow
		}
	}
}
