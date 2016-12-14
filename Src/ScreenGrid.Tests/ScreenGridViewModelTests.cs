using System;

using NUnit.Framework;

using ScreenGrid.Models;
using ScreenGrid.ViewModels;

namespace ScreenGrid.Tests
{
    [TestFixture]
    class ScreenGridViewModelTests
    {
        [Test]
        public void SelectOctaneRenderStandaloneMainWindowSuccess()
        {
            var list = new[]
            {
                new NativeWindowState { Handle = new IntPtr(0x000E0780), ClassName = "JUCE_158f853ded5", Width = 217, Height = 363 },
                new NativeWindowState { Handle = new IntPtr(0x00050786), ClassName = "JUCE_158f853ded5", Width = 1032, Height = 795 },
                new NativeWindowState { Handle = new IntPtr(0x0003053A), ClassName = "ToDoListFrame", Width = 1205, Height = 814 },
                new NativeWindowState { Handle = new IntPtr(0x000703D6), ClassName = "MozillaWindowClass", Width = 936, Height = 742 },
                new NativeWindowState { Handle = new IntPtr(0x000602F4), ClassName = "Internet Explorer_Hidden", Width = 0, Height = 0 },
            };

            var result = ScreenGridViewModel.SelectOctaneRenderStandaloneMainWindow(list, "JUCE_158f853ded5");

            Assert.AreEqual(1032, result.Width);
        }

        [Test]
        public void SelectOctaneRenderStandaloneMainWindowAnother()
        {
            var list = new[]
            {
                new NativeWindowState { Handle = new IntPtr(0x000703D6), ClassName = "MozillaWindowClass", Width = 936, Height = 742 },
                new NativeWindowState { Handle = new IntPtr(0x000E0780), ClassName = "JUCE_158f853ded5", Width = 217, Height = 363 },
                new NativeWindowState { Handle = new IntPtr(0x00050786), ClassName = "JUCE_158f853ded5", Width = 1032, Height = 795 },
                new NativeWindowState { Handle = new IntPtr(0x0003053A), ClassName = "ToDoListFrame", Width = 1205, Height = 814 },
                new NativeWindowState { Handle = new IntPtr(0x000602F4), ClassName = "Internet Explorer_Hidden", Width = 0, Height = 0 },
            };

            var result = ScreenGridViewModel.SelectOctaneRenderStandaloneMainWindow(list, "JUCE_158f853ded5");

            Assert.IsNull(result);
        }
    }
}
