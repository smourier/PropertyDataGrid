using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PropertyDataGrid
{
    [TemplatePart(Name = "PART_DataGrid", Type = typeof(DataGrid))]
    public class PropertyGrid : Control
    {
        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                (source, e) => ((PropertyGrid)source).SelectedObjectPropertyChanged(source, e)));

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid()
        {
            PropertiesSource = (CollectionViewSource)FindResource(nameof(PropertiesSource));
        }

        public CollectionViewSource PropertiesSource { get; }
        public object SelectedObject { get => GetValue(SelectedObjectProperty); set => SetValue(SelectedObjectProperty, value); }

        protected virtual void SelectedObjectPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
        }

        public override void OnApplyTemplate() => base.OnApplyTemplate();
    }
}
