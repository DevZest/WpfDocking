using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace DevZest.Windows
{
    /// <summary>Represents the size of <see cref="SplitContainer"/> resizable areas.</summary>
    /// <remarks>
    /// <para>
    /// When <see cref="Value"/> property is <see cref="Double.NaN"/>, it represents auto sizing: the
    /// actual size will be determined by the content's desired size.
    /// When <see cref="UnitType"/> property is <see cref="SplitterUnitType.Star"/>, the size
    /// will be calculated based on weighted proportion of available space.
    /// </para>
    /// <para>The <see cref="SplitterDistance"/> value can have following values in <b>XAML</b> ("?" represents a double value):</para>
    /// <b>"Auto"</b>: Auto size in pixel. The subsequent resizing by mouse drog or arrow key will result in a <see cref="SplitterUnitType.Pixel"/> type value.<br/>
    /// <b>"Auto*"</b>: Auto size in weighted proportion. The subsequent resizing by mouse drog or arrow key will result in a <see cref="SplitterUnitType.Star"/> type value.<br/>
    /// <b>"*"</b>: Equals <b>"1*"</b>.<br/>
    /// <b>"?*"</b>: Weighted proportion: resultSize = totalAvailableSize * ? / (? + 1).<br/>
    /// <b>"?"</b>: Equals <b>"?px"</b>.<br/>
    /// <b>"?px"</b>: ? device-independent units (1/96th inch per unit).<br/>
    /// <b>"?in"</b>: ? inch; 1in==96px.<br/>
    /// <b>"?cm"</b>: ? centimeters; 1cm==(96/2.54)px.<br/>
    /// <b>"?pt"</b>: ? points; 1pt==(96/72)px.<br/>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(SplitterDistanceConverter))]
    public struct SplitterDistance : IEquatable<SplitterDistance>
    {
        private double _value;
        private SplitterUnitType _unitType;
        private static readonly SplitterDistance s_autoPixel = new SplitterDistance(double.NaN, SplitterUnitType.Pixel);
        private static readonly SplitterDistance s_autoStar = new SplitterDistance(double.NaN, SplitterUnitType.Star);

        /// <summary>Initializes a new instance of the <see cref="SplitterDistance"/> class.</summary>
        /// <param name="pixels">The number of device-independent pixels (96 pixels-per-inch).</param>
        /// <exception cref="ArgumentException"><paramref name="pixels"/> is negtive, or equal to <see cref="Double.NegativeInfinity"/> or <see cref="Double.PositiveInfinity"/>.</exception>
        public SplitterDistance(double pixels)
            : this(pixels, SplitterUnitType.Pixel)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SplitterDistance"/> class.</summary>
        /// <param name="value">The initial value of this instance of <see cref="SplitterDistance"/>.</param>
        /// <param name="unitType">The <see cref="SplitterUnitType"/> held by this instance of <see cref="SplitterDistance"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="value"/> is negtive, or equal to <see cref="Double.NegativeInfinity"/> or <see cref="Double.PositiveInfinity"/>.</exception>
        public SplitterDistance(double value, SplitterUnitType unitType)
        {
            if (double.IsInfinity(value) || value < 0)
                throw new ArgumentException(SR.SplitterDistance_Constructor_InvalidValue, "value");

            if (unitType != SplitterUnitType.Pixel && unitType != SplitterUnitType.Star)
                throw new ArgumentException(SR.SplitterDistance_Constructor_InvalidUnitType, "unitType");

            _value = value;
            _unitType = unitType;
        }

        /// <exclude/>
        public static bool operator ==(SplitterDistance value1, SplitterDistance value2)
        {
            return ((value1.UnitType == value2.UnitType) && (value1.Value == value2.Value));
        }

        /// <exclude/>
        public static bool operator !=(SplitterDistance value1, SplitterDistance value2)
        {
            if (value1.UnitType == value2.UnitType)
                return (value1.Value != value2.Value);

            return true;
        }

        /// <exclude/>
        public override bool Equals(object obj)
        {
            if (obj is SplitterDistance)
            {
                SplitterDistance distance = (SplitterDistance)obj;
                return (this == distance);
            }
            return false;
        }

        /// <exclude/>
        public bool Equals(SplitterDistance other)
        {
            return (this == other);
        }

        /// <exclude/>
        public override int GetHashCode()
        {
            return (int)_value + (int)_unitType;
        }

        /// <summary>Gets a value that indicates whether the <see cref="SplitterDistance"/> holds a value that is expressed in pixels.</summary>
        /// <value><see langword="true"/> if the <see cref="UnitType"/> property is <see cref="SplitterUnitType.Pixel"/>; otherwise, <see langword="false"/>.</value>
        public bool IsAbsolute
        {
            get { return _unitType == SplitterUnitType.Pixel; }
        }

        /// <summary>Gets a value that indicates whether the <see cref="SplitterDistance"/> holds a value whose size is determined by the size properties of the content object.</summary>
        /// <value><see langword="true"/> if the <see cref="Value"/> property is <see cref="Double.NaN"/>; otherwise, <see langword="false"/>.</value>
        public bool IsAuto
        {
            get { return double.IsNaN(_value); }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="SplitterDistance"/> holds a value whose
        /// size is determined by the size properties of the content object, and expressed in pixels.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="Value"/> property is <see cref="Double.NaN"/>
        /// and <see cref="UnitType"/> property is <see cref="SplitterUnitType.Pixel"/>; otherwise, <see langword="false"/>.</value>
        public bool IsAutoPixel
        {
            get { return IsAuto && _unitType == SplitterUnitType.Pixel; }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="SplitterDistance"/> holds a value whose
        /// size is determined by the desired size of the content object, and expressed
        /// as a weighted proportion of available space.
        /// </summary>
        /// <value><see langword="true"/> if the <see cref="Value"/> property is <see cref="Double.NaN"/>
        /// and <see cref="UnitType"/> property is <see cref="SplitterUnitType.Star"/>; otherwise, <see langword="false"/>.</value>
        public bool IsAutoStar
        {
            get { return IsAuto && _unitType == SplitterUnitType.Star; }
        }

        /// <summary>Gets a value that indicates whether the <see cref="SplitterDistance"/> holds a value that is expressed as a weighted proportion of available space.</summary>
        /// <value><see langword="true"/> if the <see cref="UnitType"/> property is <see cref="SplitterUnitType.Star"/>; otherwise, <see langword="false"/>.</value>
        public bool IsStar
        {
            get { return _unitType == SplitterUnitType.Star; }
        }

        /// <summary>Gets a <see cref="Double"/> that represents the value of the <see cref="SplitterDistance"/>.</summary>
        /// <value>
        /// A <see cref="Double"/> that represents the value of the current instance.
        /// <see cref="Double.NaN"/> indicates size is determined by the desired size of the content object.
        /// </value>
        public double Value
        {
            get { return _value; }
        }

        /// <summary>Gets the associated <see cref="SplitterUnitType"/> for the <see cref="SplitterDistance"/>.</summary>
        /// <value>One of the <see cref="SplitterUnitType"/> values.</value>
        public SplitterUnitType UnitType
        {
            get { return _unitType; }
        }

        /// <exclude/>
        public override string ToString()
        {
            return SplitterDistanceConverter.ToString(this, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets an instance of <see cref="SplitterDistance"/> that holds a value whose size is determined
        /// by the size of the content object, and expressed in pixels.
        /// </summary>
        /// <value>
        /// A instance of <see cref="SplitterDistance"/> whose <see cref="Value"/> property is set <see cref="Double.NaN"/>
        /// and <see cref="UnitType"/> property is set to <see cref="SplitterUnitType.Pixel"/>.
        /// </value>
        public static SplitterDistance AutoPixel
        {
            get { return s_autoPixel; }
        }

        /// <summary>
        /// Gets an instance of <see cref="SplitterDistance"/> that holds a value whose size is determined
        /// by the size of the content object, and expressed as a weighted proportion of available space.
        /// </summary>
        /// <value>
        /// A instance of <see cref="SplitterDistance"/> whose <see cref="Value"/> property is set <see cref="Double.NaN"/>
        /// and <see cref="UnitType"/> property is set to <see cref="SplitterUnitType.Star"/>.
        /// </value>
        public static SplitterDistance AutoStar
        {
            get { return s_autoStar; }
        }
    }
}
