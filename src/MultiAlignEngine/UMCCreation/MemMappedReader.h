#pragma once
#include "Windows.h"

namespace MultiAlignEngine
{
	namespace UMCCreation
	{
		class MemMappedReader
		{
		public:
			MemMappedReader();
			~MemMappedReader();
			bool Load(char *filename);
			bool Close();

			__int64 size();
			bool MoveToLine(char *startLine, __int64 offset, __int64 length);
			bool GetNextLine(char *buffer, int maxLength, char *stopLine, int stopLineLength); 
			bool SkipToAfterLine(char *startLine, char *buffer, int startLineLength, int maxLength, char skipCharacter = '\0'); 
			bool SkipToAfterLine(char *startLine, int startLineLength, char skipCharacter = '\0'); 
			__int64 FileLength() { return filebufferlength; } 
			inline __int64 CurrentPosition() { return currentOffset; }; 
			bool eof() { return currentOffset >= filebufferlength; }
			void FillBuffer(char *buffer, int index, int bufferLength);
		private:
			char *get_adjusted_ptr(__int64 offset);
			SYSTEM_INFO info; 

			// private implementation details

			HANDLE hMemMap;            //memory mapped object
			HANDLE hFile;              //handle to current file
			__int64 filebufferlength;   //size in bytes of the entire file
			char *filebuffer;          //base of the view of the file
			char *adjustedptr;         //an adjusted version of the filebuffer pointer
			__int64 mappedoffset;       //offset of the current view within the memmap object
			__int64 mappedlength;       //length of the view
			__int64 currentOffset; // current offset.
			int MEM_BLOCK_SIZE; 
		//	static const int MEM_BLOCK_SIZE = 4 * 1024 * 1024; 

			__int64 inline calc_index_base(__int64 index)
			{
				if(index < MEM_BLOCK_SIZE / 2)
				{
					return 0;
				}
				else
				{
					return ((index + MEM_BLOCK_SIZE / 4) & 
						(~(MEM_BLOCK_SIZE / 2 - 1))) - MEM_BLOCK_SIZE / 2;
				}
			}

		};
	}
}
