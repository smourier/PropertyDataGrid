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
    [TemplatePart(Name = PartDataGridName, Type = typeof(DataGrid))]
    public class PropertyGrid : Control
    {
        public const string PartDataGridName = "PART_DataGrid";
        public const string CategoryPropertyName = "Category";

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
            PropertiesSource = new CollectionViewSource();
            PropertiesSource.GroupDescriptions.Add(new PropertyGroupDescription(CategoryPropertyName));
        }

        public CollectionViewSource PropertiesSource { get; }
        public DataGrid InnerGrid { get; private set; }
        public object SelectedObject { get => GetValue(SelectedObjectProperty); set => SetValue(SelectedObjectProperty, value); }

        public bool GroupByCategory
        {
            get => PropertiesSource.GroupDescriptions.OfType<PropertyGroupDescription>().Any(g => g.PropertyName == CategoryPropertyName);
            set
            {
                if (value)
                {
                    if (GroupByCategory)
                        return;

                    PropertiesSource.GroupDescriptions.Add(new PropertyGroupDescription(CategoryPropertyName));
                }
                else
                {
                    if (!GroupByCategory)
                        return;

                    foreach (var gp in PropertiesSource.GroupDescriptions.OfType<PropertyGroupDescription>().Where(g => g.PropertyName == CategoryPropertyName).ToArray())
                    {
                        PropertiesSource.GroupDescriptions.Remove(gp);
                    }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InnerGrid = GetTemplateChild(PartDataGridName) as DataGrid;
            if (InnerGrid == null)
                throw new PropertyGridException("0002: Cannot find a DataGrid template child named '" + PartDataGridName + "'.");

            PropertiesSource.Source = NewPropertyGridSource(SelectedObject);
            var binding = new Binding();
            binding.Source = PropertiesSource;
            InnerGrid.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        public virtual PropertyGridSource NewPropertyGridSource(object selectedObject) => new PropertyGridSource(this, selectedObject);

        protected virtual void SelectedObjectPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            //var gs = NewPropertyGridSource(e.NewValue);
            //PropertiesSource.Source = gs;
        }
    }
}
