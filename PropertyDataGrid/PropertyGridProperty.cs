using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDataGrid.Utilities;

namespace PropertyDataGrid
{
    public class PropertyGridProperty : ChangeTrackingDictionaryObject
    {
        public PropertyGridProperty(PropertyGridPropertyDescriptor descriptor, object selectedObject)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            Descriptor = descriptor;
            SelectedObject = selectedObject;

            Name = descriptor.Name;
            IsReadOnly = descriptor.IsReadOnly;
            IsBrowsable = descriptor.IsBrowsable;
            Category = descriptor.Category;
            DisplayName = descriptor.DisplayName;
            HasDefaultValue = descriptor.HasDefaultValue;
            DefaultValue = descriptor.DefaultValue;
            Value = descriptor.GetValue();
            DictionaryObjectCommitChanges();
        }

        public PropertyGridPropertyDescriptor Descriptor { get; }
        public string Name { get; }
        public object SelectedObject { get; }
        public virtual bool IsReadOnly { get => DictionaryObjectGetPropertyValue<bool>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual bool IsBrowsable { get => DictionaryObjectGetPropertyValue<bool>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual string Category { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual string Description { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual string DisplayName { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual bool HasDefaultValue { get => DictionaryObjectGetPropertyValue<bool>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual object DefaultValue { get => DictionaryObjectGetPropertyValue<object>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual object Value { get => DictionaryObjectGetPropertyValue<object>(); set => DictionaryObjectSetPropertyValue(value); }
    }
}
