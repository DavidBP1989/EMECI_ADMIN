using PagedList;
using AdmrEmeci.Manager.Models;
using AdmrEmeci.Manager.Manager;

namespace AdmrEmeci.Models
{
    public class ListOfPatientModel
    {
        //-->PagedList
        public PagedList<PatientModel> Patients { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        //<!--PagedList
        public string EmeciSelected { get; set; }
        public bool IsError { get; set; } = false;
        public string Error { get; set; }

        public void GetAllPatients(string emeci = "")
        {
            var patients = emeci != string.Empty ? Patient.GetPatients(emeci) : Patient.GetPatients();
            PagedList<PatientModel> paged = new PagedList<PatientModel>(patients, Page, PageSize);
            Patients = paged;
        }
    }
}