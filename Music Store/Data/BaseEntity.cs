using Music_Store.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Music_Store.Data
{
    public class BaseEntity : NotifyPropertyChangedBase
    {      
        public object GetProperty(string property)
        {
            if (property != null)
            {
                return GetType().GetProperty(property).GetValue(this);
            }
            else return null;
        }
    }
}
