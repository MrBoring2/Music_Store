namespace Music_Store.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MusicRecordsInTheHall")]
    public partial class MusicRecordsInTheHall
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MusicRecordId { get; set; }

        public int Count { get; set; }

        public virtual MusicRecord MusicRecord { get; set; }
    }
}
