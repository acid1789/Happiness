using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class ContentManager
    {
        public abstract T Load<T>(string fileName);
    }
}
