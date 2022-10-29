// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using System;
#if NET6_0_OR_GREATER
using System.Reflection.Metadata;
using osu.Framework.Testing;

#endif

// We publish our internal attributes to other sub-projects of the framework.
// Note, that we omit visual tests as they are meant to test the framework
// behavior "in the wild".

#if NET6_0_OR_GREATER
[assembly: MetadataUpdateHandler(typeof(LocalHotReloadCallbackReceiver))]
#endif


internal static class LocalHotReloadCallbackReceiver
{
    public static event Action<Type[]> CompilationFinished;
    public static void UpdateApplication([CanBeNull] Type[] updatedTypes) => CompilationFinished?.Invoke(updatedTypes);
}
