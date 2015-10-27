using System;

namespace Hinata.Exceptions
{
    /// <summary>記事に対して編集の権利がないことを示す例外を扱うクラスです。</summary>
    public class NotEntitledToEditItemException : Exception
    {
        public NotEntitledToEditItemException()
        {
        }

        public NotEntitledToEditItemException(string message) : base(message)
        {
        }

        public NotEntitledToEditItemException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
