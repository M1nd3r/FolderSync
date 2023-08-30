using FolderSync;

namespace FolderSyncTests {
    [TestClass]
    public class InputHandlerTests {
        private static readonly string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        [TestMethod]
        public void InputHandlerNullArgumentsTest() {
            Assert.IsFalse(InputHandler.HandleInput(null!));
        }
        [TestMethod]
        public void InputHandlerOneArgumentTest() {
            Assert.IsFalse(InputHandler.HandleInput(new string[] { "one" }));
        }
        [TestMethod]
        public void InputHandlerInvalidMandatoryArgumentsTest() {
            Assert.IsFalse(InputHandler.HandleInput(new string[] { "one", "two" }));
        }
        [TestMethod]
        public void InputHandlerFiveArgumentsTest() {
            Assert.IsFalse(InputHandler.HandleInput(new string[] { "one", "two", "three", "four", "five" }));
        }
        [TestMethod]
        public void InputHandlerValidMandatoryArgumentsTest() {
            Assert.IsTrue(InputHandler.HandleInput(new string[] { baseDir, baseDir }));
        }
        [TestMethod]
        public void InputHandlerValidMandatoryArgumentsInvalidLogPathTest() {
            Assert.IsFalse(InputHandler.HandleInput(new string[] { baseDir, baseDir, "" }));
        }
    }
}
