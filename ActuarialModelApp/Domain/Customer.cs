namespace ActuarialModelApp.Domain
{
    public class Customer
    {
        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
    }
}
