using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevZest.Windows.Docking;

namespace DevZest.DockSample
{
    /// <summary>
    /// Interaction logic for Document.xaml
    /// </summary>
    partial class Document : ICustomTypeDescriptor
    {
        private class DocumentUndoRedoReference : IDockItemUndoRedoReference
        {
            private WeakReference _weakReference;
            private int _documentId;

            public DocumentUndoRedoReference(Document document)
            {
                _weakReference = new WeakReference(document);
                _documentId = document.DocumentId;
            }

            public DockItem DockItem
            {
                get
                {
                    if (!_weakReference.IsAlive)
                    {
                        Document document = new Document(_documentId);
                        _weakReference.Target = document;
                    }
                    return _weakReference.Target as DockItem;
                }
            }
        }

        private static PropertyDescriptorCollection s_propertyDescriptors;
        public static readonly DependencyProperty DocumentIdProperty = DependencyProperty.Register("DocumentId", typeof(int), typeof(Document),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDocumentIdChanged)));

        private static void OnDocumentIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Document document = (Document)d;
            int documentId = (int)e.NewValue;
            document.OnDocumentIdChanged(documentId);
        }

        private void OnDocumentIdChanged(int documentId)
        {
            TabText = string.Format("Document{0}", documentId);
            Title = Description = string.Format("Sample document{0}", documentId);
            paragraph.Inlines.Clear();
            paragraph.Inlines.Add(Title);
        }

        private DocumentUndoRedoReference _undoRedoReference;

        public Document()
        {
            InitializeComponent();
        }

        public Document(int documentId)
            : this()
        {
            DocumentId = documentId;
        }

        public int DocumentId
        {
            get { return (int)GetValue(DocumentIdProperty); }
            set { SetValue(DocumentIdProperty, value); }
        }

        protected override IDockItemUndoRedoReference UndoRedoReference
        {
            get
            { 
                if (_undoRedoReference == null)
                    _undoRedoReference = new DocumentUndoRedoReference(this);
                return _undoRedoReference;
            }
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return new AttributeCollection(null);
        }
        
        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }
        
        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }
        
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }
        
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return null;
        }
        
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return null;
        }
        
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return null;
        }
        
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(null);
        }
        
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return new EventDescriptorCollection(null);
        }
        
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            if (s_propertyDescriptors == null)
            {
                PropertyDescriptor[] properties = new PropertyDescriptor[1];
                properties[0] = DependencyPropertyDescriptor.FromProperty(DocumentIdProperty, typeof(Document));
                s_propertyDescriptors = new PropertyDescriptorCollection(properties, true);
            }
            return s_propertyDescriptors;
        }
        
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }
        
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }
}
