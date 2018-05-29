using PagedList;

namespace AdmrEmeci.Models
{
    public class DoctorList
    {
        public PagedList<ListOfDoctor> LDoctor { get; set; }
        public string CardNumber { get; set; }
        public string CardNumberSelected { get; set; }
        public bool Error { get; set; } = false;
    }

    public class ListOfDoctor
    {
        public string DoctorName { get; set; }
        public string DoctorLastName { get; set; }
        public string EMECI { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
    }
}