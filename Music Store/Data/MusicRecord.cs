namespace Music_Store.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("MusicRecord")]
    public partial class MusicRecord : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MusicRecord()
        {
            MusicRecordInDelivery = new List<MusicRecordInDelivery>();
            MusicRecordInOrder = new List<MusicRecordInOrder>();
            Genre = new List<Genre>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Column(TypeName = "date")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Label { get; set; }

        [Required]
        public string Performers { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public int CountInStock { get; set; }

        [Column(TypeName = "text")]
        public string TreckList { get; set; }

        [Column(TypeName = "image")]
        [Required]
        public byte[] Image { get; set; }

        public string CountInHall => MusicRecordsInTheHall?.Count > 0 ? MusicRecordsInTheHall.Count.ToString() + " шт." : "Распродано";
        public int CountInHallInt => MusicRecordsInTheHall == null ? 0 : MusicRecordsInTheHall.Count;
        public string CountInStockText => CountInStock > 0 ? CountInStock.ToString() + " шт." : "Закончились";
        public string GenresString
        {
            get
            {
                string genres = string.Empty;
                if (Genre.Count > 0)
                {
                    foreach (var item in Genre)
                    {
                        if (item != Genre.LastOrDefault())
                        {
                            genres += $"{item.Title} / ";
                        }
                        else
                        {
                            genres += item.Title;
                        }
                    }
                }
                return genres;
            }
        }
        public int TypeId { get; set; }

        public virtual MusicRecordType MusicRecordType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MusicRecordInDelivery> MusicRecordInDelivery { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MusicRecordInOrder> MusicRecordInOrder { get; set; }

        public virtual MusicRecordsInTheHall MusicRecordsInTheHall { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Genre> Genre { get; set; }
    }
}
