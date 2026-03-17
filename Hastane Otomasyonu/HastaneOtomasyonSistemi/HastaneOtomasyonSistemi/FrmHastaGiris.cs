using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL komutlarını kullanabilmek için gerekli kütüphane.

namespace HastaneOtomasyonSistemi
{
    public partial class FrmHastaGiris : Form
    {
        public FrmHastaGiris()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // BAĞLANTI NESNESİ
        // ---------------------------------------------------------
        // Veritabanı işlemlerini yürütecek anahtar sınıfımızı çağırıyoruz.
        SqlBaglantisi bgl = new SqlBaglantisi();

        // ---------------------------------------------------------
        // ÜYE OL LİNKİ (YENİ KAYIT)
        // ---------------------------------------------------------
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Eğer hastanın kaydı yoksa, kayıt olma formuna yönlendiriyoruz.
            FrmHastaKayit fr = new FrmHastaKayit();
            fr.Show(); // Kayıt formunu aç.

            // NOT: Burada 'this.Hide()' yapmadık. Çünkü kullanıcı kayıt olurken
            // arkada giriş ekranının açık kalması genelde tercih edilen bir yöntemdir.
        }

        // ---------------------------------------------------------
        // GİRİŞ YAP BUTONU
        // ---------------------------------------------------------
        private void BtnGiris_Click(object sender, EventArgs e)
        {
            // ADIM 1: KOMUTUN HAZIRLANMASI (STORED PROCEDURE)
            // SQL Injection saldırılarını önlemek ve performans için Stored Procedure kullandım.
            // 'sp_HastaGiris' prosedürü, TC ve Şifre eşleşirse bize bir sonuç döndürür.
            SqlCommand komut = new SqlCommand("sp_HastaGiris", bgl.baglanti());
            komut.CommandType = CommandType.StoredProcedure; // Bunun normal bir sorgu değil, prosedür olduğunu belirtiyoruz.

            // ADIM 2: PARAMETRELERİN GÖNDERİLMESİ
            // Kullanıcının girdiği verileri, prosedürdeki değişkenlere (@TC, @Sifre) güvenli bir şekilde atıyoruz.
            komut.Parameters.AddWithValue("@TC", MskTC.Text);
            komut.Parameters.AddWithValue("@Sifre", TxtSifre.Text);

            // ADIM 3: DOĞRULAMA
            // Veritabanına soruyoruz: "Böyle bir kullanıcı var mı?"
            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read()) // Eğer okuma başarılıysa yani kayıt varas
            {
                // GİRİŞ BAŞARILI:

                // 1. Hasta Detay Formunu hafızada oluştur.
                FrmHastaDetay fr = new FrmHastaDetay();

                // 2. VERİ TAŞIMA:
                // Giriş yapan hastanın TC'sini, açılacak olan detay formuna (fr.tc) gönderiyoruz.
                // Böylece detay sayfasında "Hoşgeldin [İsim]" diyebileceğiz ve sadece o hastanın randevularını getirebileceğiz.
                fr.tc = MskTC.Text;

                // 3. Yönlendirme
                fr.Show();    // Detay formunu aç.
                this.Hide();  // Giriş formunu gizle.
            }
            else
            {
                // GİRİŞ BAŞARISIZ:
                // Kayıt bulunamadıysa kullanıcıyı uyar.
                MessageBox.Show("Hatalı TC Kimlik Numarası veya Şifre", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // ADIM 4: TEMİZLİK
            // İşlem bittikten sonra bağlantıyı kapatarak sunucuyu yormuyoruz.
            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // GERİ DÖN BUTONU
        // ---------------------------------------------------------
        private void BtnGeri_Click(object sender, EventArgs e)
        {
            // Eğer kullanıcı yanlışlıkla bu ekrana girdiyse, ana giriş ekranına döner.
            FrmGiris fr = new FrmGiris();
            fr.Show();

            // Bu formu tamamen kapatıyoruz.
            // Çünkü ana ekrana döndükten sonra bu formun arkada açık kalmasına gerek yok. İsterse tekrar açılabilir zatenm.
            this.Close();
        }
    }
}