// See LICENSE.txt for license information.

namespace VictorBush.Ego.NefsLib.Item
{
    using System;
    using VictorBush.Ego.NefsLib.DataSource;
    using VictorBush.Ego.NefsLib.Header;

    /// <summary>
    /// An item in a NeFS archive (file or directory).
    /// </summary>
    public class NefsItem : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NefsItem"/> class.
        /// </summary>
        /// <param name="id">The item id (index).</param>
        /// <param name="fileName">The file name within the archive.</param>
        /// <param name="directoryId">The directory id the item is in.</param>
        /// <param name="dataSource">The data source for the item's data.</param>
        /// <param name="flags">Item attributes bitfield from header part 6.</param>
        /// <param name="state">The item state.</param>
        public NefsItem(
            NefsItemId id,
            string fileName,
            NefsItemId directoryId,
            INefsDataSource dataSource,
            Part6Flags flags,
            NefsItemState state = NefsItemState.None)
        {
            this.Id = id;
            this.DirectoryId = directoryId;
            this.DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            this.State = state;
            this.Flags = flags;

            // Save file name
            this.FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        /// <summary>
        /// The size of the item's data in the archive.
        /// </summary>
        public UInt32 CompressedSize => this.DataSource?.Size.Size ?? 0;

        /// <summary>
        /// The current data source for this item.
        /// </summary>
        public INefsDataSource DataSource { get; private set; }

        /// <summary>
        /// The id of the directory item this item is in. When the item is in the root directory,
        /// the parent id is the same as the item's id.
        /// </summary>
        public NefsItemId DirectoryId { get; }

        /// <summary>
        /// The size of the item's data when extracted from the archive.
        /// </summary>
        public UInt32 ExtractedSize => this.DataSource?.Size.ExtractedSize ?? 0;

        /// <summary>
        /// The item's file or directory name, depending on type.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// The attribute flags from header part 6.
        /// </summary>
        public Part6Flags Flags { get; }

        /// <summary>
        /// The id of this item.
        /// </summary>
        public NefsItemId Id { get; }

        /// <summary>
        /// A flag indicating whether this item is cacheable (presumably by the game engine).
        /// </summary>
        public bool IsCacheable => this.Flags.HasFlag(Part6Flags.IsCacheable);

        /// <summary>
        /// A flag indicating whether this item is a directory.
        /// </summary>
        public bool IsDirectory => this.Flags.HasFlag(Part6Flags.IsDirectory);

        /// <summary>
        /// A flag indicating whether this item is duplicated.
        /// </summary>
        public bool IsDuplicated => this.Flags.HasFlag(Part6Flags.IsDuplicated);

        /// <summary>
        /// A flag indicating whether this item is patched (meaning unknown).
        /// </summary>
        public bool IsPatched => this.Flags.HasFlag(Part6Flags.IsCacheable);

        /// <summary>
        /// A flag indicating whether this item is transformed (meaning unknown).
        /// </summary>
        public bool IsTransformed => this.Flags.HasFlag(Part6Flags.IsTransformed);

        /// <summary>
        /// The modification state of the item. Represents any pending changes to this item. Pending
        /// changes are applied when the archive is saved.
        /// </summary>
        public NefsItemState State { get; private set; }

        /// <summary>
        /// Creates an item from header data.
        /// </summary>
        /// <param name="id">The item id.</param>
        /// <param name="header">The header data.</param>
        /// <param name="dataSourceList">The item list to use as the item data source.</param>
        /// <returns>A new <see cref="NefsItem"/>.</returns>
        public static NefsItem CreateFromHeader(NefsItemId id, NefsHeader header, NefsItemList dataSourceList)
        {
            var p1 = header.Part1.EntriesById[id];
            var p2 = header.Part2.EntriesById[id];
            var p6 = header.Part6.EntriesById[id];

            // Find parent
            var parentId = header.GetItemDirectoryId(id);

            // Offset and size
            var dataOffset = p1.Data0x00_OffsetToData.Value;
            var extractedSize = p2.Data0x0c_ExtractedSize.Value;

            // Flags
            var flags = (Part6Flags)p6.Data0x00_BitField.Value;

            // Data source
            INefsDataSource dataSource;
            if (flags.HasFlag(Part6Flags.IsDirectory))
            {
                // Item is a directory
                dataSource = new NefsEmptyDataSource();
            }
            else if (p1.IndexIntoPart4 == 0xFFFFFFFFU)
            {
                // Item is not compressed
                var size = new NefsItemSize(extractedSize);
                dataSource = new NefsItemListDataSource(dataSourceList, dataOffset, size);
            }
            else
            {
                // Item is compressed
                var p4 = header.Part4.EntriesByIndex[p1.IndexIntoPart4];
                var size = new NefsItemSize(extractedSize, p4.ChunkSizes);
                dataSource = new NefsItemListDataSource(dataSourceList, dataOffset, size);
            }

            // File name and path
            var fileName = header.GetItemFileName(id);

            // Create item
            return new NefsItem(id, fileName, parentId, dataSource, flags);
        }

        /// <summary>
        /// Clones this item metadata.
        /// </summary>
        /// <returns>A new <see cref="NefsItem"/>.</returns>
        public object Clone()
        {
            return new NefsItem(
                this.Id,
                this.FileName,
                this.DirectoryId,
                this.DataSource,
                this.Flags,
                state: this.State);
        }

        /// <summary>
        /// Updates the data source for the item and the item's state. If the data source has
        /// changed, the new item data will be written to the archive when it is saved. If
        /// compression is required, the file data will be compressed when the archive is written.
        /// </summary>
        /// <param name="dataSource">The new data source to use.</param>
        /// <param name="state">The new item state.</param>
        public void UpdateDataSource(INefsDataSource dataSource, NefsItemState state)
        {
            if (this.IsDirectory)
            {
                throw new InvalidOperationException($"Cannot perform {nameof(this.UpdateDataSource)} on a directory.");
            }

            this.DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            this.State = state;
        }

        /// <summary>
        /// Updates the item state.
        /// </summary>
        /// <param name="state">The new state.</param>
        public void UpdateState(NefsItemState state)
        {
            this.State = state;
        }
    }
}
