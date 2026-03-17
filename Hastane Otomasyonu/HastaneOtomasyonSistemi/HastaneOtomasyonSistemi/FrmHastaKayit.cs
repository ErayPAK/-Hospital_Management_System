using System.Data.SqlClient; // SQL komutlarını kullanmak için gerekli kütüphane.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HastaneOtomasyonSistemi
{
    public partial class FrmHastaKayit : Form
    {
        // ---------------------------------------------------------
        // BAĞLANTI NESNESİ
        // ---------------------------------------------------------
        // Veritabanı ile iletişim kuracak olan sınıfımızı çağırıyoruz.
        SqlBaglantisi bgl = new SqlBaglantisi();

        public FrmHastaKayit()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // KAYIT YAP BUTONU
        // ---------------------------------------------------------
        private void BtnKayitYap_Click(object sender, EventArgs e)
        {
            // ADIM 1: KOMUTUN HAZIRLANMASI
            // Hastalar tablosuna yeni bir satır eklemek için "INSERT INTO" komutunu kullanıyoruz.
            // Verileri doğrudan string içine + işaretiyle eklemek yerine PARAMETRE (@p1, @p2...) kullanıyoruz.
            // Bu yöntem hem güvenli (SQL Injection önler) hem de hatasız veri girişi sağlar.

            SqlCommand komut = new SqlCommand("insert into Tbl_Hastalar (HastaAd,HastaSoyad,HastaTC,HastaTelefon,HastaSifre,HastaCinsiyet) values (@p1,@p2,@p3,@p4,@p5,@p6)", bgl.baglanti());

            // ADIM 2: PARAMETRELERİ EŞLEŞTİRME
            // Form üzerindeki kutucuklardan (TextBox, MaskedTextBox, ComboBox) verileri alıp
            // SQL sorgusundaki parametrelerin yerine koyuyoruz.

            komut.Parameters.AddWithValue("@p1", TxtAd.Text);          // Hasta Adı
            komut.Parameters.AddWithValue("@p2", TxtSoyad.Text);       // Hasta Soyadı
            komut.Parameters.AddWithValue("@p3", MskTC.Text);          // TC Kimlik No
            komut.Parameters.AddWithValue("@p4", MskTelefon.Text);     // Telefon No
            komut.Parameters.AddWithValue("@p5", TxtSifre.Text);       // Şifre
            komut.Parameters.AddWithValue("@p6", CmbCinsiyet.Text);    // Cinsiyet

            // ADIM 3: KOMUTU ÇALIŞTIRMA
            // ExecuteNonQuery: "Sorguyu çalıştır ama geriye bir tablo döndürme" demektir.
            // Ekleme, Silme ve Güncelleme işlemlerinde bu metod kullanılır.
            komut.ExecuteNonQuery();

            // ADIM 4: BAĞLANTIYI KAPATMA
            // İşimiz bittiği için bağlantıyı kapatıyoruz. Açık kalırsa sistem yavaşlar.
            bgl.baglanti().Close();

            // ADIM 5: KULLANICIYA GERİ BİLDİRİM
            // Kaydın başarılı olduğunu kullanıcıya bildiriyoruz ve şifresini hatırlatıyoruz.
            MessageBox.Show("Kaydınız başarıyla gerçekleşmiştir. Şifreniz: " + TxtSifre.Text, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}