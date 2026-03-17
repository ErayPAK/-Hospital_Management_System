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
    // Form sınıfı
    public partial class FrmGiris : Form
    {
        public FrmGiris()
        {
            // Bu metod, form üzerindeki buton, resim, yazı gibi görsel öğeleri yükler.
            // Silinirse form boş gelir.
            InitializeComponent();
        }

        // ---------------------------------------------------------
        // DOKTOR GİRİŞİNE YÖNLENDİRME
        // ---------------------------------------------------------
        private void BtnDoktorGiris_Click(object sender, EventArgs e)
        {
            // 1. HEDEF FORMU OLUŞTURMA
            // Gitmek istediğimiz formun bir örneğini hafızada oluşturuyoruz.
            FrmDoktorGiris fr = new FrmDoktorGiris();

            // 2. HEDEF FORMU GÖSTERME
            // Hafızada oluşturduğumuz formu kullanıcının görebileceği şekilde ekrana getiriyoruz.
            fr.Show();

            // 3. MEVCUT FORMU GİZLEME
            this.Hide();
        }

        // ---------------------------------------------------------
        // SEKRETER GİRİŞİNE YÖNLENDİRME
        // ---------------------------------------------------------
        private void BtnSekreterGiris_Click(object sender, EventArgs e)
        {
            // Sekreter giriş ekranına geçiş işlemleri:
            FrmSekreterGiris fr = new FrmSekreterGiris(); // Yeni formu hazırla.
            fr.Show();   // Yeni formu aç.
            this.Hide(); // Bu formu yani ana giriş formunu gizle.
        }

        // ---------------------------------------------------------
        // HASTA GİRİŞİNE YÖNLENDİRME
        // ---------------------------------------------------------
        private void BtnHastaGiris_Click(object sender, EventArgs e)
        {
            // Hasta giriş ekranına geçiş işlemleri:
            FrmHastaGiris fr = new FrmHastaGiris(); // Yeni formu hazırla.
            fr.Show();   // Yeni formu aç.
            this.Hide(); // Bu formu yani ana giriş formunu gizle.
        }

        private void FrmGiris_Load(object sender, EventArgs e)
        {

        }
    }
}