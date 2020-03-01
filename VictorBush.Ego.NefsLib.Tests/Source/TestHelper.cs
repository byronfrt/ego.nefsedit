// See LICENSE.txt for license information.

namespace VictorBush.Ego.NefsLib.Tests
{
    using System.IO.Abstractions.TestingHelpers;

    internal static class TestHelper
    {
        /// <summary>
        /// The path to the test data file for data type tests. The file
        /// is created with the <see cref="DataTypesTestData"/> and put on
        /// the mock file system.
        /// </summary>
        public const string DataTypesTestFilePath = "DataTypesTest.dat";

        /// <summary>
        /// Test data used for data type tests.
        /// </summary>
        public static readonly byte[] DataTypesTestData = new byte[]
        {
            0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01,
            0x18, 0x17, 0x16, 0x15, 0x14, 0x13, 0x12, 0x11,
            0x28, 0x27, 0x26, 0x25, 0x24, 0x23, 0x22, 0x21,
            0x38, 0x37, 0x36, 0x35, 0x34, 0x33, 0x32, 0x31,
        };

        /// <summary>
        /// Creates a mock file system for data type tests that includes a test file.
        /// </summary>
        /// <returns>A mock file system.</returns>
        public static MockFileSystem CreateDataTypesTestFileSystem()
        {
            var fs = new MockFileSystem();
            fs.AddFile(DataTypesTestFilePath, new MockFileData(DataTypesTestData));
            return fs;
        }
    }
}
