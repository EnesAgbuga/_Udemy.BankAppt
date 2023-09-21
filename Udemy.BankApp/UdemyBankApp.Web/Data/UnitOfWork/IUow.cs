using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyBankApp.Web.Data.Interfaces;

namespace UdemyBankApp.Web.Data.UnitOfWork
{
   public interface IUow
    {
        IRepository<T> GetRepository<T>() where T : class, new();

        void SaveChanges();
    }
}
