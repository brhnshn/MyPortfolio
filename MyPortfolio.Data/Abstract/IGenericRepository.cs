using System.Collections.Generic;

namespace MyPortfolio.Data.Abstract
{
    public interface IGenericRepository<T> where T : class
    {
        List<T> GetList(); // Tüm listeyi getir
        T GetById(int id); // ID'ye göre tek kayıt getir
        void Insert(T t);  // Ekle
        void Delete(T t);  // Sil
        void Update(T t);  // Güncelle
    }
}