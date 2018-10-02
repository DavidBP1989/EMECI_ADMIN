using System;

namespace AdmrEmeci.Manager.Models
{
    public class PatientModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Emeci { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? RenovationDate { get; set; }
        public StatusDate StatusDateCard { get; set; }

        public enum StatusDate
        {
            Vencida,
            Vigente
        }
    }
}
