using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnCN_Net.BLL
{
    public class DatSanhBLL
    {
        DatSanhDAL dal = new DatSanhDAL();

        public List<DatSanh> GetAll()
        {
            return dal.GetAll();
        }

        public List<DatSanh> GetTopDatSanh()
        {
            return dal.GetTopDatSanh();
        }

        public bool Insert(DatSanh ds)
        {
            if (ds == null)
                return false;

            if (string.IsNullOrEmpty(ds.TenSuKien))
                return false;

            if (ds.GioBatDau >= ds.GioKetThuc)
                return false;

            return dal.Insert(ds);
        }

        public bool Update(DatSanh ds)
        {
            if (ds == null)
                return false;

            return dal.Update(ds);
        }

        public bool Delete(int maDat)
        {
            if (maDat <= 0)
                return false;

            return dal.Delete(maDat);
        }

        public DatSanh FindById(int maDat)
        {
            return dal.FindById(maDat);
        }
        public List<DatSanh> GetByKhachHang(int maKhachHang)
        {
            return dal.GetByKhachHang(maKhachHang);
        }
    }
}