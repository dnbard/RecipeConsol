using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConsole
{
    public struct ParserInfo
    {
        public string url;
        
        private string _type;
        public string type
        {
            get { return _type; }
            set { _type = value.ToLowerInvariant(); }
        }
    }
}
