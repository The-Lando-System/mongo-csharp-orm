using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoOrm
{
    public interface IMongoRepository<T>
    {
        T Add(T item);

        bool Delete(string id);

        IEnumerable<T> FindAll();

        IEnumerable<T> FindAllByProperty(Dictionary<string, string> keyValuePairs);

        T FindById(string id);

        bool Update(string id, T item);
    }
}
