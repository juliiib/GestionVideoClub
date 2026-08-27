namespace GestionVideoClub.Models
{
    public class Employee : Person
    {
        public string Shift {  get; private set; }
        public decimal Salary { get; private set; }
        
        public Employee(string name, string lastName, int dni, string phone, string address, string shift, decimal salary)
            : base(name, lastName, dni, phone, address)
        {  
            
            if (string.IsNullOrWhiteSpace(shift))
            {
                throw new ArgumentException("Shift cannot be null or empty.", nameof(shift));
            }

            if (salary <= 0) {
                throw new ArgumentOutOfRangeException(nameof(salary), "Salary must be greater than zero.");
            }

            Shift = shift;
            Salary = salary;
        }

        public void UpdateShift(string newShift)
        {
            if (string.IsNullOrWhiteSpace(newShift))
            {
                throw new ArgumentException("New shift cannot be null or empty.", nameof(newShift));
            }
            Shift = newShift;
        }

        public void UpdateSalary(decimal newSalary)
        {
            if (newSalary <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newSalary), "New salary must be greater than zero.");
            }
            Salary = newSalary;
        }
    }
}

