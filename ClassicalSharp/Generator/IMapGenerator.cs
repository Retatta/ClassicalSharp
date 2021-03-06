﻿// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.Threading;
using ClassicalSharp.Singleplayer;

namespace ClassicalSharp.Generator {
	
	public abstract class IMapGenerator {
		
		public abstract string GeneratorName { get; }
		
		public abstract byte[] Generate( int width, int height, int length, int seed );
		
		public string CurrentState;
		
		public float CurrentProgress;
		
		public bool Done = false;
		
		public int Width, Height, Length;	
		
		public void GenerateAsync( Game game, int width, int height, int length, int seed ) {
			Width = width; Height = height; Length = length;
			Thread thread = new Thread(
				() => {
					SinglePlayerServer server = (SinglePlayerServer)game.Network;
					try {
						server.generatedMap = Generate( width, height, length, seed );
					} catch( Exception ex ) {
						ErrorHandler.LogError( "IMapGenerator.RunAsync", ex );
					}
					Done = true;
				}
			);
			
			thread.IsBackground = true;
			thread.Name = "IMapGenerator.RunAsync";
			thread.Start();
		}
	}
}
