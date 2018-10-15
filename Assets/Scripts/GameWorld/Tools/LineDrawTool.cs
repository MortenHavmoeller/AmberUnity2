using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amber.GameWorld.Tools
{
	

	public class LineDrawTool : MonoBehaviour
	{
		private const int MAX_LINES = 2048;
		private const int DEFAULT_LINES = 256;

		private static List<DrawableLine> _linesToDraw = new List<DrawableLine>(DEFAULT_LINES);

		private static LineDrawTool _singleton;

		public static void EnsureExistance()
		{
			if (_singleton != null)
			{
				return;
			}

			GameObject go = new GameObject("line_draw_tool");
			_singleton = go.AddComponent<LineDrawTool>();
			go.hideFlags = HideFlags.DontSave;
		}

		public static void DrawLine(Vector3 a, Vector3 b, Color color)
		{
			if (_linesToDraw.Count + 1 <= MAX_LINES)
			{
				_linesToDraw.Add(new DrawableLine(a, b, color));
			}
		}

		public static void DrawBox(Vector3 center, Vector3 size, Color color)
		{
			
			if (_linesToDraw.Count + 12 <= MAX_LINES)
			{
				size *= 0.5f;

				// draw bottom
				Vector3 corner1 = center;
				corner1[0] = corner1[0] - size[0];
				corner1[1] = corner1[1] - size[1];
				corner1[2] = corner1[2] - size[2];

				Vector3 corner2 = center;
				corner2[0] = corner2[0] + size[0];
				corner2[1] = corner2[1] - size[1];
				corner2[2] = corner2[2] - size[2];

				Vector3 corner3 = center;
				corner3[0] = corner3[0] + size[0];
				corner3[1] = corner3[1] - size[1];
				corner3[2] = corner3[2] + size[2];

				Vector3 corner4 = center;
				corner4[0] = corner4[0] - size[0];
				corner4[1] = corner4[1] - size[1];
				corner4[2] = corner4[2] + size[2];

				_linesToDraw.Add(new DrawableLine(corner1, corner2, color));
				_linesToDraw.Add(new DrawableLine(corner2, corner3, color));
				_linesToDraw.Add(new DrawableLine(corner3, corner4, color));
				_linesToDraw.Add(new DrawableLine(corner4, corner1, color));

				// draw top
				Vector3 corner5 = center;
				corner5[0] = corner5[0] - size[0];
				corner5[1] = corner5[1] + size[1];
				corner5[2] = corner5[2] - size[2];

				Vector3 corner6 = center;
				corner6[0] = corner6[0] + size[0];
				corner6[1] = corner6[1] + size[1];
				corner6[2] = corner6[2] - size[2];

				Vector3 corner7 = center;
				corner7[0] = corner7[0] + size[0];
				corner7[1] = corner7[1] + size[1];
				corner7[2] = corner7[2] + size[2];

				Vector3 corner8 = center;
				corner8[0] = corner8[0] - size[0];
				corner8[1] = corner8[1] + size[1];
				corner8[2] = corner8[2] + size[2];

				_linesToDraw.Add(new DrawableLine(corner5, corner6, color));
				_linesToDraw.Add(new DrawableLine(corner6, corner7, color));
				_linesToDraw.Add(new DrawableLine(corner7, corner8, color));
				_linesToDraw.Add(new DrawableLine(corner8, corner5, color));

				// draw sides
				_linesToDraw.Add(new DrawableLine(corner1, corner5, color));
				_linesToDraw.Add(new DrawableLine(corner2, corner6, color));
				_linesToDraw.Add(new DrawableLine(corner3, corner7, color));
				_linesToDraw.Add(new DrawableLine(corner4, corner8, color));
			}
		}

		static Material lineMaterial;
		static void CreateLineMaterial()
		{
			if (!lineMaterial)
			{
				// Unity has a built-in shader that is useful for drawing
				// simple colored things.
				Shader shader = Shader.Find("Hidden/Internal-Colored");
				lineMaterial = new Material(shader);
				lineMaterial.hideFlags = HideFlags.HideAndDontSave;
				// Turn on alpha blending
				lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				// Turn backface culling off
				lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				// Turn off depth writes
				lineMaterial.SetInt("_ZWrite", 0);
			}
		}

		private void Awake()
		{
			if (_singleton != null)
			{
				Debug.LogError("more than one LineDrawTool present");
			}
			_singleton = this;
		}

		public void OnRenderObject()
		{
			CreateLineMaterial();

			lineMaterial.SetPass(0);

			GL.PushMatrix();

			//GL.MultMatrix(Matrix4x4.identity);
			GL.MultMatrix(transform.localToWorldMatrix);

			GL.Begin(GL.LINES);
			for (int i = 0; i < _linesToDraw.Count; i++)
			{
				DrawableLine line = _linesToDraw[i];

				GL.Color(line.color);

				GL.Vertex3(line.a[0], line.a[1], line.a[2]);
				GL.Vertex3(line.b[0], line.b[1], line.b[2]);
			}
			GL.End();
			GL.PopMatrix();

			_linesToDraw.Clear();
		}

		private class DrawableLine
		{
			public Vector3 a;
			public Vector3 b;
			public Color color;

			public DrawableLine(Vector3 a, Vector3 b)
			{
				this.a = a;
				this.b = b;
				color = Color.white;
			}

			public DrawableLine(Vector3 a, Vector3 b, Color color)
			{
				this.a = a;
				this.b = b;
				this.color = color;
			}
		}
	}
}

