namespace Music_Store.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MusicRecordType")]
    public partial class MusicRecordType : BaseEntity
    {
        private string title;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MusicRecordType()
        {
            MusicRecord = new HashSet<MusicRecord>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get => title; set { title = value; OnPropertyChanged(); } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MusicRecord> MusicRecord { get; set; }
    }
}
