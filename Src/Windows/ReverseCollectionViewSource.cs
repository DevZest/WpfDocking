using System;
using System.Diagnostics;
using System.Collections;
using System.Windows;
using System.Windows.Data;

namespace DevZest.Windows
{
    /// <summary>Represents the Extensible Application Markup Language (XAML) proxy of the CollectionView class, in a reversed order.</summary>
    /// <remarks>The <see cref="ReverseCollectionViewSource"/> class only works when <see cref="CollectionViewSource.View"/> is <see cref="ListCollectionView"/>(<see cref="CollectionViewSource.Source"/> is <see cref="IList"/>). Otherwise, it works as base class <see cref="CollectionViewSource"/>.</remarks>
    /// <example>
    ///     <code lang="xaml" source="..\..\Samples\Common\CSharp\ReverseCollectionViewSourceSample\Window1.xaml" />
    /// </example>
    public class ReverseCollectionViewSource : CollectionViewSource
    {
        private class ReverseComparer : IComparer
        {
            private readonly IList _items;

            public ReverseComparer(IList items)
            {
                _items = items;
            }

            public int Compare(object x, object y)
            {
                return _items.IndexOf(y).CompareTo(_items.IndexOf(x));
            }
        }

        /// <exclude/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ViewProperty)
            {
                ListCollectionView view = e.NewValue as ListCollectionView;
                IList source = Source as IList;
                if (view != null && source != null)
                    view.CustomSort = new ReverseComparer(source);
            }
        }
    }
}
