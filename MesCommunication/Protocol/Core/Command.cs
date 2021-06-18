using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesCommunication.Protocol
{
    public abstract class Command : ICommand
    {
        /// <summary>
        /// Transaction ID
        /// </summary>
        [Deserialization(5,1)]
        public string trx_id { get; set; }

        /// <summary>
        /// Transmission type
        /// I:Input  O:Output
        /// </summary>
        [Deserialization(1,2)]
        public string type_id { get; set; }

    }
}
