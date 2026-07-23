using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000096 RID: 150
public static class SECTR_Graph
{
	// Token: 0x06000326 RID: 806 RVA: 0x000142CC File Offset: 0x000124CC
	public static void DepthWalk(ref List<SECTR_Graph.Node> nodes, SECTR_Sector root, SECTR_Portal.PortalFlags stopFlags, int maxDepth)
	{
		nodes.Clear();
		if (root == null)
		{
			return;
		}
		if (maxDepth == 0)
		{
			SECTR_Graph.Node node = new SECTR_Graph.Node();
			node.Sector = root;
			nodes.Add(node);
			return;
		}
		int count = SECTR_Sector.All.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector.All[i].Visited = false;
		}
		Stack<SECTR_Graph.Node> stack = new Stack<SECTR_Graph.Node>(count);
		stack.Push(new SECTR_Graph.Node
		{
			Sector = root,
			Depth = 1
		});
		root.Visited = true;
		int num = 0;
		while (stack.Count > 0)
		{
			SECTR_Graph.Node node2 = stack.Pop();
			nodes.Add(node2);
			num++;
			if (maxDepth < 0 || node2.Depth <= maxDepth)
			{
				int count2 = node2.Sector.Portals.Count;
				for (int j = 0; j < count2; j++)
				{
					SECTR_Portal sectr_Portal = node2.Sector.Portals[j];
					if (sectr_Portal && (sectr_Portal.Flags & stopFlags) == (SECTR_Portal.PortalFlags)0)
					{
						SECTR_Sector sectr_Sector = (sectr_Portal.FrontSector == node2.Sector) ? sectr_Portal.BackSector : sectr_Portal.FrontSector;
						if (sectr_Sector && !sectr_Sector.Visited)
						{
							stack.Push(new SECTR_Graph.Node
							{
								Parent = node2,
								Sector = sectr_Sector,
								Portal = sectr_Portal,
								Depth = node2.Depth + 1
							});
							sectr_Sector.Visited = true;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00014468 File Offset: 0x00012668
	public static void BreadthWalk(ref List<SECTR_Graph.Node> nodes, SECTR_Sector root, SECTR_Portal.PortalFlags stopFlags, int maxDepth)
	{
		nodes.Clear();
		if (root == null)
		{
			return;
		}
		if (maxDepth == 0)
		{
			SECTR_Graph.Node node = new SECTR_Graph.Node();
			node.Sector = root;
			nodes.Add(node);
			return;
		}
		int count = SECTR_Sector.All.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector.All[i].Visited = false;
		}
		Queue<SECTR_Graph.Node> queue = new Queue<SECTR_Graph.Node>(count);
		queue.Enqueue(new SECTR_Graph.Node
		{
			Sector = root,
			Depth = 0
		});
		root.Visited = true;
		int num = 0;
		while (queue.Count > 0)
		{
			SECTR_Graph.Node node2 = queue.Dequeue();
			nodes.Add(node2);
			num++;
			if (maxDepth < 0 || node2.Depth < maxDepth)
			{
				int count2 = node2.Sector.Portals.Count;
				for (int j = 0; j < count2; j++)
				{
					SECTR_Portal sectr_Portal = node2.Sector.Portals[j];
					if (sectr_Portal && (sectr_Portal.Flags & stopFlags) == (SECTR_Portal.PortalFlags)0)
					{
						SECTR_Sector sectr_Sector = (sectr_Portal.FrontSector == node2.Sector) ? sectr_Portal.BackSector : sectr_Portal.FrontSector;
						if (sectr_Sector && !sectr_Sector.Visited)
						{
							queue.Enqueue(new SECTR_Graph.Node
							{
								Parent = node2,
								Sector = sectr_Sector,
								Portal = sectr_Portal,
								Depth = node2.Depth + 1
							});
							node2.Sector.Visited = true;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0001460C File Offset: 0x0001280C
	public static void FindShortestPath(ref List<SECTR_Graph.Node> path, Vector3 start, Vector3 goal, SECTR_Portal.PortalFlags stopFlags)
	{
		path.Clear();
		SECTR_Graph.openSet.Clear();
		SECTR_Graph.closedSet.Clear();
		SECTR_Sector.GetContaining(ref SECTR_Graph.initialSectors, start);
		SECTR_Sector.GetContaining(ref SECTR_Graph.goalSectors, goal);
		int count = SECTR_Graph.initialSectors.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_Sector sectr_Sector = SECTR_Graph.initialSectors[i];
			if (SECTR_Graph.goalSectors.Contains(sectr_Sector))
			{
				SECTR_Graph.Node node = new SECTR_Graph.Node();
				node.Sector = sectr_Sector;
				path.Add(node);
				return;
			}
			int count2 = sectr_Sector.Portals.Count;
			for (int j = 0; j < count2; j++)
			{
				SECTR_Portal sectr_Portal = sectr_Sector.Portals[j];
				if ((sectr_Portal.Flags & stopFlags) == (SECTR_Portal.PortalFlags)0)
				{
					SECTR_Graph.Node node2 = new SECTR_Graph.Node();
					node2.Portal = sectr_Portal;
					node2.Sector = sectr_Sector;
					node2.ForwardTraversal = (sectr_Sector == sectr_Portal.FrontSector);
					node2.Cost = Vector3.SqrMagnitude(start - sectr_Portal.transform.position);
					float num = Vector3.SqrMagnitude(goal - sectr_Portal.transform.position);
					node2.CostPlusEstimate = node2.Cost + num;
					SECTR_Graph.openSet.Enqueue(node2);
				}
			}
		}
		while (SECTR_Graph.openSet.Count > 0)
		{
			SECTR_Graph.Node node3 = SECTR_Graph.openSet.Dequeue();
			SECTR_Sector sectr_Sector2 = node3.ForwardTraversal ? node3.Portal.BackSector : node3.Portal.FrontSector;
			if (sectr_Sector2)
			{
				if (SECTR_Graph.goalSectors.Contains(sectr_Sector2))
				{
					SECTR_Graph.Node.ReconstructPath(path, node3);
					return;
				}
				int count3 = sectr_Sector2.Portals.Count;
				for (int k = 0; k < count3; k++)
				{
					SECTR_Portal sectr_Portal2 = sectr_Sector2.Portals[k];
					if (sectr_Portal2 != node3.Portal && (sectr_Portal2.Flags & stopFlags) == (SECTR_Portal.PortalFlags)0)
					{
						SECTR_Graph.Node node4 = new SECTR_Graph.Node();
						node4.Parent = node3;
						node4.Portal = sectr_Portal2;
						node4.Sector = sectr_Sector2;
						node4.ForwardTraversal = (sectr_Sector2 == sectr_Portal2.FrontSector);
						node4.Cost = node3.Cost + Vector3.SqrMagnitude(node4.Portal.transform.position - node3.Portal.transform.position);
						float num2 = Vector3.SqrMagnitude(goal - node4.Portal.transform.position);
						node4.CostPlusEstimate = node4.Cost + num2;
						SECTR_Graph.Node node5 = null;
						SECTR_Graph.closedSet.TryGetValue(node4.Portal, out node5);
						if (node5 == null || node5.CostPlusEstimate >= node4.CostPlusEstimate)
						{
							SECTR_Graph.Node node6 = null;
							for (int l = 0; l < SECTR_Graph.openSet.Count; l++)
							{
								if (SECTR_Graph.openSet[l].Portal == node4.Portal)
								{
									node6 = SECTR_Graph.openSet[l];
									break;
								}
							}
							if (node6 == null || node6.CostPlusEstimate >= node4.CostPlusEstimate)
							{
								SECTR_Graph.openSet.Enqueue(node4);
							}
						}
					}
				}
				if (!SECTR_Graph.closedSet.ContainsKey(node3.Portal))
				{
					SECTR_Graph.closedSet.Add(node3.Portal, node3);
				}
			}
		}
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00014978 File Offset: 0x00012B78
	public static string GetGraphAsDot(string graphName)
	{
		string text = "graph " + graphName;
		text += " {\n";
		text += "\tlayout=neato\n";
		foreach (SECTR_Portal sectr_Portal in SECTR_Portal.All)
		{
			text += "\t";
			text += sectr_Portal.GetInstanceID();
			text += " [";
			text = text + "label=" + sectr_Portal.name;
			text += ",shape=hexagon";
			text += "];\n";
		}
		foreach (SECTR_Sector sectr_Sector in SECTR_Sector.All)
		{
			text += "\t";
			text += sectr_Sector.GetInstanceID();
			text += " [";
			text = text + "label=" + sectr_Sector.name;
			text += ",shape=box";
			text += "];\n";
		}
		foreach (SECTR_Portal sectr_Portal2 in SECTR_Portal.All)
		{
			if (sectr_Portal2.FrontSector)
			{
				text += "\t";
				text = string.Concat(new object[]
				{
					text,
					sectr_Portal2.GetInstanceID(),
					" -- ",
					sectr_Portal2.FrontSector.GetInstanceID()
				});
				text += ";\n";
			}
			if (sectr_Portal2.BackSector)
			{
				text += "\t";
				text = string.Concat(new object[]
				{
					text,
					sectr_Portal2.GetInstanceID(),
					" -- ",
					sectr_Portal2.BackSector.GetInstanceID()
				});
				text += ";\n";
			}
		}
		text += "\n}";
		return text;
	}

	// Token: 0x04000341 RID: 833
	private static List<SECTR_Sector> initialSectors = new List<SECTR_Sector>(4);

	// Token: 0x04000342 RID: 834
	private static List<SECTR_Sector> goalSectors = new List<SECTR_Sector>(4);

	// Token: 0x04000343 RID: 835
	private static SECTR_PriorityQueue<SECTR_Graph.Node> openSet = new SECTR_PriorityQueue<SECTR_Graph.Node>(64);

	// Token: 0x04000344 RID: 836
	private static Dictionary<SECTR_Portal, SECTR_Graph.Node> closedSet = new Dictionary<SECTR_Portal, SECTR_Graph.Node>(64);

	// Token: 0x02000097 RID: 151
	public class Node : IComparable<SECTR_Graph.Node>
	{
		// Token: 0x0600032B RID: 811 RVA: 0x00014C0C File Offset: 0x00012E0C
		public int CompareTo(SECTR_Graph.Node other)
		{
			if (this.CostPlusEstimate > other.CostPlusEstimate)
			{
				return 1;
			}
			if (this.CostPlusEstimate < other.CostPlusEstimate)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00014C2F File Offset: 0x00012E2F
		public static void ReconstructPath(List<SECTR_Graph.Node> path, SECTR_Graph.Node currentNode)
		{
			if (currentNode != null)
			{
				path.Insert(0, currentNode);
				SECTR_Graph.Node.ReconstructPath(path, currentNode.Parent);
			}
		}

		// Token: 0x04000345 RID: 837
		public SECTR_Portal Portal;

		// Token: 0x04000346 RID: 838
		public SECTR_Sector Sector;

		// Token: 0x04000347 RID: 839
		public float CostPlusEstimate;

		// Token: 0x04000348 RID: 840
		public float Cost;

		// Token: 0x04000349 RID: 841
		public int Depth;

		// Token: 0x0400034A RID: 842
		public bool ForwardTraversal;

		// Token: 0x0400034B RID: 843
		public SECTR_Graph.Node Parent;
	}
}
