using FolderSync;

namespace FolderSyncTests {

    [TestClass]
    public class WithLogTests {
        private readonly static WithLog mockWithLog = new MockWithLog();

        public class MockWithLog : WithLog {
        }
        [TestMethod]
        public void CreateMockWithLogTest() {
            var withLog = new MockWithLog();
            Assert.IsNotNull(withLog);
        }
        [TestMethod]
        public void WithLogAddNullListenerTest() {
            Assert.ThrowsException<ArgumentNullException>(() => { mockWithLog.AddLogListener(null!); });
        }
    }
}
