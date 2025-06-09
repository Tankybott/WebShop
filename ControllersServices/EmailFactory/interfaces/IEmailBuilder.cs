using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.EmailFactory.interfaces
{
    public interface IEmailBuilder<TInput>
    {
        string Build(TInput input);
    }
}
