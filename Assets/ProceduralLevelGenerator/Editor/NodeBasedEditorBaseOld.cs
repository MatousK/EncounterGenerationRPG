namespace Assets.ProceduralLevelGenerator.Editor
{
	using System.Collections.Generic;
	using NodeBasedEditor;
	using UnityEditor;
	using UnityEngine;

	public class NodeBasedEditorBaseOld : EditorWindow
	{
		public Rect WindowRect = new Rect(20, 20, 120, 50);

		protected List<IEditorNodeBase> Nodes;
		protected List<ConnectionLegacy> Connections;

		protected GUIStyle NodeStyle;
		protected GUIStyle SelectedNodeStyle;
		protected readonly Dictionary<ConnectionPointType, GUIStyle> ConnectionStyles = new Dictionary<ConnectionPointType, GUIStyle>();

		protected Node SelectedToNode;
		protected Node SelectedFromNode;

		protected Vector2 Offset;
		protected Vector2 Drag;

		public NodeBasedEditorBaseOld()
		{
			minSize = new Vector2(500, 500);
		}

		public virtual void OnEnable()
		{
            NodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D
                },
                border = new RectOffset(12, 12, 12, 12),
                alignment = TextAnchor.MiddleCenter
            };
            NodeStyle.normal.textColor = Color.white;

            SelectedNodeStyle = new GUIStyle
            {
                normal =
                {
                    background =
                        EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D
                },
                border = new RectOffset(12, 12, 12, 12)
            };

            ConnectionStyles.Add(ConnectionPointType.Left, new GUIStyle
			{
				normal = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D},
				active = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D},
				border = new RectOffset(4, 4, 12, 12)
			});

			ConnectionStyles.Add(ConnectionPointType.Right, new GUIStyle
			{
				normal = { background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D },
				active = { background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D },
				border = new RectOffset(4, 4, 12, 12)
			});

			ConnectionStyles.Add(ConnectionPointType.Top, new GUIStyle
			{
				normal = { background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D},
				active = { background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D},
				border = new RectOffset(4, 4, 12, 12)
			});

			ConnectionStyles.Add(ConnectionPointType.Bottom, new GUIStyle
			{
				normal = { background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D },
				active = { background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D },
				border = new RectOffset(4, 4, 12, 12)
			});
		}

		public virtual void OnGUI()
		{
			DrawGrid(20, 0.2f, Color.gray);
			DrawGrid(100, 0.4f, Color.gray);

			DrawConnections();
			DrawNodes();

			// DrawConnectionLine(Event.current);


			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);

			if (GUI.changed) Repaint();
		}

		private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
		{
			int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
			int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

			Handles.BeginGUI();
			Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

			Offset += Drag * 0.5f;
			Vector3 newOffset = new Vector3(Offset.x % gridSpacing, Offset.y % gridSpacing, 0);

			for (int i = 0; i < widthDivs; i++)
			{
				Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
			}

			for (int j = 0; j < heightDivs; j++)
			{
				Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
			}

			Handles.color = Color.white;
			Handles.EndGUI();
		}

		private void DrawNodes()
        {
            if (Nodes == null) return;
            foreach (var node in Nodes)
            {
                node.Draw();
            }
        }

		private void DrawConnections()
        {
            if (Connections == null) return;
            foreach (var connection in Connections)
            {
                connection.Draw();
            }
        }

		protected virtual void ProcessEvents(Event e)
		{
			Drag = Vector2.zero;

			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 0)
					{
						// ClearConnectionSelection();
					}

					if (e.button == 1)
					{
						ProcessContextMenu(e.mousePosition);
					}
					break;

				case EventType.MouseDrag:
					if (e.button == 0)
					{
						OnDrag(e.delta);
					}
					break;
			}
		}

		private void ProcessNodeEvents(Event e)
		{
			if (Nodes != null)
			{
				for (int i = Nodes.Count - 1; i >= 0; i--)
				{
					bool guiChanged = Nodes[i].ProcessEvents(e);

					if (guiChanged)
					{
						GUI.changed = true;
					}
				}
			}
		}

		protected virtual void ProcessContextMenu(Vector2 mousePosition)
		{

		}

		private void OnDrag(Vector2 delta)
		{
			Drag = delta;

			if (Nodes != null)
            {
                foreach (var node in Nodes)
                {
                    node.Drag(delta);
                }
            }

			GUI.changed = true;
		}

		protected void OnClickNode(Node node)
		{
			if (node.IsSelected)
			{
				if (SelectedFromNode == null)
				{
					Debug.Log("select from");
					SelectedFromNode = node;
				}
				else
				{
					SelectedToNode = node;
					CreateConnection();
					ClearConnectionSelection();
				}
			}
			else if (SelectedFromNode == node)
			{
				Debug.Log("reset from");
				SelectedFromNode = null;
			}
		}

		protected void OnClickConnectionPoint()
		{
			//if (selectedToNode == null)
			//{
			//	selectedToNode = point;
			//}
			//else
			//{
			//	selectedFromNode = point;

			//	if (selectedFromNode.node != selectedToNode.node)
			//	{
			//		CreateConnection();
			//		ClearConnectionSelection();
			//	}
			//}
		}

		protected void OnClickRemoveNode()
		{
			//if (connections != null)
			//{
			//	List<Connection> connectionsToRemove = new List<Connection>();

			//	for (int i = 0; i < connections.Count; i++)
			//	{
			//		if (connections[i].inPoint == node.LeftConnectionPoint || connections[i].outPoint == node.RightConnectionPoint)
			//		{
			//			connectionsToRemove.Add(connections[i]);
			//		}
			//	}

			//	for (int i = 0; i < connectionsToRemove.Count; i++)
			//	{
			//		connections.Remove(connectionsToRemove[i]);
			//	}

			//	connectionsToRemove = null;
			//}

			//nodes.Remove(node);
		}

		protected void OnClickRemoveConnection(ConnectionLegacy connection)
		{
			Connections.Remove(connection);
		}

		private void CreateConnection()
		{
			if (Connections == null)
			{
				Connections = new List<ConnectionLegacy>();
			}

			Connections.Add(new ConnectionLegacy(SelectedToNode, SelectedFromNode, OnClickRemoveConnection));
		}

		private void ClearConnectionSelection()
		{
			SelectedFromNode?.SetSelected(false);
			SelectedToNode?.SetSelected(false);
			
			SelectedToNode = null;
			SelectedFromNode = null;
		}
	}
}