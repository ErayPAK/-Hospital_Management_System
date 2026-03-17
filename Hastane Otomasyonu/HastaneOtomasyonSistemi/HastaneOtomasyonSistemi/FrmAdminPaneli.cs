using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // Veritabanı işlemleri için gerekli kütüphane
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HastaneOtomasyonSistemi
{
    public partial class FrmAdminPaneli : Form
    {
        // ---------------------------------------------------------
        // BAĞLANTI NESNESİ 
        // ---------------------------------------------------------
        // Veritabanı bağlantı adresini tutan sınıfımızı burada çağırıyoruz.
        // Bu nesne, SQL ile iletişim kurmamızı sağlayacak anahtardır.
        SqlBaglantisi bgl = new SqlBaglantisi();

        public FrmAdminPaneli()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN ÇALIŞACAK KODLAR
        // ---------------------------------------------------------
        // Admin paneli açıldığı anda istatistiklerin (Doktor sayısı, randevu sayısı vb.)
        // otomatik olarak ekrana gelmesini istiyoruz.
        private void FrmAdminPaneli_Load(object sender, EventArgs e)
        {
            // ADIM 1: KOMUTUN HAZIRLANMASI
            // Normal SQL sorgusu yazmak yerine, SQL tarafında hazırladığımız
            // 'sp_AdminPanelIstatistikleri' isimli Stored Procedure'ü çağırıyoruz.
            // Bu yöntemim kullanılmasının sebebi hem daha güvenli hem de daha performanslı olmasıdır.
            SqlCommand komut = new SqlCommand("sp_AdminPanelIstatistikleri", bgl.baglanti());

            // ADIM 2: KOMUT TİPİNİ BELİRTME
            // SQL'e diyoruz ki: "Sana gönderdiğim string, normal bir yazı değil;
            // veritabanında kayıtlı bir prosedürün adıdır."
            komut.CommandType = CommandType.StoredProcedure;

            // ADIM 3: VERİLERİ OKUMA
            // Veritabanından gelen sonuç tablosunu okumak için bir okuyucu oluşturur.
            SqlDataReader dr = komut.ExecuteReader();

            // ADIM 4: VERİLERİ ARAYÜZE AKTARMA
            // Eğer prosedürden geriye bir sonuç döndüyse çalışır.
            if (dr.Read())
            {
                // dr[0], dr[1] gibi indeksler, SQL prosedüründeki Select sırasına göredir.
                // SQL prosedüründe sıra şu şekilde: DoktorSayisi, HastaSayisi, RandevuSayisi, PopulerBrans döndürmüştük.

                LblDoktorSayisi.Text = dr[0].ToString();   // Toplam Doktor Sayısı
                LblHastaSayisi.Text = dr[1].ToString();    // Toplam Hasta Sayısı
                LblRandevuSayisi.Text = dr[2].ToString();  // Toplam Randevu Sayısı
                LblPopulerBrans.Text = dr[3].ToString();   // En Çok Randevu Alınan Branş
            }

            // ADIM 5: BAĞLANTIYI KAPATMA
            // İşimiz bittiğinde bağlantıyı kapatarak sunucuyu yormuyoruz.
            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // YÖNLENDİRME BUTONLARI
        // ---------------------------------------------------------

        // "Doktor İşlemleri" butonuna tıklandığında:
        private void BtnDoktorIslemleri_Click(object sender, EventArgs e)
        {
            // Doktor yönetim formundan yani FrmDoktorPaneli yeni bir nesne türetip açıyoruz.
            FrmDoktorPaneli fr = new FrmDoktorPaneli();
            fr.Show(); // Show() metodu, mevcut pencereyi kapatmadan yenisini açar.
        }

        // "Branş İşlemleri" butonuna tıklandığında:
        private void BtnBransIslemleri_Click(object sender, EventArgs e)
        {
            // Branş ekleme/silme formunu açar.
            FrmBransPaneli fr = new FrmBransPaneli();
            fr.Show();
        }

        // "Sekreter İşlemleri" butonuna tıklandığında:
        private void BtnSekreterIslemleri_Click(object sender, EventArgs e)
        {
            // Sekreter listesini yöneten paneli açar.
            FrmSekreterPaneli fr = new FrmSekreterPaneli();
            fr.Show();
        }

        // "Hasta İşlemleri" butonuna tıklandığında:
        private void BtnHastaIslemleri_Click(object sender, EventArgs e)
        {
            // Hasta listesi ve düzenleme formunu açar.
            FrmHastaPaneli fr = new FrmHastaPaneli();
            fr.Show();
        }
    }
}