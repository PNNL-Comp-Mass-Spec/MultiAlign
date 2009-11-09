#include ".\memmappedreader.h"
namespace MultiAlignEngine
{
	namespace UMCCreation
	{

		MemMappedReader::MemMappedReader(void)
		{
			filebuffer = 0; 
			hMemMap = 0;
			hFile   = 0;
			
			//clear both buffer pointers
			filebuffer   = 0;
			adjustedptr  = 0;

			mappedoffset = 0;
			mappedlength = 0;
			filebufferlength = 0;

			GetSystemInfo(&info);
			const int wanted_memory = 4 * 1024 * 1024; 
			MEM_BLOCK_SIZE = ((int)(wanted_memory / info.dwAllocationGranularity)) * info.dwAllocationGranularity; 

		}

		MemMappedReader::~MemMappedReader(void)
		{
		}

		bool MemMappedReader::Close()
		{
			if(filebuffer == 0)
				return false;

			//close the view of the file
			UnmapViewOfFile(filebuffer);

			//
			CloseHandle(hMemMap);
			CloseHandle(this->hFile);

			hMemMap = 0;
			hFile   = 0;
			
			//clear both buffer pointers
			filebuffer   = 0;
			adjustedptr  = 0;

			mappedoffset = 0;
			mappedlength = 0;
			filebufferlength = 0;

			return TRUE;
		}

		bool MemMappedReader::Load(char *filename)
		{
			HANDLE hTemp;

			//try to open the specified file for read-only access
			hTemp = CreateFile(filename, GENERIC_READ, FILE_SHARE_READ, 0,
					OPEN_EXISTING, 0, 0); 

			//if we failed to open the file, then exit now and keep the current file open
			if(hTemp == INVALID_HANDLE_VALUE)
				return false;
		    
			//now close the CURRENT file, and let the new file become the current one
			Close();
			hFile = hTemp;

			//create a file mapping which spans the entire file.
			//This works even for the 4Gb file sizes
			hMemMap = CreateFileMapping(hFile, 0, PAGE_READONLY, 0, 0, 0);
		    
			filebufferlength = GetFileSize(hFile, 0);
			currentOffset = 0;

			mappedlength = min(filebufferlength, MEM_BLOCK_SIZE);
			filebuffer = (char *)MapViewOfFile(hMemMap, FILE_MAP_READ, 0, 0, (SIZE_T) mappedlength);

			mappedoffset = 0;
			adjustedptr = filebuffer - mappedoffset;    

			return TRUE;
		}

		/*
			Fills the buffer provided with data starting at the index and going to length bytes
		*/
		void MemMappedReader::FillBuffer(char * buffer, int index, int bufferLength)
		{
			if (index < 0 || index >= filebufferlength || index + bufferLength >= filebufferlength )
				throw "Cannot access data at the specified index.  Index out of mapped file range.";
			if (bufferLength < 0)
				throw "Buffer length cannot be negative";

			memcpy(buffer, &(filebuffer[index]), sizeof(char)*bufferLength); 
		}

		char *MemMappedReader::get_adjusted_ptr(__int64 offset)
		{
			if (offset % info.dwAllocationGranularity != 0)
				throw "Specified offset is not appropriate. Its needs to be a multiple of allocation granularity"; 

			if (offset >= filebufferlength)
			{
				return NULL; 
			}

			//otherwise, map in the new area
			if(filebuffer)
				UnmapViewOfFile(filebuffer);

			mappedlength = min(filebufferlength - offset, MEM_BLOCK_SIZE);
			filebuffer = (char *)MapViewOfFile(hMemMap, FILE_MAP_READ, 0, (DWORD) offset, (SIZE_T)mappedlength);

			mappedoffset = offset;
			adjustedptr = filebuffer - mappedoffset;    

			return adjustedptr;
		}
		bool MemMappedReader::GetNextLine(char *buffer, int maxLength, char *stopLine, int stopLineLength)
		{
			int numCopied = 0; 
			buffer[0] = '\0'; 

			while(currentOffset < mappedoffset + mappedlength && filebuffer[currentOffset-mappedoffset] != '\n' && numCopied < maxLength)
			{
				buffer[numCopied++] = filebuffer[currentOffset-mappedoffset]; 
				currentOffset++; 
			}
			if (currentOffset >= mappedoffset + mappedlength)
			{
				char *source = get_adjusted_ptr(mappedoffset+mappedlength); 
				if (source == NULL)
				{
					if(strncmp(buffer, stopLine, stopLineLength) == 0)
					{
						buffer[0] = '\0';
						return false; 
					}
					else
					{
						return true; 
					}
				} 
				while(currentOffset != mappedoffset + mappedlength && filebuffer[currentOffset-mappedoffset] != '\n' && numCopied < maxLength)
				{
					buffer[numCopied++] = filebuffer[currentOffset-mappedoffset]; 
					currentOffset++; 
				}
			}
			currentOffset++; 

			if(strncmp(buffer, stopLine, stopLineLength) == 0)
			{
				buffer[0] = '\0';
				return false; 
			}
			else
			{
				if (numCopied < maxLength)
					buffer[numCopied] = '\0'; 
				else
					buffer[maxLength-1] = '\0'; 
				return true; 
			}
		}
		bool MemMappedReader::SkipToAfterLine(char *startLine, char *bufferLine, int startLineLength, int maxLength, 
			char skipCharacter)
		{
			int numCopied = 0; 

			while(currentOffset < filebufferlength)
			{
				numCopied = 0;
				while(currentOffset < mappedoffset + mappedlength && filebuffer[currentOffset-mappedoffset] != '\n' && numCopied < maxLength)
				{
					if (filebuffer[currentOffset-mappedoffset] != skipCharacter)
						bufferLine[numCopied++] = filebuffer[currentOffset-mappedoffset]; 
					currentOffset++; 
				}
				if (currentOffset >= mappedoffset + mappedlength)
				{
					char *source = get_adjusted_ptr(mappedoffset + mappedlength); 
					if (source == NULL)
					{
						return false; 
					} 
					while(currentOffset != mappedoffset + mappedlength && filebuffer[currentOffset-mappedoffset] != '\n' && numCopied < maxLength)
					{
						if (filebuffer[currentOffset-mappedoffset] != skipCharacter)
							bufferLine[numCopied++] = filebuffer[currentOffset-mappedoffset]; 
						currentOffset++; 
					}
				}
				currentOffset++; 

				if(strncmp(bufferLine, startLine, startLineLength) == 0)
				{
					if (numCopied < maxLength)
						bufferLine[numCopied] = '\0'; 
					else
						bufferLine[maxLength-1] = '\0'; 
					return true; 
				}
			}
			return false; 
		}

		bool MemMappedReader::SkipToAfterLine(char *startLine, int startLineLength, char skipCharacter)
		{
			int numCopied = 0; 
			const int maxLength = 1024;
			char buffer[maxLength]; 

			while(currentOffset < filebufferlength)
			{
				numCopied = 0; 
				while(currentOffset < mappedoffset + mappedlength && filebuffer[currentOffset-mappedoffset] != '\n' && numCopied < maxLength)
				{
					if (filebuffer[currentOffset-mappedoffset] != skipCharacter)
						buffer[numCopied++] = filebuffer[currentOffset-mappedoffset]; 
					currentOffset++; 
				}
				if (currentOffset >= mappedoffset + mappedlength)
				{
					char *source = get_adjusted_ptr(mappedoffset+mappedlength); 
					if (source == NULL)
					{
						return false; 
					} 
					while(currentOffset != mappedoffset + mappedlength && filebuffer[currentOffset-mappedoffset] != '\n' && numCopied < maxLength)
					{
						if (filebuffer[currentOffset-mappedoffset] != skipCharacter)
							buffer[numCopied++] = filebuffer[currentOffset-mappedoffset]; 
						currentOffset++; 
					}
				}
				currentOffset++; 

				if(strncmp(buffer, startLine, startLineLength) == 0)
				{
					return true; 
				}
			}
			return false; 
		}
	}
}