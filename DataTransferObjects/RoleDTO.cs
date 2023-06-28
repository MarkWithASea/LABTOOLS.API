using LABTOOLS.API.Models;

namespace LABTOOLS.API.DataTransferObjects
{
    public class RoleDTO : IDataTransferObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}