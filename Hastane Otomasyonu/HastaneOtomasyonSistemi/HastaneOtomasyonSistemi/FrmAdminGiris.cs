using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient; // Veritabanı komutlarını kullanabilmek için SQL kütüphanesini ekledik.
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HastaneOtomasyonSistemi
{
    public partial class FrmAdminGiris : Form
    {
        // 1. ADIM: BAĞLANTI NESNESİ
        // Diğer formlarda da kullandığımız, veritabanı adresini tutan sınıftan bir nesne türetiyoruz.
        // Bu nesne bizim veritabanına giden "Köprü"müz olacak.
        SqlBaglantisi bgl = new SqlBaglantisi();

        public FrmAdminGiris()
        {
            InitializeComponent(); // Form üzerindeki görsel öğeleri yükler.
        }

        // "Giriş Yap" butonuna tıklandığında çalşacak olan olay.
        private void BtnGirisYap_Click(object sender, EventArgs e)
        {
            // 2. ADIM: SQL KOMUTUNUN HAZIRLANMASI
            // Veritabanına "Şu kullanıcı adı ve şifreye sahip biri var mı?" diye soruyoruz.
            // bgl.baglanti() metodunu çağırarak bağlantıyı açıyoruz.
            SqlCommand komut = new SqlCommand("Select * From Tbl_Yonetici where KullaniciAd=@p1 and Sifre=@p2", bgl.baglanti());

            // 3. ADIM: GÜVENLİ PARAMETRE ATAMA
            // @p1 ve @p2 parametrelerine Textbox'lardan gelen verileri aktarıyoruz.
            komut.Parameters.AddWithValue("@p1", TxtKullaniciAd.Text); // Kullanıcı Adı kutusundan gelen veri
            komut.Parameters.AddWithValue("@p2", TxtSifre.Text);       // Şifre kutusundan gelen veri

            // 4. ADIM: SORGUYU ÇALIŞTIRMA VE OKUMA
            // ExecuteReader: Veritabanından gelen satırları okumak için bir okuyucu oluşturur.
            SqlDataReader dr = komut.ExecuteReader();

            // 5. ADIM: KONTROL 
            // dr.Read(): Eğer veritabanından geriye uyumlu bir satır dönerse TRUE olur.
            if (dr.Read())
            {
                // GİRİŞ BAŞARILI İSE:

                // 1. Yeni formu yani Admin Panelini oluştur.
                FrmAdminPaneli fr = new FrmAdminPaneli();

                // 2. Yeni formu ekranda göster.
                fr.Show();

                // 3. Şu anki giriş formunu gizle.
                this.Hide();
            }
            else
            {
                // GİRİŞ BAŞARISIZ İSE:
                // Kullanıcıya hata mesajı göster.
                MessageBox.Show("Hatalı Kullanıcı Adı veya Şifre");
            }

            // 6. ADIM: TEMİZLİK
            // İşimiz bittiğinde açk kalan veritabanı bağlantısını kapatıyoruz.
            // Bu, sistem performasını korumak için çok önemlidir.
            bgl.baglanti().Close();
        }
    }
}