using System;

namespace DevZest.Windows
{
    /// <summary>Describes the kind of value that a <see cref="SplitterDistance"/> object is holding.</summary>
    /// <remarks>
    /// <see cref="SplitterUnitType.Star"/> sizing is used to distribute available space by weighted proportions:
    /// sizeInPixel = availableSizeInPixel * weightedProportion / (weightedProportion + 1).
    /// </remarks>
    public enum SplitterUnitType
    {
        /// <summary>The value is expressed as a pixel.</summary>
        Pixel,
        /// <summary>The value is expressed as a weighted proportion of available space.</summary>
        Star
    }
}
