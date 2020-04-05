namespace Assets.ProceduralLevelGenerator.Editor
{
	using System.Collections.Generic;
	using NodeBasedEditor;
	using UnityEditor;
	using UnityEngine;

	public class NodeBasedEditorWindow : EditorWindow
	{
		private readonly List<Node> nodes = new List<Node>();

		public void OnGUI()
		{
			DrawNodes();
			ProcessEvents();
		}

		private void DrawNodes()
        {
            if (nodes == null) return;
            foreach (var node in nodes)
            {
                node.Draw();
            }
        }

		private void ProcessEvents()
		{
		}
	}
}