using System;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL komutlarını kullanabilmek için gerekli kütüphane.

namespace HastaneOtomasyonSistemi
{
    public partial class FrmSekreterGiris : Form
    {
        public FrmSekreterGiris()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // BAĞLANTI NESNESİ
        // ---------------------------------------------------------
        // Veritabanı ile iletişim kuracak olan anahtar sınıfımız.
        SqlBaglantisi bgl = new SqlBaglantisi();

        // ---------------------------------------------------------
        // GİRİŞ YAP BUTONU
        // ---------------------------------------------------------
        private void BtnGiris_Click(object sender, EventArgs e)
        {
            // ADIM 1: KOMUTUN HAZIRLANMASI (STORED PROCEDURE)
            // Normal SQL sorgusu yerine, SQL tarafında önceden hazırladığımız 
            // 'sp_SekreterGiris' prosedürünü çağırıyoruz. 
            // Bu yöntem SQL Injection açıklarını kapatır ve daha hızlı çalışır.
            SqlCommand komut = new SqlCommand("sp_SekreterGiris", bgl.baglanti());

            // SQL'e diyoruz ki: "Sana gönderdiğim metin bir sorgu cümlesi değil, bir prosedür adıdır."
            komut.CommandType = System.Data.CommandType.StoredProcedure;

            // ADIM 2: PARAMETRELERİN EŞLEŞTİRİLMESİ
            // Kullanıcının formdan girdiği verileri güvenli bir şekilde SQL'e gönderiyoruz.
            komut.Parameters.AddWithValue("@TC", MskTC.Text);     // TC Kimlik No
            komut.Parameters.AddWithValue("@Sifre", TxtSifre.Text); // Şifre

            // ADIM 3: DOĞRULAMA
            // Veritabanına soruyoruz: "Bu TC ve Şifreye sahip bir sekreter var mı?"
            SqlDataReader dr = komut.ExecuteReader();

            if (dr.Read()) // Eğer okuma başarılıysa
            {
                // GİRİŞ BAŞARILI:

                // 1. Sekreter Detay Formunu hafızada oluştur.
                FrmSekreterDetay fr = new FrmSekreterDetay();

                // 2. VERİ TAŞIMA:
                // Giriş yapan sekreterin TC'sini, açılacak olan detay formuna (fr.TCnumara) aktarıyoruz.
                // Böylece detay sayfasında "Hoşgeldin [İsim]" yazabilecek ve ona özel işlemleri yapabileceğiz.
                fr.TCnumara = MskTC.Text;

                // 3. Yönlendirme
                fr.Show();   // Detay formunu aç.
                this.Hide(); // Giriş formunu gizle.
            }
            else
            {
                // GİRİŞ BAŞARISIZ:
                // Kayıt bulunamadıysa kullanıcıyı uyar.
                MessageBox.Show("Hatalı TC Kimlik Numarası veya Şifre", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // ADIM 4: TEMİZLİK
            // İşlem bittikten sonra açık kalan bağlantıyı kapatıyoruz.
            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // GERİ DÖN BUTONU
        // ---------------------------------------------------------
        private void BtnGeri_Click(object sender, EventArgs e)
        {
            // Eğer kullanıcı yanlış yere girdiyse, ana seçim ekranına geri döner.
            FrmGiris fr = new FrmGiris();
            fr.Show();

            // Bu formu tamamen bellekten kapatıyoruz.
            // Çünkü ana ekrana döndükten sonra bu formun arkada çalışmasına gerek yok.
            this.Close();
        }

        // ---------------------------------------------------------
        // ADMİN PANELİNE GEÇİŞ
        // ---------------------------------------------------------
        private void LnkAdmin_Click(object sender, EventArgs e)
        {
            // Yönetici girişleri sekreter ekranına entegre edilmiştir.
            // Yönetici girişi butonuna tıklayarak FrmAdminGiris formu yüklenir.
            FrmAdminGiris fr = new FrmAdminGiris();
            fr.Show();
            this.Hide(); // Bu formu gizle.(Sekreter giriş formunu)
        }
    }
}