// -----------------------------------------------------------------------
// <copyright from="2018" to="2018" file="NopApiClientException.cs" company="Lindell Technologies">
//    Copyright (c) Lindell Technologies All Rights Reserved.
//    Information Contained Herein is Proprietary and Confidential.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Nop.Plugin.Api.Client.Exceptions
{
    public class NopApiClientException : ApplicationException
    {
        public NopApiClientException()
            : this(null, null)
        {
        }

        public NopApiClientException(string message, bool canRetry = false)
            : this(HttpStatusCode.BadRequest, null, null, message, canRetry)
        {
        }

        public NopApiClientException(string resourceUrl, string method, bool canRetry = false)
            : this(HttpStatusCode.ServiceUnavailable, resourceUrl, method, "No Response", canRetry)
        {
        }

        public NopApiClientException(
            HttpStatusCode statusCode,
            string resourceUrl,
            string method,
            string message,
            bool canRetry = false)
            : this(statusCode, resourceUrl, method, message, null, canRetry)
        {
        }

        public NopApiClientException(
            HttpStatusCode statusCode,
            string resourceUrl,
            string method,
            string message,
            NopApiClientExceptionDetails exceptionDetails,
            bool canRetry = false)
            : base(message ?? (exceptionDetails != null ? exceptionDetails.Message : statusCode.ToString()))
        {
            ResourceUrl = resourceUrl;
            StatusCode = statusCode;
            Method = method;
            ExceptionDetails = exceptionDetails;
            CanRetry = canRetry;
        }

        public NopApiClientException(
            HttpStatusCode statusCode,
            string resourceUrl,
            string method,
            Exception exception,
            bool canRetry = false)
            : base(exception.Message, exception)
        {
            ResourceUrl = resourceUrl;
            StatusCode = statusCode;
            Method = method;
            CanRetry = canRetry;
        }

        public bool CanRetry { get; }

        public string ResourceUrl { get; }

        public HttpStatusCode StatusCode { get; }

        public string Method { get; }

        public NopApiClientExceptionDetails ExceptionDetails { get; }

        public override string Message =>
            $"{Method} request for '{ResourceUrl}' returned [{StatusCode}] {base.Message} ";

        [SecurityPermission(
            SecurityAction.Demand,
            Flags = SecurityPermissionFlag.SerializationFormatter,
            SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}