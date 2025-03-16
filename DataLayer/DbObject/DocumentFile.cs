using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.DbObject;

public class DocumentFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string HttpLink { get; set; }

    [ForeignKey("AccountId")]
    public int AccountId { get; set; } 
    public Account Account { get; set; }

    public Boolean Approved { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; } = true;
    #region Group
    public int GroupId { get; set; }
    public Group Group { get; set; }
    #endregion
    public virtual ICollection<Report> ReportedReports { get; set; } = new Collection<Report>();

}