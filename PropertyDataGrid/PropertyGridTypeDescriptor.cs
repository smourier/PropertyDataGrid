using PropertyDataGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PropertyDataGrid
{
    public class PropertyGridTypeDescriptor
    {
        public PropertyGridTypeDescriptor(PropertyGridSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Source = source;
            Properties = new ObservableCollection<PropertyGridProperty>();
            AddProperties();
        }

        public PropertyGridSource Source { get; }
        public virtual ObservableCollection<PropertyGridProperty> Properties { get; }

        public virtual PropertyGridPropertyDescriptor NewPropertyDescriptor(string name) => new PropertyGridPropertyDescriptor(this, name);

        public virtual void AddProperties()
        {
            Properties.Clear();
            var list = new List<PropertyGridProperty>();
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Source.SelectedObject))
            {
                var desc = Convert(descriptor);
                if (desc == null)
                    continue;

                var prop = new PropertyGridProperty(desc, Source.SelectedObject);
                list.Add(prop);
            }
            list.Sort();

            foreach (var prop in list)
            {
                Properties.Add(prop);
            }
        }

        protected virtual PropertyGridPropertyDescriptor Convert(PropertyDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var desc = NewPropertyDescriptor(descriptor.Name);
            desc.Category = descriptor.Category.Nullify();
            desc.Description = descriptor.Description.Nullify();
            desc.DisplayName = descriptor.DisplayName.Nullify();
            if (desc.DisplayName == null || desc.DisplayName == desc.Name)
            {
                desc.DisplayName = Decamelizer.Decamelize(desc.Name);
            }
            desc.IsBrowsable = descriptor.IsBrowsable;
            desc.IsReadOnly = descriptor.IsReadOnly;
            desc.PropertyDescriptor = descriptor;
            return desc;
        }
    }
}
