using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatRepacker.Models
{
    enum Globals : UInt16
    {
        HEADER_SIZE = 28
    }

    public class DatHeader
    {
        public char[] id { get; private set; }
        public uint fileNumber { get; private set; }
        public uint fileOffsetsOffset { get; private set; } // The offset to where the file offsets start. An address that points to more addresses
        public uint fileExtensionOffset { get; private set; }
        public uint fileNamesOffset { get; private set; }
        public uint fileSizesOffset { get; private set; }
        public uint hashMapOffset { get; private set; } // I don't really know what this is for

        /// <summary>
        /// Header of a Dat file
        /// </summary>
        /// <param name="id">"DAT"</param>
        /// <param name="fileNumber">Number of files inside the Dat</param>
        /// <param name="fileOffsetsOffset">The pointer to where the file pointers are</param>
        /// <param name="fileNamesOffset">The pointer to where the file names are</param>
        /// <param name="fileSizesOffset">The pointer to where the file sizes are</param>
        /// <param name="hashMapOffset">Usually 0</param>
        public DatHeader(char[] id, uint fileNumber, uint fileOffsetsOffset, uint fileExtensionOffset, uint fileNamesOffset, uint fileSizesOffset, uint hashMapOffset)
        {
            this.id = id;
            this.fileNumber = fileNumber;
            this.fileOffsetsOffset = fileOffsetsOffset;
            this.fileExtensionOffset = fileExtensionOffset;
            this.fileNamesOffset = fileNamesOffset;
            this.fileSizesOffset = fileSizesOffset;
            this.hashMapOffset = hashMapOffset;
        }

        /// <summary>
        /// Header of a Dat file. Assumes hashMapOffset is 0
        /// </summary>
        /// <param name="id">"DAT"</param>
        /// <param name="fileNumber">Number of files inside the Dat</param>
        /// <param name="fileOffsetsOffset">The pointer to where the file pointers are</param>
        /// <param name="fileNamesOffset">The pointer to where the file names are</param>
        /// <param name="fileSizesOffset">The pointer to where the file sizes are</param>
        public DatHeader(char[] id, uint fileNumber, uint fileOffsetsOffset, uint fileExtensionOffset, uint fileNamesOffset, uint fileSizesOffset)
        {
            this.id = id;
            this.fileNumber = fileNumber;
            this.fileOffsetsOffset = fileOffsetsOffset;
            this.fileExtensionOffset = fileExtensionOffset;
            this.fileNamesOffset = fileNamesOffset;
            this.fileSizesOffset = fileSizesOffset;
            hashMapOffset = 0;
        }

        public void SetId(char[] id) { this.id = id; }

        public void SetFileNumber(uint fileNumber) { this.fileNumber = fileNumber; }

        public void SetFileOffsetOffsets(uint offset) { fileOffsetsOffset = offset; }

        public void SetFileExtensionOffsets(uint offset) { fileExtensionOffset = offset; }

        public void SetFileNamesOffset(uint offset) { fileNamesOffset = offset; }

        public void SetFileSizesOffset(uint offset) { fileSizesOffset = offset; }

        public void SetHashMapOffset(uint offset) { hashMapOffset = offset; }
    }

    public class BayoDat
    {
        private DatHeader header;
        private List<uint> fileOffsets;
        private List<char[]> fileExtensions;
        private uint nameLength;
        private List<List<char>> fileNames;
        private List<uint> fileSizes;
        private List<byte[]> files;
        public bool bigEndian { private set; get; }

        public BayoDat(DatHeader header, uint nameLength, bool bigEndian)
        {
            this.header = header;
            fileOffsets = new List<uint>
            {
                Capacity = (int)header.fileNumber
            };
            fileExtensions = new List<char[]>
            {
                Capacity = (int)header.fileNumber
            };
            this.nameLength = nameLength;
            fileNames = new List<List<char>>
            {
                Capacity = (int)header.fileNumber
            };
            fileSizes = new List<uint>
            {
                Capacity = (int)header.fileNumber
            };
            files = new List<byte[]>
            {
                Capacity = (int)header.fileNumber
            };
            this.bigEndian = bigEndian;
        }

        public BayoDat(char[] id, uint fileNumber, uint fileOffsetsOffset, uint fileExtensionOffset, uint fileNamesOffset, uint fileSizesOffset, uint hashMapOffset, uint nameLength, bool bigEndian)
        {
            header = new DatHeader(id, fileNumber, fileOffsetsOffset, fileExtensionOffset, fileNamesOffset, fileSizesOffset, hashMapOffset);
            fileOffsets = new List<uint>
            {
                Capacity = (int)header.fileNumber
            };
            fileExtensions = new List<char[]>
            {
                Capacity = (int)header.fileNumber
            };
            this.nameLength = nameLength;
            fileNames = new List<List<char>>
            {
                Capacity = (int)header.fileNumber
            };
            fileSizes = new List<uint>
            {
                Capacity = (int)header.fileNumber
            };
            files = new List<byte[]>
            {
                Capacity = (int)header.fileNumber
            };
            this.bigEndian = bigEndian;
        }

        /// <summary>
        /// Get the Dat file header
        /// </summary>
        /// <returns></returns>
        public DatHeader GetHeader() { return header; }

        /// <summary>
        /// Add the file's information inside the lists of the Dat
        /// </summary>
        /// <param name="fileName">Assumed to be a proper name ending in an extension</param>
        /// <param name="fileData"></param>
        public void AddFile(string fileName, byte[] fileData)
        {
            // Add file name
            char[] nameBytes = fileName.ToCharArray();
            AddFile(nameBytes, fileData);
        }

        /// <summary>
        /// Add the file's information inside the lists of the Dat
        /// </summary>
        /// <param name="fileName">Assumed to be a proper name ending in an extension</param>
        /// <param name="fileData"></param>
        public void AddFile(char[] fileName, byte[] fileData)
        {
            int i;
            // Add file name
            List<char> nameSBytes = new List<char>(fileName)
            {
                Capacity = (int)nameLength
            };
            fileNames.Add(nameSBytes);
            // Add file extension
            char[] extBytes =
            [
                fileName[fileName.Length - 3],
                fileName[fileName.Length - 2],
                fileName[fileName.Length - 1],
                Convert.ToChar(0),
            ];
            fileExtensions.Add(extBytes);
            // Add file size
            fileSizes.Add((uint)fileData.Length);
            // Add file data
            files.Add(fileData);
            // Adjust count and offsets
            header.SetFileNumber(header.fileNumber + 1);
            header.SetFileExtensionOffsets(header.fileExtensionOffset + 4);
            header.SetFileNamesOffset(header.fileNamesOffset + 8);
            header.SetFileSizesOffset(header.fileSizesOffset + 12);
            if (fileOffsets.Count > 0)
            {
                for ( i = 0; i < fileOffsets.Count; i++)
                {
                    fileOffsets[i] = fileOffsets[i] + 16;
                }
                fileOffsets.Add(fileOffsets[i-1] + fileSizes[i-1]);
            }
            else
            {
                fileOffsets.Add(header.fileSizesOffset + 4);
            }
        }

        /// <summary>
        /// Remove all the information of a file at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveFile(int index)
        {
            bool success = true;
            try
            {
                fileOffsets.RemoveAt(index);
                fileExtensions.RemoveAt(index);
                fileNames.RemoveAt(index);
                fileSizes.RemoveAt(index);
                files.RemoveAt(index);
                // Adjust count and offsets
                header.SetFileNumber(header.fileNumber - 1);
                header.SetFileExtensionOffsets(header.fileExtensionOffset - 4);
                header.SetFileNamesOffset(header.fileNamesOffset - 8);
                header.SetFileSizesOffset(header.fileSizesOffset - 12);
            }
            catch (ArgumentOutOfRangeException)
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Load prepared content data into the Dat. Overrides any present information
        /// </summary>
        /// <param name="fileOffsets"></param>
        /// <param name="fileExtensions"></param>
        /// <param name="fileNames"></param>
        /// <param name="fileSizes"></param>
        /// <param name="files"></param>
        public void LoadContents(List<uint> fileOffsets, List<char[]> fileExtensions, List<List<char>> fileNames, List<uint> fileSizes, List<byte[]> files, uint nameLength)
        {
            this.fileOffsets = fileOffsets;
            this.fileExtensions = fileExtensions;
            this.fileNames = fileNames;
            this.fileSizes = fileSizes;
            this.files = files;
            this.nameLength = nameLength;
        }

        /// <summary>
        /// Get the total amount of files stored
        /// </summary>
        /// <returns></returns>
        public int FileCount()
        {
            return files.Count;
        }

        /// <summary>
        /// Get a tuple of lists containing the names and sizes of the files
        /// </summary>
        /// <returns></returns>
        public (List<List<char>> names, List<uint> sizes) GetFilesInfo()
        {
            return (fileNames, fileSizes);
        }

        /// <summary>
        /// Get a tuple of lists containing the names, sizes and data of the files
        /// </summary>
        /// <returns></returns>
        public (List<List<char>> names, List<uint> sizes, List<byte[]> data) GetFiles()
        {
            return (fileNames, fileSizes, files);
        }

    }


}
