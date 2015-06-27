﻿using System;
using ClassicalSharp;

namespace DefaultPlugin {
	
	public partial class ClassicBlockInfo : BlockInfo {
		
		bool[] hidden = new bool[blocksCount * blocksCount * 6];

		void SetupCullingCache() {
			for( byte tile = 1; tile < blocksCount; tile++ ) {
				for( byte neighbour = 1; neighbour < blocksCount; neighbour++ ) {
					bool hidden = IsHidden( tile, neighbour );
					if( hidden ) {
						SetHidden( tile, neighbour, TileSide.Left, true );
						SetHidden( tile, neighbour, TileSide.Right, true );
						SetHidden( tile, neighbour, TileSide.Front, true );
						SetHidden( tile, neighbour, TileSide.Back, true );
						SetHidden( tile, neighbour, TileSide.Top, BlockHeight( tile ) == 1 );
						SetHidden( tile, neighbour, TileSide.Bottom, BlockHeight( neighbour ) == 1 );
					}
				}
			}
		}
		
		bool IsHidden( byte tile, byte block ) {
			return 
				( ( tile == block || ( IsOpaque( block ) && !IsLiquid( block ) ) ) && !IsSprite( tile ) ) || 
				( IsLiquid( tile ) && block == (byte)Block.Ice );
		}
		
		void SetHidden( byte tile, byte block, int tileSide, bool value ) {
			hidden[( tile * blocksCount + block ) * 6 + tileSide] = value;
		}
		
		public override bool IsFaceHidden( byte tile, byte block, int tileSide ) {
			return hidden[( tile * blocksCount + block ) * 6 + tileSide];
		}
	}
}