using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PropertyDataGrid
{
    public class PropertyGridCellTemplateSelector : DataTemplateSelector
    {
        public PropertyGridCellTemplateSelector()
        {
            DataTemplates = new ObservableCollection<PropertyGridCellTemplate>();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<PropertyGridCellTemplate> DataTemplates { get; private set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var property = item as PropertyGridProperty;
            if (property == null)
                return base.SelectTemplate(item, container);

            return base.SelectTemplate(item, container);
        }
    }
}
