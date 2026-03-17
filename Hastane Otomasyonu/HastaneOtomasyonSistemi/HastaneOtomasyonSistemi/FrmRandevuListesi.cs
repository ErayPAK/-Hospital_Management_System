using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HastaneOtomasyonSistemi
{
    public partial class FrmRandevuListesi : Form
    {
        public FrmRandevuListesi()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // BAĞLANTI NESNESİ
        // ---------------------------------------------------------
        SqlBaglantisi bgl = new SqlBaglantisi();

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmRandevuListesi_Load(object sender, EventArgs e)
        {
            // ADIM 1: SANAL TABLO OLUŞTURMA
            // Veritabanından gelecek verileri hafızada tutmak için bir sanal tablo oluşturuyoruz.
            DataTable dt = new DataTable();

            // ADIM 2: VERİLERİ ÇEKME (VIEW KULLANIMI)
            // BURASI ÇOK ÖNEMLİ:
            // Normalde randevu bilgilerini ve hasta adını yan yana göstermek için
            // "Select * From Tbl_Randevular INNER JOIN Tbl_Hastalar ON..." gibi uzun bir sorgu yazmamız gerekirdi.
            // Ancak bu işlem SQL tarafında 'View_RandevuDetay' adıyla hazırladığım sanal tabloyla yapıldı.
            // Bu sayede C# tarafında kodumuz çok sade ve temiz kaldı.

            SqlDataAdapter da = new SqlDataAdapter("Select * From View_RandevuDetay", bgl.baglanti());

            // ADIM 3: DOLDURMA VE GÖSTERME
            da.Fill(dt); // View'den gelen birleştirilmiş veriyi sanal tabloya doldur.
            dataGridView1.DataSource = dt; // Tabloyu ekradaki DataGridView'e  bağla.
        }
    }
}