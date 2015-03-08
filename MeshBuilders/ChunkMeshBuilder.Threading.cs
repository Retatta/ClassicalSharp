using System;
using System.Threading;

namespace ClassicalSharp {

	public partial class ChunkMeshBuilder {
		
		EventWaitHandle handle = new AutoResetEvent( false );
		Thread worker;
		int[] chunks;
		RawChunkDrawInfo[] builtChunks;
		readonly object chunksLocker = new object();
		readonly object builtLocker = new object();
		int chunksX, chunksY, chunksZ;
		
		void InitWorkerThread() {
			worker = new Thread( WorkerThread );
			worker.Name = "ClassicalSharp.MeshBuilder()";
			worker.IsBackground = true;
			worker.Start();
		}
		
		public void MarkChunkForUpdate( int cx, int cy, int cz ) {
			lock( chunksLocker ) {
				chunks[cx + chunksX * ( cy + cz * chunksY )]++;
				handle.Set();
			}
		}
		
		public void OnNewMap() {
			lock( chunksLocker ) {
				if( chunks != null ) {
					for( int i = 0; i < chunks.Length; i++ ) {
						chunks[i] = 0;
					}
				}
			}
			// Hacky way of waiting until the worker thread has stopped building chunks.
			while( ( worker.ThreadState & ThreadState.WaitSleepJoin ) == 0 ) {
				Thread.Sleep( 1 );
			}
			
			if( builtChunks != null ) {
				for( int i = 0; i < builtChunks.Length; i++ ) {
					builtChunks[i] = null;
				}
			}
		}
		
		public RawChunkDrawInfo GetBuiltChunk( int cx, int cy, int cz ) {
			lock( builtLocker ) {
				int index = cx + chunksX * ( cy + cz * chunksY );
				RawChunkDrawInfo info = builtChunks[index];
				if( info != null ) {
					builtChunks[index] = null;
				}
				return info;
			}
		}
		
		public void OnNewMapLoaded( int chunksX, int chunksY, int chunksZ ) {
			// We don't need to lock here, as the worker thread is guaranteed to be sleeping
			if( ( worker.ThreadState & ThreadState.WaitSleepJoin ) == 0 ) {
				throw new InvalidOperationException( "Chunk building thread not sleeping!?" );
			}
			chunks = new int[chunksX * chunksY * chunksZ];
			this.chunksX = chunksX;
			this.chunksY = chunksY;
			this.chunksZ = chunksZ;
			for( int i = 0; i < chunks.Length; i++ ) {
				chunks[i] = 1;
			}
			builtChunks = new RawChunkDrawInfo[chunks.Length];
			map = Window.Map;
			width = map.Width;
			height = map.Height;
			length = map.Length;
			maxX = width - 1;
			maxY = height - 1;
			maxZ = length - 1;
			handle.Set();
		}
		
		
		int GetFirstModifiedChunk( out int state ) {
			state = 0;
			for( int i = 0; i < chunks.Length; i++ ) {
				state = chunks[i];
				if( state > 0 ) {
					return i;
				}
			}
			return -1;
		}
		void WorkerThread() {
			while( true ) {
				int index = -1, processedState = 0;
				lock( chunksLocker ) {
					if( chunks != null ) {
						index = GetFirstModifiedChunk( out processedState );
					}
				}
				if( index == -1 ) {
					handle.WaitOne();
				} else {
					int cx = index % chunksX;
					int cy = ( index / chunksX ) % chunksY;
					int cz = ( index / chunksX ) / chunksY;
					RawChunkDrawInfo info = GetDrawInfo( cx << 4, cy << 4, cz << 4 );
					lock( chunksLocker ) {
						int currentState = chunks[index];
						currentState -= processedState;
						if( currentState < 0 ) currentState = 0;
						chunks[index] = currentState;
					}
					lock( builtLocker ) {
						builtChunks[index] = info;
					}
				}
			}
		}
	}
}
