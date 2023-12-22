namespace GraphApp
{
    /// <summary>
    /// Represents an interface for the things that can be named using string.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>
        /// The name of an object.
        /// </summary>
        public string Name { get; }
    }
}