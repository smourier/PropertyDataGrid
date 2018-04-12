using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyDataGrid
{
    public class PropertyGridSource : IListSource
    {
        public PropertyGridSource(object selectedObject)
        {
            if (selectedObject == null)
                throw new ArgumentNullException(nameof(selectedObject));

            SelectedObject = selectedObject;
        }

        public object SelectedObject { get; }
        bool IListSource.ContainsListCollection => false;
        IList IListSource.GetList() => throw new NotImplementedException();

        public virtual PropertyGridTypeDescriptor GetTypeDescriptor() => new PropertyGridTypeDescriptor(SelectedObject);
    }
}
