using Common.Enumerations;

namespace Domain.Entities
{
    public class ModuleActionRolePermission : BaseEntity
    {
        public int Id { get; set; }
        public ModuleGroup ModuleGroup { get; set; }
        public ModuleAction ModuleAction { get; set; }
        public string RoleId { get; set; }
        public UserType UserType { get; set; }

        public virtual Role Role { get; set; }
    }
}
