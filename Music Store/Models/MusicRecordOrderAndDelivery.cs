using Music_Store.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Store.Models
{
    public class MusicRecordOrderAndDelivery
    {
        public MusicRecordOrderAndDelivery(MusicRecord musicRecord, int count)
        {
            MusicRecord = musicRecord;
            Count = count;
        }

        public MusicRecord MusicRecord { get; set; }
        public int Count { get; set; }
    }
}
