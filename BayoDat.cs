using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatRepacker
{
    public class DatHeader
    {
        public sbyte[] id { get; private set; }
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
        public DatHeader(sbyte[] id, uint fileNumber, uint fileOffsetsOffset, uint fileExtensionOffset, uint fileNamesOffset, uint fileSizesOffset, uint hashMapOffset)
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
        public DatHeader(sbyte[] id, uint fileNumber, uint fileOffsetsOffset, uint fileExtensionOffset, uint fileNamesOffset, uint fileSizesOffset)
        {
            this.id = id;
            this.fileNumber = fileNumber;
            this.fileOffsetsOffset = fileOffsetsOffset;
            this.fileExtensionOffset = fileExtensionOffset;
            this.fileNamesOffset = fileNamesOffset;
            this.fileSizesOffset = fileSizesOffset;
            hashMapOffset = 0;
        }

        public void SetId(sbyte[] id) { this.id = id; }

        public void SetFileNumber(uint fileNumber) { this.fileNumber = fileNumber; }

        public void SetFileOffsetOffsets(uint offset) { this.fileOffsetsOffset = offset; }

        public void SetFileExtensionOffsets(uint offset) { this.fileExtensionOffset = offset; }

        public void SetFileNamesOffset(uint offset) { this.fileNamesOffset = offset; }

        public void SetFileSizesOffset(uint offset) { this.fileSizesOffset = offset; }

        public void SetHashMapOffset(uint offset) { this.hashMapOffset = offset; }
    }

    public class BayoDat
    {
        private DatHeader header;
        private List<uint> fileOffsets;
        private List<sbyte[]> fileExtensions;
        private uint nameLength;
        private List<List<sbyte>> fileNames;
        private List<uint> fileSizes;
        private List<byte[]> files;

        public BayoDat(DatHeader header, uint nameLength)
        {
            this.header = header;
            fileOffsets = new List<uint>();
            fileOffsets.Capacity = (int)header.fileNumber;
            fileExtensions = new List<sbyte[]>();
            fileExtensions.Capacity = (int)header.fileNumber;
            this.nameLength = nameLength;
            fileNames = new List<List<sbyte>>();
            fileNames.Capacity = (int)header.fileNumber;
            fileSizes = new List<uint>();
            fileSizes.Capacity = (int)header.fileNumber;
            files = new List<byte[]>();
            files.Capacity = (int)header.fileNumber;
        }

        public BayoDat(sbyte[] id, uint fileNumber, uint fileOffsetsOffset, uint fileExtensionOffset, uint fileNamesOffset, uint fileSizesOffset, uint hashMapOffset, uint nameLength)
        {
            header = new DatHeader(id, fileNumber, fileOffsetsOffset, fileExtensionOffset, fileNamesOffset, fileSizesOffset, hashMapOffset);
            fileOffsets = new List<uint>();
            fileOffsets.Capacity = (int)header.fileNumber;
            fileExtensions = new List<sbyte[]>();
            fileExtensions.Capacity = (int)header.fileNumber;
            this.nameLength = nameLength;
            fileNames = new List<List<sbyte>>();
            fileNames.Capacity = (int)header.fileNumber;
            fileSizes = new List<uint>();
            fileSizes.Capacity = (int)header.fileNumber;
            files = new List<byte[]>();
            files.Capacity = (int)header.fileNumber;
        }

        /// <summary>
        /// Add the file's information inside the lists of the Dat
        /// </summary>
        /// <param name="fileName">Assumed to be a proper name ending in an extension</param>
        /// <param name="fileData"></param>
        public void AddFile(string fileName, byte[] fileData)
        {
            // Add file name
            byte[] nameBytes = Encoding.UTF8.GetBytes(fileName);
            AddFile(nameBytes, fileData);
        }

        /// <summary>
        /// Add the file's information inside the lists of the Dat
        /// </summary>
        /// <param name="fileName">Assumed to be a proper name ending in an extension</param>
        /// <param name="fileData"></param>
        public void AddFile(byte[] fileName, byte[] fileData)
        {
            // Add file name
            List<sbyte> nameSBytes = new List<sbyte>(Array.ConvertAll(fileName, b => unchecked((sbyte)b)));
            nameSBytes.Capacity = (int)nameLength;
            fileNames.Add(nameSBytes);
            // Add file extension
            sbyte[] extBytes =
            [
                (sbyte)fileName[fileName.Length - 3],
                (sbyte)fileName[fileName.Length - 2],
                (sbyte)fileName[fileName.Length - 1],
                0,
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
        }

        /// <summary>
        /// Add the file's information inside the lists of the Dat
        /// </summary>
        /// <param name="fileName">Assumed to be a proper name ending in an extension</param>
        /// <param name="fileData"></param>
        public void AddFile(sbyte[] fileName, byte[] fileData)
        {
            List<sbyte> nameSBytes = new List<sbyte>(fileName);
            nameSBytes.Capacity = (int)nameLength;
            fileNames.Add(nameSBytes);
            // Add file extension
            sbyte[] extBytes =
            [
                (sbyte)fileName[fileName.Length - 3],
                (sbyte)fileName[fileName.Length - 2],
                (sbyte)fileName[fileName.Length - 1],
                0,
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
        public (List<List<sbyte>> names, List<uint> sizes) GetFilesInfo()
        {
            return (fileNames, fileSizes);
        }

        /// <summary>
        /// Get a tuple of lists containing the names, sizes and data of the files
        /// </summary>
        /// <returns></returns>
        public (List<List<sbyte>> names, List<uint> sizes, List<byte[]> data) GetFiles()
        {
            return (fileNames, fileSizes, files);
        }

    }


}
