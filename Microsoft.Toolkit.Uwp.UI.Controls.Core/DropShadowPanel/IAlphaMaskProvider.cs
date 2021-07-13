// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Composition;

/// <summary>
/// Any user control can implement this interface to provide a custom alpha mask to it's parent <see cref="DropShadowPanel"/>
/// </summary>
public interface IAlphaMaskProvider
{
    /// <summary>
    /// This method should return the appropiate alpha mask to be used in the shadow of this control
    /// </summary>
    /// <returns>The alpha mask as a composition brush</returns>
    CompositionBrush GetAlphaMask();
}
