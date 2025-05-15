using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatRepacker
{
    public class DatHeader
    {
        public string id { get; private set; }
        public uint fileNumber { get; private set; }
        public uint fileOffsetsOffset { get; private set; } // The offset to where the file offsets start. An address that points to more addresses
        public uint fileNamesOffset { get; private set; }
        public uint fileSizesOffset { get; private set; }
        public uint hasMapOffset { get; private set; } // I don't really know what this is for

        /// <summary>
        /// Header of a Dat file
        /// </summary>
        /// <param name="id">"DAT"</param>
        /// <param name="fileNumber">Number of files inside the Dat</param>
        /// <param name="fileOffsetsOffset">The pointer to where the file pointers are</param>
        /// <param name="fileNamesOffset">The pointer to where the file names are</param>
        /// <param name="fileSizesOffset">The pointer to where the file sizes are</param>
        /// <param name="hasMapOffset">Usually 0</param>
        public DatHeader(string id, uint fileNumber, uint fileOffsetsOffset, uint fileNamesOffset, uint fileSizesOffset, uint hasMapOffset)
        {
            this.id = id;
            this.fileNumber = fileNumber;
            this.fileOffsetsOffset = fileOffsetsOffset;
            this.fileNamesOffset = fileNamesOffset;
            this.fileSizesOffset = fileSizesOffset;
            this.hasMapOffset = hasMapOffset;
        }

        /// <summary>
        /// Header of a Dat file. Assumes hasMapOffset is 0
        /// </summary>
        /// <param name="id">"DAT"</param>
        /// <param name="fileNumber">Number of files inside the Dat</param>
        /// <param name="fileOffsetsOffset">The pointer to where the file pointers are</param>
        /// <param name="fileNamesOffset">The pointer to where the file names are</param>
        /// <param name="fileSizesOffset">The pointer to where the file sizes are</param>
        public DatHeader(string id, uint fileNumber, uint fileOffsetsOffset, uint fileNamesOffset, uint fileSizesOffset)
        {
            this.id = id;
            this.fileNumber = fileNumber;
            this.fileOffsetsOffset = fileOffsetsOffset;
            this.fileNamesOffset = fileNamesOffset;
            this.fileSizesOffset = fileSizesOffset;
            hasMapOffset = 0;
        }

        public void SetId(string id) { this.id = id; }

        public void SetFileNumber(uint fileNumber) { this.fileNumber = fileNumber; }

        public void SetFileOffsetOffsets(uint offset) {  this.fileOffsetsOffset = offset; }

        public void SetFileNamesOffset(uint offset) {  this.fileNamesOffset = offset; }

        public void SetFileSizesOffset(uint offset) { this.fileSizesOffset = offset; }

        public void SetHasMapOffset(uint offset) { this.hasMapOffset = offset; }
    }

    public class BayoDat
    {
        private DatHeader? header;
    }


}
