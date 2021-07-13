namespace Moqesq
{

    /// <summary>
    /// Specify logic for comparison of two objects based on the name of their property
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <param name="expected">Expected property value</param>
    /// <param name="actual">Actual property value</param>
    /// <returns></returns>
    public delegate bool CheckValue(string propertyName, object expected, object? actual);
}
