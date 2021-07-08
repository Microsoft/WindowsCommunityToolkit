// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

// We can suppress the .init flag for local variables for the entire module.
// This doesn't affect the correctness of methods in this assembly, as none of them
// are relying on the JIT ensuring that all local memory is zeroed out to work. Doing
// this can provide some minor performance benefits, depending on the workload.
[module: SkipLocalsInit]