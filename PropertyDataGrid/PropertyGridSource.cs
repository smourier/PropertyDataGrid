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
        public PropertyGridSource(PropertyGrid grid, object selectedObject)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            if (selectedObject == null)
                throw new ArgumentNullException(nameof(selectedObject));

            Grid = grid;
            SelectedObject = selectedObject;
        }

        public PropertyGrid Grid { get; }
        public object SelectedObject { get; }
        bool IListSource.ContainsListCollection => false;
        IList IListSource.GetList() => NewTypeDescriptor().Properties;

        public virtual PropertyGridTypeDescriptor NewTypeDescriptor() => new PropertyGridTypeDescriptor(this);
    }
}
