// See LICENSE.txt for license information.

namespace VictorBush.Ego.NefsLib.Tests.Header
{
    using System;
    using System.Collections.Generic;
    using VictorBush.Ego.NefsLib.DataSource;
    using VictorBush.Ego.NefsLib.Header;
    using VictorBush.Ego.NefsLib.Item;
    using Xunit;

    public class NefsHeaderPart6Tests
    {
        [Fact]
        public void NefsHeaderPart6_MultipleItems_EntriesPopulated()
        {
            var items = new NefsItemList(@"C:\archive.nefs");

            var file1DataSource = new NefsItemListDataSource(items, 123, new NefsItemSize(456, new List<UInt32> { 11, 12, 13 }));
            var file1Flags = Part6Flags.IsTransformed | Part6Flags.IsCacheable;
            var file1 = new NefsItem(new NefsItemId(0), "file1", new NefsItemId(0), file1DataSource, file1Flags);
            items.Add(file1);

            var file2DataSource = new NefsItemListDataSource(items, 456, new NefsItemSize(789, new List<UInt32> { 14, 15, 16 }));
            var file2Flags = Part6Flags.IsCacheable;
            var file2 = new NefsItem(new NefsItemId(1), "file2", new NefsItemId(1), file2DataSource, file2Flags);
            items.Add(file2);

            var dir1DataSource = new NefsEmptyDataSource();
            var dir1Flags = Part6Flags.IsDirectory;
            var dir1 = new NefsItem(new NefsItemId(2), "dir1", new NefsItemId(2), dir1DataSource, dir1Flags);
            items.Add(dir1);

            var p6 = new NefsHeaderPart6(items);

            Assert.Equal(3, p6.EntriesById.Count);

            /*
            file1
            */

            Assert.True(p6.EntriesById[file1.Id].IsCacheable);
            Assert.False(p6.EntriesById[file1.Id].IsDirectory);
            Assert.False(p6.EntriesById[file1.Id].IsDuplicated);
            Assert.False(p6.EntriesById[file1.Id].IsPatched);
            Assert.True(p6.EntriesById[file1.Id].IsTransformed);

            /*
            file2
            */

            Assert.True(p6.EntriesById[file2.Id].IsCacheable);
            Assert.False(p6.EntriesById[file2.Id].IsDirectory);
            Assert.False(p6.EntriesById[file2.Id].IsDuplicated);
            Assert.False(p6.EntriesById[file2.Id].IsPatched);
            Assert.False(p6.EntriesById[file2.Id].IsTransformed);

            /*
            dir1
            */

            Assert.False(p6.EntriesById[dir1.Id].IsCacheable);
            Assert.True(p6.EntriesById[dir1.Id].IsDirectory);
            Assert.False(p6.EntriesById[dir1.Id].IsDuplicated);
            Assert.False(p6.EntriesById[dir1.Id].IsPatched);
            Assert.False(p6.EntriesById[dir1.Id].IsTransformed);
        }

        [Fact]
        public void NefsHeaderPart6_NoItems_EntriesEmpty()
        {
            var items = new NefsItemList(@"C:\archive.nefs");
            var p6 = new NefsHeaderPart6(items);
            Assert.Empty(p6.EntriesByIndex);
            Assert.Empty(p6.EntriesById);
        }
    }
}
