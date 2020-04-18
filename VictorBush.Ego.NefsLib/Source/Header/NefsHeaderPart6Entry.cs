// See LICENSE.txt for license information.

namespace VictorBush.Ego.NefsLib.Header
{
    using System;
    using VictorBush.Ego.NefsLib.DataTypes;
    using VictorBush.Ego.NefsLib.Item;

    /// <summary>
    /// Bitfield flags for part 6 data.
    /// </summary>
    [Flags]
    public enum Part6Flags
    {
        /// <summary>
        /// No flags set.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Indicates if item is transformed (I think this means encrypted?)
        /// </summary>
        IsTransformed = 0x1,

        /// <summary>
        /// Indicates if item is a directory.
        /// </summary>
        IsDirectory = 0x2,

        /// <summary>
        /// Meaning unknown.
        /// </summary>
        IsDuplicated = 0x4,

        /// <summary>
        /// Indicates if item is cacheable by game engine.
        /// </summary>
        IsCacheable = 0x8,

        /// <summary>
        /// Meaning unknown.
        /// </summary>
        IsPatched = 0x20,
    }

    /// <summary>
    /// An entry in header part 6 for an item in an archive.
    /// </summary>
    public class NefsHeaderPart6Entry
    {
        /// <summary>
        /// The size of a part 6 entry.
        /// </summary>
        public const uint Size = 0x4;

        /// <summary>
        /// Initializes a new instance of the <see cref="NefsHeaderPart6Entry"/> class.
        /// </summary>
        /// <param name="id">The item id this entry is for.</param>
        /// <param name="flags">Flags.</param>
        public NefsHeaderPart6Entry(NefsItemId id, Part6Flags flags)
        {
            this.Id = id;
            this.Data0x00_BitField.Value = (uint)flags;
        }

        /// <summary>
        /// Gets the item id this is for. This value is not written in the header but is stored here
        /// for reference.
        /// </summary>
        public NefsItemId Id { get; }

        /// <summary>
        /// A flag indicating whether this item is cacheable (presumably by the game engine).
        /// </summary>
        public bool IsCacheable => (this.Data0x00_BitField.Value & (int)Part6Flags.IsCacheable) > 0;

        /// <summary>
        /// A flag indicating whether this item is a directory.
        /// </summary>
        public bool IsDirectory => (this.Data0x00_BitField.Value & (int)Part6Flags.IsDirectory) > 0;

        /// <summary>
        /// A flag indicating whether this item is duplicated.
        /// </summary>
        public bool IsDuplicated => (this.Data0x00_BitField.Value & (int)Part6Flags.IsDuplicated) > 0;

        /// <summary>
        /// A flag indicating whether this item is patched (meaning unknown).
        /// </summary>
        public bool IsPatched => (this.Data0x00_BitField.Value & (int)Part6Flags.IsCacheable) > 0;

        /// <summary>
        /// A flag indicating whether this item is transformed (meaning unknown).
        /// </summary>
        public bool IsTransformed => (this.Data0x00_BitField.Value & (int)Part6Flags.IsTransformed) > 0;

        /// <summary>
        /// Data at offset 0x00.
        /// </summary>
        [FileData]
        internal UInt32Type Data0x00_BitField { get; } = new UInt32Type(0x00);
    }
}
