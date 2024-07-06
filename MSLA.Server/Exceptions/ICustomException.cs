using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Exceptions
{
    /// <summary>This interface is to be implemented by every Custom Exception</summary>
    public interface ICustomException
    {
        /// <summary>The Message to be returned</summary>
        string CustomMessage
        { get; }
    }
}
