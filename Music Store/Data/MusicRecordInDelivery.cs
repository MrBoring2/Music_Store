namespace Music_Store.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MusicRecordInDelivery")]
    public partial class MusicRecordInDelivery
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MusicRecordId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DeliveryId { get; set; }

        public int CountInDelivry { get; set; }

        public virtual Delivery Delivery { get; set; }

        public virtual MusicRecord MusicRecord { get; set; }
    }
}
