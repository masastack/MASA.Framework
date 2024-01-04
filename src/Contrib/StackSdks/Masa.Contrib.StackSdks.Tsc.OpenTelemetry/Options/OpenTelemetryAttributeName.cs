// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry;

/// <summary>
/// Constants for semantic attribute names outlined by the OpenTelemetry specifications.
/// <see href="https://github.com/open-telemetry/opentelemetry-specification/blob/master/specification/trace/semantic_conventions/README.md"/>.
/// </summary>
internal static class OpenTelemetryAttributeName
{
    /// <summary>
    /// Constants for deployment semantic attribute names outlined by the OpenTelemetry specifications.
    /// <see href="https://github.com/open-telemetry/opentelemetry-specification/blob/11cc73939a32e3a2e6f11bdeab843c61cf8594e9/specification/resource/semantic_conventions/deployment_environment.md"/>.
    /// </summary>
    internal static class Deployment
    {
        /// <summary>
        /// The name of the deployment environment (aka deployment tier).
        /// </summary>
        /// <example>staging; production.</example>
        public const string ENVIRONMENT = "deployment.environment";
    }

    /// <summary>
    /// Constants for end user semantic attribute names outlined by the OpenTelemetry specifications.
    /// <see href="https://github.com/open-telemetry/opentelemetry-specification/blob/master/specification/trace/semantic_conventions/span-general.md"/>.
    /// </summary>
    internal static class EndUser
    {
        /// <summary>
        /// Username or client_id extracted from the access token or Authorization header in the inbound request from outside the system.
        /// </summary>
        /// <example>E.g. username.</example>
        public const string ID = "enduser.id";

        /// <summary>
        /// Actual/assumed role the client is making the request under extracted from token or application security context.
        /// </summary>
        /// <example>E.g. admin.</example>
        public const string ROLE = "enduser.role";

        /// <summary>
        /// Scopes or granted authorities the client currently possesses extracted from token or application security context.
        /// The value would come from the scope associated with an OAuth 2.0 Access Token or an attribute value in a SAML 2.0 Assertion.
        /// </summary>
        /// <example>E.g. read:message,write:files.</example>
        public const string SCOPE = "enduser.scope";

        /// <summary>
        /// custom attr
        /// </summary>
        public const string USER_NICK_NAME = "enduser.nick_name";
    }

    /// <summary>
    /// Constants for HTTP semantic attribute names outlined by the OpenTelemetry specifications.
    /// <see href="https://github.com/open-telemetry/opentelemetry-specification/blob/master/specification/trace/semantic_conventions/http.md"/>.
    /// </summary>
    internal static class Http
    {
        public const string STATUS_CODE = "status_code";

        /// <summary>
        /// The URI scheme identifying the used protocol.
        /// </summary>
        /// <example>E.g. http or https.</example>
        public const string SCHEME = "http.scheme";

        /// <summary>
        /// Kind of HTTP protocol used.
        /// </summary>
        /// <example>E.g. 1.0, 1.1, 2.0, SPDY or QUIC.</example>
        public const string FLAVOR = "http.flavor";

        /// <summary>
        /// The IP address of the original client behind all proxies, if known (e.g. from X-Forwarded-For).
        /// </summary>
        /// <example>E.g. 83.164.160.102.</example>
        public const string CLIENT_IP = "http.client_ip";

        /// <summary>
        /// The size of the request payload body in bytes. This is the number of bytes transferred excluding headers and is often,
        /// but not always, present as the Content-Length header. For requests using transport encoding, this should be the
        /// compressed size.
        /// </summary>
        /// <example>E.g. 3495.</example>
        public const string REQUEST_CONTENT_LENGTH = "http.request_content_length";

        /// <summary>
        /// custom attr
        /// </summary>
        public const string REQUEST_CONTENT_BODY = "http.request_content_body";

        /// <summary>
        /// The content type of the request body.
        /// </summary>
        /// <example>E.g. application/json.</example>
        public const string REQUEST_CONTENT_TYPE = "http.request_content_type";

        /// <summary>
        /// The size of the response payload body in bytes. This is the number of bytes transferred excluding headers and is often,
        /// but not always, present as the Content-Length header. For requests using transport encoding, this should be the
        /// compressed size.
        /// </summary>
        /// <example>E.g. 3495.</example>
        public const string RESPONSE_CONTENT_LENGTH = "http.response_content_length";

        /// <summary>
        /// The content type of the response body.
        /// </summary>
        /// <example>E.g. application/json.</example>
        public const string RESPONSE_CONTENT_TYPE = "http.response_content_type";

        /// <summary>
        /// custom attr
        /// </summary>
        public const string RESPONSE_CONTENT_BODY = "http.response_content_body";

        /// <summary>
        /// https://opentelemetry.io/docs/specs/semconv/http/http-spans/#common-attributes
        /// </summary>
        public const string REQUEST_USER_AGENT = "user_agent.original";

        public const string REQUEST_AUTHORIZATION = "authorization";
    }

    /// <summary>
    /// Constants for host semantic attribute names outlined by the OpenTelemetry specifications.
    /// <see href="https://github.com/open-telemetry/opentelemetry-specification/blob/11cc73939a32e3a2e6f11bdeab843c61cf8594e9/specification/resource/semantic_conventions/host.md"/>.
    /// </summary>
    internal static class Host
    {
        /// <summary>
        /// Name of the host. On Unix systems, it may contain what the hostname command returns, or the fully qualified hostname,
        /// or another name specified by the user.
        /// </summary>
        /// <example>E.g. opentelemetry-test.</example>
        public const string NAME = "host.name";
    }

    /// <summary>
    /// Constants for service semantic attribute names outlined by the OpenTelemetry specifications.
    /// <see href="https://github.com/open-telemetry/opentelemetry-specification/blob/master/specification/trace/semantic_conventions/messaging.md"/>.
    /// </summary>
    internal static class Service
    {
        public const string NAME = "service.name";

        /// <summary>
        /// custom attr
        /// </summary>
        public const string PROJECT_NAME = "service.project.name";

        public const string LAYER = "service.layer";
    }  

    internal static class ExceptionAttributeName
    {
        public const string TYPE = "exception.type";

        public const string MESSAGE = "exception.message";

        public const string STACKTRACE = "exception.stacktrace";
    }
}
