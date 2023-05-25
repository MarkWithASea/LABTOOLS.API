using System.ComponentModel.DataAnnotations;

namespace LABTOOLS.API.Models
{
    public interface IEntity
    {
        [Key]
        public int Id { get; set; }
    } 
}