using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformTool
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class CustomToolAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public CustomToolAttribute(String Name, String Description)
        {
            this.Name = Name;
            this.Description = Description;
        }
    }
}
