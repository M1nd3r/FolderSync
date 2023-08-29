using FolderSync;

namespace FolderSyncTests {
    [TestClass]
    public class FolderTests {
        [TestMethod]
        public void FolderNullCreationTest() {
            Action folderWithNullPathCreation = () => {
                Folder f = new Folder(null);
            };
            Assert.ThrowsException<ArgumentNullException>(folderWithNullPathCreation);
        }
        [TestMethod]
        public void FolderEmptyCreationTest() {
            Folder f = new Folder("");
            Assert.IsNotNull(f);
        }
        [TestMethod]
        public void AddFolderToItselfTest() {
            Assert.ThrowsException<ArgumentException>(AddFolderToItself);
        }
        private void AddFolderToItself() {
            Folder f = new Folder("C:\\");
            f.AddSubfolder(f);            
        }
    }
}
