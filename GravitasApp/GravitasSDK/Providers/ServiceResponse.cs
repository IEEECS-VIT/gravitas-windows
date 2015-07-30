using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravitasSDK.Providers
{
    /// <summary>
    /// Status codes relating to network or parsing errors.
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// Successful execution.
        /// </summary>
        Success = 0,
        /// <summary>
        /// Connectivity or network issues.
        /// </summary>
        NoInternet = 1,
        /// <summary>
        /// Server errors such as "Unavailable", "Database Error" or "Gateway Timeout".
        /// </summary>
        ServerError = 2,
        /// <summary>
        /// The data has not changed from the previous version.
        /// </summary>
        NoChange = 3,
        /// <summary>
        /// An unknown error occured.
        /// </summary>
        UnknownError = 4,
        /// <summary>
        /// Occurs if the file or content is missing.
        /// </summary>
        NoData = 5,
        /// <summary>
        /// The service provider is busy.
        /// </summary>
        Busy = 6
    }

    /// <summary>
    /// Stores the status and content of the response received for some request.
    /// </summary>
    /// <remarks>
    /// This class is intended to be used as a return type for methods that process a request. Any instance of this class is read-only.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of content the response contains.
    /// </typeparam>
    public sealed class Response<T>
    {
        private readonly StatusCode _code;
        private readonly T _content;

        public StatusCode Code
        {
            get { return _code; }
        }
        public T Content
        {
            get { return _content; }
        }

        public Response(StatusCode code, T content)
        {
            _code = code;
            _content = content;
        }
    }
}
