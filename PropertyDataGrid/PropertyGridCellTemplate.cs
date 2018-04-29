using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace PropertyDataGrid
{
    [ContentProperty("DataTemplate")]
    public class PropertyGridCellTemplate
    {
        public DataTemplate DataTemplate { get; set; }  // note: may be null
    }
}
