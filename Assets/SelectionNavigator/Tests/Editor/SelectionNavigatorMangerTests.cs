using NUnit.Framework;
using SpaceCan.SelectionNavigator;

public class SelectionNavigatorMangerTests
{
    [Test]
    public void BackWithEmptyHistoryDoesNotThrow()
    {
        SelectionNavigatorManager.Clear();
        SelectionNavigatorManager.Back();
    }

    [Test]
    public void ForwardWithEmptyHistoryDoesNotThrow()
    {
        SelectionNavigatorManager.Clear();
        SelectionNavigatorManager.Forward();
    }
}