namespace bs
{
    [Serializable]
    internal class SaException : Exception
    {
        public SaException(string message) : base(message) { }
    }
}
