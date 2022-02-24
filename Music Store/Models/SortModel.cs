using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Store.Models
{
    public class SortModel : NotifyPropertyChangedBase
    {
        private bool isAscending;
        public SortModel(string title, string property, bool isAscending)
        {
            Title = title;
            Property = property;
            this.isAscending = isAscending;
        }

        public string Title { get; set; }
        public string Property { get; set; }
        public bool IsAscending { get => isAscending; set { isAscending = value; OnPropertyChanged(); } }
    }
}
