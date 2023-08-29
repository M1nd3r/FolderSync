using FolderSync;

namespace FolderSyncTests {
    [TestClass]
    public class FolderTests {
        private static readonly Folder
            rootFolder = new("C:\\"),
            folderA = new("C:\\A"),
            folderB = new("C:\\B"),
            folderCinA = new("C:\\A\\C"),
            testFolder = new("C:\\Test\\TestFolder");
        private static readonly string
            anotherTestPath = "C:\\Random\\Test\\Path",
            anotherTestPathOuch = "C:\\Random\\Test\\Path\\ouch";


        [TestMethod]
        public void FolderNullCreationTest() {
            Assert.ThrowsException<ArgumentNullException>(() => { Folder f = new(null!); });
        }
        [TestMethod]
        public void FolderEmptyCreationTest() {
            Folder f = new("");
            Assert.IsNotNull(f);
        }
        [TestMethod]
        public void AddFolderToItselfTest() {
            Assert.ThrowsException<ArgumentException>(() => {
                folderA.AddSubfolder(folderA);
            });
        }
        [TestMethod]
        public void AddParentFolderAsSubfolderTest() {
            Assert.ThrowsException<ArgumentException>(() => {
                folderCinA.AddSubfolder(folderA);
            });
        }
        [TestMethod]
        public void AddNullFileToFolderTest() {
            Assert.ThrowsException<ArgumentNullException>(() => { folderA.AddFile(null!); });
        }
        [TestMethod]
        public void AddNullFilesToFolderTest() {
            Assert.ThrowsException<ArgumentNullException>(() => { folderA.AddFiles(null!); });
        }
        [TestMethod]
        public void GetPathOfAFolderEqualTest() {
            var folder = new Folder(anotherTestPath);
            Assert.AreEqual(folder.Path, anotherTestPath);
        }
        [TestMethod]
        public void GetPathOfAFolderNotEqualTest() {

            var folder = new Folder(anotherTestPath);
            Assert.AreNotEqual(folder.Path, anotherTestPathOuch);
        }
        [TestMethod]
        public void GetFolderNameRootTest() {
            Assert.AreEqual("", rootFolder.Name);
        }
        [TestMethod]
        public void GetFolderNameTest() {
            Assert.AreEqual("TestFolder", testFolder.Name);
        }

        [TestMethod]
        public void DoesNotContainNullSubfolderTest() {
            Assert.IsFalse(testFolder.ContainsFile(null!));
        }
        [TestMethod]
        public void ContainsSubfolderTest() {
            folderA.AddSubfolder(folderCinA);
            Assert.IsTrue(folderA.ContainsFolder(folderCinA));
        }
        [TestMethod]
        public void GetSubfolderTest() {
            folderA.AddSubfolder(folderCinA);
            Assert.IsNotNull(folderA.GetFolder(folderCinA.Name));
        }
        [TestMethod]
        public void GetNullSubfolderTest() {
            folderA.AddSubfolder(folderCinA);
            Assert.IsNull(folderA.GetFolder(folderB.Name));
        }
    }
}
