using FolderSync;

namespace FolderSyncTests {
    [TestClass]

    public class FilesInfoTests {
        private static readonly int hundredPiRounded = 314;
        private static readonly string path1 = "C:\\Folder\\Sunbfolder\\file.txt";
        private static readonly FilesInfo
            info1 = new("info1", hundredPiRounded, "hex"),
            infoMinimal = new("", 0, null!),
            info2 = new(path1, 100, "abcd");
        [TestMethod]
        public void FilesInfoNullCreationTest() {
            Assert.ThrowsException<ArgumentNullException>(() => { FilesInfo f = new(null!, 0, null!); });
        }
        [TestMethod]
        public void FilesInfoNegativeSizeCreationTest() {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { FilesInfo f = new("", -5, null!); });
        }
        [TestMethod]
        public void FilesInfoByteSizeTest() {
            Assert.AreEqual(hundredPiRounded, info1.SizeBytes);
        }
        [TestMethod]
        public void FilesInfoByteSizeNotEqualTest() {
            Assert.AreNotEqual(3140, info1.SizeBytes);
        }
        [TestMethod]
        public void FilesInfoNameFirstTest() {
            Assert.AreEqual("info1", info1.Name);
        }
        [TestMethod]
        public void FilesInfoNameSecondTest() {
            Assert.AreEqual("", infoMinimal.Name);
        }
        [TestMethod]
        public void FilesInfoNameThirdTest() {
            Assert.AreEqual("file.txt", info2.Name);
        }
        [TestMethod]
        public void FilesInfoNameNotEqualTest() {
            Assert.AreNotEqual("file", info2.Name);
        }
        [TestMethod]
        public void FilesInfoPathTest() {
            Assert.AreEqual(path1, info2.Path);
        }
        [TestMethod]
        public void FilesInfoPathNotEqualTest() {
            Assert.AreNotEqual(path1, info1.Path);
        }
        [TestMethod]
        public void FilesInfoCompareEqualTest() {
            Assert.IsTrue(info1.IsEqual(info1));
        }
        [TestMethod]
        public void FilesInfoCompareEqualSecondTest() {
            Assert.IsTrue(info2.IsEqual(info2));
        }
        [TestMethod]
        public void FilesInfoCompareNotEqualTest() {
            Assert.IsFalse(info1.IsEqual(info2));
        }
    }
}
