// <copyright file="IJsonSerializerSettingsProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.Extensions.Json
{
    using Newtonsoft.Json;

    /// <summary>
    /// A factory to get the default <see cref="JsonSerializerSettings"/> for the context.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This sets up a standard <see cref="JsonSerializerSettings"/> instance with opinionated defaults.
    /// </para>
    /// <para>
    /// The default implementation uses string-based serialized for enums, and resolves <see cref="JsonConverter"/>
    /// instances that are registered in the container.
    /// </para>
    /// </remarks>
    public interface IJsonSerializerSettingsProvider
    {
        /// <summary>
        /// Gets an instance of the default JsonSerializerSettings.
        /// </summary>
        JsonSerializerSettings Instance { get; }
    }
}