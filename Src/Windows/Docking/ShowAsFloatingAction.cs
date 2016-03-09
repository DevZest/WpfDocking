using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking
{
    /// <summary>Represents the action to show source <see cref="DockItem"/> as floating window.</summary>
    /// <remarks>This show action corresponds <see cref="DockItem.Show(DockControl, Rect, DockItemShowMethod)"/> method.</remarks>
    public sealed class ShowAsFloatingAction : ShowAction
    {
        private double _left = double.NaN;
        private double _top = double.NaN;
        private double _width = double.NaN;
        private double _height = double.NaN;

        /// <summary>Initializes a new instance of the <see cref="ShowAsFloatingAction"/> class.</summary>
        public ShowAsFloatingAction()
        {
        }

        internal ShowAsFloatingAction(int source, Rect floatingWindowBounds, DockItemShowMethod showMethod)
            : base(source, showMethod)
        {
            _left = floatingWindowBounds.Left;
            _top = floatingWindowBounds.Top;
            _width = floatingWindowBounds.Width;
            _height = floatingWindowBounds.Height;
        }

        /// <summary>Gets or sets the position of the floating window's left edge, in relation to the desktop. </summary>
        /// <value>The position of the floating window's left edge, in logical units (1/96th of an inch), the default is system default value <see cref="Double.NaN"/>.</value>
        [TypeConverter(typeof(LengthConverter))]
        [DefaultValue(double.NaN)]
        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>Gets or sets the position of the floating window's top edge, in relation to the desktop. </summary>
        /// <value>The position of the floating window's top edge, in logical units (1/96th of an inch), the default is system default value <see cref="Double.NaN"/>.</value>
        [TypeConverter(typeof(LengthConverter))]
        [DefaultValue(double.NaN)]
        public double Top
        {
            get { return _top; }
            set { _top = value; }
        }

        /// <summary>Gets or sets the width of the floating window. </summary>
        /// <value>The width of the floating window, in logical units (1/96th of an inch).
        /// The default value is <see cref="Double.NaN"/>, automatically sizes itself to fit the width of
        /// its content.</value>
        [TypeConverter(typeof(LengthConverter))]
        [DefaultValue(double.NaN)]
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>Gets or sets the height of the floating window. </summary>
        /// <value>The height of the floating window, in logical units (1/96th of an inch).
        /// The default value is <see cref="Double.NaN"/>, automatically sizes itself to fit the height of
        /// its content.</value>
        [TypeConverter(typeof(LengthConverter))]
        [DefaultValue(double.NaN)]
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        internal override DockTreePosition? GetDockTreePosition(DockControl dockControl)
        {
            return DockTreePosition.Floating;
        }

        internal override bool GetIsAutoHide(DockControl dockControl)
        {
            return false;
        }

        internal sealed override void Run(DockItem dockItem, DockControl dockControl)
        {
            dockItem.Show(dockControl, new Rect(Left, Top, Width, Height), ShowMethod);
        }
    }
}
