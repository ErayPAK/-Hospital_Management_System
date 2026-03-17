using System;
using System.Data.SqlClient; // Veritabanı işlemleri için gerekli kütüphane.
using System.Windows.Forms;

namespace HastaneOtomasyonSistemi
{
    public partial class FrmDuyurular : Form
    {
        public FrmDuyurular()
        {
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // BAĞLANTI NESNESİ
        // ---------------------------------------------------------
        // Veritabanı ile iletişim kuracak köprümüz.
        SqlBaglantisi bgl = new SqlBaglantisi();

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmDuyurular_Load(object sender, EventArgs e)
        {
            // ADIM 1: TEMİZLİK
            // Form her açıldığında veya yenilendiğinde RichTextBox'ın içini temizliyoruz.
            // Bunu yapmazsak, pencereyi kapatıp açtğımızda eski yazıların üzerine tekrar yazar.
            RchDuyuruListesi.Clear();

            // ADIM 2: VERİLERİ ÇEKME
            // Veritabanındaki 'Tbl_Duyurular' tablosuna gidip tüm duyuru metinlerini istiyoruz.
            SqlCommand komut = new SqlCommand("Select Duyuru From Tbl_Duyurular", bgl.baglanti());
            SqlDataReader dr = komut.ExecuteReader();

            // ADIM 3: LİSTELEME VE FORMATLAMA
            // Veritabanından kaç tane duyuru geleceğini bilmediğimiz için
            // While döngüsü kullanıyoruz. Okuyacak satır bitene kadar döngü döner.

            int sayac = 1; // Duyuruları numaralandırmak için (Duyuru 1, Duyuru 2...) bir sayaç.

            while (dr.Read())
            {
                // ÖNEMLİ DETAY: '+=' OPERATÖRÜ
                // RchDuyuruListesi.Text += ... diyerek, kutudaki mevcut yazının ALTINA ekleme yapıyoruz.

                RchDuyuruListesi.Text += "DUYURU " + sayac + ":\n";    // Başlık (Örn: DUYURU 1:)

                // dr[0]: Sorgumuzdaki 'Duyuru' sütununu temsil eder.
                RchDuyuruListesi.Text += dr[0].ToString() + "\n";      // İçerik

                // Görsel olarak ayırmak için tireler ve boşluklar ekledim.
                RchDuyuruListesi.Text += "------------------------------------------------------\n\n";

                sayac++; // Bir sonraki duyuru numarası için sayacı 1 artır.
            }

            // ADIM 4: BAĞLANTI KAPATMA
            bgl.baglanti().Close();
        }
    }
}