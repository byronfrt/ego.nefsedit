﻿// See LICENSE.txt for license information.

namespace VictorBush.Ego.NefsLib.Item
{
    using System;
    using VictorBush.Ego.NefsLib.DataSource;
    using VictorBush.Ego.NefsLib.Header;
    using VictorBush.Ego.NefsLib.Utility;

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
        /// <param name="filePathInArchive">The file path within the archive.</param>
        /// <param name="directoryId">The directory id the item is in.</param>
        /// <param name="type">The type of item.</param>
        /// <param name="dataSource">The data source for the item's data.</param>
        /// <param name="unknownData">Unknown metadata.</param>
        public NefsItem(
            NefsItemId id,
            string fileName,
            string filePathInArchive,
            NefsItemId directoryId,
            NefsItemType type,
            INefsDataSource dataSource,
            NefsItemUnknownData unknownData)
        {
            this.Id = id;
            this.DirectoryId = directoryId;
            this.Type = type;
            this.DataSource = dataSource;

            // Unknown data
            this.Part6Unknown0x00 = unknownData.Part6Unknown0x00;
            this.Part6Unknown0x01 = unknownData.Part6Unknown0x01;
            this.Part6Unknown0x02 = unknownData.Part6Unknown0x02;
            this.Part6Unknown0x03 = unknownData.Part6Unknown0x03;
            this.Part7Unknown0x00 = unknownData.Part7Unknown0x00;
            this.Part7Unknown0x04 = unknownData.Part7Unknown0x04;

            // Save file name
            this.FileName = fileName;
            this.FilePathInArchive = filePathInArchive;

            // Compute file path hash
            this.FilePathInArchiveHash = HashHelper.HashStringMD5(this.FilePathInArchive);
        }

        /// <summary>
        /// The size of the item's data in the archive.
        /// </summary>
        public UInt32 CompressedSize => this.DataSource?.Size.Size ?? 0;

        /// <summary>
        /// The data source for the new data for this item.
        /// </summary>
        public INefsDataSource DataSource { get; internal set; }

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
        /// Gets path to the item within the archive.
        /// Example: cars/models/fr2/interior/fr2.xml.
        /// </summary>
        public string FilePathInArchive { get; }

        /// <summary>
        /// Hash string of the file path.
        /// </summary>
        public string FilePathInArchiveHash { get; }

        /// <summary>
        /// The id of this item.
        /// </summary>
        public NefsItemId Id { get; }

        /// <summary>
        /// Unknown data in the part 6 entry.
        /// </summary>
        public byte Part6Unknown0x00 { get; }

        /// <summary>
        /// Unknown data in the part 6 entry.
        /// </summary>
        public byte Part6Unknown0x01 { get; }

        /// <summary>
        /// Unknown data in the part 6 entry.
        /// </summary>
        public byte Part6Unknown0x02 { get; }

        /// <summary>
        /// Unknown data in the part 6 entry.
        /// </summary>
        public byte Part6Unknown0x03 { get; }

        /// <summary>
        /// Unknown data in the part 7 entry.
        /// </summary>
        public UInt32 Part7Unknown0x00 { get; }

        /// <summary>
        /// Unknown data in the part 7 entry.
        /// </summary>
        public UInt32 Part7Unknown0x04 { get; }

        /// <summary>
        /// The modification state of the item. Represents any pending changes to this item. Pending
        /// changes are applied when the archive is saved.
        /// </summary>
        public NefsItemState State { get; internal set; }

        /// <summary>
        /// The type of item this is.
        /// </summary>
        public NefsItemType Type { get; }

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
            var p4 = header.Part4.EntriesByIndex[id.Value];
            var p6 = header.Part6.Entries[(int)id.Value];

            // Determine type
            var type = p2.ExtractedSize.Value == 0 ? NefsItemType.Directory : NefsItemType.File;

            // Find parent
            var parentId = header.GetItemDirectoryId(id);

            // Offset and size
            var dataOffset = p1.OffsetToData.Value;
            var extractedSize = p2.ExtractedSize.Value;
            var size = new NefsItemSize(extractedSize, p4.ChunkSizes);

            // Get file name
            var fileName = header.GetItemFileName(id);
            var filePath = header.GetItemFilePath(id);

            // Create data source
            var dataSource = new NefsItemListDataSource(dataSourceList, dataOffset, size);

            // Gather unknown metadata
            var unknown = new NefsItemUnknownData
            {
                Part6Unknown0x00 = p6.Byte0.Value[0],
                Part6Unknown0x01 = p6.Byte1.Value[0],
                Part6Unknown0x02 = p6.Byte2.Value[0],
                Part6Unknown0x03 = p6.Byte3.Value[0],
            };

            // Create item
            return new NefsItem(id, fileName, filePath, parentId, type, dataSource, unknown);
        }

        /// <summary>
        /// Clones this item metadata.
        /// </summary>
        /// <returns>A new <see cref="NefsItem"/>.</returns>
        public object Clone()
        {
            var unknownData = new NefsItemUnknownData
            {
                Part6Unknown0x00 = this.Part6Unknown0x00,
                Part6Unknown0x01 = this.Part6Unknown0x01,
                Part6Unknown0x02 = this.Part6Unknown0x02,
                Part6Unknown0x03 = this.Part6Unknown0x03,
                Part7Unknown0x00 = this.Part7Unknown0x00,
                Part7Unknown0x04 = this.Part7Unknown0x04,
            };

            return new NefsItem(
                this.Id,
                this.FileName,
                this.FilePathInArchive,
                this.DirectoryId,
                this.Type,
                this.DataSource,
                unknownData);
        }

        /// <summary>
        /// Flags the item to have it's file data replaced when the archive is written. If
        /// compression is desired, the file data will be compressed when the archive is written.
        /// </summary>
        /// <param name="dataSource">The new data source for the item.</param>
        public void ReplaceFile(INefsDataSource dataSource)
        {
            if (this.Type == NefsItemType.Directory)
            {
                throw new InvalidOperationException($"Cannot perform {nameof(this.ReplaceFile)} on a directory.");
            }

            this.DataSource = dataSource;
            this.State = NefsItemState.Replaced;
        }
    }
}
