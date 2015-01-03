﻿using OpenTK;
using System;
using System.Drawing;
using ClassicalSharp.GraphicsAPI;
using ClassicalSharp.Renderers;

namespace ClassicalSharp.Model {

	public class ZombieModel : IModel {
		
		ModelSet Set;
		public ZombieModel( Game window ) : base( window ) {
			vertices = new VertexPos3fTex2fCol4b[6 * 6];
			Set = new ModelSet();
			Set.Head = MakeHead();
			Set.Torso = MakeTorso();
			Set.LeftLeg = MakeLeftLeg( 0.25f, 0f );
			Set.RightLeg = MakeRightLeg( 0, 0.25f );
			Set.LeftArm = MakeLeftArm( 0.5f, 0.25f );
			Set.RightArm = MakeRightArm( 0.25f, 0.5f );
			vertices = null;

			DefaultSkinTextureId = graphics.LoadTexture( "zombie.png" );
		}
		
		ModelPart MakeLeftArm( float x1, float x2 ) {
			return MakePart( 40, 16, 4, 12, 4, 4, 4, 12, -x2, -x1, 0.75f, 1.5f, -0.125f, 0.125f, false );
		}
		
		ModelPart MakeRightArm( float x1, float x2 ) {
			return MakePart( 40, 16, 4, 12, 4, 4, 4, 12, x1, x2, 0.75f, 1.5f, -0.125f, 0.125f, false );
		}
		
		ModelPart MakeHead() {
			return MakePart( 0, 0, 8, 8, 8, 8, 8, 8, -0.25f, 0.25f, 1.5f, 2f, -0.25f, 0.25f, false );
		}
		
		ModelPart MakeTorso() {
			return MakePart( 16, 16, 4, 12, 8, 4, 8, 12, -0.25f, 0.25f, 0.75f, 1.5f, -0.125f, 0.125f, false );
		}
		
		ModelPart MakeLeftLeg( float x1, float x2 ) {
			return MakePart( 0, 16, 4, 12, 4, 4, 4, 12, -x2, -x1, 0f, 0.75f, -0.125f, 0.125f, false );
		}
		
		ModelPart MakeRightLeg( float x1, float x2 ) {
			return MakePart( 0, 16, 4, 12, 4, 4, 4, 12, x1, x2, 0f, 0.75f, -0.125f, 0.125f, false );
		}
		
		public override float NameYOffset {
			get { return 2.075f; }
		}
		
		Vector3 pos;
		float yaw, pitch;
		float rightLegXRot, rightArmZRot;
		float leftLegXRot, leftArmZRot;
		
		public override void RenderModel( Player player, PlayerRenderer renderer ) {
			pos = player.Position;
			yaw = player.YawDegrees;
			pitch = player.PitchDegrees;
			
			leftLegXRot = player.leftLegXRot * 180 / (float)Math.PI;
			leftArmZRot = player.leftArmZRot * 180 / (float)Math.PI;
			rightLegXRot = player.rightLegXRot * 180 / (float)Math.PI;
			rightArmZRot = player.rightArmZRot * 180 / (float)Math.PI;
			
			graphics.PushMatrix();
			graphics.Translate( pos.X, pos.Y, pos.Z );
			graphics.RotateY( -yaw );
			DrawPlayerModel( player, renderer );
			graphics.PopMatrix();
		}
		
		private void DrawPlayerModel( Player player, PlayerRenderer renderer ) {
			graphics.Texturing = true;
			int texId = DefaultSkinTextureId;
			graphics.Bind2DTexture( texId );
			
			DrawRotateX( 0, 1.5f, 0, -pitch, Set.Head );
			Set.Torso.Render();
			DrawRotateX( 0, 0.75f, 0, leftLegXRot, Set.LeftLeg );
			DrawRotateX( 0, 0.75f, 0, rightLegXRot, Set.RightLeg );
			DrawRotateXZ( 0, 1.375f, 0, 90f, leftArmZRot, Set.LeftArm );
			DrawRotateXZ( 0, 1.375f, 0, 90f, rightArmZRot, Set.RightArm );
			graphics.AlphaTest = true;
		}
		
		public override void Dispose() {
			Set.Dispose();
			graphics.DeleteTexture( DefaultSkinTextureId );
		}
		
		class ModelSet {
			
			public ModelPart Head, Torso, LeftLeg, RightLeg, LeftArm, RightArm;
			
			public void Dispose() {
				RightArm.Dispose();
				LeftArm.Dispose();
				RightLeg.Dispose();
				LeftLeg.Dispose();
				Torso.Dispose();
				Head.Dispose();
			}
		}
	}
}