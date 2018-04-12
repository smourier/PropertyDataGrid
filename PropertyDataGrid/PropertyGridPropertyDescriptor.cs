using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDataGrid.Utilities;

namespace PropertyDataGrid
{
    public class PropertyGridPropertyDescriptor : DictionaryObject
    {
        public PropertyGridPropertyDescriptor(PropertyGridTypeDescriptor typeDescriptor, string name)
        {
            if (typeDescriptor == null)
                throw new ArgumentNullException(nameof(typeDescriptor));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            TypeDescriptor = typeDescriptor;
            Name = name;
        }

        public PropertyGridTypeDescriptor TypeDescriptor { get; }
        public string Name { get; }
        public virtual bool IsReadOnly { get => DictionaryObjectGetPropertyValue<bool>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual bool IsBrowsable { get => DictionaryObjectGetPropertyValue<bool>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual string Category { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual string Description { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual string DisplayName { get => DictionaryObjectGetPropertyValue<string>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual bool HasDefaultValue { get => DictionaryObjectGetPropertyValue<bool>(); set => DictionaryObjectSetPropertyValue(value); }
        public virtual object DefaultValue { get => DictionaryObjectGetPropertyValue<object>(); set => DictionaryObjectSetPropertyValue(value); }
    }
}
