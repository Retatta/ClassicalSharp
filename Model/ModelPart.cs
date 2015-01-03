﻿using System;
using ClassicalSharp.GraphicsAPI;

namespace ClassicalSharp {
	
	public class ModelPart {
		
		public int VbId;
		public int Count;
		public IGraphicsApi Graphics;
		
		public ModelPart( VertexPos3fTex2fCol4b[] vertices, IGraphicsApi graphics ) {
			Count = vertices.Length;
			Graphics = graphics;
			VbId = Graphics.InitVb( vertices, DrawMode.Triangles, VertexFormat.VertexPos3fTex2fCol4b );
		}
		
		public void Render() {
			Graphics.DrawVbPos3fTex2fCol4b( DrawMode.Triangles, VbId, Count );
		}
		
		public void Dispose() {
			Graphics.DeleteVb( VbId );
		}
	}
	
	public enum SkinType {
		Type64x32,
		Type64x64,
		Type64x64Slim,
	}
}