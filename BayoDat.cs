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
        public DatHeader(sbyte[] id, uint fileNumber, uint fileOffsetsOffset, uint fileNamesOffset, uint fileSizesOffset, uint hashMapOffset)
        {
            this.id = id;
            this.fileNumber = fileNumber;
            this.fileOffsetsOffset = fileOffsetsOffset;
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
        public DatHeader(sbyte[] id, uint fileNumber, uint fileOffsetsOffset, uint fileNamesOffset, uint fileSizesOffset)
        {
            this.id = id;
            this.fileNumber = fileNumber;
            this.fileOffsetsOffset = fileOffsetsOffset;
            this.fileNamesOffset = fileNamesOffset;
            this.fileSizesOffset = fileSizesOffset;
            hashMapOffset = 0;
        }

        public void SetId(sbyte[] id) { this.id = id; }

        public void SetFileNumber(uint fileNumber) { this.fileNumber = fileNumber; }

        public void SetFileOffsetOffsets(uint offset) {  this.fileOffsetsOffset = offset; }

        public void SetFileNamesOffset(uint offset) {  this.fileNamesOffset = offset; }

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
        private List<List<byte>> files;

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
            files = new List<List<byte>>();
        }

        public BayoDat(sbyte[] id, uint fileNumber, uint fileOffsetsOffset, uint fileNamesOffset, uint fileSizesOffset, uint hashMapOffset, uint nameLength)
        {
            header = new DatHeader(id, fileNumber, fileOffsetsOffset, fileNamesOffset, fileSizesOffset, hashMapOffset);
            fileOffsets = new List<uint>();
            fileOffsets.Capacity = (int)header.fileNumber;
            fileExtensions = new List<sbyte[]>();
            fileExtensions.Capacity = (int)header.fileNumber;
            this.nameLength = nameLength;
            fileNames = new List<List<sbyte>>();
            fileNames.Capacity = (int)header.fileNumber;
            fileSizes = new List<uint>();
            fileSizes.Capacity = (int)header.fileNumber;
            files = new List<List<byte>>();
        }
    }


}
