using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class ExamplePlaymodeTest
{
    [Test]
    public void TestPasses()
    {
        // This is a simple placeholder test.
        // It's designed to always pass to confirm the test runner is working.
        Assert.Pass();
    }

    // A UnityTest allows you to use `yield return` to skip frames.
    [UnityTest]
    public IEnumerator TestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame.
        yield return null;
        Assert.Pass();
    }
}