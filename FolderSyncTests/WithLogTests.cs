using FolderSync;

namespace FolderSyncTests {

    [TestClass]
    public class WithLogTests {

        public class MockWithLog : WithLog {
        }
        [TestMethod]
        public  void CreateMockWithLogTest() {
            var withLog = new MockWithLog();
            Assert.IsNotNull(withLog);
        }
        [TestMethod]
        public  void CreateMockWithLogTest2() {
            var withLog = new MockWithLog();
            Assert.IsNotNull(withLog);
        }
    }
    
}
