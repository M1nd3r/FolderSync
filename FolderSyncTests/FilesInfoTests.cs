using FolderSync;

namespace FolderSyncTests {
    [TestClass]

    public class FilesInfoTests {
        [TestMethod]
        public void FilesInfoNullCreationTest() {
            Assert.ThrowsException<ArgumentNullException>(() => { FilesInfo f = new(null!, 0, null!); });
        }
        [TestMethod]
        public void FilesInfoNegativeSizeCreationTest() {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { FilesInfo f = new("", -5, null!); });
        }
    }
}
