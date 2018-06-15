using PagedList;
using System;

namespace AdmrEmeci.Models
{
    public class PatientList
    {
        public PagedList<ListOfPatient> LPatient { get; set; }
        public string CardNumber { get; set; }
        public string CardNumberSelected { get; set; }
        public bool Error { get; set; } = false;
    }

    public class ListOfPatient
    {
        public string PatientName { get; set; }
        public string PatientLastName { get; set; }
        public string EMECI { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? DueDate { get; set; }
    }
}