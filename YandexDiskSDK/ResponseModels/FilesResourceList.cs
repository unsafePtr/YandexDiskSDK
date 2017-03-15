using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK.ResponseModels
{
    public class FilesResourceList : IEnumerable<Resource>
    {
        public List<Resource> Items { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }

        public IEnumerator<Resource> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
