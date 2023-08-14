using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_most_complex_interpreted_language_yet
{
    public class Function
    {
        public string name;
        public Action action;

        public Function(string name, Action action)
        {
            this.name = name;
            this.action = action;
        }

        public void invoke()
        {
            action();
        }
    }

    public class Function<T1>
    {
        public string name;
        public Action<T1> action;
        public Dictionary<string, T1> variables = new Dictionary<string, T1>();

        public Function(string name, Action<T1> action, Dictionary<string, T1> vars)
        {
            this.name = name;
            this.action = action;
            variables = vars;
        }

        public void invoke(T1 one)
        {
            if (!variables.ContainsKey(one.ToString()))
            {
                action(one);
            } else
            {
                action(variables[one.ToString()]);
            }
        }
    }

    public class Function<T1, T2>
    {
        public string name;
        public Action<T1, T2> action;

        public Function(string name, Action<T1, T2> action)
        {
            this.name = name;
            this.action = action;
        }

        public void invoke(T1 one, T2 two)
        {
            action(one, two);
        }
    }
}
