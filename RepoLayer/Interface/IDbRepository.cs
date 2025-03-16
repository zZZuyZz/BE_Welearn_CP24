using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interface
{
    public interface IDbRepository
    {
        public Task Nuke();
        public Task Nuke2();
    }
}
