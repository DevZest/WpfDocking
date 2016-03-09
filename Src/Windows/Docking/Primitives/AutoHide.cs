using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace DevZest.Windows.Docking.Primitives
{
    /// <summary>Provides attached properties for <see cref="DockControl"/> object's auto-hide settings.</summary>
    public static class AutoHide
    {
        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.Animation"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the <see cref="AutoHideAnimation"/> value.</summary>
        /// <value>The <see cref="AutoHideAnimation"/> value.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty AnimationProperty = DependencyProperty.RegisterAttached("Animation", typeof(AutoHideAnimation), typeof(AutoHide),
            new FrameworkPropertyMetadata(AutoHideAnimation.Slide));

        /// <summary>Identifies the <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.AnimationDuration"/> attached property.</summary>
        /// <AttachedPropertyComments>
        /// <summary>Gets or sets the duration of the animation.</summary>
        /// <value>The duration of the animation.</value>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.RegisterAttached("AnimationDuration", typeof(Duration), typeof(AutoHide),
            new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(250))));

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.Animation" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.Animation" /> attached property.</returns>
        public static AutoHideAnimation GetAnimation(DockControl dockControl)
        {
            return (AutoHideAnimation)dockControl.GetValue(AnimationProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.Animation" /> attached property
        /// for a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetAnimation(DockControl dockControl, AutoHideAnimation value)
        {
            dockControl.SetValue(AnimationProperty, value);
        }

        /// <summary>Gets the value of <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.AnimationDuration" /> attached property
        /// from a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> from which to read the property value.</param>
        /// <returns>The value of <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.AnimationDuration" /> attached property.</returns>
        public static Duration GetAnimationDuration(DockControl dockControl)
        {
            return (Duration)dockControl.GetValue(AnimationDurationProperty);
        }

        /// <summary>Sets the value of <see cref="P:DevZest.Windows.Docking.Primitives.AutoHide.AnimationDuration" /> attached property
        /// for a given <see cref="DockControl" />.</summary>
        /// <param name="dockControl">The <see cref="DockControl"/> on which to set the property value.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetAnimationDuration(DockControl dockControl, Duration value)
        {
            dockControl.SetValue(AnimationDurationProperty, value);
        }
    }
}
