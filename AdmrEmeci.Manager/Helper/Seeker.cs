using System.Collections.Generic;
using AdmrEmeci.Manager.Models;
using System.Linq;

namespace AdmrEmeci.Manager.Helper
{
    public class Seeker
    {
        public enum TypeOfSearch
        {
            byName,
            byNumber
        }

        public static List<SeekerModel> Autocomplete(TypeOfSearch typeSearch, string prefix)
        {
            Entities dB = new Entities();
            return (typeSearch == TypeOfSearch.byName ? ByName(prefix) : ByNumber(prefix));
        }

        static List<SeekerModel> ByName(string prefix)
        {
            return new Entities().Registro
                .Where(x => x.Tipo == "P" && (x.Nombre + " " + x.Apellido).StartsWith(prefix))
                .Select(x => new SeekerModel
                {
                    Name = x.Nombre + " " + x.Apellido,
                    Value = x.Emeci
                }).Take(10).ToList();
        }

        static List<SeekerModel> ByNumber(string prefix)
        {
            return new Entities().Registro
                .Where(x => x.Tipo == "P" && x.Emeci.StartsWith(prefix))
                .Select(x => new SeekerModel
                {
                    Name = x.Emeci,
                    Value = x.Emeci
                }).Take(10).ToList();
        }
    }
}
