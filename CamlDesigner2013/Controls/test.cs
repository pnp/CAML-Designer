using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace CamlDesigner2013.Controls
{
    public class test
    {
        public static ObservableCollection<test> createTests()
        {
            ObservableCollection<test> list = new ObservableCollection<test>();
            list.Add(new test("dummy1"));
            list.Add(new test("dummy2"));
            list.Add(new test("dummy3"));
            return list;
        }

        public test(string title)
        {
            this.Title = title;
        }

        public string Title { get; set; }
    }
}
