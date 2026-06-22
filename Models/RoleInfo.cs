namespace QLXeMay.Models
{
    internal sealed class RoleInfo
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
