﻿using System;
using System.Buffers.Binary;
using System.Text;
using System.IO;

namespace DatRepacker.Models;

/*
 * Original Author: Skyth
 * https://github.com/blueskythlikesclouds/BayoMotTool/blob/master/Source/BayoMotTool/EndianBinaryReader.cs
 * Thanks again for letting me use it!
 */

/// <summary>
/// Binary reader with built in endian support
/// </summary>
public class EndianBinaryReader : BinaryReader
{
    public EndianBinaryReader(Stream input) : base(input)
    {
    }

    public EndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
    {
    }

    public EndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
    {
    }

    public bool IsBigEndian { get; set; }

    public override short ReadInt16()
    {
        return IsBigEndian ? BinaryPrimitives.ReverseEndianness(base.ReadInt16()) : base.ReadInt16();
    }

    public override int ReadInt32()
    {
        return IsBigEndian ? BinaryPrimitives.ReverseEndianness(base.ReadInt32()) : base.ReadInt32();
    }

    public override float ReadSingle()
    {
        return IsBigEndian ? BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(base.ReadInt32())) : base.ReadSingle();
    }

    public override ushort ReadUInt16()
    {
        return IsBigEndian ? BinaryPrimitives.ReverseEndianness(base.ReadUInt16()) : base.ReadUInt16();
    }

    public override uint ReadUInt32()
    {
        return IsBigEndian ? BinaryPrimitives.ReverseEndianness(base.ReadUInt32()) : base.ReadUInt32();
    }
}
