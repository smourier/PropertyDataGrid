using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyDataGrid
{
    public class PropertyGridTypeDescriptor
    {
        public PropertyGridTypeDescriptor(object selectedObject)
        {
            if (selectedObject == null)
                throw new ArgumentNullException(nameof(selectedObject));

            SelectedObject = selectedObject;
        }

        public object SelectedObject { get; }

        public virtual PropertyGridPropertyDescriptor NewPropertyDescriptor(string name) => new PropertyGridPropertyDescriptor(this, name);

        public virtual IEnumerable<PropertyGridPropertyDescriptor> GetProperties()
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(SelectedObject))
            {
                var prop = NewPropertyDescriptor(descriptor.Name);
                prop.Category = descriptor.Category;
                prop.Description = descriptor.Description;
                prop.DisplayName = descriptor.DisplayName;
                prop.IsBrowsable = descriptor.IsBrowsable;
                prop.IsReadOnly = descriptor.IsReadOnly;
                yield return prop;
            }
        }
    }
}
