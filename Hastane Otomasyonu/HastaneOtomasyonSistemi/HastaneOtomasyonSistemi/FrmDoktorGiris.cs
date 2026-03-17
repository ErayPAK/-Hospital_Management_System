using System;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL komutlarını kullanabilmek için kütüphaneyi ekledik.

namespace HastaneOtomasyonSistemi
{
    public partial class FrmDoktorGiris : Form
    {
        public FrmDoktorGiris()
        {
            InitializeComponent();
        } 

        // ---------------------------------------------------------
        // BAĞLANTI NESNESİ
        // ---------------------------------------------------------
        // Veritabanı ile iletişim kuracak olan sınıfımızı çağırıyoruz.
        SqlBaglantisi bgl = new SqlBaglantisi();

        // ---------------------------------------------------------
        // GİRİŞ YAP BUTONU
        // ---------------------------------------------------------
        private void Button1_Click(object sender, EventArgs e)
        {
            // ADIM 1: KOMUTUN HAZIRLANMASI (STORED PROCEDURE KULLANIMI)
            // Normalde "Select * From..." yazarız ama burada veritabanında kayıtlı
            // 'sp_DoktorGiris' isimli prosedürü çağırıyoruz.
            SqlCommand komut = new SqlCommand("sp_DoktorGiris", bgl.baglanti());

            // ADIM 2: KOMUT TÜRÜNÜ BELİRTME
            // SQL'e diyoruz ki: "Sana gönderdiğim yazı normal bir sorgu değil, bir Prosedür ismidir."
            // Bu satır olmazsa program hata verir.
            komut.CommandType = System.Data.CommandType.StoredProcedure;

            // ADIM 3: PARAMETRELERİ GÖNDERME
            // Prosedür bizden @TC ve @Sifre bekliyor. Bunları kutucuklardan alıp güvenli şekilde gönderiyoruz.
            komut.Parameters.AddWithValue("@TC", MskTC.Text);       // TC Kimlik No
            komut.Parameters.AddWithValue("@Sifre", TxtSifre.Text); // Şifre

            // ADIM 4: SORGULAMA VE DOĞRULAMA
            // Veritabanına soruyoruz: "Böyle bir doktor var mı?"
            SqlDataReader dr = komut.ExecuteReader();

            // if (dr.Read()): Eğer veritabanından olumlu bir sonuç dönerse giriş başarılıdır.
            if (dr.Read())
            {
                // GİRİŞ BAŞARILI İSE:

                // 1. Doktor Detay Formunu hafızada oluştur.
                FrmDoktorDetay fr = new FrmDoktorDetay();

                // 2. VERİ TAŞIMA:
                // Giriş yapan doktorun TC'sini, açılacak olan detay formuna gönderiyoruz.
                // Böylece detay formunda "Hoşgeldin Ahmet Bey" diyebileceğiz ve sadece onun randevularını getirebileceğiz.
                fr.TC = MskTC.Text;

                // 3. Yeni formu göster, giriş formunu gizle.
                fr.Show();
                this.Hide();
            }
            else
            {
                // GİRİŞ BAŞARISIZ İSE:
                // Kayıt bulunamadıysa kullanıcıya hata mesajı ver.
                MessageBox.Show("Hatalı TC Kimlik No veya Şifre", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // ADIM 5: TEMİZLİK
            // İşlem bittikten sonra bağlantıyı kapat.
            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // GERİ DÖN BUTONU
        // ---------------------------------------------------------
        private void BtnGeri_Click(object sender, EventArgs e)
        {
            // Eğer doktor yanlışlıkla bu ekrana girdiyse, ana giriş seçimi ekranına geri döner.
            FrmGiris fr = new FrmGiris();
            fr.Show();     // Ana ekranı aç
            this.Close();  // Bu ekranı tamamen kapat.
        }
    }
}