using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(Source.SelectedObject))
            {
                var desc = NewPropertyDescriptor(descriptor.Name);
                desc.Category = descriptor.Category;
                desc.Description = descriptor.Description;
                desc.DisplayName = descriptor.DisplayName;
                desc.IsBrowsable = descriptor.IsBrowsable;
                desc.IsReadOnly = descriptor.IsReadOnly;
                desc.PropertyDescriptor = descriptor;

                var prop = new PropertyGridProperty(desc, Source.SelectedObject);
                Properties.Add(prop);
            }
        }
    }
}
