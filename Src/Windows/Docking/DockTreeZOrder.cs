using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace DevZest.Windows.Docking
{
    /// <summary>Describes the z-order of left, right, top and bottom <see cref="DockTree"/> objects for <see cref="DockControl"/>.</summary>
    /// <remarks>The <see cref="DockTreeZOrder"/> has <see cref="First"/>, <see cref="Second"/>, <see cref="Third"/> and <see cref="Fourth"/>
    /// properties to determine the z-order of the dock trees. The <see cref="Fourth"/> dock tree position is on the top of z-order.</remarks>
    [StructLayout(LayoutKind.Sequential), TypeConverter(typeof(DockTreeZOrderConverter))]
    public struct DockTreeZOrder
    {
        private static DockTreeZOrder s_default = new DockTreeZOrder(Dock.Left, Dock.Right, Dock.Top, Dock.Bottom);

        private bool _initialized;
        private Dock _first;
        private Dock _second;
        private Dock _third;
        private Dock _fourth;
        
        /// <summary>Gets the default value for the <see cref="DockTreeZOrder"/>.</summary>
        /// <value>The default value for the <see cref="DockTreeZOrder"/>, which is "Left, Right, Top, Bottom".</value>
        public static DockTreeZOrder Default
        {
            get { return s_default; }
        }

        /// <summary>Initialize a new instance of <see cref="DockTreeZOrder"/> structure.</summary>
        /// <param name="first">The first dock tree position.</param>
        /// <param name="second">The second dock tree position.</param>
        /// <param name="third">The third dock tree position.</param>
        /// <param name="fourth">The fourth dock tree position.</param>
        /// <remarks>The specified fourth dock tree position is on the top of z-order.</remarks>
        public DockTreeZOrder(Dock first, Dock second, Dock third, Dock fourth)
        {
            int firstInvalid = FindFirstInvalidValue(first, second, third, fourth);
            if (firstInvalid == 0)
                throw new ArgumentException(SR.Exception_DockTreeZOrder_InvalidDockValue, "first");
            if (firstInvalid == 1)
                throw new ArgumentException(SR.Exception_DockTreeZOrder_InvalidDockValue, "second");
            if (firstInvalid == 2)
                throw new ArgumentException(SR.Exception_DockTreeZOrder_InvalidDockValue, "third");
            if (firstInvalid == 3)
                throw new ArgumentException(SR.Exception_DockTreeZOrder_InvalidDockValue, "fourth");

            _first = first;
            _second = second;
            _third = third;
            _fourth = fourth;
            _initialized = true;
        }

        /// <summary>Gets or sets the first dock tree position.</summary>
        /// <value>The first dock tree position.</value>
        public Dock First
        {
            get { return _initialized ? _first : Default.First; }
        }

        /// <summary>Gets or sets the second dock tree position.</summary>
        /// <value>The second dock tree position.</value>
        public Dock Second
        {
            get { return _initialized ? _second : Default.Second; }
        }

        /// <summary>Gets or sets the third dock tree position.</summary>
        /// <value>The third dock tree position.</value>
        public Dock Third
        {
            get { return _initialized ? _third : Default.Third; }
        }

        /// <summary>Gets or sets the fourth dock tree position.</summary>
        /// <value>The fourth dock tree position.</value>
        public Dock Fourth
        {
            get { return _initialized? _fourth : Default.Fourth; }
        }

        /// <summary>Returns the index of the z-order for the specified dock tree position.</summary>
        /// <param name="position">The specified dock tree position.</param>
        /// <returns>The zero based index of the z-order.</returns>
        public int IndexOf(Dock position)
        {
            if (First == position)
                return 0;
            else if (Second == position)
                return 1;
            else if (Third == position)
                return 2;
            else if (Fourth == position)
                return 3;
            else
                return -1;

        }

        /// <summary>Gets the dock tree position at the given zero-based index of z-order.</summary>
        /// <param name="index">The given zero-based index of z-order.</param>
        /// <returns>The dock tree position.</returns>
        public Dock this[int index]
        {
            get
            {
                if (index == 0)
                    return First;
                else if (index == 1)
                    return Second;
                else if (index == 2)
                    return Third;
                else if (index == 3)
                    return Fourth;
                else
                    throw new ArgumentOutOfRangeException("index");
            }
        }

        internal static bool CheckValues(Dock first, Dock second, Dock third, Dock fourth)
        {
            return FindFirstInvalidValue(first, second, third, fourth) == -1;
        }

        private static int FindFirstInvalidValue(Dock first, Dock second, Dock third, Dock fourth)
        {
            if (!IsValidDockValue(first))
                return 0;
            if (!IsValidDockValue(second) || second == first)
                return 1;
            if (!IsValidDockValue(third) || third == first || third == second)
                return 2;
            if (!IsValidDockValue(fourth) || fourth == first || fourth == second || fourth == third)
                return 3;

            return -1;
        }

        private static bool IsValidDockValue(Dock dock)
        {
            return (dock == Dock.Left || dock == Dock.Right || dock == Dock.Top || dock == Dock.Bottom);
        }

        /// <exclude/>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}, {1}, {2}, {3}", First, Second, Third, Fourth);
        }

        /// <exclude/>
        public override int GetHashCode()
        {
            return (int)First + (int)Second << 3 + (int)Third << 6 + (int)Fourth << 9;
        }

        /// <exclude/>
        public override bool Equals(object obj)
        {
            if (!(obj is DockTreeZOrder))
                return false;

            return Equals((DockTreeZOrder)obj);
        }

        /// <exclude/>
        public bool Equals(DockTreeZOrder value)
        {
            return First == value.First && Second == value.Second && Third == value.Third && Fourth == value.Fourth;
        }

        /// <exclude/>
        public static bool operator ==(DockTreeZOrder value1, DockTreeZOrder value2)
        {
            return value1.Equals(value2);
        }

        /// <exclude/>
        public static bool operator !=(DockTreeZOrder value1, DockTreeZOrder value2)
        {
            return !value1.Equals(value2);
        }

        /// <summary>Returns the dock tree z-order after brings the specified dock tree position to front.</summary>
        /// <param name="dock">The specified dock tree position.</param>
        /// <returns>The dock tree z-order.</returns>
        public DockTreeZOrder BringToFront(Dock dock)
        {
            if (First == dock)
                return new DockTreeZOrder(Second, Third, Fourth, First);
            else if (Second == dock)
                return new DockTreeZOrder(First, Third, Fourth, Second);
            else if (Third == dock)
                return new DockTreeZOrder(First, Second, Fourth, Third);
            else if (Fourth == dock)
                return new DockTreeZOrder(First, Second, Third, Fourth);
            else
                throw new ArgumentException(SR.Exception_DockTreeZOrder_InvalidDockValue, "dock");
        }

        /// <summary>Returns the dock tree z-order after sends the specified dock tree position to back.</summary>
        /// <param name="dock">The specified dock tree position.</param>
        /// <returns>The dock tree z-order.</returns>
        public DockTreeZOrder SendToBack(Dock dock)
        {
            if (First == dock)
                return new DockTreeZOrder(First, Second, Third, Fourth);
            else if (Second == dock)
                return new DockTreeZOrder(Second, First, Third, Fourth);
            else if (Third == dock)
                return new DockTreeZOrder(Third, First, Second, Fourth);
            else if (Fourth == dock)
                return new DockTreeZOrder(Fourth, First, Second, Third);
            else
                throw new ArgumentException(SR.Exception_DockTreeZOrder_InvalidDockValue, "dock");
        }
    }
}
